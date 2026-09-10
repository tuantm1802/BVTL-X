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
            year: new Date().getFullYear(),
            month: ''
        },
        logs: [],
        totalRows: 0,
        pageIndex: 1,
        pageSize: 15,
        totalPages: 1,
        logsLoading: false,

        // Trigger Run Now Modal State
        manualExport: {
            reportType: 'ALL',
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

        triggerExportNow() {
            if (!this.manualExport.year || !this.manualExport.month) {
                toastr.error('Vui lòng chọn năm và tháng cần xuất báo cáo.');
                return;
            }

            const confirmMsg = `Bạn có chắc chắn muốn xuất ${this.getReportTypeName(this.manualExport.reportType)} cho Tháng ${this.manualExport.month}/${this.manualExport.year} ngay bây giờ không?`;
            if (!confirm(confirmMsg)) return;

            this.isExporting = true;
            this.exportProgressMsg = 'Đang tiến hành kiểm tra số liệu và tạo file báo cáo (vui lòng chờ trong giây lát)...';

            const formData = new URLSearchParams();
            formData.append('year', this.manualExport.year);
            formData.append('month', this.manualExport.month);
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
                case 'TONGHOP_BVTL': return 'Báo cáo Tổng hợp BVTL (.xlsx)';
                case 'ALL': return 'Tất cả 3 loại báo cáo';
                default: return code;
            }
        },

        getReportBadgeClass(type) {
            switch (type) {
                case 'TCV_CD45': return 'badge bg-danger text-white';
                case 'HOATDONG_CD45': return 'badge bg-primary text-white';
                case 'TONGHOP_BVTL': return 'badge bg-success text-white';
                default: return 'badge bg-secondary text-white';
            }
        }
    }));
});
