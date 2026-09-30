import os
import sys

try:
    sys.stdout.reconfigure(encoding='utf-8')
    sys.stderr.reconfigure(encoding='utf-8')
except Exception:
    pass

from playwright.sync_api import sync_playwright

BASE_URL = 'https://localhost:44374'
SCREENSHOTS_DIR = r'D:\Projects\BVTL-X\tests\screenshots'
os.makedirs(SCREENSHOTS_DIR, exist_ok=True)

with sync_playwright() as p:
    browser = p.chromium.launch(channel='chrome', headless=False, slow_mo=300)
    page = browser.new_page(ignore_https_errors=True, viewport={'width': 1280, 'height': 850})
    
    # 1. Login tuantmhcm first
    print("1. Đăng nhập tuantmhcm...")
    page.goto(f"{BASE_URL}/Login/Index", wait_until="networkidle")
    page.locator("input[name='UserName']").fill("tuantmhcm")
    page.locator("input[name='Password']").fill("123123")
    page.locator("button:has-text('Đăng nhập')").click()
    page.wait_for_timeout(2000)
    
    # 2. Try accessing restricted User/Index -> redirected to Error404 / AccessDenied
    print("2. Truy cập /ErrorPage/Error404...")
    page.goto(f"{BASE_URL}/ErrorPage/Error404", wait_until="networkidle")
    page.wait_for_timeout(2000)
    
    img_path = os.path.join(SCREENSHOTS_DIR, 'new_permission_notice_page.png')
    page.screenshot(path=img_path)
    print("Screenshot saved at:", img_path)
    
    # Copy to artifacts directory
    artifact_dir = r'C:\Users\TUANTM\.gemini\antigravity\brain\2c818e19-0f91-40fc-af44-68cafda58dda\screenshots'
    os.makedirs(artifact_dir, exist_ok=True)
    import shutil
    shutil.copy(img_path, os.path.join(artifact_dir, 'new_permission_notice_page.png'))
    print("Copied to artifacts screenshots directory.")
    browser.close()
