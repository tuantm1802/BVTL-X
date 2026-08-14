document.addEventListener('alpine:init', function () {
    Alpine.data('alpineBaoCaoChiSo6Thang', function () {
        return {
            modelSearch: {
                totalItems: 0,
                currentPage: 1,
                maxSize: 5,
                pageSize: 10,
                SortColumn: "ParamCode DESC",
                TypeReport: 3,
                Year: new Date().getFullYear(),
                Months: '',
                CityCodes: '',
                MaNhomTBH: '',
                MaDuAn: ''
            },
            ListYear: [],
            ListCity: [],
            List6Thang: [
                { Id: 1, Name: '6 tháng đầu năm' },
                { Id: 2, Name: '6 tháng cuối năm' }
            ],
            SauThang: 1,
            ListCityCode: [],
            ListMaNhomTBH: [],
            ListNhomTBH: [],
            ListDuAn: [],
            ListData: [],

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
                self.SauThang = month <= 6 ? 1 : 2;
                self.modelSearch.Year = date.getFullYear();

                self.GetBottomAction(function () {
                    self.Changecity(function () {
                        self.LoadPage(1);
                    });
                });
            },

            formatNumber: function (value) {
                if (value === null || value === undefined || value === "") return '';
                return Number(value).toLocaleString('en-US');
            },

            GetBottomAction: function (callback) {
                var self = this;
                $.ajax({
                    type: 'POST',
                    url: '/BaoCaoChiSo/GetBottomAction',
                    contentType: 'application/json',
                    data: '{}',
                    success: function (response) {
                        if (response && response.Buttoms) {
                            self.RoleBtnUpdate = response.Buttoms.indexOf('btnUpdate') !== -1;
                            self.RoleBtnSearch = response.Buttoms.indexOf('btnSearch') !== -1;
                        }
                        self.ListCity = (response && response.Citys) ? response.Citys : [];
                        self.ListDuAn = (response && response.DuAns) ? response.DuAns : [];
                        if (self.ListDuAn.length > 0 && !self.modelSearch.MaDuAn) {
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

            LoadPage: function (genTable) {
                var self = this;
                if (!self.modelSearch.Year || self.modelSearch.Year == 0) {
                    if (window.toastr) toastr.error("Vui lòng chọn năm!");
                    return;
                }

                if (!self.SauThang || self.SauThang <= 0) {
                    if (window.toastr) toastr.error("Vui lòng chọn kỳ báo cáo!");
                    return;
                }

                self.modelSearch.Months = self.SauThang == 1 ? '1,2,3,4,5,6' : '7,8,9,10,11,12';
                self.modelSearch.CityCodes = (self.ListCityCode && self.ListCityCode.length > 0) ? self.ListCityCode.join(',') : '';
                self.modelSearch.MaNhomTBH = (self.ListMaNhomTBH && self.ListMaNhomTBH.length > 0) ? self.ListMaNhomTBH.join(',') : '';

                if (window.showToast) showToast();
                self.ListData = [];

                $.ajax({
                    type: 'POST',
                    url: '/BaoCaoChiSo/SearchData',
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
                if (!self.modelSearch.Year || self.modelSearch.Year == 0) {
                    if (window.toastr) toastr.error("Vui lòng chọn năm!");
                    return;
                }

                if (!self.SauThang || self.SauThang <= 0) {
                    if (window.toastr) toastr.error("Vui lòng chọn kỳ báo cáo!");
                    return;
                }

                self.modelSearch.Months = self.SauThang == 1 ? '1,2,3,4,5,6' : '7,8,9,10,11,12';
                self.modelSearch.CityCodes = (self.ListCityCode && self.ListCityCode.length > 0) ? self.ListCityCode.join(',') : '';
                self.modelSearch.MaNhomTBH = (self.ListMaNhomTBH && self.ListMaNhomTBH.length > 0) ? self.ListMaNhomTBH.join(',') : '';

                var params = new URLSearchParams({
                    Months: self.modelSearch.Months || '',
                    Year: self.modelSearch.Year || '',
                    CityCodes: self.modelSearch.CityCodes || '',
                    maNhomTBHs: self.modelSearch.MaNhomTBH || '',
                    maDuAn: self.modelSearch.MaDuAn || ''
                });

                window.location.href = '/BaoCaoChiSo/ExportDataChiSo6Thang?' + params.toString();
            },

            Changecity: function (callback) {
                var self = this;
                var cityCodesStr = (self.ListCityCode && self.ListCityCode.length > 0) ? self.ListCityCode.join(',') : '';
                self.ListNhomTBH = [];
                self.ListMaNhomTBH = [];

                $.ajax({
                    type: 'POST',
                    url: '/BaoCaoThang/GetNhomTBHByCityCodes',
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
