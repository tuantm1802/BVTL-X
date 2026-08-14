function baoCao6ThangComponent() {
    return {
        modelSearch: {
            totalItems: 0,
            currentPage: 1,
            maxSize: 5,
            pageSize: 10,
            SortColumn: "ParamCode DESC",
            Year: new Date().getFullYear(),
            Months: '',
            MaDuAn: '',
            CityCodes: '',
            MaNhomTBH: ''
        },
        SauThang: 1,
        ListYear: [],
        List6Thang: [
            { Id: 1, Name: '6 tháng đầu năm' },
            { Id: 2, Name: '6 tháng cuối năm' }
        ],
        ListCity: [],
        ListCityCode: [],
        ListNhomTBH: [],
        ListMaNhomTBH: [],
        ListDuAn: [],
        ListData: [],
        RoleBtnUpdate: false,
        RoleBtnSearch: false,

        init() {
            var date = new Date();
            var tmpList = [];
            for (var i = date.getFullYear() - 5; i < date.getFullYear() + 5; i++) {
                tmpList.push({ Id: i, Name: i + '' });
            }
            this.ListYear = tmpList;
            
            this.getBottomAction();
            this.changeCity();
            
            // Wait for data to load, then load page
            setTimeout(() => {
                this.loadPage(1);
            }, 300);
        },

        getBottomAction() {
            var self = this;
            $.ajax({
                type: 'post',
                url: '/BaoCao6Thang/GetBottomAction',
                data: {},
                success: function (response) {
                    if (response.Buttoms != null) {
                        response.Buttoms.forEach(function (item) {
                            if (item == 'btnUpdate') self.RoleBtnUpdate = true;
                            if (item == 'btnSearch') self.RoleBtnSearch = true;
                        });
                    }
                    self.ListDuAn = response.DuAns || [];
                    self.ListCity = response.Citys || [];
                    if (self.ListDuAn.length > 0) {
                        self.modelSearch.MaDuAn = self.ListDuAn[0].maduan;
                    }
                }
            });
        },

        changeCity() {
            var self = this;
            var cityCodes = (this.ListCityCode || []).join(',');
            
            this.ListNhomTBH = [];
            this.ListMaNhomTBH = [];
            
            $.ajax({
                type: 'post',
                url: '/BaoCao6Thang/GetNhomTBHByCityCodes',
                cache: false,
                async: false,
                data: { CityCodes: cityCodes },
                success: function (response) {
                    self.ListNhomTBH = response.NhomTBHs || [];
                }
            });
        },

        loadPage(genTable) {
            if (this.modelSearch.Year == null || this.modelSearch.Year == 0) {
                toastr.error("Vui lòng chọn năm!");
                return;
            }
            if (this.modelSearch.MaDuAn == null || this.modelSearch.MaDuAn == '') {
                toastr.error("Vui lòng chọn dự án!");
                return;
            }
            if (this.SauThang > 0) {
                this.modelSearch.Months = (this.SauThang == 1) ? '1,2,3,4,5,6' : '7,8,9,10,11,12';
            } else {
                toastr.error("Vui lòng chọn kỳ báo cáo!");
                return;
            }

            this.modelSearch.CityCodes = (this.ListCityCode || []).join(',');
            this.modelSearch.MaNhomTBH = (this.ListMaNhomTBH || []).join(',');

            showToast();
            var self = this;
            $.ajax({
                type: 'post',
                url: '/BaoCao6Thang/SearchData',
                cache: false,
                async: false,
                data: this.modelSearch,
                success: function (response) {
                    self.ListData = response.data || [];
                }
            });
            hideLoading();
        },

        exportExcel() {
            if (this.modelSearch.Year == null || this.modelSearch.Year == 0) {
                toastr.error("Vui lòng chọn năm!");
                return;
            }
            if (this.modelSearch.MaDuAn == null || this.modelSearch.MaDuAn == '') {
                toastr.error("Vui lòng chọn dự án!");
                return;
            }
            if (this.SauThang > 0) {
                this.modelSearch.Months = (this.SauThang == 1) ? '1,2,3,4,5,6' : '7,8,9,10,11,12';
            } else {
                toastr.error("Vui lòng chọn kỳ báo cáo!");
                return;
            }

            this.modelSearch.CityCodes = (this.ListCityCode || []).join(',');
            this.modelSearch.MaNhomTBH = (this.ListMaNhomTBH || []).join(',');

            window.location.href = '/BaoCao6Thang/ExportData?Months=' + this.modelSearch.Months + '&Year=' + this.modelSearch.Year
                + '&CityCodes=' + (this.modelSearch.CityCodes || '')
                + '&maNhomTBHs=' + (this.modelSearch.MaNhomTBH || '')
                + '&maDuAn=' + (this.modelSearch.MaDuAn || '');
        }
    }
}
