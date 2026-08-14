document.addEventListener('alpine:init', function () {
    Alpine.data('alpineBaoCaoQuyCH07', function () {
        return {
            modelSearch: {
                Year: new Date().getFullYear(),
                CityCodes: '',
                MaNhomTBH: '',
                Months: '',
                MaDuAn: 'CH07'
            },
            Quy: '',
            ListYear: [],
            ListCity: [],
            ListCityCode: [],
            ListMaNhomTBH: [],
            ListNhomTBH: [],
            ListDuAn: [],
            ListData: [],
            RoleBtnSearch: false,
            RoleBtnUpdate: false,

            init: function () {
                var self = this;
                var currentYear = new Date().getFullYear();
                self.ListYear = [];
                for (var i = currentYear - 5; i <= currentYear + 5; i++) {
                    self.ListYear.push({ Id: i, Name: i.toString() });
                }

                var month = new Date().getMonth() + 1;
                if (month >= 1 && month <= 3) self.Quy = "I";
                else if (month >= 4 && month <= 6) self.Quy = "II";
                else if (month >= 7 && month <= 9) self.Quy = "III";
                else self.Quy = "IV";

                self.GetBottomAction(function () {
                    self.Changecity(function () {
                        self.LoadPage(1);
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
                    url: '/BaoCaoQuyCH07/GetBottomAction',
                    contentType: 'application/json',
                    data: '{}',
                    success: function (data) {
                        if (data && data.Buttoms) {
                            self.RoleBtnUpdate = data.Buttoms.indexOf('btnUpdate') !== -1;
                            self.RoleBtnSearch = data.Buttoms.indexOf('btnSearch') !== -1;
                        }
                        self.ListCity = (data && data.Citys) ? data.Citys : [];
                        self.ListDuAn = (data && data.DuAns) ? data.DuAns : [];
                        if (typeof callback === 'function') callback();
                    },
                    error: function (err) {
                        console.error(err);
                        if (typeof callback === 'function') callback();
                    }
                });
            },

            Changecity: function (callback) {
                var self = this;
                var citys = (self.ListCityCode && self.ListCityCode.length > 0) ? self.ListCityCode.join(',') : '';
                self.ListNhomTBH = [];
                self.ListMaNhomTBH = [];

                $.ajax({
                    type: 'POST',
                    url: '/BaoCaoQuyCH07/GetNhomTBHByCityCodes',
                    data: { CityCodes: citys },
                    success: function (data) {
                        self.ListNhomTBH = (data && data.NhomTBHs) ? data.NhomTBHs : [];
                        if (typeof callback === 'function') callback();
                    },
                    error: function (err) {
                        console.error(err);
                        if (typeof callback === 'function') callback();
                    }
                });
            },

            LoadPage: function (genTable) {
                var self = this;
                if (!self.modelSearch.Year) {
                    if (window.toastr) toastr.error("Vui lòng chọn năm!");
                    return;
                }

                if (!self.Quy) {
                    if (window.toastr) toastr.error("Vui lòng chọn quý!");
                    return;
                }

                switch (self.Quy) {
                    case 'I': self.modelSearch.Months = '1,2,3'; break;
                    case 'II': self.modelSearch.Months = '4,5,6'; break;
                    case 'III': self.modelSearch.Months = '7,8,9'; break;
                    case 'IV': self.modelSearch.Months = '10,11,12'; break;
                }

                self.modelSearch.CityCodes = (self.ListCityCode && self.ListCityCode.length > 0) ? self.ListCityCode.join(',') : '';
                self.modelSearch.MaNhomTBH = (self.ListMaNhomTBH && self.ListMaNhomTBH.length > 0) ? self.ListMaNhomTBH.join(',') : '';

                if (window.showToast) showToast();
                self.ListData = [];

                $.ajax({
                    type: 'POST',
                    url: '/BaoCaoQuyCH07/SearchData',
                    data: self.modelSearch,
                    success: function (data) {
                        self.ListData = (data && data.data) ? data.data : [];
                        if (window.hideLoading) hideLoading();
                    },
                    error: function (err) {
                        console.error(err);
                        if (window.hideLoading) hideLoading();
                    }
                });
            },

            Refesh: function () {
                this.LoadPage(0);
            },

            ExportExcel: function () {
                var self = this;
                if (!self.modelSearch.Year) {
                    if (window.toastr) toastr.error("Vui lòng chọn năm!");
                    return;
                }

                self.modelSearch.CityCodes = (self.ListCityCode && self.ListCityCode.length > 0) ? self.ListCityCode.join(',') : '';
                self.modelSearch.MaNhomTBH = (self.ListMaNhomTBH && self.ListMaNhomTBH.length > 0) ? self.ListMaNhomTBH.join(',') : '';

                var url = '/BaoCaoQuyCH07/ExportData?Months=' + self.modelSearch.Months
                    + '&Year=' + self.modelSearch.Year
                    + '&CityCodes=' + self.modelSearch.CityCodes
                    + '&quy=' + self.Quy
                    + '&maNhomTBHs=' + self.modelSearch.MaNhomTBH
                    + '&maDuAn=' + (self.modelSearch.MaDuAn || 'CH07');

                window.location.href = url;
            }
        };
    });
});
