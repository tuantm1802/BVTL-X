app.controller("NhomCapBacController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $rootScope) {
    $scope.noSelected = false;
    $scope.checkadditem = false;
    $scope.checkAll = false;
    $scope.DanhSachUpdate = [];
    $scope.ListUp = [];
    $scope.DanhSach = [];
    $scope.rows = [];
    $scope.level = 0;
    $scope.rows = [];
    angular.element(document).ready(function () {
        $scope.LoadPage();
        GetButtonAction();
    });

    // Phân quyền các nút chức năng
    $scope.RoleBtnCreate = false;
    $scope.RoleBtnSave = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnCancel = false;
    function GetButtonAction() {
        $.ajax({
            type: 'post',
            url: '/NhomCapBac/GetButtonAction',
            data: {},
            success: function (response) {
                if (response.buttons != null) {
                    angular.forEach(response.buttons, function (item) {
                        if (item == 'btnCreate') {
                            $scope.RoleBtnCreate = true;
                        }
                        if (item == 'btnSave') {
                            $scope.RoleBtnSave = true;
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

    $scope.count = 0;
    $scope.Addmm = function () {
        $scope.count = 0;
        if ($scope.checkData() == false) {
            toastr.error("Bạn chỉ được chọn một dữ liệu để thêm mới");
        } else {
            $scope.checkadditem = true;
            angular.forEach($scope.DanhSach, function (data) {
                add(data);
            });
            if ($scope.rows == '') {
            $scope.noSelected = true;
            $scope.rows.push({
                TEN_NHOM_CB: '',
                MO_TA: '',
                SO_THU_TU: '',
                KY_HIEU_VIET_TAT: '',
                pID: null, ISB: '',
                CB: false,
                ST: false
            });
            }
        }

    }
    $scope.checkData = function () {
        angular.forEach($scope.DanhSach, function (data) {
            if (data.selected == true && data.status == false) {
                $scope.count++;
            }
            loadcheckdata(data);
        });
        if ($scope.count > 1) {
            return false;
        } else {
            return true;
        }
    }
    function loadcheckdata(item) {
        var i;
        if (item.children != null) {
            for (i = 0; i < item.children.length; i++) {
                if (item.children[i].selected == true && item.children[i].status == false) {
                    $scope.count++;
                }
                loadcheckdata(item.children[i]);
            }
        }
    }

    $scope.Hiden = function () {
        $scope.rows = [];
        $scope.checkadditem = false;
        $scope.noSelected = false;
        angular.forEach($scope.DanhSach, function (data) {
            Loaditemopen(data);
            LoadHiden(data);
        });
        $scope.LoadPage();
    };
    function LoadHiden(child) {
        var item, len, i;
        child.Edit = false;
        if (child.children != null) {
            item = child.children;
            for (i = 0, len = item.length; i < len; i++) {
                LoadHiden(item[i]);
            }
        }
    }
    function add(child) {
        var item, len, i;
        if (child.selected == true && child.status == false) {
            $scope.rows.push({
                TEN_NHOM_CB: '',
                MO_TA: '', SO_THU_TU: '',
                KY_HIEU_VIET_TAT: '',
                pID: child.ID, ISB: '',
                CB: false,
                ST: false
            });
        }
        if (child.children != null) {
            item = child.children;
            for (i = 0, len = item.length; i < len; i++) {
                add(item[i]);
            }
        }
    }
    $scope.loadTemplate = function (index, level, item) {
        if (level == 0 && $scope.RoleBtnSave) {
            $scope.DanhSach[index].Edit = true;
        } else {
            load(index, level, $scope.DanhSach, item);
        }
    }
    function load(index, level, child, item) {
        var itemData = child.find(x => x.ID === item.ID);
        var indexItem = child.indexOf(itemData);
        if (itemData !== undefined && $scope.RoleBtnSave) {
            itemData.Edit = true;
            child[child.indexOf(indexItem)] = itemData;
        } else {
            angular.forEach(child, function (val, key) {
                load(index, level, val.children, item)
            })
        }
    }

    $scope.submit = function () {
        if ($scope.rows != "") {
            var flag = true;

            $scope.rows.map((item) => {
                if (item.TEN_NHOM_CB === "") {
                    flag = false;
                    item.CB = true;
                } else {
                    item.CB = false;
                }
                if (item.SO_THU_TU === "") {
                    flag = false;
                    item.ST = true;
                } else if (item.SO_THU_TU < 0 || Number.isInteger(Number(item.SO_THU_TU)) == false) {
                    toastr.error("Số thứ tự chưa chính xác.");
                    flag = false;
                } else {
                    item.ST = false;
                }
            });
            if (flag == true) {
                $.ajax({
                    type: 'post',
                    url: '/NhomCapBac/Add',
                    data: { ds: $scope.rows },
                    success: function (data) {
                        if (data.Error) {
                            toastr.error(data.Title);
                        } else {
                            toastr.success(data.Title);
                            $scope.rows = [];
                            $scope.checkadditem = false;
                            $scope.noSelected = false;
                            $scope.DanhSachUpdate = [];
                            angular.forEach($scope.DanhSach, function (data) {
                                Loaditemopen(data);
                            });
                            $scope.LoadPage();
                        }
                        hideLoading();
                    }
                });
            }
        } else {
            angular.forEach($scope.DanhSach, function (data) {
                if (data.Edit == true) {
                    if (data.SO_THU_TU === "" || data.SO_THU_TU < 0) {
                        toastr.error("Số thứ tự đã nhập chưa chính xác.");
                        return;
                    }
                    else {
                        $scope.ListUp.push(data);
                    }
                } else update(data);
            });
            $.ajax({
                type: 'post',
                url: '/NhomCapBac/edit',
                data: { ds: $scope.ListUp },
                success: function (data) {
                    if (data.Error) {
                        toastr.error(data.Title);
                    } else {
                        toastr.success(data.Title);
                        $scope.DanhSachUpdate = [];
                        angular.forEach($scope.DanhSach, function (data) {
                            Loaditemopen(data);
                        });
                        $scope.LoadPage();
                    }
                    hideLoading();
                }
            });
        }

    };
    function update(child) {
        if (child.Edit == true) {
            $scope.ListUp.push(child);
        }
        var item, len, i;
        if (child.children != null) {
            item = child.children;
            for (i = 0, len = item.length; i < len; i++) {
                update(item[i]);
            }
        }
    }

    // Check all các dong
    $scope.selectAll = false;
    $scope.CheckAll = function () {
        if ($scope.checkadditem) {
            $scope.selectAll = false;
            $scope.rows = [];
            $scope.checkadditem = false;
            toastr.error("Bạn chỉ được chọn một dữ liệu để thêm mới. Vui lòng thực hiện lại");
        } else {
            for (i = 0; i < $scope.DanhSach.length; i++) {
                $scope.DanhSach[i].selected = !$scope.selectAll;
                CheckAllitem($scope.DanhSach[i]);
            }
        }
    }

    function CheckAllitem(child) {
        var item, i;
        if (child.children != null) {
            item = child.children;
            for (i = 0; i < item.length; i++) {
                item[i].status = false;
                item[i].selected = !$scope.selectAll;
                CheckAllitem(item[i]);
            }
        }
    }
    $scope.check = false;
    $scope.checkAllchild = function (item) {
        if ($scope.checkadditem) {
            item.selected = false;
            $scope.rows = [];
            $scope.noSelected = false;
            $scope.checkadditem = false;
            Loaditemchild(item, false);
            toastr.error("Bạn chỉ được chọn một dữ liệu để thêm mới. Vui lòng thực hiện lại");
        } else {
            $scope.check = !item.selected
            Loaditemchild(item);
        }
    }
    function Loaditemopen(child) {
        var item, len, i;
        $scope.DanhSachUpdate.push({ ID: child.ID, opened: child.opened });
        if (child.children != null) {
            item = child.children;
            for (i = 0, len = item.length; i < len; i++) {
                Loaditemopen(item[i]);
            }
        }
    }
    function LoadUpdate(child) {
        var item, len, i;
        $scope.DanhSachUpdate.map((ite) => {
            if (ite.ID == child.ID) {
                child.opened = ite.opened;
            }
        });
        if (child.children != null) {
            item = child.children;
            for (i = 0, len = item.length; i < len; i++) {
                LoadUpdate(item[i]);
            }
        }
    }
    function Loaditemchild(item, ck) {
        var i;
        if (ck == false) {
            if (item.children != null) {
                for (i = 0; i < item.children.length; i++) {
                    item.children[i].selected = $scope.check;
                    item.children[i].status = true;
                    Loaditemchild(item.children[i], false);
                }
            }
        } else {
            if (item.children != null) {
                for (i = 0; i < item.children.length; i++) {
                    item.children[i].selected = $scope.check;
                    item.children[i].status = $scope.check;
                    Loaditemchild(item.children[i], true);
                }
            }
        }

    }
    $scope.LoadPage = function () {
        // Lấy danh sách
        $.ajax({
            type: 'post',
            url: '/NhomCapBac/DanhSach',
            data: {},
            success: function (data) {
                hideLoading();
                if (data.Error == true) {
                    toastr.error('Lấy dữ liệu thất bại');
                } else {
                    $scope.DanhSach = data.data;
                    if ($scope.DanhSachUpdate != null) {
                        angular.forEach($scope.DanhSach, function (data) {
                            LoadUpdate(data);
                        });
                    }
                    $scope.$apply();
                }
            }
        });
    }

    $scope.listXoa = [];
    $scope.delete = function () {
        angular.forEach($scope.DanhSach, function (data) {
            dete(data);
        })
        if ($scope.DanhSach.length == 0) {
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
                            url: '/NhomCapBac/delete',
                            data: { dt: $scope.listXoa },
                            success: function (data) {
                                if (data.Error) {
                                    toastr.error(data.notifyTitle);
                                    $scope.listXoa = [];
                                } else {
                                    toastr.success(data.notifyTitle);
                                    $scope.DanhSachUpdate = [];
                                    angular.forEach($scope.DanhSach, function (data) {
                                        Loaditemopen(data);
                                    });
                                    $scope.LoadPage();
                                    $scope.listXoa = [];
                                }
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
    };
    function dete(child) {
        if (child.selected == true) {
            $scope.listXoa.push(child);
            //CheckAll(child);
        }
        var item, len, i;
        if (child.children != null) {
            item = child.children;
            for (i = 0, len = item.length; i < len; i++) {
                dete(item[i]);
            }
        }
    }

    $rootScope.idd = true;
    $rootScope.tncb = true;
    $rootScope.mt = true;
    $rootScope.stt = true;
    $rootScope.tb = true;
    $rootScope.khvt = true;
});

