function baoCaoTuanComponent() {
    return {
        modelSearch: {
            totalItems: 0,
            currentPage: 1,
            maxSize: 5,
            pageSize: 10,
            SortColumn: "ParamCode DESC",
            Year: new Date().getFullYear(),
            Week: 1,
            MaDuAn: '',
            CityCodes: '',
            MaNhomTBH: ''
        },
        Week: 1,
        ListYear: [],
        ListWeek: [],
        ListCity: [],
        ListCityCode: [],
        ListNhomTBH: [],
        ListMaNhomTBH: [],
        ListDuAn: [],
        ListData: [],
        RoleBtnUpdate: false,
        RoleBtnSearch: false,
        
        // Tab and Chart
        activeTab: 'table',
        chartInstance: null,

        init() {
            var date = new Date();
            var tmpList = [];
            for (var i = date.getFullYear() - 5; i < date.getFullYear() + 5; i++) {
                tmpList.push({ Id: i, Name: i + '' });
            }
            this.ListYear = tmpList;

            var tmpWeeks = [];
            for (var w = 1; w <= 53; w++) {
                tmpWeeks.push({ Id: w, Name: "Tuần " + w });
            }
            this.ListWeek = tmpWeeks;
            
            // Calculate current week of year
            var oneJan = new Date(date.getFullYear(), 0, 1);
            var numberOfDays = Math.floor((date - oneJan) / (24 * 60 * 60 * 1000));
            var currentWeek = Math.ceil((date.getDay() + 1 + numberOfDays) / 7);
            if (currentWeek > 53) currentWeek = 53;
            this.Week = currentWeek;

            this.getBottomAction();
            this.changeCity();
            
            // Wait for data to load, then load page
            setTimeout(() => {
                this.loadPage(1);
            }, 300);
        },

        setTab(tabName) {
            this.activeTab = tabName;
            if (tabName == 'chart') {
                setTimeout(() => {
                    this.renderChart();
                }, 100);
            }
        },

        getBottomAction() {
            var self = this;
            $.ajax({
                type: 'post',
                url: '/BaoCaoTuan/GetBottomAction',
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
                url: '/BaoCaoTuan/GetNhomTBHByCityCodes',
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
            if (this.Week == null || this.Week == '') {
                toastr.error("Vui lòng chọn tuần!");
                return;
            }

            this.modelSearch.Week = this.Week;
            this.modelSearch.CityCodes = (this.ListCityCode || []).join(',');
            this.modelSearch.MaNhomTBH = (this.ListMaNhomTBH || []).join(',');

            showToast();
            var self = this;
            $.ajax({
                type: 'post',
                url: '/BaoCaoTuan/SearchData',
                data: { modelSearch: this.modelSearch },
                success: function (response) {
                    if (response.Error == false) {
                        self.ListData = response.data || [];
                        if (self.activeTab === 'chart') {
                            setTimeout(() => { self.renderChart(); }, 100);
                        }
                    } else {
                        toastr.error(response.Title);
                    }
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

            this.modelSearch.CityCodes = (this.ListCityCode || []).join(',');
            this.modelSearch.MaNhomTBH = (this.ListMaNhomTBH || []).join(',');

            var url = '/BaoCaoTuan/ExportData?Year=' + this.modelSearch.Year +
                '&Week=' + this.Week +
                '&CityCodes=' + (this.modelSearch.CityCodes || '') +
                '&maNhomTBHs=' + (this.modelSearch.MaNhomTBH || '') +
                '&maDuAn=' + (this.modelSearch.MaDuAn || '');
            window.location.href = url;
        },

        renderChart() {
            if (!this.ListData || this.ListData.length === 0) {
                return;
            }

            var chartData = this.ListData.filter(function (item) {
                return item.IsShow === 'Y' && item.ThongTinBC && item.ThongTinBC.trim() !== '' && item.STT;
            });

            chartData = chartData.slice(0, 6);

            var labels = chartData.map(function (item) {
                var title = item.ThongTinBC.trim();
                if (title.length > 35) {
                    title = title.substring(0, 32) + "...";
                }
                return title;
            });

            var msmData = chartData.map(function (item) { return item.MSM || 0; });
            var pudData = chartData.map(function (item) { return item.PUD || 0; });
            var swData = chartData.map(function (item) { return item.SW || 0; });

            var ctx = document.getElementById('weeklyReportChart');
            if (!ctx) return;

            if (this.chartInstance) {
                this.chartInstance.destroy();
            }

            this.chartInstance = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [
                        {
                            label: 'MSM (Nam quan hệ đồng tính)',
                            data: msmData,
                            backgroundColor: '#0d9488',
                            borderColor: '#0f766e',
                            borderWidth: 1
                        },
                        {
                            label: 'PUD (Người tiêm chích ma túy)',
                            data: pudData,
                            backgroundColor: '#374151',
                            borderColor: '#1f2937',
                            borderWidth: 1
                        },
                        {
                            label: 'SW (Người bán dâm)',
                            data: swData,
                            backgroundColor: '#14b8a6',
                            borderColor: '#0d9488',
                            borderWidth: 1
                        }
                    ]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            position: 'top',
                            labels: {
                                font: {
                                    family: 'Times New Roman',
                                    size: 12
                                }
                            }
                        }
                    },
                    scales: {
                        x: {
                            grid: {
                                display: false
                            },
                            ticks: {
                                font: {
                                    family: 'Times New Roman',
                                    size: 11
                                }
                            }
                        },
                        y: {
                            beginAtZero: true,
                            ticks: {
                                font: {
                                    family: 'Times New Roman'
                                }
                            }
                        }
                    }
                }
            });
        }
    }
}
