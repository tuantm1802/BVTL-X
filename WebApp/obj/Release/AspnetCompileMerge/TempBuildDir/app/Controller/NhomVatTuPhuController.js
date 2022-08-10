app.controller("NhomVatTuPhuController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $location, $rootScope) {

    $rootScope.STT = true;
    $rootScope.MaNhom = true;
    $rootScope.TenNhomVT = true;
    $rootScope.DonViTinh = true;
    $rootScope.MoTa = true;

    $scope.modelSearch = {};
    $scope.search = [];
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.DanhSach = [];
    $scope.rows = [];
    $scope.ListDVT = [];

    angular.element(document).ready(function () {
        $scope.LoadPage();
        $scope.LoadDVT();
        GetButtonAction();
    });

    $scope.RoleBtnCreate = false;
    $scope.RoleBtnSave = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnCancel = false;
    function GetButtonAction() {
        $.ajax({
            type: 'post',
            url: '/NhomVatTuPhu/GetButtonAction',
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

    $scope.selectAll = false;
    $scope.checkAll = () => {
        angular.forEach($scope.DanhSach, function (obj) {
            obj["SELECT"] = !$scope.selectAll;
            if (obj["SELECT"] === true) obj["EditMode"] = true;
            else obj["EditMode"] = false;
        })
    }

    $scope.pageChanged = function () {
        $scope.LoadPage();
    };

    $scope.loadTemplate = function (index) {
        if ($scope.RoleBtnSave)
            $scope.DanhSach[index].Edit = true;
    }
    $scope.LoadDVT = function () {
        $.ajax({
            type: 'POST',
            url: '/NhomVatTuPhu/GetDVT',
            data: {},
            success: function (data) {
                $scope.ListDVT = data.data;
                $scope.$apply();
            }
        });
    };
    $scope.LoadPage = function () {
        $.ajax({
            type: 'POST',
            url: '/NhomVatTuPhu/GetAllData',
            data: $scope.modelSearch,
            success: function (data) {
                $scope.modelSearch.totalItems = data.totalItems;
                $scope.DanhSach.Edit = false;
                $scope.DanhSach = data.data;
                angular.forEach($scope.DanhSach, function (obj) {
                    obj["EditMode"] = false;
                })
                //$scope.modelSearch.pageSize = data.pageSize;
                $scope.$apply();
            }
        });
    };

    $scope.Add = function () {
        $scope.rows.push({
            TEN_NHOM_SP: '',
            MO_TA: '',
            DVT_ID: null
        });
    };

    $scope.Submit = function () {
        if ($scope.rows != "") {
            $scope.check = true;
            angular.forEach($scope.rows, function (data) {
                if (data.TEN_NHOM_SP == null || data.TEN_NHOM_SP == "") {
                    toastr.error("Chưa nhập tên nhóm vật tư phụ");
                    $scope.check = false;
                    return;
                }
                if (data.DVT_ID == null || data.DVT_ID === 0 || data.DVT_ID === "") {
                    toastr.error("Chưa chọn đơn vị tính");
                    $scope.check = false;
                    return;
                }

            })
            if ($scope.check == true) {
                $.ajax({
                    type: 'POST',
                    url: '/NhomVatTuPhu/Add',
                    data: { dt: $scope.rows },
                    success: function (data) {
                        if (data.Error) {
                            toastr.error(data.Title);
                        } else {
                            toastr.success(data.Title);
                            $scope.rows = [];
                            $scope.LoadPage();
                        }
                        hideLoading();
                    }
                });
            }
        }
        else {
            $.ajax({
                type: 'POST',
                url: '/NhomVatTuPhu/Edit',
                data: { ds: $scope.DanhSach },
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
    };

    $scope.Undo = () => {
        $scope.rows = [];
        $scope.pageChanged();
    }

    $scope.listDelete = [];
    $scope.Delete = function () {
        angular.forEach($scope.DanhSach, function (data) {
            if (data.SELECT == true) {
                $scope.listDelete.push(data);
            }
        })
        if ($scope.listDelete.length == 0) {
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
                            url: '/NhomVatTuPhu/Delete',
                            data: { dt: $scope.listDelete },
                            success: function (data) {
                                toastr.warning(data.notifyTitle);
                                $scope.LoadPage();
                                $scope.listDelete = [];
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
    };

});
