document.addEventListener('alpine:init', function () {
    Alpine.data('alpineBaoCaoThang', function () {
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
                        if (self.modelSearch.MaDuAn) {
                            self.LoadPage(1);
                        }
                    });
                });
            },

            setTab: function (tabName) {
                var self = this;
                self.activeTab = tabName;
                if (tabName === 'chart') {
                    setTimeout(function () {
                        self.renderChart();
                    }, 100);
                }
            },

            formatNumber: function (value) {
                if (value === null || value === undefined || value === "") return '';
                return Number(value).toLocaleString('en-US');
            },

            GetBottomAction: function (callback) {
                var self = this;
                $.ajax({
                    type: 'POST',
                    url: '/BaoCaoThang/GetBottomAction',
                    contentType: 'application/json',
                    data: '{}',
                    success: function (response) {
                        if (response && response.Buttoms) {
                            self.RoleBtnUpdate = response.Buttoms.indexOf('btnUpdate') !== -1;
                            self.RoleBtnSearch = response.Buttoms.indexOf('btnSearch') !== -1;
                        }
                        self.ListCity = (response && response.Citys) ? response.Citys : [];
                        self.ListDuAn = (response && response.DuAns) ? response.DuAns : [];
                        if (!self.modelSearch.MaDuAn && self.ListDuAn.length > 0) {
                            self.modelSearch.MaDuAn = self.ListDuAn[0].maduan;
                        }
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

                if (!self.modelSearch.MaDuAn) {
                    if (window.toastr) toastr.error("Vui lòng chọn dự án!");
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
                    url: '/BaoCaoThang/SearchData',
                    cache: false,
                    data: self.modelSearch,
                    success: function (response) {
                        self.ListData = (response && response.data) ? response.data : [];
                        if (self.activeTab === 'chart') {
                            self.renderChart();
                        }
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

                if (!self.modelSearch.MaDuAn) {
                    if (window.toastr) toastr.error("Vui lòng chọn dự án!");
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
                    maDuAn: self.modelSearch.MaDuAn || ''
                });

                window.location.href = '/BaoCaoThang/ExportData?' + params.toString();
            },

            Changecity: function (callback) {
                var self = this;
                var cityCodesStr = (self.ListCityCode && self.ListCityCode.length > 0) ? self.ListCityCode.join(',') : '';
                self.ListNhomTBH = [];
                self.ListMaNhomTBH = [];

                $.ajax({
                    type: 'POST',
                    url: '/BaoCaoThang/GetNhomTBHByCityCodes',
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
            },

            renderChart: function () {
                var self = this;
                if (!self.ListData || self.ListData.length === 0) return;

                var chartData = self.ListData.filter(function (item) {
                    return item.IsShow === 'Y' && item.ThongTinBC && item.ThongTinBC.trim() !== '' && item.STT;
                }).slice(0, 6);

                var labels = chartData.map(function (item) {
                    var title = item.ThongTinBC.trim();
                    if (title.length > 35) title = title.substring(0, 32) + "...";
                    return title;
                });

                var msmData = chartData.map(function (item) { return item.MSM || 0; });
                var pudData = chartData.map(function (item) { return item.PUD || 0; });
                var swData = chartData.map(function (item) { return item.SW || 0; });

                var ctx = document.getElementById('monthlyReportChart');
                if (!ctx) return;

                if (self.chartInstance) {
                    self.chartInstance.destroy();
                }

                if (window.Chart) {
                    self.chartInstance = new Chart(ctx, {
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
            }
        };
    });
});
