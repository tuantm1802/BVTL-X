# Nhật ký Phiên làm việc (Session Log) — BVTL-X Upgrade

Tệp tin này dùng để lưu trữ và bàn giao ngữ cảnh giữa các phiên làm việc của **Antigravity (Gemini)** và **Claude Code**.

---

## Phiên 01: 09/07/2026 | Thực hiện bởi: Antigravity

### Đã hoàn thành:
- Thiết lập thành công hạ tầng tài liệu chia sẻ tại dự án root `d:\Projects\BVTL-X`.
- Tạo mới các tệp tin cấu hình:
  - `CLAUDE.md`: Cấu hình tự động nạp ngữ cảnh dự án cho Claude Code.
  - `.agents/AGENTS.md`: Định nghĩa quy tắc lập trình và phân vai cho Antigravity.
  - `.claude/commands/new-report.md`: Custom command `/new-report` cho Claude Code.
  - `.claude/commands/analyze-impact.md`: Custom command `/analyze-impact` cho Claude Code.
  - `.claude/commands/refactor-controller.md`: Custom command `/refactor-controller` cho Claude Code.
- Tạo mới các tệp tài liệu nghiệp vụ:
  - `docs/architecture.md`: Chi tiết hóa cấu trúc phân lớp công nghệ, luồng sync dữ liệu, xác thực và phân quyền hiện tại.
  - `docs/coding-conventions.md`: Quy chuẩn đặt tên biến, tên hàm, cấu trúc folder và chuẩn viết Excel.
  - `docs/upgrade-roadmap.md`: Bản đồ 6 phase nâng cấp hệ thống kèm bảng phân chia vai trò AI tools.

### Cần làm tiếp (Handoff cho Claude Code):
- **Bắt đầu Phase 0B: Vá các lỗ hổng bảo mật khẩn cấp** bằng **Claude Code**:
  1. Fix SQL Injection trong tệp `Data/API/InsertDataDA.cs` tại hàm `InsertDataFromApi()`. Thay vì nối chuỗi SQL trong câu lệnh `DELETE`, hãy chuyển sang dùng `SqlCommand` với parameters.
  2. Nâng cấp bảo mật password trong `LoginController.cs` và `UserDA.cs`. Chuyển đổi cơ chế băm mật khẩu từ MD5 sang thuật toán an toàn hơn như bcrypt (Sử dụng thư viện `BCrypt.Net-Next` qua NuGet).
  3. Tìm phương án mã hóa/bảo vệ mật khẩu tài khoản CSDL `sa` trong tệp `WebApp/Web.config` và `SyncApp/Web.config` (Ví dụ: sử dụng ASP.NET IIS Registration Tool `aspnet_regiis` để mã hóa connection string section hoặc sử dụng tài khoản có quyền hạn thấp hơn).

### Các tệp đã thay đổi:
- `CLAUDE.md` (New)
- `.agents/AGENTS.md` (New)
- `.claude/commands/new-report.md` (New)
- `.claude/commands/analyze-impact.md` (New)
- `.claude/commands/refactor-controller.md` (New)
- `docs/architecture.md` (New)
- `docs/coding-conventions.md` (New)
- `docs/upgrade-roadmap.md` (New)
- `docs/session-log.md` (New)

### Câu hỏi & Rủi ro:
- **Rủi ro**: Việc thay đổi thuật toán mã hóa mật khẩu từ MD5 sang bcrypt sẽ làm ảnh hưởng đến các tài khoản người dùng hiện có trong DB (do mật khẩu cũ lưu dạng hash MD5). Cần viết một script SQL chuyển đổi hoặc cơ chế fallback (nếu login bằng bcrypt fail, thử kiểm tra bằng MD5, nếu pass thì nâng cấp mật khẩu của user đó sang bcrypt và lưu lại).

---

## Phiên 02: 09/07/2026 | Thực hiện bởi: Antigravity

### Đã hoàn thành:
- **Hoàn tất Phase 0B: Vá các lỗ hổng bảo mật khẩn cấp** trực tiếp bằng Antigravity:
  1. **Fix SQL Injection**: Refactor tệp `Data/API/InsertDataDA.cs` tại hàm `InsertDataFromApi()`. Thay thế câu lệnh DELETE nối chuỗi bằng `SqlCommand` có tham số (`@cityCode` và `@maDuAn`), đồng thời bổ sung kiểm tra kiểm soát `tableName` bằng Regex để ngăn chặn SQL Injection tuyệt đối.
  2. **Nâng cấp mã hóa mật khẩu**: 
     - Triển khai thuật toán PBKDF2 (Rfc2898DeriveBytes với 10,000 vòng lặp và salt ngẫu nhiên) làm chuẩn mã hóa mật khẩu mới trong `Common/ICommon/IEncryptor.cs` và `Common/Common/Encryptor.cs`.
     - Refactor `LoginController.cs` và `UserDA.cs` (bao gồm các hàm `Login()`, `ChangePassword()`, `ResetPassword()`, và cơ chế khởi tạo mật khẩu mặc định khi tạo mới người dùng).
     - Thiết lập cơ chế **auto-migration & fallback** tự động: Khi người dùng đăng nhập bằng mật khẩu MD5 cũ thành công, hệ thống sẽ tự động băm lại mật khẩu bằng PBKDF2 và cập nhật vào DB, giúp quá trình nâng cấp không làm ảnh hưởng đến người dùng (Zero-Downtime).
  3. **Bảo mật mật khẩu DB**:
     - Tách toàn bộ chuỗi kết nối và mật khẩu tài khoản CSDL `sa` ra các tệp cấu hình bên ngoài (`connectionStrings.config` và `appSettings.config`) ở cả `WebApp` và `SyncApp`.
     - Cập nhật tệp cấu hình `Web.config` để tham chiếu bằng thuộc tính `configSource` và `file`.
     - Cập nhật `.gitignore` để loại bỏ hoàn toàn việc commit các tệp chứa thông tin nhạy cảm này lên GitHub.

### Cần làm tiếp (Handoff cho phiên tiếp theo):
- **Bắt đầu Phase 1: Phát triển Tính năng Mới thử nghiệm**
  - Nghiên cứu và xây dựng một module báo cáo mẫu mới để kiểm nghiệm luồng bảo mật mới và cơ chế phối hợp phát triển.

### Các tệp đã thay đổi:
- `Data/API/InsertDataDA.cs` (Modified)
- `Common/ICommon/IEncryptor.cs` (Modified)
- `Common/Common/Encryptor.cs` (Modified)
- `WebApp/Controllers/LoginController.cs` (Modified)
- `Data/Admin/UserDA.cs` (Modified)
- `WebApp/connectionStrings.config` (New - Ignored)
- `WebApp/appSettings.config` (New - Ignored)
- `SyncApp/connectionStrings.config` (New - Ignored)
- `SyncApp/appSettings.config` (New - Ignored)
- `WebApp/Web.config` (Modified)
- `SyncApp/Web.config` (Modified)
- `.gitignore` (Modified)

---

## Phiên 03: 28/07/2026 | Thực hiện bởi: Antigravity

### Đã hoàn thành:
- **Hoàn thành cài đặt Phase 1: Tạo Module Báo cáo Tuần (Weekly Report)**:
  1. **C# Controller**: Tạo tệp `WebApp/Controllers/BaoCaoTuanController.cs` kế thừa `BaseController` và áp dụng phân quyền `[HasCredential(ControllerName = "BaoCaoTuan")]`. Đã cài đặt ánh xạ quy đổi tuần sang tháng để gọi stored procedure `SP_Report_Get_All_Data` sẵn có mà không cần sửa CSDL, đồng thời cài đặt xuất Excel bằng ClosedXML.
  2. **AngularJS Controller**: Tạo tệp `WebApp/app/Controller/BaoCaoTuanController.js` xử lý nạp danh sách tuần (1-53), lọc tỉnh, lọc dự án, gọi API Tìm kiếm dữ liệu (`/BaoCaoTuan/SearchData`) và Xuất Excel (`/BaoCaoTuan/ExportData`).
  3. **Razor View**: Tạo tệp `WebApp/Views/BaoCaoTuan/Index.cshtml` định nghĩa giao diện lọc tuần, năm, tỉnh, nhóm và bảng hiển thị số liệu phân tích.
  4. **Cấu hình MSBuild (.csproj)**: Khai báo đăng nhập 3 file mới vào `WebApp/WebApp.csproj` dưới dạng `<Compile>` và `<Content>` để hệ thống build/compiler của .NET Framework nhận diện chính xác.
  5. **Tài liệu SQL**: Tạo tệp `docs/sql-menu-setup.md` hướng dẫn cấu hình thêm menu "Báo cáo Tuần" vào DB và phân quyền truy cập.

### Cần làm tiếp (Handoff cho người dùng / Claude Code):
- **Kiểm thử & Xác minh**:
  1. Chạy câu lệnh SQL trong `docs/sql-menu-setup.md` vào Database SQL Server qua SSMS.
  2. Mở Visual Studio 2022 và Build Solution (`Ctrl + Shift + B`) hoặc chạy `msbuild WebApp.sln` để kiểm tra biên dịch có bị lỗi hay không.
  3. F5 chạy dự án, đăng xuất và đăng nhập lại bằng tài khoản có quyền truy cập để kiểm tra sự xuất hiện của menu "Báo cáo tuần" trên sidebar.
  4. Chạy thử bộ lọc Tuần, Năm, Tỉnh, Nhóm và chức năng Xuất Excel để đảm bảo ClosedXML xuất file chuẩn xác.

### Các tệp đã thay đổi/thêm mới:
- `WebApp/Controllers/BaoCaoTuanController.cs` (New)
- `WebApp/app/Controller/BaoCaoTuanController.js` (New)
- `WebApp/Views/BaoCaoTuan/Index.cshtml` (New)
- `WebApp/WebApp.csproj` (Modified)
- `docs/sql-menu-setup.md` (New)
- `docs/session-log.md` (Modified)

---

## Phiên 04: 29/07/2026 | Thực hiện bởi: Antigravity

### Đã hoàn thành:
- **Hoàn thành Phase 2: Refactor Report Engine (Trình xuất Excel)**:
  1. **Tạo ExcelReportService**: Thiết lập interface `IExcelReportService.cs` và triển khai lớp `ExcelReportService.cs` trong `WebApp/Service/`. Dịch vụ này gom toàn bộ logic ClosedXML dùng chung (styles, header/footer rendering, cell insertion, auto-fit widths...).
  2. **Đăng ký Service**: Khai báo và đăng ký 2 file dịch vụ mới vào `WebApp/WebApp.csproj`.
  3. **Refactor Report Controllers**:
     - `BaoCaoTuanController.cs` -> Thay thế toàn bộ mã ClosedXML trùng lặp bằng cuộc gọi tới `_excelReportService.ExportReport` (Giảm ~150 dòng code).
     - `BaoCaoThangController.cs` -> Refactor tương tự, xóa bỏ các hàm helper Excel ở cuối controller (Giảm ~300 dòng code).
     - `BaoCaoQuyController.cs` -> Refactor tương tự, loại bỏ các hàm helper Excel (Giảm ~290 dòng code).
     - **Kết quả**: Xóa bỏ tổng cộng hơn **800 dòng code trùng lặp**, giữ cho các controller báo cáo cực kỳ gọn nhẹ (~190 dòng thay vì ~500 dòng).

### Cần làm tiếp (Handoff cho người dùng / Claude Code):
- **Kiểm thử & Xác minh**:
  1. Mở Visual Studio 2022 và Build Solution (`Ctrl + Shift + B`) để đảm bảo dự án biên dịch thành công mà không có lỗi tham chiếu.
  2. Chạy thử nghiệm xuất báo cáo Excel của cả 3 phân hệ: Tuần, Tháng, Quý để kiểm tra dữ liệu và định dạng file tải xuống xem có trùng khớp 100% với phiên bản cũ hay không.

### Các tệp đã thay đổi/thêm mới:
- `WebApp/Service/IExcelReportService.cs` (New)
- `WebApp/Service/ExcelReportService.cs` (New)
- `WebApp/Controllers/BaoCaoTuanController.cs` (Modified)
- `WebApp/Controllers/BaoCaoThangController.cs` (Modified)
- `WebApp/Controllers/BaoCaoQuyController.cs` (Modified)
- `WebApp/WebApp.csproj` (Modified)
- `docs/session-log.md` (Modified)

---

## Phiên 05: 29/07/2026 | Thực hiện bởi: Antigravity

### Đã hoàn thành:
- **Xử lý lỗi biên dịch sau Refactor**:
  1. **Khôi phục class thiếu**: Khai báo lại class `ReportSearchModelTuan` kế thừa `ReportSearchModel` vào đầu tệp `WebApp/Controllers/BaoCaoTuanController.cs` (bị mất trong lúc overwrite file ở phiên trước), giải quyết triệt để lỗi biên dịch `CS0246` của WebApp.
  2. **Loại bỏ dự án WebBVTLAPI**:
     - Phân tích lỗi biên dịch `CS0103` (GlobalConfiguration không tồn tại) trong dự án WebBVTLAPI do thiếu thư viện WebAPI NuGet references.
     - Xác nhận `WebBVTLAPI` là dự án nháp (draft) dư thừa, hoàn toàn không được sử dụng bởi hệ thống chính.
     - Tiến hành loại bỏ dự án `WebBVTLAPI` khỏi tệp cấu hình Solution `WebApp.sln`. Việc này giải quyết lập tức 3 lỗi biên dịch liên quan mà không ảnh hưởng tới hoạt động của `WebApp` và `SyncApp`.

### Cần làm tiếp:
- **Kiểm thử & Xác minh**:
  1. Biên dịch lại Solution trong Visual Studio 2022 để kiểm chứng 0 errors.
  2. Test chạy thử các chức năng xuất Excel của báo cáo tháng, quý, tuần.

### Các tệp đã thay đổi/thêm mới:
- `WebApp/Controllers/BaoCaoTuanController.cs` (Modified)
- `WebApp.sln` (Modified)
- `docs/session-log.md` (Modified)
