function removeVietnameseTones(str) {
    if (!str) return '';
    str = str.toLowerCase();
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/\u0300|\u0301|\u0303|\u0309|\u0323/g, "");
    str = str.replace(/\u02C6|\u0306|\u031B/g, "");
    return str.trim();
}

document.addEventListener('alpine:init', function () {
    Alpine.data('alpineBaoCaoTuVanCD45', function () {
        return {
            listL1: [],
            listL2: [],
            filteredL1: [],
            filteredL2: [],
            searchL1Keyword: '',
            searchL2Keyword: '',
            activeTab: 'l1', // 'l1' | 'l2'
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
            selectedTCV: '',
            loaiTuVan: 'ALL', // 'ALL' | 'L1' | 'L2'

            listCities: [],
            listNhoms: [],
            filteredNhoms: [],
            listTCVs: [],
            filteredTCVs: [],
            isAdmin: true,

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
                if (val === null || val === undefined || val === 0 || val === '0') return '-';
                return Number(val).toLocaleString('vi-VN');
            },

            updateDateRange: function () {
                var self = this;
                var y = parseInt(self.nam) || new Date().getFullYear();

                if (self.loaiKy === 'Thang') {
                    var m = parseInt(self.thang) || (new Date().getMonth() + 1);
                    var prevMonth = m === 1 ? 12 : m - 1;
                    var prevYear = m === 1 ? y - 1 : y;
                    var strPrevMonth = prevMonth < 10 ? '0' + prevMonth : prevMonth;
                    var strM = m < 10 ? '0' + m : m;

                    self.fromDate = '26/' + strPrevMonth + '/' + prevYear;
                    self.toDate = '25/' + strM + '/' + y;
                } else if (self.loaiKy === 'Quy') {
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

            validateDateRange: function (showAlert) {
                var self = this;
                if (!self.fromDate || !self.toDate) {
                    self.dateError = 'Vui lòng chọn đầy đủ Từ ngày và Đến ngày!';
                    if (showAlert && window.toastr) toastr.warning(self.dateError);
                    return false;
                }
                var d1 = self.parseDateVN(self.fromDate);
                var d2 = self.parseDateVN(self.toDate);
                if (!d1 || !d2) {
                    self.dateError = 'Định dạng ngày không hợp lệ (dd/MM/yyyy)!';
                    if (showAlert && window.toastr) toastr.warning(self.dateError);
                    return false;
                }
                if (d1 > d2) {
                    self.dateError = 'Từ ngày không được lớn hơn Đến ngày!';
                    if (showAlert && window.toastr) toastr.warning(self.dateError);
                    return false;
                }
                self.dateError = '';
                return true;
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

            loadDanhMuc: function () {
                var self = this;
                $.ajax({
                    url: '/BaoCaoTuVanCD45/GetFilterData',
                    type: 'POST',
                    success: function (res) {
                        if (res.Success) {
                            self.listCities = res.Cities || [];
                            self.listNhoms = res.Nhoms || [];
                            self.listTCVs = res.TCVs || [];
                            self.isAdmin = res.IsAdmin !== false;
                            self.applyFilterCascades();
                            self.loadData();
                        } else {
                            if (window.toastr) toastr.error(res.Message || 'Lỗi tải danh mục');
                        }
                    },
                    error: function (xhr) {
                        if (window.toastr) toastr.error('Lỗi kết nối máy chủ');
                    }
                });
            },

            onCityChange: function () {
                this.selectedNhom = '';
                this.selectedTCV = '';
                this.applyFilterCascades();
                this.fetchTCVs();
            },

            onNhomChange: function () {
                this.selectedTCV = '';
                this.applyFilterCascades();
                this.fetchTCVs();
            },

            applyFilterCascades: function () {
                var self = this;
                if (!self.selectedCity) {
                    self.filteredNhoms = self.listNhoms;
                } else {
                    self.filteredNhoms = self.listNhoms.filter(function (n) {
                        return n.city_code === self.selectedCity;
                    });
                }
            },

            fetchTCVs: function () {
                var self = this;
                $.ajax({
                    url: '/BaoCaoTuVanCD45/GetTCVsByFilter',
                    type: 'POST',
                    data: {
                        cityCode: self.selectedCity,
                        maNhom: self.selectedNhom
                    },
                    success: function (res) {
                        if (res.Success) {
                            self.filteredTCVs = res.TCVs || [];
                        }
                    }
                });
            },

            loadData: function () {
                var self = this;
                if (!self.validateDateRange(true)) return;

                self.isLoading = true;
                $.ajax({
                    url: '/BaoCaoTuVanCD45/SearchTuVan',
                    type: 'POST',
                    data: {
                        fromDate: self.fromDate,
                        toDate: self.toDate,
                        cityCode: self.selectedCity,
                        maNhom: self.selectedNhom,
                        maTCV: self.selectedTCV,
                        loaiTuVan: self.loaiTuVan
                    },
                    success: function (res) {
                        self.isLoading = false;
                        if (res.Success) {
                            self.listL1 = res.L1 || [];
                            self.listL2 = res.L2 || [];
                            self.filterL1();
                            self.filterL2();

                            if (self.loaiTuVan === 'L2' && self.activeTab === 'l1') {
                                self.activeTab = 'l2';
                            } else if (self.loaiTuVan === 'L1' && self.activeTab === 'l2') {
                                self.activeTab = 'l1';
                            }
                        } else {
                            if (window.toastr) toastr.error(res.Message || 'Lỗi tra cứu báo cáo tư vấn');
                        }
                    },
                    error: function (xhr) {
                        self.isLoading = false;
                        if (window.toastr) toastr.error('Lỗi kết nối khi tải dữ liệu');
                    }
                });
            },

            filterL1: function () {
                var self = this;
                var kw = removeVietnameseTones(self.searchL1Keyword.trim());
                if (!kw) {
                    self.filteredL1 = self.listL1;
                    return;
                }
                self.filteredL1 = self.listL1.filter(function (item) {
                    var str = (item.RECORD_ID || '') + ' ' +
                              (item.MA_KH_NGHIEN_CUU || '') + ' ' +
                              (item.CITY_NAME || '') + ' ' +
                              (item.TEN_NHOM || '') + ' ' +
                              (item.TEN_TCV || '') + ' ' +
                              (item.DIA_DIEM_TEXT || '') + ' ' +
                              (item.Q3A_TEXT || '') + ' ' +
                              (item.Q3B_TEXT || '') + ' ' +
                              (item.Q3C_NHU_CAU || '') + ' ' +
                              (item.Q41_CAN_THIEP_GTH || '') + ' ' +
                              (item.Q42_NHU_CAU_TG || '');
                    return removeVietnameseTones(str).indexOf(kw) >= 0;
                });
            },

            filterL2: function () {
                var self = this;
                var kw = removeVietnameseTones(self.searchL2Keyword.trim());
                if (!kw) {
                    self.filteredL2 = self.listL2;
                    return;
                }
                self.filteredL2 = self.listL2.filter(function (item) {
                    var str = (item.RECORD_ID || '') + ' ' +
                              (item.MA_KH_NGHIEN_CUU || '') + ' ' +
                              (item.LAN_TU_VAN || '') + ' ' +
                              (item.CITY_NAME || '') + ' ' +
                              (item.TEN_NHOM || '') + ' ' +
                              (item.TEN_TCV || '') + ' ' +
                              (item.DIA_DIEM_TEXT || '') + ' ' +
                              (item.Q21A_CAM_THAY || '') + ' ' +
                              (item.Q22_VAN_DE || '') + ' ' +
                              (item.Q31A_CAM_XUC || '') + ' ' +
                              (item.Q31B_CANG_THANG_NGAN || '') + ' ' +
                              (item.Q31C_CANG_THANG_DAI || '') + ' ' +
                              (item.Q31D_THOI_QUEN || '') + ' ' +
                              (item.Q31E_NOI_DUNG_KHAC || '') + ' ' +
                              (item.Q32_CAN_THIEP_CHINH || '') + ' ' +
                              (item.Q41_DONG_Y || '') + ' ' +
                              (item.Q421_VIEC_LAM || '');
                    return removeVietnameseTones(str).indexOf(kw) >= 0;
                });
            },

            exportExcel: function () {
                var self = this;
                if (!self.validateDateRange(true)) return;

                var selectedNhomObj = self.listNhoms.find(function (n) { return n.manhom_tbh === self.selectedNhom; });
                var tenNhom = selectedNhomObj ? selectedNhomObj.tennhom_tbh : '';

                var selectedTcvObj = self.filteredTCVs.find(function (t) { return t.MA_TCV === self.selectedTCV; });
                var tenTCV = selectedTcvObj ? selectedTcvObj.TEN_TCV : '';

                var url = '/BaoCaoTuVanCD45/ExportExcel?' +
                    'FromDate=' + encodeURIComponent(self.fromDate) +
                    '&ToDate=' + encodeURIComponent(self.toDate) +
                    '&CityCode=' + encodeURIComponent(self.selectedCity || '') +
                    '&MaNhom=' + encodeURIComponent(self.selectedNhom || '') +
                    '&MaTCV=' + encodeURIComponent(self.selectedTCV || '') +
                    '&LoaiTuVan=' + encodeURIComponent(self.loaiTuVan || 'ALL') +
                    '&TenNhom=' + encodeURIComponent(tenNhom || '') +
                    '&TenTCV=' + encodeURIComponent(tenTCV || '');

                window.location.href = url;
            }
        };
    });
});
