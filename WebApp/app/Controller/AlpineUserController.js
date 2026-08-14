document.addEventListener('alpine:init', function () {
    Alpine.data('alpineUser', function () {
        return {
            modelSearch: {
                KeyWord: '',
                Status: '',
                RoleID: '',
                currentPage: 1,
                pageSize: 10,
                totalItems: 0,
                SortColumn: 'UserName'
            },
            ListUser: [],
            RawListUser: [],
            ListRoles: [],
            selectedUserId: null,
            stats: {
                total: 0,
                active: 0,
                inactive: 0,
                rolesCount: 0
            },

            RoleBtnCreate: true,
            RoleBtnUpdate: true,
            RoleBtnSearch: true,
            RoleBtnDelete: true,
            RoleBtnView: true,

            totalPages: 1,
            pages: [],

            init: function () {
                window.alpineUserInstance = this;
                this.GetBottomAction();
                this.GetDanhMuc();
                this.LoadPage(1);
            },

            deleteUser: function (id, username) {
                var self = this;
                var targetId = id || self.selectedUserId;
                if (!targetId) {
                    if (window.toastr) toastr.error("Vui lòng chọn một người dùng.");
                    return;
                }

                var targetUser = username || (self.ListUser.find(function (u) { return u.ID === targetId; }) || {}).UserName || 'người dùng này';

                var doDelete = function () {
                    if (window.showToast) showToast();
                    $.ajax({
                        type: 'POST',
                        url: '/User/Delete',
                        data: { Id: targetId },
                        success: function (data) {
                            if (window.hideLoading) hideLoading();
                            if (data && data.Error) {
                                if (window.toastr) toastr.error(data.Title || "Thao tác thất bại");
                            } else {
                                if (window.toastr) toastr.success(data.Title || "Khóa/Tạm ngừng người dùng thành công");
                                self.GetDanhMuc();
                                self.LoadPage(self.modelSearch.currentPage);
                            }
                        },
                        error: function () {
                            if (window.hideLoading) hideLoading();
                            if (window.toastr) toastr.error("Đã xảy ra lỗi khi tạm ngừng tài khoản");
                        }
                    });
                };

                if (window.$ngConfirm) {
                    $ngConfirm({
                        title: 'Xác nhận Khóa / Tạm ngừng tài khoản',
                        content: 'Bạn có chắc chắn muốn khóa/tạm ngừng tài khoản người dùng <b>' + targetUser + '</b> không?',
                        buttons: {
                            delete: {
                                text: 'Khóa tài khoản',
                                btnClass: 'btn-danger',
                                action: function () { doDelete(); }
                            },
                            close: { text: 'Hủy', btnClass: 'btn-secondary' }
                        }
                    });
                } else if (confirm('Bạn có chắc chắn muốn khóa/tạm ngừng tài khoản người dùng ' + targetUser + ' không?')) {
                    doDelete();
                }
            },

            activeUser: function (id, username) {
                var self = this;
                var targetId = id || self.selectedUserId;
                if (!targetId) {
                    if (window.toastr) toastr.error("Vui lòng chọn một người dùng.");
                    return;
                }

                var targetUser = username || (self.ListUser.find(function (u) { return u.ID === targetId; }) || {}).UserName || 'người dùng này';

                var doActive = function () {
                    if (window.showToast) showToast();
                    $.ajax({
                        type: 'POST',
                        url: '/User/ActiveUser',
                        data: { Id: targetId },
                        success: function (data) {
                            if (window.hideLoading) hideLoading();
                            if (data && data.Error) {
                                if (window.toastr) toastr.error(data.Title || "Thao tác thất bại");
                            } else {
                                if (window.toastr) toastr.success(data.Title || "Kích hoạt tài khoản thành công");
                                self.GetDanhMuc();
                                self.LoadPage(self.modelSearch.currentPage);
                            }
                        },
                        error: function () {
                            if (window.hideLoading) hideLoading();
                            if (window.toastr) toastr.error("Đã xảy ra lỗi khi kích hoạt tài khoản");
                        }
                    });
                };

                if (window.$ngConfirm) {
                    $ngConfirm({
                        title: 'Xác nhận Kích hoạt tài khoản',
                        content: 'Bạn có chắc chắn muốn kích hoạt lại tài khoản người dùng <b>' + targetUser + '</b> không?',
                        buttons: {
                            delete: {
                                text: 'Kích hoạt',
                                btnClass: 'btn-success',
                                action: function () { doActive(); }
                            },
                            close: { text: 'Hủy', btnClass: 'btn-secondary' }
                        }
                    });
                } else if (confirm('Bạn có chắc chắn muốn kích hoạt tài khoản người dùng ' + targetUser + ' không?')) {
                    doActive();
                }
            },

            GetBottomAction: function () {
                var self = this;
                $.ajax({
                    type: 'POST',
                    url: '/User/GetBottomAction',
                    contentType: 'application/json',
                    data: '{}',
                    success: function (response) {
                        if (response && response.Buttoms) {
                            self.RoleBtnCreate = response.Buttoms.indexOf('btnCreate') !== -1;
                            self.RoleBtnUpdate = response.Buttoms.indexOf('btnUpdate') !== -1;
                            self.RoleBtnSearch = response.Buttoms.indexOf('btnSearch') !== -1;
                            self.RoleBtnDelete = response.Buttoms.indexOf('btnDelete') !== -1;
                            self.RoleBtnView = response.Buttoms.indexOf('btnView') !== -1;
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
                    url: '/User/GetDanhMuc',
                    contentType: 'application/json',
                    data: '{}',
                    success: function (response) {
                        if (response) {
                            if (response.DataRoles) {
                                self.ListRoles = response.DataRoles;
                                self.stats.rolesCount = response.DataRoles.length;
                            }
                            if (response.systemTotalUsers !== undefined) {
                                self.stats.total = response.systemTotalUsers;
                                self.stats.active = response.systemActiveUsers;
                                self.stats.inactive = response.systemInactiveUsers;
                            }
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
                    Status: self.modelSearch.Status,
                    RoleID: self.modelSearch.RoleID,
                    currentPage: self.modelSearch.currentPage,
                    pageSize: self.modelSearch.pageSize,
                    SortColumn: self.modelSearch.SortColumn
                };

                $.ajax({
                    type: 'POST',
                    url: '/User/GetListUser',
                    data: postData,
                    success: function (response) {
                        if (window.hideLoading) hideLoading();
                        if (response && response.data) {
                            self.RawListUser = response.data;
                            self.applyFiltersAndPagination(response.totalItems, response);
                        } else {
                            self.ListUser = [];
                            self.RawListUser = [];
                            self.modelSearch.totalItems = 0;
                            self.calculateStats();
                            self.calculatePagination();
                        }
                    },
                    error: function (err) {
                        if (window.hideLoading) hideLoading();
                        console.error(err);
                        if (window.toastr) toastr.error("Không thể tải danh sách người dùng");
                    }
                });
            },

            applyFiltersAndPagination: function (serverTotal, responseObj) {
                serverTotal = serverTotal || 0;
                responseObj = responseObj || null;
                var self = this;
                var filtered = self.RawListUser.slice();

                var kw = (self.modelSearch.KeyWord || '').toLowerCase().trim();
                if (kw) {
                    filtered = filtered.filter(function (u) {
                        return (u.UserName && u.UserName.toLowerCase().indexOf(kw) !== -1) ||
                            (u.Name && u.Name.toLowerCase().indexOf(kw) !== -1) ||
                            (u.IdNumber && u.IdNumber.toLowerCase().indexOf(kw) !== -1) ||
                            (u.Email && u.Email.toLowerCase().indexOf(kw) !== -1);
                    });
                }

                if (self.modelSearch.RoleID) {
                    filtered = filtered.filter(function (u) {
                        return u.GroupID == self.modelSearch.RoleID || u.UserGroupID == self.modelSearch.RoleID;
                    });
                }

                if (self.modelSearch.Status !== '' && self.modelSearch.Status !== null && self.modelSearch.Status !== undefined) {
                    var isAct = (self.modelSearch.Status === 'true' || self.modelSearch.Status === true);
                    filtered = filtered.filter(function (u) {
                        var uStatus = (u.Status === true || u.Status === 1 || u.Status === 'Active');
                        return isAct ? uStatus : !uStatus;
                    });
                }

                self.ListUser = filtered;
                self.modelSearch.totalItems = (kw || self.modelSearch.RoleID || self.modelSearch.Status !== '') ? filtered.length : (serverTotal || filtered.length);

                if (responseObj && responseObj.systemTotalUsers !== undefined) {
                    self.stats.total = responseObj.systemTotalUsers;
                    self.stats.active = responseObj.systemActiveUsers;
                    self.stats.inactive = responseObj.systemInactiveUsers;
                } else {
                    self.calculateStats();
                }

                self.calculatePagination();
            },

            calculateStats: function () {
                var self = this;
                var act = 0;
                var inact = 0;
                var sourceList = (self.RawListUser && self.RawListUser.length > 0) ? self.RawListUser : self.ListUser;
                if (sourceList && sourceList.length > 0) {
                    sourceList.forEach(function (u) {
                        if (u.Status === true || u.Status === 1 || u.Status === 'Active') {
                            act++;
                        } else {
                            inact++;
                        }
                    });
                }
                if (!self.stats.total) {
                    self.stats.total = self.modelSearch.totalItems;
                    self.stats.active = act;
                    self.stats.inactive = inact;
                }
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
                this.modelSearch.RoleID = '';
                this.LoadPage(1);
            },

            selectRow: function (userId) {
                this.selectedUserId = userId;
            },

            getInitials: function (name) {
                if (!name) return 'U';
                var parts = name.trim().split(' ');
                if (parts.length === 1) return parts[0].charAt(0).toUpperCase();
                return (parts[0].charAt(0) + parts[parts.length - 1].charAt(0)).toUpperCase();
            },

            getRandomColorClass: function (name) {
                var classes = ['bg-primary', 'bg-success', 'bg-info', 'bg-warning text-dark', 'bg-secondary', 'bg-danger'];
                if (!name) return classes[0];
                var hash = 0;
                for (var i = 0; i < name.length; i++) {
                    hash = name.charCodeAt(i) + ((hash << 5) - hash);
                }
                var index = Math.abs(hash) % classes.length;
                return classes[index];
            },

            getAngularScope: function () {
                try {
                    var elem = document.querySelector('[ng-controller="UserController"]');
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
                    this.openBootstrapModalFallback('/User/_Add');
                }
            },

            edit: function (id) {
                var targetId = id || this.selectedUserId;
                if (!targetId) {
                    if (window.toastr) toastr.error("Vui lòng chọn một người dùng.");
                    return;
                }
                var scope = this.getAngularScope();
                if (scope && typeof scope.edit === 'function') {
                    scope.edit(targetId);
                    if (scope.$root && !scope.$root.$$phase) scope.$applyAsync();
                } else {
                    this.openBootstrapModalFallback('/User/_Edit', targetId);
                }
            },

            viewDetail: function (id) {
                var targetId = id || this.selectedUserId;
                if (!targetId) {
                    if (window.toastr) toastr.error("Vui lòng chọn một người dùng.");
                    return;
                }
                var scope = this.getAngularScope();
                if (scope && typeof scope.ViewDetail === 'function') {
                    scope.ViewDetail(targetId);
                    if (scope.$root && !scope.$root.$$phase) scope.$applyAsync();
                } else {
                    this.openBootstrapModalFallback('/User/_View', targetId);
                }
            },

            resetPassword: function (id) {
                var targetId = id || this.selectedUserId;
                if (!targetId) {
                    if (window.toastr) toastr.error("Vui lòng chọn một người dùng.");
                    return;
                }
                var scope = this.getAngularScope();
                if (scope && typeof scope.resetPassword === 'function') {
                    scope.resetPassword(targetId);
                    if (scope.$root && !scope.$root.$$phase) scope.$applyAsync();
                } else {
                    this.openBootstrapModalFallback('/User/_ResetPassword', targetId);
                }
            },

            openBootstrapModalFallback: function (url, itemId) {
                var self = this;
                itemId = itemId || null;
                var container = document.getElementById('userDynamicModalContainer');
                if (!container) {
                    container = document.createElement('div');
                    container.id = 'userDynamicModalContainer';
                    document.body.appendChild(container);
                }

                $.get(url, function (html) {
                    var modalHtml = `
                        <div class="modal fade" id="dynamicUserModal" tabindex="-1" role="dialog" aria-hidden="true">
                            <div class="modal-dialog modal-xl" role="document">
                                <div class="modal-content">
                                    ${html}
                                </div>
                            </div>
                        </div>
                    `;
                    container.innerHTML = modalHtml;
                    var $modal = $('#dynamicUserModal');
                    $modal.modal('show');

                    $modal.on('hidden.bs.modal', function () {
                        self.LoadPage(self.modelSearch.currentPage);
                    });
                });
            }
        };
    });
});
