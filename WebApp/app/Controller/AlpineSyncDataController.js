document.addEventListener('alpine:init', function () {
    Alpine.data('alpineSyncData', function () {
        return {
            items: [],
            totalItems: 0,
            keyword: '',
            statusFilter: '1', // Mặc định: '1' (Đang kích hoạt), '0' (Đã hủy), 'all' (Tất cả)
            isLoading: false,
            currentPage: 1,
            pageSize: 10,
            totalPages: 1,
            pages: [],

            // Scheduler status
            scheduler: {
                isRunning: false,
                isStandby: false,
                isAutoSyncEnabled: true,
                lastStartedTime: '—',
                jobCount: 0,
                jobs: [],
                isAdmin: false,
                isLoadingStatus: false
            },

            // Modal Cấu hình lịch đồng bộ
            editScheduleModal: {
                isOpen: false,
                apiId: 0,
                apiCode: '',
                apiName: '',
                reportId: '',
                timeLoopType: 'interval', // 'interval' | 'daily'
                intervalValue: 60,
                dailyHour: 0,
                isActive: true,
                isSaving: false
            },

            init: function () {
                this.loadData();
                this.loadSchedulerStatus();
            },

            loadData: function () {
                var self = this;
                self.isLoading = true;

                $.ajax({
                    type: 'POST',
                    url: '/SyncData/GetAllByPage',
                    data: {
                        KeyWord: self.keyword || '',
                        status: self.statusFilter,
                        currentPage: self.currentPage,
                        pageSize: self.pageSize
                    },
                    success: function (response) {
                        if (response && !response.Error) {
                            self.items = response.data || [];
                            self.totalItems = response.totalItems || 0;
                            self.calculatePagination();
                        } else {
                            self.items = [];
                            self.totalItems = 0;
                            self.calculatePagination();
                        }
                    },
                    error: function () {
                        self.items = [];
                        self.totalItems = 0;
                        self.calculatePagination();
                    },
                    complete: function () {
                        self.isLoading = false;
                    }
                });
            },

            calculatePagination: function () {
                this.totalPages = Math.ceil(this.totalItems / this.pageSize) || 1;
                var pagesArr = [];
                var start = Math.max(1, this.currentPage - 2);
                var end = Math.min(this.totalPages, this.currentPage + 2);
                for (var i = start; i <= end; i++) {
                    pagesArr.push(i);
                }
                this.pages = pagesArr;
            },

            goToPage: function (page) {
                if (page >= 1 && page <= this.totalPages && page !== this.currentPage) {
                    this.currentPage = page;
                    this.loadData();
                }
            },

            changePageSize: function (size) {
                this.pageSize = parseInt(size) || 10;
                this.currentPage = 1;
                this.loadData();
            },

            changeStatusFilter: function (status) {
                this.statusFilter = status;
                this.currentPage = 1;
                this.loadData();
            },

            search: function () {
                this.currentPage = 1;
                this.loadData();
            },

            resetSearch: function () {
                this.keyword = '';
                this.statusFilter = '1';
                this.currentPage = 1;
                this.loadData();
            },

            loadSchedulerStatus: function () {
                var self = this;
                self.scheduler.isLoadingStatus = true;

                $.ajax({
                    type: 'POST',
                    url: '/SyncData/GetSchedulerStatus',
                    success: function (res) {
                        if (res && res.success) {
                            self.scheduler.isRunning = res.isRunning;
                            self.scheduler.isStandby = res.isStandby;
                            self.scheduler.isAutoSyncEnabled = res.isAutoSyncEnabled;
                            self.scheduler.lastStartedTime = res.lastStartedTime;
                            self.scheduler.jobCount = res.jobCount;
                            self.scheduler.jobs = res.jobs || [];
                            self.scheduler.isAdmin = res.isAdmin;
                        }
                    },
                    complete: function () {
                        self.scheduler.isLoadingStatus = false;
                    }
                });
            },

            toggleAutoSync: function () {
                var self = this;
                var targetState = !self.scheduler.isAutoSyncEnabled;
                var actionText = targetState ? "BẬT" : "TẠM DỪNG";

                if (confirm("Bạn có chắc chắn muốn " + actionText + " tiến trình Tự động Đồng bộ trên toàn hệ thống?")) {
                    $.ajax({
                        type: 'POST',
                        url: '/SyncData/ToggleAutoSync',
                        data: { enable: targetState },
                        success: function (res) {
                            if (res && res.success) {
                                toastr.success(res.message);
                                self.scheduler.isAutoSyncEnabled = res.isAutoSyncEnabled;
                                self.loadSchedulerStatus();
                            } else {
                                toastr.error(res ? res.message : "Có lỗi xảy ra");
                            }
                        },
                        error: function () {
                            toastr.error("Không thể kết nối đến máy chủ.");
                        }
                    });
                }
            },

            restartScheduler: function () {
                var self = this;
                if (confirm("Khởi động lại toàn bộ Quartz Scheduler?")) {
                    self.scheduler.isLoadingStatus = true;
                    $.ajax({
                        type: 'POST',
                        url: '/SyncData/RestartScheduler',
                        success: function (res) {
                            if (res && res.success) {
                                toastr.success(res.message);
                                self.loadSchedulerStatus();
                            } else {
                                toastr.error(res ? res.message : "Có lỗi xảy ra");
                            }
                        },
                        complete: function () {
                            self.scheduler.isLoadingStatus = false;
                        }
                    });
                }
            },

            triggerJobNow: function (reportId, apiName) {
                var self = this;
                $.ajax({
                    type: 'POST',
                    url: '/SyncData/TriggerJobNow',
                    data: { reportId: reportId },
                    success: function (res) {
                        if (res && res.success) {
                            toastr.info("Đã kích hoạt chạy ngầm cho: " + (apiName || reportId));
                            setTimeout(function () {
                                self.loadData();
                                self.loadSchedulerStatus();
                            }, 3000);
                        } else {
                            toastr.error(res ? res.message : "Có lỗi xảy ra");
                        }
                    }
                });
            },

            triggerAllNow: function () {
                var self = this;
                if (confirm("Bạn có chắc muốn kích hoạt chạy đồng bộ ngầm cho TẤT CẢ các tiến trình đang hoạt động?")) {
                    $.ajax({
                        type: 'POST',
                        url: '/SyncData/TriggerAllNow',
                        success: function (res) {
                            if (res && res.success) {
                                toastr.success(res.message);
                                setTimeout(function () {
                                    self.loadData();
                                    self.loadSchedulerStatus();
                                }, 5000);
                            } else {
                                toastr.error(res ? res.message : "Có lỗi xảy ra");
                            }
                        }
                    });
                }
            },

            manualSync: function (id, name, event) {
                var self = this;
                var btn = event ? event.target.closest('button') : null;
                if (btn) {
                    btn.disabled = true;
                    $(btn).find('i').addClass('fa-spin');
                }

                toastr.info("Đang đồng bộ trực tiếp: " + name + "...");

                $.ajax({
                    type: 'POST',
                    url: '/SyncData/SyncDataFromApi',
                    data: { Id: id },
                    success: function (res) {
                        if (res && !res.Error) {
                            toastr.success("Đồng bộ thành công: " + name);
                            self.loadData();
                            self.loadSchedulerStatus();
                        } else {
                            toastr.error("Đồng bộ thất bại: " + (res.Title || 'Lỗi không xác định'));
                        }
                    },
                    error: function () {
                        toastr.error("Không thể kết nối đến máy chủ.");
                    },
                    complete: function () {
                        if (btn) {
                            btn.disabled = false;
                            $(btn).find('i').removeClass('fa-spin');
                        }
                    }
                });
            },

            openScheduleModal: function (item) {
                this.editScheduleModal.apiId = item.Api_Id || item.ID;
                this.editScheduleModal.apiCode = item.Api_Code || '';
                this.editScheduleModal.apiName = item.NameSyncdata || item.ReportId || '';
                this.editScheduleModal.reportId = item.ReportId || '';
                this.editScheduleModal.isActive = item.IsActive === true;

                var timeLoop = item.TimeReCall || 60;
                if (timeLoop <= 23 && timeLoop >= 0) {
                    this.editScheduleModal.timeLoopType = 'daily';
                    this.editScheduleModal.dailyHour = timeLoop;
                    this.editScheduleModal.intervalValue = 60;
                } else {
                    this.editScheduleModal.timeLoopType = 'interval';
                    this.editScheduleModal.intervalValue = timeLoop;
                    this.editScheduleModal.dailyHour = 0;
                }

                this.editScheduleModal.isOpen = true;
                $('#scheduleConfigModal').modal('show');
            },

            saveScheduleConfig: function () {
                var self = this;
                var calculatedTimeLoop = 60;

                if (self.editScheduleModal.timeLoopType === 'daily') {
                    calculatedTimeLoop = parseInt(self.editScheduleModal.dailyHour);
                } else {
                    calculatedTimeLoop = parseInt(self.editScheduleModal.intervalValue);
                    if (isNaN(calculatedTimeLoop) || calculatedTimeLoop < 30) {
                        calculatedTimeLoop = 30; // Tối thiểu 30 giây
                    }
                }

                self.editScheduleModal.isSaving = true;

                $.ajax({
                    type: 'POST',
                    url: '/SyncData/UpdateSchedule',
                    data: {
                        apiId: self.editScheduleModal.apiId,
                        timeLoop: calculatedTimeLoop,
                        active: self.editScheduleModal.isActive
                    },
                    success: function (res) {
                        if (res && res.success) {
                            toastr.success(res.message);
                            $('#scheduleConfigModal').modal('hide');
                            self.loadData();
                            self.loadSchedulerStatus();
                        } else {
                            toastr.error(res ? res.message : "Có lỗi xảy ra");
                        }
                    },
                    error: function () {
                        toastr.error("Không thể lưu cấu hình.");
                    },
                    complete: function () {
                        self.editScheduleModal.isSaving = false;
                    }
                });
            },

            formatTimeLoop: function (timeLoop) {
                if (timeLoop === undefined || timeLoop === null) return 'Chưa thiết lập';
                var val = parseInt(timeLoop);
                if (val >= 0 && val <= 23) {
                    return 'Hàng ngày lúc ' + (val < 10 ? '0' + val : val) + ':00';
                }
                if (val >= 3600) {
                    var hours = (val / 3600).toFixed(1).replace('.0', '');
                    return 'Lặp lại mỗi ' + hours + ' giờ';
                }
                if (val >= 60) {
                    var mins = Math.round(val / 60);
                    return 'Lặp lại mỗi ' + mins + ' phút';
                }
                return 'Lặp lại mỗi ' + val + ' giây';
            },

            formatDateTime: function (val) {
                if (!val) return 'Chưa đồng bộ';
                if (typeof val === 'string') {
                    if (val.indexOf('/Date(') !== -1) {
                        var timestamp = parseInt(val.replace(/\/Date\((\d+)\)\//, '$1'));
                        if (!isNaN(timestamp)) {
                            var d = new Date(timestamp);
                            var day = ('0' + d.getDate()).slice(-2);
                            var month = ('0' + (d.getMonth() + 1)).slice(-2);
                            var year = d.getFullYear();
                            var hours = ('0' + d.getHours()).slice(-2);
                            var minutes = ('0' + d.getMinutes()).slice(-2);
                            var seconds = ('0' + d.getSeconds()).slice(-2);
                            return day + '/' + month + '/' + year + ' ' + hours + ':' + minutes + ':' + seconds;
                        }
                    }
                    return val;
                }
                return val;
            },

            getNextRunTime: function (reportId) {
                if (!reportId || !this.scheduler.jobs) return '—';
                var jobKey = reportId + "_Job";
                var matched = this.scheduler.jobs.find(function (j) {
                    return j.JobName === jobKey;
                });
                return matched ? matched.NextFireTime : '—';
            }
        };
    });
});
