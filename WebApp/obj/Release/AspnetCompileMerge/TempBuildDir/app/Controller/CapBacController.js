app.controller("CapBacController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $rootScope) {
    $scope.GetListCapBac = [];
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.NhomCBId = {};
    $scope.NhomCBId.idd = 0;
    $scope.rows = [];
    $scope.ItemShow = false;

    // Hiển thị dữ liệu
    angular.element(document).ready(function () {
        $scope.LoadPage();
        $scope.Loadncapbac();
        GetButtonAction();
    });

    //Phân quyền các nút chắc năng
    $scope.RoleBtnCreate = false;
    $scope.RoleBtnSave = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnCancel = false;
    $scope.isDisable = true;
    function GetButtonAction() {
        $.ajax({
            type: 'post',
            url: '/CapBac/GetButtonAction',
            data: {},
            success: function (response) {
                if (response.buttons != null) {
                    angular.forEach(response.buttons, function (item) {
                        if (item == 'btnCreate') {
                            $scope.RoleBtnCreate = true;
                        }
                        if (item == 'btnSave') {
                            $scope.RoleBtnSave = true;
                            $scope.isDisable = false;
                        }
                        if (item == 'btnDelete') {
                            $scope.RoleBtnDelete = true;
                        }
                        if (item == 'btnCancel') {
                            $scope.RoleBtnCancel = true;
                        }
                    });
                }
                $scope.$apply();
            }
        });
    }

    $scope.tableEdit = function (index) {
        if ($scope.RoleBtnSave)
            $scope.GetListCapBac[index].Edit = true;
    };


    $scope.SearchCapBac = function () {
        $scope.LoadPage();
    };

    $scope.LoadPage = function () {
        $scope.modelSearch.KeyWord = $scope.search;
        $.ajax({
            type: 'post',
            url: '/CapBac/GetListCapBac',
            data: $scope.modelSearch,
            success: function (data) {
                $scope.modelSearch.totalItems = data.totalItems;
                $scope.GetListCapBac = data.data;
                $scope.modelSearch.pageSize = data.pageSize;
                $scope.GetListCapBac.Edit = false;
                $scope.$apply();
                angular.forEach($scope.GetListCapBac, function (obj) {
                    obj["EditMode"] = false;
                })
                $scope.rows = [];
            }
        });
    };
    $scope.pageChanged = function () {
        $scope.LoadPage();
    };
    $scope.addData = function () {
        $scope.rows.push({
            TEN_CB: '',
            MO_TA: '',
            SO_THU_TU: '',
            NHOM_CB_ID: $scope.NhomCBId.idd,
            KXPH: '',
            ST: false,
            TCB: false,
            NCB: false
        });
    };
    $scope.cancelAction = function () {
        $scope.rows = [];
        $scope.pageChanged();
    };
    $scope.saveAction = function () {
        $scope.check = false;
        if ($scope.rows != "") {
            angular.forEach($scope.rows, function (data) {
                if (data.TEN_CB == null || data.TEN_CB == '') {
                    toastr.error("Chưa nhập tên cấp bậc.");
                    data.TCB = true;
                    $scope.check = false;
                    return;
                }
                if (data.SO_THU_TU < 0) {
                    toastr.error("Số thứ tự không được nhỏ hơn 0.");
                    $scope.check = false;
                    return;
                }

                $scope.NhomCBTreeInit.data.map((item) => {
                    if (data.TEN_NHOM_CB.indexOf(item.Name) !== -1) {
                        $scope.NhomCBId.idd = item.Id;
                    }
                    $scope.check = true;
                })
                if ($scope.NhomCBId.idd == null || $scope.NhomCBId.idd == 0) {
                    data.NCB = true;
                    $scope.check = false;
                    return;
                }
                else {
                    data.NHOM_CB_ID = $scope.NhomCBId.idd;
                }

            })
            if ($scope.check == true) {
                $.ajax({
                    type: 'post',
                    url: '/CapBac/Add',
                    data: { ds: $scope.rows },
                    success: function (data) {
                        if (data.Error) {
                            toastr.error(data.Title);
                        } else {
                            //$scope.ListHangHoa = [];
                            toastr.success(data.Title);
                            $scope.ItemShow = false;
                            $scope.LoadPage();
                        }
                        $scope.NhomCBId.idd = null;
                        $scope.rows = [];
                        hideLoading();
                    }
                });
            }
        } else {
            $scope.check = false;
            $scope.ListEdit = [];
            angular.forEach($scope.GetListCapBac, function (data) {
                if (data.SO_THU_TU < 0) {
                    toastr.error("Số thứ tự không được nhỏ hơn 0.");
                    $scope.check = false;
                    return;
                }

                $scope.NhomCBTreeInit.data.map((item) => {
                    if (data.TEN_NHOM_CB.indexOf(item.Name) !== -1) {
                        data.NHOM_CB_ID = item.Id;
                        $scope.check = true;
                    }
                });
                $scope.ListEdit.push(data);
            })
            if ($scope.check == true) {
                $.ajax({
                    type: 'post',
                    url: '/CapBac/Edit',
                    data: { ds: $scope.ListEdit },
                    success: function (data) {
                        if (data.Error) {
                            toastr.error(data.Title);
                        } else {
                            toastr.success(data.Title);
                            $scope.LoadPage();
                        }
                        hideLoading();
                    }
                });
            }
        }
        ;
    };

    $scope.NhomCapBacId = null;

    $scope.Loadncapbac = function () {
        $.ajax({
            type: 'post',
            url: '/CapBac/GetAllNhomCapBac',
            data: {},
            success: function (data) {
                $scope.NhomCBTreeInit = [];

                $scope.RootItem = null;
                $scope.NhomCBTreeInit.data = data.NhomCapBacs;
                console.log($scope.NhomCBTreeInit.data);
                $scope.NhomCBCallback = function (data) {
                    $scope.NhomCBComboTree = data;
                };

                $scope.$apply();
            }
        });
    }
    $scope.Loadncapbac();

    $scope.ChangeNhomCapBac = function (data) {
        angular.forEach($scope.NhomCBTreeInit.data, function (item) {
            if (item.Name.indexOf(data) !== -1) {
                $scope.NhomCBId.idd = item.Id;
            }
        })

    }

    $scope.ConvertTree = function (lstTreeModel, data, Id) {
        var lstPageMenu = data.filter(function (x) {
            return (x.ParentId == Id);
        });

        if (lstPageMenu != null && lstPageMenu.length > 0) {
            lstTreeModel.subs = [];
            for (var i = 0; i < lstPageMenu.length; i++) {
                var tree =
                {
                    id: lstPageMenu[i].Id,
                    title: lstPageMenu[i].Name
                };
                // Kiểm tra xem có con không
                var dataChilds = data.filter(function (x) {
                    return (x.ParentId == lstPageMenu[i].Id);
                });
                if (dataChilds != null && dataChilds.length > 0) {
                    $scope.ConvertTree(tree, data, lstPageMenu[i].Id);
                }
                lstTreeModel.subs.push(tree);
            }
        }
    }
    $scope.selectAll = false;
    $scope.checkAll = () => {
        angular.forEach($scope.GetListCapBac, function (obj) {
            obj["select"] = !$scope.selectAll;
            if (obj["select"] === true) obj["EditMode"] = true;
            else obj["EditMode"] = false;
        })
    }
    $scope.listXoa = [];
    $scope.deleteData = function () {
        angular.forEach($scope.GetListCapBac, function (data) {
            if (data.select == true) {
                $scope.listXoa.push(data);
            }
        })
        if ($scope.listXoa.length == 0) {
            toastr.error('Chưa chọn mục nào để xóa.');
            return;
        }
        $ngConfirm({
            title: 'Thông báo',
            content: 'Bạn có chắc chắn muốn xóa các bản ghi đã chọn không?',
            scope: $scope,
            buttons: {
                delete: {
                    text: 'Xóa',
                    btnClass: 'btn-red',
                    action: function (scope, button) {
                        $.ajax({
                            type: 'post',
                            url: '/Capbac/Delete',
                            data: { dt: $scope.listXoa },
                            success: function (data) {
                                toastr.warning(data.notifyTitle);
                                $scope.LoadPage();
                                $scope.listXoa = [];
                                $scope.selectAll = false;
                            }
                        });
                    }
                },
                close: {
                    text: 'Hủy',
                    btnClass: 'btn-green',
                    action: function (scope, button) {
                    }
                }
            }
        });
    }

    $scope.NhomCapBacId = 0;
    $scope.NhomCapBacName = "";
    $rootScope.stt = true;
    $rootScope.tenCapBac = true;
    $rootScope.moTa = true;
    $rootScope.soThuTu = true;
    $rootScope.nhomCapBac = true;
    $rootScope.koXetPhongHam = true;
});
