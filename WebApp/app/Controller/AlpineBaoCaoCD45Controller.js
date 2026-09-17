document.addEventListener('alpine:init', function () {
    Alpine.data('alpineBaoCaoCD45', function () {
        return {
            items: [],
            isLoading: false,
            loaiKy: 'Thang', // Thang, Quy, 6Thang, 12Thang, TuyChon
            thang: (new Date().getMonth() + 1).toString(),
            nam: new Date().getFullYear(),
            quy: 'I',
            ky6Thang: '1',
            fromDate: '',
            toDate: '',
            dateError: '',
            selectedCity: '',
            selectedNhom: '',
            listCities: [],
            listNhoms: [],
            filteredNhoms: [],
            drillModalTitle: '',
            drillItems: [],
            filteredDrillItems: [],
            drillSearchText: '',
            isDrillLoading: false,

            init: function () {
                var self = this;
                var now = new Date();
                var curMonth = now.getMonth() + 1;
                self.thang = curMonth.toString();
                self.nam = now.getFullYear();

                if (curMonth >= 1 && curMonth <= 3) self.quy = 'I';
                else if (curMonth >= 4 && curMonth <= 6) self.quy = 'II';
                else if (curMonth >= 7 && curMonth <= 9) self.quy = 'III';
                else if (curMonth >= 10 && curMonth <= 12) self.quy = 'IV';

                self.ky6Thang = curMonth <= 6 ? '1' : '2';

                self.updateDateRange();
                self.loadDanhMuc();
            },

            formatNumber: function (val) {
                if (val === null || val === undefined || val === 0 || val === "0") return '-';
                return Number(val).toLocaleString('vi-VN');
            },

            loadDanhMuc: function () {
                var self = this;
                $.ajax({
                    type: 'POST',
                    url: '/BaoCaoCD45/GetFilterData',
                    success: function (res) {
                        if (res.Success) {
                            self.listCities = res.Cities || [];
                            self.listNhoms = res.Nhoms || [];
                            self.filteredNhoms = self.listNhoms;
                        }
                        self.loadData();
                    },
                    error: function () {
                        self.loadData();
                    }
                });
            },

            onCityChange: function () {
                var self = this;
                if (!self.selectedCity) {
                    self.filteredNhoms = self.listNhoms;
                } else {
                    self.filteredNhoms = self.listNhoms.filter(function (x) {
                        return x.city_code === self.selectedCity;
                    });
                }
                self.selectedNhom = '';
            },

            parseDateVN: function (dateStr) {
                if (!dateStr || typeof dateStr !== 'string') return null;
                var parts = dateStr.trim().split('/');
                if (parts.length === 3) {
                    var day = parseInt(parts[0], 10);
                    var month = parseInt(parts[1], 10) - 1;
                    var year = parseInt(parts[2], 10);
                    if (!isNaN(day) && !isNaN(month) && !isNaN(year)) {
                        var d = new Date(year, month, day);
                        if (d.getFullYear() === year && d.getMonth() === month && d.getDate() === day) {
                            return d;
                        }
                    }
                }
                var isoParts = dateStr.trim().split('-');
                if (isoParts.length === 3) {
                    var year = parseInt(isoParts[0], 10);
                    var month = parseInt(isoParts[1], 10) - 1;
                    var day = parseInt(isoParts[2], 10);
                    if (!isNaN(day) && !isNaN(month) && !isNaN(year)) {
                        var d = new Date(year, month, day);
                        if (d.getFullYear() === year && d.getMonth() === month && d.getDate() === day) {
                            return d;
                        }
                    }
                }
                return null;
            },

            validateDateRange: function (showToast) {
                var self = this;
                if (showToast === undefined) showToast = true;

                if (!self.fromDate || !self.fromDate.trim()) {
                    self.dateError = "Vui lòng nhập 'Từ ngày'!";
                    if (showToast && window.toastr) toastr.error(self.dateError);
                    return false;
                }
                if (!self.toDate || !self.toDate.trim()) {
                    self.dateError = "Vui lòng nhập 'Đến ngày'!";
                    if (showToast && window.toastr) toastr.error(self.dateError);
                    return false;
                }

                var dFrom = self.parseDateVN(self.fromDate);
                if (!dFrom) {
                    self.dateError = "'Từ ngày' không đúng định dạng dd/mm/yyyy!";
                    if (showToast && window.toastr) toastr.error(self.dateError);
                    return false;
                }

                var dTo = self.parseDateVN(self.toDate);
                if (!dTo) {
                    self.dateError = "'Đến ngày' không đúng định dạng dd/mm/yyyy!";
                    if (showToast && window.toastr) toastr.error(self.dateError);
                    return false;
                }

                if (dTo < dFrom) {
                    self.dateError = "Khoảng thời gian không hợp lệ: 'Đến ngày' không được nhỏ hơn 'Từ ngày' (Từ ngày phải nhỏ hơn hoặc bằng Đến ngày)!";
                    if (showToast && window.toastr) toastr.error(self.dateError);
                    return false;
                }

                self.dateError = '';
                return true;
            },

            updateDateRange: function () {
                var self = this;
                var y = parseInt(self.nam) || new Date().getFullYear();

                if (self.loaiKy === 'Thang') {
                    var m = parseInt(self.thang) || (new Date().getMonth() + 1);
                    // Chu kỳ REDCap: 26 tháng trước đến 25 tháng này
                    var prevMonth = m === 1 ? 12 : m - 1;
                    var prevYear = m === 1 ? y - 1 : y;
                    var strPrevMonth = prevMonth < 10 ? '0' + prevMonth : prevMonth;
                    var strM = m < 10 ? '0' + m : m;

                    self.fromDate = '26/' + strPrevMonth + '/' + prevYear;
                    self.toDate = '25/' + strM + '/' + y;
                } else if (self.loaiKy === 'Quy') {
                    // Chu kỳ Quý: Tính theo quy tắc 26-25 tương ứng với 3 tháng trong quý
                    if (self.quy === 'I') { self.fromDate = '26/12/' + (y - 1); self.toDate = '25/03/' + y; }
                    else if (self.quy === 'II') { self.fromDate = '26/03/' + y; self.toDate = '25/06/' + y; }
                    else if (self.quy === 'III') { self.fromDate = '26/06/' + y; self.toDate = '25/09/' + y; }
                    else if (self.quy === 'IV') { self.fromDate = '26/09/' + y; self.toDate = '25/12/' + y; }
                } else if (self.loaiKy === '6Thang') {
                    if (self.ky6Thang === '1') { self.fromDate = '26/12/' + (y - 1); self.toDate = '25/06/' + y; }
                    else { self.fromDate = '26/06/' + y; self.toDate = '25/12/' + y; }
                } else if (self.loaiKy === '12Thang') {
                    self.fromDate = '26/12/' + (y - 1);
                    self.toDate = '25/12/' + y;
                }
                self.dateError = '';
            },

            loadData: function () {
                var self = this;
                if (!self.validateDateRange(true)) {
                    return;
                }
                self.isLoading = true;

                // Map loaiKy sang giá trị LoaiBaoCao: 'Thang' → 'Thang', 'Quy' → 'Quy', '6Thang' → '6T', '12Thang' → '12T', 'TuyChon' → 'TuyChon' (server sẽ hiển thị tất cả)
                var loaiBaoCaoMap = { 'Thang': 'Thang', 'Quy': 'Quy', '6Thang': '6T', '12Thang': '12T', 'TuyChon': 'TuyChon' };
                var loaiBaoCao = loaiBaoCaoMap[self.loaiKy] || 'TuyChon';

                $.ajax({
                    type: 'POST',
                    url: '/BaoCaoCD45/SearchBaoCao',
                    data: {
                        FromDate: self.fromDate,
                        ToDate: self.toDate,
                        MaTinh: self.selectedCity,
                        MaNhom: self.selectedNhom,
                        LoaiBaoCao: loaiBaoCao
                    },
                    success: function (res) {
                        self.isLoading = false;
                        if (res.Success) {
                            self.items = res.Data || [];
                            if (res.Warning && window.toastr) {
                                toastr.warning(res.Warning, 'Cảnh báo tính toàn vẹn (VR-01)', { timeOut: 10000, closeButton: true });
                            }
                        } else {
                            if (window.toastr) toastr.error(res.Message);
                        }
                    },
                    error: function () {
                        self.isLoading = false;
                        if (window.toastr) toastr.error('Có lỗi xảy ra khi tải dữ liệu báo cáo!');
                    }
                });
            },

            exportExcel: function () {
                var self = this;
                if (!self.validateDateRange(true)) {
                    return;
                }
                var loaiBaoCaoMap = { 'Thang': 'Thang', 'Quy': 'Quy', '6Thang': '6T', '12Thang': '12T', 'TuyChon': 'TuyChon' };
                var loaiBaoCao = loaiBaoCaoMap[self.loaiKy] || 'TuyChon';
                var url = '/BaoCaoCD45/ExportExcel?FromDate=' + encodeURIComponent(self.fromDate) +
                    '&ToDate=' + encodeURIComponent(self.toDate) +
                    '&MaTinh=' + encodeURIComponent(self.selectedCity) +
                    '&MaNhom=' + encodeURIComponent(self.selectedNhom) +
                    '&LoaiBaoCao=' + encodeURIComponent(loaiBaoCao);
                window.location.href = url;
            },

            openDrillDown: function (item, colKey, colTitle) {
                var self = this;
                if (!self.validateDateRange(true)) {
                    return;
                }
                console.log('[CD45 DrillDown] Clicked:', item ? item.Code : null, colKey, item ? item[colKey] : null);

                if (!item || !item.Code) {
                    console.warn('[CD45 DrillDown] Missing item or Code:', item);
                    return;
                }
                var val = item[colKey];
                if (val === null || val === undefined || val === 0 || val === '0') {
                    console.log('[CD45 DrillDown] Value is 0 or empty, ignoring.');
                    return;
                }

                var doiTuongMap = { 'Tong': 0, 'PUD': 1, 'PLHIV': 2, 'TG': 3, 'SW': 5, 'MSM': 4 };
                var doiTuong = doiTuongMap[colKey] !== undefined ? doiTuongMap[colKey] : 0;

                self.drillModalTitle = 'Chi tiết: ' + item.ChiTieu + ' (' + colTitle + ': ' + self.formatNumber(val) + ' KH)';
                self.drillItems = [];
                self.filteredDrillItems = [];
                self.drillSearchText = '';
                self.isDrillLoading = true;

                // Show modal safely for Bootstrap 5, Bootstrap 4/jQuery, or CSS fallback
                var modalEl = document.getElementById('modalDrillDown');
                if (modalEl) {
                    if (window.bootstrap && bootstrap.Modal) {
                        var modal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
                        modal.show();
                    } else if (window.jQuery && typeof $(modalEl).modal === 'function') {
                        $(modalEl).modal('show');
                    } else {
                        modalEl.classList.add('show');
                        modalEl.style.display = 'block';
                        modalEl.removeAttribute('aria-hidden');
                        modalEl.setAttribute('aria-modal', 'true');
                    }
                }

                $.ajax({
                    type: 'POST',
                    url: '/BaoCaoCD45/GetDrillDownData',
                    data: {
                        ChiTieuCode: item.Code,
                        FromDate: self.fromDate,
                        ToDate: self.toDate,
                        MaTinh: self.selectedCity,
                        MaNhom: self.selectedNhom,
                        DoiTuong: doiTuong
                    },
                    success: function (res) {
                        self.isDrillLoading = false;
                        if (res.Success) {
                            self.drillItems = res.Data || [];
                            self.filterDrill();
                        } else {
                            if (window.toastr) toastr.error(res.Message);
                        }
                    },
                    error: function (xhr, status, error) {
                        self.isDrillLoading = false;
                        console.error('[CD45 DrillDown] AJAX Error:', status, error, xhr.responseText);
                        if (window.toastr) toastr.error('Lỗi khi tải chi tiết khách hàng!');
                    }
                });
            },

            closeDrillDown: function () {
                var modalEl = document.getElementById('modalDrillDown');
                if (modalEl) {
                    if (window.bootstrap && bootstrap.Modal) {
                        var modal = bootstrap.Modal.getInstance(modalEl);
                        if (modal) modal.hide();
                    }
                    if (window.jQuery && typeof $(modalEl).modal === 'function') {
                        $(modalEl).modal('hide');
                    }
                    modalEl.classList.remove('show');
                    modalEl.style.display = 'none';
                    modalEl.setAttribute('aria-hidden', 'true');
                    modalEl.removeAttribute('aria-modal');
                    var backdrops = document.querySelectorAll('.modal-backdrop');
                    backdrops.forEach(function (b) { b.remove(); });
                }
            },

            filterDrill: function () {
                var self = this;
                var kw = (self.drillSearchText || '').trim().toLowerCase();
                if (!kw) {
                    self.filteredDrillItems = self.drillItems;
                    return;
                }
                self.filteredDrillItems = self.drillItems.filter(function (x) {
                    return (x.RECORD_ID && x.RECORD_ID.toLowerCase().indexOf(kw) >= 0)
                        || (x.CITY_CODE && x.CITY_CODE.toLowerCase().indexOf(kw) >= 0)
                        || (x.TEN_TINH && x.TEN_TINH.toLowerCase().indexOf(kw) >= 0)
                        || (x.MA_NHOM && x.MA_NHOM.toLowerCase().indexOf(kw) >= 0)
                        || (x.TEN_NHOM && x.TEN_NHOM.toLowerCase().indexOf(kw) >= 0)
                        || (x.MA_TCV && x.MA_TCV.toLowerCase().indexOf(kw) >= 0)
                        || (x.TEN_TCV && x.TEN_TCV.toLowerCase().indexOf(kw) >= 0)
                        || (x.DOI_TUONG_TEXT && x.DOI_TUONG_TEXT.toLowerCase().indexOf(kw) >= 0)
                        || (x.CHI_TIET && x.CHI_TIET.toLowerCase().indexOf(kw) >= 0);
                });
            }
        };
    });
});
