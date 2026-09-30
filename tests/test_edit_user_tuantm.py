"""
Automated Test for Edit User Popup (user 'tuantm' ID 243)
Modifying Tỉnh/Thành phố quản lý
"""
import os
import sys
import time
import pyodbc

try:
    sys.stdout.reconfigure(encoding='utf-8')
    sys.stderr.reconfigure(encoding='utf-8')
except Exception:
    pass

from playwright.sync_api import sync_playwright

BASE_URL = "https://localhost:44374"
SCREENSHOTS_DIR = os.path.join(os.path.dirname(os.path.abspath(__file__)), "screenshots")
os.makedirs(SCREENSHOTS_DIR, exist_ok=True)

def run():
    print("==========================================================================")
    print("   TEST POPUP SỬA THÔNG TIN NGƯỜI DÙNG: USER 'tuantm'                    ")
    print("==========================================================================")

    conn = pyodbc.connect('DRIVER={ODBC Driver 17 for SQL Server};SERVER=103.77.167.206;DATABASE=BVTL_REPORTING_DEV;UID=sa;PWD=GLy74MwZ;')
    cursor = conn.cursor()
    cursor.execute("SELECT ID, UserName, CityCodes, MaDuAn FROM BVTL_QT_NGUOI_DUNG WHERE UserName = 'tuantm'")
    row = cursor.fetchone()
    print(f"Trạng thái DB ban đầu của tuantm: ID={row[0]}, CityCodes={row[2]}, MaDuAn={row[3]}")
    user_id = row[0]

    with sync_playwright() as p:
        browser = p.chromium.launch(channel="chrome", headless=False, slow_mo=500)
        context = browser.new_context(ignore_https_errors=True)
        page = context.new_page()

        # Login admin
        print("\n1. Đăng nhập admin...")
        page.goto(f"{BASE_URL}/Login/Index", wait_until="networkidle")
        page.locator("input[name='UserName'], input#UserName").first.fill("admin")
        page.locator("input[name='Password'], input#Password").first.fill("123456789a@")
        page.locator("button:has-text('Đăng nhập')").first.click()
        page.wait_for_timeout(2000)

        # Vào trang Quản lý người dùng
        print("\n2. Mở trang Quản lý người dùng (/User/Index)...")
        page.goto(f"{BASE_URL}/User/Index", wait_until="networkidle")
        page.wait_for_timeout(2500)

        # Tìm user tuantm
        print("\n3. Tìm kiếm tài khoản 'tuantm'...")
        search_input = page.locator("input[x-model='modelSearch.KeyWord']").first
        if search_input.count() > 0:
            search_input.fill("tuantm")
            page.wait_for_timeout(500)
            page.locator("button[x-on\\:click='search()'], button:has-text('Tìm')").first.click()
            page.wait_for_timeout(2500)

        # Bấm nút Sửa cho tuantm (ID 243)
        print("\n4. Bấm nút Sửa (icon cây bút) của user tuantm...")
        page.evaluate("() => { const alp = window.Alpine.$data(document.querySelector('[x-data]')); if (alp && alp.edit) alp.edit(243); }")
        page.wait_for_timeout(3000)

        # Kiểm tra dữ liệu trong Angular Scope
        print("\n5. Kiểm tra dữ liệu nạp vào form Edit của tuantm...")
        eval_script = """
        () => {
            const el = document.querySelector("form[name='formDemo']") || document.querySelector("select[name='ListCityCode']");
            if (!el) return null;
            const scope = angular.element(el).scope();
            if (!scope) return null;
            return {
                UserName: scope.model ? scope.model.UserName : null,
                Name: scope.model ? scope.model.Name : null,
                ListMaDuAn: scope.ListMaDuAn,
                ListCityCode: scope.ListCityCode,
                ListTestGroupId: scope.ListTestGroupId
            };
        }
        """
        scope_data = page.evaluate(eval_script)
        print(f"Dữ liệu trong Angular Scope ban đầu: {scope_data}")

        # Đổi tỉnh quản lý thành ['HNO', 'BDI'] bằng cách cập nhật scope.ListCityCode và scope.$apply()
        print("\n6. Đổi tỉnh quản lý thành ['HNO', 'BDI']...")
        change_city_script = """
        () => {
            const el = document.querySelector("select[name='ListCityCode']");
            if (!el) return { success: false, msg: 'No select element' };
            const scope = angular.element(el).scope();
            if (!scope) return { success: false, msg: 'No scope' };

            // Cập nhật scope
            scope.ListCityCode = ['HNO', 'BDI'];

            if (!scope.ListMaDuAn || scope.ListMaDuAn.length === 0) {
                scope.ListMaDuAn = ['CD45'];
            }

            scope.$apply();
            return {
                success: true,
                newCityCode: scope.ListCityCode
            };
        }
        """
        change_res = page.evaluate(change_city_script)
        print(f"Kết quả set tỉnh: {change_res}")
        page.wait_for_timeout(1500)
        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "edit_03_city_changed.png"))

        # Bấm nút Cập nhật
        print("\n7. Bấm nút 'Cập nhật người dùng'...")
        modal = page.locator(".modal-dialog .modal-content").last
        submit_btn = modal.locator("button:has-text('Cập nhật người dùng')").first
        submit_btn.click()
        page.wait_for_timeout(3500)
        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "edit_04_after_submit.png"))

        browser.close()

    # 8. Kiểm tra CSDL sau khi Submit
    print("\n8. Kiểm tra CSDL sau khi submit:")
    cursor.execute("SELECT ID, UserName, CityCodes, MaDuAn FROM BVTL_QT_NGUOI_DUNG WHERE ID = ?", user_id)
    updated_user = cursor.fetchone()
    print(f"BVTL_QT_NGUOI_DUNG -> CityCodes mới: {updated_user[2]}")

    cursor.execute("SELECT CityCode FROM BVTL_QT_NGUOI_DUNG_CITY WHERE NguoiDungId = ? AND IsActive = 1", user_id)
    cities_in_table = [r[0] for r in cursor.fetchall()]
    print(f"BVTL_QT_NGUOI_DUNG_CITY -> Tỉnh mới: {cities_in_table}")

    conn.close()

    is_pass = False
    # Kiểm tra BDI và HNO có mặt trong cả chuỗi CityCodes và bảng BVTL_QT_NGUOI_DUNG_CITY
    if updated_user[2] and 'BDI' in updated_user[2] and 'HNO' in updated_user[2]:
        if 'BDI' in cities_in_table and 'HNO' in cities_in_table:
            is_pass = True

    if is_pass:
        print("\n===> KẾT QUẢ TEST SỬA TỈNH USER 'tuantm': ✅ PASS HOÀN TOÀN (100%)!")
    else:
        print(f"\n===> KẾT QUẢ TEST SỬA TỈNH USER 'tuantm': ❌ FAIL! DB: CityCodes={updated_user[2]}, Table={cities_in_table}")

if __name__ == "__main__":
    run()
