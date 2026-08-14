document.addEventListener('alpine:init', function () {
    Alpine.data('alpineRole', function () {
        return {
            modelSearch: {
                KeyWord: '',
                Status: '',
                currentPage: 1,
                pageSize: 10,
                totalItems: 0,
                SortColumn: 'Name'
            },
            ListRole: [],
            RawListRole: [],
            selectedRoleId: null,
            stats: {
                totalRoles: 0,
                activeRoles: 0,
                inactiveRoles: 0
            },

            RoleBtnCreate: true,
            RoleBtnUpdate: true,
            RoleBtnSearch: true,
            RoleBtnDelete: true,

            totalPages: 1,
            pages: [],

            init: function () {
                window.alpineRoleInstance = this;
                this.GetBottomAction();
                this.LoadPage(1);
            },

            GetBottomAction: function () {
                var self = this;
                $.ajax({
                    type: 'POST',
                    url: '/Role/GetBottomAction',
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
                    url: '/Role/GetAllByPage',
                    data: postData,
                    success: function (response) {
                        if (window.hideLoading) hideLoading();
                        if (response && response.data) {
                            self.RawListRole = response.data;
                            self.applyFiltersAndPagination(response.totalItems);
                        } else {
                            self.ListRole = [];
                            self.RawListRole = [];
                            self.modelSearch.totalItems = 0;
                            self.calculateStats();
                            self.calculatePagination();
                        }
                    },
                    error: function (err) {
                        if (window.hideLoading) hideLoading();
                        console.error(err);
                        if (window.toastr) toastr.error("Không thể tải danh sách nhóm quyền");
                    }
                });
            },

            applyFiltersAndPagination: function (serverTotal) {
                var self = this;
                serverTotal = serverTotal || 0;
                var filtered = self.RawListRole.slice();

                var kw = (self.modelSearch.KeyWord || '').toLowerCase().trim();
                if (kw) {
                    filtered = filtered.filter(function (item) {
                        return (item.ID && item.ID.toLowerCase().indexOf(kw) !== -1) ||
                            (item.Name && item.Name.toLowerCase().indexOf(kw) !== -1) ||
                            (item.Descripttion && item.Descripttion.toLowerCase().indexOf(kw) !== -1);
                    });
                }

                if (self.modelSearch.Status !== '' && self.modelSearch.Status !== null && self.modelSearch.Status !== undefined) {
                    var isAct = (self.modelSearch.Status === 'true' || self.modelSearch.Status === true);
                    filtered = filtered.filter(function (item) {
                        return item.IsActive === isAct;
                    });
                }

                self.ListRole = filtered;
                self.modelSearch.totalItems = (kw || self.modelSearch.Status !== '') ? filtered.length : (serverTotal || filtered.length);

                self.calculateStats();
                self.calculatePagination();
            },

            calculateStats: function () {
                var self = this;
                var act = 0;
                var inact = 0;
                var list = (self.RawListRole && self.RawListRole.length > 0) ? self.RawListRole : self.ListRole;
                list.forEach(function (item) {
                    if (item.IsActive === true || item.IsActive === 1 || item.IsActive === 'true') {
                        act++;
                    } else {
                        inact++;
                    }
                });
                self.stats.totalRoles = self.modelSearch.totalItems || list.length;
                self.stats.activeRoles = act;
                self.stats.inactiveRoles = inact;
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

            selectRow: function (roleId) {
                this.selectedRoleId = roleId;
            },

            getAngularScope: function () {
                try {
                    var elem = document.querySelector('[ng-controller="RoleController"]');
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
                    this.openBootstrapModalFallback('/Role/_Add');
                }
            },

            edit: function (id) {
                var targetId = id || this.selectedRoleId;
                if (!targetId) {
                    if (window.toastr) toastr.error("Vui lòng chọn một nhóm quyền.");
                    return;
                }
                var scope = this.getAngularScope();
                if (scope && typeof scope.edit === 'function') {
                    scope.RoleIdSeleted = targetId;
                    scope.edit(targetId);
                    if (scope.$root && !scope.$root.$$phase) scope.$applyAsync();
                } else {
                    this.openBootstrapModalFallback('/Role/_Edit', targetId);
                }
            },

            deleteRole: function (id, name) {
                var self = this;
                var targetId = id || self.selectedRoleId;
                if (!targetId) {
                    if (window.toastr) toastr.error("Vui lòng chọn một nhóm quyền.");
                    return;
                }

                var roleName = name || (self.ListRole.find(function (r) { return r.ID === targetId; }) || {}).Name || 'nhóm quyền này';

                var doDelete = function () {
                    if (window.showToast) showToast();
                    $.ajax({
                        type: 'POST',
                        url: '/Role/Delete',
                        data: { Id: targetId },
                        success: function (data) {
                            if (window.hideLoading) hideLoading();
                            if (data && data.Error) {
                                if (window.toastr) toastr.error(data.Title || "Xóa thất bại");
                            } else {
                                if (window.toastr) toastr.success(data.Title || "Xóa nhóm quyền thành công");
                                self.LoadPage(self.modelSearch.currentPage);
                            }
                        },
                        error: function () {
                            if (window.hideLoading) hideLoading();
                            if (window.toastr) toastr.error("Đã xảy ra lỗi khi xóa nhóm quyền");
                        }
                    });
                };

                if (window.$ngConfirm) {
                    $ngConfirm({
                        title: 'Xác nhận Xóa Nhóm Quyền',
                        content: 'Bạn có chắc chắn muốn xóa nhóm quyền <b>' + roleName + '</b> (' + targetId + ') không?',
                        buttons: {
                            delete: {
                                text: 'Xóa bản ghi',
                                btnClass: 'btn-danger',
                                action: function () { doDelete(); }
                            },
                            close: { text: 'Hủy', btnClass: 'btn-secondary' }
                        }
                    });
                } else if (confirm('Bạn có chắc chắn muốn xóa nhóm quyền ' + roleName + ' không?')) {
                    doDelete();
                }
            },

            openBootstrapModalFallback: function (url, itemId) {
                var self = this;
                itemId = itemId || null;
                var container = document.getElementById('roleDynamicModalContainer');
                if (!container) {
                    container = document.createElement('div');
                    container.id = 'roleDynamicModalContainer';
                    document.body.appendChild(container);
                }

                $.get(url, function (html) {
                    var modalHtml = `
                        <div class="modal fade" id="dynamicRoleModal" tabindex="-1" role="dialog" aria-hidden="true">
                            <div class="modal-dialog modal-xl" role="document">
                                <div class="modal-content border-0 shadow-lg" style="border-radius: 12px; overflow: hidden;">
                                    ${html}
                                </div>
                            </div>
                        </div>
                    `;
                    container.innerHTML = modalHtml;
                    var $modal = $('#dynamicRoleModal');
                    $modal.modal('show');

                    $modal.on('hidden.bs.modal', function () {
                        self.LoadPage(self.modelSearch.currentPage);
                    });
                });
            }
        };
    });
});
