document.addEventListener('alpine:init', function () {
    Alpine.data('alpineDataQuality', function () {
        return {
            activeTab: 'details', // 'details' | 'grouped' | 'byUnit'
            logs: [],
            groupedLogs: [],
            statsByNhom: [],
            stats: {
                TotalLogs: 0,
                TotalAutoCleaned: 0,
                TotalWarnings: 0,
                TotalErrors: 0,
                TotalResolved: 0,
                TotalPendingAction: 0
            },
            isLoading: false,
            isLoadingGrouped: false,
            isLoadingByNhom: false,
            isScanningDuplicates: false,
            filterMaDuAn: 'CD45',
            filterApiCode: '',
            filterSeverity: 'WARNING',
            filterIsResolved: '',
            filterKeyword: '',
            pageIndex: 1,
            pageSize: 20,
            totalRows: 0,
            totalPages: 1,

            init: function () {
                var self = this;
                self.loadStats();
                self.loadLogs();
            },

            switchTab: function (tab) {
                var self = this;
                self.activeTab = tab;
                if (tab === 'grouped' && self.groupedLogs.length === 0) {
                    self.loadGroupedLogs();
                } else if (tab === 'byUnit' && self.statsByNhom.length === 0) {
                    self.loadStatsByNhom();
                }
            },

            loadStats: function () {
                var self = this;
                $.ajax({
                    type: 'POST',
                    url: '/DataQuality/GetStats',
                    data: { maDuAn: self.filterMaDuAn },
                    success: function (res) {
                        if (res.Success && res.Stats) {
                            self.stats = res.Stats;
                        }
                    }
                });
            },

            loadLogs: function (page) {
                var self = this;
                if (page) self.pageIndex = page;
                self.isLoading = true;

                var isRes = null;
                if (self.filterIsResolved === 'true') isRes = true;
                if (self.filterIsResolved === 'false') isRes = false;

                $.ajax({
                    type: 'POST',
                    url: '/DataQuality/GetLogs',
                    data: {
                        maDuAn: self.filterMaDuAn,
                        apiCode: self.filterApiCode,
                        severity: self.filterSeverity,
                        keyword: self.filterKeyword,
                        isResolved: isRes,
                        pageIndex: self.pageIndex,
                        pageSize: self.pageSize
                    },
                    success: function (res) {
                        self.isLoading = false;
                        if (res.Success) {
                            self.logs = res.Data || [];
                            self.totalRows = res.TotalRows || 0;
                            self.totalPages = Math.ceil(self.totalRows / self.pageSize) || 1;
                        } else {
                            if (window.toastr) toastr.error(res.Message);
                        }
                    },
                    error: function () {
                        self.isLoading = false;
                        if (window.toastr) toastr.error('Có lỗi xảy ra khi tải dữ liệu nhật ký.');
                    }
                });
            },

            loadGroupedLogs: function () {
                var self = this;
                self.isLoadingGrouped = true;
                $.ajax({
                    type: 'POST',
                    url: '/DataQuality/GetGroupedLogs',
                    data: {
                        maDuAn: self.filterMaDuAn,
                        severity: self.filterSeverity
                    },
                    success: function (res) {
                        self.isLoadingGrouped = false;
                        if (res.Success) {
                            self.groupedLogs = res.Data || [];
                        } else {
                            if (window.toastr) toastr.error(res.Message);
                        }
                    },
                    error: function () {
                        self.isLoadingGrouped = false;
                        if (window.toastr) toastr.error('Có lỗi xảy ra khi tải bảng gom nhóm.');
                    }
                });
            },

            loadStatsByNhom: function () {
                var self = this;
                self.isLoadingByNhom = true;
                $.ajax({
                    type: 'POST',
                    url: '/DataQuality/GetStatsByNhom',
                    data: {
                        maDuAn: self.filterMaDuAn
                    },
                    success: function (res) {
                        self.isLoadingByNhom = false;
                        if (res.Success) {
                            self.statsByNhom = res.Data || [];
                        } else {
                            if (window.toastr) toastr.error(res.Message);
                        }
                    },
                    error: function () {
                        self.isLoadingByNhom = false;
                        if (window.toastr) toastr.error('Có lỗi xảy ra khi tải thống kê theo đơn vị.');
                    }
                });
            },

            scanDuplicates: function () {
                var self = this;
                if (!confirm("Hệ thống sẽ thực hiện quét trùng lặp hồ sơ đa trường (Họ tên + Ngày sinh/Năm sinh + Tỉnh) trên bảng Khách hàng F1.\nBạn có muốn tiếp tục?")) {
                    return;
                }

                self.isScanningDuplicates = true;
                $.ajax({
                    type: 'POST',
                    url: '/DataQuality/ScanDuplicates',
                    data: { maDuAn: self.filterMaDuAn },
                    success: function (res) {
                        self.isScanningDuplicates = false;
                        if (res.Success) {
                            if (window.toastr) {
                                toastr.success(res.Message);
                            } else {
                                alert(res.Message);
                            }
                            self.loadStats();
                            self.loadLogs(1);
                            if (self.activeTab === 'grouped') self.loadGroupedLogs();
                            if (self.activeTab === 'byUnit') self.loadStatsByNhom();
                        } else {
                            if (window.toastr) toastr.error("Lỗi quét trùng: " + res.Message);
                        }
                    },
                    error: function () {
                        self.isScanningDuplicates = false;
                        if (window.toastr) toastr.error("Có lỗi xảy ra trong quá trình quét trùng lặp.");
                    }
                });
            },

            markResolved: function (logItem) {
                var self = this;
                var note = prompt("Nhập ghi chú xử lý (ví dụ: Đã báo TCV sửa lại trên REDCap):", "Đã rà soát và điều chỉnh trên REDCap");
                if (note === null) return;

                $.ajax({
                    type: 'POST',
                    url: '/DataQuality/ResolveLog',
                    data: { id: logItem.ID, note: note },
                    success: function (res) {
                        if (res.Success) {
                            if (window.toastr) toastr.success("Đã đánh dấu xử lý thành công!");
                            logItem.IS_RESOLVED = true;
                            logItem.RESOLVED_NOTE = note;
                            self.loadStats();
                            if (self.activeTab === 'grouped') self.loadGroupedLogs();
                            if (self.activeTab === 'byUnit') self.loadStatsByNhom();
                        } else {
                            if (window.toastr) toastr.error("Không thể cập nhật trạng thái: " + res.Message);
                        }
                    }
                });
            },

            exportWarnings: function () {
                var self = this;
                var url = '/DataQuality/ExportExcelWarnings?maDuAn=' + encodeURIComponent(self.filterMaDuAn) +
                    '&apiCode=' + encodeURIComponent(self.filterApiCode);
                window.location.href = url;
            },

            formatDate: function (dtStr) {
                if (!dtStr) return '';
                // Handle JSON Date /Date(123456789)/ or ISO
                if (dtStr.indexOf('/Date(') !== -1) {
                    var timestamp = parseInt(dtStr.replace(/\/Date\((\d+)\)\//, '$1'));
                    var d = new Date(timestamp);
                    return d.toLocaleDateString('vi-VN') + ' ' + d.toLocaleTimeString('vi-VN');
                }
                var d = new Date(dtStr);
                return isNaN(d.getTime()) ? dtStr : (d.toLocaleDateString('vi-VN') + ' ' + d.toLocaleTimeString('vi-VN'));
            }
        };
    });
});
