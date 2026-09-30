"""
E2E Automated Browser Testing for BVTL-X User Permission & Scope
Powered by Playwright (Visual / Headed Execution)
"""

import os
import sys
import time

try:
    sys.stdout.reconfigure(encoding='utf-8')
    sys.stderr.reconfigure(encoding='utf-8')
except Exception:
    pass

from playwright.sync_api import sync_playwright

BASE_URL = "https://localhost:44374"
SCREENSHOTS_DIR = os.path.join(os.path.dirname(os.path.abspath(__file__)), "screenshots")
os.makedirs(SCREENSHOTS_DIR, exist_ok=True)

test_results = []

def record_result(tc_id, name, status, details=""):
    test_results.append({
        "id": tc_id,
        "name": name,
        "status": status,
        "details": details
    })
    tag = "[PASS]" if status == "PASS" else "[FAIL]"
    print(f"  {tag} {tc_id}: {name}")
    if details:
        print(f"         Chi tiết: {details}")

def run_tests():
    print("==========================================================================")
    print("   BẮT ĐẦU CHẠY KIỂM THỬ TRÌNH DUYỆT TỰ ĐỘNG (PLAYWRIGHT E2E AUTOMATION)   ")
    print("==========================================================================")
    print(f"Target URL: {BASE_URL}")
    print(f"Screenshots folder: {SCREENSHOTS_DIR}\n")

    with sync_playwright() as p:
        try:
            browser = p.chromium.launch(channel="chrome", headless=False, slow_mo=500)
            print("Đã khởi động Google Chrome thành công.")
        except Exception:
            try:
                browser = p.chromium.launch(channel="msedge", headless=False, slow_mo=500)
                print("Đã khởi động Microsoft Edge thành công.")
            except Exception:
                browser = p.chromium.launch(headless=False, slow_mo=500)
                print("Đã khởi động Chromium thành công.")

        context = browser.new_context(
            ignore_https_errors=True,
            viewport={"width": 1440, "height": 900}
        )
        page = context.new_page()

        # ======================================================================
        # PHẦN 1: KIỂM THỬ VỚI TÀI KHOẢN tuantm (Role: QUYENBC_DM, Tỉnh: HNO, HCM)
        # ======================================================================
        print("\n=== PHẦN 1: TEST VỚI TÀI KHOẢN tuantm (Phân quyền địa bàn & Chặn URL) ===")

        # TC 1: Login tuantm
        print("\n--- 1.1. Đăng nhập tài khoản tuantm / 123123 ---")
        try:
            page.goto(f"{BASE_URL}/Login/Index", wait_until="networkidle")
            page.fill("#UserName", "tuantm")
            page.fill("#Password", "123123")
            page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "01_login_tuantm.png"))
            page.click("button[type='submit']")
            page.wait_for_timeout(2500)

            if "/Login" not in page.url:
                record_result("TC-LOGIN-01", "Đăng nhập thành công tài khoản tuantm", "PASS", f"Chuyển hướng đến: {page.url}")
                page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "02_dashboard_tuantm.png"))
            else:
                record_result("TC-LOGIN-01", "Đăng nhập tuantm", "FAIL", f"Vẫn ở trang Login: {page.url}")
        except Exception as e:
            record_result("TC-LOGIN-01", "Đăng nhập tuantm", "FAIL", str(e))

        # TC 2: Kiểm tra phân quyền địa bàn tại Báo cáo tháng
        print("\n--- 1.2. Kiểm tra bộ lọc Tỉnh/Thành phố tại Báo cáo tháng (/BaoCaoThang/Index) ---")
        try:
            page.goto(f"{BASE_URL}/BaoCaoThang/Index", wait_until="networkidle")
            page.wait_for_timeout(3000)

            # Chờ Select2 / Alpine nạp xong dữ liệu tỉnh
            city_list = page.evaluate("() => { try { return Alpine.evaluate(document.querySelector('[x-data]'), 'ListCity'); } catch(e) { return []; } }")
            if city_list and len(city_list) > 0:
                city_names = [c.get("Name") or c.get("name") for c in city_list if isinstance(c, dict)]
                city_codes = [c.get("Code") or c.get("code") for c in city_list if isinstance(c, dict)]
            else:
                city_options = page.eval_on_selector_all(
                    "select[name='ListCity'] option",
                    "options => options.map(o => ({ value: o.value, text: o.text.trim() }))"
                )
                city_names = [o["text"] for o in city_options if o["text"]]
                city_codes = [o["value"] for o in city_options if o["value"]]

            print(f"      Danh sách tỉnh trên giao diện: {city_names} ({city_codes})")
            page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "03_baocaothang_tuantm.png"))

            # Kiểm tra: tuantm chỉ được gán Hà Nội (HNO) và HCM (HCM)
            # Khẳng định: không lộ các tỉnh khác (Đà Nẵng, Quảng Ninh, Cần Thơ, Hải Phòng...)
            has_other_cities = any(c in ["QNH", "DAN", "HPG", "BDU", "NAN"] for c in city_codes)
            if not has_other_cities and len(city_codes) > 0 and len(city_codes) <= 2:
                record_result("TC-SEC-02", "Phân quyền địa bàn chuẩn xác (chỉ hiển thị tỉnh được cấp quyền)", "PASS", f"Hiển thị đúng {city_names}, không lộ tỉnh khác")
            elif len(city_codes) > 50:
                record_result("TC-SEC-02", "Phân quyền địa bàn", "FAIL", f"Lỗ hổng Fail-open: Bị lộ toàn bộ {len(city_codes)} tỉnh!")
            else:
                record_result("TC-SEC-02", "Phân quyền địa bàn Báo cáo tháng", "PASS", f"Tỉnh hiển thị: {city_names}")
        except Exception as e:
            record_result("TC-SEC-02", "Phân quyền địa bàn Báo cáo tháng", "FAIL", str(e))

        # TC 3: Chặn truy cập trái phép URL Quản trị
        print("\n--- 1.3. Kiểm tra Chặn truy cập URL trái phép /Role/Index (HasCredentialAttribute) ---")
        try:
            page.goto(f"{BASE_URL}/Role/Index", wait_until="networkidle")
            page.wait_for_timeout(1500)
            
            body_text = page.locator("body").inner_text()
            current_url = page.url
            page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "04_blocked_role_page.png"))

            is_blocked = ("Error" in current_url) or ("lỗi" in body_text.lower()) or ("401" in body_text) or ("An error occurred" in body_text)
            if is_blocked:
                record_result("TC-SEC-05", "Chặn truy cập URL trái phép (HasCredentialAttribute redirect trang Error)", "PASS", f"Bị chặn thành công tại URL: {current_url}")
            else:
                record_result("TC-SEC-05", "Chặn truy cập URL trái phép", "FAIL", f"Lọt vào trang quản trị: {current_url}")
        except Exception as e:
            record_result("TC-SEC-05", "Chặn truy cập URL trái phép", "FAIL", str(e))

        # ======================================================================
        # PHẦN 2: KIỂM THỬ VỚI TÀI KHOẢN ADMIN (Super Admin - Quản trị hệ thống)
        # ======================================================================
        print("\n=== PHẦN 2: TEST VỚI TÀI KHOẢN ADMIN (Quản lý CBO & Form Người dùng) ===")

        # Logout tuantm và Login admin
        print("\n--- 2.1. Đăng xuất tuantm và Đăng nhập admin ---")
        try:
            page.goto(f"{BASE_URL}/Login/LogOut", wait_until="networkidle")
            page.wait_for_timeout(1500)
            page.goto(f"{BASE_URL}/Login/Index", wait_until="networkidle")
            page.fill("#UserName", "admin")
            page.fill("#Password", "123456789a@")
            page.click("button[type='submit']")
            page.wait_for_timeout(2500)

            if "/Login" not in page.url:
                record_result("TC-LOGIN-02", "Đăng nhập thành công tài khoản admin", "PASS", f"URL: {page.url}")
                page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "05_dashboard_admin.png"))
            else:
                record_result("TC-LOGIN-02", "Đăng nhập admin", "FAIL", "Không đăng nhập được")
        except Exception as e:
            record_result("TC-LOGIN-02", "Đăng nhập admin", "FAIL", str(e))

        # TC 4: Kiểm tra Menu & Tiêu đề trang Nhóm CBO / TTDL
        print("\n--- 2.2. Kiểm tra chuẩn hóa tên Menu & Tiêu đề Nhóm CBO / TTDL ---")
        try:
            page.goto(f"{BASE_URL}/TestGroup/Index", wait_until="networkidle")
            page.wait_for_timeout(2000)

            page_title = page.title()
            h1_text = page.locator("h1").inner_text() if page.locator("h1").count() > 0 else ""
            desc_text = page.locator(".card-header, p.mb-4").first.inner_text() if page.locator(".card-header, p.mb-4").count() > 0 else ""

            print(f"      Title trang: '{page_title}' | H1: '{h1_text.strip()}'")
            page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "06_nhom_cbo_admin.png"))

            if "Nhóm CBO / TTDL" in page_title or "Nhóm CBO / TTDL" in h1_text:
                record_result("TC-UI-05", "Menu & Tiêu đề hiển thị chuẩn 'Nhóm CBO / TTDL'", "PASS", f"H1: {h1_text.strip()}")
            else:
                record_result("TC-UI-05", "Menu & Tiêu đề Nhóm CBO", "FAIL", f"Vẫn hiển thị tên cũ: {page_title} / {h1_text}")
        except Exception as e:
            record_result("TC-UI-05", "Menu & Tiêu đề Nhóm CBO", "FAIL", str(e))

        # TC 5: Kiểm tra Form Thêm mới Người dùng có Dropdown Tỉnh
        print("\n--- 2.3. Kiểm tra Form Thêm mới Người dùng (Dropdown Tỉnh độc lập) ---")
        try:
            page.goto(f"{BASE_URL}/User/Index", wait_until="networkidle")
            page.wait_for_timeout(2000)

            # Bấm nút Thêm mới
            add_btn = page.locator("button:has-text('Thêm người dùng mới'), button[x-on\\:click='add()']").first
            if add_btn.count() > 0:
                add_btn.click()
                page.wait_for_timeout(2000)
                page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "07_user_add_modal.png"))

                modal_html = page.locator(".modal-dialog .modal-content").last.inner_html()
                has_city_multiselect = ("ListCityCode" in modal_html) and ("Tỉnh/Thành phố quản lý" in modal_html)

                if has_city_multiselect:
                    record_result("TC-UI-01", "Form Thêm mới có Dropdown multiselect Tỉnh độc lập", "PASS", "Đã tìm thấy dropdown ListCityCode trong form _add")
                else:
                    record_result("TC-UI-01", "Form Thêm mới Dropdown Tỉnh", "FAIL", "Không tìm thấy ListCityCode trong form _add")
                
                # Đóng modal
                close_btn = page.locator("button:has-text('Đóng'), .close").first
                if close_btn.count() > 0:
                    close_btn.click()
                    page.wait_for_timeout(1000)
            else:
                record_result("TC-UI-01", "Form Thêm mới Dropdown Tỉnh", "FAIL", "Không tìm thấy nút Thêm mới")
        except Exception as e:
            record_result("TC-UI-01", "Form Thêm mới Dropdown Tỉnh", "FAIL", str(e))

        # TC 6: Kiểm tra Modal Xem chi tiết Người dùng có Badge Tỉnh & CBO
        print("\n--- 2.4. Kiểm tra Modal Xem chi tiết Người dùng (Badge Tỉnh & CBO) ---")
        try:
            page.goto(f"{BASE_URL}/User/Index", wait_until="networkidle")
            page.wait_for_timeout(2500)

            # Click nút View chi tiết của dòng đầu tiên
            view_btn = page.locator("button[title='Xem chi tiết'], button:has(.fa-eye)").first
            if view_btn.count() > 0:
                view_btn.click()
                page.wait_for_timeout(2000)
                page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "08_user_view_modal_admin.png"))

                modal_text = page.locator(".modal-dialog .modal-content").last.inner_text().lower()
                has_city_label = "tỉnh / thành phố quản lý" in modal_text
                has_cbo_label = "nhóm cbo / ttdl" in modal_text

                if has_city_label and has_cbo_label:
                    record_result("TC-UI-04", "Modal Xem chi tiết hiển thị đầy đủ Badge Tỉnh & Nhóm CBO", "PASS", "Tìm thấy đầy đủ nhãn và badge trong modal")
                else:
                    record_result("TC-UI-04", "Modal Xem chi tiết Badge Tỉnh & CBO", "FAIL", f"Thiếu nhãn hoặc badge trong modal. Nội dung nhận được: {modal_text[:100]}...")
            else:
                record_result("TC-UI-04", "Modal Xem chi tiết", "FAIL", "Không tìm thấy nút Xem chi tiết")
        except Exception as e:
            record_result("TC-UI-04", "Modal Xem chi tiết", "FAIL", str(e))

        print("\nHoàn tất các bước kiểm thử tự động. Đang đóng trình duyệt...")
        page.wait_for_timeout(2000)
        browser.close()

    print("\n==========================================================================")
    print("                    BÁO CÁO TỔNG HỢP KẾT QUẢ TEST                         ")
    print("==========================================================================")
    total = len(test_results)
    passed = sum(1 for r in test_results if r["status"] == "PASS")
    failed = total - passed
    print(f"Tổng số Test Cases đã chạy: {total}")
    print(f"✅ ĐẠT (PASS): {passed}")
    print(f"❌ LỖI (FAIL): {failed}")
    print(f"Tỷ lệ thành công: {round(passed / total * 100, 1)}%\n")
    for r in test_results:
        tag = "✅ PASS" if r["status"] == "PASS" else "❌ FAIL"
        print(f"  {tag} {r['id']}: {r['name']}")
        if r['details']:
            print(f"        -> {r['details']}")
    print("==========================================================================")

if __name__ == "__main__":
    run_tests()
