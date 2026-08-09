# Kế hoạch Triển khai Phase 6: Cải tiến Giao diện (UI/UX)

Dựa trên Lộ trình nâng cấp hệ thống (Phase 3 trong Roadmap cũ, tương đương Phase 6 hiện tại), chúng ta sẽ thực hiện cuộc đại tu lớn nhất về Kiến trúc Giao diện, hướng tới một Web App hiện đại, mượt mà và bảo mật hơn (loại bỏ AngularJS EOL).

## 1. Phương án Kỹ thuật & Thẩm mỹ (Aesthetics)

### A. Nâng cấp Thư viện Cốt lõi (Core UI)
- **Từ Bootstrap 3/4 lên Bootstrap 5.3**: 
  Thay thế toàn bộ thư viện JS/CSS cũ bằng phiên bản Bootstrap 5 mới nhất. Việc này đòi hỏi phải đổi tên rất nhiều thuộc tính (ví dụ: `data-toggle` thành `data-bs-toggle`, `data-target` thành `data-bs-target`).
- **Thiết kế Thẩm mỹ Cao (Premium Design)**:
  - Bổ sung **Google Fonts** (Inter hoặc Roboto) làm font chủ đạo.
  - Tinh chỉnh Sidebar và Header: Thêm hiệu ứng Glassmorphism (Kính mờ), Gradients tinh tế và Micro-animations (hiệu ứng hover mượt mà) để tạo cảm giác "Wow" cho hệ thống báo cáo.
  - Tối ưu hóa các bảng dữ liệu (DataTables) cho đồng nhất với theme mới.

### B. Chuyển đổi AngularJS sang Alpine.js
- **Tình trạng hiện tại**: Dự án có hơn 50 Controller viết bằng AngularJS 1.x (đã hết vòng đời hỗ trợ - EOL).
- **Giải pháp**: Sử dụng **Alpine.js** thay thế hoàn toàn AngularJS. Alpine.js mang lại trải nghiệm declarative data-binding tương tự AngularJS (như `x-model`, `@click`) nhưng gọn nhẹ và hiện đại hơn.
  - Sử dụng `fetch` API thay cho `$http`.
  - Sử dụng `x-for` và `x-text` thay cho `ng-repeat` và `{{ }}`.
  - Sử dụng `@click` (hoặc `x-on:click`) và `x-model` thay cho `ng-click`, `ng-model`.
- **Thực thi Thí điểm (Proof-of-Concept)**: 
  Sẽ không thể chuyển đổi 50+ màn hình trong 1 lần làm việc. Tôi đề xuất chúng ta sẽ chuyển đổi **Màn hình Báo Cáo Tháng (`BaoCaoThang/Index.cshtml`)** trước. Sau khi màn hình này chạy mượt mà bằng Alpine.js + Bootstrap 5, bạn (và đội ngũ) có thể áp dụng tương tự cho các màn hình còn lại.

---

## 2. Kế hoạch Code (Proposed Changes)

### `WebApp/Views/Shared/_Layout.cshtml` & `_MenuLeft.cshtml`
- [MODIFY] [_Layout.cshtml](file:///D:/Projects/BVTL-X/WebApp/Views/Shared/_Layout.cshtml) Thay đổi URL tải thư viện sang Bootstrap 5.
- [MODIFY] Nâng cấp cấu trúc CSS (thêm link Google Fonts, custom variables).
- [MODIFY] Sửa đổi thuộc tính `data-*` sang `data-bs-*` để Sidebar/Menu có thể hoạt động đóng/mở chuẩn xác.

### `WebApp/Content/custom-style.css`
- [NEW] [custom-style.css](file:///D:/Projects/BVTL-X/WebApp/Content/custom-style.css) Bổ sung các token CSS hiện đại (Glassmorphism, Gradient, Hover animations).

### `WebApp/Views/BaoCaoThang/Index.cshtml`
- [MODIFY] [Index.cshtml](file:///D:/Projects/BVTL-X/WebApp/Views/BaoCaoThang/Index.cshtml) Loại bỏ các directives AngularJS và thay thế bằng directives của Alpine.js (`x-data`, `x-model`, `@click`). Nhúng file js/cdn của Alpine.js.
- [MODIFY] Chuyển đổi Select2 và DatetimePicker để tương thích với component của Alpine.js.

### `WebApp/app/Controller/BaoCaoThangController.js`
- [DELETE] [BaoCaoThangController.js](file:///D:/Projects/BVTL-X/WebApp/app/Controller/BaoCaoThangController.js) Sẽ không cần dùng file AngularJS này nữa.
- [NEW] [AlpineBaoCaoThang.js](file:///D:/Projects/BVTL-X/WebApp/app/Controller/AlpineBaoCaoThang.js) Viết lại logic controller dưới dạng store hoặc component data của Alpine.js.

---

## 3. Xác minh (Verification Plan)
- **UI Testing**: Mở giao diện trang chủ và Đảm bảo Thanh menu, Header hoạt động đóng/mở mượt mà. Đánh giá tính thẩm mỹ tổng thể.
- **Function Testing**: Mở trang Báo Cáo Tháng, bấm tải dữ liệu, chọn tỉnh thành... Đảm bảo Alpine.js xử lý data binding và logic thay thế xuất sắc AngularJS mà không có lỗi Console.

> [!WARNING]  
> Việc loại bỏ AngularJS và cài Bootstrap 5 có thể làm vỡ layout ở một số màn hình khác (những màn hình chưa được chuyển đổi). Tuy nhiên, ta đang phát triển trên bản sao `DEV`, do đó việc này là cần thiết để tạo bộ khung mẫu trước.

> [!IMPORTANT]
> **Đây là kế hoạch được khôi phục từ phiên làm việc trước.** Tôi đã xác định mã nguồn nằm tại `D:\Projects\BVTL-X\WebApp`. Bạn vui lòng Approve kế hoạch này để chúng ta bắt đầu thực hiện chỉnh sửa mã nguồn nhé!
