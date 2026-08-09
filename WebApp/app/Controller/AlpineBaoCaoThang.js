document.addEventListener('alpine:init', () => {
    Alpine.data('alpineBaoCaoThang', () => ({
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
        ListThang: [],
        Thangs: [],
        ListCityCode: [],
        ListMaNhomTBH: [],
        ListNhomTBH: [],
        ListDuAn: [],
        ListData: [],
        
        activeTab: 'table',
        chartInstance: null,
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

            if (!this.Thangs || this.Thangs.length === 0) {
                this.Thangs = [date.getMonth() + 1];
            }

            await this.GetBottomAction();
            await this.Changecity();

            if (this.modelSearch.MaDuAn) {
                this.LoadPage(1);
            }
        },

        setTab(tabName) {
            this.activeTab = tabName;
            if (tabName === 'chart') {
                setTimeout(() => {
                    this.renderChart();
                }, 100);
            }
        },

        formatNumber(value) {
            if (value === null || value === undefined || value === "") return '';
            return Number(value).toLocaleString('en-US');
        },

        async GetBottomAction() {
            try {
                const res = await fetch('/BaoCaoThang/GetBottomAction', {
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

            if (!this.modelSearch.MaDuAn) {
                if(window.toastr) toastr.error("Vui lòng chọn dự án!");
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
                url: '/BaoCaoThang/SearchData',
                cache: false,
                data: this.modelSearch,
                success: (response) => {
                    this.ListData = response.data;
                    if (this.activeTab === 'chart') {
                        this.renderChart();
                    }
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

            if (!this.modelSearch.MaDuAn) {
                if(window.toastr) toastr.error("Vui lòng chọn dự án!");
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

            window.location.href = '/BaoCaoThang/ExportData?' + params.toString();
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
                    url: '/BaoCaoThang/GetNhomTBHByCityCodes',
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
        },

        renderChart() {
            if (!this.ListData || this.ListData.length === 0) return;

            let chartData = this.ListData.filter(item => item.IsShow === 'Y' && item.ThongTinBC && item.ThongTinBC.trim() !== '' && item.STT);
            chartData = chartData.slice(0, 6);

            const labels = chartData.map(item => {
                let title = item.ThongTinBC.trim();
                if (title.length > 35) title = title.substring(0, 32) + "...";
                return title;
            });

            const msmData = chartData.map(item => item.MSM || 0);
            const pudData = chartData.map(item => item.PUD || 0);
            const swData = chartData.map(item => item.SW || 0);

            const ctx = document.getElementById('monthlyReportChart');
            if (!ctx) return;

            if (this.chartInstance) {
                this.chartInstance.destroy();
            }

            this.chartInstance = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [
                        { label: 'MSM', data: msmData, backgroundColor: '#0d9488', borderColor: '#0f766e', borderWidth: 1 },
                        { label: 'PUD', data: pudData, backgroundColor: '#374151', borderColor: '#1f2937', borderWidth: 1 },
                        { label: 'SW',  data: swData,  backgroundColor: '#14b8a6', borderColor: '#0d9488', borderWidth: 1 }
                    ]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false
                }
            });
        }
    }));
});
