document.addEventListener('alpine:init', () => {
    Alpine.data('alpineScheduledReport', () => ({
        // Settings State
        settings: {
            runDay: 5,
            runHour: 8,
            isActive: true,
            telegramChatId: '589101034',
            lastRunTime: null,
            nextRunTime: null,
            totalExportedFiles: 0
        },
        settingsLoading: false,
        settingsSaving: false,

        // Filter & Paging State
        filter: {
            reportType: '',
            periodType: '',
            year: new Date().getFullYear(),
            month: ''
        },
        logs: [],
        totalRows: 0,
        pageIndex: 1,
        pageSize: 15,
        totalPages: 1,
        logsLoading: false,

        // Delete Modal State
        deleteItem: null,
        isDeleting: false,

        // Trigger Run Now Modal State
        manualExport: {
            reportType: 'ALL',
            periodType: 'Month',
            quarter: Math.floor((new Date().getMonth() - 1) / 3) + 1,
            year: new Date().getFullYear(),
            month: new Date().getMonth() === 0 ? 12 : new Date().getMonth()
        },
        isExporting: false,
        exportProgressMsg: '',

        init() {
            this.loadSettings();
            this.loadLogs();
        },

        loadSettings() {
            this.settingsLoading = true;
            fetch('/ScheduledReport/GetSettings')
                .then(res => res.json())
                .then(res => {
                    if (res.Success && res.Settings) {
                        this.settings = {
                            runDay: res.Settings.RunDay,
                            runHour: res.Settings.RunHour,
                            isActive: res.Settings.IsActive,
                            telegramChatId: res.Settings.TelegramChatId || '589101034',
                            lastRunTime: res.Settings.LastRunTime,
                            nextRunTime: res.Settings.NextRunTime,
                            totalExportedFiles: res.Settings.TotalExportedFiles
                        };
                    }
                })
                .catch(err => {
                    console.error('Lỗi tải cấu hình lịch:', err);
                })
                .finally(() => {
                    this.settingsLoading = false;
                });
        },

        saveSettings() {
            if (this.settings.runDay < 1 || this.settings.runDay > 28) {
                toastr.error('Ngày chạy tự động phải từ ngày 1 đến ngày 28.');
                return;
            }
            if (this.settings.runHour < 0 || this.settings.runHour > 23) {
                toastr.error('Giờ chạy tự động phải từ 0 đến 23.');
                return;
            }

            this.settingsSaving = true;
            const formData = new URLSearchParams();
            formData.append('runDay', this.settings.runDay);
            formData.append('runHour', this.settings.runHour);
            formData.append('isActive', this.settings.isActive);

            fetch('/ScheduledReport/SaveSettings', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: formData.toString()
            })
                .then(res => res.json())
                .then(res => {
                    if (res.Success) {
                        toastr.success(res.Message || 'Cập nhật cấu hình lịch chạy thành công!');
                        this.loadSettings();
                    } else {
                        toastr.error(res.Message || 'Không thể lưu cấu hình.');
                    }
                })
                .catch(err => {
                    toastr.error('Lỗi kết nối máy chủ: ' + err);
                })
                .finally(() => {
                    this.settingsSaving = false;
                });
        },

        loadLogs(page = 1) {
            this.pageIndex = page;
            this.logsLoading = true;

            const params = new URLSearchParams({
                reportType: this.filter.reportType || '',
                periodType: this.filter.periodType || '',
                year: this.filter.year || '',
                month: this.filter.month || '',
                pageIndex: this.pageIndex,
                pageSize: this.pageSize
            });

            fetch(`/ScheduledReport/GetLogs?${params.toString()}`)
                .then(res => res.json())
                .then(res => {
                    if (res.Success) {
                        this.logs = res.Data || [];
                        this.totalRows = res.TotalRows || 0;
                        this.totalPages = res.TotalPages || 1;
                    } else {
                        this.logs = [];
                    }
                })
                .catch(err => {
                    console.error('Lỗi tải danh sách báo cáo:', err);
                    this.logs = [];
                })
                .finally(() => {
                    this.logsLoading = false;
                });
        },

        getComputedDateRangeText() {
            var y = parseInt(this.manualExport.year) || new Date().getFullYear();
            if (this.manualExport.periodType === 'Quarter') {
                var q = parseInt(this.manualExport.quarter) || 1;
                if (q === 1) return '26/12/' + (y - 1) + ' đến 25/03/' + y + ' (Quý I/' + y + ')';
                if (q === 2) return '26/03/' + y + ' đến 25/06/' + y + ' (Quý II/' + y + ')';
                if (q === 3) return '26/06/' + y + ' đến 25/09/' + y + ' (Quý III/' + y + ')';
                return '26/09/' + y + ' đến 25/12/' + y + ' (Quý IV/' + y + ')';
            } else if (this.manualExport.periodType === 'Year') {
                return '26/12/' + (y - 1) + ' đến 25/12/' + y + ' (Cả Năm ' + y + ')';
            } else {
                var m = parseInt(this.manualExport.month) || 1;
                var prevM = m === 1 ? 12 : m - 1;
                var prevY = m === 1 ? y - 1 : y;
                var strPrevM = prevM < 10 ? '0' + prevM : prevM;
                var strM = m < 10 ? '0' + m : m;
                return '26/' + strPrevM + '/' + prevY + ' đến 25/' + strM + '/' + y + ' (Tháng ' + m + '/' + y + ')';
            }
        },

        triggerExportNow() {
            if (!this.manualExport.year) {
                toastr.error('Vui lòng chọn năm cần xuất báo cáo.');
                return;
            }

            var periodDesc = this.getComputedDateRangeText();
            const confirmMsg = `Bạn có chắc chắn muốn xuất ${this.getReportTypeName(this.manualExport.reportType)} cho kỳ:\n${periodDesc}\nngay bây giờ không?`;
            if (!confirm(confirmMsg)) return;

            this.isExporting = true;
            this.exportProgressMsg = 'Đang tiến hành kiểm tra số liệu và tạo file báo cáo (vui lòng chờ trong giây lát)...';

            const formData = new URLSearchParams();
            formData.append('year', this.manualExport.year);
            formData.append('month', this.manualExport.month);
            formData.append('periodType', this.manualExport.periodType);
            formData.append('quarter', this.manualExport.quarter);
            formData.append('reportType', this.manualExport.reportType);

            fetch('/ScheduledReport/TriggerExportNow', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: formData.toString()
            })
                .then(res => res.json())
                .then(res => {
                    if (res.Success) {
                        toastr.success(res.Message || 'Xuất báo cáo thành công!');
                        this.loadLogs(1);
                        this.loadSettings();
                    } else {
                        toastr.warning(res.Message || 'Không thể xuất báo cáo.');
                    }
                })
                .catch(err => {
                    toastr.error('Lỗi xử lý: ' + err);
                })
                .finally(() => {
                    this.isExporting = false;
                    this.exportProgressMsg = '';
                });
        },

        downloadFile(id) {
            window.location.href = `/ScheduledReport/DownloadFile?id=${id}`;
        },

        formatDateTime(dtStr) {
            if (!dtStr) return '—';
            try {
                // If MS JSON Date /Date(12345)/
                if (dtStr.includes('/Date(')) {
                    const timestamp = parseInt(dtStr.replace(/\/Date\((\d+)\)\//, '$1'));
                    const d = new Date(timestamp);
                    return d.toLocaleString('vi-VN');
                }
                const d = new Date(dtStr);
                return isNaN(d.getTime()) ? dtStr : d.toLocaleString('vi-VN');
            } catch {
                return dtStr;
            }
        },

        formatFileSize(kb) {
            if (!kb || kb <= 0) return '0 KB';
            if (kb > 1024) {
                return (kb / 1024).toFixed(2) + ' MB';
            }
            return kb + ' KB';
        },

        getReportTypeName(code) {
            switch (code) {
                case 'TCV_CD45': return 'Báo cáo TCV CD45 (.ZIP)';
                case 'HOATDONG_CD45': return 'Báo cáo Hoạt động CD45 (.xlsx)';
                case 'ALL': return 'Cả 2 loại báo cáo (TCV + Hoạt động)';
                default: return code;
            }
        },

        getReportBadgeClass(type) {
            switch (type) {
                case 'TCV_CD45': return 'badge bg-danger text-white';
                case 'HOATDONG_CD45': return 'badge bg-primary text-white';
                default: return 'badge bg-secondary text-white';
            }
        },

        confirmDelete(item) {
            this.deleteItem = item;
            const modalEl = document.getElementById('modalConfirmDelete');
            if (modalEl) {
                const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
                modal.show();
            }
        },

        executeDelete() {
            if (!this.deleteItem || !this.deleteItem.Id) return;

            this.isDeleting = true;
            const formData = new URLSearchParams();
            formData.append('id', this.deleteItem.Id);

            fetch('/ScheduledReport/DeleteLog', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: formData.toString()
            })
                .then(res => res.json())
                .then(res => {
                    if (res.Success) {
                        toastr.success(res.Message || 'Đã xóa báo cáo thành công!');
                        const modalEl = document.getElementById('modalConfirmDelete');
                        if (modalEl) {
                            const modal = bootstrap.Modal.getInstance(modalEl);
                            if (modal) modal.hide();
                        }
                        this.deleteItem = null;
                        this.loadLogs(this.pageIndex);
                        this.loadSettings();
                    } else {
                        toastr.error(res.Message || 'Không thể xóa báo cáo.');
                    }
                })
                .catch(err => {
                    toastr.error('Lỗi kết nối máy chủ: ' + err);
                })
                .finally(() => {
                    this.isDeleting = false;
                });
        }
    }));
});
