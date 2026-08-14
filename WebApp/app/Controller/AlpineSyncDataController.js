document.addEventListener('alpine:init', function () {
    Alpine.data('alpineSyncData', function () {
        return {
            modelSearch: {
                KeyWord: '',
                Status: '',
                currentPage: 1,
                pageSize: 50,
                totalItems: 0,
                SortColumn: 'NameSyncdata'
            },
            ListData: [],
            RawListData: [],
            selectedApiId: null,
            syncingIds: {},
            stats: {
                totalApis: 0,
                activeApis: 0,
                lastSyncTime: '—'
            },

            RoleBtnSyncData: true,
            RoleBtnSearch: true,

            totalPages: 1,
            pages: [],

            init: function () {
                window.alpineSyncDataInstance = this;
                this.GetBottomAction();
                this.LoadPage(1);
            },

            GetBottomAction: function () {
                var self = this;
                $.ajax({
                    type: 'POST',
                    url: '/SyncData/GetBottomAction',
                    contentType: 'application/json',
                    data: '{}',
                    success: function (response) {
                        if (response && response.Buttoms) {
                            self.RoleBtnSyncData = response.Buttoms.indexOf('btnSyncData') !== -1;
                            self.RoleBtnSearch = response.Buttoms.indexOf('btnSearch') !== -1;
                        }
                    },
                    error: function (err) {
                        console.error(err);
                    }
                });
            },

            LoadPage: function (page) {
                var self = this;
                page = page || 1;
                self.modelSearch.currentPage = page;
                if (window.showToast) showToast();

                var postData = {
                    KeyWord: (self.modelSearch.KeyWord || '').trim(),
                    currentPage: self.modelSearch.currentPage,
                    pageSize: self.modelSearch.pageSize,
                    SortColumn: self.modelSearch.SortColumn
                };

                $.ajax({
                    type: 'POST',
                    url: '/SyncData/GetAllByPage',
                    data: postData,
                    success: function (response) {
                        if (window.hideLoading) hideLoading();
                        if (response && response.data) {
                            self.RawListData = response.data;
                            self.applyFiltersAndPagination(response.totalItems);
                        } else {
                            self.ListData = [];
                            self.RawListData = [];
                            self.modelSearch.totalItems = 0;
                            self.calculateStats();
                            self.calculatePagination();
                        }
                    },
                    error: function (err) {
                        if (window.hideLoading) hideLoading();
                        console.error(err);
                        if (window.toastr) toastr.error("Không thể tải danh sách tiến trình đồng bộ");
                    }
                });
            },

            applyFiltersAndPagination: function (serverTotal) {
                var self = this;
                serverTotal = serverTotal || 0;
                var filtered = self.RawListData.slice();

                var kw = (self.modelSearch.KeyWord || '').toLowerCase().trim();
                if (kw) {
                    filtered = filtered.filter(function (item) {
                        return (item.Api_Code && item.Api_Code.toLowerCase().indexOf(kw) !== -1) ||
                            (item.NameSyncdata && item.NameSyncdata.toLowerCase().indexOf(kw) !== -1) ||
                            (item.ReportId && item.ReportId.toLowerCase().indexOf(kw) !== -1) ||
                            (item.HrefApi && item.HrefApi.toLowerCase().indexOf(kw) !== -1);
                    });
                }

                if (self.modelSearch.Status !== '' && self.modelSearch.Status !== null && self.modelSearch.Status !== undefined) {
                    var isAct = (self.modelSearch.Status === 'true' || self.modelSearch.Status === true);
                    filtered = filtered.filter(function (item) {
                        return item.IsActive === isAct;
                    });
                }

                self.ListData = filtered;
                self.modelSearch.totalItems = (kw || self.modelSearch.Status !== '') ? filtered.length : (serverTotal || filtered.length);

                self.calculateStats();
                self.calculatePagination();
            },

            calculateStats: function () {
                var self = this;
                var act = 0;
                var latestDate = null;
                var list = (self.RawListData && self.RawListData.length > 0) ? self.RawListData : self.ListData;

                list.forEach(function (item) {
                    if (item.IsActive === true || item.IsActive === 1 || item.IsActive === 'Active') {
                        act++;
                    }
                    if (item.End_Time_Sync) {
                        var d = new Date(item.End_Time_Sync);
                        if (!isNaN(d.getTime())) {
                            if (!latestDate || d > latestDate) {
                                latestDate = d;
                            }
                        }
                    }
                });

                self.stats.totalApis = self.modelSearch.totalItems || list.length;
                self.stats.activeApis = act;
                self.stats.lastSyncTime = latestDate ? self.formatDate(latestDate) : '—';
            },

            calculatePagination: function () {
                var self = this;
                self.totalPages = Math.ceil(self.modelSearch.totalItems / self.modelSearch.pageSize) || 1;
                var current = self.modelSearch.currentPage;
                var start = Math.max(1, current - 2);
                var end = Math.min(self.totalPages, current + 2);

                self.pages = [];
                for (var i = start; i <= end; i++) {
                    self.pages.push(i);
                }
            },

            changePageSize: function (size) {
                this.modelSearch.pageSize = parseInt(size);
                this.LoadPage(1);
            },

            search: function () {
                this.LoadPage(1);
            },

            resetSearch: function () {
                this.modelSearch.KeyWord = '';
                this.modelSearch.Status = '';
                this.LoadPage(1);
            },

            selectRow: function (apiId) {
                this.selectedApiId = apiId;
            },

            formatDate: function (dateVal) {
                if (!dateVal) return '—';
                try {
                    if (window.moment) {
                        return moment(dateVal).format('DD/MM/YYYY HH:mm:ss');
                    }
                    var d = new Date(dateVal);
                    if (isNaN(d.getTime())) return dateVal;
                    var pad = function (n) { return n < 10 ? '0' + n : n; };
                    return pad(d.getDate()) + '/' + pad(d.getMonth() + 1) + '/' + d.getFullYear() + ' ' +
                        pad(d.getHours()) + ':' + pad(d.getMinutes()) + ':' + pad(d.getSeconds());
                } catch (e) {
                    return dateVal;
                }
            },

            syncDataRow: function (item) {
                var self = this;
                var apiId = item.Api_Id;
                var name = item.NameSyncdata || item.Api_Code || 'tiến trình này';

                var doSync = function () {
                    self.syncingIds[apiId] = true;
                    if (window.showToast) showToast();

                    $.ajax({
                        type: 'POST',
                        url: '/SyncData/SyncDataFromApi',
                        data: { Id: apiId },
                        success: function (data) {
                            delete self.syncingIds[apiId];
                            if (window.hideLoading) hideLoading();

                            if (data && data.Error) {
                                if (window.toastr) toastr.error(data.Title || "Đồng bộ thất bại");
                            } else {
                                if (window.toastr) toastr.success(data.Title || "Đồng bộ tiến trình thành công!");
                                self.LoadPage(self.modelSearch.currentPage);
                            }
                        },
                        error: function (err) {
                            delete self.syncingIds[apiId];
                            if (window.hideLoading) hideLoading();
                            console.error(err);
                            if (window.toastr) toastr.error("Đã xảy ra lỗi kết nối khi đồng bộ dữ liệu");
                        }
                    });
                };

                if (window.$ngConfirm) {
                    $ngConfirm({
                        title: 'Xác nhận Đồng bộ Dữ liệu',
                        content: 'Bạn có chắc chắn muốn kích hoạt đồng bộ dữ liệu thủ công cho tiến trình <b>' + name + '</b> (Mã: ' + item.Api_Code + ') không?',
                        buttons: {
                            confirm: {
                                text: 'Đồng bộ ngay',
                                btnClass: 'btn-primary',
                                action: function () { doSync(); }
                            },
                            close: { text: 'Hủy bỏ', btnClass: 'btn-secondary' }
                        }
                    });
                } else if (confirm('Bạn có chắc chắn muốn đồng bộ tiến trình ' + name + ' không?')) {
                    doSync();
                }
            }
        };
    });
});
