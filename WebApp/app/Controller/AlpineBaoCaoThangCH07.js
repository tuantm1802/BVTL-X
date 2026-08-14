document.addEventListener('alpine:init', function () {
    Alpine.data('alpineBaoCaoThangCH07', function () {
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
                MaDuAn: 'CH07'
            },
            ListYear: [],
            ListCity: [],
            ListThang: [],
            Thangs: [],
            ListCityCode: [],
            ListMaNhomTBH: [],
            ListNhomTBH: [],
            ListDuAn: [],
            ListData: [],
            
            RoleBtnUpdate: false,
            RoleBtnSearch: false,

            init: function () {
                var self = this;
                var date = new Date();
                self.ListYear = [];
                for (var i = date.getFullYear() - 5; i <= date.getFullYear() + 5; i++) {
                    self.ListYear.push({ Id: i, Name: i.toString() });
                }
                self.ListThang = [];
                for (var j = 1; j <= 12; j++) {
                    self.ListThang.push({ Id: j, Name: j.toString() });
                }
                self.modelSearch.Year = date.getFullYear();

                if (!self.Thangs || self.Thangs.length === 0) {
                    self.Thangs = [date.getMonth() + 1];
                }

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
                    url: '/BaoCaoThangCH07/GetBottomAction',
                    contentType: 'application/json',
                    data: '{}',
                    success: function (response) {
                        if (response && response.Buttoms) {
                            self.RoleBtnUpdate = response.Buttoms.indexOf('btnUpdate') !== -1;
                            self.RoleBtnSearch = response.Buttoms.indexOf('btnSearch') !== -1;
                        }
                        self.ListCity = (response && response.Citys) ? response.Citys : [];
                        self.ListDuAn = (response && response.DuAns) ? response.DuAns : [];
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

                if (self.Thangs && self.Thangs.length > 0) {
                    self.modelSearch.Months = self.Thangs.join(',');
                } else {
                    if (window.toastr) toastr.error("Vui lòng chọn tháng!");
                    return;
                }
                
                self.modelSearch.CityCodes = (self.ListCityCode && self.ListCityCode.length > 0) ? self.ListCityCode.join(',') : '';
                self.modelSearch.MaNhomTBH = (self.ListMaNhomTBH && self.ListMaNhomTBH.length > 0) ? self.ListMaNhomTBH.join(',') : '';

                if (window.showToast) showToast();
                self.ListData = [];
                
                $.ajax({
                    type: 'POST',
                    url: '/BaoCaoThangCH07/SearchData',
                    cache: false,
                    data: self.modelSearch,
                    success: function (response) {
                        self.ListData = (response && response.data) ? response.data : [];
                        if (window.hideLoading) hideLoading();
                    },
                    error: function (err) {
                        if (window.hideLoading) hideLoading();
                        console.error(err);
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

                if (self.Thangs && self.Thangs.length > 0) {
                    self.modelSearch.Months = self.Thangs.join(',');
                } else {
                    if (window.toastr) toastr.error("Vui lòng chọn tháng!");
                    return;
                }

                self.modelSearch.CityCodes = (self.ListCityCode && self.ListCityCode.length > 0) ? self.ListCityCode.join(',') : '';
                self.modelSearch.MaNhomTBH = (self.ListMaNhomTBH && self.ListMaNhomTBH.length > 0) ? self.ListMaNhomTBH.join(',') : '';

                var params = new URLSearchParams({
                    Year: self.modelSearch.Year || '',
                    Months: self.modelSearch.Months || '',
                    CityCodes: self.modelSearch.CityCodes || '',
                    maNhomTBHs: self.modelSearch.MaNhomTBH || '',
                    maDuAn: self.modelSearch.MaDuAn || 'CH07'
                });

                window.location.href = '/BaoCaoThangCH07/ExportData?' + params.toString();
            },

            Changecity: function (callback) {
                var self = this;
                var cityCodesStr = (self.ListCityCode && self.ListCityCode.length > 0) ? self.ListCityCode.join(',') : '';
                self.ListNhomTBH = [];
                self.ListMaNhomTBH = [];

                $.ajax({
                    type: 'POST',
                    url: '/BaoCaoThangCH07/GetNhomTBHByCityCodes',
                    cache: false,
                    data: { CityCodes: cityCodesStr },
                    success: function (response) {
                        self.ListNhomTBH = (response && response.NhomTBHs) ? response.NhomTBHs : [];
                        if (typeof callback === 'function') callback();
                    },
                    error: function (err) {
                        console.error(err);
                        if (typeof callback === 'function') callback();
                    }
                });
            }
        };
    });
});
