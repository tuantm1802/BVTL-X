document.addEventListener('alpine:init', () => {
    Alpine.data('alpineBaoCaoNamCH07', () => ({
        modelSearch: {
            totalItems: 0,
            currentPage: 1,
            maxSize: 5,
            pageSize: 10,
            SortColumn: "ParamCode DESC",
            Year: 0,
            Years: '',
            CityCodes: '',
            MaNhomTBH: '',
            MaDuAn: 'CH07'
        },
        ListYear: [],
        ListYearCode: [],
        ListCity: [],
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
            this.ListYear.unshift({ Id: 0, Name: '-' });
            
            this.modelSearch.Year = date.getFullYear();
            this.ListYearCode.push(date.getFullYear());

            await this.GetBottomAction();
            await this.Changecity();
            
            this.$nextTick(() => {
                // Initialize manual jQuery listeners if needed for UI plugins
            });
            
            this.LoadPage(1);
        },

        formatNumber(value) {
            if (value === null || value === undefined || value === "") return '';
            return Number(value).toLocaleString('en-US');
        },

        async GetBottomAction() {
            try {
                const res = await fetch('/BaoCaoNamCH07/GetBottomAction', {
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
            if (!this.modelSearch.Year) {
                if(window.toastr) toastr.error("Vui lòng chọn năm!");
                return;
            } else {
                this.modelSearch.Years = (this.ListYearCode && this.ListYearCode.length > 0) ? this.ListYearCode.join(',') : '';
            }

            this.modelSearch.CityCodes = (this.ListCityCode && this.ListCityCode.length > 0) ? this.ListCityCode.join(',') : '';
            this.modelSearch.MaNhomTBH = (this.ListMaNhomTBH && this.ListMaNhomTBH.length > 0) ? this.ListMaNhomTBH.join(',') : '';

            if(window.showToast) showToast();
            this.ListData = [];
            
            $.ajax({
                type: 'post',
                url: '/BaoCaoNamCH07/SearchData',
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
            if (!this.modelSearch.Year) {
                if(window.toastr) toastr.error("Vui lòng chọn năm!");
                return;
            }

            this.modelSearch.CityCodes = (this.ListCityCode && this.ListCityCode.length > 0) ? this.ListCityCode.join(',') : '';
            this.modelSearch.MaNhomTBH = (this.ListMaNhomTBH && this.ListMaNhomTBH.length > 0) ? this.ListMaNhomTBH.join(',') : '';

            const params = new URLSearchParams({
                Year: this.modelSearch.Year || '',
                CityCodes: this.modelSearch.CityCodes || '',
                maNhomTBHs: this.modelSearch.MaNhomTBH || '',
                maDuAn: this.modelSearch.MaDuAn || ''
            });

            window.location.href = '/BaoCaoNamCH07/ExportData?' + params.toString();
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
                    url: '/BaoCaoNamCH07/GetNhomTBHByCityCodes',
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
