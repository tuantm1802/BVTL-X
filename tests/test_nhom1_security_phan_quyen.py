"""
Automated Test Suite for NHÓM 1: BẢO MẬT & PHÂN QUYỀN ĐỊA BÀN
Tests:
- TC-SEC-01: Fail-Closed khi tài khoản chưa được gán tỉnh (hienvu)
- TC-SEC-02: Phân quyền địa bàn theo đúng phạm vi (tuantm: HNO, BDI/GLA, HCM; cả 34 mới và 63 cũ)
- TC-SEC-03: Phân quyền nhiều tỉnh (tuantmhcm: NBI, HCM) trên CD45
- TC-SEC-04: Quyền Super Admin xem toàn bộ danh mục tỉnh (admin)
- TC-SEC-05: Chặn truy cập trái phép URL quản trị (HasCredentialAttribute)
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

test_results = {}

def run_tests():
    print("=" * 80)
    print("    KIỂM THỬ TỰ ĐỘNG NHÓM 1: BẢO MẬT & PHÂN QUYỀN ĐỊA BÀN (PLAYWRIGHT)")
    print("=" * 80)

    with sync_playwright() as p:
        browser = p.chromium.launch(channel="chrome", headless=False, slow_mo=400)
        context = browser.new_context(ignore_https_errors=True)
        page = context.new_page()

        # -------------------------------------------------------------
        # 1. TC-SEC-01: Fail-Closed cho user chưa gán tỉnh (hienvu)
        # -------------------------------------------------------------
        print("\n>>> [1/5] TC-SEC-01: Kiểm tra Fail-Closed cho tài khoản hienvu (CityCodes rỗng)")
        page.goto(f"{BASE_URL}/Login/Index", wait_until="networkidle")
        page.locator("input[name='UserName'], input#UserName").first.fill("hienvu")
        page.locator("input[name='Password'], input#Password").first.fill("123123")
        page.locator("button:has-text('Đăng nhập')").first.click()
        page.wait_for_timeout(2000)

        # Mở BaoCaoCD45
        page.goto(f"{BASE_URL}/BaoCaoCD45/Index", wait_until="networkidle")
        page.wait_for_timeout(2500)
        
        # Kiểm tra options tỉnh
        options = page.locator("select[x-model='selectedCity'] option").all_inner_texts()
        print(f"   [hienvu] Dropdown Tỉnh options: {options}")
        
        # Với Fail-closed, chỉ có option mặc định, không có bất kỳ tỉnh cụ thể nào
        has_real_provinces = any("(" in opt and ")" in opt for opt in options)
        screenshot_path = os.path.join(SCREENSHOTS_DIR, "tc_sec_01_fail_closed_hienvu.png")
        page.screenshot(path=screenshot_path)
        
        if not has_real_provinces:
            print("   ✅ TC-SEC-01 PASS: Fail-Closed hoạt động đúng! Không rò rỉ tỉnh nào cho hienvu.")
            test_results["TC-SEC-01"] = "PASS"
        else:
            print(f"   ❌ TC-SEC-01 FAIL: Bị lộ tỉnh: {options}")
            test_results["TC-SEC-01"] = "FAIL"

        # Logout
        page.goto(f"{BASE_URL}/Login/Logout", wait_until="networkidle")
        page.wait_for_timeout(1000)

        # -------------------------------------------------------------
        # 2. TC-SEC-03: Multi-Provinces cho tuantmhcm (NBI, HCM)
        # -------------------------------------------------------------
        print("\n>>> [2/5] TC-SEC-03: Kiểm tra Multi-Provinces cho tuantmhcm (Gán NBI, HCM)")
        page.goto(f"{BASE_URL}/Login/Index", wait_until="networkidle")
        page.locator("input[name='UserName'], input#UserName").first.fill("tuantmhcm")
        page.locator("input[name='Password'], input#Password").first.fill("123123")
        page.locator("button:has-text('Đăng nhập')").first.click()
        page.wait_for_timeout(2000)

        # Kiểm tra trang /BaoCaoCD45/Index
        page.goto(f"{BASE_URL}/BaoCaoCD45/Index", wait_until="networkidle")
        page.wait_for_timeout(2500)
        cd45_options = page.locator("select[x-model='selectedCity'] option").all_inner_texts()
        print(f"   [tuantmhcm] Options tại /BaoCaoCD45/Index: {cd45_options}")
        screenshot_cd45 = os.path.join(SCREENSHOTS_DIR, "tc_sec_03_tuantmhcm_cd45_cities.png")
        page.screenshot(path=screenshot_cd45)

        # Kiểm tra trang /BaoCaoTCVCD45/Index
        page.goto(f"{BASE_URL}/BaoCaoTCVCD45/Index", wait_until="networkidle")
        page.wait_for_timeout(2500)
        tcv_options = page.locator("select[x-model='selectedCity'] option").all_inner_texts()
        print(f"   [tuantmhcm] Options tại /BaoCaoTCVCD45/Index: {tcv_options}")
        screenshot_tcv = os.path.join(SCREENSHOTS_DIR, "tc_sec_03_tuantmhcm_tcv_cities.png")
        page.screenshot(path=screenshot_tcv)

        # Xác thực: Trong options chỉ được chứa NBI, HCM và option mặc định
        cd45_text = " ".join(cd45_options)
        tcv_text = " ".join(tcv_options)
        is_nbi_present = ("NBI" in cd45_text or "Ninh Bình" in cd45_text) and ("NBI" in tcv_text or "Ninh Bình" in tcv_text)
        is_hcm_present = ("HCM" in cd45_text or "Hồ Chí Minh" in cd45_text) and ("HCM" in tcv_text or "Hồ Chí Minh" in tcv_text)
        has_other_provinces = ("Hà Nội" in cd45_text or "HNO" in cd45_text or "Đà Nẵng" in cd45_text or "Cần Thơ" in cd45_text)

        if is_nbi_present and is_hcm_present and not has_other_provinces:
            print("   ✅ TC-SEC-03 PASS: Dropdown của tuantmhcm CHỈ chứa đúng 2 tỉnh NBI và HCM!")
            test_results["TC-SEC-03"] = "PASS"
        else:
            print(f"   ❌ TC-SEC-03 FAIL: Options không khớp kỳ vọng. Other provinces leaked: {has_other_provinces}")
            test_results["TC-SEC-03"] = "FAIL"

        # -------------------------------------------------------------
        # 3. TC-SEC-05: Chặn truy cập trái phép URL quản trị (HasCredential)
        # -------------------------------------------------------------
        print("\n>>> [3/5] TC-SEC-05: Kiểm tra HasCredential chặn URL trái phép đối với tuantmhcm")
        # tuantmhcm chỉ có QUYENBC_DM, không được vào /User/Index hoặc /Role/Index
        page.goto(f"{BASE_URL}/User/Index", wait_until="networkidle")
        page.wait_for_timeout(2000)
        current_url = page.url
        body_text = page.locator("body").inner_text()
        print(f"   [tuantmhcm] Truy cập /User/Index -> URL hiện tại: {current_url}")
        
        is_blocked = ("Error404" in current_url or "AccessDenied" in current_url or "Login" in current_url 
                      or "không có quyền" in body_text.lower() or "error" in body_text.lower() 
                      or page.locator("#gridUser, table#tblUser").count() == 0)
        screenshot_blocked = os.path.join(SCREENSHOTS_DIR, "tc_sec_05_blocked_unauthorized_url.png")
        page.screenshot(path=screenshot_blocked)

        # Chắc chắn rằng không có bảng danh sách người dùng được hiển thị
        user_table_present = page.locator("table#tblUser, input[ng-model='UserName']").count() > 0
        if is_blocked and not user_table_present:
            print("   ✅ TC-SEC-05 PASS: Đã chặn truy cập trái phép vào /User/Index thành công! Không nạp danh sách user.")
            test_results["TC-SEC-05"] = "PASS"
        else:
            print(f"   ❌ TC-SEC-05 FAIL: User không có quyền nhưng vẫn vào được {current_url}")
            test_results["TC-SEC-05"] = "FAIL"

        # Logout
        page.goto(f"{BASE_URL}/Login/Logout", wait_until="networkidle")
        page.wait_for_timeout(1000)

        # -------------------------------------------------------------
        # 4. TC-SEC-02: Phân quyền địa bàn cho tuantm (HNO, BDI, HCM)
        # -------------------------------------------------------------
        print("\n>>> [4/5] TC-SEC-02: Kiểm tra phân quyền địa bàn cho tuantm (HNO, BDI, HCM)")
        page.goto(f"{BASE_URL}/Login/Index", wait_until="networkidle")
        page.locator("input[name='UserName'], input#UserName").first.fill("tuantm")
        page.locator("input[name='Password'], input#Password").first.fill("123123")
        page.locator("button:has-text('Đăng nhập')").first.click()
        page.wait_for_timeout(2000)

        # Màn hình BaoCaoCD45 (chế độ 34 tỉnh mới)
        page.goto(f"{BASE_URL}/BaoCaoCD45/Index", wait_until="networkidle")
        page.wait_for_timeout(2500)
        tuantm_options_34 = page.locator("select[x-model='selectedCity'] option").all_inner_texts()
        print(f"   [tuantm] Options 34 mới tại /BaoCaoCD45/Index: {tuantm_options_34}")
        
        # Chuyển sang chế độ 63 cũ
        page.locator("button:has-text('63 cũ')").click()
        page.wait_for_timeout(2000)
        tuantm_options_63 = page.locator("select[x-model='selectedCity'] option").all_inner_texts()
        print(f"   [tuantm] Options 63 cũ tại /BaoCaoCD45/Index: {tuantm_options_63}")
        
        screenshot_tuantm = os.path.join(SCREENSHOTS_DIR, "tc_sec_02_tuantm_cities.png")
        page.screenshot(path=screenshot_tuantm)

        text_34 = " ".join(tuantm_options_34)
        text_63 = " ".join(tuantm_options_63)

        # tuantm có: HNO, BDI, HCM (HCM gộp từ CBO HC_MYH)
        # 34 mới: HNO, HCM, và GLA (do BDI gộp vào GLA theo NQ 202)
        # 63 cũ: HNO, HCM, BDI
        # Tuyệt đối không có NBI (Ninh Bình) hay DAN (Đà Nẵng)
        pass_34 = ("HNO" in text_34 and "HCM" in text_34 and "GLA" in text_34 and "NBI" not in text_34)
        pass_63 = ("HNO" in text_63 and "HCM" in text_63 and "BDI" in text_63 and "NBI" not in text_63)

        if pass_34 and pass_63:
            print("   ✅ TC-SEC-02 PASS: tuantm hiển thị đúng chính xác địa bàn ở cả 2 chế độ (34 mới và 63 cũ), không rò rỉ tỉnh ngoài!")
            test_results["TC-SEC-02"] = "PASS"
        else:
            print(f"   ❌ TC-SEC-02 FAIL: Pass 34={pass_34}, Pass 63={pass_63}")
            test_results["TC-SEC-02"] = "FAIL"

        # Logout
        page.goto(f"{BASE_URL}/Login/Logout", wait_until="networkidle")
        page.wait_for_timeout(1000)

        # -------------------------------------------------------------
        # 5. TC-SEC-04: Super Admin xem toàn bộ danh mục tỉnh (admin)
        # -------------------------------------------------------------
        print("\n>>> [5/5] TC-SEC-04: Kiểm tra tài khoản Super Admin xem toàn bộ tỉnh (admin)")
        page.goto(f"{BASE_URL}/Login/Index", wait_until="networkidle")
        page.locator("input[name='UserName'], input#UserName").first.fill("admin")
        page.locator("input[name='Password'], input#Password").first.fill("123456789a@")
        page.locator("button:has-text('Đăng nhập')").first.click()
        page.wait_for_timeout(2000)

        page.goto(f"{BASE_URL}/BaoCaoCD45/Index", wait_until="networkidle")
        page.wait_for_timeout(2500)
        admin_options = page.locator("select[x-model='selectedCity'] option").all_inner_texts()
        print(f"   [admin] Số lượng options tỉnh (34 mới): {len(admin_options)}")
        
        # Test 63 cũ với admin
        page.locator("button:has-text('63 cũ')").click()
        page.wait_for_timeout(2000)
        admin_options_63 = page.locator("select[x-model='selectedCity'] option").all_inner_texts()
        print(f"   [admin] Số lượng options tỉnh (63 cũ): {len(admin_options_63)}")

        screenshot_admin = os.path.join(SCREENSHOTS_DIR, "tc_sec_04_admin_full_cities.png")
        page.screenshot(path=screenshot_admin)

        if len(admin_options) >= 34 and len(admin_options_63) >= 63:
            print(f"   ✅ TC-SEC-04 PASS: Admin nạp đầy đủ toàn bộ {len(admin_options)} tỉnh mới và {len(admin_options_63)} tỉnh cũ!")
            test_results["TC-SEC-04"] = "PASS"
        else:
            print(f"   ❌ TC-SEC-04 FAIL: Admin chỉ thấy {len(admin_options)} options mới, {len(admin_options_63)} options cũ.")
            test_results["TC-SEC-04"] = "FAIL"

        browser.close()

    print("\n" + "=" * 80)
    print("                    KẾT QUẢ TỔNG HỢP KIỂM THỬ NHÓM 1")
    print("=" * 80)
    for tc, res in test_results.items():
        print(f"   - {tc}: {res}")
    
    all_pass = all(res == "PASS" for res in test_results.values()) and len(test_results) == 5
    if all_pass:
        print("\n🎉 TẤT CẢ 5/5 TEST CASES NHÓM 1 ĐÃ PASS 100%!")
    else:
        print("\n⚠️ CÓ TEST CASE CHƯA ĐẠT. VUI LÒNG KIỂM TRA LẠI LOG.")
    print("=" * 80)

if __name__ == "__main__":
    run_tests()
