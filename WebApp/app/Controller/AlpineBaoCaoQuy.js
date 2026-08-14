document.addEventListener('alpine:init', function () {
    Alpine.data('alpineBaoCaoQuy', function () {
        return {
            modelSearch: {
                totalItems: 0,
                currentPage: 1,
                maxSize: 5,
                pageSize: 10,
                SortColumn: "ParamCode DESC",
                Year: new Date().getFullYear(),
                Months: '',
                CityCodes: '',
                MaNhomTBH: '',
                MaDuAn: ''
            },
            ListYear: [],
            ListCity: [],
            ListCityCode: [],
            ListMaNhomTBH: [],
            ListNhomTBH: [],
            ListDuAn: [],
            ListData: [],
            Quy: 'I',
            ParamIdSeleted: 0,
            RoleBtnUpdate: false,
            RoleBtnSearch: false,

            init: function () {
                var self = this;
                self.ListMaNhomTBH = [];
                self.ListNhomTBH = [];

                var date = new Date();
                self.ListYear = [];
                for (var i = date.getFullYear() - 5; i <= date.getFullYear() + 5; i++) {
                    self.ListYear.push({ Id: i, Name: i.toString() });
                }

                var month = date.getMonth() + 1;
                if (month >= 1 && month <= 3) {
                    self.Quy = 'I';
                } else if (month >= 4 && month <= 6) {
                    self.Quy = 'II';
                } else if (month >= 7 && month <= 9) {
                    self.Quy = 'III';
                } else if (month >= 10 && month <= 12) {
                    self.Quy = 'IV';
                }

                self.modelSearch.Year = date.getFullYear();

                self.GetBottomAction(function () {
                    self.Changecity(function () {
                        if (self.modelSearch.MaDuAn) {
                            self.LoadPage(1);
                        }
                    });
                });
            },

            formatNumber: function (value) {
                if (value === null || value === undefined || value === '') return '';
                return Number(value).toLocaleString('en-US');
            },

            GetBottomAction: function (callback) {
                var self = this;
                $.ajax({
                    type: 'POST',
                    url: '/BaoCaoQuy/GetBottomAction',
                    contentType: 'application/json',
                    data: '{}',
                    success: function (response) {
                        if (response && response.Buttoms) {
                            self.RoleBtnUpdate = response.Buttoms.indexOf('btnUpdate') !== -1;
                            self.RoleBtnSearch = response.Buttoms.indexOf('btnSearch') !== -1;
                        }
                        self.ListCity = (response && response.Citys) ? response.Citys : [];
                        self.ListDuAn = (response && response.DuAns) ? response.DuAns : [];
                        if (!self.modelSearch.MaDuAn && self.ListDuAn.length > 0) {
                            self.modelSearch.MaDuAn = self.ListDuAn[0].maduan;
                        }
                        if (typeof callback === 'function') callback();
                    },
                    error: function (err) {
                        console.error(err);
                        if (typeof callback === 'function') callback();
                    }
                });
            },

            buildSearchModel: function () {
                var self = this;
                if (!self.modelSearch.Year || self.modelSearch.Year == 0) {
                    if (window.toastr) toastr.error('Vui lòng chọn năm!');
                    return false;
                }

                if (!self.modelSearch.MaDuAn) {
                    if (window.toastr) toastr.error('Vui lòng chọn dự án!');
                    return false;
                }

                self.modelSearch.Months = '';
                if (!self.Quy) {
                    if (window.toastr) toastr.error('Vui lòng chọn quý!');
                    return false;
                }

                if (self.Quy === 'I') {
                    self.modelSearch.Months = '1,2,3';
                } else if (self.Quy === 'II') {
                    self.modelSearch.Months = '4,5,6';
                } else if (self.Quy === 'III') {
                    self.modelSearch.Months = '7,8,9';
                } else if (self.Quy === 'IV') {
                    self.modelSearch.Months = '10,11,12';
                }

                self.modelSearch.CityCodes = (self.ListCityCode && self.ListCityCode.length > 0) ? self.ListCityCode.join(',') : '';
                self.modelSearch.MaNhomTBH = (self.ListMaNhomTBH && self.ListMaNhomTBH.length > 0) ? self.ListMaNhomTBH.join(',') : '';

                return true;
            },

            LoadPage: function (genTable) {
                var self = this;
                if (!self.buildSearchModel()) {
                    return;
                }

                if (window.showToast) showToast();
                self.ListData = [];

                $.ajax({
                    type: 'POST',
                    url: '/BaoCaoQuy/SearchData',
                    cache: false,
                    data: self.modelSearch,
                    success: function (response) {
                        self.ListData = (response && response.data) ? response.data : [];
                        if (window.hideLoading) hideLoading();
                    },
                    error: function (xhr, status, error) {
                        console.error(error || status);
                        if (window.hideLoading) hideLoading();
                    }
                });
            },

            Refesh: function () {
                this.LoadPage(0);
            },

            ExportExcel: function () {
                var self = this;
                if (!self.buildSearchModel()) {
                    return;
                }

                var params = new URLSearchParams({
                    Months: self.modelSearch.Months || '',
                    Year: self.modelSearch.Year || '',
                    CityCodes: self.modelSearch.CityCodes || '',
                    quy: self.Quy || '',
                    maNhomTBHs: self.modelSearch.MaNhomTBH || '',
                    maDuAn: self.modelSearch.MaDuAn || ''
                });

                window.location.href = '/BaoCaoQuy/ExportData?' + params.toString();
            },

            Changecity: function (callback) {
                var self = this;
                var cityCodesStr = (self.ListCityCode && self.ListCityCode.length > 0) ? self.ListCityCode.join(',') : '';
                self.ListNhomTBH = [];
                self.ListMaNhomTBH = [];

                $.ajax({
                    type: 'POST',
                    url: '/BaoCaoQuy/GetNhomTBHByCityCodes',
                    cache: false,
                    data: { CityCodes: cityCodesStr },
                    success: function (response) {
                        self.ListNhomTBH = (response && response.NhomTBHs) ? response.NhomTBHs : [];
                        if (typeof callback === 'function') callback();
                    },
                    error: function (xhr, status, error) {
                        console.error(error || status);
                        if (typeof callback === 'function') callback();
                    }
                });
            }
        };
    });
});
