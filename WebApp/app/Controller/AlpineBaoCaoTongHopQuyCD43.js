document.addEventListener('alpine:init', () => {
    Alpine.data('alpineBaoCaoTongHopQuyCD43', () => ({
        modelSearch: {
            totalItems: 0,
            currentPage: 1,
            maxSize: 5,
            pageSize: 10,
            SortColumn: "ParamCode DESC",
            TuNam: 0,
            DenNam: 0,
            TuThang: 0,
            DenThang: 0,
            CityCodes: '',
            MaNhomTBH: '',
            MaDuAn: 'CD43'
        },
        ListYear: [],
        ListCity: [],
        ListCityCode: [],
        ListMaNhomTBH: '',
        ListNhomTBH: [],
        ListDuAn: [],
        ListData: [],
        ListQuy: [],
        
        TuQuy: 'I',
        DenQuy: 'I',
        
        RoleBtnUpdate: false,
        RoleBtnSearch: false,

        async init() {
            const date = new Date();
            for (let i = date.getFullYear() - 5; i < date.getFullYear() + 5; i++) {
                this.ListYear.push({ Id: i, Name: i + '' });
            }
            
            const month = date.getMonth() + 1;
            if (month >= 1 && month <= 3) {
                this.TuQuy = "I";
                this.DenQuy = "I";
            } else if (month >= 4 && month <= 6) {
                this.TuQuy = "II";
                this.DenQuy = "II";
            } else if (month >= 7 && month <= 9) {
                this.TuQuy = "III";
                this.DenQuy = "III";
            } else {
                this.TuQuy = "IV";
                this.DenQuy = "IV";
            }
            
            this.modelSearch.TuNam = date.getFullYear();
            this.modelSearch.DenNam = date.getFullYear();

            await this.GetBottomAction();
            await this.Changecity();
            
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
                const res = await fetch('/BaoCaoCD43/GetBottomAction', {
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
                this.modelSearch.MaDuAn = 'CD43';
            } catch (err) {
                console.error(err);
            }
        },

        LoadPage(genTable) {
            if (!this.modelSearch.TuNam || this.modelSearch.TuNam == 0) {
                if(window.toastr) toastr.error("Vui lòng chọn từ năm!");
                return;
            }

            if (!this.modelSearch.DenNam || this.modelSearch.DenNam == 0) {
                if(window.toastr) toastr.error("Vui lòng chọn đến năm!");
                return;
            }

            if (this.modelSearch.TuNam > this.modelSearch.DenNam) {
                if(window.toastr) toastr.error("Vui lòng chọn từ năm < đến năm!");
                return;
            }

            if (!this.TuQuy) {
                if(window.toastr) toastr.error("Vui lòng chọn từ quý!");
                return;
            } else {
                if (this.TuQuy == 'I') {
                    this.modelSearch.TuThang = 1;
                } else if (this.TuQuy == 'II') {
                    this.modelSearch.TuThang = 4;
                } else if (this.TuQuy == 'III') {
                    this.modelSearch.TuThang = 7;
                } else if (this.TuQuy == 'IV') {
                    this.modelSearch.TuThang = 10;
                }
            }

            if (!this.DenQuy) {
                if(window.toastr) toastr.error("Vui lòng chọn đến quý!");
                return;
            } else {
                if (this.DenQuy == 'I') {
                    this.modelSearch.DenThang = 3;
                } else if (this.DenQuy == 'II') {
                    this.modelSearch.DenThang = 6;
                } else if (this.DenQuy == 'III') {
                    this.modelSearch.DenThang = 9;
                } else if (this.DenQuy == 'IV') {
                    this.modelSearch.DenThang = 12;
                }
            }

            if (this.modelSearch.TuNam == this.modelSearch.DenNam && this.modelSearch.TuThang > this.modelSearch.DenThang) {
                if(window.toastr) toastr.error("Vui lòng chọn từ quý <= đến quý!");
                return;
            }
            
            if (this.ListCityCode && this.ListCityCode.length > 0) {
                this.modelSearch.CityCodes = this.ListCityCode.join(',');
            } else {
                this.modelSearch.CityCodes = '';
            }

            this.modelSearch.MaNhomTBH = this.ListMaNhomTBH || '';

            if(window.showToast) showToast();
            this.ListData = [];
            
            $.ajax({
                type: 'post',
                url: '/BaoCaoCD43/SearchDataBaoCaoHoatDong',
                cache: false,
                data: this.modelSearch,
                success: (response) => {
                    this.ListData = response.data;
                    if (response.data != null && response.data.length > 0) {
                        this.ListQuy = response.data[0].ListQuy;
                    }
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
            if (!this.modelSearch.TuNam || this.modelSearch.TuNam == 0) {
                if(window.toastr) toastr.error("Vui lòng chọn từ năm!");
                return;
            }

            if (!this.modelSearch.DenNam || this.modelSearch.DenNam == 0) {
                if(window.toastr) toastr.error("Vui lòng chọn đến năm!");
                return;
            }

            if (!this.TuQuy) {
                if(window.toastr) toastr.error("Vui lòng chọn từ quý!");
                return;
            } else {
                if (this.TuQuy == 'I') {
                    this.modelSearch.TuThang = 1;
                } else if (this.TuQuy == 'II') {
                    this.modelSearch.TuThang = 4;
                } else if (this.TuQuy == 'III') {
                    this.modelSearch.TuThang = 7;
                } else if (this.TuQuy == 'IV') {
                    this.modelSearch.TuThang = 10;
                }
            }

            if (!this.DenQuy) {
                if(window.toastr) toastr.error("Vui lòng chọn đến quý!");
                return;
            } else {
                if (this.DenQuy == 'I') {
                    this.modelSearch.DenThang = 3;
                } else if (this.DenQuy == 'II') {
                    this.modelSearch.DenThang = 6;
                } else if (this.DenQuy == 'III') {
                    this.modelSearch.DenThang = 9;
                } else if (this.DenQuy == 'IV') {
                    this.modelSearch.DenThang = 12;
                }
            }

            this.modelSearch.CityCodes = (this.ListCityCode && this.ListCityCode.length > 0) ? this.ListCityCode.join(',') : '';
            this.modelSearch.MaNhomTBH = this.ListMaNhomTBH || '';

            const params = new URLSearchParams({
                TuThang: this.modelSearch.TuThang || '',
                TuNam: this.modelSearch.TuNam || '',
                DenThang: this.modelSearch.DenThang || '',
                DenNam: this.modelSearch.DenNam || '',
                CityCodes: this.modelSearch.CityCodes || '',
                maNhomTBHs: this.modelSearch.MaNhomTBH || ''
            });

            window.location.href = '/BaoCaoCD43/ExportDataTongHopQuy?' + params.toString();
        },

        Changecity() {
            let cityCodesStr = '';
            if (this.ListCityCode && this.ListCityCode.length > 0) {
                cityCodesStr = this.ListCityCode.join(',');
            }
            this.ListNhomTBH = [];
            this.ListMaNhomTBH = '';

            return new Promise((resolve) => {
                $.ajax({
                    type: 'post',
                    url: '/BaoCaoCD43/GetNhomTBHByMaNhomMap',
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
