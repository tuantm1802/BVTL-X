import os
import sys

try:
    sys.stdout.reconfigure(encoding='utf-8')
    sys.stderr.reconfigure(encoding='utf-8')
except Exception:
    pass

from playwright.sync_api import sync_playwright

BASE_URL = 'https://localhost:44374'
with sync_playwright() as p:
    browser = p.chromium.launch(channel='chrome', headless=False, slow_mo=300)
    page = browser.new_page(ignore_https_errors=True, viewport={'width': 1280, 'height': 850})
    
    # 1. Login admin
    print("1. Đăng nhập admin...")
    page.goto(f"{BASE_URL}/Login/Index", wait_until="networkidle")
    page.locator("input[name='UserName']").fill("admin")
    page.locator("input[name='Password']").fill("123456789a@")
    page.locator("button:has-text('Đăng nhập')").click()
    page.wait_for_timeout(2000)
    
    # 2. Open user list
    print("2. Mở /User/Index...")
    page.goto(f"{BASE_URL}/User/Index", wait_until="networkidle")
    page.wait_for_timeout(2000)
    
    # 3. Click Edit on user tuantm (ID 243) or admin3
    print("3. Mở popup Sửa thông qua alp.edit...")
    page.evaluate("() => { const alp = window.Alpine.$data(document.querySelector('[x-data]')); if (alp && alp.edit) alp.edit(243); }")
    page.wait_for_timeout(3000)
    
    # 4. Clear Name field
    print("4. Xóa Họ và tên...")
    name_input = page.locator("input[name='Name']")
    name_input.fill("")
    page.wait_for_timeout(500)
    
    # 5. Click Cập nhật button
    print("5. Bấm Cập nhật để kích hoạt validate...")
    save_btn = page.locator("button[ng-click*='submit']").first
    save_btn.click()
    page.wait_for_timeout(1500)
    
    # 6. Take screenshot
    os.makedirs('D:/Projects/BVTL-X/tests/screenshots', exist_ok=True)
    screenshot_path = 'D:/Projects/BVTL-X/tests/screenshots/reproduce_validation_error.png'
    page.screenshot(path=screenshot_path)
    print("Screenshot saved:", screenshot_path)
    
    # Copy to artifacts directory
    artifact_dir = r'C:\Users\TUANTM\.gemini\antigravity\brain\2c818e19-0f91-40fc-af44-68cafda58dda\screenshots'
    os.makedirs(artifact_dir, exist_ok=True)
    import shutil
    shutil.copy(screenshot_path, os.path.join(artifact_dir, 'reproduce_validation_error.png'))
    
    # 7. Get HTML & styles of error element
    err_info = page.evaluate('''() => {
        const el = document.querySelector('#Name-error');
        if (!el) return { error: 'Not found' };
        const matched = [];
        for (const sheet of document.styleSheets) {
            try {
                for (const rule of sheet.cssRules || []) {
                    if (rule.selectorText && el.matches(rule.selectorText)) {
                        matched.push({
                            sheet: sheet.href ? sheet.href.split('/').pop() : 'inline',
                            selector: rule.selectorText,
                            cssText: rule.cssText
                        });
                    }
                }
            } catch (e) {}
        }
        return {
            fontSize: window.getComputedStyle(el).fontSize,
            lineHeight: window.getComputedStyle(el).lineHeight,
            color: window.getComputedStyle(el).color,
            width: window.getComputedStyle(el).width,
            matchedRules: matched
        };
    }''')
    import pprint
    pprint.pprint(err_info)
    browser.close()
