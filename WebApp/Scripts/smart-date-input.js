/**
 * Smart Date Input Formatter & Bootstrap Datepicker Sync
 * Tự động format thông minh dd/mm/yyyy khi gõ bàn phím:
 * - Gõ 4..9 ở vị trí ngày -> tự động thành 04/..09/
 * - Gõ 1..3 ở vị trí ngày: nếu gõ tiếp số -> 1x/..3x/, nếu gõ '/' hoặc dừng 500ms hoặc rời ô -> tự động thành 01/..03/
 * - Gõ 2..9 ở vị trí tháng -> tự động thành 02/..09/
 * - Gõ 1 ở vị trí tháng: nếu gõ 0..2 -> 10/..12/, nếu gõ '/' hoặc dừng 500ms -> 01/
 * - Dán hoặc nhập chuỗi như 1/9/2026, 1-9-26 -> tự động chuẩn hóa 01/09/2026
 * - Phím Backspace xóa mượt mà không bị kẹt dấu '/'
 * - Đồng bộ hai chiều với Alpine.js state (fromDate, toDate) và Bootstrap Datepicker
 */
(function (window, $) {
    'use strict';

    var isSyncing = false;

    // Đồng bộ giá trị vào Alpine.js model
    function syncAlpineDate(input, val) {
        if (!input || isSyncing) return;
        isSyncing = true;
        try {
            // Tìm scope Alpine cha gần nhất
            var alpineEl = input.closest ? input.closest('[x-data]') : null;
            if (alpineEl && alpineEl._x_dataStack && alpineEl._x_dataStack.length > 0) {
                var comp = alpineEl._x_dataStack[0];
                var modelName = input.getAttribute('x-model');
                if (modelName && comp && comp[modelName] !== undefined) {
                    comp[modelName] = val;
                }
                if (typeof comp.validateDateRange === 'function') {
                    var valid = comp.validateDateRange(false);
                    if (valid && typeof comp.reloadData === 'function' && alpineEl.getAttribute('x-data') === 'alpineBaoCaoTCVCD45') {
                        comp.reloadData();
                    }
                }
            }

            // Phát sự kiện DOM tiêu chuẩn để Alpine.js binding cập nhật
            var evInput = new Event('input', { bubbles: true });
            input.dispatchEvent(evInput);
            var evChange = new Event('change', { bubbles: true });
            input.dispatchEvent(evChange);
        } finally {
            isSyncing = false;
        }
    }

    // Chuẩn hóa chuỗi ngày tháng d/m/yyyy -> dd/mm/yyyy
    function normalizeDateString(raw) {
        if (!raw) return '';
        var clean = raw.trim().replace(/[-.]/g, '/');
        var parts = clean.split('/');
        if (parts.length >= 2) {
            var d = parseInt(parts[0], 10);
            var m = parseInt(parts[1], 10);
            var y = parts[2] !== undefined ? parts[2].trim() : '';
            if (isNaN(d) || isNaN(m)) return raw;
            d = Math.max(1, Math.min(31, d));
            m = Math.max(1, Math.min(12, m));
            var dd = d < 10 ? '0' + d : '' + d;
            var mm = m < 10 ? '0' + m : '' + m;
            if (parts.length >= 3 && y.length > 0) {
                var yNum = parseInt(y, 10);
                if (!isNaN(yNum)) {
                    if (y.length <= 2) {
                        yNum = yNum < 50 ? 2000 + yNum : 1900 + yNum;
                    }
                    return dd + '/' + mm + '/' + yNum;
                }
                return dd + '/' + mm + '/' + y;
            }
            return dd + '/' + mm + '/';
        }
        return raw;
    }

    // Thiết lập smart date input cho 1 phần tử
    function setupSmartDateInput(input) {
        if (!input || input._smartDateAttached) return;
        input._smartDateAttached = true;

        var timer = null;
        var isDeleting = false;

        function clearDebounce() {
            if (timer) {
                clearTimeout(timer);
                timer = null;
            }
        }

        function setCursor(pos) {
            setTimeout(function () {
                if (input.setSelectionRange) {
                    input.setSelectionRange(pos, pos);
                }
            }, 0);
        }

        function applyVal(newVal, cursorOffset) {
            clearDebounce();
            input.value = newVal;
            if (typeof cursorOffset === 'number') {
                setCursor(cursorOffset);
            }
            syncAlpineDate(input, newVal);
            if ($ && $(input).data('datepicker')) {
                $(input).datepicker('update', newVal);
            }
        }

        // 1. Lắng nghe KEYDOWN để xử lý phím số, dấu phân cách và Backspace
        input.addEventListener('keydown', function (e) {
            if (isSyncing) return;
            isDeleting = (e.key === 'Backspace' || e.key === 'Delete');
            var val = input.value || '';
            var start = input.selectionStart;
            var end = input.selectionEnd;

            // Xử lý Backspace ngay sau dấu '/'
            if (e.key === 'Backspace' && start === end && start > 0) {
                if (val[start - 1] === '/') {
                    e.preventDefault();
                    clearDebounce();
                    isDeleting = true;
                    // Xóa dấu '/' và giữ nguyên con trỏ tại vị trí đó
                    var updated = val.slice(0, start - 1) + val.slice(start);
                    input.value = updated;
                    setCursor(start - 1);
                    syncAlpineDate(input, updated);
                    return;
                }
            }

            // Xử lý phím Enter -> chuẩn hóa ngày
            if (e.key === 'Enter') {
                input.blur();
                return;
            }

            // Xử lý các phím phân cách: '/', '-', '.', ' '
            if (['/', '-', '.', ' '].includes(e.key)) {
                if (start === end) {
                    // Nếu đang là 1 chữ số ngày 1..9 -> tự chuyển thành 01/..09/
                    if (/^[1-9]$/.test(val)) {
                        e.preventDefault();
                        applyVal('0' + val + '/', 3);
                        return;
                    }
                    // Nếu tháng là 1 chữ số sau dd/, vd "05/8" -> "05/08/"
                    var mSingle = val.match(/^(\d{2}\/)([1-9])$/);
                    if (mSingle) {
                        e.preventDefault();
                        applyVal(mSingle[1] + '0' + mSingle[2] + '/', 6);
                        return;
                    }
                    // Nếu đã có 2 số ngày (vd "15") -> thêm '/'
                    if (/^\d{2}$/.test(val)) {
                        e.preventDefault();
                        applyVal(val + '/', 3);
                        return;
                    }
                    // Nếu đã có dd/mm (vd "15/09") -> thêm '/'
                    if (/^\d{2}\/\d{2}$/.test(val)) {
                        e.preventDefault();
                        applyVal(val + '/', 6);
                        return;
                    }
                }
                e.preventDefault();
                return;
            }

            // Xử lý nhập phím số 0-9
            if (/^[0-9]$/.test(e.key)) {
                // Khi bắt đầu nhập ngày (ô đang rỗng hoặc bôi đen toàn bộ)
                if (start === 0 && end === val.length) {
                    if (['4', '5', '6', '7', '8', '9'].includes(e.key)) {
                        // Ngày không thể bắt đầu bằng 4..9 -> tự động thành 04/..09/
                        e.preventDefault();
                        applyVal('0' + e.key + '/', 3);
                        return;
                    }
                }

                // Khi bắt đầu nhập tháng sau "dd/"
                if (start === end && /^\d{2}\/$/.test(val)) {
                    if (['2', '3', '4', '5', '6', '7', '8', '9'].includes(e.key)) {
                        // Tháng không thể bắt đầu bằng 2..9 -> tự động thành 02/..09/
                        e.preventDefault();
                        applyVal(val + '0' + e.key + '/', 6);
                        return;
                    }
                }

                // Nếu đang có '3' và gõ tiếp 2..9 (ngày 32..39 không tồn tại)
                if (start === 1 && end === 1 && val === '3' && ['2', '3', '4', '5', '6', '7', '8', '9'].includes(e.key)) {
                    e.preventDefault();
                    if (['2', '3', '4', '5', '6', '7', '8', '9'].includes(e.key)) {
                        applyVal('03/0' + e.key + '/', 6);
                    } else {
                        applyVal('03/' + e.key, 4);
                    }
                    return;
                }

                // Nếu đang có 'dd/1' và gõ tiếp 3..9 (tháng 13..19 không tồn tại)
                var m1 = val.match(/^(\d{2}\/)1$/);
                if (start === 4 && end === 4 && m1 && ['3', '4', '5', '6', '7', '8', '9'].includes(e.key)) {
                    e.preventDefault();
                    applyVal(m1[1] + '01/' + e.key, 7);
                    return;
                }
            }
        });

        // 2. Lắng nghe INPUT (debounce khi gõ 1, 2, 3 và auto-format)
        input.addEventListener('input', function (e) {
            if (isSyncing) return;
            clearDebounce();
            if (isDeleting) {
                isDeleting = false;
                syncAlpineDate(input, input.value);
                return;
            }

            var val = input.value || '';

            // Nếu người dùng nhập 1 chữ số 1, 2, 3 ở ô ngày:
            // Chờ 500ms, nếu không gõ tiếp thì tự động chuyển thành 01/, 02/, 03/
            if (['1', '2', '3'].includes(val)) {
                timer = setTimeout(function () {
                    if (['1', '2', '3'].includes(input.value)) {
                        applyVal('0' + input.value + '/', 3);
                    }
                }, 500);
                return;
            }

            // Nếu nhập xong 2 chữ số ngày (vd: 15) -> tự động thêm '/'
            if (/^\d{2}$/.test(val)) {
                var d = parseInt(val, 10);
                if (d > 31) d = 31;
                if (d < 1) d = 1;
                var dd = d < 10 ? '0' + d : '' + d;
                applyVal(dd + '/', 3);
                return;
            }

            // Nếu tháng đang là 1 chữ số sau "dd/1" (vd "15/1"):
            // Chờ 500ms, nếu không gõ tiếp thì tự động chuyển thành "dd/01/"
            if (/^\d{2}\/1$/.test(val)) {
                timer = setTimeout(function () {
                    if (/^\d{2}\/1$/.test(input.value)) {
                        var parts = input.value.split('/');
                        applyVal(parts[0] + '/01/', 6);
                    }
                }, 500);
                return;
            }

            // Nếu nhập xong 2 chữ số tháng (vd: "15/10") -> tự động thêm '/'
            if (/^\d{2}\/\d{2}$/.test(val)) {
                var partsM = val.split('/');
                var m = parseInt(partsM[1], 10);
                if (m > 12) m = 12;
                if (m < 1) m = 1;
                var mm = m < 10 ? '0' + m : '' + m;
                applyVal(partsM[0] + '/' + mm + '/', 6);
                return;
            }

            // Giới hạn tối đa 10 ký tự (dd/mm/yyyy)
            if (val.length > 10) {
                input.value = val.substring(0, 10);
                val = input.value;
            }

            // Nếu đã nhập đủ định dạng dd/mm/yyyy
            if (/^\d{1,2}[\/\-\.]\d{1,2}[\/\-\.]\d{4}$/.test(val)) {
                var norm = normalizeDateString(val);
                if (norm !== val) {
                    applyVal(norm, norm.length);
                } else {
                    syncAlpineDate(input, norm);
                }
            } else {
                syncAlpineDate(input, val);
            }
        });

        // 3. Lắng nghe BLUR (khi người dùng chuyển ô hoặc click ra ngoài)
        input.addEventListener('blur', function () {
            clearDebounce();
            var val = input.value ? input.value.trim() : '';
            if (!val) return;

            // Nếu chỉ gõ 1 chữ số ngày 1..9 rồi rời ô -> tự đệm '0'
            if (/^[1-9]$/.test(val)) {
                applyVal('0' + val, 2);
                return;
            }

            // Nếu "dd/m" với m là 1..9 -> đệm '0' cho tháng
            var mSingle = val.match(/^(\d{2}\/)([1-9])$/);
            if (mSingle) {
                applyVal(mSingle[1] + '0' + mSingle[2], 5);
                return;
            }

            // Chuẩn hóa toàn bộ nếu có dấu phân cách
            if (val.includes('/') || val.includes('-') || val.includes('.')) {
                var norm = normalizeDateString(val);
                if (norm && norm !== val) {
                    applyVal(norm, norm.length);
                }
            }
        });

        // 4. Lắng nghe PASTE (khi dán chuỗi ngày tháng)
        input.addEventListener('paste', function (e) {
            clearDebounce();
            var pasted = (e.clipboardData || window.clipboardData).getData('text');
            if (pasted) {
                var norm = normalizeDateString(pasted);
                if (norm && norm !== pasted) {
                    e.preventDefault();
                    applyVal(norm, norm.length);
                }
            }
        });
    }

    // Đăng ký hàm toàn cục
    window.setupSmartDateInput = setupSmartDateInput;

    window.initSmartDateInputs = function (selector) {
        var sel = selector || 'input.datepicker, input.smart-date, input[x-model="fromDate"], input[x-model="toDate"]';
        if (!$) return;

        $(sel).each(function () {
            var $this = $(this);
            // Khởi tạo Bootstrap Datepicker nếu chưa có
            if (!$this.data('datepicker')) {
                $this.datepicker({
                    format: 'dd/mm/yyyy',
                    autoclose: true,
                    todayHighlight: true,
                    forceParse: false
                }).on('changeDate', function () {
                    syncAlpineDate(this, $(this).val());
                }).on('show', function () {
                    if (this.value) {
                        $(this).datepicker('update', this.value);
                    }
                });
            } else {
                // Đảm bảo event changeDate cập nhật Alpine và forceParse tắt
                if ($this.data('datepicker').o) {
                    $this.data('datepicker').o.forceParse = false;
                }
                $this.off('changeDate.smartSync').on('changeDate.smartSync', function () {
                    syncAlpineDate(this, $(this).val());
                });
            }

            // Gắn bộ format thông minh
            setupSmartDateInput(this);
        });
    };

    // Tự động khởi chạy khi DOM sẵn sàng
    if ($) {
        $(document).ready(function () {
            window.initSmartDateInputs();
        });
    } else {
        document.addEventListener('DOMContentLoaded', function () {
            window.initSmartDateInputs();
        });
    }

})(window, window.jQuery);