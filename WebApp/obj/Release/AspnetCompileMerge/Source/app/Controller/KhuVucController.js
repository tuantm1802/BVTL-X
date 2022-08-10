app.controller("KhuVucController", function ($scope, $ngConfirm, hideLoading, $rootScope) {
    //#region Khởi tạo giáo trị
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;

    $scope.listKhuVucPB = [];
    $scope.listDelete = [];
    $scope.DanhSach = [];
    $scope.rows = [];

    $rootScope.TenKhuVuc = true;
    $scope.RoleBtnCreate = false;
    $scope.RoleBtnSave = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnCancel = false;
    $scope.isDisable = true;
    $rootScope.MoTa = true;
    $rootScope.STT = true;
    $scope.ckc = false;

    angular.element(document).ready(function () {
        $scope.LoadPage();
        GetButtonAction();
        $scope.listAllKhuVuc();
    });

    $scope.LoadPage = function () {
        $.ajax({
            type: 'post',
            url: '/KhuVuc/GetListKhuVucTheoUnitID',
            data: $scope.modelSearch,
            success: function (data) {
                $scope.modelSearch.totalItems = data.totalItems;
                $scope.unitID = data.unitID;
                $scope.DanhSach.Edit = false;
                $scope.DanhSach = data.data;
                angular.forEach($scope.DanhSach, function (obj) {
                    obj["EditMode"] = false;
                })
                $scope.modelSearch.pageSize = data.pageSize;
                $scope.$apply();
            }
        });
    };

    //$scope.listAllKhuVuc = function () {
    //    $.ajax({
    //        type: 'get',
    //        url: '/KhuVuc/listAllKhuVuc',
    //        data: {},
    //        success: function (data) {
    //            $scope.listAllKhuVuc = data.listAllKhuVuc;
    //            $scope.$apply();
    //        }
    //    });
    //};

    //Phân quyền các nút chắc năng

    function GetButtonAction() {
        $.ajax({
            type: 'post',
            url: '/KhuVuc/GetButtonAction',
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


    //#endregion Khởi tạo giáo trị

    //#region Xử lý button

    //Thêm mới
    $scope.Add = () => {
        $scope.rows.push({ TEN_KHU_VUC: '', MO_TA: '' });
    }
    //Lưu trữ khi p1 đăng nhập
    $scope.Submit = function () {
        if ($scope.rows != "") {
            angular.forEach($scope.rows, function (data) {
                if (data.TEN_KHU_VUC == null || data.TEN_KHU_VUC == "") {
                    toastr.error("Bạn chưa nhập tên khu vực");
                    $scope.ckc = true;
                    return;
                }
            })
            if ($scope.ckc == false) {
                $.ajax({
                    type: 'post',
                    url: '/KhuVuc/Add',
                    data: { dt: $scope.rows, unitID: $scope.unitID },
                    success: function (data) {
                        if (data.Error) {
                            toastr.error(data.Title, 'Thêm mới dữ liệu thất bại');
                        } else {
                            toastr.success(data.Title, 'Thêm mới dữ liệu thành công');
                            $scope.LoadPage();
                            $scope.rows = [];
                        }
                        hideLoading();
                    }
                })
            }
            $scope.ckc = false;
        } else {
            $.ajax({
                type: 'post',
                url: '/KhuVuc/Edit',
                data: { ds: $scope.DanhSach },
                success: function (data) {
                    if (data.Error) {
                        toastr.error(data.Title, 'Thay đổi dữ liệu thất bại');
                    } else {
                        toastr.success(data.Title);
                        $scope.LoadPage();
                    }
                    hideLoading();
                }
            });
        }
    };
    //Lưu trữ các phòng ban khi chọn khu vực
    //$scope.SubmitPhongBan = function () {
    //    angular.forEach($scope.listAllKhuVuc, function (data) {
    //        if (data.Selected == true) {
    //            $scope.listKhuVucPB.push(data);
    //        }
    //    })
    //    $.ajax({
    //        type: 'post',
    //        url: '/KhuVuc/EditPhongBan',
    //        data: { editPB: $scope.listKhuVucPB, unitID: $scope.unitID },
    //        success: function (data) {
    //            if (data.Error) {
    //                toastr.error(data.Title, 'Thay đổi dữ liệu thất bại');
    //            } else {
    //                toastr.success(data.Title);
    //                $('#list_khu_vuc').modal('hide');
    //                $scope.listKhuVucPB = [];
    //                $scope.pageChanged();
    //            }
    //            hideLoading();
    //        }
    //    });

    //};
    //Hủy thay đổi
    $scope.Undo = function () {
        $scope.rows = [];
        $scope.LoadPage();
    };
    //Xóa dữ liệu
    $scope.Delete = function () {
        angular.forEach($scope.DanhSach, function (data) {
            if (data.select == true) {
                $scope.listDelete.push(data);
            }
        })
        if ($scope.listDelete.length == 0) {
            toastr.error('Chưa chọn mục nào để xóa.');
            return;
        }
        $ngConfirm({
            title: 'Thông báo',
            content: 'Bạn có chắc chắn muốn xóa dữ liệu đã chọn không?',
            scope: $scope,
            buttons: {
                delete: {
                    text: 'Xóa',
                    btnClass: 'btn-red',
                    action: function () {
                        $.ajax({
                            type: 'post',
                            url: '/KhuVuc/Delete',
                            data: { dt: $scope.listDelete, unitID: $scope.unitID },
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
                    action: function () {
                    }
                }
            }
        });
        //if ($scope.unitID == 1) {
        //    $ngConfirm({
        //        title: 'Thông báo',
        //        content: 'Bạn có chắc chắn muốn xóa dữ liệu đã chọn không?',
        //        scope: $scope,
        //        buttons: {
        //            delete: {
        //                text: 'Xóa',
        //                btnClass: 'btn-red',
        //                action: function () {
        //                    $.ajax({
        //                        type: 'post',
        //                        url: '/KhuVuc/Delete',
        //                        data: { dt: $scope.listDelete },
        //                        success: function (data) {
        //                            toastr.warning(data.notifyTitle);
        //                            $scope.LoadPage();
        //                            $scope.listDelete = [];
        //                            $scope.selectAll = false;
        //                        }
        //                    });
        //                }
        //            },
        //            close: {
        //                text: 'Hủy',
        //                btnClass: 'btn-green',
        //                action: function () {
        //                }
        //            }
        //        }
        //    });
        //} else {
        //    $ngConfirm({
        //        title: 'Thông báo',
        //        content: 'Bạn có chắc chắn muốn xóa dữ liệu đã chọn không?',
        //        scope: $scope,
        //        buttons: {
        //            delete: {
        //                text: 'Xóa',
        //                btnClass: 'btn-red',
        //                action: function () {
        //                    $.ajax({
        //                        type: 'post',
        //                        url: '/KhuVuc/DeletePhongBan',
        //                        data: { delPhongBan: $scope.listDelete, unitID: $scope.unitID },
        //                        success: function () {
        //                            $scope.LoadPage();
        //                            $scope.listDelete = [];
        //                            $scope.selectAll = false;
        //                        }
        //                    });
        //                }
        //            },
        //            close: {
        //                text: 'Hủy',
        //                btnClass: 'btn-green',
        //                action: function () {
        //                }
        //            }
        //        }
        //    });
        //}

    };
    ////Đóng modal
    //$scope.cancel = function () {
    //    $('#list_khu_vuc').modal('hide');
    //};

    //#endregion Xử lý button

    //#region Xử lý tiện ích

    ////Check all trên modal
    //$scope.selectAllModal = false;
    //$scope.checkAllModal = () => {
    //    angular.forEach($scope.listAllKhuVuc, function (obj) {
    //        obj["Selected"] = !$scope.selectAllModal;
    //        if (obj["Selected"] === true) obj["EditMode"] = true;
    //        else obj["EditMode"] = false;
    //    })
    //}

    //Check all trên bảng
    $scope.selectAll = false;
    $scope.checkAll = () => {
        angular.forEach($scope.DanhSach, function (obj) {
            obj["select"] = !$scope.selectAll;
            if (obj["select"] === true) obj["EditMode"] = true;
            else obj["EditMode"] = false;
        })
    }

    $scope.loadTemplate = function (index) {
        if ($scope.RoleBtnSave)
            $scope.DanhSach[index].Edit = true;
    };
    $scope.pageChanged = function () {
        $scope.LoadPage();
    };
    //#endregion Xử lý tiện ích
});