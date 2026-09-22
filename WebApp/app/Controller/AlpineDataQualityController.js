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
            currentDrillUnit: null,
            currentDrillMetric: '',
            unitDrillModalTitle: '',
            unitDrillItems: [],
            filteredUnitDrillItems: [],
            unitDrillSearchText: '',
            isUnitDrillLoading: false,
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

            openUnitDrillDown: function (unit, metricType, metricLabel) {
                var self = this;
                self.currentDrillUnit = unit;
                self.currentDrillMetric = metricType;
                var count = 0;
                if (metricType === 'PENDING') count = unit.TotalPending;
                else if (metricType === 'WARNING') count = unit.TotalWarnings;
                else if (metricType === 'ERROR') count = unit.TotalErrors;
                var cityDisplay = unit.TEN_TINH ? (unit.TEN_TINH + (unit.CITY_CODE && unit.CITY_CODE !== unit.TEN_TINH ? ' (' + unit.CITY_CODE + ')' : '')) : (unit.CITY_CODE || '-');
                var nhomDisplay = unit.TEN_NHOM ? (unit.TEN_NHOM + (unit.MA_NHOM && unit.MA_NHOM !== 'UNKNOWN' && unit.MA_NHOM !== unit.TEN_NHOM ? ' (' + unit.MA_NHOM + ')' : '')) : (unit.MA_NHOM || 'CHƯA PHÂN NHÓM');
                self.unitDrillModalTitle = 'Chi tiết Cảnh báo: Tỉnh [' + cityDisplay + '] - Nhóm [' + nhomDisplay + '] | ' + metricLabel + ' (' + (count || 0) + ' bản ghi)';
                self.unitDrillItems = [];
                self.filteredUnitDrillItems = [];
                self.unitDrillSearchText = '';
                self.isUnitDrillLoading = true;

                // Show modal safely for Bootstrap 5, Bootstrap 4/jQuery, or CSS fallback
                var modalEl = document.getElementById('modalUnitDrillDown');
                if (modalEl) {
                    if (window.bootstrap && bootstrap.Modal) {
                        var modal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
                        modal.show();
                    } else if (window.jQuery && typeof $(modalEl).modal === 'function') {
                        $(modalEl).modal('show');
                    } else {
                        modalEl.classList.add('show');
                        modalEl.style.display = 'block';
                        modalEl.removeAttribute('aria-hidden');
                        modalEl.setAttribute('aria-modal', 'true');
                    }
                }

                $.ajax({
                    type: 'POST',
                    url: '/DataQuality/GetLogsByUnit',
                    data: {
                        maDuAn: self.filterMaDuAn,
                        cityCode: unit.CITY_CODE,
                        maNhom: unit.MA_NHOM,
                        metricType: metricType
                    },
                    success: function (res) {
                        self.isUnitDrillLoading = false;
                        if (res.Success) {
                            self.unitDrillItems = res.Data || [];
                            self.filterUnitDrill();
                        } else {
                            if (window.toastr) toastr.error(res.Message);
                        }
                    },
                    error: function (xhr, status, error) {
                        self.isUnitDrillLoading = false;
                        console.error('[Unit DrillDown] AJAX Error:', status, error);
                        if (window.toastr) toastr.error('Lỗi khi tải chi tiết cảnh báo theo đơn vị!');
                    }
                });
            },

            closeUnitDrillDown: function () {
                var modalEl = document.getElementById('modalUnitDrillDown');
                if (modalEl) {
                    if (window.bootstrap && bootstrap.Modal) {
                        var modal = bootstrap.Modal.getInstance(modalEl);
                        if (modal) modal.hide();
                    }
                    if (window.jQuery && typeof $(modalEl).modal === 'function') {
                        $(modalEl).modal('hide');
                    }
                    modalEl.classList.remove('show');
                    modalEl.style.display = 'none';
                    modalEl.setAttribute('aria-hidden', 'true');
                    modalEl.removeAttribute('aria-modal');
                    var backdrops = document.querySelectorAll('.modal-backdrop');
                    backdrops.forEach(function (b) { b.remove(); });
                }
            },

            filterUnitDrill: function () {
                var self = this;
                var kw = (self.unitDrillSearchText || '').trim().toLowerCase();
                if (!kw) {
                    self.filteredUnitDrillItems = self.unitDrillItems;
                    return;
                }
                self.filteredUnitDrillItems = self.unitDrillItems.filter(function (x) {
                    return (x.RECORD_ID && x.RECORD_ID.toLowerCase().indexOf(kw) >= 0)
                        || (x.RULE_CODE && x.RULE_CODE.toLowerCase().indexOf(kw) >= 0)
                        || (x.MESSAGE && x.MESSAGE.toLowerCase().indexOf(kw) >= 0)
                        || (x.FIELD_NAME && x.FIELD_NAME.toLowerCase().indexOf(kw) >= 0)
                        || (x.API_CODE && x.API_CODE.toLowerCase().indexOf(kw) >= 0);
                });
            },

            markResolvedInDrill: function (logItem) {
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
                            self.loadStatsByNhom();
                            if (self.activeTab === 'grouped') self.loadGroupedLogs();
                        } else {
                            if (window.toastr) toastr.error("Không thể cập nhật trạng thái: " + res.Message);
                        }
                    }
                });
            },

            jumpToDetailsTab: function () {
                var self = this;
                self.closeUnitDrillDown();
                self.activeTab = 'details';
                if (self.currentDrillUnit && self.currentDrillUnit.CITY_CODE && self.currentDrillUnit.CITY_CODE !== '-') {
                    self.filterKeyword = self.currentDrillUnit.CITY_CODE;
                }
                if (self.currentDrillMetric === 'ERROR') {
                    self.filterSeverity = 'ERROR';
                    self.filterIsResolved = 'false';
                } else if (self.currentDrillMetric === 'WARNING') {
                    self.filterSeverity = 'WARNING';
                    self.filterIsResolved = 'false';
                } else if (self.currentDrillMetric === 'RESOLVED') {
                    self.filterSeverity = '';
                    self.filterIsResolved = 'true';
                } else if (self.currentDrillMetric === 'PENDING') {
                    self.filterSeverity = '';
                    self.filterIsResolved = 'false';
                }
                self.loadLogs(1);
            },

            getExportButtonLabel: function () {
                var self = this;
                if (self.activeTab === 'grouped') {
                    return 'Xuất Excel Gom nhóm Quy tắc';
                } else if (self.activeTab === 'byUnit') {
                    return 'Xuất Excel Thống kê Đơn vị';
                }
                return 'Xuất Excel Chi tiết Sự kiện';
            },

            getExportMenuLabel: function () {
                var self = this;
                if (self.activeTab === 'grouped') {
                    return 'Xuất Bảng Gom nhóm Quy tắc (Tab 2)';
                } else if (self.activeTab === 'byUnit') {
                    return 'Xuất Bảng Thống kê Đơn vị (Tab 3)';
                }
                return 'Xuất Danh sách Chi tiết theo Bộ lọc (Tab 1)';
            },

            exportCurrentTab: function () {
                var self = this;
                if (self.activeTab === 'grouped') {
                    window.location.href = '/DataQuality/ExportExcel?tabType=grouped&maDuAn=' + encodeURIComponent(self.filterMaDuAn) +
                        '&severity=' + encodeURIComponent(self.filterSeverity || '');
                } else if (self.activeTab === 'byUnit') {
                    window.location.href = '/DataQuality/ExportExcel?tabType=byUnit&maDuAn=' + encodeURIComponent(self.filterMaDuAn);
                } else {
                    window.location.href = '/DataQuality/ExportExcel?tabType=details&maDuAn=' + encodeURIComponent(self.filterMaDuAn) +
                        '&apiCode=' + encodeURIComponent(self.filterApiCode || '') +
                        '&severity=' + encodeURIComponent(self.filterSeverity || '') +
                        '&isResolved=' + encodeURIComponent(self.filterIsResolved || '') +
                        '&keyword=' + encodeURIComponent(self.filterKeyword || '');
                }
            },

            exportMultiSheet: function () {
                var self = this;
                window.location.href = '/DataQuality/ExportExcel?tabType=multi&maDuAn=' + encodeURIComponent(self.filterMaDuAn) +
                    '&apiCode=' + encodeURIComponent(self.filterApiCode || '') +
                    '&severity=' + encodeURIComponent(self.filterSeverity || '') +
                    '&isResolved=' + encodeURIComponent(self.filterIsResolved || '') +
                    '&keyword=' + encodeURIComponent(self.filterKeyword || '');
            },

            exportWarningsOnly: function () {
                var self = this;
                window.location.href = '/DataQuality/ExportExcel?tabType=warnings&maDuAn=' + encodeURIComponent(self.filterMaDuAn) +
                    '&apiCode=' + encodeURIComponent(self.filterApiCode || '');
            },

            exportWarnings: function () {
                this.exportCurrentTab();
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
