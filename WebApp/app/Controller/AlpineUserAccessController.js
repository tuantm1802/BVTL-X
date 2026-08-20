/**
 * AlpineUserAccessController.js
 * Quản lý giao diện Giám sát Truy cập, Phiên Online & Lịch sử Đăng nhập
 */
document.addEventListener('alpine:init', () => {
    Alpine.data('userAccessApp', () => ({
        activeTab: 'online', // 'online' | 'history' | 'charts'
        loading: false,
        autoRefresh: true,
        refreshTimer: null,

        // KPIs
        kpis: {
            onlineUsersCount: 0,
            idleUsersCount: 0,
            dailyActiveUsers: 0,
            todayPageViews: 0,
            todayFailedLogins: 0
        },

        // Tab 1: Online Users
        onlineSearch: {
            keyword: '',
            page: 1,
            pageSize: 10
        },
        onlineUsers: [],
        onlineTotal: 0,
        onlineTotalPages: 1,

        // Tab 2: Login History
        historySearch: {
            keyword: '',
            status: 'all',
            fromDate: '',
            toDate: '',
            currentPage: 1,
            pageSize: 10
        },
        historyList: [],
        historyTotal: 0,
        historyTotalPages: 1,

        // Tab 3: Charts
        trafficDays: 7,
        trafficStats: {
            timeLabels: [],
            pageViewsData: [],
            uniqueUsersData: [],
            deviceDistribution: {},
            browserDistribution: {}
        },
        trafficChart: null,
        deviceChart: null,
        // User Profile Modal state
        selectedUser: null,
        userProfile: null,
        profileLoading: false,

        init() {
            this.loadDashboardKPIs();
            this.loadOnlineUsers();
            this.loadLoginHistory();
            this.loadTrafficStats();

            // Thiết lập Auto-refresh mỗi 20s cho danh sách Online & KPIs
            this.refreshTimer = setInterval(() => {
                if (this.autoRefresh && document.visibilityState !== 'hidden') {
                    this.loadDashboardKPIs(true);
                    if (this.activeTab === 'online') {
                        this.loadOnlineUsers(true);
                    }
                }
            }, 20000);
        },

        // 1. Tải KPIs
        loadDashboardKPIs(silent = false) {
            if (!silent) this.loading = true;
            $.ajax({
                url: '/SystemMonitor/GetDashboardKPIs',
                type: 'POST',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                success: (res) => {
                    if (res && res.kpi) {
                        this.kpis = {
                            onlineUsersCount: res.kpi.OnlineUsersCount !== undefined ? res.kpi.OnlineUsersCount : (res.kpi.onlineUsersCount || 0),
                            idleUsersCount: res.kpi.IdleUsersCount !== undefined ? res.kpi.IdleUsersCount : (res.kpi.idleUsersCount || 0),
                            dailyActiveUsers: res.kpi.DailyActiveUsers !== undefined ? res.kpi.DailyActiveUsers : (res.kpi.dailyActiveUsers || 0),
                            todayPageViews: res.kpi.TodayPageViews !== undefined ? res.kpi.TodayPageViews : (res.kpi.todayPageViews || 0),
                            todayFailedLogins: res.kpi.TodayFailedLogins !== undefined ? res.kpi.TodayFailedLogins : (res.kpi.todayFailedLogins || 0)
                        };
                    }
                },
                error: (err) => {
                    console.error('Lỗi khi tải KPIs:', err);
                },
                complete: () => {
                    if (!silent) this.loading = false;
                }
            });
        },

        // 2. Tải Danh sách Online Users
        loadOnlineUsers(silent = false) {
            if (!silent) this.loading = true;
            $.ajax({
                url: '/SystemMonitor/GetOnlineUsers',
                type: 'POST',
                data: {
                    keyword: this.onlineSearch.keyword,
                    page: this.onlineSearch.page,
                    pageSize: this.onlineSearch.pageSize
                },
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                success: (res) => {
                    if (res && !res.Error) {
                        this.onlineUsers = res.data || [];
                        this.onlineTotal = res.totalItems || 0;
                        this.onlineTotalPages = Math.ceil(this.onlineTotal / this.onlineSearch.pageSize) || 1;
                    } else if (res && res.Error) {
                        toastr.error(res.Title || 'Không thể tải danh sách người dùng online');
                    }
                },
                error: (err) => {
                    toastr.error('Lỗi kết nối máy chủ');
                },
                complete: () => {
                    if (!silent) this.loading = false;
                }
            });
        },

        changeOnlinePage(newPage) {
            if (newPage >= 1 && newPage <= this.onlineTotalPages && newPage !== this.onlineSearch.page) {
                this.onlineSearch.page = newPage;
                this.loadOnlineUsers();
            }
        },

        // Xem thông tin chi tiết người dùng (Profile)
        viewUserProfile(user) {
            this.selectedUser = user;
            this.profileLoading = true;
            this.userProfile = null;

            // Hiển thị modal trước
            const modalEl = document.getElementById('userProfileModal');
            if (modalEl) {
                const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
                modal.show();
            }

            $.ajax({
                url: '/SystemMonitor/GetUserProfile',
                type: 'POST',
                data: { userId: user.UserId },
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                success: (res) => {
                    if (res && res.success && res.data) {
                        this.userProfile = res.data;
                    } else {
                        toastr.warning(res.message || 'Không thể lấy thông tin chi tiết người dùng');
                    }
                },
                error: (err) => {
                    toastr.error('Lỗi khi tải thông tin hồ sơ người dùng');
                },
                complete: () => {
                    this.profileLoading = false;
                }
            });
        },

        // Đăng xuất (Ngắt phiên làm việc từ xa)
        confirmForceLogout(user) {
            const userNameDisplay = user.FullName ? `${user.FullName} (${user.UserName})` : user.UserName;
            if (!confirm(`Bạn có chắc chắn muốn ngắt phiên làm việc (Đăng xuất) của người dùng "${userNameDisplay}" không?`)) {
                return;
            }

            this.loading = true;
            $.ajax({
                url: '/SystemMonitor/ForceLogout',
                type: 'POST',
                data: { sessionId: user.SessionId },
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                success: (res) => {
                    if (res && res.success) {
                        toastr.success(res.message || `Đã ngắt phiên đăng nhập của ${userNameDisplay}`);
                        this.loadOnlineUsers();
                        this.loadDashboardKPIs(true);
                    } else {
                        toastr.error(res.message || 'Không thể ngắt phiên đăng nhập');
                    }
                },
                error: (err) => {
                    toastr.error('Lỗi kết nối máy chủ khi thực hiện đăng xuất');
                },
                complete: () => {
                    this.loading = false;
                }
            });
        },

        // 3. Tải Lịch sử Đăng nhập
        loadLoginHistory() {
            this.loading = true;
            $.ajax({
                url: '/SystemMonitor/GetLoginHistory',
                type: 'POST',
                data: { search: this.historySearch },
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                success: (res) => {
                    if (res && !res.Error) {
                        this.historyList = res.data || [];
                        this.historyTotal = res.totalItems || 0;
                        this.historyTotalPages = Math.ceil(this.historyTotal / this.historySearch.pageSize) || 1;
                    } else if (res && res.Error) {
                        toastr.error(res.Title || 'Không thể tải lịch sử đăng nhập');
                    }
                },
                error: (err) => {
                    toastr.error('Lỗi kết nối máy chủ');
                },
                complete: () => {
                    this.loading = false;
                }
            });
        },

        changeHistoryPage(newPage) {
            if (newPage >= 1 && newPage <= this.historyTotalPages && newPage !== this.historySearch.currentPage) {
                this.historySearch.currentPage = newPage;
                this.loadLoginHistory();
            }
        },

        resetHistoryFilter() {
            this.historySearch = {
                keyword: '',
                status: 'all',
                fromDate: '',
                toDate: '',
                currentPage: 1,
                pageSize: 10
            };
            this.loadLoginHistory();
        },

        // 4. Tải Thống kê Lưu lượng & Vẽ biểu đồ
        loadTrafficStats() {
            $.ajax({
                url: '/SystemMonitor/GetTrafficStats',
                type: 'POST',
                data: { days: this.trafficDays },
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                success: (res) => {
                    if (res && !res.Error && res.stats) {
                        this.trafficStats = res.stats;
                        this.$nextTick(() => {
                            this.renderTrafficCharts();
                        });
                    }
                },
                error: (err) => {
                    console.error('Lỗi khi tải biểu đồ lưu lượng:', err);
                }
            });
        },

        renderTrafficCharts() {
            // A. Biểu đồ Đường Lưu lượng
            const ctxTraffic = document.getElementById('trafficLineChart');
            if (ctxTraffic) {
                if (this.trafficChart) this.trafficChart.destroy();
                this.trafficChart = new Chart(ctxTraffic, {
                    type: 'line',
                    data: {
                        labels: this.trafficStats.timeLabels || [],
                        datasets: [
                            {
                                label: 'Lượt truy cập (Page Views)',
                                data: this.trafficStats.pageViewsData || [],
                                borderColor: '#4e73df',
                                backgroundColor: 'rgba(78, 115, 223, 0.05)',
                                pointRadius: 4,
                                pointBackgroundColor: '#4e73df',
                                pointBorderColor: '#fff',
                                pointHoverRadius: 6,
                                fill: true,
                                tension: 0.3
                            },
                            {
                                label: 'Người dùng hoạt động (Users)',
                                data: this.trafficStats.uniqueUsersData || [],
                                borderColor: '#1cc88a',
                                backgroundColor: 'rgba(28, 200, 138, 0.05)',
                                pointRadius: 4,
                                pointBackgroundColor: '#1cc88a',
                                pointBorderColor: '#fff',
                                pointHoverRadius: 6,
                                fill: true,
                                tension: 0.3
                            }
                        ]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            legend: { position: 'top' },
                            tooltip: { mode: 'index', intersect: false }
                        },
                        scales: {
                            y: { beginAtZero: true, grid: { color: '#f0f2f5' } },
                            x: { grid: { display: false } }
                        }
                    }
                });
            }

            // B. Biểu đồ Tròn Thiết bị
            const ctxDevice = document.getElementById('deviceDoughnutChart');
            if (ctxDevice) {
                if (this.deviceChart) this.deviceChart.destroy();
                const devLabels = Object.keys(this.trafficStats.deviceDistribution || {});
                const devData = Object.values(this.trafficStats.deviceDistribution || {});
                this.deviceChart = new Chart(ctxDevice, {
                    type: 'doughnut',
                    data: {
                        labels: devLabels.length > 0 ? devLabels : ['Chưa có dữ liệu'],
                        datasets: [{
                            data: devData.length > 0 ? devData : [1],
                            backgroundColor: ['#4e73df', '#36b9cc', '#f6c23e', '#e74a3b'],
                            borderWidth: 2
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: { legend: { position: 'bottom' } },
                        cutout: '70%'
                    }
                });
            }

            // C. Biểu đồ Tròn Trình duyệt
            const ctxBrowser = document.getElementById('browserDoughnutChart');
            if (ctxBrowser) {
                if (this.browserChart) this.browserChart.destroy();
                const brLabels = Object.keys(this.trafficStats.browserDistribution || {});
                const brData = Object.values(this.trafficStats.browserDistribution || {});
                this.browserChart = new Chart(ctxBrowser, {
                    type: 'doughnut',
                    data: {
                        labels: brLabels.length > 0 ? brLabels : ['Chưa có dữ liệu'],
                        datasets: [{
                            data: brData.length > 0 ? brData : [1],
                            backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc', '#f6c23e', '#e74a3b', '#858796'],
                            borderWidth: 2
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: { legend: { position: 'bottom' } },
                        cutout: '70%'
                    }
                });
            }
        },

        // Helper Status Badge
        getStatusBadgeClass(status) {
            switch (status) {
                case 'Online': return 'badge bg-success text-white';
                case 'Idle': return 'badge bg-warning text-dark';
                case 'Success': return 'badge bg-success text-white';
                case 'WrongPassword': return 'badge bg-danger text-white';
                case 'AccountLocked': return 'badge bg-dark text-white';
                case 'UserNotFound': return 'badge bg-secondary text-white';
                default: return 'badge bg-secondary text-white';
            }
        },

        getStatusText(status) {
            switch (status) {
                case 'Online': return '🟢 Trực tuyến';
                case 'Idle': return '🟡 Tạm rời';
                case 'Success': return 'Thành công';
                case 'WrongPassword': return 'Sai mật khẩu';
                case 'AccountLocked': return 'Tài khoản bị khóa';
                case 'UserNotFound': return 'Không tồn tại';
                default: return status || 'Không xác định';
            }
        }
    }));
});
