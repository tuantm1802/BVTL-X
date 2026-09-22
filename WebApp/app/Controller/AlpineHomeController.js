document.addEventListener('alpine:init', function () {
    Alpine.data('alpineHome', function () {
        return {
            activeTab: 'visual', // 'visual' | 'tables'
            cityMode: 'NEW34',   // 'NEW34' (NQ 202/2025) hoặc 'OLD63' (Lịch sử)
            selectedTinh: '',
            selectedTinhName: 'Toàn bộ Tỉnh/Thành',
            selectedNhom: '',
            selectedNhomName: 'Toàn bộ Nhóm',
            selectedAgeTable1: '',
            fromDate: '',
            toDate: '',
            ListCity: [],
            ListNhomAll: [],
            ListNhom: [],
            lastSyncTime: 'Đang tải...',
            isLoading: false,

            // Dashboard Data Stores
            overview: {
                TongKhachHang: 0,
                TongSangLocQST: 0,
                QSTNguyCoCao: 0,
                TyLeQSTNguyCoCao: 0,
                TongKhamSKTT: 0,
                TongLuotKhamSKTT: 0,
                TongTuVanL1: 0,
                TongHoTroXH: 0,
                TongTaiLieuPhat: 0
            },
            byTargetGroup: [],
            byAgeGroup: [],
            mentalHealth: [],
            byProvince: [],
            cascadeFunnel: {
                Step1_TiepCanTruyenThong: 0,
                Step2_SangLocQST: 0,
                Step3_NguyCoCaoQST: 0,
                Step4_TuVanTamLy: 0,
                Step5_KhamChuyenKhoa: 0,
                Step6_TaiKhamSKTT: 0
            },
            socialSupport: {
                TongNhanHoTro: 0,
                HoTroBHYT: 0,
                HoTroMethadone: 0,
                XetNghiemHIV: 0,
                STIs: 0,
                ViemGan: 0
            },

            // Chart references
            chartInstances: {},

            init: function () {
                window.alpineHomeInstance = this;
                this.loadFilterData();
                this.loadDashboardData();
            },

            setCityMode: function (mode) {
                if (this.cityMode !== mode) {
                    this.cityMode = mode;
                    this.selectedTinh = '';
                    this.selectedTinhName = 'Toàn bộ Tỉnh/Thành';
                    this.selectedNhom = '';
                    this.selectedNhomName = 'Toàn bộ Nhóm';
                    this.loadFilterData();
                    this.loadDashboardData();
                }
            },

            loadFilterData: function () {
                var self = this;
                $.ajax({
                    type: 'POST',
                    url: '/Home/GetFilterData',
                    data: { cityMode: self.cityMode },
                    success: function (response) {
                        if (response && response.Success) {
                            self.ListCity = response.Cities || [];
                            self.ListNhomAll = response.Nhoms || [];
                            self.ListNhom = self.ListNhomAll;
                        }
                    }
                });
            },

            selectCity: function (code, name) {
                this.selectedTinh = code || '';
                this.selectedNhom = '';
                this.selectedNhomName = 'Toàn bộ Nhóm';

                if (this.selectedTinh) {
                    var foundCity = this.ListCity.find(function (c) { return c.CityCode === code; });
                    this.selectedTinhName = foundCity ? foundCity.CityName : (name || code);
                    var mappedCodes = (foundCity && foundCity.OldCodes && foundCity.OldCodes.length) ? foundCity.OldCodes : [code];
                    this.ListNhom = this.ListNhomAll.filter(function (n) {
                        return !n.CityCode || mappedCodes.indexOf(n.CityCode) !== -1;
                    });
                } else {
                    this.selectedTinhName = 'Toàn bộ Tỉnh/Thành';
                    this.ListNhom = this.ListNhomAll;
                }

                this.loadDashboardData();
            },

            selectNhom: function (code, name) {
                this.selectedNhom = code || '';
                if (!this.selectedNhom) {
                    this.selectedNhomName = 'Toàn bộ Nhóm';
                } else {
                    var found = this.ListNhomAll.find(function (n) { return n.MaNhom === code; });
                    this.selectedNhomName = found ? (found.TenNhom + ' (' + (found.CityCode || '') + ')') : (name || 'Toàn bộ Nhóm');
                }
                this.loadDashboardData();
            },

            resetFilters: function () {
                this.selectedTinh = '';
                this.selectedTinhName = 'Toàn bộ Tỉnh/Thành';
                this.selectedNhom = '';
                this.selectedNhomName = 'Toàn bộ Nhóm';
                this.selectedAgeTable1 = '';
                this.fromDate = '';
                this.toDate = '';
                this.ListNhom = this.ListNhomAll;
                this.loadDashboardData();
            },

            loadDashboardData: function () {
                var self = this;
                self.isLoading = true;
                if (window.showToast) showToast();

                var params = {
                    cityCode: self.selectedTinh || null,
                    maNhom: self.selectedNhom || null,
                    fromDate: self.fromDate || null,
                    toDate: self.toDate || null,
                    nhomTuoiTable1: self.selectedAgeTable1 || null,
                    cityMode: self.cityMode
                };

                $.ajax({
                    type: 'POST',
                    url: '/Home/GetDashboardCD45Data',
                    data: params,
                    success: function (response) {
                        if (response && response.Success && response.Data) {
                            var d = response.Data;
                            self.overview = d.Overview || self.overview;
                            self.byTargetGroup = d.ByTargetGroup || [];
                            self.byAgeGroup = d.ByAgeGroup || [];
                            self.mentalHealth = d.MentalHealth || [];
                            self.byProvince = d.ByProvince || [];
                            self.cascadeFunnel = d.CascadeFunnel || self.cascadeFunnel;
                            self.socialSupport = d.SocialSupport || self.socialSupport;

                            if (d.Overview && d.Overview.LastSyncTimeString) {
                                self.lastSyncTime = d.Overview.LastSyncTimeString;
                            } else {
                                self.lastSyncTime = 'Mới cập nhật';
                            }

                            self.$nextTick(function () {
                                self.renderAllCharts();
                            });
                        }
                    },
                    error: function () {
                        self.lastSyncTime = 'Chưa xác định';
                    },
                    complete: function () {
                        self.isLoading = false;
                        if (window.hideLoading) hideLoading();
                    }
                });
            },

            switchTab: function (tab) {
                this.activeTab = tab;
                if (tab === 'visual') {
                    var self = this;
                    setTimeout(function () {
                        self.renderAllCharts();
                    }, 100);
                }
            },

            destroyChart: function (key) {
                if (this.chartInstances[key]) {
                    this.chartInstances[key].destroy();
                    delete this.chartInstances[key];
                }
            },

            renderAllCharts: function () {
                if (typeof Chart === 'undefined') return;
                this.renderFunnelChart();
                this.renderQstTargetGroupChart();
                this.renderQstAgeGroupChart();
                this.renderProvinceDonutChart();
                this.renderPcl5Chart();
            },

            // 1. Phễu dịch vụ chăm sóc SKTT
            renderFunnelChart: function () {
                var el = document.getElementById('chartFunnel');
                if (!el) return;
                this.destroyChart('funnel');

                var f = this.cascadeFunnel;
                var labels = [
                    '1. Tiếp cận TT',
                    '2. Sàng lọc QST',
                    '3. QST Nguy cơ cao',
                    '4. Tư vấn L1',
                    '5. Khám SKTT',
                    '6. Tái khám'
                ];
                var dataVals = [
                    f.Step1_TiepCanTruyenThong || 0,
                    f.Step2_SangLocQST || 0,
                    f.Step3_NguyCoCaoQST || 0,
                    f.Step4_TuVanTamLy || 0,
                    f.Step5_KhamChuyenKhoa || 0,
                    f.Step6_TaiKhamSKTT || 0
                ];

                var ctx = el.getContext('2d');
                this.chartInstances['funnel'] = new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: labels,
                        datasets: [{
                            label: 'Số khách hàng',
                            data: dataVals,
                            backgroundColor: [
                                '#3b82f6', // Blue
                                '#06b6d4', // Cyan
                                '#f59e0b', // Amber
                                '#8b5cf6', // Violet
                                '#10b981', // Emerald
                                '#ec4899'  // Pink
                            ],
                            borderRadius: 6,
                            borderSkipped: false
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            legend: { display: false },
                            tooltip: {
                                callbacks: {
                                    afterLabel: function (context) {
                                        var total = dataVals[0];
                                        if (total > 0) {
                                            var pct = ((context.parsed.y / total) * 100).toFixed(1);
                                            return 'Tỷ lệ so với tiếp cận: ' + pct + '%';
                                        }
                                        return '';
                                    }
                                }
                            }
                        },
                        scales: {
                            y: {
                                beginAtZero: true,
                                grid: { color: '#f1f5f9' }
                            },
                            x: {
                                grid: { display: false }
                            }
                        }
                    }
                });
            },

            // 2. QST theo nhóm đối tượng
            renderQstTargetGroupChart: function () {
                var el = document.getElementById('chartQstTargetGroup');
                if (!el) return;
                this.destroyChart('qstTarget');

                var labels = [];
                var m1 = [], m2 = [], m3 = [], m4 = [];

                this.byTargetGroup.forEach(function (g) {
                    labels.push(g.TenDoiTuong ? g.TenDoiTuong.split(' ')[0] : 'Khác');
                    m1.push(g.Muc1_RatCao || 0);
                    m2.push(g.Muc2_Cao || 0);
                    m3.push(g.Muc3_TrungBinh || 0);
                    m4.push(g.Muc4_Thap || 0);
                });

                var ctx = el.getContext('2d');
                this.chartInstances['qstTarget'] = new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: labels,
                        datasets: [
                            {
                                label: 'Mức 1: Rất cao (>=8)',
                                data: m1,
                                backgroundColor: '#ef4444', // Red
                                borderRadius: 4
                            },
                            {
                                label: 'Mức 2: Cao (6-7)',
                                data: m2,
                                backgroundColor: '#f97316', // Orange
                                borderRadius: 4
                            },
                            {
                                label: 'Mức 3: Trung bình (4-5)',
                                data: m3,
                                backgroundColor: '#3b82f6', // Blue
                                borderRadius: 4
                            },
                            {
                                label: 'Mức 4: Thấp (<4)',
                                data: m4,
                                backgroundColor: '#10b981', // Emerald
                                borderRadius: 4
                            }
                        ]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            legend: { position: 'top' }
                        },
                        scales: {
                            x: { grid: { display: false } },
                            y: { beginAtZero: true, grid: { color: '#f1f5f9' } }
                        }
                    }
                });
            },

            // 3. QST theo nhóm tuổi
            renderQstAgeGroupChart: function () {
                var el = document.getElementById('chartQstAgeGroup');
                if (!el) return;
                this.destroyChart('qstAge');

                var labels = [];
                var m1 = [], m2 = [], m3 = [], m4 = [];

                this.byAgeGroup.forEach(function (a) {
                    labels.push(a.NhomTuoi);
                    m1.push(a.Muc1 || 0);
                    m2.push(a.Muc2 || 0);
                    m3.push(a.Muc3 || 0);
                    m4.push(a.Muc4 || 0);
                });

                var ctx = el.getContext('2d');
                this.chartInstances['qstAge'] = new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: labels,
                        datasets: [
                            { label: 'Mức 1 (>=8)', data: m1, backgroundColor: '#ef4444' },
                            { label: 'Mức 2 (6-7)', data: m2, backgroundColor: '#f97316' },
                            { label: 'Mức 3 (4-5)', data: m3, backgroundColor: '#3b82f6' },
                            { label: 'Mức 4 (<4)', data: m4, backgroundColor: '#10b981' }
                        ]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            tooltip: {
                                callbacks: {
                                    footer: function (tooltipItems) {
                                        var sum = 0;
                                        tooltipItems.forEach(function (ti) {
                                            sum += ti.parsed.y;
                                        });
                                        return 'Tổng cộng: ' + sum.toLocaleString();
                                    }
                                }
                            }
                        },
                        scales: {
                            x: { stacked: true, grid: { display: false } },
                            y: { 
                                stacked: true, 
                                beginAtZero: true, 
                                grid: { color: '#f1f5f9' },
                                grace: '8%'
                            }
                        }
                    },
                    plugins: [{
                        id: 'stackedBarTotals',
                        afterDatasetsDraw: function (chart) {
                            var ctx = chart.ctx;
                            chart.data.labels.forEach(function (label, index) {
                                var total = 0;
                                var barX = null;
                                var minY = null;
                                chart.data.datasets.forEach(function (dataset, dIdx) {
                                    var val = dataset.data[index] || 0;
                                    total += val;
                                    var meta = chart.getDatasetMeta(dIdx);
                                    if (meta && meta.data[index] && !meta.hidden && val > 0) {
                                        var el = meta.data[index];
                                        barX = el.x;
                                        if (minY === null || el.y < minY) {
                                            minY = el.y;
                                        }
                                    }
                                });
                                if (total > 0 && barX !== null && minY !== null) {
                                    ctx.save();
                                    ctx.textAlign = 'center';
                                    ctx.textBaseline = 'bottom';
                                    ctx.font = 'bold 11px "Segoe UI", sans-serif';
                                    ctx.fillStyle = '#1e293b';
                                    ctx.fillText(total.toLocaleString(), barX, minY - 3);
                                    ctx.restore();
                                }
                            });
                        }
                    }]
                });
            },

            // 4. Phân bố theo tỉnh thành
            renderProvinceDonutChart: function () {
                var el = document.getElementById('chartProvince');
                if (!el) return;
                this.destroyChart('province');

                var labels = [];
                var dataVals = [];
                this.byProvince.forEach(function (p) {
                    labels.push(p.CityName || p.CityCode);
                    dataVals.push(p.TongKH || 0);
                });

                var ctx = el.getContext('2d');
                this.chartInstances['province'] = new Chart(ctx, {
                    type: 'doughnut',
                    data: {
                        labels: labels,
                        datasets: [{
                            data: dataVals,
                            backgroundColor: [
                                '#3b82f6', '#10b981', '#f59e0b', '#8b5cf6', '#ec4899', '#06b6d4'
                            ],
                            borderWidth: 2,
                            borderColor: '#ffffff'
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            legend: { position: 'bottom' }
                        },
                        cutout: '65%'
                    }
                });
            },

            // 5. Sang chấn PTSD (PCL-5)
            renderPcl5Chart: function () {
                var el = document.getElementById('chartPcl5');
                if (!el) return;
                this.destroyChart('pcl5');

                var labels = [];
                var pos = [];
                var neg = [];

                this.mentalHealth.forEach(function (m) {
                    labels.push(m.TenDoiTuong || 'Khác');
                    pos.push(m.PCL5_DuongTinh || 0);
                    neg.push(m.PCL5_AmTinh || 0);
                });

                var ctx = el.getContext('2d');
                this.chartInstances['pcl5'] = new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: labels,
                        datasets: [
                            {
                                label: 'PTSD Dương tính (Nguy cơ)',
                                data: pos,
                                backgroundColor: '#ef4444',
                                borderRadius: 4
                            },
                            {
                                label: 'PTSD Âm tính',
                                data: neg,
                                backgroundColor: '#10b981',
                                borderRadius: 4
                            }
                        ]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            legend: { position: 'top' }
                        },
                        scales: {
                            x: { stacked: true, grid: { display: false } },
                            y: { stacked: true, beginAtZero: true, grid: { color: '#f1f5f9' } }
                        }
                    }
                });
            },

            // Helper tính tổng Bảng 1
            getTotalTable1: function (key) {
                if (!this.byTargetGroup || this.byTargetGroup.length === 0) return 0;
                return this.byTargetGroup.reduce(function (sum, item) {
                    return sum + (item[key] || 0);
                }, 0);
            },

            getTotalTable1NguyCoCaoPct: function () {
                var screened = this.getTotalTable1('SoKHSangLoc');
                if (!screened) return '0%';
                var risk = this.getTotalTable1('Muc1_RatCao') + this.getTotalTable1('Muc2_Cao');
                return (risk / screened * 100).toFixed(1) + '%';
            }
        };
    });
});
