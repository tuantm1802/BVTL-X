document.addEventListener('alpine:init', function () {
    Alpine.data('alpineTestGroup', function () {
        return {
            modelSearch: {
                KeyWord: '',
                CityCode: '',
                currentPage: 1,
                pageSize: 10,
                totalItems: 0,
                SortColumn: 'manhom_tbh'
            },
            ListData: [],
            RawListData: [],
            ListCity: [],
            selectedGroupId: null,
            stats: {
                totalGroups: 0,
                totalCities: 0
            },

            RoleBtnCreate: true,
            RoleBtnUpdate: true,
            RoleBtnSearch: true,
            RoleBtnDelete: true,

            totalPages: 1,
            pages: [],

            init: function () {
                window.alpineTestGroupInstance = this;
                this.GetBottomAction();
                this.GetDanhMuc();
                this.LoadPage(1);
            },

            GetBottomAction: function () {
                var self = this;
                $.ajax({
                    type: 'POST',
                    url: '/TestGroup/GetBottomAction',
                    contentType: 'application/json',
                    data: '{}',
                    success: function (response) {
                        if (response && response.Buttoms) {
                            self.RoleBtnCreate = response.Buttoms.indexOf('btnCreate') !== -1;
                            self.RoleBtnUpdate = response.Buttoms.indexOf('btnUpdate') !== -1;
                            self.RoleBtnSearch = response.Buttoms.indexOf('btnSearch') !== -1;
                            self.RoleBtnDelete = response.Buttoms.indexOf('btnDelete') !== -1;
                        }
                    },
                    error: function (err) {
                        console.error(err);
                    }
                });
            },

            GetDanhMuc: function () {
                var self = this;
                $.ajax({
                    type: 'POST',
                    url: '/TestGroup/GetDanhMuc',
                    contentType: 'application/json',
                    data: '{}',
                    success: function (response) {
                        if (response && response.Citys) {
                            self.ListCity = response.Citys;
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
                    CityCode: self.modelSearch.CityCode || '',
                    currentPage: self.modelSearch.currentPage,
                    pageSize: self.modelSearch.pageSize,
                    SortColumn: self.modelSearch.SortColumn
                };

                $.ajax({
                    type: 'POST',
                    url: '/TestGroup/GetAllByPage',
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
                        if (window.toastr) toastr.error("Không thể tải danh sách Nhóm thu thập dữ liệu");
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
                        return (item.manhom_tbh && item.manhom_tbh.toLowerCase().indexOf(kw) !== -1) ||
                            (item.tennhom_tbh && item.tennhom_tbh.toLowerCase().indexOf(kw) !== -1) ||
                            (item.CityName && item.CityName.toLowerCase().indexOf(kw) !== -1);
                    });
                }

                if (self.modelSearch.CityCode) {
                    filtered = filtered.filter(function (item) {
                        return item.city_code === self.modelSearch.CityCode;
                    });
                }

                self.ListData = filtered;
                self.modelSearch.totalItems = (kw || self.modelSearch.CityCode) ? filtered.length : (serverTotal || filtered.length);

                self.calculateStats();
                self.calculatePagination();
            },

            calculateStats: function () {
                var self = this;
                self.stats.totalGroups = self.modelSearch.totalItems || self.RawListData.length;
                var citiesSet = {};
                var list = (self.RawListData && self.RawListData.length > 0) ? self.RawListData : self.ListData;
                list.forEach(function (item) {
                    if (item.CityName) {
                        citiesSet[item.CityName] = true;
                    }
                });
                self.stats.totalCities = Object.keys(citiesSet).length;
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
                this.modelSearch.CityCode = '';
                this.LoadPage(1);
            },

            selectRow: function (groupId) {
                this.selectedGroupId = groupId;
            },

            getAngularScope: function () {
                try {
                    var elem = document.querySelector('[ng-controller="TestGroupController"]');
                    if (elem && window.angular) {
                        return angular.element(elem).scope();
                    }
                } catch (e) {
                    console.error("Error getting angular scope:", e);
                }
                return null;
            },

            add: function () {
                var scope = this.getAngularScope();
                if (scope && typeof scope.add === 'function') {
                    scope.add();
                    if (scope.$root && !scope.$root.$$phase) scope.$applyAsync();
                } else {
                    this.openBootstrapModalFallback('/TestGroup/_Add');
                }
            },

            edit: function (code) {
                var targetCode = code || this.selectedGroupId;
                if (!targetCode) {
                    if (window.toastr) toastr.error("Vui lòng chọn một nhóm thu thập dữ liệu.");
                    return;
                }
                var scope = this.getAngularScope();
                if (scope && typeof scope.edit === 'function') {
                    scope.NhomTTDLIdSeleted = targetCode;
                    scope.edit(targetCode);
                    if (scope.$root && !scope.$root.$$phase) scope.$applyAsync();
                } else {
                    this.openBootstrapModalFallback('/TestGroup/_Edit', targetCode);
                }
            },

            deleteGroup: function (code, name) {
                var self = this;
                var targetCode = code || self.selectedGroupId;
                if (!targetCode) {
                    if (window.toastr) toastr.error("Vui lòng chọn một nhóm thu thập dữ liệu.");
                    return;
                }

                var groupName = name || (self.ListData.find(function (g) { return g.manhom_tbh === targetCode; }) || {}).tennhom_tbh || 'nhóm này';

                var doDelete = function () {
                    if (window.showToast) showToast();
                    $.ajax({
                        type: 'POST',
                        url: '/TestGroup/Delete',
                        data: { maNhom: targetCode },
                        success: function (data) {
                            if (window.hideLoading) hideLoading();
                            if (data && data.Error) {
                                if (window.toastr) toastr.error(data.Title || "Xóa thất bại");
                            } else {
                                if (window.toastr) toastr.success(data.Title || "Xóa nhóm thu thập thành công");
                                self.LoadPage(self.modelSearch.currentPage);
                            }
                        },
                        error: function () {
                            if (window.hideLoading) hideLoading();
                            if (window.toastr) toastr.error("Đã xảy ra lỗi khi xóa nhóm");
                        }
                    });
                };

                if (window.$ngConfirm) {
                    $ngConfirm({
                        title: 'Xác nhận Xóa Nhóm TTDL',
                        content: 'Bạn có chắc chắn muốn xóa nhóm thu thập dữ liệu <b>' + groupName + '</b> (' + targetCode + ') không?',
                        buttons: {
                            delete: {
                                text: 'Xóa bản ghi',
                                btnClass: 'btn-danger',
                                action: function () { doDelete(); }
                            },
                            close: { text: 'Hủy', btnClass: 'btn-secondary' }
                        }
                    });
                } else if (confirm('Bạn có chắc chắn muốn xóa nhóm ' + groupName + ' không?')) {
                    doDelete();
                }
            },

            openBootstrapModalFallback: function (url, itemId) {
                var self = this;
                itemId = itemId || null;
                var container = document.getElementById('testGroupDynamicModalContainer');
                if (!container) {
                    container = document.createElement('div');
                    container.id = 'testGroupDynamicModalContainer';
                    document.body.appendChild(container);
                }

                $.get(url, function (html) {
                    var modalHtml = `
                        <div class="modal fade" id="dynamicTestGroupModal" tabindex="-1" role="dialog" aria-hidden="true">
                            <div class="modal-dialog modal-xl" role="document">
                                <div class="modal-content border-0 shadow-lg" style="border-radius: 12px; overflow: hidden;">
                                    ${html}
                                </div>
                            </div>
                        </div>
                    `;
                    container.innerHTML = modalHtml;
                    var $modal = $('#dynamicTestGroupModal');
                    $modal.modal('show');

                    $modal.on('hidden.bs.modal', function () {
                        self.LoadPage(self.modelSearch.currentPage);
                    });
                });
            }
        };
    });
});
