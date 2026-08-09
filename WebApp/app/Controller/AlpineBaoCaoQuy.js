document.addEventListener('alpine:init', () => {
    Alpine.data('alpineBaoCaoQuy', () => ({
        modelSearch: {
            totalItems: 0,
            currentPage: 1,
            maxSize: 5,
            pageSize: 10,
            SortColumn: "ParamCode DESC",
            Year: 0,
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

        async init() {
            this.ListMaNhomTBH = [];
            this.ListNhomTBH = [];

            const date = new Date();
            for (let i = date.getFullYear() - 5; i < date.getFullYear() + 5; i++) {
                this.ListYear.push({ Id: i, Name: i + '' });
            }

            const month = date.getMonth() + 1;
            if (month >= 1 && month <= 3) {
                this.Quy = 'I';
            } else if (month >= 4 && month <= 6) {
                this.Quy = 'II';
            } else if (month >= 7 && month <= 9) {
                this.Quy = 'III';
            } else if (month >= 10 && month <= 12) {
                this.Quy = 'IV';
            }

            this.modelSearch.Year = date.getFullYear();

            await this.GetBottomAction();
            await this.Changecity();

            if (this.modelSearch.MaDuAn) {
                this.LoadPage(1);
            }
        },

        formatNumber(value) {
            if (value === null || value === undefined || value === '') return '';
            return Number(value).toLocaleString('en-US');
        },

        async GetBottomAction() {
            try {
                const res = await fetch('/BaoCaoQuy/GetBottomAction', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });
                const response = await res.json();

                if (response.Buttoms != null) {
                    response.Buttoms.forEach(item => {
                        if (item === 'btnUpdate') this.RoleBtnUpdate = true;
                        if (item === 'btnSearch') this.RoleBtnSearch = true;
                    });
                }

                this.ListCity = response.Citys || [];
                this.ListDuAn = response.DuAns || [];
                if (!this.modelSearch.MaDuAn && this.ListDuAn.length > 0) {
                    this.modelSearch.MaDuAn = this.ListDuAn[0].maduan;
                }
            } catch (err) {
                console.error(err);
            }
        },

        buildSearchModel() {
            if (!this.modelSearch.Year || this.modelSearch.Year == 0) {
                if (window.toastr) toastr.error('Vui lòng chọn năm!');
                return false;
            }

            if (!this.modelSearch.MaDuAn) {
                if (window.toastr) toastr.error('Vui lòng chọn dự án!');
                return false;
            }

            this.modelSearch.Months = '';
            if (!this.Quy) {
                if (window.toastr) toastr.error('Vui lòng chọn quý!');
                return false;
            }

            if (this.Quy === 'I') {
                this.modelSearch.Months = '1,2,3';
            } else if (this.Quy === 'II') {
                this.modelSearch.Months = '4,5,6';
            } else if (this.Quy === 'III') {
                this.modelSearch.Months = '7,8,9';
            } else if (this.Quy === 'IV') {
                this.modelSearch.Months = '10,11,12';
            }

            this.modelSearch.CityCodes = (this.ListCityCode && this.ListCityCode.length > 0) ? this.ListCityCode.join(',') : '';
            this.modelSearch.MaNhomTBH = (this.ListMaNhomTBH && this.ListMaNhomTBH.length > 0) ? this.ListMaNhomTBH.join(',') : '';

            return true;
        },

        LoadPage(genTable) {
            if (!this.buildSearchModel()) {
                return;
            }

            if (window.showToast) showToast();
            this.ListData = [];

            $.ajax({
                type: 'post',
                url: '/BaoCaoQuy/SearchData',
                cache: false,
                data: this.modelSearch,
                success: (response) => {
                    this.ListData = response.data || [];
                    if (window.hideLoading) hideLoading();
                },
                error: (xhr, status, error) => {
                    console.error(error || status);
                    if (window.hideLoading) hideLoading();
                }
            });
        },

        Refesh() {
            this.LoadPage(0);
        },

        ExportExcel() {
            if (!this.buildSearchModel()) {
                return;
            }

            const params = new URLSearchParams({
                Months: this.modelSearch.Months || '',
                Year: this.modelSearch.Year || '',
                CityCodes: this.modelSearch.CityCodes || '',
                quy: this.Quy || '',
                maNhomTBHs: this.modelSearch.MaNhomTBH || '',
                maDuAn: this.modelSearch.MaDuAn || ''
            });

            window.location.href = '/BaoCaoQuy/ExportData?' + params.toString();
        },

        Changecity() {
            const cityCodesStr = (this.ListCityCode && this.ListCityCode.length > 0) ? this.ListCityCode.join(',') : '';
            this.ListNhomTBH = [];
            this.ListMaNhomTBH = [];

            return new Promise((resolve) => {
                $.ajax({
                    type: 'post',
                    url: '/BaoCaoQuy/GetNhomTBHByCityCodes',
                    cache: false,
                    data: {
                        CityCodes: cityCodesStr
                    },
                    success: (response) => {
                        this.ListNhomTBH = response.NhomTBHs || [];
                        resolve(response);
                    },
                    error: (xhr, status, error) => {
                        console.error(error || status);
                        resolve(null);
                    }
                });
            });
        }
    }));
});
