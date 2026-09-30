"""
Check what cities dropdown displays for tuantmhcm in BaoCaoCD45 and other pages
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

with sync_playwright() as p:
    browser = p.chromium.launch(channel="chrome", headless=False, slow_mo=500)
    page = browser.new_page(ignore_https_errors=True)

    print("1. Đăng nhập tuantmhcm...")
    page.goto(f"{BASE_URL}/Login/Index", wait_until="networkidle")
    page.locator("input[name='UserName']").fill("tuantmhcm")
    page.locator("input[name='Password']").fill("123123")
    page.locator("button:has-text('Đăng nhập')").click()
    page.wait_for_timeout(2500)

    # 2. Mở Báo cáo CD45
    print("\n2. Mở /BaoCaoCD45/Index...")
    page.goto(f"{BASE_URL}/BaoCaoCD45/Index", wait_until="networkidle")
    page.wait_for_timeout(3000)
    page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "tuantmhcm_01_baocao_cd45.png"))

    # Lấy danh sách options trong select Tỉnh của BaoCaoCD45
    cities_cd45 = page.locator("select[id*='city'], select[name*='city'], select[x-model*='City'], select[x-model*='city'] option").all_inner_texts()
    print(f"Số lượng options tỉnh tìm thấy trong BaoCaoCD45: {len(cities_cd45)}")
    print(f"Danh sách tỉnh trong BaoCaoCD45: {cities_cd45[:15]}")

    # 3. Mở Báo cáo TCV CD45
    print("\n3. Mở /BaoCaoTCVCD45/Index...")
    page.goto(f"{BASE_URL}/BaoCaoTCVCD45/Index", wait_until="networkidle")
    page.wait_for_timeout(3000)
    page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "tuantmhcm_02_baocao_tcv.png"))
    cities_tcv = page.locator("select[id*='city'], select[name*='city'], select[x-model*='City'], select[x-model*='city'] option").all_inner_texts()
    print(f"Số lượng options tỉnh tìm thấy trong BaoCaoTCVCD45: {len(cities_tcv)}")
    print(f"Danh sách tỉnh trong BaoCaoTCVCD45: {cities_tcv}")

    # 4. Mở Báo cáo tháng (nếu có thể vào)
    print("\n4. Thử mở /BaoCaoThang/Index...")
    page.goto(f"{BASE_URL}/BaoCaoThang/Index", wait_until="networkidle")
    page.wait_for_timeout(3000)
    page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "tuantmhcm_03_baocaothang.png"))
    cities_bcthang = page.locator("select[name='ListCity'] option").all_inner_texts()
    print(f"Số lượng options tỉnh tìm thấy trong BaoCaoThang: {len(cities_bcthang)}")
    print(f"Danh sách tỉnh trong BaoCaoThang: {cities_bcthang}")

    browser.close()
