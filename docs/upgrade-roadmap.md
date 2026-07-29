# Kế hoạch & Lộ trình Nâng cấp Dự án BVTL-X

Kế hoạch này vạch ra các giai đoạn nâng cấp dự án BVTL-X sử dụng mô hình kết hợp giữa **Antigravity (Gemini)** và **Claude Code**.

---

## 1. Phân chia Vai trò giữa 2 AI Tools

| Nhiệm vụ | Antigravity (Gemini) 🟢 | Claude Code 🔵 |
|:---------|:------------------------|:---------------|
| **Research & Planning** | **Chính** (Nhờ khả năng phân tích ngữ cảnh lớn) | Phụ |
| **Code Implementation** | Phụ | **Chính** (Nhờ khả năng viết code và truy cập file tốt) |
| **Build, Run & Testing** | Không hỗ trợ | **Chính** (Thực thi lệnh shell cục bộ) |
| **Review & Security Audit** | **Chính** (Kiểm tra chất lượng code và rủi ro bảo mật) | Phụ |
| **Documentation & Logs** | Linh hoạt | Linh hoạt |

---

## 2. Các Giai đoạn Nâng cấp (Phases)

### Giai đoạn 0: Thiết lập & Vá bảo mật Khẩn cấp (Phase 0)
- **Mục tiêu**: Đồng bộ ngữ cảnh cho AI tools và vá các lỗi bảo mật nghiêm trọng hiện tại.
- **Tác vụ**:
  - Tạo các file cấu hình `CLAUDE.md`, `AGENTS.md` và tài liệu hướng dẫn trong `docs/`.
  - Fix lỗi **SQL Injection** trong tệp `InsertDataDA.cs` (chuyển sang parametrized query).
  - Chuyển đổi mã hóa mật khẩu từ **MD5 sang bcrypt** trong module đăng nhập.
  - Mã hóa chuỗi connection string trong Web.config.
- **Thời gian dự kiến**: 2 ngày.
- **Phân công**: Antigravity soạn tài liệu & plan → Claude Code thực hiện sửa code & kiểm thử.

### Giai đoạn 1: Triển khai Tính năng Mới thử nghiệm (Phase 1)
- **Mục tiêu**: Kiểm chứng quy trình phối hợp giữa 2 AI tools trên một tác vụ nghiệp vụ thực tế.
- **Tác vụ**:
  - Nghiên cứu và xây dựng thêm 1 module báo cáo mới (ví dụ: Báo cáo chỉ số y tế đặc thù).
  - Xây dựng Controller, View Razor, Excel Export và cập nhật menu hệ thống.
- **Thời gian dự kiến**: 2 tuần.
- **Phân công**: Antigravity thiết kế specs → Claude Code viết code → Antigravity review.

### Giai đoạn 2: Refactor Trình xuất Báo cáo Excel (Phase 2)
- **Mục tiêu**: Giảm thiểu sự trùng lặp mã nguồn (code duplication) trong các controller báo cáo.
- **Tác vụ**:
  - Tạo class dùng chung `ReportBuilder.cs` và interface `IReportTemplate.cs` trong `Common/`.
  - Refactor các controller báo cáo (`BaoCaoThangController`, `BaoCaoQuy`...) để chuyển phần format Excel cứng về trình dùng chung.
  - Giảm số lượng dòng code của các controller báo cáo từ ~500 dòng xuống ~150 dòng mà không thay đổi định dạng đầu ra.
- **Thời gian dự kiến**: 2 tuần.

### Giai đoạn 3: Nâng cấp Giao diện & Thư viện UI (Phase 3)
- **Mục tiêu**: Hiện đại hóa giao diện người dùng và loại bỏ các thư viện đã hết hỗ trợ.
- **Tác vụ**:
  - Nâng cấp phiên bản Bootstrap từ **4.x lên 5.3**.
  - Chuẩn hóa CSS, loại bỏ inline style trong các tệp View.
  - Lập kế hoạch và thực hiện chuyển đổi từng phần từ **AngularJS 1.x (đã EOL) sang Javascript thuần (Vanilla JS)** hoặc thư viện nhẹ hơn (e.g. Alpine.js).
- **Thời gian dự kiến**: 3 tuần.
- **Lưu ý**: Thực hiện trên nhánh Git riêng (`feature/ui-upgrade`), chuyển đổi từng màn hình một để tránh lỗi vỡ giao diện.

### Giai đoạn 4: Refactor Module Đồng bộ Dữ liệu (Phase 4)
- **Mục tiêu**: Tăng độ ổn định, hiệu năng và tính an toàn của luồng đồng bộ SyncApp.
- **Tác vụ**:
  - Refactor `ProcessService.cs` và `SyncDataFromApi_SaveToDB.cs`.
  - Áp dụng các mẫu phục hồi lỗi (resilience patterns) như Retry, Circuit Breaker sử dụng thư viện **Polly**.
  - Tách HttpClient dùng chung qua `IHttpClientFactory` thay vì tạo mới HttpClient liên tục.
  - Chuyển đổi các tác vụ đồng bộ tuần tự nặng sang mô hình bất đồng bộ (**async/await**).
- **Thời gian dự kiến**: 2 tuần.

### Giai đoạn 5: Cải tiến Kiến trúc & Unit Tests (Phase 5)
- **Mục tiêu**: Nâng cao tính dễ bảo trì và mở rộng của dự án.
- **Tác vụ**:
  - Nghiên cứu tích hợp một DI Container đơn giản (ví dụ: Autofac hoặc Unity) cho dự án ASP.NET MVC 5.
  - Khởi tạo dự án kiểm thử tự động (Unit Test Project) trong solution.
  - Viết các test case cơ bản cho luồng xử lý tính toán báo cáo và phân quyền.
- **Thời gian dự kiến**: 3 tuần.
