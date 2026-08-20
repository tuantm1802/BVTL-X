document.addEventListener('alpine:init', function () {
    Alpine.data('alpineBaoCaoCD45', function () {
        return {
            items: [],
            isLoading: false,
            loaiKy: 'Thang', // Thang, Quy, 6Thang, 12Thang, TuyChon
            thang: new Date().getMonth() + 1,
            nam: new Date().getFullYear(),
            quy: 'I',
            ky6Thang: '1',
            fromDate: '',
            toDate: '',
            selectedCity: '',
            selectedNhom: '',
            listCities: [],
            listNhoms: [],
            filteredNhoms: [],

            init: function () {
                var self = this;
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
                    if (self.quy === 'I') { self.fromDate = '01/01/' + y; self.toDate = '31/03/' + y; }
                    else if (self.quy === 'II') { self.fromDate = '01/04/' + y; self.toDate = '30/06/' + y; }
                    else if (self.quy === 'III') { self.fromDate = '01/07/' + y; self.toDate = '30/09/' + y; }
                    else if (self.quy === 'IV') { self.fromDate = '01/10/' + y; self.toDate = '31/12/' + y; }
                } else if (self.loaiKy === '6Thang') {
                    if (self.ky6Thang === '1') { self.fromDate = '01/01/' + y; self.toDate = '30/06/' + y; }
                    else { self.fromDate = '01/07/' + y; self.toDate = '31/12/' + y; }
                } else if (self.loaiKy === '12Thang') {
                    self.fromDate = '01/01/' + y;
                    self.toDate = '31/12/' + y;
                }
            },

            loadData: function () {
                var self = this;
                self.isLoading = true;

                $.ajax({
                    type: 'POST',
                    url: '/BaoCaoCD45/SearchBaoCao',
                    data: {
                        FromDate: self.fromDate,
                        ToDate: self.toDate,
                        MaTinh: self.selectedCity,
                        MaNhom: self.selectedNhom
                    },
                    success: function (res) {
                        self.isLoading = false;
                        if (res.Success) {
                            self.items = res.Data || [];
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
                var url = '/BaoCaoCD45/ExportExcel?FromDate=' + encodeURIComponent(self.fromDate) +
                    '&ToDate=' + encodeURIComponent(self.toDate) +
                    '&MaTinh=' + encodeURIComponent(self.selectedCity) +
                    '&MaNhom=' + encodeURIComponent(self.selectedNhom);
                window.location.href = url;
            }
        };
    });
});
