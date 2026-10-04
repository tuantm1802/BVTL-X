document.addEventListener('alpine:init', function () {
    Alpine.data('alpineBaoCaoBacSiCD45', function () {
        return {
            summary: [],
            details: [],
            filteredDetails: [],
            detailSearch: '',
            activeTab: 'summary', // 'summary' | 'details'
            isLoading: false,

            loaiKy: 'Thang', // 'Thang' | 'TuyChon'
            thang: (new Date().getMonth() + 1).toString(),
            nam: new Date().getFullYear(),
            fromDate: '',
            toDate: '',
            dateError: '',

            cityMode: 'NEW34',
            selectedCity: '',
            selectedDoctor: '',
            selectedNhom: '',

            listCities: [],
            listDoctors: [],
            listNhoms: [],
            filteredDoctors: [],
            filteredNhoms: [],
            isAdmin: true,

            init: function () {
                var self = this;
                var now = new Date();
                self.thang = (now.getMonth() + 1).toString();
                self.nam = now.getFullYear();

                self.updateDateRange();
                self.loadDanhMuc();
            },

            formatNumber: function (val) {
                if (val === null || val === undefined || val === 0 || val === '0') return '-';
                return Number(val).toLocaleString('vi-VN');
            },

            setCityMode: function (mode) {
                if (this.cityMode !== mode) {
                    this.cityMode = mode;
                    this.selectedCity = '';
                    this.selectedDoctor = '';
                    this.selectedNhom = '';
                    this.loadDanhMuc();
                }
            },

            updateDateRange: function () {
                var self = this;
                var y = parseInt(self.nam) || new Date().getFullYear();

                if (self.loaiKy === 'Thang') {
                    var m = parseInt(self.thang) || (new Date().getMonth() + 1);
                    // Báo cáo Bác sĩ chạy từ ngày 1 tới ngày cuối cùng của tháng
                    var strM = m < 10 ? '0' + m : m;
                    var lastDay = new Date(y, m, 0).getDate();
                    var strLastDay = lastDay < 10 ? '0' + lastDay : lastDay;

                    self.fromDate = '01/' + strM + '/' + y;
                    self.toDate = strLastDay + '/' + strM + '/' + y;
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
                    var d = new Date(year, month, day);
                    if (d.getFullYear() === year && d.getMonth() === month && d.getDate() === day) {
                        return d;
                    }
                }
                return null;
            },

            loadDanhMuc: function () {
                var self = this;
                $.ajax({
                    type: 'POST',
                    url: '/BaoCaoBacSiCD45/GetFilterData',
                    data: { cityMode: self.cityMode },
                    success: function (res) {
                        if (res.Success) {
                            self.listCities = res.Cities || [];
                            self.listDoctors = res.Doctors || [];
                            self.listNhoms = res.Nhoms || [];
                            self.filteredDoctors = self.listDoctors;
                            self.filteredNhoms = self.listNhoms;
                            self.isAdmin = res.IsAdmin !== undefined ? res.IsAdmin : true;
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
                    self.filteredDoctors = self.listDoctors;
                    self.filteredNhoms = self.listNhoms;
                } else {
                    var foundCity = self.listCities.find(function (c) { return c.CityCode === self.selectedCity; });
                    var mappedCodes = (foundCity && foundCity.OldCodes && foundCity.OldCodes.length) ? foundCity.OldCodes : [self.selectedCity];

                    self.filteredDoctors = self.listDoctors.filter(function (d) {
                        return !d.CITY_CODE || mappedCodes.indexOf(d.CITY_CODE) !== -1;
                    });
                    self.filteredNhoms = self.listNhoms.filter(function (n) {
                        return !n.city_code || mappedCodes.indexOf(n.city_code) !== -1;
                    });
                }
                self.selectedDoctor = '';
                self.selectedNhom = '';
            },

            loadData: function () {
                var self = this;
                if (!self.validateDateRange(true)) return;
                self.isLoading = true;

                $.ajax({
                    type: 'POST',
                    url: '/BaoCaoBacSiCD45/SearchBaoCao',
                    data: {
                        FromDate: self.fromDate,
                        ToDate: self.toDate,
                        MaTinh: self.selectedCity,
                        DoctorId: self.selectedDoctor,
                        MaNhom: self.selectedNhom
                    },
                    success: function (res) {
                        self.isLoading = false;
                        if (res.Success) {
                            self.summary = res.Summary || [];
                            self.details = res.Details || [];
                            self.filterDetails();
                        } else {
                            if (window.toastr) toastr.error(res.Message);
                        }
                    },
                    error: function () {
                        self.isLoading = false;
                        if (window.toastr) toastr.error('Có lỗi xảy ra khi tải dữ liệu báo cáo bác sĩ!');
                    }
                });
            },

            filterDetails: function () {
                var self = this;
                var q = (self.detailSearch || '').trim().toLowerCase();
                if (!q) {
                    self.filteredDetails = self.details;
                    return;
                }
                self.filteredDetails = self.details.filter(function (d) {
                    return (d.RECORD_ID && d.RECORD_ID.toLowerCase().indexOf(q) !== -1) ||
                           (d.TEN_BAC_SI && d.TEN_BAC_SI.toLowerCase().indexOf(q) !== -1) ||
                           (d.TEN_TINH && d.TEN_TINH.toLowerCase().indexOf(q) !== -1) ||
                           (d.TEN_NHOM && d.TEN_NHOM.toLowerCase().indexOf(q) !== -1) ||
                           (d.TEN_CO_SO_Y_TE && d.TEN_CO_SO_Y_TE.toLowerCase().indexOf(q) !== -1) ||
                           (d.TEN_CHAN_DOAN && d.TEN_CHAN_DOAN.toLowerCase().indexOf(q) !== -1);
                });
            },

            exportExcel: function () {
                var self = this;
                if (!self.validateDateRange(true)) return;

                var url = '/BaoCaoBacSiCD45/ExportExcel?FromDate=' + encodeURIComponent(self.fromDate) +
                    '&ToDate=' + encodeURIComponent(self.toDate) +
                    '&MaTinh=' + encodeURIComponent(self.selectedCity) +
                    '&DoctorId=' + encodeURIComponent(self.selectedDoctor) +
                    '&MaNhom=' + encodeURIComponent(self.selectedNhom);
                window.location.href = url;
            },

            getTotalSummary: function (key) {
                var self = this;
                if (!self.summary || self.summary.length === 0) return 0;
                return self.summary.reduce(function (sum, item) {
                    return sum + (item[key] || 0);
                }, 0);
            }
        };
    });
});
