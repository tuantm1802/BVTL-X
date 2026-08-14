function baoCaoNamComponent() {
    return {
        modelSearch: {
            totalItems: 0,
            currentPage: 1,
            maxSize: 5,
            pageSize: 10,
            SortColumn: "ParamCode DESC",
            Year: new Date().getFullYear(),
            MaDuAn: '',
            CityCodes: '',
            MaNhomTBH: ''
        },
        ListYear: [],
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
            var tmpAll = { Id: 0, Name: '-' };
            tmpList.push(tmpAll);
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
                url: '/BaoCaoNam/GetBottomAction',
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
                url: '/BaoCaoNam/GetNhomTBHByCityCodes',
                cache: false,
                async: false,
                data: { CityCodes: cityCodes },
                success: function (response) {
                    self.ListNhomTBH = response.NhomTBHs || [];
                }
            });
        },

        loadPage(genTable) {
            if (this.modelSearch.Year == null) {
                toastr.error("Vui lòng chọn năm!");
                return;
            }
            if (this.modelSearch.MaDuAn == null || this.modelSearch.MaDuAn == '') {
                toastr.error("Vui lòng chọn dự án!");
                return;
            }

            this.modelSearch.CityCodes = (this.ListCityCode || []).join(',');
            this.modelSearch.MaNhomTBH = (this.ListMaNhomTBH || []).join(',');

            showToast();
            var self = this;
            $.ajax({
                type: 'post',
                url: '/BaoCaoNam/SearchData',
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
            if (this.modelSearch.Year == null) {
                toastr.error("Vui lòng chọn năm!");
                return;
            }
            if (this.modelSearch.MaDuAn == null || this.modelSearch.MaDuAn == '') {
                toastr.error("Vui lòng chọn dự án!");
                return;
            }

            this.modelSearch.CityCodes = (this.ListCityCode || []).join(',');
            this.modelSearch.MaNhomTBH = (this.ListMaNhomTBH || []).join(',');

            window.location.href = '/BaoCaoNam/ExportData?Year=' + this.modelSearch.Year
                + '&CityCodes=' + (this.modelSearch.CityCodes || '')
                + '&maNhomTBHs=' + (this.modelSearch.MaNhomTBH || '')
                + '&maDuAn=' + (this.modelSearch.MaDuAn || '');
        }
    }
}
