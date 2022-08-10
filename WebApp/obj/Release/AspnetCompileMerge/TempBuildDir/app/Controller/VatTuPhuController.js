app.controller("VatTuPhuController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $rootScope) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.DSNhomVTP = [];
    $scope.ListDVT = [];
    $scope.DSVatTuPhu = [];
    $scope.nhomVTPSearch = {};
    $scope.tnsp = [];
    $scope.rows = [];
    $scope.selectedRowNhomVTP = -1;
    $scope.ShowContent = false;
    $rootScope.STT = true;
    $rootScope.TenVatTu = true;
    $rootScope.MoTa = true;
    $rootScope.NhomVT = true;
    $rootScope.DonViTinh = true;

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
            url: '/VatTuPhu/GetButtonAction',
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

    $scope.pageChanged = function () {
        $scope.LoadPage();
    };
    $scope.Undo = function () {
        $scope.rows = [];
        angular.forEach($scope.DSVatTuPhu, function (data) {
            data.Edit = false;
        })
    }
    $scope.SelectedAll = function () {
        if (!$scope.SELECTALL) {
            angular.forEach($scope.DSVatTuPhu, function (data) {
                data.SELECT = true;
            });
        } else {
            angular.forEach($scope.DSVatTuPhu, function (data) {
                data.SELECT = false;
            });
        }
    }
    $scope.loadTemplate = function (index) {
        $scope.DSVatTuPhu[index].Edit = true;
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
    $scope.Add = function () {
        $scope.rows.push({
            TEN_SP: '',
            MO_TA: '',
            IS_VAT_TU: 'Y',
            NHOM_SP_ID: $scope.tnsp[0].ID,
            TEN_NHOM_SP: $scope.tnsp[0].TEN_NHOM_SP,
            TSP: false,
            DVTID: false
        });
    }
    $scope.Submit = function () {
        if ($scope.rows != "") {
            var flag = true;

            $scope.rows.map((item) => {
                if (item.TEN_SP === "") {
                    flag = false;
                    item.TSP = true;
                } else {
                    item.TSP = false;
                }
                if (item.DVT_ID === undefined) {
                    flag = false;
                    item.DVTID = true;
                } else {
                    item.DVTID = false;
                }

            });
            if (flag == true) {
                $.ajax({
                    type: 'POST',
                    url: '/VatTuPhu/Add',
                    data: { dt: $scope.rows },
                    success: function (data) {
                        if (data.error) {
                            toastr.error('Thêm mới dữ liệu lỗi');
                        } else {
                            hideLoading();
                            $scope.pageChanged();
                            GetDSVatTuPhu($scope.tnsp[0]);
                            $scope.rows = [];
                            toastr.success('Thêm mới dữ liệu thành công');
                            $scope.$apply();
                        }

                    }
                });
            }

        }
        else {
            $scope.ListEdit = [];
            angular.forEach($scope.DSVatTuPhu, function (data) {
                if (data.Edit == true) {
                    $scope.ListEdit.push(data);
                }
            })
            $.ajax({
                type: 'POST',
                url: '/VatTuPhu/Edit',
                data: { ds: $scope.ListEdit },
                success: function (data) {
                    if (data.Error) {
                        toastr.error('Sửa thất bại');
                    }
                    else {
                        toastr.success('Sửa thành công');
                        $scope.pageChanged();
                        GetDSVatTuPhu($scope.tnsp[0]);
                        $scope.$apply();
                    }
                }
            });
        }
    };
    $scope.Delete = function () {
        $scope.listDelete = [];
        angular.forEach($scope.DSVatTuPhu, function (data) {
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
                            url: '/VatTuPhu/Delete',
                            data: { dt: $scope.listDelete },
                            success: function (data) {
                                toastr.warning(data.notifyTitle);
                                $scope.pageChanged();
                                GetDSVatTuPhu($scope.tnsp[0]);
                                $scope.$apply();
                                $scope.listDelete = [];
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
    $scope.LoadPage = function () {
        $.ajax({
            type: 'POST',
            url: '/VatTuPhu/GetNhomVTP',
            data: $scope.nhomVTPSearch,
            success: function (data) {
                $scope.DSNhomVTP = data.data;
                $scope.$apply();
            }
        });
    }
    $scope.VTPSelect = {};
    $scope.SelectNhomVTP = function (index, item) {
        $scope.selectedRowNhomVTP = index;
        $scope.ShowContent = true;
        $scope.tnsp = [{ ID: item.ID, TEN_NHOM_SP: item.TEN_NHOM_SP }];
        GetDSVatTuPhu(item);
        GetButtonAction();

    }
    function GetDSVatTuPhu(item) {
        $scope.SELECTALL = false;
        $.ajax({
            type: 'POST',
            url: '/VatTuPhu/GetDSVatTuPhu',
            data: { index: item.ID, ds: $scope.modelSearch },
            success: function (data) {
                $scope.modelSearch.totalItems = data.totalItems;
                $scope.DSVatTuPhu = data.data;
                $scope.DSVatTuPhu.Edit = false;
                angular.forEach($scope.DSVatTuPhu, function (obj) {
                    obj["EditMode"] = false;
                })
                $scope.modelSearch.pageSize = data.pageSize;
                $scope.$apply();
            }
        });
    }
})