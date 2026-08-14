document.addEventListener('alpine:init', function () {
    Alpine.data('alpineHome', function () {
        return {
            selectedTinh: '',
            selectedTinhName: 'Toàn bộ Tỉnh/Thành',
            selectedNhom: '',
            selectedNhomName: 'Toàn bộ Nhóm',
            ListCity: [],
            ListNhomTBH: [],
            lastSyncTime: 'Đang tải...',
            isLoading: false,

            // Dashboard Data Stores
            dbTanSuatChemsex3ThangTheoDoTuoi: [],
            dbSuDungDaChatTrongChemsexTheoDoTuoi: [],
            dbTanSuatChemsex3ThangTheoDiemAssist: [],
            dbSuDungDaChatTrongChemsexTheoDoiTuongQHTD: [],
            dbTanSuatChemsex3ThangTheoDiemACE: [],
            dbSuDungDaChatTrongChemsexTheoDiemAssistMaTuyDa: [],
            dbSuDungDaChatTrongChemsexTheoDiemACE: [],
            dbSuDungDaChatTrongChemsexTheoDiemQST: [],
            dbTanSuatChemsexTrong3ThangTheoDiemQST: [],
            dbSuDungDaChatTrongChemsexTheoQHTDTT: [],
            dbSuDungDaChatTrongChemsexTheoBanDam: [],

            // Summary metrics
            metrics: {
                totalSurvey: 0,
                highRiskCount: 0,
                multiSubstanceRate: '0%'
            },

            init: function () {
                window.alpineHomeInstance = this;
                this.loadCities();
                this.loadNhoms();
                this.fetchEndTimeSync();
                this.loadAllDashboardData();
            },

            fetchEndTimeSync: function () {
                var self = this;
                $.ajax({
                    type: 'POST',
                    url: '/SyncData/GetEndTimeSync',
                    data: { apiCode: 'API_ALL_CD43_KHACH_HANG_TTCB' },
                    success: function (response) {
                        if (response && response.success && response.endTimeSync) {
                            self.lastSyncTime = response.endTimeSync;
                        } else {
                            self.lastSyncTime = 'Mới cập nhật';
                        }
                    },
                    error: function () {
                        self.lastSyncTime = 'Chưa xác định';
                    }
                });
            },

            loadCities: function () {
                var self = this;
                $.ajax({
                    type: 'POST',
                    url: '/BaoCaoCD43/GetBottomAction',
                    success: function (response) {
                        if (response && response.Citys) {
                            self.ListCity = response.Citys;
                        }
                    }
                });
            },

            loadNhoms: function (cityCode) {
                var self = this;
                $.ajax({
                    type: 'POST',
                    url: '/BaoCaoCD43/GetNhomTBHByMaNhomMap',
                    data: { CityCodes: cityCode || '' },
                    success: function (response) {
                        if (response && response.NhomTBHs) {
                            self.ListNhomTBH = response.NhomTBHs;
                        }
                    }
                });
            },

            selectCity: function (code, name) {
                this.selectedTinh = code || '';
                this.selectedTinhName = name || 'Toàn bộ Tỉnh/Thành';
                this.selectedNhom = '';
                this.selectedNhomName = 'Toàn bộ Nhóm';
                this.loadNhoms(this.selectedTinh);
                this.loadAllDashboardData();
            },

            selectNhom: function (code, name) {
                this.selectedNhom = code || '';
                this.selectedNhomName = name || 'Toàn bộ Nhóm';
                this.loadAllDashboardData();
            },

            resetFilters: function () {
                this.selectedTinh = '';
                this.selectedTinhName = 'Toàn bộ Tỉnh/Thành';
                this.selectedNhom = '';
                this.selectedNhomName = 'Toàn bộ Nhóm';
                this.loadNhoms('');
                this.loadAllDashboardData();
            },

            loadAllDashboardData: function () {
                var self = this;
                self.isLoading = true;
                if (window.showToast) showToast();

                var params = {
                    maNhom: self.selectedNhom || null,
                    maTinh: self.selectedTinh || null
                };

                var requests = [
                    $.post('/Home/GetTanSuatChemsex3ThangTheoDoTuoi', params),
                    $.post('/Home/GetSuDungDaChatTrongChemsexTheoDoTuoi', params),
                    $.post('/Home/GetTanSuatChemsex3ThangTheoDiemAssist', params),
                    $.post('/Home/GetSuDungDaChatTrongChemsexTheoDoiTuongQHTD', params),
                    $.post('/Home/GetTanSuatChemsex3ThangTheoDiemACE', params),
                    $.post('/Home/GetSuDungDaChatTrongChemsexTheoDiemAssistMaTuyDa', params),
                    $.post('/Home/GetSuDungDaChatTrongChemsexTheoDiemACE', params),
                    $.post('/Home/GetSuDungDaChatTrongChemsexTheoDiemQST', params),
                    $.post('/Home/GetTanSuatChemsexTrong3ThangTheoDiemQST', params),
                    $.post('/Home/GetSuDungDaChatTrongChemsexTheoQHTDTT', params),
                    $.post('/Home/GetSuDungDaChatTrongChemsexTheoBanDam', params)
                ];

                $.when.apply($, requests).done(function (
                    r1, r2, r3, r4, r5, r6, r7, r8, r9, r10, r11
                ) {
                    self.dbTanSuatChemsex3ThangTheoDoTuoi = (r1[0] && r1[0].data) || [];
                    self.dbSuDungDaChatTrongChemsexTheoDoTuoi = (r2[0] && r2[0].data) || [];
                    self.dbTanSuatChemsex3ThangTheoDiemAssist = (r3[0] && r3[0].data) || [];
                    self.dbSuDungDaChatTrongChemsexTheoDoiTuongQHTD = (r4[0] && r4[0].data) || [];
                    self.dbTanSuatChemsex3ThangTheoDiemACE = (r5[0] && r5[0].data) || [];
                    self.dbSuDungDaChatTrongChemsexTheoDiemAssistMaTuyDa = (r6[0] && r6[0].data) || [];
                    self.dbSuDungDaChatTrongChemsexTheoDiemACE = (r7[0] && r7[0].data) || [];
                    self.dbSuDungDaChatTrongChemsexTheoDiemQST = (r8[0] && r8[0].data) || [];
                    self.dbTanSuatChemsexTrong3ThangTheoDiemQST = (r9[0] && r9[0].data) || [];
                    self.dbSuDungDaChatTrongChemsexTheoQHTDTT = (r10[0] && r10[0].data) || [];
                    self.dbSuDungDaChatTrongChemsexTheoBanDam = (r11[0] && r11[0].data) || [];

                    self.calculateMetrics();
                }).always(function () {
                    self.isLoading = false;
                    if (window.hideLoading) hideLoading();
                });
            },

            calculateMetrics: function () {
                var self = this;
                var total = 0;
                var multiSubstance = 0;

                (self.dbSuDungDaChatTrongChemsexTheoDoTuoi || []).forEach(function (item) {
                    var co = parseInt(item.CoSuDungDaChat) || 0;
                    var khong = parseInt(item.KhongSuDungDaChat) || 0;
                    multiSubstance += co;
                    total += (co + khong);
                });

                self.metrics.totalSurvey = total;
                self.metrics.highRiskCount = multiSubstance;
                self.metrics.multiSubstanceRate = total > 0 ? ((multiSubstance / total) * 100).toFixed(1) + '%' : '0%';
            }
        };
    });
});
