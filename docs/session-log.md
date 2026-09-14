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
