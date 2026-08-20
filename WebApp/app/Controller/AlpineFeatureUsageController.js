/**
 * AlpineFeatureUsageController.js
 * Quản lý giao diện Thống Kê & Phân Tích Mức Độ Khai Thác Chức Năng / Module
 */
document.addEventListener('alpine:init', () => {
    Alpine.data('featureUsageApp', () => ({
        activeTab: 'rankings', // 'rankings' | 'details' | 'roles'
        loading: false,

        // KPIs
        kpis: {
            topFeatureName: 'Đang tải...',
            topFeatureHits: 0,
            totalMonthlyMutations: 0,
            totalMonthlyExports: 0,
            systemAvgResponseTimeMs: 0
        },

        // Tab 1: Rankings & Top Features
        rankDays: 30,
        topFeatures: [],
        underutilizedFeatures: [],
        topChart: null,

        // Tab 2: Feature Usage Log / Details
        detailSearch: {
            keyword: '',
            controllerName: 'all',
            actionType: 'all',
            fromDate: '',
            toDate: '',
            currentPage: 1,
            pageSize: 10
        },
        usageDetails: [],
        usageTotal: 0,
        usageTotalPages: 1,

        // Tab 3: Role Usage
        roleDistribution: {},
        roleChart: null,

        init() {
            this.loadKPIs();
            this.loadTopFeatures();
            this.loadUnderutilizedFeatures();
            this.loadUsageDetails();
            this.loadRoleUsage();
        },

        loadKPIs() {
            $.ajax({
                url: '/SystemMonitor/GetDashboardKPIs',
                type: 'POST',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                success: (res) => {
                    if (res && res.kpi) {
                        this.kpis = {
                            topFeatureName: res.kpi.TopFeatureName || res.kpi.topFeatureName || 'Chưa có dữ liệu',
                            topFeatureHits: res.kpi.TopFeatureHits !== undefined ? res.kpi.TopFeatureHits : (res.kpi.topFeatureHits || 0),
                            totalMonthlyMutations: res.kpi.TotalMonthlyMutations !== undefined ? res.kpi.TotalMonthlyMutations : (res.kpi.totalMonthlyMutations || 0),
                            totalMonthlyExports: res.kpi.TotalMonthlyExports !== undefined ? res.kpi.TotalMonthlyExports : (res.kpi.totalMonthlyExports || 0),
                            systemAvgResponseTimeMs: res.kpi.SystemAvgResponseTimeMs !== undefined ? res.kpi.SystemAvgResponseTimeMs : (res.kpi.systemAvgResponseTimeMs || 0)
                        };
                    }
                },
                error: (err) => {
                    console.error('Lỗi khi tải KPIs:', err);
                }
            });
        },

        loadTopFeatures() {
            this.loading = true;
            $.ajax({
                url: '/SystemMonitor/GetTopFeatures',
                type: 'POST',
                data: { top: 10, days: this.rankDays },
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                success: (res) => {
                    if (res && !res.Error) {
                        this.topFeatures = res.data || [];
                        this.$nextTick(() => {
                            this.renderTopChart();
                        });
                    }
                },
                complete: () => {
                    this.loading = false;
                }
            });
        },

        loadUnderutilizedFeatures() {
            $.ajax({
                url: '/SystemMonitor/GetUnderutilizedFeatures',
                type: 'POST',
                data: { days: this.rankDays },
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                success: (res) => {
                    if (res && !res.Error) {
                        this.underutilizedFeatures = res.data || [];
                    }
                }
            });
        },

        renderTopChart() {
            const ctx = document.getElementById('topFeaturesBarChart');
            if (ctx) {
                if (this.topChart) this.topChart.destroy();
                const labels = this.topFeatures.map(f => f.FeatureName || f.ControllerName);
                const hits = this.topFeatures.map(f => f.TotalHits);

                this.topChart = new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: labels.length > 0 ? labels : ['Chưa có dữ liệu'],
                        datasets: [{
                            label: 'Lượt tương tác',
                            data: hits.length > 0 ? hits : [0],
                            backgroundColor: 'rgba(78, 115, 223, 0.85)',
                            hoverBackgroundColor: '#2e59d9',
                            borderColor: '#4e73df',
                            borderRadius: 4
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        indexAxis: 'y', // Horizontal Bar Chart
                        plugins: {
                            legend: { display: false },
                            tooltip: { mode: 'index', intersect: false }
                        },
                        scales: {
                            x: { beginAtZero: true, grid: { color: '#f0f2f5' } },
                            y: { grid: { display: false } }
                        }
                    }
                });
            }
        },

        loadUsageDetails() {
            this.loading = true;
            $.ajax({
                url: '/SystemMonitor/GetFeatureUsageStats',
                type: 'POST',
                data: { search: this.detailSearch },
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                success: (res) => {
                    if (res && !res.Error) {
                        this.usageDetails = res.data || [];
                        this.usageTotal = res.totalItems || 0;
                        this.usageTotalPages = Math.ceil(this.usageTotal / this.detailSearch.pageSize) || 1;
                    } else if (res && res.Error) {
                        toastr.error(res.Title || 'Không thể tải chi tiết sử dụng');
                    }
                },
                complete: () => {
                    this.loading = false;
                }
            });
        },

        changeDetailPage(newPage) {
            if (newPage >= 1 && newPage <= this.usageTotalPages && newPage !== this.detailSearch.currentPage) {
                this.detailSearch.currentPage = newPage;
                this.loadUsageDetails();
            }
        },

        resetDetailFilter() {
            this.detailSearch = {
                keyword: '',
                controllerName: 'all',
                actionType: 'all',
                fromDate: '',
                toDate: '',
                currentPage: 1,
                pageSize: 10
            };
            this.loadUsageDetails();
        },

        loadRoleUsage() {
            $.ajax({
                url: '/SystemMonitor/GetRoleUsageDistribution',
                type: 'POST',
                data: { days: this.rankDays },
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                success: (res) => {
                    if (res && !res.Error && res.data) {
                        this.roleDistribution = res.data;
                        this.$nextTick(() => {
                            this.renderRoleChart();
                        });
                    }
                }
            });
        },

        renderRoleChart() {
            const ctx = document.getElementById('roleUsageChart');
            if (ctx) {
                if (this.roleChart) this.roleChart.destroy();
                const labels = Object.keys(this.roleDistribution || {});
                const data = Object.values(this.roleDistribution || {});

                this.roleChart = new Chart(ctx, {
                    type: 'doughnut',
                    data: {
                        labels: labels.length > 0 ? labels : ['Chưa có dữ liệu'],
                        datasets: [{
                            data: data.length > 0 ? data : [1],
                            backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc', '#f6c23e', '#e74a3b', '#6f42c1', '#858796'],
                            borderWidth: 2
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: { legend: { position: 'bottom' } },
                        cutout: '65%'
                    }
                });
            }
        },

        getActionTypeBadge(type) {
            switch (type) {
                case 'View': return 'badge bg-light text-dark border';
                case 'Search': return 'badge bg-info text-white';
                case 'Create': return 'badge bg-success text-white';
                case 'Update': return 'badge bg-warning text-dark';
                case 'Delete': return 'badge bg-danger text-white';
                case 'ExportExcel': return 'badge bg-primary text-white';
                case 'Sync': return 'badge bg-dark text-white';
                default: return 'badge bg-secondary text-white';
            }
        },

        getActionTypeText(type) {
            switch (type) {
                case 'View': return 'Xem trang';
                case 'Search': return 'Tìm kiếm';
                case 'Create': return 'Thêm mới';
                case 'Update': return 'Chỉnh sửa';
                case 'Delete': return 'Xóa dữ liệu';
                case 'ExportExcel': return 'Xuất Excel/Báo cáo';
                case 'Sync': return 'Đồng bộ';
                default: return type || 'Thao tác';
            }
        }
    }));
});
