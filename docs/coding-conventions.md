# Quy ước Lập trình & Chuẩn hóa Code (BVTL-X)

Tài liệu này định nghĩa các quy chuẩn lập trình bắt buộc áp dụng khi nâng cấp dự án BVTL-X. Cả Antigravity và Claude Code đều phải tuân thủ nghiêm ngặt các quy tắc này.

---

## 1. Quy tắc Đặt tên (Naming Conventions)

### 1.1. C# Backend
- **Controller**: Đặt tên dạng `{TênModule}Controller.cs` (Ví dụ: `BaoCaoThangController.cs`). Tất cả các controller trong `WebApp` bắt buộc phải kế thừa `BaseController`.
- **Interface DA**: Đặt tên dạng `I{TênThựcThể}DA.cs` nằm trong thư mục `Data/InterfaceDA/Admin/` hoặc `Data/InterfaceDA/API/`.
- **Implementation DA**: Đặt tên dạng `{TênThựcThể}DA.cs` nằm trong thư mục `Data/Admin/`.
- **Model mở rộng**: Đặt tên dạng `{TênThựcThể}Model.cs` hoặc đặt trong thư mục `Model/ModelExtend/`.
- **Method & Property**: Sử dụng **PascalCase** (Ví dụ: `GetDataReport()`, `CityCode`).
- **Variable**: Sử dụng **camelCase** (Ví dụ: `userId`, `reportId`).

### 1.2. Frontend & Giao diện
- **View Razor**: Đặt tên trong thư mục `Views/{TênController}/` dạng `Index.cshtml`.
- **AngularJS Controller**: Đặt tên file dạng `{name}Controller.js` trong thư mục `WebApp/app/Controller/`.
- **AngularJS Service**: Đặt tên file dạng `{name}Service.js` trong thư mục `WebApp/app/Service/`.

---

## 2. Quy chuẩn về Cấu trúc Code

### 2.1. Cấu trúc Báo cáo (Excel Export)
- **Không tự định nghĩa lại style Excel tại controller**: Các định dạng font chữ, border, căn lề phải được tách biệt.
- **Không sử dụng chỉ số ô cứng (hardcoded cells)**: Hạn chế viết trực tiếp `ws.Cell("B42")` cho các dữ liệu động. Sử dụng biến đếm dòng (`row`) và cột tương đối để tránh lỗi khi cấu trúc báo cáo thay đổi.
- **Giải phóng bộ nhớ**: Luôn bọc đối tượng `XLWorkbook` trong block `using` để giải phóng tài nguyên hệ thống.

### 2.2. Xử lý Dữ liệu & CSDL
- **Phân quyền Route**: Luôn gắn thuộc tính `[HasCredential(ControllerName = "xxx")]` lên đầu Controller Class hoặc Action Method để kích hoạt kiểm tra quyền.
- **Sử dụng Parameter**: Tuyệt đối **không** nối chuỗi SQL. Bắt buộc dùng `SqlParameter` hoặc Entity Framework LINQ để tránh SQL Injection.
- **Không khởi tạo trực tiếp**: Trong giai đoạn chưa tích hợp Dependency Injection Container (Autofac/Unity), tiếp tục dùng khai báo Interface (Ví dụ: `ICityDA _cityDA = new CityDA();`) để chuẩn bị sẵn cho việc refactor ở Phase 5.

---

## 3. Quy chuẩn UI/UX Giao diện
- **Sử dụng CSS Class**: Không sử dụng inline style (`style="..."`) trực tiếp trên thẻ HTML trong Razor View. Đưa toàn bộ style vào các file CSS chung.
- **Giao diện Responsive**: Sử dụng hệ thống Grid System của Bootstrap 4 (hoặc Bootstrap 5 sau khi nâng cấp) để đảm bảo giao diện hiển thị tốt trên mọi kích thước màn hình.
- **JS Framework**: Hạn chế viết thêm mã AngularJS 1.x mới. Ưu tiên viết Vanilla JS (Javascript thuần) hoặc sử dụng các thư viện gọn nhẹ nếu cần làm việc với giao diện động, chuẩn bị cho việc loại bỏ hoàn toàn AngularJS 1.x.

---

## 4. Ngôn ngữ & Quy chuẩn Viết Code
- **Mã nguồn (Code)**: Tên biến, tên hàm, class, database table, comment trong code bắt buộc sử dụng **tiếng Anh**.
- **Tài liệu & Kế hoạch (Docs & Logs)**: Các tệp markdown hỗ trợ AI, kế hoạch triển khai, mô tả lỗi viết bằng **tiếng Việt**.
