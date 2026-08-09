document.addEventListener('alpine:init', () => {
    Alpine.data('alpineBaoCao6ThangCH07', () => ({
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
            MaDuAn: 'CH07'
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

        async init() {
            const date = new Date();
            for (let i = date.getFullYear() - 5; i < date.getFullYear() + 5; i++) {
                this.ListYear.push({ Id: i, Name: i + '' });
            }
            this.modelSearch.Year = date.getFullYear();

            await this.GetBottomAction();
            await this.Changecity();
            
            this.$nextTick(() => {
                // For select2 or other jQuery bindings
            });
            
            this.LoadPage(1);
        },

        formatNumber(value) {
            if (value === null || value === undefined || value === "") return '';
            return Number(value).toLocaleString('en-US');
        },

        async GetBottomAction() {
            try {
                const res = await fetch('/BaoCao6ThangCH07/GetBottomAction', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' }
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
                this.modelSearch.MaDuAn = 'CH07';
            } catch (err) {
                console.error(err);
            }
        },

        LoadPage(genTable) {
            if (!this.modelSearch.Year || this.modelSearch.Year == 0) {
                if(window.toastr) toastr.error("Vui lòng chọn năm!");
                return;
            }

            this.modelSearch.Months = '';
            if (this.SauThang > 0) {
                if (this.SauThang == 1) {
                    this.modelSearch.Months ='1,2,3,4,5,6';
                } else {
                    this.modelSearch.Months = '7,8,9,10,11,12';
                }
            } else {
                if(window.toastr) toastr.error("Vui lòng chọn kỳ báo cáo!");
                return;
            }

            this.modelSearch.CityCodes = (this.ListCityCode && this.ListCityCode.length > 0) ? this.ListCityCode.join(',') : '';
            this.modelSearch.MaNhomTBH = (this.ListMaNhomTBH && this.ListMaNhomTBH.length > 0) ? this.ListMaNhomTBH.join(',') : '';

            if(window.showToast) showToast();
            this.ListData = [];
            
            $.ajax({
                type: 'post',
                url: '/BaoCao6ThangCH07/SearchData',
                cache: false,
                data: this.modelSearch,
                success: (response) => {
                    this.ListData = response.data;
                    if(window.hideLoading) hideLoading();
                },
                error: () => {
                    if(window.hideLoading) hideLoading();
                }
            });
        },

        Refesh() {
            this.LoadPage(0);
        },

        ExportExcel() {
            if (!this.modelSearch.Year || this.modelSearch.Year == 0) {
                if(window.toastr) toastr.error("Vui lòng chọn năm!");
                return;
            }

            this.modelSearch.Months = '';
            if (this.SauThang > 0) {
                if (this.SauThang == 1) {
                    this.modelSearch.Months ='1,2,3,4,5,6';
                } else {
                    this.modelSearch.Months = '7,8,9,10,11,12';
                }
            } else {
                if(window.toastr) toastr.error("Vui lòng chọn kỳ báo cáo!");
                return;
            }

            this.modelSearch.CityCodes = (this.ListCityCode && this.ListCityCode.length > 0) ? this.ListCityCode.join(',') : '';
            this.modelSearch.MaNhomTBH = (this.ListMaNhomTBH && this.ListMaNhomTBH.length > 0) ? this.ListMaNhomTBH.join(',') : '';

            const params = new URLSearchParams({
                Months: this.modelSearch.Months || '',
                Year: this.modelSearch.Year || '',
                CityCodes: this.modelSearch.CityCodes || '',
                maNhomTBHs: this.modelSearch.MaNhomTBH || '',
                maDuAn: this.modelSearch.MaDuAn || ''
            });

            window.location.href = '/BaoCao6ThangCH07/ExportData?' + params.toString();
        },

        Changecity() {
            let cityCodesStr = '';
            if (this.ListCityCode && this.ListCityCode.length > 0) {
                cityCodesStr = this.ListCityCode.join(',');
            }
            this.ListNhomTBH = [];
            this.ListMaNhomTBH = [];

            return new Promise((resolve) => {
                $.ajax({
                    type: 'post',
                    url: '/BaoCao6ThangCH07/GetNhomTBHByCityCodes',
                    cache: false,
                    data: { CityCodes: cityCodesStr },
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
