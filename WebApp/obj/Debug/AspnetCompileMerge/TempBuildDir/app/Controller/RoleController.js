app.controller("RoleController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.KeyWord = '';
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "ROLE_TYPE DESC";
    $scope.ReverseSort = false;
    $scope.ListRole = [];

    angular.element(document).ready(function () {
        showToast();
        GetBottomAction();
        $scope.LoadPage();
    });

    $scope.RoleBtnCreate = false;
    $scope.RoleBtnUpdate = false;
    $scope.RoleBtnSearch = false;
    $scope.RoleBtnDelete = false;

    function GetBottomAction() {
        $.ajax({
            type: 'post',
            url: '/Role/GetBottomAction',
            data: {},
            success: function (response) {
                if (response.Buttoms != null) {
                    angular.forEach(response.Buttoms, function (item) {
                        if (item == 'btnCreate') {
                            $scope.RoleBtnCreate = true;
                        }
                        if (item == 'btnUpdate') {
                            $scope.RoleBtnUpdate = true;
                        }
                        if (item == 'btnSearch') {
                            $scope.RoleBtnSearch = true;
                        }
                        if (item == 'btnDelete') {
                            $scope.RoleBtnDelete = true;
                        }
                    });
                }
                console.log('RoleBtnUpdate');
                console.log($scope.RoleBtnUpdate);
                $scope.$apply();
            }
        });
    }

    $scope.pageChanged = function () {
        $scope.LoadPage();
    };

    $scope.ViewDetail = function (ID, rowIndex) {
        $scope.selectedRow = rowIndex;
    };

    $scope.LoadPage = function () {
        $.ajax({
            type: 'post',
            url: '/Role/GetAllByPage',
            data: $scope.modelSearch,
            success: function (data) {
                $scope.modelSearch.totalItems = data.totalItems;
                $scope.ListRole = data.data;
                $scope.modelSearch.pageSize = data.pageSize;
                $scope.$apply();
                hideLoading();
            }
        });
    };



    $scope.Refesh = function () {
        $scope.LoadPage();
    };

    $scope.Sort = function (event, sortRow) {
        if (event.currentTarget.classList.contains('arrow-up')) {
            $scope.modelSearch.SortColumn = sortRow + " DESC";
            $scope.LoadPage();
        }
        if (event.currentTarget.classList.contains('arrow-down')) {
            $scope.modelSearch.SortColumn = sortRow + " ASC";
            $scope.LoadPage();
        }
    };

    $scope.add = function () {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/Role/_Add',
            controller: 'add',
            size: 'xl',
            backdrop: 'static'
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            $scope.LoadPage();
        });
    };
    $scope.edit = function (itemId) {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/Role/_Edit',
            controller: 'edit',
            size: 'xl',
            backdrop: 'static',
            resolve: {
                itemId: function () {
                    return itemId;
                }
            }
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            $scope.LoadPage();
        });
    };
    $scope.delete = function (itemId, name) {
        $ngConfirm({
            title: 'Thông báo',
            content: 'Bạn có chắc chắn muốn xóa quyền ' + name + ' không?',
            scope: $scope,
            buttons: {
                delete: {
                    text: 'Xóa',
                    btnClass: 'btn-blue',
                    action: function (scope, button) {
                        $.ajax({
                            type: 'post',
                            url: '/Role/Delete',
                            data: { Id: itemId },
                            success: function (data) {
                                if (data.Error) {
                                    toastr.error(data.Title);
                                } else {
                                    toastr.success(data.Title);
                                    //$scope.cancel();
                                    $scope.LoadPage();
                                }
                            }
                        });

                    }
                },
                close: {
                    text: 'Hủy',
                    action: function (scope, button) {

                    }
                }
            }
        });
    };
    $scope.SortData = function (column) {
        var _columns = $scope.modelSearch.SortColumn.split(' ');;
        $scope.ReverseSort = (_columns[0] == column) ? !$scope.ReverseSort : false;
        var sortDirection = $scope.ReverseSort ? 'DESC' : 'ASC';
        $scope.modelSearch.SortColumn = column + ' ' + sortDirection;
        $scope.LoadPage();
    }

});

app.controller('add', function ($scope, $uibModalInstance, $ngConfirm, showToast, hideLoading) {
    $scope.ListPageMenu = [];
    $scope.ListUnit = [];
    $scope.ListSCOPE = [/*{ ID: "A", Name: 'Toàn hệ thống' },*/ /*{ ID: "D", Name: 'Áp dụng theo Domain_code' },*/ { ID: "U", Name: 'Áp dụng theo đơn vị' }];
    $scope.ListAppCode = [];
    angular.element(document).ready(function () {
        showToast();
        GetDanhMuc();
    });

    var setting = {
        check: {
            enable: true
        },
        data: {
            simpleData: {
                enable: true,
                idKey: "id",
                pIdKey: "pId",
                rootPId: ""
            }
        }
    };


    function GetDanhMuc() {
        $scope.ListPageMenu = [];
        $scope.ListUnit = [];
        $scope.ListAppCode = [];
        $.ajax({
            type: 'post',
            url: '/Role/GetDanhMuc',
            data: {},
            success: function (data) {
                hideLoading();
                $scope.ListUnit = data.Units;
                $scope.ListAppCode = data.AppCodes;
                $scope.model.SCOPE = $scope.ListSCOPE[0].ID;
                $scope.model.APP_CODE = $scope.ListAppCode[0].APP_CODE;
                $scope.$apply();
                $scope.ListPageMenu = data.TreeDatas;
                $.fn.zTree.init($("#treeRole"), setting, $scope.ListPageMenu);
                var zTree = $.fn.zTree.getZTreeObj("treeRole");
                var type = { "Y": "ps", "N": "ps" };
                zTree.setting.check.chkboxType = type;

            }
        });
    }



    $scope.ChangeAppCode = function (e) {
        $scope.ListPageMenu = [];
        if ($scope.model.APP_CODE != null && $scope.model.APP_CODE != '') {
            $.ajax({
                type: 'post',
                url: '/Role/GetPageMenuByAppCode',
                data: { appCode: $scope.model.APP_CODE, Id: 0 },
                success: function (data) {
                    hideLoading();
                    $scope.ListPageMenu = data.TreeDatas;
                    $.fn.zTree.init($("#treeRole"), setting, $scope.ListPageMenu);
                    var zTree = $.fn.zTree.getZTreeObj("treeRole");
                    var type = { "Y": "ps", "N": "ps" };
                    zTree.setting.check.chkboxType = type;

                }
            });
        }
    };

    $scope.model = {};
    $scope.submit = function () {
        $scope.model.Status = $scope.model.StatusTemp === '1' ? true : false;
        $("#formSubmit").validate({
            rules: {
                ROLE_TYPE: {
                    required: true,
                    maxlength: 50
                },
                SCOPE: {
                    required: true
                },
                APP_CODE: {
                    required: true
                } ,
                ROLE_DESC: {
                    required: true
                }
                

            },
            messages: {
                ROLE_TYPE: {
                    required: "Vui lòng nhập mã",
                    maxlength: "Mã không được vượt quá 50 ký tự"
                },
                SCOPE: {
                    required: "Vui lòng nhập Phạm vi"
                },
                APP_CODE: {
                    required: "Vui lòng nhập Phòng ban"
                },
                ROLE_DESC: {
                    required: "Vui lòng nhập Tên quyền"
                }
            }
        });
        if ($("#formSubmit").valid()) {

            var treeObj = $.fn.zTree.getZTreeObj("treeRole");
            var nodes = treeObj.getCheckedNodes();

            angular.forEach($scope.ListPageMenu, function (pageMenu) {

                var dataSearch = nodes.filter(function (item) {
                    return item.id === pageMenu.id;
                });

                if (dataSearch !== null && dataSearch.length > 0) {
                    pageMenu.checked = true;
                }
                else {
                    pageMenu.checked = false;
                }
            })

            if ($scope.model.SCOPE == "D" && ($scope.model.DOMAIN_CODE == null || $scope.model.DOMAIN_CODE == "")) {
                toastr.error("Bạn chưa nhập DOMAIN_CODE!");
                return;
            }
            if ($scope.model.SCOPE == "U" && ($scope.model.UNIT_ID == null || $scope.model.UNIT_ID == "" || $scope.model.UNIT_ID == 0)) {
                toastr.error("Bạn chưa chọn Đơn vị!");
                return;
            }

            $.ajax({
                type: 'post',
                url: '/Role/Add',
                data: { role: $scope.model, pageMenus: $scope.ListPageMenu },
                success: function (data) {
                    if (data.Error) {
                        toastr.error(data.Title);
                    } else {
                        toastr.success(data.Title);
                        $scope.cancel();
                    }
                }
            });
        }
    };


    $scope.cancel = function () {
        $uibModalInstance.close();
    };

    $scope.SelectFile = function (e) {
        $scope.model.Avartar = e.target.files[0];
        document.getElementById("pathPhoto").src = e.target.files[0];
    };
});

app.controller('edit', function ($scope, $uibModalInstance, itemId, $ngConfirm, showToast, hideLoading) {
    $scope.ListPageMenu = [];
    $scope.ListUnit = [];
    $scope.ListAppCode = [];
    $scope.ListSCOPE = [/*{ ID: "A", Name: 'Toàn hệ thống' }, { ID: "D", Name: 'Áp dụng theo Domain_code' },*/ { ID: "U", Name: 'Áp dụng theo đơn vị' }];
    $scope.ListRole = [];
    var setting = {
        check: {
            enable: true
        },
        data: {
            simpleData: {
                enable: true,
                idKey: "id",
                pIdKey: "pId",
                rootPId: ""
            }
        }
    };


    $scope.model = {};
    angular.element(document).ready(function () {
        GetDanhMuc();
        $.ajax({
            type: 'post',
            url: '/Role/GetItemByID',
            data: { Id: itemId },
            success: function (data) {
                if (data.Error) {
                    toastr.error(data.Title);
                } else {
                    $scope.model = data.data;

                    $scope.ListPageMenu = data.TreeDatas;
                    $.fn.zTree.init($("#treeRole"), setting, $scope.ListPageMenu);
                    var zTree = $.fn.zTree.getZTreeObj("treeRole");
                    var type = { "Y": "ps", "N": "ps" };
                    zTree.setting.check.chkboxType = type;
                    $scope.$apply();
                }
            }
        });
    });

    function GetDanhMuc() {
        $scope.ListUnit = [];
        $scope.ListAppCode = [];
        $.ajax({
            type: 'post',
            url: '/Role/GetDanhMuc',
            data: {},
            success: function (data) {
                hideLoading();
                $scope.ListUnit = data.Units;
                $scope.ListAppCode = data.AppCodes;
            }
        });
    }

    $scope.ChangeAppCode = function (e) {
        $scope.ListPageMenu = [];
        if ($scope.model.APP_CODE != null && $scope.model.APP_CODE != '') {
            $.ajax({
                type: 'post',
                url: '/Role/GetPageMenuByAppCode',
                data: { appCode: $scope.model.APP_CODE, Id: $scope.model.ROLE_ID },
                success: function (data) {
                    hideLoading();
                    $scope.ListPageMenu = data.TreeDatas;
                    $.fn.zTree.init($("#treeRole"), setting, $scope.ListPageMenu);
                    var zTree = $.fn.zTree.getZTreeObj("treeRole");
                    var type = { "Y": "ps", "N": "ps" };
                    zTree.setting.check.chkboxType = type;

                }
            });
        }
    };

    $scope.submit = function () {
        $scope.model.Status = $scope.model.StatusTemp === '1' ? true : false;
        $("#formSubmit").validate({
            rules: {
                ROLE_TYPE: {
                    required: true,
                    maxlength: 50
                },
                SCOPE: {
                    required: true
                },
                APP_CODE: {
                    required: true
                },
                ROLE_DESC: {
                    required: true
                }


            },
            messages: {
                ROLE_TYPE: {
                    required: "Vui lòng nhập mã",
                    maxlength: "Mã không được vượt quá 50 ký tự"
                },
                SCOPE: {
                    required: "Vui lòng nhập Phạm vi"
                },
                APP_CODE: {
                    required: "Vui lòng nhập Phòng ban"
                },
                ROLE_DESC: {
                    required: "Vui lòng nhập Tên quyền"
                }
            }
        });
        if ($("#formSubmit").valid()) {

            var treeObj = $.fn.zTree.getZTreeObj("treeRole");
            var nodes = treeObj.getCheckedNodes();

            angular.forEach($scope.ListPageMenu, function (pageMenu) {

                var dataSearch = nodes.filter(function (item) {
                    return item.id === pageMenu.id;
                });

                if (dataSearch !== null && dataSearch.length > 0) {
                    pageMenu.checked = true;
                }
                else {
                    pageMenu.checked = false;
                }

            })

            if ($scope.model.SCOPE == "D" && ($scope.model.DOMAIN_CODE == null || $scope.model.DOMAIN_CODE == "")) {
                toastr.error("Bạn chưa nhập DOMAIN_CODE!");
                return;
            }
            if ($scope.model.SCOPE == "U" && ($scope.model.UNIT_ID == null || $scope.model.UNIT_ID == "" || $scope.model.UNIT_ID == 0)) {
                toastr.error("Bạn chưa chọn Đơn vị!");
                return;
            }

            $.ajax({
                type: 'post',
                url: '/Role/Edit',
                data: { role: $scope.model, pageMenus: $scope.ListPageMenu },
                success: function (data) {
                    if (data.Error) {
                        toastr.error(data.Title);
                    } else {
                        toastr.success(data.Title);
                        $scope.cancel();
                        $scope.LoadPage();
                    }
                }
            });
        }
    };

    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});