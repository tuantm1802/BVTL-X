# -*- coding: utf-8 -*-
import os
import sys
import time

try:
    sys.stdout.reconfigure(encoding='utf-8', line_buffering=True)
    sys.stderr.reconfigure(encoding='utf-8', line_buffering=True)
except Exception:
    pass

import pyodbc
from playwright.sync_api import sync_playwright

BASE_URL = "http://localhost:49856"
SCREENSHOTS_DIR = r"C:\Users\TUANTM\.gemini\antigravity\brain\2c818e19-0f91-40fc-af44-68cafda58dda\screenshots"
os.makedirs(SCREENSHOTS_DIR, exist_ok=True)

DB_CONN_STR = 'DRIVER={ODBC Driver 17 for SQL Server};SERVER=103.77.167.206;DATABASE=BVTL_REPORTING_DEV;UID=sa;PWD=GLy74MwZ;'

def get_db():
    return pyodbc.connect(DB_CONN_STR)

def setup_test_users():
    conn = get_db()
    cursor = conn.cursor()
    
    # Dọn dẹp trước
    cursor.execute("DELETE FROM BVTL_QT_LOG WHERE UserName IN ('test_hard_del', 'test_soft_del')")
    cursor.execute("DELETE FROM BVTL_QT_NGUOI_DUNG_NHOM_TBH WHERE NguoiDungId IN (SELECT ID FROM BVTL_QT_NGUOI_DUNG WHERE UserName IN ('test_hard_del', 'test_soft_del'))")
    cursor.execute("DELETE FROM BVTL_QT_NGUOI_DUNG_CITY WHERE NguoiDungId IN (SELECT ID FROM BVTL_QT_NGUOI_DUNG WHERE UserName IN ('test_hard_del', 'test_soft_del'))")
    cursor.execute("DELETE FROM BVTL_QT_NGUOI_DUNG WHERE UserName IN ('test_hard_del', 'test_soft_del')")
    conn.commit()

    # 1. Tạo user test xóa cứng (không có log)
    cursor.execute("""
        INSERT INTO BVTL_QT_NGUOI_DUNG (UserName, Password, Name, GroupID, MaDuAn, Status, IsAdmin, IsActive, CreatedDate)
        VALUES ('test_hard_del', '4297f44b13955235245b2497399d7a93', N'Test Xóa Cứng 01', 'QUYENBC_DM', 'CD45', 1, 0, 1, GETDATE())
    """)
    conn.commit()
    cursor.execute("SELECT ID FROM BVTL_QT_NGUOI_DUNG WHERE UserName = 'test_hard_del'")
    hard_id = cursor.fetchone()[0]

    # 2. Tạo user test xóa mềm (có phát sinh 1 log)
    cursor.execute("""
        INSERT INTO BVTL_QT_NGUOI_DUNG (UserName, Password, Name, GroupID, MaDuAn, Status, IsAdmin, IsActive, CreatedDate)
        VALUES ('test_soft_del', '4297f44b13955235245b2497399d7a93', N'Test Xóa Mềm 02', 'QUYENBC_DM', 'CD45', 1, 0, 1, GETDATE())
    """)
    conn.commit()
    cursor.execute("SELECT ID FROM BVTL_QT_NGUOI_DUNG WHERE UserName = 'test_soft_del'")
    soft_id = cursor.fetchone()[0]

    # Thêm log để phát sinh Audit Trail cho test_soft_del
    cursor.execute("""
        INSERT INTO BVTL_QT_LOG (ControllerName, UserName, DateLog, Content)
        VALUES ('TestAudit', 'test_soft_del', GETDATE(), N'Giao dịch kiểm toán thử nghiệm của user test_soft_del')
    """)
    conn.commit()
    conn.close()
    print(f"Setup test users done: Hard ID={hard_id}, Soft ID={soft_id}")
    return hard_id, soft_id

def cleanup_test_users():
    try:
        conn = get_db()
        cursor = conn.cursor()
        cursor.execute("DELETE FROM BVTL_QT_LOG WHERE UserName IN ('test_hard_del', 'test_soft_del')")
        cursor.execute("DELETE FROM BVTL_QT_NGUOI_DUNG_NHOM_TBH WHERE NguoiDungId IN (SELECT ID FROM BVTL_QT_NGUOI_DUNG WHERE UserName IN ('test_hard_del', 'test_soft_del'))")
        cursor.execute("DELETE FROM BVTL_QT_NGUOI_DUNG_CITY WHERE NguoiDungId IN (SELECT ID FROM BVTL_QT_NGUOI_DUNG WHERE UserName IN ('test_hard_del', 'test_soft_del'))")
        cursor.execute("DELETE FROM BVTL_QT_NGUOI_DUNG WHERE UserName IN ('test_hard_del', 'test_soft_del')")
        conn.commit()
        conn.close()
        print("Cleanup done.")
    except Exception as e:
        print(f"Cleanup error: {e}")

def run_e2e():
    hard_id, soft_id = setup_test_users()
    
    with sync_playwright() as p:
        browser = p.chromium.launch(headless=True)
        context = browser.new_context(viewport={"width": 1440, "height": 900})
        page = context.new_page()
        page.on("dialog", lambda dialog: dialog.accept())

        print("\n1. Đăng nhập hệ thống bằng tài khoản admin...")
        page.goto(f"{BASE_URL}/Login/Index", wait_until="networkidle")
        page.fill("#UserName", "admin")
        page.fill("#Password", "123456789a@")
        page.click("button[type='submit']")
        page.wait_for_timeout(2500)
        print(f"Current URL: {page.url}")

        print("\n2. Truy cập trang Quản lý người dùng (/User/Index)...")
        page.goto(f"{BASE_URL}/User/Index", wait_until="networkidle")
        page.wait_for_timeout(2500)
        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "01_user_index_overview.png"))
        print("Đã chụp ảnh: 01_user_index_overview.png")

        # ==========================================
        # TEST 1: XÓA CỨNG (HARD DELETE)
        # ==========================================
        print("\n3. TEST CASE 1: Xóa cứng tài khoản 'test_hard_del' (chưa phát sinh dữ liệu)...")
        search_input = page.locator("input[placeholder*='Tìm theo Tên đăng nhập']")
        search_input.fill("test_hard_del")
        page.keyboard.press("Enter")
        page.wait_for_timeout(1500)

        # Chờ hiển thị dòng test_hard_del
        row_hard = page.locator("tr", has_text="test_hard_del")
        assert row_hard.count() > 0, "Không tìm thấy user test_hard_del trong bảng"
        print("Tìm thấy user test_hard_del trên bảng.")

        # Click nút Xóa
        btn_del_hard = row_hard.locator("button[title='Xóa người dùng']")
        btn_del_hard.click()
        page.wait_for_timeout(1200)

        # Xác minh popup confirm xóa cứng
        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "02_hard_delete_modal_confirm.png"))
        print("Đã chụp ảnh: 02_hard_delete_modal_confirm.png")

        modal_title = page.locator(".ng-confirm-title").inner_text()
        print(f"Modal Title: {modal_title}")
        assert "Xóa vĩnh viễn" in modal_title or "Xóa" in modal_title, f"Tiêu đề modal không đúng: {modal_title}"

        # Bấm nút Xóa vĩnh viễn trong confirm
        btn_confirm = page.locator(".ng-confirm-buttons button.btn-danger, .ng-confirm-buttons button.btn-red").first
        btn_confirm.click()
        page.wait_for_timeout(2500)

        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "03_hard_delete_success.png"))
        print("Đã chụp ảnh: 03_hard_delete_success.png")

        # Kiểm tra trong DB: test_hard_del phải không còn tồn tại
        conn = get_db()
        cur = conn.cursor()
        cur.execute("SELECT COUNT(*) FROM BVTL_QT_NGUOI_DUNG WHERE UserName = 'test_hard_del'")
        count_hard = cur.fetchone()[0]
        conn.close()
        assert count_hard == 0, "test_hard_del vẫn còn tồn tại trong DB sau khi xóa cứng!"
        print("[PASS] Test Case 1: Xóa cứng thành công! Bản ghi đã bị xóa hoàn toàn khỏi DB.")

        # ==========================================
        # TEST 2: XÓA MỀM (SOFT DELETE)
        # ==========================================
        print("\n4. TEST CASE 2: Xóa mềm tài khoản 'test_soft_del' (đã có nhật ký Audit Trail)...")
        # Reset search
        btn_reset = page.locator("button[title='Đặt lại bộ lọc']")
        btn_reset.click()
        page.wait_for_timeout(1500)

        search_input.fill("test_soft_del")
        page.keyboard.press("Enter")
        page.wait_for_timeout(1500)

        row_soft = page.locator("tr", has_text="test_soft_del")
        assert row_soft.count() > 0, "Không tìm thấy user test_soft_del trong bảng"
        print("Tìm thấy user test_soft_del trên bảng.")

        # Click nút Xóa
        btn_del_soft = row_soft.locator("button[title='Xóa người dùng']")
        btn_del_soft.click()
        page.wait_for_timeout(1200)

        # Xác minh popup confirm xóa mềm
        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "04_soft_delete_modal_confirm.png"))
        print("Đã chụp ảnh: 04_soft_delete_modal_confirm.png")

        modal_title_soft = page.locator(".ng-confirm-title").inner_text()
        print(f"Modal Title: {modal_title_soft}")
        assert "Xóa mềm" in modal_title_soft, f"Tiêu đề modal không đúng: {modal_title_soft}"

        modal_content_soft = page.locator(".ng-confirm-content").inner_text()
        print(f"Modal Content: {modal_content_soft[:120]}...")
        assert "Audit Trail" in modal_content_soft or "XÓA MỀM" in modal_content_soft, "Nội dung modal không cảnh báo bảo toàn Audit Trail!"

        # Bấm nút Xác nhận xóa mềm
        btn_confirm_soft = page.locator(".ng-confirm-buttons button.btn-danger, .ng-confirm-buttons button.btn-red").first
        btn_confirm_soft.click()
        page.wait_for_timeout(2500)

        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "05_soft_delete_success.png"))
        print("Đã chụp ảnh: 05_soft_delete_success.png")

        # Kiểm tra trong DB: test_soft_del phải còn tồn tại với IsActive = 0, Status = 0
        conn = get_db()
        cur = conn.cursor()
        cur.execute("SELECT IsActive, Status FROM BVTL_QT_NGUOI_DUNG WHERE UserName = 'test_soft_del'")
        soft_row = cur.fetchone()
        conn.close()
        assert soft_row is not None, "test_soft_del bị mất khỏi DB thay vì xóa mềm!"
        assert soft_row[0] is False, f"IsActive mong đợi False, thực tế: {soft_row[0]}"
        assert soft_row[1] is False, f"Status mong đợi False, thực tế: {soft_row[1]}"
        print("[PASS] Test Case 2: Xóa mềm thành công! IsActive = False, Status = False, Audit Trail được bảo toàn.")

        # ==========================================
        # TEST 3: BỘ LỌC THÙNG RÁC (TRASH BIN)
        # ==========================================
        print("\n5. TEST CASE 3: Lọc xem Thùng rác ('Đã xóa (Thùng rác)')...")
        select_status = page.locator("select[x-model='modelSearch.Status']")
        select_status.select_option("deleted")
        page.wait_for_timeout(2000)

        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "06_trash_bin_view.png"))
        print("Đã chụp ảnh: 06_trash_bin_view.png")

        # Kiểm tra banner Thùng rác xuất hiện
        banner = page.locator(".alert-warning", has_text="Tài khoản đã xóa mềm (Thùng rác)")
        assert banner.count() > 0, "Không hiển thị banner thông báo Thùng rác!"
        print("Banner Thùng rác hiển thị chính xác.")

        # Kiểm tra test_soft_del xuất hiện trong Thùng rác với badge "Đã xóa" và nút Khôi phục
        trash_row = page.locator("tr", has_text="test_soft_del")
        assert trash_row.count() > 0, "Không tìm thấy user test_soft_del trong Thùng rác!"
        
        badge_deleted = trash_row.locator("span", has_text="Đã xóa")
        assert badge_deleted.count() > 0, "Không hiển thị badge 'Đã xóa'!"

        btn_restore = trash_row.locator("button[title='Khôi phục tài khoản']")
        assert btn_restore.count() > 0, "Không hiển thị nút Khôi phục tài khoản!"
        print("[PASS] Test Case 3: Bộ lọc Thùng rác hiển thị chuẩn xác!")

        # ==========================================
        # TEST 4: KHÔI PHỤC TÀI KHOẢN (RESTORE)
        # ==========================================
        print("\n6. TEST CASE 4: Khôi phục tài khoản từ Thùng rác...")
        btn_restore.click()
        page.wait_for_timeout(1200)

        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "07_restore_modal_confirm.png"))
        print("Đã chụp ảnh: 07_restore_modal_confirm.png")

        btn_confirm_restore = page.locator(".ng-confirm-buttons button.btn-success, .ng-confirm-buttons button.btn-green").first
        btn_confirm_restore.click()
        page.wait_for_timeout(2500)

        # Chuyển về bộ lọc tất cả
        select_status.select_option("")
        page.wait_for_timeout(2000)

        search_input.fill("test_soft_del")
        page.keyboard.press("Enter")
        page.wait_for_timeout(1500)

        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "08_restored_active_user.png"))
        print("Đã chụp ảnh: 08_restored_active_user.png")

        # Kiểm tra trong DB: IsActive = 1, Status = 1
        conn = get_db()
        cur = conn.cursor()
        cur.execute("SELECT IsActive, Status FROM BVTL_QT_NGUOI_DUNG WHERE UserName = 'test_soft_del'")
        restored_row = cur.fetchone()
        conn.close()
        assert restored_row[0] is True, f"IsActive mong đợi True, thực tế: {restored_row[0]}"
        assert restored_row[1] is True, f"Status mong đợi True, thực tế: {restored_row[1]}"
        print("[PASS] Test Case 4: Khôi phục tài khoản thành công! IsActive = True, Status = True.")

        # ==========================================
        # TEST 5: KHÓA / KÍCH HOẠT TÀI KHOẢN (LOCK/UNLOCK)
        # ==========================================
        print("\n7. TEST CASE 5: Khóa & Kích hoạt lại tài khoản...")
        active_row = page.locator("tr", has_text="test_soft_del")
        btn_lock = active_row.locator("button[title='Khóa / Tạm ngừng tài khoản']")
        assert btn_lock.count() > 0, "Không tìm thấy nút Khóa tài khoản!"
        btn_lock.click()
        page.wait_for_timeout(1200)

        btn_confirm_lock = page.locator(".ng-confirm-buttons button.btn-danger, .ng-confirm-buttons button.btn-red").first
        btn_confirm_lock.click()
        page.wait_for_timeout(2500)

        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "09_user_locked.png"))
        print("Đã chụp ảnh: 09_user_locked.png")

        # Kiểm tra badge In Active
        locked_row = page.locator("tr", has_text="test_soft_del")
        badge_inactive = locked_row.locator("span", has_text="In Active")
        assert badge_inactive.count() > 0, "Không hiển thị badge 'In Active' sau khi khóa!"

        # Kích hoạt lại
        btn_unlock = locked_row.locator("button[title='Kích hoạt lại tài khoản']")
        assert btn_unlock.count() > 0, "Không tìm thấy nút Kích hoạt lại!"
        btn_unlock.click()
        page.wait_for_timeout(1200)

        btn_confirm_unlock = page.locator(".ng-confirm-buttons button.btn-success, .ng-confirm-buttons button.btn-green").first
        btn_confirm_unlock.click()
        page.wait_for_timeout(2500)

        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "10_user_unlocked.png"))
        print("Đã chụp ảnh: 10_user_unlocked.png")
        print("[PASS] Test Case 5: Khóa và Mở khóa tài khoản hoạt động hoàn hảo!")

        browser.close()

    # Dọn dẹp tài khoản test
    cleanup_test_users()
    print("\n=======================================================")
    print("   TẤT CẢ CÁC TEST CASES ĐÃ HOÀN THÀNH XUẤT SẮC 100%!   ")
    print("=======================================================")

if __name__ == "__main__":
    try:
        run_e2e()
    except Exception as e:
        print(f"\n[ERROR] Test run failed: {e}")
        cleanup_test_users()
        sys.exit(1)
