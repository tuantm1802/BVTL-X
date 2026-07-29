# Tài liệu Kiến trúc Hệ thống BVTL-X

Tài liệu này mô tả chi tiết kiến trúc hiện tại của dự án BVTL-X dựa trên kết quả nghiên cứu mã nguồn thực tế.

---

## 1. Tổng quan Công nghệ
- **Nền tảng**: .NET Framework 4.7.2
- **Mô hình**: ASP.NET MVC 5 (v5.2.7) cho cả `WebApp` và `SyncApp`
- **Công cụ truy cập dữ liệu (ORM)**: Entity Framework 6 (Database-first với tệp EDMX `BVTL_REPORTING.edmx`)
- **Cơ sở dữ liệu**: SQL Server (kết nối trực tiếp qua `System.Data.SqlClient`)
- **Thư viện log**: log4net (ghi log ra tệp luân phiên kích thước tối đa 100MB tại `C:\log\`)
- **Công cụ lập lịch**: Quartz.NET 3.0.7 (chạy trong `SyncApp`)
- **Báo cáo**: ClosedXML (cho xuất Excel trực tiếp) và Microsoft ReportViewer 15.0 (cho báo cáo dạng RDLC)

---

## 2. Bản đồ Dự án & Phân lớp (WebApp.sln)

### 2.1. WebApp (Dự án giao diện & nghiệp vụ chính)
Chứa toàn bộ logic hiển thị, phân quyền và các bộ điều khiển nghiệp vụ y tế.
- `Controllers/`: Chứa các controller MVC xử lý nghiệp vụ, quản trị và báo cáo.
- `Views/`: Chứa các tệp giao diện Razor (.cshtml). Có 41 thư mục view tương ứng với các phân hệ khác nhau.
- `Report/ReportFile/`: Chứa 13 tệp thiết kế báo cáo `.rdlc` (ReportViewer).
- `ReportDataSet/`: Chứa các tệp `.xsd` định nghĩa dataset cho báo cáo.
- `Service/`: Chứa các service xử lý Token JWT, phân quyền...

### 2.2. SyncApp (SyncBVTL.Push - Dự án đồng bộ nền)
Chạy độc lập với WebApp, chịu trách nhiệm kết nối API nguồn để đồng bộ dữ liệu về CSDL local.
- `Jobs/`: Định nghĩa các tác vụ đồng bộ chạy ngầm sử dụng Quartz.NET (e.g., `GetDataAPIJob.cs`).
- `ScheduleTasks/`: Cấu hình khởi động Quartz.NET scheduler và nạp cấu hình API lặp lại.
- `Services/`: Chứa `ProcessService.cs` để đọc cấu hình API và thực thi luồng đồng bộ.

### 2.3. Data (Lớp truy cập dữ liệu)
Được thiết kế tách biệt giao diện (`InterfaceDA`) và thực thi cụ thể (`Admin` và `API`).
- `InterfaceDA/Admin/`: Khai báo 30 interfaces định nghĩa các phương thức CRUD của các thực thể nghiệp vụ.
- `Data/Admin/`: Triển khai cụ thể các interfaces trên bằng cách sử dụng Entity Framework hoặc ADO.NET (e.g., `BaoCaoTongHopDA.cs`).
- `Data/API/`: Triển khai cụ thể các nghiệp vụ đồng bộ dữ liệu nâng cao (e.g., `SyncDataFromApi_SaveToDB.cs`).

### 2.4. Model (Lớp dữ liệu thực thể)
- `Model/Model/`: Chứa các class được sinh tự động bởi Entity Framework từ CSDL.
- `Model/ModelExtend/`: Chứa các class mở rộng (partial class) phục vụ gom nhóm dữ liệu hoặc truyền tham số.

### 2.5. Common & Simple
- `Common/`: Chứa các cấu trúc lọc dữ liệu chung, filter phân quyền (custom attribute), và các hàm xử lý chuỗi/mã hóa.
- `Simple/`: Thư viện cơ sở hỗ trợ các lớp dùng chung.

---

## 3. Luồng Dữ liệu Đồng bộ (Data Sync Flow)

```
[Hệ thống ngoài (API)] 
       ▲
       │ (HTTP GET/POST JSON)
       ▼
[SyncApp - GetDataAPIJob] ──(Gọi)──► [ProcessService / SyncDataFromApi_SaveToDB]
                                                    │
                                                    ▼
                                          [Entity Data Mapping]
                                                    │
                                                    ▼ (SqlBulkCopy)
                                          [CSDL SQL Server local]
```

1. **Khởi chạy**: `JobScheduler` đọc danh sách cấu hình từ bảng `BVTL_API`. Mỗi API cấu hình thời gian lặp (ví dụ 60 giây).
2. **Kích hoạt job**: Quartz.NET kích hoạt `GetDataAPIJob` định kỳ.
3. **Gọi API**: Dùng `HttpClient` gửi yêu cầu nhận Token, sau đó kéo dữ liệu JSON từ API nguồn.
4. **Ánh xạ dữ liệu**: Chuyển đổi dữ liệu JSON sang định dạng database model thông qua tệp ánh xạ thủ công `ConvertResultApiToEntity.cs` (dung lượng 140KB).
5. **Ghi vào DB**:
   - Xóa dữ liệu cũ theo điều kiện: `DELETE FROM [table] WHERE CITY_CODE = '...' AND MADUAN = '...'`
   - Sử dụng `SqlBulkCopy` để chèn nhanh hàng loạt dữ liệu mới vào DB.

---

## 4. Cơ chế Xác thực & Phân quyền (Authentication & Authorization)

1. **Đăng nhập**: Người dùng nhập thông tin → `LoginController.cs` kiểm tra mật khẩu đã mã hóa MD5 thông qua lớp truy cập dữ liệu → Ghi đối tượng `UserLogin` vào `Session["USER_SESSION"]`.
2. **Phân quyền Route (Menu-based)**:
   - Các controller nghiệp vụ được đánh dấu bằng custom attribute `[HasCredential(ControllerName = "xxx")]`.
   - Bộ lọc kiểm tra xem controller tương ứng có nằm trong danh sách `Session["Menus"]` của người dùng hay không.
3. **Token Cookie**: `BaseController.cs` kiểm tra session mỗi khi có request. Nếu session hết hạn, nó sẽ cố gắng giải mã cookie `UserToken` chứa chuỗi JWT để tái tạo session (thời hạn token là 30 phút).
