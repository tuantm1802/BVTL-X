document.addEventListener('alpine:init', () => {
    Alpine.data('alpineBaoCaoThangCH07', () => ({
        modelSearch: {
            totalItems: 0,
            currentPage: 1,
            maxSize: 5,
            pageSize: 10,
            SortColumn: "ParamCode DESC",
            Year: 0,
            Months: '',
            CityCodes: '',
            MaNhomTBH: ''
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

        async init() {
            const date = new Date();
            for (let i = date.getFullYear() - 5; i < date.getFullYear() + 5; i++) {
                this.ListYear.push({ Id: i, Name: i + '' });
            }
            for (let i = 1; i < 13; i++) {
                this.ListThang.push({ Id: i, Name: i + '' });
            }
            this.modelSearch.Year = date.getFullYear();

            await this.GetBottomAction();
            await this.Changecity();
            
            // Allow select2 jQuery bindings to initialize
            this.$nextTick(() => {
                // Initialize manual jQuery listeners if needed
            });
            
            this.LoadPage(1);
        },

        formatNumber(value) {
            if (value === null || value === undefined || value === "") return '';
            return Number(value).toLocaleString('en-US');
        },

        async GetBottomAction() {
            try {
                const res = await fetch('/BaoCaoThangCH07/GetBottomAction', {
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

        LoadPage(genTable) {
            if (!this.modelSearch.Year || this.modelSearch.Year == 0) {
                if(window.toastr) toastr.error("Vui lòng chọn năm!");
                return;
            }

            if (this.Thangs && this.Thangs.length > 0) {
                this.modelSearch.Months = this.Thangs.join(',');
            } else {
                if(window.toastr) toastr.error("Vui lòng chọn tháng!");
                return;
            }
            
            if (this.ListCityCode && this.ListCityCode.length > 0) {
                this.modelSearch.CityCodes = this.ListCityCode.join(',');
            } else {
                this.modelSearch.CityCodes = '';
            }

            if (this.ListMaNhomTBH && this.ListMaNhomTBH.length > 0) {
                this.modelSearch.MaNhomTBH = this.ListMaNhomTBH.join(',');
            } else {
                this.modelSearch.MaNhomTBH = '';
            }

            if(window.showToast) showToast();
            this.ListData = [];
            
            $.ajax({
                type: 'post',
                url: '/BaoCaoThangCH07/SearchData',
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

            if (this.Thangs && this.Thangs.length > 0) {
                this.modelSearch.Months = this.Thangs.join(',');
            } else {
                if(window.toastr) toastr.error("Vui lòng chọn tháng!");
                return;
            }

            this.modelSearch.CityCodes = (this.ListCityCode && this.ListCityCode.length > 0) ? this.ListCityCode.join(',') : '';
            this.modelSearch.MaNhomTBH = (this.ListMaNhomTBH && this.ListMaNhomTBH.length > 0) ? this.ListMaNhomTBH.join(',') : '';

            const params = new URLSearchParams({
                Year: this.modelSearch.Year || '',
                Months: this.modelSearch.Months || '',
                CityCodes: this.modelSearch.CityCodes || '',
                maNhomTBHs: this.modelSearch.MaNhomTBH || '',
                maDuAn: this.modelSearch.MaDuAn || ''
            });

            window.location.href = '/BaoCaoThangCH07/ExportData?' + params.toString();
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
                    url: '/BaoCaoThangCH07/GetNhomTBHByCityCodes',
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
