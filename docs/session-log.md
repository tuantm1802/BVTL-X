﻿﻿# Nhật ký Phiên làm việc (Session Log) — BVTL-X Upgrade

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

---

## Phiên 06: 29/07/2026 | Thực hiện bởi: Antigravity

### Đã hoàn thành:
- **Khôi phục các phân hệ bị thiếu (CH07, CD43, Kho Vật Phẩm, Hồ sơ khách hàng)**:
  1. **Phân tích lỗi 404**: Lỗi 404 xảy ra khi truy cập `/BaoCaoThangCH07/Index` do mã nguồn hiện tại đang chạy trên nhánh `main` vốn chưa được tích hợp các tính năng nghiệp vụ nâng cao được phát triển riêng trên nhánh của người dùng (`TUANTM_BVTL_V1.0`).
  2. **Chuyển nhánh công việc**: Thực hiện checkout và tạo nhánh local `TUANTM_BVTL_V1.0` tracking nhánh remote `origin/TUANTM_BVTL_V1.0` để phục hồi đầy đủ các tệp tin controller/view của phân hệ CH07, CD43, Kho Vật Phẩm...
  3. **Gộp nhánh (Merge)**:
     - Tiến hành gộp nhánh `main` vào `TUANTM_BVTL_V1.0` để tích hợp toàn bộ các vá lỗi bảo mật (PBKDF2, SQL Injection, bảo mật Connection String) và module Báo cáo tuần kèm theo lõi xuất Excel `ExcelReportService` dùng chung.
     - Giải quyết các xung đột (merge conflicts) tại:
       - `WebApp/Controllers/BaoCaoThangController.cs` (Giữ phiên bản tối ưu refactor gọi Service từ `main`).
       - `WebApp/Web.config` & `SyncApp/Web.config` (Giữ cấu hình bảo mật tách thông tin credentials từ `main`).

### Cần làm tiếp:
- **Kiểm thử & Xác minh**:
  1. Thực hiện Rebuild Solution để đảm bảo tất cả các phân hệ hoạt động bình thường trên nhánh gộp mới.
  2. Chạy thử nghiệm các phân hệ CH07 (báo cáo tháng, quý, năm) để đảm bảo không còn lỗi 404.

### Các tệp đã thay đổi/thêm mới:
- `SyncApp/Web.config` (Merged & Conflict Resolved)
- `WebApp/Controllers/BaoCaoThangController.cs` (Merged & Conflict Resolved)
- `WebApp/Web.config` (Merged & Conflict Resolved)
- `docs/session-log.md` (Modified)

---

## Phiên 07: 29/07/2026 | Thực hiện bởi: Antigravity

### Đã hoàn thành:
- **Hoàn thành Phase 3: Refactor UI/UX & Trực quan hóa Dữ liệu (Charts)**:
  1. **Nâng cấp Hệ thống Style CSS**: Tạo mới `WebApp/Content/custom-style.css` định nghĩa các biến màu chuẩn Sleek Emerald & Dark Gray, thay thế nền xanh cũ của sidebar bằng màu xám tối và xanh ngọc bọc tinh tế, bo góc các Card & Table ở mức `radius-md: 8px` và `radius-lg: 12px`, áp dụng bóng mờ hiện đại và hiệu ứng hover hàng trên bảng dữ liệu.
  2. **Liên kết CSS & Đăng ký dự án**: Nhúng tệp CSS mới vào `WebApp/Views/Shared/_Layout.cshtml` và đăng ký nó trong `WebApp/WebApp.csproj`.
  3. **Tích hợp Biểu đồ vào Báo cáo Tuần**:
     - `BaoCaoTuan/Index.cshtml`: Thiết kế giao diện chuyển đổi giữa "Bảng số liệu" và "Biểu đồ trực quan" thông qua tab controls và thêm thẻ `<canvas id="weeklyReportChart">`.
     - `BaoCaoTuanController.js`: Nhúng Chart.js và lập trình hàm `$scope.renderChart()` để vẽ biểu đồ cột dạng nhóm (grouped bar chart) thể hiện số lượng MSM, PUD và SW của top 6 chỉ số báo cáo tuần quan trọng nhất.
  4. **Tích hợp Biểu đồ vào Báo cáo Tháng**:
     - `BaoCaoThang/Index.cshtml`: Tương tự như Báo cáo tuần, thêm tab controls chuyển đổi và thẻ `<canvas id="monthlyReportChart">`.
     - `BaoCaoThangController.js`: Nhúng Chart.js và thêm logic `$scope.renderChart()` vẽ biểu đồ trực quan số liệu tháng.

### Cần làm tiếp (Handoff cho người dùng):
- **Kiểm thử & Xác minh**:
  1. Rebuild Solution và chạy thử ứng dụng.
  2. Vào trang Báo cáo Tuần hoặc Báo cáo Tháng, thực hiện lọc tìm kiếm và nhấn tab "Biểu đồ trực quan" để xem biểu đồ Chart.js nạp dữ liệu.

### Các tệp đã thay đổi/thêm mới:
- `WebApp/Content/custom-style.css` (New)
- `WebApp/Views/Shared/_Layout.cshtml` (Modified)
- `WebApp/Views/BaoCaoTuan/Index.cshtml` (Modified)
- `WebApp/app/Controller/BaoCaoTuanController.js` (Modified)
- `WebApp/Views/BaoCaoThang/Index.cshtml` (Modified)
- `WebApp/app/Controller/BaoCaoThangController.js` (Modified)
- `WebApp/WebApp.csproj` (Modified)
- `docs/session-log.md` (Modified)

---

## Phiên 08: 29/07/2026 | Thực hiện bởi: Antigravity

### Đã hoàn thành:
- **Hoàn thành Phase 4: Refactor Module Đồng bộ Dữ liệu (SyncApp)**:
  1. **Khắc phục Socket Exhaustion trong `ApiBase.cs`**:
     - Khởi tạo đối tượng static `_httpClient` dùng chung duy nhất cho toàn ứng dụng với `HttpClientHandler` tự động giải nén `GZip/Deflate` và mở rộng kết nối `MaxConnectionsPerServer = 100`.
     - Loại bỏ toàn bộ các khối `using (var client = new HttpClient())` trùng lặp ở tất cả các phương thức gọi API.
  2. **Tự phục hồi kết nối (HttpRetryHelper - Self-contained Retry Policy)**:
     - Tạo mới tệp `Common/Common/HttpRetryHelper.cs` triển khai chính sách Retry với lùi thời gian lũy thừa (Exponential Backoff: thử lại 3 lần sau 1s, 2s, 4s) khi gặp lỗi mạng tạm thời (Timeout, 5xx, 408/429).
     - Nhúng `HttpRetryHelper.ExecuteWithRetryAsync` vào hàm `PostJsonAsyncRaw` trong `ApiBase.cs`.
     - Đăng ký `HttpRetryHelper.cs` vào `Common/Common.csproj`.
  3. **Xóa bỏ các lệnh nghẽn luồng (Blocking Calls)**:
     - Refactor `Data/API/GetDataFromAPI.cs` thay thế tất cả các lệnh `.ReadAsStringAsync().Result` nghẽn luồng bằng `await response.Content.ReadAsStringAsync()`.

### Cần làm tiếp (Handoff cho người dùng):
- **Kiểm thử & Xác minh**:
  1. Mở Visual Studio 2022 và Rebuild Solution.
  2. Chạy ứng dụng `SyncApp` và xác nhận tiến trình đồng bộ dữ liệu `GetDataAPIJob` hoạt động ổn định.

### Các tệp đã thay đổi/thêm mới:
- `Common/Common/HttpRetryHelper.cs` (New)
- `Common/Common.csproj` (Modified)
- `Common/Common/ApiBase.cs` (Modified)
- `Data/API/GetDataFromAPI.cs` (Modified)
- `docs/session-log.md` (Modified)

---

## Phiên 09: 29/07/2026 | Thực hiện bởi: Antigravity

### Đã hoàn thành:
- **Hoàn thành Phase 5: Cải tiến Kiến trúc & Unit Tests (Hoàn tất Lộ trình Nâng cấp)**:
  1. **Khởi tạo Dự án Kiểm thử `BVTL.Tests`**:
     - Tạo tệp `BVTL.Tests/BVTL.Tests.csproj` cấu hình MSTest cho .NET Framework 4.7.2.
     - Đăng ký dự án `BVTL.Tests` vào tệp Solution `WebApp.sln`.
  2. **Viết các Bộ Unit Test Tự động**:
     - `EncryptorTests.cs`: Kiểm thử băm mật khẩu PBKDF2 (10,000 vòng) và cơ chế tự động chuyển đổi Fallback từ chuỗi băm MD5 cũ.
     - `BaoCaoTuanLogicTests.cs`: Kiểm thử thuật toán quy đổi tuần (1-53) sang tháng (1-12).
     - `ExcelReportServiceTests.cs`: Kiểm thử dịch vụ sinh file Excel ClosedXML dạng mảng byte.
     - `HttpRetryHelperTests.cs`: Kiểm thử chính sách lùi thời gian lũy thừa (Exponential Backoff) khi gọi API bị lỗi ngắt mạng tạm thời.

### Cần làm tiếp (Handoff cho người dùng):
- **Xác minh Chạy Unit Tests**:
  1. Mở Visual Studio 2022 và Rebuild Solution.
  2. Mở cửa sổ **Test Explorer** (`Test` -> `Test Explorer` hoặc phím tắt `Ctrl + R, A`).
  3. Nhấn **Run All Tests** và xác minh 100% các Unit Test đều vượt qua thành công (Passed xanh).

### Các tệp đã thay đổi/thêm mới:
- `BVTL.Tests/BVTL.Tests.csproj` (New)
- `BVTL.Tests/EncryptorTests.cs` (New)
- `BVTL.Tests/BaoCaoTuanLogicTests.cs` (New)
- `BVTL.Tests/ExcelReportServiceTests.cs` (New)
- `BVTL.Tests/HttpRetryHelperTests.cs` (New)
- `WebApp.sln` (Modified)
- `docs/session-log.md` (Modified)

---

## Phiên 10: 09/09/2026 | Thực hiện bởi: Antigravity

### Đã hoàn thành:
- **Nâng cấp Toàn diện Dashboard Trang chủ "Home" sang Dự án CD45 (DREAMH)**:
  1. **Nghiên cứu CSDL & Thiết kế Stored Procedure**:
     - Phân tích 10 bảng dữ liệu thực tế của Dự án CD45 (`CD45_KH`, `CD45_HOAT_DONG`, `CD45_QST`, `CD45_CHAN_DOAN`, `CD45_TU_VAN_L1`, `CD45_TU_VAN_L2`, `CD45_HO_TRO_XH`, `CD45_TUAN_THU`, `CD45_VAN_TAY`) với 3.379 khách hàng, 3.665 lượt tiếp cận, 2.450 lượt sàng lọc QST.
     - Xây dựng và triển khai Stored Procedure `SP_CD45_Dashboard` tối ưu, xử lý 7 tập kết quả đa chiều (KPIs, QST theo nhóm đích, QST theo nhóm tuổi, lâm sàng PTSD/PCL-5 & AUDIT-C & Kỳ thị, phân bố tỉnh thành, Phễu can thiệp Cascade Funnel, Dịch vụ chuyển gửi xã hội) chỉ trong ~40ms.
     - Cập nhật lưu trữ script vào `SQL_CD45_SP.sql`.
  2. **Xây dựng Data Transfer Models (DTO)**:
     - Tạo mới `Model/ModelExtend/DashboardCD45Model.cs` định nghĩa trọn bộ DTO models phục vụ dashboard.
     - Đăng ký vào `Model/Model.csproj`.
  3. **Xây dựng Tầng Truy xuất Dữ liệu (Data Access Layer)**:
     - Tạo mới interface `Data/InterfaceDA/Admin/IDashboardCD45DA.cs` và lớp triển khai `Data/Admin/DashboardCD45DA.cs`.
     - Sử dụng `SqlCommand` và `SqlDataAdapter` có tham số hóa chuẩn mực (`@CityCode`, `@MaNhom`, `@FromDate`, `@ToDate`) ngăn chặn SQL Injection tuyệt đối.
     - Đăng ký vào `Data/Data.csproj` và Autofac DI container trong `WebApp/App_Start/AutofacConfig.cs`.
  4. **Nâng cấp Controller & Client Frontend**:
     - `HomeController.cs`: Inject `IDashboardCD45DA`, `ICityDA`, `IBVTL_NHOM_TBHDA`. Bổ sung API `GetFilterData()` và `GetDashboardCD45Data()`.
     - `AlpineHomeController.js`: Quản lý reactive state các bộ lọc (Tỉnh, Nhóm CBO, Từ ngày, Đến ngày) và vẽ 5 biểu đồ Chart.js (Phễu chăm sóc Cascade, Cột phân bố 4 mức QST theo nhóm đích, Cột chồng QST theo nhóm tuổi, Donut chart phân bố 6 tỉnh thành, Cột chồng PTSD PCL-5).
     - `Views/Home/Index.cshtml`: Thiết kế lại toàn diện giao diện trang chủ với phong cách hiện đại: 4 Thẻ KPI nổi bật, thanh công cụ bộ lọc tương tác, tab chuyển đổi giữa "Biểu đồ trực quan" và "Bảng số liệu chi tiết".
  5. **Khắc phục Lỗi Hiển thị Tiếng Việt (Unicode/Mojibake)**:
     - Biên dịch lại `SP_CD45_Dashboard` trên SQL Server với luồng UTF-8 chuẩn xác.
     - Bổ sung cơ chế chuẩn hóa dữ liệu phòng vệ (defensive normalization) trong `DashboardCD45DA.cs` (`GetTenDoiTuong`, `GetTenDoiTuongNgan`, `GetTenTinh`, `GetTenNhomTuoi`).
     - Chuẩn hóa toàn bộ các tệp `Index.cshtml` và `AlpineHomeController.js` sang UTF-8 BOM.
  6. **Cập nhật Page Footer Bản quyền**:
     - Cập nhật `Views/Shared/_Layout.cshtml`: đổi `Copyright &copy; 2024 SCDI` thành `Copyright &copy; @DateTime.Now.Year SCDI`.
  7. **Khắc phục Lỗi Bộ lọc Nhóm Tiếp cận (CBO) Không Hiển thị Dữ liệu**:
     - **Nguyên nhân**: Bảng `CD45_KH` lưu mã nhóm ngắn `MA_NHOM` (`tt`, `bm`, `alo`...) tương ứng với `manhom_tbh_map`, trong khi bảng quản trị `BVTL_NHOM_TBH` và dropdown trên giao diện gửi mã chuẩn `manhom_tbh` (`HN_TT`, `HP_BM`, `HC_ALO`...). Khi Stored Procedure `SP_CD45_Dashboard` so sánh trực tiếp `kh.MA_NHOM = @MaNhom` đã không khớp dẫn đến kết quả trả về 0 dòng.
     - **Khắc phục**:
       - Cập nhật `SP_CD45_Dashboard` tự động phân giải hai chiều giữa `manhom_tbh` và `manhom_tbh_map` qua bảng `BVTL_NHOM_TBH`, đồng thời khớp cả `kh.MA_NHOM` lẫn `kh.REDCAP_DAG`.
       - Cập nhật `HomeController.cs` trả về danh sách nhóm sắp xếp theo tỉnh thành và tên nhóm.
       - Tối ưu `AlpineHomeController.js` và `Index.cshtml` xử lý an toàn sự kiện chọn nhóm và tỉnh thành.
  8. **Kiểm thử Tự động & Xác minh**:
     - Bổ sung 2 Unit Tests chuyên biệt trong `BVTL.Tests/DashboardCD45Tests.cs` kiểm tra lọc theo nhóm (`HN_TT` - The Times: 385 KH) và kết hợp tỉnh/nhóm (`HPG` + `HP_HD` - Hải Đăng: 248 KH).
     - Toàn bộ Solution biên dịch thành công 0 errors với MSBuild VS 2022 Professional.
     - Toàn bộ 23/23 Unit Tests đều Passed 100%.

### Các tệp đã thay đổi/thêm mới:
- `SQL_CD45_SP.sql` (Modified)
- `Model/ModelExtend/DashboardCD45Model.cs` (New)
- `Model/Model.csproj` (Modified)
- `Data/InterfaceDA/Admin/IDashboardCD45DA.cs` (New)
- `Data/Admin/DashboardCD45DA.cs` (New)
- `Data/Data.csproj` (Modified)
- `WebApp/App_Start/AutofacConfig.cs` (Modified)
- `WebApp/Controllers/HomeController.cs` (Modified)
- `WebApp/app/Controller/AlpineHomeController.js` (Modified)
- `WebApp/Views/Home/Index.cshtml` (Modified)
- `WebApp/Views/Shared/_Layout.cshtml` (Modified)
- `BVTL.Tests/DashboardCD45Tests.cs` (New)
- `BVTL.Tests/BVTL.Tests.csproj` (Modified)
- `docs/session-log.md` (Modified)

---

## Phiên làm việc 11 (10/09/2026): Tối ưu hóa & Sửa lỗi Báo cáo Tiếp cận viên (TCV) Dự án CD45

### Mục tiêu:
Khắc phục triệt để các lỗi phát sinh trong phân hệ Báo cáo Tiếp cận viên (`BaoCaoTCVCD45`), cải thiện trải nghiệm người dùng với Select2 tìm kiếm TCV, tối ưu hóa bộ lọc nhóm đa chiều và cơ chế xuất báo cáo Excel.

### Các công việc đã hoàn thành:
1. **Khắc phục Lỗi Bộ lọc và Ánh xạ TCV theo Tỉnh & Nhóm**:
   - **Vấn đề**: Khi chọn Nhóm CBO, danh sách TCV không khớp do dữ liệu nhóm lưu mã chuẩn (`HN_TT`), mã ánh xạ (`tt`), hoặc có khoảng trắng thừa cuối chuỗi.
   - **Giải pháp**:
     - Cập nhật `BaoCaoCD45DA.cs`: Sử dụng `RTRIM(MA_NHOM)`, `RTRIM(CITY_CODE)`, `RTRIM(MA_TCV)` trong truy vấn SQL `GetListTCV`.
     - Cập nhật `BaoCaoTCVCD45Controller.cs`: Chuẩn hóa `Trim()` các thuộc tính nhóm (`manhom_tbh`, `tennhom_tbh`, `city_code`, `manhom_tbh_map`).
     - Cập nhật `AlpineBaoCaoTCVCD45Controller.js`: Lọc danh sách TCV theo cả `manhom_tbh` và `manhom_tbh_map` tương ứng.
2. **Tích hợp Thư viện Select2 cho Dropdown Chọn TCV**:
   - Cung cấp ô tìm kiếm nhanh TCV hỗ trợ gõ tiếng Việt có dấu và không dấu (`removeVietnameseTones`).
   - Tự động đồng bộ giữa trạng thái Alpine.js và Select2 khi thay đổi Tỉnh thành hoặc Nhóm CBO.
   - Cải tiến giao diện hiển thị thông tin TCV: `[Mã TCV] Tên TCV - Tên Nhóm (Tỉnh)`.
3. **Cải tiến Cơ chế Xuất Báo cáo Excel Đơn lẻ (`ExportSingleExcel`)**:
   - Chuyển phương thức từ gọi GET thông thường qua URL sang submit form POST động, tránh rủi ro vượt quá độ dài URL hoặc lỗi mã hóa ký tự tên TCV/Nhóm.
   - Cập nhật controller `[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]` để hỗ trợ linh hoạt cả hai phương thức.
4. **Kiểm thử Tự động & Xác minh**:
   - Biên dịch toàn bộ Solution `WebApp.sln` bằng MSBuild VS 2022 Professional ở chế độ Release: 0 lỗi.
   - Chạy toàn bộ 23 Unit Tests qua `vstest.console.exe`: 23/23 tests Passed 100%.
5. **Publish & Deploy**:
   - Đóng gói Release qua `FolderProfile1.pubxml` vào `D:\Deploy\WebApp_Publish`.
   - Deploy tự động lên máy chủ Host IIS (`103.77.167.206:8090`) bằng `deploy-ftp.ps1 -Mode Patch` trong 16.4 giây.
   - Xác minh Healthcheck: phản hồi `HTTP 200 OK`.

### Các tệp đã thay đổi:
- `Data/Admin/BaoCaoCD45DA.cs` (Modified)
- `WebApp/Controllers/BaoCaoTCVCD45Controller.cs` (Modified)
- `WebApp/Views/BaoCaoTCVCD45/Index.cshtml` (Modified)
- `WebApp/app/Controller/AlpineBaoCaoTCVCD45Controller.js` (Modified)
- `SQL_CD45_SP.sql` (Modified)
- `WebApp/Controllers/HomeController.cs` (Modified)
- `WebApp/Views/Home/Index.cshtml` (Modified)
- `WebApp/app/Controller/AlpineHomeController.js` (Modified)
- `BVTL.Tests/DashboardCD45Tests.cs` (Modified)
- `docs/session-log.md` (Modified)

---

## Phiên làm việc 12 (11/09/2026): Tích hợp DrillDown Chi tiết Khách hàng, Hoàn thiện Quản lý CD45 & Hệ thống Xuất Báo cáo Tự động Định kỳ

### Mục tiêu:
Khắc phục các vấn đề phát sinh sau đợt kiểm thử thực tế từ file chẩn đoán lỗi (`List_Bugs.xlsx`): tối ưu xem chi tiết thông tin lâm sàng/xã hội của Khách hàng CD45, kích hoạt tính năng DrillDown từ Dashboard sang danh sách khách hàng chi tiết, và phát triển hệ thống tự động xuất/lưu trữ báo cáo định kỳ (`ScheduledReport`).

### Các công việc đã hoàn thành:
1. **Phát triển & Tích hợp Stored Procedure DrillDown (`SP_CD45_DrillDown`)**:
   - Viết tệp `SQL_CD45_SP_DrillDown.sql` hỗ trợ truy vấn lọc đa chiều danh sách khách hàng từ các thẻ KPI và biểu đồ trên Dashboard (theo tình trạng HIV, mức nguy cơ QST, chuyển gửi điều trị, tiếp cận truyền thông...).
   - Bổ sung phương thức `GetDrillDownData` trong `DashboardCD45DA` và API trong `HomeController.cs`.
   - Kết nối tương tác trực tiếp trên giao diện Dashboard (`Views/Home/Index.cshtml`, `AlpineHomeController.js`) cho phép người dùng click vào các chỉ số để mở ngay danh sách khách hàng tương ứng.
2. **Nâng cấp Modal Xem Chi tiết Khách hàng CD45 (`KhachHangCD45`)**:
   - Cải tiến `AlpineKhachHangCD45Controller.js` và `Views/KhachHangCD45/Index.cshtml`: Bổ sung modal chi tiết đầy đủ thông tin hành chính, kết quả sàng lọc QST/HIV, dữ liệu Chẩn đoán F6 và Hỗ trợ xã hội F4 lấy từ cấu trúc REDCap.
   - Xử lý chuẩn hóa ánh xạ mã nhóm (`MA_NHOM`), mã TCV (`MA_TCV`), và chuẩn hóa dữ liệu ngày tham gia.
3. **Phát triển Hệ thống Tự động Xuất Báo cáo Định kỳ (`ScheduledReport`)**:
   - Xây dựng tầng Data Access: `IScheduledReportDA` và `ScheduledReportDA` quản lý cấu hình và lịch sử xuất file.
   - Triển khai dịch vụ background `ReportExportService` và job Quartz `PeriodicReportExportJob` trong `JobScheduler.cs`.
   - Thiết kế giao diện quản lý cấu hình tự động xuất báo cáo tại `Views/ScheduledReport/Index.cshtml` và `AlpineScheduledReportController.js`.
   - Bổ sung trọn bộ Unit Tests `ScheduledReportTests.cs` kiểm tra đọc cấu hình, kiểm tra tính sẵn sàng của dữ liệu và xuất file ZIP (28/28 tests passed).
4. **Biên dịch, Đóng gói & Triển khai**:
   - MSBuild Release toàn bộ Solution `WebApp.sln` đạt 0 errors.
   - Đóng gói Publish vào `D:\Deploy\WebApp_Publish` và Deploy Patch qua `deploy-ftp.ps1` (26 files trong 17.2s).
   - Xác minh Host `103.77.167.206:8090/Login/Index` phản hồi `HTTP 200 OK`.
5. **Kiểm tra Tinh chỉnh & Sửa lỗi Cú pháp JS (11/09/2026 - Tối)**:
   - Sửa lỗi cú pháp dấu phẩy trong đối tượng Alpine `AlpineKhachHangCD45Controller.js` (`getBadgeClassHiv`).
   - Kiểm thử tự động: toàn bộ 28/28 Unit Tests Passed 100%.
   - Deploy Patch lên Host và phát hành Tag Git `v1.3.1`.

### Các tệp đã thay đổi:
- `SQL_CD45_SP_DrillDown.sql` (New)
- `SQL_CD45_SP.sql` (Modified)
- `Data/Admin/CD45KhachHangDA.cs` (Modified)
- `Data/Admin/CD45NhomTcvDA.cs` (Modified)
- `Data/Admin/DashboardCD45DA.cs` (Modified)
- `Data/InterfaceDA/Admin/IDashboardCD45DA.cs` (Modified)
- `Data/Admin/ScheduledReportDA.cs` (New)
- `Data/InterfaceDA/Admin/IScheduledReportDA.cs` (New)
- `Model/ModelExtend/Report/ExportedReportLogModel.cs` (New)
- `WebApp/Controllers/HomeController.cs` (Modified)
- `WebApp/Controllers/KhachHangCD45Controller.cs` (Modified)
- `WebApp/Controllers/NhomTCVCD45Controller.cs` (Modified)
- `WebApp/Controllers/ScheduledReportController.cs` (New)
- `WebApp/Services/IReportExportService.cs` (New)
- `WebApp/Services/ReportExportService.cs` (New)
- `WebApp/Services/Jobs/PeriodicReportExportJob.cs` (New)
- `WebApp/Services/ScheduleTasks/JobScheduler.cs` (Modified)
- `WebApp/Views/Home/Index.cshtml` (Modified)
- `WebApp/Views/KhachHangCD45/Index.cshtml` (Modified)
- `WebApp/Views/ScheduledReport/Index.cshtml` (New)
- `WebApp/app/Controller/AlpineHomeController.js` (Modified)
- `WebApp/app/Controller/AlpineKhachHangCD45Controller.js` (Modified)
- `WebApp/app/Controller/AlpineScheduledReportController.js` (New)
- `BVTL.Tests/ScheduledReportTests.cs` (New)
- `BVTL.Tests/DashboardCD45Tests.cs` (Modified)
- `.gitignore` (Modified - ignore `/TESTING/`)
- `docs/session-log.md` (Modified)

---

## Phiên làm việc 13 (11/09/2026 - Đêm): Ánh xạ Nhãn Dịch vụ Hỗ trợ Xã hội (F4) Khách hàng CD45

### Mục tiêu:
Hoàn thiện hiển thị nhãn tiếng Việt rõ ràng cho các Dịch vụ Hỗ trợ Xã hội (form F4) trên modal chi tiết hồ sơ Khách hàng CD45 thay vì hiển thị mã số thô (1, 2, 3... 10).

### Các công việc đã hoàn thành:
1. **Mở rộng DTO Model**:
   - `Model/ModelExtend/CD45KhachHangModel.cs`: Bổ sung thuộc tính `TenDichVu` trong `CD45_HoTroXhItemModel`.
2. **Xử lý Ánh xạ tại Tầng Data Access**:
   - `Data/Admin/CD45KhachHangDA.cs`: Thêm bảng ánh xạ tĩnh `DictF4Services` (1: Thẻ BHYT, 2: Methadone, 3: Giấy tờ tùy thân, 4: Cư trú, 5: Trợ cấp xã hội, 6: Việc làm, 7: Giáo dục, 8: Pháp lý, 9: Khác, 10: TV & XN HIV) và phương thức `FormatF4Services`.
   - Tự động map giá trị `TenDichVu` cho danh sách `ListHoTroXh` khi lấy chi tiết khách hàng.
3. **Cập nhật Giao diện & Alpine.js**:
   - `WebApp/app/Controller/AlpineKhachHangCD45Controller.js`: Thêm `DICT_F4_SERVICES` và hàm hỗ trợ hiển thị `formatF4Services(val)`.
   - `WebApp/Views/KhachHangCD45/Index.cshtml`: Cập nhật bảng Lịch sử Hỗ trợ xã hội (F4) hiển thị `formatF4Services(ht.DICH_VU || ht.TenDichVu)`.
4. **Kiểm thử & Triển khai**:
   - Biên dịch Release: 0 errors.
   - Chạy 28/28 Unit Tests: 100% Passed.
   - Đóng gói Publish & Deploy FTP: 26 files trong 16.9s.
   - Xác minh Healthcheck: `HTTP 200 OK`.
   - Cập nhật Git Tag: `v1.3.2`.

### Các tệp đã thay đổi:
- `Data/Admin/CD45KhachHangDA.cs` (Modified)
- `Model/ModelExtend/CD45KhachHangModel.cs` (Modified)
- `WebApp/Views/KhachHangCD45/Index.cshtml` (Modified)
- `WebApp/app/Controller/AlpineKhachHangCD45Controller.js` (Modified)
- `docs/session-log.md` (Modified)

---

## Phiên làm việc 14 (14/09/2026): Thiết lập Hệ thống Đóng dấu Phiên bản & Đối chiếu Tự động Local vs Host

### Mục tiêu:
Xây dựng giải pháp toàn diện giúp người phát triển, quản trị viên và người kiểm thử nhận biết tức thì phiên bản đang chạy trên Host IIS có khớp với phiên bản mã nguồn tại Local hay không.

### Các công việc đã hoàn thành:
1. **Xây dựng Lớp Quản lý Phiên bản (`AppVersionHelper`)**:
   - Thêm `Common/Common/AppVersionHelper.cs` và DTO `AppVersionInfo` quản lý đọc tệp `version.json` từ thư mục gốc website với cơ chế fallback đọc Assembly version khi chưa có tệp JSON.
2. **Tự động Đóng dấu Metadata khi Publish**:
   - Cập nhật hàm `Invoke-PublishApp` trong `manage-version.ps1`: tự động trích xuất Git Commit SHA, Git Tag, Nhánh hiện tại và Thời điểm đóng gói, ghi vào `version.json` ở cả thư mục nguồn `WebApp/version.json` và thư mục xuất bản `D:\Deploy\WebApp_Publish/version.json`.
3. **Mở API & Hiển thị Trực quan trên Giao diện**:
   - Bổ sung Action `GetSystemVersion` trả về JSON thông tin phiên bản tại `LoginController` (cho phép truy cập không cần đăng nhập) và `HomeController`.
   - Cấu hình thẻ MIME `.json` trong `WebApp/Web.config` để IIS cho phép truy cập tệp tĩnh `http://host:8090/version.json`.
   - Hiển thị huy hiệu phiên bản kèm thời gian đóng gói dưới chân trang `Views/Shared/_Layout.cshtml` và màn hình đăng nhập `Views/Login/Index.cshtml`.
4. **Tích hợp Lệnh CLI Tự động Đối chiếu (`manage-version.ps1 compare`)**:
   - Bổ sung lệnh `.\manage-version.ps1 compare`: tự động gửi request HTTP lên Host, lấy metadata phiên bản Host và đối chiếu với commit/tag/branch/working tree của Local Git.
   - Xuất bảng so sánh màu sắc trực quan (Xanh: Khớp, Đỏ: Lệch).
5. **Kiểm thử Tự động & Triển khai**:
   - Viết 4 unit tests trong `BVTL.Tests/AppVersionHelperTests.cs` kiểm tra đọc dữ liệu, format nhãn và parse JSON (toàn bộ 32/32 tests passed 100%).
   - Rebuild Release 0 errors, publish và deploy Patch lên Host IIS (`103.77.167.206:8090`).
   - Kiểm tra trực tiếp API Host và lệnh compare: xác nhận Host và Local hoàn toàn đồng bộ.

### Các tệp đã thay đổi/thêm mới:
- `Common/Common/AppVersionHelper.cs` (New)
- `Common/Common.csproj` (Modified)
- `WebApp/Controllers/LoginController.cs` (Modified)
- `WebApp/Controllers/HomeController.cs` (Modified)
- `WebApp/Web.config` (Modified)
- `WebApp/Views/Shared/_Layout.cshtml` (Modified)
- `WebApp/Views/Login/Index.cshtml` (Modified)
- `WebApp/version.json` (New)
- `BVTL.Tests/AppVersionHelperTests.cs` (New)
- `BVTL.Tests/BVTL.Tests.csproj` (Modified)
- `deploy-ftp.ps1` (Modified)
- `manage-version.ps1` (Modified)
- `docs/session-log.md` (Modified)

---

## Phiên làm việc 15 (14/09/2026): Tối ưu Hệ thống Xuất Báo cáo ScheduledReport & Lưu trữ App_Data

### Mục tiêu:
Khắc phục các lỗi liên quan đến xuất báo cáo tự động ScheduledReport, tối ưu việc lọc log trong Unit Test, xử lý giải phóng tệp tạm, đồng bộ đường dẫn tải file báo cáo và tạo cấu trúc lưu trữ `App_Data/ExportedReports`.

### Các công việc đã hoàn thành:
1. **Tối ưu ReportExportService & ScheduledReportDA**:
   - Thêm điều kiện bỏ qua việc ghi log vào database khi chạy `UnitTest` (`triggerType != "UnitTest"`) để không làm ảnh hưởng đến dữ liệu thực tế.
   - Thêm bộ lọc `TriggerType IS NULL OR TriggerType <> 'UnitTest'` trong truy vấn danh sách log báo cáo `ScheduledReportDA`.
2. **Xử lý dọn dẹp file tạm trong Unit Test**:
   - Cập nhật `ScheduledReportTests.cs` bổ sung khối `try/finally` tự động xóa tệp ZIP tạm thời sau khi hoàn tất kiểm thử.
3. **Cải thiện tính năng tải báo cáo (`ScheduledReportController`)**:
   - Nâng cấp action `DownloadExportedReport`: linh hoạt phân giải đường dẫn file vật lý qua nhiều cấp fallback (`App_Data/ExportedReports/{Year}/{Month}/{FileName}` hoặc `App_Data/ExportedReports/{FileName}`) thay vì chỉ phụ thuộc vào đường dẫn tuyệt đối tĩnh trong DB.
4. **Cập nhật Giao diện ScheduledReport**:
   - Cập nhật badge hiển thị phương thức kích hoạt (`item.TriggerType`) phân biệt rõ `AutoSchedule` (Tự động), `Manual` (Thủ công) và các loại khác.
5. **Cấu hình Thư mục Lưu trữ**:
   - Bổ sung định nghĩa `Folder Include="App_Data\ExportedReports\"` vào `WebApp.csproj` cùng tệp `.gitkeep` để đảm bảo cấu trúc thư mục lưu trữ báo cáo luôn sẵn sàng trên IIS.
6. **Kiểm thử, Đóng gói & Triển khai**:
   - Rebuild Release thành công, vượt qua toàn bộ 32/32 unit tests.
   - Đóng gói Publish, cập nhật version lên `v1.4.1` và đẩy bản vá lên Host IIS qua FTP.
   - Cập nhật Git Repository và đối chiếu đồng bộ hoàn toàn giữa Local và Host.

### Các tệp đã thay đổi/thêm mới:
- `BVTL.Tests/ScheduledReportTests.cs` (Modified)
- `Data/Admin/ScheduledReportDA.cs` (Modified)
- `WebApp/Controllers/ScheduledReportController.cs` (Modified)
- `WebApp/Services/ReportExportService.cs` (Modified)
- `WebApp/Views/ScheduledReport/Index.cshtml` (Modified)
- `WebApp/WebApp.csproj` (Modified)
- `WebApp/App_Data/ExportedReports/.gitkeep` (New)
- `docs/session-log.md` (Modified)

---

## Phiên làm việc 16 (15/09/2026): Nâng cấp Báo cáo CD45, Quản lý Tệp Báo cáo & Đồng bộ Bản vá v1.4.2

### Mục tiêu:
Sửa lỗi tính toán Mục VI và chuẩn hóa danh sách tên Tiếp cận viên CD45 trong CSDL SQL Server; hoàn thiện định dạng thẩm mỹ khi xuất Excel (dấu `-` cho số 0, ẩn giá trị ô tiêu đề nhóm); nâng cấp toàn diện phân hệ Báo cáo Định kỳ ScheduledReport (cơ chế Upsert tránh trùng lặp bản ghi và tính năng Xóa báo cáo kèm tệp vật lý máy chủ).

### Các công việc đã hoàn thành:
1. **Sửa Logic CSDL SQL Server CD45 (`SQL_CD45_SP.sql`, `SQL_CD45_SP_DrillDown.sql`)**:
   - Khắc phục lỗi đếm nhầm tại Mục VI (Dịch vụ chuyển gửi khác) do điều kiện so khớp chuỗi số `LIKE '%1%'` lẫn với mã `10` (HIV).
   - Đồng bộ bảng tạm `INNER JOIN #TmpKH` bên trong `LEFT JOIN CD45_HO_TRO_XH` để đảm bảo tổng số liệu các mục con luôn bằng tổng 5 nhóm đích.
   - Thêm script `SQL_CD45_Fix_TCV_Names.sql` chuẩn hóa lại 21 tên TCV CD45 viết sai chính tả.
2. **Chuẩn hóa Định dạng Xuất Báo cáo Excel (ClosedXML)**:
   - Cập nhật `BaoCaoCD45Controller.cs`, `BaoCaoTCVCD45Controller.cs` và `ReportExportService.cs`: các ô có giá trị bằng 0 hoặc null được hiển thị bằng dấu gạch ngang (`-`) căn giữa; các dòng tiêu đề nhóm in đậm (bold header) được xóa sạch giá trị để bảng biểu chuyên nghiệp, dễ đọc.
3. **Nâng cấp Hệ thống Quản lý Báo cáo Đã Xuất (`ScheduledReport`)**:
   - Bổ sung phương thức `SaveOrUpdateExportLog` (Upsert): tự động cập nhật bản ghi khi xuất lại báo cáo cho cùng một kỳ thay vì tạo thêm bản ghi mới gây trùng lặp.
   - Bổ sung tính năng Xóa báo cáo (`DeleteLog`): hỗ trợ xóa đồng thời bản ghi trong CSDL và tệp vật lý `.xlsx`/`.zip` trên máy chủ lưu trữ.
   - Xây dựng modal cảnh báo xác nhận xóa (`modalConfirmDelete`) trên giao diện Alpine.js với màu sắc cảnh báo rủi ro cao.
   - Bỏ tùy chọn xuất Tổng hợp BVTL trong dropdown xuất định kỳ của CD45.
4. **Mở rộng Bộ Kiểm thử Tự động (Unit Tests)**:
   - Bổ sung kiểm thử `ScheduledReportDA_SaveOrUpdateExportLog_And_DeleteExportLog_ShouldUpsertAndCleanup`.
   - Bổ sung kiểm thử `ReportExportService_ExportHoatDongCD45ExcelAsync_ShouldSucceedAndDisplayDashesForZero` kiểm tra ClosedXML và định dạng dấu `-`.
   - Bổ sung assembly references `DocumentFormat.OpenXml` và `ExcelNumberFormat` vào `BVTL.Tests.csproj`.
   - Toàn bộ 34/34 tests pass 100%.
5. **Đóng gói Publish, Gắn thẻ Git Tag & Triển khai Host**:
   - Gắn thẻ release `v1.4.2`, đẩy toàn bộ mã nguồn lên nhánh `TUANTM_BVTL_V1.0`.
   - Đóng gói Publish vào `D:\Deploy\WebApp_Publish` và file nén `BVTL_WebApp_Publish_v1.4.2.rar`.
   - Triển khai bản vá (Patch) lên Host IIS qua FTP và xác nhận tính đồng bộ 100% giữa Local và Host.

### Các tệp đã thay đổi/thêm mới:
- `BVTL.Tests/BVTL.Tests.csproj` (Modified)
- `BVTL.Tests/ScheduledReportTests.cs` (Modified)
- `Data/Admin/ScheduledReportDA.cs` (Modified)
- `Data/InterfaceDA/Admin/IScheduledReportDA.cs` (Modified)
- `SQL_CD45_SP.sql` (Modified)
- `SQL_CD45_SP_DrillDown.sql` (Modified)
- `SQL_CD45_Fix_Section_VI.sql` (New)
- `SQL_CD45_Fix_TCV_Names.sql` (New)
- `WebApp/Controllers/BaoCaoCD45Controller.cs` (Modified)
- `WebApp/Controllers/BaoCaoTCVCD45Controller.cs` (Modified)
- `WebApp/Controllers/ScheduledReportController.cs` (Modified)
- `WebApp/Services/ReportExportService.cs` (Modified)
- `WebApp/Views/ScheduledReport/Index.cshtml` (Modified)
- `WebApp/app/Controller/AlpineScheduledReportController.js` (Modified)
- `docs/session-log.md` (Modified)

---

## Phiên làm việc 17 (15/09/2026): Kiểm Soát Khoảng Thời Gian Báo Cáo, Chuẩn Hóa Chu Kỳ CD45 (26-25) & Bản Vá v1.4.3

### Mục tiêu:
Bắt lỗi và ngăn chặn người dùng tìm kiếm/xuất báo cáo khi Đến ngày nhỏ hơn Từ ngày; đánh số thứ tự tuần tự lịch sử tiếp cận của khách hàng; chuẩn hóa chu kỳ tính báo cáo dự án CD45 (từ ngày 26 tháng trước đến 25 tháng này cho Tháng, Quý, Năm) trong hệ thống xuất báo cáo định kỳ ScheduledReport; mở rộng bộ kiểm thử tự động lên 44 tests.

### Các công việc đã hoàn thành:
1. **Kiểm Soát Tính Hợp Lệ Khoảng Thời Gian (`ValidateDateRange`)**:
   - Bổ sung phương thức xác thực tập trung `ValidateDateRange` vào `BaseController.cs` kiểm tra định dạng và bắt buộc Từ ngày <= Đến ngày.
   - Áp dụng vào các Action tìm kiếm và xuất Excel của `BaoCaoCD45Controller.cs` và `BaoCaoTCVCD45Controller.cs`.
   - Bổ sung kiểm tra phía Client trong `AlpineBaoCaoCD45Controller.js` và `AlpineBaoCaoTCVCD45Controller.js` với cảnh báo trực quan bằng Toastr.
2. **Đánh Số Thứ Tự Tuần Tự Lịch Sử Tiếp Cận (`SoThuTu`)**:
   - Thêm trường `SoThuTu` vào `CD45_KhachHang_LichSuTiepCanModel` và cập nhật logic sắp xếp trong `CD45KhachHangDA.cs` để đánh số thứ tự tăng dần 1, 2, 3... (ưu tiên lần đầu F7 ở dòng 1, tiếp đến các lần F8).
   - Hiển thị cột STT chuẩn hóa trên bảng lịch sử tư vấn tại `Views/KhachHangCD45/Index.cshtml`.
3. **Chuẩn Hóa Chu Kỳ Tính Báo Cáo CD45 (Ngày 26 đến ngày 25) trong ScheduledReport**:
   - Thêm hàm `CalculatePeriodDateRange` trong `ReportExportService.cs` tính đúng chu kỳ theo Tháng (26 tháng trước - 25 tháng này), Quý (Q1..Q4) và Năm (26/12 - 25/12).
   - Tối ưu tên tệp xuất báo cáo và tham số lọc theo loại kỳ `periodType` (Month / Quarter / Year).
   - Cập nhật bộ lọc và hiển thị loại kỳ trên giao diện `ScheduledReport/Index.cshtml` và `AlpineScheduledReportController.js`.
4. **Mở Rộng Bộ Kiểm Thử Tự Động (Unit Tests)**:
   - Thêm 10 bài test mới trong `ExcelReportServiceTests.cs` và `ScheduledReportTests.cs`.
   - Toàn bộ 44/44 unit tests vượt qua thành công 100% (thời gian chạy ~15s).
5. **Cấu Hình `.gitignore` & Triển Khai v1.4.3**:
   - Cập nhật `.gitignore` bỏ qua các tệp báo cáo sinh ra ở runtime trong `App_Data/ExportedReports/*`.
   - Gắn thẻ release `v1.4.3`, đóng gói Publish và triển khai Patch lên Host IIS qua FTP.
   - Đối chiếu đồng bộ 100% giữa Local và Host.

### Các tệp đã thay đổi/thêm mới:
- `.gitignore` (Modified)
- `BVTL.Tests/BVTL.Tests.csproj` (Modified)
- `BVTL.Tests/ExcelReportServiceTests.cs` (Modified)
- `BVTL.Tests/ScheduledReportTests.cs` (Modified)
- `Data/Admin/CD45KhachHangDA.cs` (Modified)
- `Data/Admin/ScheduledReportDA.cs` (Modified)
- `Data/InterfaceDA/Admin/IScheduledReportDA.cs` (Modified)
- `Model/ModelExtend/CD45KhachHangModel.cs` (Modified)
- `WebApp/Controllers/BaoCaoCD45Controller.cs` (Modified)
- `WebApp/Controllers/BaoCaoTCVCD45Controller.cs` (Modified)
- `WebApp/Controllers/BaseController.cs` (Modified)
- `WebApp/Controllers/ScheduledReportController.cs` (Modified)
- `WebApp/Services/IReportExportService.cs` (Modified)
- `WebApp/Services/ReportExportService.cs` (Modified)
- `WebApp/Views/BaoCaoCD45/Index.cshtml` (Modified)
- `WebApp/Views/BaoCaoTCVCD45/Index.cshtml` (Modified)
- `WebApp/Views/KhachHangCD45/Index.cshtml` (Modified)
- `WebApp/Views/ScheduledReport/Index.cshtml` (Modified)
- `WebApp/app/Controller/AlpineBaoCaoCD45Controller.js` (Modified)
- `WebApp/app/Controller/AlpineBaoCaoTCVCD45Controller.js` (Modified)
- `WebApp/app/Controller/AlpineScheduledReportController.js` (Modified)
- `docs/session-log.md` (Modified)

---

## Phiên làm việc 18 (16/09/2026): Quản lý Cấu hình Chỉ tiêu Báo cáo CD45, Làm giàu Dữ liệu Drill-down & Bản Vá v1.4.4

### Mục tiêu:
Xây dựng phân hệ quản lý Cấu hình Chỉ tiêu Báo cáo CD45 theo Ma trận chỉ tiêu chuẩn; làm giàu thông tin Tên Tỉnh, Tên Nhóm, Tên Tiếp cận viên khi người dùng tra cứu Drill-down danh sách khách hàng trong Báo cáo Hoạt động CD45; mở rộng bộ kiểm thử tự động lên 45 unit tests.

### Các công việc đã hoàn thành:
1. **Thiết Lập Phân Hệ Cấu Hình Chỉ Tiêu Báo Cáo CD45**:
   - Thêm bảng CSDL và kịch bản phân quyền menu `SQL_CD45_CauHinhChiTieu.sql`, `SQL_Add_Menu_CauHinhChiTieu.sql`.
   - Bổ sung `CD45_CauHinhChiTieuModel` trong `Model/ModelExtend/BaoCaoCD45Model.cs`.
   - Bổ sung giao diện quản lý [CauHinhChiTieu.cshtml](file:///d:/Projects/BVTL-X/WebApp/Views/BaoCaoCD45/CauHinhChiTieu.cshtml) và tích hợp vào thanh điều hướng `Views/Shared/_MenuLeft.cshtml`.
   - Cập nhật `BaoCaoCD45DA.cs` và `IBaoCaoCD45DA.cs` hỗ trợ lấy và cập nhật ma trận chỉ tiêu báo cáo.
2. **Làm Giàu Dữ Liệu Drill-down Báo Cáo CD45**:
   - Cải tiến `BaoCaoCD45DA.GetDrillDown` tự động liên kết lấy đầy đủ Tên Tỉnh (`TEN_TINH`), Tên Nhóm (`TEN_NHOM`), Tên Tiếp cận viên (`TEN_TCV`) thay vì chỉ hiển thị mã định danh thô.
   - Đồng bộ các tham số tìm kiếm và xuất Excel trong `BaoCaoCD45Controller.cs`, `ReportExportService.cs` và giao diện `AlpineBaoCaoCD45Controller.js`.
3. **Mở Rộng Bộ Kiểm Thử Tự Động (Unit Tests)**:
   - Thêm bài test `BaoCaoCD45DA_GetDrillDown_ShouldEnrichNamesForTinhNhomTCV` kiểm tra tính chính xác của dữ liệu drill-down đã làm giàu tên tỉnh/nhóm/TCV.
   - Toàn bộ **45/45 unit tests** đều vượt qua thành công 100%.
4. **Cấu Hình Dự Án & Triển Khai v1.4.4**:
   - Bổ sung tệp view mới vào `WebApp.csproj`.
   - Đảm bảo mã hóa toàn bộ tệp `.sql` và `.cshtml` tuân thủ nghiêm ngặt UTF-8 với BOM (`utf-8-sig`).
   - Gắn thẻ release `v1.4.4`, đóng gói Publish và đẩy bản vá lên Host IIS qua FTP.
   - Đối chiếu đồng bộ 100% giữa Local và Host.

### Các tệp đã thay đổi/thêm mới:
- `BVTL.Tests/ExcelReportServiceTests.cs` (Modified)
- `Data/Admin/BaoCaoCD45DA.cs` (Modified)
- `Data/InterfaceDA/IBaoCaoCD45DA.cs` (Modified)
- `Model/ModelExtend/BaoCaoCD45Model.cs` (Modified)
- `SQL_CD45_SP.sql` (Modified)
- `SQL_Add_Menu_CauHinhChiTieu.sql` (New)
- `SQL_CD45_CauHinhChiTieu.sql` (New)
- `WebApp/Controllers/BaoCaoCD45Controller.cs` (Modified)
- `WebApp/Services/ReportExportService.cs` (Modified)
- `WebApp/Views/BaoCaoCD45/Index.cshtml` (Modified)
- `WebApp/Views/BaoCaoCD45/CauHinhChiTieu.cshtml` (New)
- `WebApp/Views/KhachHangCD45/Index.cshtml` (Modified)
- `WebApp/Views/Shared/_MenuLeft.cshtml` (Modified)
- `WebApp/WebApp.csproj` (Modified)
- `WebApp/app/Controller/AlpineBaoCaoCD45Controller.js` (Modified)
- `WebApp/app/Controller/AlpineKhachHangCD45Controller.js` (Modified)
- `docs/DREAMH/CD45_Bieu_mau_bao_cao_ket_qua_hoat_dong 1.xlsx` (Modified)
- `docs/DREAMH/CD45_Ma_Tran_Cau_hinh_Chi_Tieu_Bao_Cao.xlsx` (New)
- `docs/session-log.md` (Modified)

---

## Phiên làm việc 19 (17/09/2026): Triển khai Giai đoạn P0 - Bộ Quy tắc Chuẩn hóa & Xác thực Dữ liệu (Validation Rules) & Tối ưu Cảnh báo DAG

### Mục tiêu:
Phân tích và triển khai gói ưu tiên P0 của bộ Validate Rules chuẩn hóa dữ liệu cho dự án CD45 (DREAMH) với mốc khởi động chính thức là **2026-01-01**; thiết lập chốt chặn kiểm toán số học tự động khi xuất báo cáo (VR-01); áp dụng luật kiểm soát định dạng Record ID và ràng buộc đối chiếu DAG (VR-02(1)); áp dụng luật kiểm soát tính hợp lệ ngày tháng (VR-03); đồng thời giải quyết triệt để vấn đề spam hàng chục ngàn bản ghi cảnh báo/info R3_DAG_NORMALIZED khi đồng bộ dữ liệu.

### Các công việc đã hoàn thành:
1. **VR-01 [BLOCKING] Thiết Lập Chốt Chặn Kiểm Toán Số Học Tự Động (Pre-Export Gatekeeper)**:
   - Xây dựng lớp trợ năng Common/Common/ReportValidatorHelper.cs kiểm tra điều kiện bảo toàn: Tổng = PUD + PLHIV + TG + SW + MSM trên từng dòng dữ liệu báo cáo CD45. Bỏ qua các dòng tiêu đề/section (IsBold, SEC_*).
   - Tích hợp kiểm toán vào BaoCaoCD45Controller.cs (hàm ExportExcel chặn xuất file nếu phát hiện sai lệch số học; hàm SearchBaoCao trả cảnh báo mềm Warning để hiển thị toastr).
   - Tích hợp kiểm toán vào ReportExportService.cs (ExportHoatDongCD45ExcelAsync trả ExportResult kèm thông báo chi tiết mã chỉ tiêu và dòng vi phạm).
   - Cập nhật giao diện AlpineBaoCaoCD45Controller.js tự động bật toastr warning nếu có dòng dữ liệu lệch cấu trúc tổng.
2. **VR-02(1) [BLOCKING] Kiểm Soát Định Dạng Record ID & Đối Chiếu DAG**:
   - Cập nhật CleanRecordId trong DataCleanerHelper.cs: Kiểm soát chặt 9 ký tự (^D(HN|HP|HY|NA|NB|HC)\d{2}\d{4}$). Nếu sai định dạng -> ghi log ERR_RECORD_ID_FORMAT (SEVERITY = ERROR, ACTION = QUARANTINED), trả 
ull để ngăn nạp dữ liệu sai cấu trúc.
   - Bổ sung luật đối chiếu mã tỉnh của Record ID với mã tiền tố của DAG. Nếu không khớp (ví dụ mã tỉnh Hà Nội DHN... nhưng DAG lại thuộc Ninh Bình hi_vng / NB_HV) -> ghi log ERR_RECORD_ID_DAG_MISMATCH, trả 
ull (chặn cứng).
3. **VR-03(a, c) [BLOCKING] Kiểm Soát Mốc Thời Gian Khởi Động Dự Án & Logic Ngày Hẹn Khám**:
   - Khởi tạo mốc dự án chính thức: ProjectStartDate = new DateTime(2026, 1, 1).
   - Cập nhật CleanDate trong DataCleanerHelper.cs: Chặn tất cả ngày diễn ra trước ngày khởi động dự án < 2026-01-01 (ERR_DATE_BEFORE_PROJECT) và ngày tương lai > Today (ERR_DATE_FUTURE), trả về 
ull. Cho phép ngoại lệ với các cột ngày hẹn trong tương lai (isAppointmentDate = true).
   - Cập nhật ConvertCD45ApiToEntity.cs: Chặn logic ngày hẹn khám/tư vấn xảy ra trước ngày khám/tư vấn thực tế (F6: ERR_F6_APPOINTMENT_BEFORE_VISIT; F7/F8: ERR_F7_APPOINTMENT_BEFORE_VISIT, ERR_F8_APPOINTMENT_BEFORE_VISIT).
4. **Giải Pháp Tối Ưu Hóa Cảnh Báo Chuẩn Hóa Dữ Liệu (Chống Spam Log DAG)**:
   - Xây dựng cơ chế gom nhóm DagSummaryCollector: Thay vì ghi hàng vạn log R3_DAG_NORMALIZED cấp row-level (do 100% DAG từ REDCap đều ở dạng slug cần mapping), hệ thống gom lại thành 1 log tóm tắt theo batch cho mỗi nhóm (BATCH (N=...)).
   - Cập nhật ProcessDag: Không ghi log INFO ở từng dòng; chỉ ghi log WARNING khi gặp DAG lạ chưa từng được ánh xạ (ERR_DAG_UNMAPPED).
   - Cập nhật giao diện Views/DataQuality/Index.cshtml và AlpineDataQualityController.js: Mặc định lọc cấp độ WARNING khi người dùng mở trang, giúp tập trung vào các vấn đề thực sự cần xử lý.
   - Tạo script CSDL SQL_Clean_Old_Standardization_Logs.sql hỗ trợ dọn dẹp các log INFO DAG cũ hơn 7 ngày.
5. **Mở Rộng Bộ Kiểm Thử Tự Động (Unit Tests)**:
   - Tạo bộ kiểm thử mới BVTL.Tests/DataValidationP0Tests.cs gồm 12 bài test chuyên biệt kiểm thử VR-01, VR-02, VR-03 và DagSummaryCollector.
   - Toàn bộ **57/57 unit tests** (45 tests hồi quy + 12 tests mới) đều vượt qua thành công 100%.

### Các tệp đã thay đổi/thêm mới:
- BVTL.Tests/BVTL.Tests.csproj (Modified)
- BVTL.Tests/DataValidationP0Tests.cs (New)
- Common/Common.csproj (Modified)
- Common/Common/ConvertCD45ApiToEntity.cs (Modified)
- Common/Common/DataCleanerHelper.cs (Modified)
- Common/Common/ReportValidatorHelper.cs (New)
- SQL_Clean_Old_Standardization_Logs.sql (New)
- WebApp/Controllers/BaoCaoCD45Controller.cs (Modified)
- WebApp/Services/ReportExportService.cs (Modified)
- WebApp/Views/DataQuality/Index.cshtml (Modified)
- WebApp/app/Controller/AlpineBaoCaoCD45Controller.js (Modified)
- WebApp/app/Controller/AlpineDataQualityController.js (Modified)
- docs/session-log.md (Modified)

---

## Phiên làm việc 20 (17/09/2026): Hoàn thiện Toàn diện Giai đoạn P1 & P2 - Bộ Quy tắc Xác thực Dữ liệu & Nâng cấp Data Quality Dashboard

### Mục tiêu:
Triển khai trọn vẹn Giai đoạn P1 và P2 của Bộ Quy tắc Chuẩn hóa & Xác thực Dữ liệu (Validation Rules VR-01 đến VR-07) cho hệ thống BVTL-X (Dự án CD45 / DREAMH), bao gồm các ràng buộc quan hệ liên form F1-F10, kiểm soát tiến trình và đơn điệu thời gian, phát hiện trùng lặp hồ sơ đa trường (Họ tên + Ngày sinh/Năm sinh + Tỉnh), cảnh báo cụm và mâu thuẫn nghiệp vụ, đồng thời nâng cấp toàn diện Data Quality Monitoring Dashboard với giao diện Đa Tab (Chi tiết, Gom nhóm quy tắc, Thống kê theo Tỉnh/CBO).

### Các công việc đã hoàn thành:

1. **Giai đoạn P1: Ràng buộc Quan hệ Dữ liệu & Tiến trình Dịch vụ**:
   - **VR-04(a) [BLOCKING] Ràng buộc Khách hàng F1 Complete**: Xây dựng `Common/Common/CD45ValidationContext.cs` nạp danh sách khách hàng F1 hợp lệ (`Status = 2`) và nhóm đích. Khách hàng chưa hoàn thành F1 hoặc thiếu nhóm đích sẽ bị chặn nhập vào F2-F10 (`ERR_F1_MISSING`, `ERR_F1_NOT_COMPLETE`, `ERR_F1_INVALID_TARGET_GROUP`).
   - **VR-04(a) [BLOCKING] Phụ thuộc F8 -> F7 Complete & F5 -> F6 Đã khám**: Chặn F8 nếu khách hàng chưa có buổi F7 hoàn thành (`ERR_F8_WITHOUT_F7_COMPLETED`); Chặn F5 nếu khách hàng chưa từng khám tại F6 (`ERR_F5_WITHOUT_F6_VISIT`).
   - **VR-03(b) [BLOCKING] Ràng buộc Thời gian Diễn tiến Dịch vụ**: Chặn các bản ghi dịch vụ F2-F10 có ngày diễn ra trước ngày tạo hồ sơ F1 (`ERR_SERVICE_BEFORE_F1`).
   - **VR-05(a, b, c) [BLOCKING / WARNING] Kiểm soát Thứ tự & Đơn điệu Tiến trình**: Kiểm soát không trùng lần/ngày, đơn điệu ngày theo số lần tăng dần, và gắn cảnh báo nhảy cóc bước (`WARN_PROGRESS_SKIPPED_STEP`).
   - **VR-06(a) Phân định Độc lập Thứ tự Truyền thông Mục II vs Sinh hoạt Nhóm Mục IV.3**: Cập nhật Stored Procedure `SQL_CD45_SP.sql` và `SQL_CD45_SP_DrillDown.sql` xếp hạng độc lập thứ tự buổi truyền thông bằng `ROW_NUMBER() OVER (PARTITION BY RECORD_ID ORDER BY NGAY_HOAT_DONG, REPEAT_INSTANCE)`.
   - **VR-06(b) Chuẩn hóa Khám đầu / Tái khám F6**: Tự động chuyển đổi `lan_kham = 1` thành "Khám đầu", `lan_kham > 1` thành "Tái khám" (`INFO_F6_VISIT_TYPE_NORMALIZED`).
   - **VR-06(c) Chuẩn hóa Điểm QST F3 & Ép Mức 1 khi có Hành vi Tự hại**: Tự động tính toán lại tổng điểm QST, ép mức độ trầm cảm lên Mức 1 nếu câu hỏi tự hại/tự sát có điểm > 0 (`AUTO_F3_QST_RECALCULATED`, `WARN_F3_SELF_HARM_ELEVATED`).
   - **VR-06(d) Cảnh báo Dịch vụ Sau Mất Dấu F9**: Tích hợp module `ConvertF9` cho `CD45_THEO_DAU_Entity` và cảnh báo khi có dịch vụ sau ngày xác nhận mất dấu (`WARN_SERVICE_AFTER_LOST_TO_FOLLOWUP`).
   - **VR-07(a) Cảnh báo Repeat Instance Rỗng**: Bỏ qua các instance rỗng từ REDCap và ghi nhận cảnh báo (`WARN_EMPTY_INSTANCE`).

2. **Giai đoạn P2: Dò Trùng Hồ Sơ, Cảnh Báo Cụm & Nâng Cấp Dashboard**:
   - **VR-02(2) [WARNING] Dò Trùng Hồ Sơ Đa Trường (F1)**:
     - Tạo mới Stored Procedure `SP_CD45_Scan_Duplicate_Clients` (`SQL_CD45_Duplicate_Scan_SP.sql`) quét trùng lặp khách hàng theo tổ hợp (Họ tên + Ngày sinh + Mã tỉnh) và (Họ tên + Năm sinh + Mã tỉnh) nhưng khác Record ID.
     - Tự động ghi nhận log cảnh báo `WARN_DUPLICATE_CLIENT_PROFILE` vào `BVTL_DATA_STANDARDIZATION_LOG`.
   - **VR-07(b) [WARNING] Cảnh báo Cụm Bất Thường (Cluster Incomplete)**:
     - Phát hiện và gắn cờ cảnh báo khi có $\ge 3$ khách hàng của cùng một TCV bị Incomplete trong cùng một ngày (`WARN_CLUSTER_INCOMPLETE`).
   - **VR-07(c) [WARNING] Kiểm tra Cặp Mâu Thuẫn Nghiệp Vụ**:
     - Phát hiện không tham gia nghiên cứu nhưng có mã nghiên cứu (`WARN_CONTRADICTORY_RESEARCH_INFO`).
     - Phát hiện khách hàng nhóm đích PLHIV nhưng lại làm test sàng lọc HIV mới tại Form F4 (`WARN_CONTRADICTORY_HIV_STATUS`).
   - **Nâng Cấp Data Quality Monitoring Dashboard**:
     - Cập nhật `DataQualityDA.cs`: Triển khai `GetGroupedLogs`, `GetStatsByNhom`, `ScanDuplicateClients`.
     - Cập nhật `DataQualityController.cs`: Bổ sung 3 HTTP POST endpoints (`GetGroupedLogs`, `GetStatsByNhom`, `ScanDuplicates`).
     - Cập nhật `AlpineDataQualityController.js`: Quản lý state đa tab, tự động tải dữ liệu theo tab, và hỗ trợ kích hoạt quét trùng đa trường F1 với toastr notification.
     - Nâng cấp `Views/DataQuality/Index.cshtml` (UTF-8 with BOM): Bổ sung thanh Tab điều hướng gồm:
       1. **Tab 1 - Chi tiết Sự kiện & Cảnh báo**: Tìm kiếm, lọc nâng cao, phân trang, và nút "Đã sửa".
       2. **Tab 2 - Gom Nhóm Theo Quy Tắc (Grouped Summary)**: Bảng thống kê theo từng mã lỗi vi phạm (VR-01 đến VR-07), số lượng tồn đọng, hồ sơ gần nhất, mẫu cảnh báo.
       3. **Tab 3 - Thống kê Theo Đơn Vị (Tỉnh / CBO)**: Đánh giá chất lượng dữ liệu từng tỉnh và từng nhóm CBO, phân loại mức độ rủi ro (Cao, Trung bình, Sạch).
       4. **Nút Hành Động**: "Quét trùng hồ sơ F1" và "Xuất Excel Cảnh báo cho M&E".

3. **Kiểm Thử Tự Động Toàn Diện (Unit Tests)**:
   - Viết mới `BVTL.Tests/DataValidationP1Tests.cs` (14 bài test P1) và `BVTL.Tests/DataValidationP2Tests.cs` (15 bài test P2).
   - Đạt kết quả kiểm thử tuyệt đối: **86/86 unit tests PASSED (100%)**, không có bất kỳ regression nào trên toàn bộ solution.

### Các tệp đã thay đổi/thêm mới:
- `BVTL.Tests/BVTL.Tests.csproj` (Modified)
- `BVTL.Tests/DataValidationP1Tests.cs` (New)
- `BVTL.Tests/DataValidationP2Tests.cs` (New)
- `Common/Common.csproj` (Modified)
- `Common/Common/CD45ValidationContext.cs` (New)
- `Common/Common/ConvertCD45ApiToEntity.cs` (Modified)
- `Common/Common/DataCleanerHelper.cs` (Modified)
- `Data/Admin/DataQualityDA.cs` (Modified)
- `Data/API/SyncDataFromApi_SaveToDB.cs` (Modified)
- `Data/Data.csproj` (Modified)
- `Data/InterfaceDA/IDataQualityDA.cs` (Modified)
- `Model/ModelExtend/API/CD45/DreamhDbEntities.cs` (Modified)
- `SQL_CD45_Duplicate_Scan_SP.sql` (New - UTF-8 BOM)
- `SQL_CD45_SP.sql` (Modified - UTF-8 BOM)
- `SQL_CD45_SP_DrillDown.sql` (Modified - UTF-8 BOM)
- `WebApp/Controllers/DataQualityController.cs` (Modified)
- `WebApp/Views/DataQuality/Index.cshtml` (Modified - UTF-8 BOM)
- `WebApp/app/Controller/AlpineDataQualityController.js` (Modified)
- `docs/session-log.md` (Modified - UTF-8 BOM)


---

## Phiên làm việc 21 (17/09/2026): Rà soát và Tinh chỉnh Quy tắc Validate RECORD_ID - Xử lý False Positive cho mã DNT (Quỳnh Hương Xanh / Nha Trang) & Dọn dẹp Log CSDL

### Mục tiêu:
Khắc phục vấn đề cảnh báo nhầm (False Positive) cho mã khách hàng `DNT210242` và `DNT210241` thuộc nhóm Quỳnh Hương Xanh chi nhánh Nha Trang, đồng thời rà soát toàn bộ các mã khách hàng bị gắn cờ cảnh báo trong CSDL, đảm bảo các mã sai thực sự tiếp tục bị chặn và dọn dẹp sạch sẽ các cảnh báo oan.

### Các công việc đã hoàn thành:

1. **Phân tích & Xác minh Dữ liệu**:
   - Khách hàng `DNT210242` và `DNT210241` có cấu trúc: `D` + `NT` (Nha Trang) + `21` (Nhóm Quỳnh Hương Xanh) + `0242` (STT 4 số) = đúng chuẩn 9 ký tự. Cột `CITY_CODE` trong `CD45_KH` lưu giá trị `NT`.
   - Nguyên nhân cảnh báo: Regex cũ trong `DataCleanerHelper.cs` bị giới hạn cứng trong 6 tỉnh (`^D(HN|HP|HY|NA|NB|HC)\d{2}\d{4}$`), thiếu tiền tố `NT`.
   - Rà soát toàn bộ các bản ghi bị cảnh báo `ERR_RECORD_ID_FORMAT` trong CSDL:
     - **Cảnh báo nhầm (False Positive)**: Chỉ duy nhất 2 mã `DNT210241` và `DNT210242` (do thiếu mã tỉnh `NT`).
     - **Sai thật sự (True Positive - tiếp tục duy trì chặn)**:
       - `DNA21271` và `DNA21025`: 8 ký tự (thiếu 1 số 0 ở phần STT).
       - `DNA2002008` và `DNA2200159`: 10 ký tự (dư 1 số 0).
       - `21251` và `21252`: 5 ký tự (mất chữ `D` và mã tỉnh).
       - `D NA210106`: Bị chèn khoảng trắng ở giữa.
       - `DNA`: Chỉ có 3 ký tự (dữ liệu rác).

2. **Cập nhật Mã Nguồn**:
   - `Common/Common/DataCleanerHelper.cs`:
     - Cập nhật Regex kiểm tra định dạng sang dạng tổng quát toàn quốc: `^D[A-Z]{2}\d{2}\d{4}$` (chấp nhận mọi mã tỉnh 2 chữ cái in hoa, 2 số nhóm CBO, 4 số STT = đúng 9 ký tự).
     - Cập nhật quy tắc đối chiếu DAG vs Record ID: Thêm ngoại lệ cho nhóm Quỳnh Hương Xanh (`qunh_hng_xanh` / `qhx`) chấp nhận cả mã tỉnh `NAN` (`NA` - Nghệ An) và `NT` (Nha Trang).
   - `Model/ModelExtend/API/CD45/CD45Helper.cs`:
     - Bổ sung `case "NT": return "NT";` trong hàm `ExtractCityCode`.
   - `BVTL.Tests/DataValidationP0Tests.cs`:
     - Thêm bài test `CleanRecordId_WithDNTCodeAndQuynhHuongXanh_ShouldSucceed` xác thực mã `DNT210242` đạt chuẩn và không sinh bất kỳ log ERROR nào.
     - Thêm bài test `CleanRecordId_WithDNA21271_EightChars_ShouldBlockAndReturnNull` xác thực mã thiếu ký tự `DNA21271` (8 ký tự) bị chặn đúng quy chuẩn.

3. **Dọn dẹp Log CSDL & Kiểm thử Tự động**:
   - Đã xóa sạch 18 dòng log cảnh báo oan `ERR_RECORD_ID_FORMAT` của `DNT210241` và `DNT210242` trong bảng `BVTL_DATA_STANDARDIZATION_LOG`.
   - Biên dịch thành công Release Solution `WebApp.sln`.
   - Chạy toàn bộ bộ kiểm thử tự động VSTest: **88/88 unit tests PASSED (100%)**.

### Các tệp đã thay đổi:
- `Common/Common/DataCleanerHelper.cs` (Modified)
- `Model/ModelExtend/API/CD45/CD45Helper.cs` (Modified)
- `BVTL.Tests/DataValidationP0Tests.cs` (Modified)
- `docs/session-log.md` (Modified - UTF-8 BOM)

---

## Phiên làm việc 22 (18/09/2026): Chuẩn Hóa Biểu Mẫu Báo Cáo Excel Khổ In A4 & Hỗ Trợ Xuất Theo Tỉnh/Nhóm Trong ScheduledReport (Bản Vá v1.5.1)

### Mục tiêu:
Chuẩn hóa toàn diện định dạng biểu mẫu báo cáo Excel (Báo cáo Hoạt động CD45 và Báo cáo TCV CD45) theo đúng quy chuẩn in ấn khổ A4 và khối chữ ký chân trang chuẩn; nâng cấp phân hệ Báo Cáo Định Kỳ ScheduledReport cho phép người dùng tùy chọn phạm vi xuất linh hoạt (Toàn quốc, Tỉnh / Thành phố, hoặc từng Nhóm CBO cụ thể) thay vì chỉ xuất cứng toàn quốc.

### Các công việc đã hoàn thành:
1. **Chuẩn Hóa Khung Biểu Mẫu Excel & Khối Chữ Ký Chân Trang**:
   - Tái cấu trúc khối chữ ký chân trang (Signature block) trong `ReportExportService.cs`, `BaoCaoCD45Controller.cs` và `BaoCaoTCVCD45Controller.cs`:
     - Cột A:B: Chữ ký "Tiếp cận viên" + "(Ký, ghi rõ họ tên)" + Tên TCV in đậm (nếu có).
     - Cột C:E: Chữ ký "Cán bộ dự án" + "(Ký, ghi rõ họ tên)".
     - Cột F:H: Chữ ký "MnE" + "(Ký, ghi rõ họ tên)".
   - Căn chỉnh độ rộng cột chuẩn mực: Cột 1 (#): 5.5, Cột 2 (Chỉ tiêu/Thông tin): 44.0 + WrapText, 6 cột số liệu (Tổng, PUD, PLHIV, TG, SW, MSM): độ rộng 10.0 bằng nhau tuyệt đối.
   - Thiết lập trang in: Khổ A4 dọc (`XLPageOrientation.Portrait`), fit vừa vặn trong 1 trang ngang (`FitToPages(1, 0)`), lề in chuẩn (Trái/Phải: 0.4", Trên/Dưới: 0.6").
2. **Nâng Cấp Xuất Báo Cáo Phân Hệ ScheduledReport Theo Phạm Vi Tỉnh / Nhóm**:
   - Bổ sung API `GetFilterData` tại `ScheduledReportController.cs` trả về danh mục Tỉnh và Nhóm CBO.
   - Cập nhật giao diện modal "Xuất thủ công" tại `Views/ScheduledReport/Index.cshtml` và `AlpineScheduledReportController.js` với dropdown chọn Tỉnh và Nhóm (cascading).
   - Bổ sung tham số phạm vi xuất (`cityCode`, `maNhom`) vào `TriggerExportNow`, hỗ trợ xuất linh hoạt cả Báo cáo Hoạt động và gói ZIP Báo cáo TCV.
   - Đặt tên tệp xuất báo cáo chuẩn xác theo phạm vi: `BaoCao_HoatDong_CD45_Thang07_2026_TOANQUOC.xlsx`, `BaoCao_HoatDong_CD45_Thang07_2026_NAN.xlsx`, `BaoCao_HoatDong_CD45_Thang07_2026_NAN_bm.xlsx`, v.v.
   - Gói ZIP Báo cáo TCV CD45 toàn quốc được tổ chức theo cấu trúc phân tầng thư mục: `[Tên Tỉnh]/[Tên Nhóm]/[Tên Tệp TCV].xlsx`.
3. **Mở Rộng Bộ Kiểm Thử Tự Động (Unit Tests)**:
   - Thêm 5 bài test mới trong `ExcelReportServiceTests.cs` và `ScheduledReportTests.cs` kiểm tra định dạng chữ ký merged, độ rộng cột đồng đều, cấu hình in A4, và xuất báo cáo theo phạm vi Tỉnh/Nhóm.
   - Toàn bộ **93/93 unit tests** đều vượt qua thành công 100%.
4. **Đóng Gói Publish & Triển Khai v1.5.1**:
   - Gắn thẻ release `v1.5.1`, đóng gói Publish vào `D:\Deploy\WebApp_Publish` và file nén `BVTL_WebApp_Publish_v1.5.1.rar`.
   - Triển khai bản vá (Patch) lên Host IIS qua FTP và xác nhận tính đồng bộ 100% giữa Local và Host.

### Các tệp đã thay đổi/thêm mới:
- `BVTL.Tests/BVTL.Tests.csproj` (Modified)
- `BVTL.Tests/ExcelReportServiceTests.cs` (Modified)
- `BVTL.Tests/ScheduledReportTests.cs` (Modified)
- `WebApp/Controllers/BaoCaoCD45Controller.cs` (Modified)
- `WebApp/Controllers/BaoCaoTCVCD45Controller.cs` (Modified)
- `WebApp/Controllers/ScheduledReportController.cs` (Modified)
- `WebApp/Services/ReportExportService.cs` (Modified)
- `WebApp/Views/ScheduledReport/Index.cshtml` (Modified)
- `WebApp/app/Controller/AlpineScheduledReportController.js` (Modified)
- `docs/session-log.md` (Modified)


---

## Phiên làm việc 23 (18/09/2026): Rà soát Toàn diện Hệ thống Danh mục & Nâng cấp Giao diện / Phân trang Danh mục Tỉnh / Thành phố

### Mục tiêu:
1. Rà soát hiện trạng toàn bộ hệ thống danh mục của dự án (BVTL-X / CD45 DREAMH). Đánh giá các danh mục còn thiếu hoặc cần bổ sung/kích hoạt để quản trị dữ liệu can thiệp y tế & cộng đồng chặt chẽ.
2. Khắc phục dứt điểm vấn đề Danh mục Tỉnh/Thành phố thiếu thông tin hiển thị và phân trang (người dùng trước đây chỉ xem được 20 tỉnh đầu tiên, không có thông tin bản ghi, thiếu mã rút gọn dự án và trạng thái bị hardcode).

### Các công việc đã hoàn thành:
1. **Nâng Cấp Giao Diện & Phân Trang Danh mục Tỉnh / Thành phố**:
   - `WebApp/Views/City/Index.cshtml`:
     - Bổ sung thanh Phân trang (Pagination) chuẩn SB Admin 2 kèm dòng hiển thị: *"Hiển thị từ {fromRecord} đến {toRecord} trong tổng số {totalItems} Tỉnh/Thành phố (Trang {currentPage} / {totalPages})"*.
     - Bổ sung bộ chọn số dòng/trang linh hoạt: `10, 20, 30, 50, Tất cả (63 tỉnh/thành)`.
     - Tách riêng cột **Mã Viết Tắt (CD45 / BVTL)** với badge màu sắc nổi bật cho các tỉnh trọng điểm (`HN`, `HP`, `NA`, `NB`, `HC`, `HY`).
     - Động hóa cột **Trạng thái**: Hiển thị badge xanh *"Hoạt động"* nếu `IsActive = true`, badge xám *"Ngừng"* nếu `IsActive = false`.
     - Thêm thẻ thống kê nhanh (KPI card) trên đầu trang: Tổng số 63 Tỉnh/Thành & 6 Tỉnh trọng điểm triển khai dự án CD45.
     - Lưu tệp chuẩn mực với **UTF-8 with BOM (`utf-8-sig`)**.
   - `WebApp/app/Controller/AlpineCityController.js`:
     - Bổ sung các computed getters: `totalPages`, `fromRecord`, `toRecord`, `visiblePages`.
     - Bổ sung các action: `changePage(p)`, `changePageSize(size)`.
     - Thiết lập tham số sắp xếp mặc định `SortColumn: 'Code'` tránh lỗi sắp xếp không xác định.
   - `WebApp/Controllers/CityController.cs`:
     - Bổ sung fallback kiểm soát tham số `SortColumn = "Code"`, `currentPage = 1`, `pageSize = 20` trong hàm `GetAll()`.

2. **Chuẩn Hóa CSDL & Menu Hệ Thống**:
   - Menu `BVTL_QT_PAGE_MENU` (ID = 28): Đổi tên hiển thị thành **"Tỉnh / Thành phố"** và gán nhóm `TITLE_GROUP_MENU = N'Danh mục chung'`.
   - Bảng `BVTL_DU_AN`: Bổ sung mã dự án **`CD45` (Dự án CD45 - DREAMH)** vào danh mục dự án.

3. **Mở Rộng Bộ Kiểm Thử Tự Động (Unit Tests)**:
   - Thêm 2 bài test mới trong `BVTL.Tests/DashboardCD45Tests.cs`:
     - `CityDA_GetAllByPage_ShouldReturn63ProvincesAndProperPaging`
     - `CityDA_GetAllByPage_WithKeyword_ShouldFilterAccurately`
   - Cấu hình `appSettings.config` cho project kiểm thử `BVTL.Tests`.
   - Toàn bộ **95/95 unit tests** PASSED 100%.

### Các tệp đã thay đổi:
- `BVTL.Tests/App.config` (Modified)
- `BVTL.Tests/DashboardCD45Tests.cs` (Modified)
- `WebApp/Controllers/CityController.cs` (Modified)
- `WebApp/Views/City/Index.cshtml` (Modified - UTF-8 BOM)
- `WebApp/app/Controller/AlpineCityController.js` (Modified)
- `docs/session-log.md` (Modified - UTF-8 BOM)


---

## Phiên làm việc 24 (19/09/2026): Nâng cấp Tính năng Drill-Down Chi tiết Cảnh báo theo Đơn vị (Tỉnh / CBO) trên Data Quality Dashboard

### Mục tiêu:
Nâng cấp màn hình Giám sát & Chuẩn hóa Dữ liệu REDCap (Tab 3: *Thống kê theo Đơn vị - Tỉnh / CBO*), cho phép người dùng click trực tiếp vào từng ô số liệu thống kê (Cần xử lý, Tổng Cảnh báo, Tổng Lỗi chặn, Đã khắc phục) để mở hộp thoại Modal xem danh sách chi tiết các cảnh báo/lỗi cụ thể tương ứng, hỗ trợ tìm kiếm nhanh, xử lý trực tiếp ("Đã sửa") và đồng bộ số liệu tức thời.

### Các công việc đã hoàn thành:

1. **Tầng Backend (DA & Controller)**:
   - `Data/InterfaceDA/IDataQualityDA.cs`: Bổ sung phương thức `GetLogsByUnit(string maDuAn, string cityCode, string maNhom, string metricType)`.
   - `Data/Admin/DataQualityDA.cs`: Triển khai `GetLogsByUnit` sử dụng CTE `CTE_LogNhom` chuẩn xác, lọc linh hoạt theo `CITY_CODE`, `MA_NHOM` và `metricType` (`PENDING`, `WARNING`, `ERROR`, `RESOLVED`, `ALL`), đảm bảo dữ liệu trả về khớp 100% với số liệu thống kê của từng dòng.
   - `WebApp/Controllers/DataQualityController.cs`: Bổ sung HTTP POST endpoint `GetLogsByUnit` trả về danh sách nhật ký và tổng số bản ghi dưới dạng JSON.

2. **Tầng Giao Diện & Tương Tác (Razor View & Alpine.js)**:
   - `WebApp/Views/DataQuality/Index.cshtml` (STRICT UTF-8 WITH BOM):
     - Chuyển đổi các ô số liệu Tab 3 thành các link tương tác có gạch chân, con trỏ tay và tooltip chỉ dẫn thao tác khi giá trị > 0.
     - Bổ sung Modal Drill-down `#modalUnitDrillDown` (chuẩn `modal-xl`) có thanh tìm kiếm tức thời theo từ khóa, huy hiệu đếm bản ghi hiển thị, bảng cuộn danh sách cảnh báo chi tiết, nút "Đã sửa" trực tiếp và nút chuyển tiếp sang Tab Chi tiết.
   - `WebApp/app/Controller/AlpineDataQualityController.js`:
     - Quản lý trạng thái drilldown: `currentDrillUnit`, `currentDrillMetric`, `unitDrillModalTitle`, `unitDrillItems`, `filteredUnitDrillItems`, `unitDrillSearchText`, `isUnitDrillLoading`.
     - Triển khai các phương thức: `openUnitDrillDown`, `closeUnitDrillDown` (hỗ trợ đa phiên bản Bootstrap và CSS fallback), `filterUnitDrill`, `markResolvedInDrill` (kích hoạt cập nhật đồng thời số liệu thống kê Tab 3 và các thẻ chỉ số), `jumpToDetailsTab`.

3. **Kiểm Thử Tự Động (Unit Tests)**:
   - `BVTL.Tests/DataValidationP2Tests.cs`: Bổ sung 4 bài test kiểm tra `GetLogsByUnit`:
     - `GetLogsByUnit_WithPendingMetric_ShouldReturnOnlyUnresolvedItems`: Khớp 26 bản ghi `NA / UNKNOWN`.
     - `GetLogsByUnit_WithErrorsMetric_ShouldReturnOnlyErrors`: Khớp 27 bản ghi `ERROR`.
     - `GetLogsByUnit_WithWarningsMetric_ShouldReturnOnlyWarnings`: Khớp 16 bản ghi `WARNING`.
     - `GetLogsByUnit_WithResolvedMetric_ShouldReturnOnlyResolvedItems`: Khớp 17 bản ghi `RESOLVED`.
   - Kết quả kiểm thử: Toàn bộ **99/99 unit tests PASSED (100%)**.

### Các tệp đã thay đổi:
- `Data/InterfaceDA/IDataQualityDA.cs` (Modified)
- `Data/Admin/DataQualityDA.cs` (Modified)
- `WebApp/Controllers/DataQualityController.cs` (Modified)
- `WebApp/Views/DataQuality/Index.cshtml` (Modified - UTF-8 BOM)
- `WebApp/app/Controller/AlpineDataQualityController.js` (Modified)
- `BVTL.Tests/DataValidationP2Tests.cs` (Modified)
- `docs/session-log.md` (Modified - UTF-8 BOM)


---

## Phiên làm việc 25 (19/09/2026): Nâng cấp Hiển thị Tên Tỉnh/Thành phố & Tên Nhóm CBO trên Tab Thống kê theo Đơn vị (Data Quality Dashboard)

### Mục tiêu:
Nâng cấp hiển thị trên Tab 3 (*Thống kê theo Đơn vị - Tỉnh / CBO*) của màn hình Giám sát & Chuẩn hóa Dữ liệu: Chuyển đổi các cột mã viết tắt kỹ thuật (`CITY_CODE` và `MA_NHOM`) sang hiển thị Tên đầy đủ, chính xác của Tỉnh/Thành phố và Nhóm CBO theo nghiệp vụ thực tế, đồng thời giữ mã phụ đề để thuận tiện cho việc đối chiếu tra cứu.

### Các công việc đã hoàn thành:

1. **Tầng Model & Data Access (DA)**:
   - `Model/ModelExtend/API/CD45/DreamhDbEntities.cs`: Bổ sung 2 thuộc tính `TEN_NHOM` và `TEN_TINH` vào class `DataQualityStatsByNhomModel`.
   - `Data/Admin/DataQualityDA.cs`:
     - Xây dựng phương thức `EnrichStatsWithNames` kết hợp linh hoạt bảng danh mục `BVTL_CITES` với danh mục chuẩn hóa 2 ký tự (`NA` -> Nghệ An, `HP` -> Hải Phòng, `HN` -> Hà Nội, `NB` -> Ninh Bình, `HY` -> Hưng Yên, `HC` -> TP Hồ Chí Minh, `NT` -> Nha Trang, `LU` -> Cụm hồ sơ TCV, `AT` -> Gom nhóm tự động).
     - Kết hợp danh mục nhóm CBO từ `BVTL_NHOM_TBH` (mã chính + mã phụ map) và `CD45_NHOM_TCV` (`vn` -> Về nhà, `hd` -> Hải Đăng, `tg` -> The Gate, `qhx` -> Quỳnh Hương Xanh, `UNKNOWN` -> Chưa phân nhóm...).
     - Tự động điền `TEN_TINH` và `TEN_NHOM` cho toàn bộ các dòng thống kê trước khi trả về cho client.

2. **Tầng Giao Diện & Tương Tác (View & Alpine.js)**:
   - `WebApp/Views/DataQuality/Index.cshtml` (STRICT UTF-8 WITH BOM):
     - Cập nhật tiêu đề bảng thành: **Tỉnh / Thành phố** và **Nhóm CBO**.
     - Cột Tỉnh hiển thị Tên tỉnh đậm rõ ràng (`font-weight-bold text-gray-800`), bên dưới có dòng chú thích mã nhỏ (`Mã: HNO`, `Mã: NA`...).
     - Cột Nhóm hiển thị Tên nhóm CBO màu xanh thương hiệu (`font-weight-bold text-primary`), bên dưới có mã nhóm (`Mã: vn`, `Mã: hd`...).
   - `WebApp/app/Controller/AlpineDataQualityController.js`:
     - Cập nhật hàm `openUnitDrillDown`: Tiêu đề Modal Drill-down hiển thị trực tiếp Tên Tỉnh và Tên Nhóm (Ví dụ: `Chi tiết Cảnh báo: Tỉnh [Hà Nội (HNO)] - Nhóm [Về nhà (vn)] | Cần xử lý (Pending) (11 bản ghi)`).

3. **Kiểm Thử Tự Động (Unit Tests)**:
   - `BVTL.Tests/DataValidationP2Tests.cs`:
     - Cập nhật bài test `DataQualityStatsByNhomModel_PropertiesMapping_ShouldHoldExpectedValues` kiểm tra 2 thuộc tính mới.
     - Bổ sung bài test `GetStatsByNhom_ShouldEnrichNamesForTinhAndNhom`: Xác nhận 100% dòng dữ liệu đều được làm giàu Tên Tỉnh và Tên Nhóm; kiểm tra các giá trị cụ thể `HNO / vn` ra đúng `Hà Nội / Về nhà`, `HPG / hd` ra đúng `Hải Phòng / Hải Đăng`.
   - Biên dịch Solution Release thành công 0 lỗi. Toàn bộ 20/20 test P2 PASSED (100%).

### Các tệp đã thay đổi:
- `Model/ModelExtend/API/CD45/DreamhDbEntities.cs` (Modified)
- `Data/Admin/DataQualityDA.cs` (Modified)
- `WebApp/Views/DataQuality/Index.cshtml` (Modified - UTF-8 BOM)
- `WebApp/app/Controller/AlpineDataQualityController.js` (Modified)
- `BVTL.Tests/DataValidationP2Tests.cs` (Modified)
- `docs/session-log.md` (Modified - UTF-8 BOM)
---

## Session 24: [2026-09-21] Chỉ rõ danh tính TCV '4' và Cải tiến chi tiết Cảnh báo cụm bất thường (WARN_CLUSTER_INCOMPLETE)

### Yêu cầu người dùng:
1. Xác định và chỉ rõ danh tính Tiếp cận viên: **TCV '4' là TCV nào? Họ tên đầy đủ là gì?**
2. Thực hiện cải tiến cảnh báo cụm (`WARN_CLUSTER_INCOMPLETE`): Liệt kê danh sách các Mã KH cụ thể và Tên TCV ngay trong thông báo cảnh báo.

---

### Kết quả điều tra thực tế trong CSDL:
1. **TCV '4'**:
   - **Họ và tên**: **Vũ Thị Phương Lan**
   - **Nhóm CBO**: **Về nhà** (Mã: `vn`)
   - **Tỉnh/Thành**: **Hà Nội** (`HNO`)
   - **Cụm 3 khách hàng bị Incomplete ngày 18/08/2026**: `DHN020241`, `DHN020243`, `DHN020255` (Tiền tố `DHN02`: CD45 Hà Nội nhóm Về nhà).
2. **TCV '2'** (xuất hiện cùng cảnh báo cụm):
   - **Họ và tên**: **Chu Thị Thanh**
   - **Nhóm CBO**: **Về nhà** (Mã: `vn`)
   - **Tỉnh/Thành**: **Hà Nội** (`HNO`)
   - **Cụm 7 khách hàng bị Incomplete ngày 17/08/2026**: `DHN020198`, `DHN020199`, `DHN020200`, `DHN020201`, `DHN020204`, `DHN020205`, `DHN020207`.

---

### Các công việc đã hoàn thành:

1. **Ngữ cảnh kiểm thực dữ liệu (`CD45ValidationContext.cs`)**:
   - Bổ sung `TcvNameLookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)`.
   - Thêm phương thức `RegisterTcv(string maNhom, string maTcv, string tenTcv)` và `GetTcvName(string maNhom, string maTcv)`.

2. **Dịch vụ đồng bộ API (`SyncDataFromApi_SaveToDB.cs`)**:
   - Nạp tự động danh bạ 112 TCV từ bảng `CD45_NHOM_TCV` vào `valContext` khi khởi động tiến trình đồng bộ API.

3. **Cải tiến thuật toán cảnh báo cụm (`DataCleanerHelper.cs`)**:
   - Mở rộng phương thức `CheckClusterIncomplete<T>` với các tham số: `Func<T, string> getRecordId`, `Func<T, string> getMaNhom`, `Func<string, string, string> resolveTcvName`.
   - Thu thập danh sách Mã KH duy nhất thuộc cụm: `(gồm các KH: {ma1}, {ma2}, ...)`.
   - Tra cứu và ghép Tên TCV kèm Mã: `TCV '{tenTcv}' (Mã: {maTcv})`.
   - Định dạng nội dung thông báo đầy đủ thông tin hỗ trợ người quản trị kiểm soát ngay trên màn hình.

4. **Tích hợp toàn diện các Form dịch vụ (`ConvertCD45ApiToEntity.cs`)**:
   - Cập nhật wrapper method và toàn bộ 7 vị trí gọi kiểm tra cụm từ `ConvertF2` đến `ConvertF8` (F2 Hoạt động, F3 QST, F4 Hỗ trợ XH, F5 Tuân thủ, F6 Khám chẩn đoán SKTT, F7 Tư vấn lần 1, F8 Tư vấn lần 2).

5. **Cập nhật dữ liệu Log thực tế trong CSDL**:
   - Chạy lệnh SQL cập nhật 2 bản ghi log ID `31444` và `31442` trong bảng `BVTL_DATA_STANDARDIZATION_LOG` sang định dạng thông báo mới, hiển thị đầy đủ tên TCV và danh sách mã KH ngay lập tức.

6. **Kiểm thử tự động (Unit Tests)**:
   - Thêm unit test `CheckClusterIncomplete_WhenRecordIdAndTcvNameProvided_ShouldIncludeClientIdsAndTcvNameInMessage` vào `BVTL.Tests/DataValidationP2Tests.cs`.
   - Kết quả: 21/21 test P2 PASSED (100%), 49/49 DataValidation tests PASSED (100%).

---

### Các tệp đã thay đổi:
- `Common/Common/CD45ValidationContext.cs` (Modified)
- `Common/Common/DataCleanerHelper.cs` (Modified)
- `Common/Common/ConvertCD45ApiToEntity.cs` (Modified)
- `Data/API/SyncDataFromApi_SaveToDB.cs` (Modified)
- `BVTL.Tests/DataValidationP2Tests.cs` (Modified)
- `docs/session-log.md` (Modified - UTF-8 BOM)

---

## Session 25: [2026-09-21] Khắc phục triệt để lỗi phân nhóm CBO & Tỉnh/Thành phố trên Tab Thống kê (Tab 3)

### Yêu cầu người dùng:
1. Giải đáp thắc mắc: Hệ thống đang lấy **Tên nhóm CBO** theo trường nào? Có phải lấy theo trường "Data Access Group (`redcap_data_access_group`)" từ API của các F1 - F10 không?
2. Khắc phục lỗi bất thường trên bảng Thống kê theo Đơn vị:
   - "Mã tỉnh 51" - "Chưa phân nhóm" (198 lỗi)
   - "Mã tỉnh 12" - "Chưa phân nhóm" (18 lỗi)
   - "Hải Phòng (HP)" - "Chưa phân nhóm" (45 lỗi chặn)
   - "Nghệ An (NA)" - "Chưa phân nhóm" (26 lỗi)

---

### Phân tích nguyên nhân gốc rễ:
1. **Cơ chế truy vấn cũ**:
   - Trong `DataQualityDA.cs` (`GetStatsByNhom` và `GetLogsByUnit`), hệ thống cũ **không** lấy trực tiếp từ `redcap_data_access_group` mà thực hiện `LEFT JOIN CD45_KH kh ON log.RECORD_ID = kh.RECORD_ID`.
   - Lấy `ISNULL(kh.MA_NHOM, 'UNKNOWN') AS MA_NHOM` và `ISNULL(kh.CITY_CODE, SUBSTRING(log.RECORD_ID, 2, 2)) AS CITY_CODE`.
2. **Nguyên nhân phát sinh lỗi bất thường**:
   - Các bản ghi bị lỗi định dạng mã khách hàng (`ERR_RECORD_ID_FORMAT`) như `151117`..`151138`, `21251`..`21252`, `DHP10099`.. bị chặn cứng (QUARANTINED) ở tầng validate nên **không bao giờ được nạp vào bảng `CD45_KH`**.
   - Do đó, `kh.MA_NHOM` luôn là `NULL` -> fallback thành `'UNKNOWN'` (**"Chưa phân nhóm"**).
   - Đoạn cắt chuỗi mù quáng `SUBSTRING(log.RECORD_ID, 2, 2)`:
     - Với `151117`: ký tự 2-3 là `'51'` -> hiển thị thành **"Mã tỉnh 51"** (198 lỗi)!
     - Với `21251`: ký tự 2-3 là `'12'` -> hiển thị thành **"Mã tỉnh 12"** (18 lỗi)!
3. **Đối chiếu thực tế với Quy hoạch mã nhóm**:
   - `151117..151138`: Thuộc nhóm **Gió Mới** (Mã: `gm`, Tỉnh: **Ninh Bình** `NBI`, tiền tố chuẩn `DNB15`). Do người dùng nhập thiếu tiền tố `DNB`!
   - `21251..21252`: Thuộc nhóm **Quỳnh Hương Xanh** (Mã: `qhx`, Tỉnh: **Nghệ An** `NAN`, tiền tố chuẩn `DNA21`). Do người dùng nhập thiếu tiền tố `DNA`!
   - `DHP10099..`: Thuộc nhóm **Vòng Tay Bè Bạn** (Mã: `vtbb`, Tỉnh: **Hải Phòng** `HPG`, chuẩn `DHP10`). Do người dùng nhập thiếu số 0 (8 ký tự thay vì 9)!

---

### Các giải pháp & Thay đổi đã triển khai:
1. **Nâng cấp CSDL (`BVTL_REPORTING_DEV`)**:
   - Bổ sung 2 cột mới `MA_NHOM varchar(50) NULL` và `CITY_CODE varchar(50) NULL` vào bảng `BVTL_DATA_STANDARDIZATION_LOG`.
   - Chạy script backfill toàn bộ dữ liệu log lịch sử sang đúng nhóm CBO và tỉnh/thành phố.
2. **Cập nhật Model (`DreamhDbEntities.cs`)**:
   - Bổ sung thuộc tính `MA_NHOM` và `CITY_CODE` vào entity `BVTL_DATA_STANDARDIZATION_LOG_Entity`.
3. **Cập nhật Module Suy luận & Gán Nhóm (`CD45Helper.cs` & `DataCleanerHelper.cs`)**:
   - Xây dựng `CD45Helper.InferGroupAndCity` tra cứu theo quy hoạch 22 nhóm CD45 từ `redcap_data_access_group` (DAG) hoặc từ mã nhóm số (`15` -> `gm`/`NBI`, `21` -> `qhx`/`NAN`, `10` -> `vtbb`/`HPG`, v.v.).
   - Cập nhật `DataCleanerHelper.CleanRecordId`, `FlushToLogs`, `ProcessDag`, `CheckClusterIncomplete` tự động gán `MA_NHOM` và `CITY_CODE` ngay khi tạo log.
4. **Cập nhật Luồng Lưu Dữ liệu (`SyncDataFromApi_SaveToDB.cs` & `InsertDataDA.cs`)**:
   - Tự động kiểm tra và làm giàu `MA_NHOM` / `CITY_CODE` cho toàn bộ danh sách `stdLogs` từ `valContext` trước khi lưu vào CSDL.
   - Cập nhật câu lệnh `MERGE INTO BVTL_DATA_STANDARDIZATION_LOG` đồng bộ cả `MA_NHOM` và `CITY_CODE`.
5. **Cập nhật Thống kê & Drill-Down (`DataQualityDA.cs`)**:
   - Thay đổi câu lệnh CTE `CTE_LogNhom`: Sử dụng `COALESCE(log.MA_NHOM, kh.MA_NHOM, 'UNKNOWN')` và `COALESCE(log.CITY_CODE, kh.CITY_CODE, ...)`.
   - Loại bỏ hoàn toàn lỗi cắt chuỗi mù quáng `SUBSTRING(log.RECORD_ID, 2, 2)`.
6. **Kiểm thử tự động (Unit Tests)**:
   - Cập nhật `BVTL.Tests/DataValidationP2Tests.cs`: thêm test `InferGroupAndCity_ShouldMapMalformedClientIdsAndDags` và chuẩn hóa các test drill-down theo invariant động.
   - Kết quả: **102 / 102 unit tests PASSED (100%)** với VSTest.

---

### Các tệp đã thay đổi:
- `Model/ModelExtend/API/CD45/DreamhDbEntities.cs` (Modified)
- `Model/ModelExtend/API/CD45/CD45Helper.cs` (Modified)
- `Common/Common/CD45ValidationContext.cs` (Modified)
- `Common/Common/DataCleanerHelper.cs` (Modified)
- `Data/API/SyncDataFromApi_SaveToDB.cs` (Modified)
- `Data/API/InsertDataDA.cs` (Modified)
- `Data/Admin/DataQualityDA.cs` (Modified)
- `BVTL.Tests/DataValidationP2Tests.cs` (Modified)
- `docs/session-log.md` (Modified - UTF-8 BOM)

---

## Session 26: [2026-09-21] Chuẩn hóa Thông tin chi tiết Khám SKTT (Hiển thị Tên Cơ sở khám & Tên Chẩn đoán bệnh) trong Báo cáo hoạt động CD45

### Yêu cầu người dùng:
1. Trong Popup chi tiết (chỉ tiêu **"Số lượt KH được chuyển gửi khám SKTT"** hoặc **"Số KH được chuyển gửi khám SKTT"**) tại màn hình **Báo cáo - Báo cáo hoạt động CD45**:
   - Trường **"Thông tin chi tiết"** hiện đang hiển thị mã số thô: `Lần khám: 2 - Cơ sở: 7 - 132`, `Lần khám: 2 - Cơ sở: 7 - 126`, `Lần khám: 2 - Cơ sở: 6 - 132`... Cần hiển thị **Tên của Cơ sở khám** thay vì chỉ hiển thị mã số.
   - Giải thích rõ các con số `126`, `131`, `132` ở cuối chuỗi là mã của thông tin gì? Hiển thị **Tên tương ứng** để thông tin rõ ràng, minh bạch cho người dùng.

---

### Phân tích & Giải đáp nguồn gốc dữ liệu:
1. **Con số `126`, `131`, `132` ở cuối chuỗi là gì?**
   - Đây chính là **Mã Chẩn đoán chính (Primary Diagnosis)** bệnh lý tâm thần theo danh mục ICD-10 của REDCap Form F6 (trường `cd.CHAN_DOAN_CHINH` / `f6_diagnose_pri`).
   - Cụ thể:
     - `114`: **F33- Rối loạn trầm cảm tái diễn**
     - `126`: **F51- Rối loạn giấc ngủ không thực tổn**
     - `131`: **F41.2- Rối loạn hỗn hợp lo âu và trầm cảm**
     - `132`: **Khác**
     - `113`: **F32- Giai đoạn trầm cảm**
     - `118`: **F40- Rối loạn lo âu ám ảnh sợ hãi**
     - (Toàn bộ 132 mã theo phân loại bệnh lý ICD-10 của dự án CD45 DREAMH).
2. **Mã cơ sở khám (`cd.CO_SO_Y_TE`)**:
   - `1`: Hà Nội - Bệnh viện Lão khoa
   - `2`: Hưng Yên - BV SKTT Thái Bình
   - `3`: Hưng Yên - PK Meheal
   - `4`: Hà Nội - Phòng khám Dr Phi
   - `5`: Ninh Bình - BV SKTT Ninh Bình
   - `6`: Bệnh viện tâm thần Nghệ An
   - `7`: Bệnh viện SKTT Hải Phòng
   - `8`: Bệnh viện tâm thần TP.HCM
   - `9`: Bệnh viện Thủ Đức

---

### Các thay đổi kỹ thuật đã triển khai:
1. **Tầng Data Access C# (`Data/Admin/BaoCaoCD45DA.cs`)**:
   - Định nghĩa từ điển danh mục:
     - `DictHospital` (Mã 1..9 -> Tên cơ sở y tế đầy đủ).
     - `DictDiagnose` (Mã 1..132 -> Tên chẩn đoán bệnh lý theo ICD-10).
   - Xây dựng phương thức chuẩn hóa `FormatKhamSKTTChiTiet(CD45_DrillDown_ItemModel item)`:
     - Sử dụng Regex bóc tách linh hoạt: Lần khám, Mã cơ sở, Mã chẩn đoán.
     - Ánh xạ sang Tên cơ sở và Tên chẩn đoán tương ứng.
     - Định dạng đầu ra: `Lần khám: {lan} - Cơ sở: {TenCoSo} - Chẩn đoán: {TenChanDoan}`.
     - Đảm bảo tính Idempotent (không làm biến dạng nếu dữ liệu đã được làm giàu trước đó).
   - Tích hợp tự động vào `EnrichDrillDownData`: Mọi yêu cầu lấy dữ liệu chi tiết (Drill-Down) từ người dùng đều tự động được làm giàu tên cơ sở và chẩn đoán.
2. **Tầng Cơ sở dữ liệu (`SQL_CD45_SP_DrillDown.sql`, `SQL_CD45_SP.sql`, DB `BVTL_REPORTING_DEV`)**:
   - Cập nhật câu lệnh trích xuất chi tiết trong `SP_CD45_GetDrillDown` (các chỉ tiêu `III_3`, `III_4`, `III_4_1`, `III_4_2`, `III_4_3`, `III_5`):
     Bổ sung tiền tố nhãn ` - Chẩn đoán: ` nếu có mã chẩn đoán.
   - Deploy cập nhật trực tiếp `SP_CD45_GetDrillDown` lên CSDL `BVTL_REPORTING_DEV`.
3. **Tầng Giao diện Client (`WebApp/app/Controller/AlpineBaoCaoCD45Controller.js`)**:
   - Tinh chỉnh tiêu đề Modal Drill-down: Nhận biết chỉ tiêu dạng "lượt" (ví dụ: `Số lượt KH được chuyển gửi khám SKTT`) để hiển thị đơn vị chính xác là `(PLHIV: 51 lượt)` thay vì `51 KH`.
4. **Kiểm thử tự động (`BVTL.Tests/ExcelReportServiceTests.cs`)**:
   - Bổ sung bài test `BaoCaoCD45DA_FormatKhamSKTTChiTiet_ShouldEnrichHospitalAndDiagnosisNames`: Kiểm tra toàn diện 9 ca kiểm thử (các mã 7, 6, 1, các mã bệnh 132, 126, 131, 114, trường hợp không có chẩn đoán, tính idempotent...).
   - Bổ sung bài test `BaoCaoCD45DA_GetDrillDown_KhamSKTT_ShouldEnrichHospitalAndDiagnosis`: Kiểm tra trực tiếp dữ liệu thật trả về từ CSDL, kiểm tra khách hàng `DHP090029`, `DHP090053`, `DNA210040`.
   - Kết quả kiểm thử: **103 / 103 tests PASSED (100%)**.

---

### Các tệp đã thay đổi:
- `Data/Admin/BaoCaoCD45DA.cs` (Modified)
- `SQL_CD45_SP_DrillDown.sql` (Modified - UTF-8 BOM)
- `SQL_CD45_SP.sql` (Modified - UTF-8 BOM)
- `WebApp/app/Controller/AlpineBaoCaoCD45Controller.js` (Modified)
- `BVTL.Tests/ExcelReportServiceTests.cs` (Modified)
- `docs/session-log.md` (Modified - UTF-8 BOM)

---

## Session 27: [2026-09-22] Triển khai Tính năng Chuyển đổi "34 Tỉnh mới (Hiện tại - NQ 202/2025/QH15)" và "63 Tỉnh cũ (Lịch sử)", Quản trị Tỉnh trọng điểm CD45

### Yêu cầu người dùng:
1. Rà soát danh mục Tỉnh/Thành phố (`/City/Index`): Bổ sung hiển thị thông tin phân trang, khắc phục tình trạng thiếu sót dữ liệu trên giao diện.
2. Sắp xếp ưu tiên 6 tỉnh trọng điểm CD45 (`HNO`, `HPG`, `HCM`, `NAN`, `NBI`, `HYE`) lên đầu, bổ sung bộ lọc nhanh 6 tỉnh trọng điểm.
3. Bổ sung tính năng cho Quản trị viên (Admin) thiết lập/bật tắt Tỉnh trọng điểm CD45 và quản lý mã viết tắt `Code_Map`.
4. Cập nhật phương án sáp nhập 63 tỉnh thành phố thành **34 Tỉnh/Thành phố mới** theo **Nghị quyết số 202/2025/QH15** (hiệu lực từ 01/07/2025, gồm 11 tỉnh giữ nguyên và 23 tỉnh mới thành lập từ sáp nhập 2-3 tỉnh cũ).
5. Xây dựng tính năng chuyển đổi chế độ xem **"34 Tỉnh mới (Hiện tại - NQ 202)"** và **"63 Tỉnh cũ (Lịch sử)"** trên toàn hệ thống:
   - **Danh mục Tỉnh/Thành phố** (`/City/Index`)
   - **Dashboard CD45** (`/Home/Index`)
   - **Báo cáo Hoạt động CD45** (`/BaoCaoCD45/Index`)
6. Bảo toàn 100% dữ liệu lịch sử REDCap và quy tắc sinh mã `RECORD_ID` (`D` + mã tỉnh 2 ký tự + nhóm 2 số + STT 4 số). Khi chọn 1 tỉnh mới sáp nhập, hệ thống tự động tổng hợp số liệu của tất cả các tỉnh cũ thành phần.

---

### Các thay đổi kỹ thuật đã triển khai:
1. **Tầng Cơ sở dữ liệu (`BVTL_REPORTING_DEV`)**:
   - `SQL_Create_34_New_Cities_And_Mapping.sql`:
     - Tạo bảng `BVTL_DM_TINH_MOI` (34 bản ghi tỉnh/thành mới theo NQ 202/2025/QH15).
     - Tạo bảng ánh xạ `BVTL_MAP_TINH_CU_MOI` (63 bản ghi mapping chi tiết tỉnh cũ -> tỉnh mới).
   - `SQL_CD45_SP_CityMapping_Upgrade.sql` & `SQL_City_KeyProvince_Upgrade.sql`:
     - Nâng cấp `City_Get_By_Page`: Hỗ trợ `@CityMode VARCHAR(10) = 'NEW34'` hoặc `'OLD63'`, `@IsKeyOnly bit = 0`, `@OrderByName = 'KeyFirst'`, tìm kiếm từ khóa trên cả `Code`, `Name`, `Code_Map` và `OldNamesSummary`.
     - Nâng cấp `SP_CD45_Dashboard`: Phân giải `@CityCode` sang các mã tỉnh cũ cấu thành qua CTE/Bảng ánh xạ (`@MappedCityCodes`), Section 5 (ByProvince) tự động gom nhóm theo 34 tỉnh mới hoặc 63 tỉnh cũ tùy theo `@CityMode`.
     - Nâng cấp `SP_CD45_GetBaoCao` & `SP_CD45_GetDrillDown`: Tự động nhận diện danh sách mã tỉnh thành phần để trích xuất số liệu và danh sách khách hàng chính xác.
2. **Tầng Data Access & Model C#**:
   - `Model/ModelExtend/CityMappingModel.cs`: Định nghĩa `CityNewModel` và `CityMappingModel` (thêm vào `Model/Model.csproj`).
   - `Model/ModelExtend/CityPageModel.cs` & `Model/ModelExtend/Base/ModelSearch.cs`: Bổ sung `CityMode`, `OldCount`, `OldNamesSummary`.
   - `Data/InterfaceDA/Admin/ICityDA.cs` & `Data/Admin/CityDA.cs`:
     - Bổ sung `GetAllNewCities(keyOnly)`, `GetCityMappings()`, `GetMappedOldCityCodes(newCityCode)`.
     - `GetAllByPage` hỗ trợ đầy đủ `CityMode`.
     - `UpdateKeyProvince(code, codeMap, isKey)` kiểm tra tính hợp lệ và cập nhật cờ tỉnh trọng điểm.
   - `Data/InterfaceDA/Admin/IDashboardCD45DA.cs` & `Data/Admin/DashboardCD45DA.cs`:
     - Bổ sung tham số `cityMode = "NEW34"` / `"OLD63"`.
     - Trả về `CityName` trực tiếp từ tập kết quả Stored Procedure.
   - `Data/Admin/BaoCaoCD45DA.cs`:
     - Tích hợp 34 tỉnh mới vào từ điển tên tỉnh hiển thị báo cáo.
3. **Tầng Bộ điều khiển & Giao diện (Controllers & Views)**:
   - `WebApp/Controllers/CityController.cs` & `WebApp/Views/City/Index.cshtml` & `AlpineCityController.js`:
     - Segmented Button chuyển đổi: `[34 Tỉnh/Thành mới (NQ 202/2025)]` vs `[63 Tỉnh/Thành lịch sử]`.
     - Bộ lọc nhanh: `[Tất cả: 34/63]` và `[⭐ Tỉnh trọng điểm CD45: 6]`.
     - Cột mới "Đơn vị sáp nhập (NQ 202/2025)" hiển thị danh sách các tỉnh cũ thành phần.
     - Modal Admin "Thiết lập Tỉnh trọng điểm CD45" cho phép bật/tắt tỉnh trọng điểm và cấu hình `Code_Map`.
   - `WebApp/Controllers/HomeController.cs` & `WebApp/Views/Home/Index.cshtml` & `AlpineHomeController.js`:
     - Thêm nút gạt phân loại tỉnh `34 Tỉnh mới` / `63 Tỉnh cũ`.
     - Tự động lọc danh sách nhóm CBO theo các tỉnh cũ thành phần khi chọn một tỉnh mới đã sáp nhập.
     - Đồng bộ biểu đồ và Bảng 4 theo tỉnh.
   - `WebApp/Controllers/BaoCaoCD45Controller.cs` & `WebApp/Views/BaoCaoCD45/Index.cshtml` & `AlpineBaoCaoCD45Controller.js`:
     - Thêm nút chuyển đổi chế độ tỉnh `34 mới` / `63 cũ`.
     - Đồng bộ lọc nhóm CBO và số liệu báo cáo 6 nhóm chỉ tiêu.
4. **Kiểm thử tự động (Unit & Integration Tests)**:
   - Thêm tệp kiểm thử `BVTL.Tests/CityMappingTests.cs` (6 test cases toàn diện: 34 tỉnh mới, 63 tỉnh ánh xạ, 6 tỉnh trọng điểm, phân trang 2 chế độ, Dashboard tổng hợp dữ liệu tỉnh sáp nhập, Báo cáo hoạt động).
   - Bổ sung vào `BVTL.Tests/BVTL.Tests.csproj`.
   - Cập nhật `BVTL.Tests/DashboardCD45Tests.cs`.
   - Toàn bộ **111 / 111 bài kiểm thử tự động VSTest đều PASSED (100%)**.

---

### Các tệp đã thêm mới & thay đổi:
- `SQL_Create_34_New_Cities_And_Mapping.sql` (New - UTF-8 BOM)
- `SQL_CD45_SP_CityMapping_Upgrade.sql` (New - UTF-8 BOM)
- `SQL_City_KeyProvince_Upgrade.sql` (New - UTF-8 BOM)
- `SQL_CD45_SP.sql` (Modified - UTF-8 BOM)
- `SQL_CD45_SP_DrillDown.sql` (Modified - UTF-8 BOM)
- `Model/ModelExtend/CityMappingModel.cs` (New)
- `Model/ModelExtend/CityPageModel.cs` (Modified)
- `Model/ModelExtend/Base/ModelSearch.cs` (Modified)
- `Model/Model.csproj` (Modified)
- `Data/InterfaceDA/Admin/ICityDA.cs` (Modified)
- `Data/Admin/CityDA.cs` (Modified)
- `Data/InterfaceDA/Admin/IDashboardCD45DA.cs` (Modified)
- `Data/Admin/DashboardCD45DA.cs` (Modified)
- `Data/Admin/BaoCaoCD45DA.cs` (Modified)
- `WebApp/Controllers/CityController.cs` (Modified)
- `WebApp/Views/City/Index.cshtml` (Modified - UTF-8 BOM)
- `WebApp/app/Controller/AlpineCityController.js` (Modified)
- `WebApp/Controllers/HomeController.cs` (Modified)
- `WebApp/Views/Home/Index.cshtml` (Modified - UTF-8 BOM)
- `WebApp/app/Controller/AlpineHomeController.js` (Modified)
- `WebApp/Controllers/BaoCaoCD45Controller.cs` (Modified)
- `WebApp/Views/BaoCaoCD45/Index.cshtml` (Modified - UTF-8 BOM)
- `WebApp/app/Controller/AlpineBaoCaoCD45Controller.js` (Modified)
- `BVTL.Tests/CityMappingTests.cs` (New)
- `BVTL.Tests/DashboardCD45Tests.cs` (Modified)
- `BVTL.Tests/BVTL.Tests.csproj` (Modified)
- `docs/session-log.md` (Modified - UTF-8 BOM)

---

## Session 28: [2026-09-22] Nâng cấp Xuất Excel Đa Tab Data Quality, Khôi phục Cấu hình Chỉ tiêu Mặc định & Bộ lọc Kỳ Báo cáo TCV CD45

### Yêu cầu người dùng:
1. **Nâng cấp Xuất Excel Giám sát & Chuẩn hóa Dữ liệu (`/DataQuality/Index`)**:
   - Triển khai Phương án 3 (Dropdown/Split button thông minh kết hợp):
     - Click nút chính: Tự động xuất đúng dữ liệu của Tab người dùng đang đứng (Chi tiết sự kiện, Gom nhóm quy tắc, hoặc Thống kê đơn vị).
     - Click dropdown mở rộng: Cho phép tùy chọn xuất Tab hiện tại, xuất Báo cáo Toàn diện M&E (3 Sheet trong 1 file Excel), hoặc xuất nhanh Cảnh báo chưa xử lý.
2. **Khôi phục Cấu hình Chỉ tiêu Mặc định (`/BaoCaoCD45/CauHinhChiTieu`)**:
   - Bổ sung nút bấm "Khôi phục mặc định" kèm hộp thoại xác nhận an toàn.
   - Thêm các trường dữ liệu `Default_Thang`, `Default_Quy`, `Default_6T`, `Default_12T` vào bảng `CD45_BCTIEU_CAU_HINH`.
   - Viết Stored Procedure `SP_CD45_ResetCauHinhChiTieuMacDinh` để reset nhanh các chỉ tiêu về chuẩn ma trận Excel.
3. **Bộ lọc Kỳ Báo cáo Hoạt động TCV (`/BaoCaoTCVCD45/Index`)**:
   - Bổ sung nút bấm chọn nhanh Kỳ báo cáo: Tháng, Quý, 6 Tháng, Năm (12T), Tùy chọn.
   - Đồng bộ tham số `LoaiBaoCao` cho cả chức năng Tìm kiếm dữ liệu, Xuất Excel đơn lẻ (1 TCV) và Xuất file ZIP toàn bộ TCV.
   - Cải tiến tiêu đề báo cáo và quy tắc đặt tên file Excel/ZIP chứa nhãn kỳ báo cáo rõ ràng.

---

### Các thay đổi kỹ thuật đã triển khai:

1. **Màn hình Giám sát Dữ liệu REDCap (`/DataQuality`)**:
   - `WebApp/Controllers/DataQualityController.cs`:
     - Viết action tổng quát `ExportExcel(string tabType, string maDuAn, string apiCode, string severity, string keyword, string isResolved)`.
     - Hỗ trợ đầy đủ các chế độ: `details` (Sheet ChiTiet_SuKien), `grouped` (Sheet GomNhom_QuyTac), `byunit` (Sheet ThongKe_DonVi), `multi` (Cả 3 sheet trong 1 file), `warnings` (Cảnh báo chưa xử lý).
     - 3 hàm helper ClosedXML: `BuildSheetDetails`, `BuildSheetGrouped`, `BuildSheetByUnit` định dạng chuẩn nhận diện thương hiệu xanh, căn lề, format số liệu, kẻ khung và tô màu Severity.
     - Duy trì alias `ExportExcelWarnings` đảm bảo tương thích ngược.
   - `WebApp/app/Controller/AlpineDataQualityController.js`:
     - Bổ sung `getExportButtonLabel()`, `getExportMenuLabel()`, `exportCurrentTab()`, `exportMultiSheet()`, `exportWarningsOnly()`.
   - `WebApp/Views/DataQuality/Index.cshtml`:
     - Cập nhật Split/Dropdown button màu đỏ (`btn-group`) với biểu tượng Excel, nhãn động và dropdown menu 3 tùy chọn.

2. **Cấu hình Chỉ tiêu Báo cáo CD45 (`/BaoCaoCD45/CauHinhChiTieu`)**:
   - `SQL_CD45_Default_Columns.sql` & `SQL_CD45_SP_ResetDefault.sql`:
     - Thêm cột cấu hình mặc định vào `CD45_BCTIEU_CAU_HINH` và nạp dữ liệu chuẩn ma trận Excel.
     - Tạo Stored Procedure `SP_CD45_ResetCauHinhChiTieuMacDinh`.
   - `Data/InterfaceDA/IBaoCaoCD45DA.cs` & `Data/Admin/BaoCaoCD45DA.cs`:
     - Bổ sung phương thức `ResetCauHinhMacDinh(string updatedBy)`.
   - `WebApp/Controllers/BaoCaoCD45Controller.cs`:
     - Bổ sung action `[HttpPost] ResetCauHinhMacDinh()`.
   - `WebApp/Views/BaoCaoCD45/CauHinhChiTieu.cshtml`:
     - Bổ sung nút bấm "Khôi phục mặc định" cạnh nút "Lưu cấu hình", tích hợp SweetAlert2 xác nhận an toàn.

3. **Báo cáo Hoạt động TCV CD45 (`/BaoCaoTCVCD45`)**:
   - `WebApp/Controllers/BaoCaoTCVCD45Controller.cs`:
     - Hỗ trợ tham số `LoaiBaoCao` cho `SearchData`, `ExportSingleExcel`, `ExportExcel`, `ExportExcelZip`.
     - Thêm nhãn kỳ báo cáo vào tiêu đề dòng 2 trên bảng tính Excel và tên tệp tin `.xlsx` / `.zip`.
   - `WebApp/Views/BaoCaoTCVCD45/Index.cshtml` & `AlpineBaoCaoTCVCD45Controller.js`:
     - Thêm nhóm nút gạt chọn nhanh Kỳ báo cáo (Tháng / Quý / 6T / Năm 12T / Tùy chọn).
     - Tự động đồng bộ dải ngày tương ứng khi chọn kỳ và truyền tham số khi tải/xuất dữ liệu.

4. **Kiểm thử tự động (`BVTL.Tests`)**:
   - `BVTL.Tests/DataValidationP2Tests.cs`:
     - 5 bài kiểm thử mới cho Xuất Excel: Details, Grouped, ByUnit, MultiSheet, Legacy Warnings.
   - Toàn bộ **116 / 116 bài kiểm thử tự động VSTest đều PASSED (100%)**.

---

### Các tệp đã thêm mới & thay đổi:
- `SQL_CD45_Default_Columns.sql` (New - UTF-8 BOM)
- `SQL_CD45_SP_ResetDefault.sql` (New - UTF-8 BOM)
- `Data/InterfaceDA/IBaoCaoCD45DA.cs` (Modified)
- `Data/Admin/BaoCaoCD45DA.cs` (Modified)
- `Model/ModelExtend/BaoCaoCD45Model.cs` (Modified)
- `WebApp/Controllers/DataQualityController.cs` (Modified)
- `WebApp/app/Controller/AlpineDataQualityController.js` (Modified)
- `WebApp/Views/DataQuality/Index.cshtml` (Modified - UTF-8 BOM)
- `WebApp/Controllers/BaoCaoCD45Controller.cs` (Modified)
- `WebApp/Views/BaoCaoCD45/CauHinhChiTieu.cshtml` (Modified - UTF-8 BOM)
- `WebApp/Controllers/BaoCaoTCVCD45Controller.cs` (Modified)
- `WebApp/Views/BaoCaoTCVCD45/Index.cshtml` (Modified - UTF-8 BOM)
- `WebApp/app/Controller/AlpineBaoCaoTCVCD45Controller.js` (Modified)
- `WebApp/Services/ReportExportService.cs` (Modified)
- `BVTL.Tests/DataValidationP2Tests.cs` (Modified)
- `docs/session-log.md` (Modified - UTF-8 BOM)

---

## Session 29: [2026-09-22] Thiết lập Xưng danh / Tiền tố Nhóm CBO (DNXH, Nhóm, TT, CLB) & Chuẩn hóa Báo cáo Chỉ tiêu VII.1 Số quyển phát

### Mục tiêu:
1. Triển khai giải pháp **Phương án 1 (Khuyến nghị cao nhất)** - Thiết lập Xưng danh riêng / Tiền tố Loại hình Tổ chức (Prefix) cho các Nhóm CBO (ví dụ: "Doanh nghiệp xã hội: The Times", "Doanh nghiệp xã hội: Alocare" thay vì mặc định "Nhóm: ...") đồng bộ xuyên suốt từ Cơ sở dữ liệu, Model, Dịch vụ Xuất Excel/ZIP đến Giao diện Quản trị.
2. Chuẩn hóa Báo cáo Hoạt động Chỉ tiêu VII.1 (Tài liệu truyền thông):
   - Đổi từ đếm số dòng (COUNT) sang tổng hợp số lượng quyển phát thực tế (SUM `SL_TAI_LIEU_PHAT`).
   - Cập nhật hiển thị Drill-down và dòng tổng cộng chân bảng hiển thị rõ "Tổng số quyển phát" và "số lượt phát/KH".
3. Tinh chỉnh Engine Giám sát & Chuẩn hóa Dữ liệu REDCap (`DataCleanerHelper.cs`):
   - Khử trùng lặp: Tự động loại bỏ các cảnh báo đơn lẻ `WARN_FORM_INCOMPLETE` khi đã được gom vào cụm cảnh báo 3+ form chưa hoàn thiện trong ngày.
   - Giữ nguyên số lượng tài liệu phát qua hàm `CleanDocumentDelivery`.

---

### Đã hoàn thành:

1. **Cơ sở dữ liệu (Database Layer)**:
   - Tạo tệp `SQL_Nhom_Prefix_Upgrade.sql` (chuẩn UTF-8 with BOM) và thực thi thành công 9 batch trên cơ sở dữ liệu `BVTL_REPORTING_DEV`.
   - Bổ sung 2 cột cấu hình:
     - `PREFIX NVARCHAR(50) DEFAULT N'Nhóm'` (Tiền tố đầy đủ dùng trong báo cáo Excel và văn bản).
     - `SHORT_PREFIX NVARCHAR(20) DEFAULT N'Nhóm'` (Tiền tố viết tắt dùng trong tên thư mục ZIP và mã viết tắt).
     vào cả 2 bảng: `BVTL_NHOM_TBH` và `CD45_NHOM_TCV`.
   - Khởi tạo dữ liệu (Seeding) cho toàn bộ các nhóm tại TP. Hồ Chí Minh (`HC_ALO`, `HC_G3V`, `HC_MYH`, `HC_TGA`, `alo`, `g3vn`, `myh`, `tg`) sang `PREFIX = N'Doanh nghiệp xã hội'`, `SHORT_PREFIX = N'DNXH'`.
   - Cập nhật Stored Procedure `NhomTBH_Get_By_Page` và tạo mới Stored Procedure `SP_CD45_UpdateNhomPrefix` để cập nhật đồng bộ 2 bảng.
   - Cập nhật `SQL_CD45_SP.sql` & `SQL_CD45_SP_DrillDown.sql`: Tính đúng tổng số quyển phát (`SUM(ISNULL(k.SL_TAI_LIEU_PHAT, 1))`) cho Chỉ tiêu VII.1.

2. **Mô hình Dữ liệu & Data Access Layer**:
   - Tạo partial class `Model/ModelExtend/BVTL_NHOM_TBH_Extend.cs` (được đăng ký trong `Model/Model.csproj`) với thuộc tính `[NotMapped]` tránh xung đột metadata EF6 EDMX và cung cấp các phương thức mở rộng:
     - `GetXungDanh()`: Trả về xưng danh đầy đủ, mặc định là `"Nhóm"`.
     - `GetShortXungDanh()`: Trả về xưng danh viết tắt, fallback về `PREFIX` hoặc `"Nhóm"`.
     - `GetFullDisplayName()`: Ví dụ `"Doanh nghiệp xã hội: Alocare"`, `"Nhóm: Bình Minh"`.
     - `GetTitleName()`: Ví dụ `"Doanh nghiệp xã hội Alocare"`.
     - `GetShortTitleName()`: Ví dụ `"DNXH Alocare"`, `"Nhóm Bình Minh"`.
   - Cập nhật `Model/ModelExtend/CD45KhachHangModel.cs` (`CD45_NhomTcvViewModel` bổ sung `PREFIX`, `SHORT_PREFIX`, `GetXungDanh()`, `GetShortXungDanh()`).
   - Cập nhật `Model/ModelExtend/BaoCaoCD45Model.cs` (`CD45_TCV_ItemModel` bổ sung `PREFIX`, `SHORT_PREFIX`).
   - Cập nhật `Data/InterfaceDA/Admin/IBVTL_NHOM_TBHDA.cs` & `Data/Admin/BVTL_NHOM_TBHDA.cs`:
     - Tối ưu `GetAll()` và `GetItemByMaNhom()` nạp trực tiếp `PREFIX`, `SHORT_PREFIX` qua `SqlQuery`.
     - Bổ sung phương thức `UpdatePrefix(string maNhom, string prefix, string shortPrefix)`.
   - Cập nhật `Data/Admin/CD45NhomTcvDA.cs` (`GetListNhomTcv` nạp `PREFIX`, `SHORT_PREFIX`).
   - Cập nhật `Data/Admin/BaoCaoCD45DA.cs` (`GetListTCV` nạp `PREFIX`, `SHORT_PREFIX`).
   - Cập nhật `Data/Admin/DataQualityDA.cs`: Đảm bảo đồng bộ truy vấn log chuẩn hóa.

3. **Dịch vụ Xuất Excel & Nén ZIP Đa cấp (`ReportExportService.cs`, Controllers)**:
   - `WebApp/Services/ReportExportService.cs`:
     - `BuildTCVWorksheet`: Nhận tham số `xungDanh`, hiển thị chính xác tiền tố ở ô A3 (`{xungDanh}: {tenNhom} | Tiếp cận viên: {tenTCV}`).
     - `BuildHoatDongCD45Worksheet`: Hiển thị đúng xưng danh nhóm trong ô A2.
     - `ExportHoatDongCD45ExcelAsync`: Nhận diện và gán đúng xưng danh theo nhóm.
     - `ExportTCVCD45ZipAsync`: Đặt tên thư mục nhóm bên trong file ZIP theo cấu trúc `{SHORT_PREFIX} {TEN_NHOM}` (ví dụ: `DNXH Alocare`, `Nhóm Bình Minh`) và tệp tổng hợp nhóm `BaoCao_TongHop_{SHORT_PREFIX}_{TEN_NHOM}`.
   - `WebApp/Controllers/BaoCaoCD45Controller.cs` & `WebApp/Controllers/BaoCaoTCVCD45Controller.cs`:
     - Tự động tra cứu `xungDanh` theo mã nhóm khi xuất Excel đơn lẻ hoặc nén ZIP hàng loạt.
   - `WebApp/Controllers/NhomTCVCD45Controller.cs`:
     - `GetFilterData`: Trả về `Prefix`, `ShortPrefix`, `DisplayName` (`[DNXH] Alocare`).
     - `ExportExcel`: Thêm cột "Xưng danh / Loại hình" trong file Excel xuất mạng lưới TCV.
     - `UpdateNhomPrefix`: Action API cập nhật xưng danh nhóm từ giao diện.

4. **Giao diện Người dùng Mạng lưới Nhóm & Báo cáo Hoạt động**:
   - `WebApp/Views/NhomTCVCD45/Index.cshtml` & `AlpineNhomTCVCD45Controller.js` (UTF-8 with BOM):
     - Bổ sung cột "Loại hình / Xưng danh" trên bảng danh sách, hiển thị badge trực quan và icon nút bấm chỉnh sửa nhanh.
     - Bổ sung nút liên kết "Đổi xưng danh" ngay bên cạnh bộ lọc CBO khi người dùng chọn một nhóm cụ thể.
     - Modal "Thiết lập Xưng danh / Loại hình Nhóm CBO": Presets nhanh (*Doanh nghiệp xã hội*, *Nhóm*, *Trung tâm*, *Phòng khám*, *Câu lạc bộ*), ô nhập liệu tùy biến, khung xem trước (Live Preview).
   - `WebApp/Views/BaoCaoCD45/Index.cshtml` & `AlpineBaoCaoCD45Controller.js` (UTF-8 with BOM):
     - Nâng cấp modal Drill-down cho Chỉ tiêu VII.1: Hiển thị badge tổng số quyển phát, số lượt phát, và dòng tổng cộng chân bảng rõ ràng.

5. **Kiểm thử tự động (Unit Testing & VSTest)**:
   - `BVTL.Tests/ExcelReportServiceTests.cs`:
     - `NhomTBH_PrefixModelHelpers_ShouldProvideProperDefaultsAndFormatting`: Kiểm tra toàn diện giá trị mặc định, xưng danh tùy biến và các chuỗi hiển thị.
     - `ReportExportService_BuildTCVWorksheet_WithCustomPrefix_ShouldRenderInCellA3`: Kiểm tra việc hiển thị chính xác xưng danh `"Doanh nghiệp xã hội: Alocare"` tại ô A3 trong file Excel sinh ra.
   - `BVTL.Tests/DataValidationP2Tests.cs`: Toàn bộ các test case kiểm tra chuẩn hóa và xuất Excel đều đạt.
   - Toàn bộ các bài kiểm thử hệ thống đều **PASSED (100%)**.

---

### Các tệp đã thêm mới & thay đổi:
- `SQL_Nhom_Prefix_Upgrade.sql` (New - UTF-8 BOM)
- `SQL_CD45_SP.sql` (Modified - UTF-8 BOM)
- `SQL_CD45_SP_DrillDown.sql` (Modified - UTF-8 BOM)
- `Model/ModelExtend/BVTL_NHOM_TBH_Extend.cs` (New)
- `Model/Model.csproj` (Modified)
- `Model/ModelExtend/CD45KhachHangModel.cs` (Modified)
- `Model/ModelExtend/BaoCaoCD45Model.cs` (Modified)
- `Common/Common/DataCleanerHelper.cs` (Modified)
- `Data/InterfaceDA/Admin/IBVTL_NHOM_TBHDA.cs` (Modified)
- `Data/Admin/BVTL_NHOM_TBHDA.cs` (Modified)
- `Data/Admin/CD45NhomTcvDA.cs` (Modified)
- `Data/Admin/BaoCaoCD45DA.cs` (Modified)
- `Data/Admin/DataQualityDA.cs` (Modified)
- `WebApp/Services/ReportExportService.cs` (Modified)
- `WebApp/Controllers/BaoCaoCD45Controller.cs` (Modified)
- `WebApp/Controllers/BaoCaoTCVCD45Controller.cs` (Modified)
- `WebApp/Controllers/NhomTCVCD45Controller.cs` (Modified)
- `WebApp/Controllers/DataQualityController.cs` (Modified)
- `WebApp/Views/NhomTCVCD45/Index.cshtml` (Modified - UTF-8 BOM)
- `WebApp/Views/BaoCaoCD45/Index.cshtml` (Modified - UTF-8 BOM)
- `WebApp/app/Controller/AlpineNhomTCVCD45Controller.js` (Modified)
- `WebApp/app/Controller/AlpineBaoCaoCD45Controller.js` (Modified)
- `BVTL.Tests/ExcelReportServiceTests.cs` (Modified)
- `BVTL.Tests/DataValidationP2Tests.cs` (Modified)
- `docs/session-log.md` (Modified - UTF-8 BOM)

---

## Session 30: [2026-09-23] Sửa lỗi Hiển thị Nhóm HCM chưa có TCV trên Quản trị Mạng lưới (/NhomTCVCD45) & Khắc phục Cơ chế Nạp Xưng danh EF6 trên Form Báo cáo TCV, Xuất Excel & Báo cáo Định kỳ

### Mục tiêu:
1. **Khắc phục Bug 1 (/NhomTCVCD45 - Quản trị Mạng lưới Nhóm & TCV)**:
   - Các nhóm CBO tại TP. Hồ Chí Minh (`HC_ALO` - Alocare, `HC_G3V` - G3VN, `HC_MYH` - Myhands, `HC_TGA` - The Gate) do chưa có Tiếp cận viên thực địa trong bảng `CD45_NHOM_TCV` nên trước đây bị loại hoàn toàn khỏi danh sách hiển thị và bộ lọc theo tỉnh TP.HCM.
   - Chuẩn hóa các chỉ số KPI thống kê mạng lưới (`TongNhom`, `TongTinh`) đếm đủ 22 nhóm CBO và 6 tỉnh/thành phố thuộc dự án CD45.
2. **Khắc phục Bug 2 (BaoCaoTCVCD45/Index, Xuất Excel, Báo cáo Định kỳ Quartz & ScheduledReport)**:
   - Tìm ra và xử lý triệt để nguyên nhân gốc rễ trong tầng Entity Framework 6: Entity `BVTL_NHOM_TBH` có các thuộc tính `PREFIX` và `SHORT_PREFIX` gắn thẻ `[NotMapped]`. Khi gọi `db.Database.SqlQuery<BVTL_NHOM_TBH>`, EF6 dùng entity materializer và tự động bỏ qua các trường `[NotMapped]`, khiến giá trị luôn là `null` và luôn fallback về `"Nhóm"`.
   - Cung cấp POCO DTO `BVTL_NHOM_TBH_DTO` cho các hàm `GetAll()` và `GetItemByMaNhom()` trong `BVTL_NHOM_TBHDA.cs` để bảo toàn thuộc tính xưng danh trên toàn bộ ứng dụng.
   - Bổ sung `Prefix`, `ShortPrefix`, `DisplayName` vào `GetFilterData()` của `BaoCaoTCVCD45Controller.cs` và `ScheduledReportController.cs`.
   - Nâng cấp giao diện `BaoCaoTCVCD45/Index.cshtml` và `AlpineBaoCaoTCVCD45Controller.js` hiển thị xưng danh động tại dropdown chọn nhóm, checklist xuất ZIP hàng loạt, badge và banner xem trước.
   - Hoàn thiện cơ chế fallback nhiều tầng trong `ReportExportService.cs` (`ExportTCVCD45ZipAsync`, `ExportHoatDongCD45ExcelAsync`).

### Các công việc đã hoàn thành:
1. **Model & Data Layer**:
   - `Model/ModelExtend/BVTL_NHOM_TBH_Extend.cs`: Định nghĩa POCO DTO `BVTL_NHOM_TBH_DTO` không bị ảnh hưởng bởi cơ chế materialization của EF6.
   - `Data/Admin/BVTL_NHOM_TBHDA.cs`: Cập nhật `GetAll()` và `GetItemByMaNhom()` sử dụng `SqlQuery<BVTL_NHOM_TBH_DTO>` và ánh xạ sang entity `BVTL_NHOM_TBH`, đảm bảo `PREFIX` và `SHORT_PREFIX` được đọc chính xác từ CSDL.
   - `Data/Admin/CD45NhomTcvDA.cs`:
     - Tái cấu trúc `GetListNhomTcv`: Dùng `UNION ALL` giữa TCV trong `CD45_NHOM_TCV` và các nhóm trong `BVTL_NHOM_TBH` (CD45) chưa có TCV. Gán `ID < 0`, `MA_TCV = NULL`, `TEN_TCV = NULL`, nạp đầy đủ xưng danh từ `BVTL_NHOM_TBH`.
     - Cập nhật `GetKpiStats`: Tính `TongNhom` (22) và `TongTinh` (6) từ `BVTL_NHOM_TBH`.
   - `Data/Admin/BaoCaoCD45DA.cs`: Cập nhật `GetListTCV` join `BVTL_NHOM_TBH` để ưu tiên `n.PREFIX` và `n.SHORT_PREFIX`.
2. **Controllers & Reporting Services**:
   - `WebApp/Controllers/BaoCaoTCVCD45Controller.cs`:
     - `GetFilterData()`: Trả về `Prefix`, `ShortPrefix`, `DisplayName` cho từng nhóm.
     - `ExportSingleExcel()` & `ExportExcelZip()`: Tích hợp logic fallback xưng danh đa tầng (`_BVTL_NHOM_TBHDA` -> `_BaoCaoCD45DA` -> `"Nhóm"`).
   - `WebApp/Controllers/ScheduledReportController.cs`: Cập nhật `GetFilterData()` trả về `Prefix`, `ShortPrefix`, `DisplayName`.
   - `WebApp/Controllers/NhomTCVCD45Controller.cs`: `ExportExcel` xử lý an toàn với các nhóm chưa có TCV.
   - `WebApp/Services/ReportExportService.cs`: Bổ sung fallback xưng danh trong `ExportTCVCD45ZipAsync` và `ExportHoatDongCD45ExcelAsync`.
3. **Giao diện Người dùng (UI & Scripts)**:
   - `WebApp/Views/NhomTCVCD45/Index.cshtml` (UTF-8 with BOM): Hiển thị `(Chưa có TCV)` và badge cảnh báo `Chưa có TCV` khi dòng dữ liệu là nhóm chưa có TCV. Nút sửa xưng danh hoạt động đầy đủ.
   - `WebApp/Views/BaoCaoTCVCD45/Index.cshtml` (UTF-8 with BOM): Cập nhật dropdown nhóm, checklist chọn TCV, badge và banner xem trước sử dụng xưng danh động (`selectedTCVObj.PREFIX || 'Nhóm'`).
   - `WebApp/app/Controller/AlpineBaoCaoTCVCD45Controller.js`: Cập nhật hàm `syncSelect2` hiển thị `[Mã TCV] Tên TCV - [Prefix] Tên Nhóm (Tỉnh)`.
4. **Kiểm thử Tự động & Nghiệm thu Hệ thống**:
   - Viết mới các bài test trong `BVTL.Tests/ExcelReportServiceTests.cs`:
     - `BVTL_NHOM_TBHDA_GetAll_ShouldRetrievePrefixFromDatabase`: Xác nhận đọc đúng `"Doanh nghiệp xã hội"` từ CSDL.
     - `CD45NhomTcvDA_GetListNhomTcv_ShouldIncludeHcmGroupsWith0Tcv`: Xác nhận 4 nhóm TP.HCM hiển thị đầy đủ kể cả khi chưa có TCV.
     - `CD45NhomTcvDA_GetKpiStats_ShouldCountAllGroupsAndProvinces`: Xác nhận `TongNhom >= 22` và `TongTinh >= 6`.
     - `BaoCaoTCVCD45Controller_GetFilterData_ShouldReturnPrefixAndDisplayName`: Xác nhận trả đủ thuộc tính xưng danh.
     - `ReportExportService_BuildTCVWorksheet_WithCustomPrefix_ShouldRenderInCellA3`: Xác nhận ô A3 render chính xác `Doanh nghiệp xã hội: Alocare`.
     - `ReportExportService_ExportTCVCD45ZipAsync_ForHCM_ShouldExportCityAndGroupSummaries`: Kiểm tra xuất báo cáo tổng hợp Tỉnh và nhóm CBO cho TP.HCM trong file ZIP.
   - Toàn bộ **128/128 unit tests** đạt trạng thái **PASSED (100%)**.
   - Toàn bộ solution biên dịch thành công 0 lỗi trên MSBuild cấu hình Release.
   - Đảm bảo nghiêm ngặt UTF-8 with BOM trên tất cả các tệp `.cshtml`, `.sql`, `.ps1`.

### Các tệp đã thay đổi:
- `Model/ModelExtend/BVTL_NHOM_TBH_Extend.cs` (Modified)
- `Data/Admin/BVTL_NHOM_TBHDA.cs` (Modified)
- `Data/Admin/CD45NhomTcvDA.cs` (Modified)
- `Data/Admin/BaoCaoCD45DA.cs` (Modified)
- `WebApp/Controllers/BaoCaoTCVCD45Controller.cs` (Modified)
- `WebApp/Controllers/NhomTCVCD45Controller.cs` (Modified)
- `WebApp/Controllers/ScheduledReportController.cs` (Modified)
- `WebApp/Services/ReportExportService.cs` (Modified)
- `WebApp/Views/NhomTCVCD45/Index.cshtml` (Modified - UTF-8 BOM)
- `WebApp/Views/BaoCaoTCVCD45/Index.cshtml` (Modified - UTF-8 BOM)
- `WebApp/app/Controller/AlpineBaoCaoTCVCD45Controller.js` (Modified)
- `BVTL.Tests/ExcelReportServiceTests.cs` (Modified)
- `BVTL.Tests/ScheduledReportTests.cs` (Modified)
- `docs/session-log.md` (Modified - UTF-8 BOM)

---

## Session 31: [2026-09-24] Thiết lập Chức danh Người ký Đại diện (CHUC_DANH: Giám đốc / Trưởng nhóm) cho Nhóm CBO & Chuẩn hóa Khối Chữ ký Báo cáo Excel

### Mục tiêu:
1. Cho phép cấu hình Chức danh người ký đại diện (`CHUC_DANH`) cho các Nhóm CBO/DNXH (ví dụ: "Giám đốc" đối với các Doanh nghiệp xã hội tại TP.HCM, "Trưởng nhóm" đối với các nhóm CBO truyền thống).
2. Chuẩn hóa Khối chữ ký dưới chân bảng tính Excel báo cáo hoạt động CD45 (`BaoCaoCD45Controller.cs` và `ReportExportService.cs`):
   - Thay thế chức danh cố định bằng chức danh động kèm tên nhóm: `{CHUC_DANH} "{TEN_NHOM}"` (ví dụ: `Giám đốc "Alocare"`, `Trưởng nhóm "Bình Minh"`).
   - Thiết lập chuẩn chữ ký 3 bên: Đại diện đơn vị / CBO, Cán bộ dự án, MnE.
3. Nâng cấp Modal Thiết lập Xưng danh & Chức danh trong Giao diện Quản trị Mạng lưới (`/NhomTCVCD45`):
   - Bổ sung trường nhập liệu Chức danh người ký đại diện (`CHUC_DANH`) kèm preset gợi ý: *Giám đốc*, *Trưởng nhóm*, *Chủ nhiệm*, *Trưởng phòng khám*, *Giám đốc Trung tâm*.
   - Khung xem trước (Live Preview) trực quan tiêu đề báo cáo và khối chữ ký cuối trang theo thời gian thực.
4. Tầng Cơ sở dữ liệu:
   - Tạo tệp `SQL_Nhom_ChucDanh_Upgrade.sql` (UTF-8 with BOM): Bổ sung cột `CHUC_DANH` vào `BVTL_NHOM_TBH` và `CD45_NHOM_TCV`, seed mặc định "Giám đốc" cho các nhóm TP.HCM và nhóm The Time.
   - Cập nhật Stored Procedure `SP_CD45_UpdateNhomPrefix` hỗ trợ lưu tham số `@ChucDanh`.

### Các tệp đã thêm mới & thay đổi:
- `SQL_Nhom_ChucDanh_Upgrade.sql` (New - UTF-8 BOM)
- `Model/ModelExtend/BVTL_NHOM_TBH_Extend.cs` (Modified)
- `Model/ModelExtend/CD45KhachHangModel.cs` (Modified)
- `Model/ModelExtend/BaoCaoCD45Model.cs` (Modified)
- `Data/InterfaceDA/Admin/IBVTL_NHOM_TBHDA.cs` (Modified)
- `Data/Admin/BVTL_NHOM_TBHDA.cs` (Modified)
- `Data/Admin/CD45NhomTcvDA.cs` (Modified)
- `WebApp/Services/ReportExportService.cs` (Modified)
- `WebApp/Controllers/BaoCaoCD45Controller.cs` (Modified)
- `WebApp/Controllers/NhomTCVCD45Controller.cs` (Modified)
- `WebApp/Views/NhomTCVCD45/Index.cshtml` (Modified - UTF-8 BOM)
- `WebApp/app/Controller/AlpineNhomTCVCD45Controller.js` (Modified)
- `BVTL.Tests/ScheduledReportTests.cs` (Modified)
- `docs/session-log.md` (Modified - UTF-8 BOM)

---

## Session 32: [2026-09-25] Phân biệt Tiến trình API Đồng bộ và Tác vụ Hệ thống trên Giao diện Quản lý Auto-Sync (/SyncData)

### Mục tiêu:
1. Nâng cấp bộ đếm tiến trình ngầm Quartz trên màn hình Quản lý Đồng bộ Dữ liệu Tự động (`/SyncData/Index`).
2. Tách bạch rõ ràng số lượng tiến trình API đồng bộ dữ liệu (`syncJobCount`) và các tác vụ hệ thống nền (`systemJobCount`, ví dụ tác vụ tự động xuất báo cáo định kỳ `ScheduledReportJob`).
3. Cải tiến giao diện hiển thị badge: `[syncJobCount] (+[systemJobCount] HT)` kèm tooltip giải thích chi tiết, tránh gây hiểu lầm cho người quản trị khi thấy số lượng tiến trình ngầm lệch so với số lượng API cấu hình.

### Các công việc đã hoàn thành:
1. **Backend Controller (`WebApp/Controllers/SyncDataController.cs`)**:
   - Trong action `GetSchedulerStatus()`: Quét danh sách trigger của scheduler Quartz và phân loại tiến trình dựa vào `jobKey.Group` ("AutoSyncGroup") hoặc `jobKey.Name.EndsWith("_Job")`.
   - Trả về các trường bổ sung: `syncJobCount`, `systemJobCount`, và thuộc tính `IsSyncJob` trong từng item của `jobs`.
2. **Client Controller (`WebApp/app/Controller/AlpineSyncDataController.js`)**:
   - Cập nhật state `scheduler` bổ sung `syncJobCount: 0`, `systemJobCount: 0`.
   - Tự động nạp và gán giá trị khi polling/cập nhật trạng thái scheduler.
3. **Giao diện Người dùng (`WebApp/Views/SyncData/Index.cshtml`)**:
   - Đảm bảo lưu chuẩn UTF-8 with BOM (`utf-8-sig`).
   - Cập nhật card "Tiến trình API ngầm": Hiển thị số lượng API đồng bộ chính, hiển thị thêm nhãn phụ `(+1 HT)` nếu có tác vụ hệ thống đang kích hoạt kèm tooltip diễn giải rõ ràng.

### Các tệp đã thay đổi:
- `WebApp/Controllers/SyncDataController.cs` (Modified)
- `WebApp/app/Controller/AlpineSyncDataController.js` (Modified)
- `WebApp/Views/SyncData/Index.cshtml` (Modified - UTF-8 BOM)
- `docs/session-log.md` (Modified - UTF-8 BOM)

