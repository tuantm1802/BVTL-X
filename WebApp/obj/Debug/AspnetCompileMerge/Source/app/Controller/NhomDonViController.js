app.controller("NhomDonViController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $rootScope) {
    //#region Khởi tạo giáo trị
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.GetNDV = [];
    $scope.listNhomDonViPB = [];
    $scope.ItemShow = false;
    $scope.rows = [];
    $scope.ckc = false

    angular.element(document).ready(function () {
        LoadPage();
        GetButtonAction();
        listAllNhomDonVi();
    });

    $scope.loaiNhom = [
        { text: "--Chọn nhóm đơn vị--" },
        { text: "CA Các tỉnh thành phố trực thuộc TW" },
        { text: "CA các đơn vị trực thuộc bộ" },
        { text: "Các học viện, trường CAND" },
        { text: "Các bệnh viện" },
        { text: "Các trại giam, trường giáo dưỡng trực thuộc bộ" }
    ];

    //Phân quyền các nút chức năng
    $scope.RoleBtnCreate = false;
    $scope.RoleBtnSave = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnCancel = false;
    $scope.isDisable = true;
    function GetButtonAction() {
        $.ajax({
            type: 'post',
            url: '/NhomDonVi/GetButtonAction',
            data: {},
            success: function (response) {
                if (response.buttons != null) {
                    angular.forEach(response.buttons, function (item) {
                        if (item == 'btnCreate') {
                            $scope.RoleBtnCreate = true;
                            $scope.isDisable = false;
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

    function LoadPage () {
        $.ajax({
            type: "post",
            url: "/NhomDonVi/GetNDV",
            data: $scope.modelSearch,
            success: function (data) {
                $scope.modelSearch.totalItems = data.totalItems;
                $scope.unitID = data.unitID;
                if ($scope.unitID !== 1) {
                    $("#changeByUnitID").text("Cấu hình");
                };
                $scope.GetNDV.Edit = false;
                $scope.GetNDV = data.data;
                angular.forEach($scope.GetNDV, function (obj) {
                    obj["EditMode"] = false;
                });
                //$scope.modelSearch.pageSize = data.pageSize;
                $scope.$apply();
            },
        });
    };
    
    function listAllNhomDonVi () {
        $.ajax({
            type: 'post',
            url: '/NhomDonVi/listAllNhomDonVi',
            data: {},
            success: function (data) {
                $scope.listAllNhomDonVi = data.listAllNhomDonVi;
                $scope.$apply();
            },
        });
    };
    //#endregion Khởi tạo giáo trị

    //#region Xử lý button

    //Thêm mới
    $scope.AddData = function () {
        if ($scope.unitID == 1) {
            $scope.rows.push({ TEN_NHOM_DV: '', MO_TA: '', LOAI_NHOM_DV_TEXT: $scope.loaiNhom[0].text });
        } else {
            switch ($scope.unitID) {
                case 3: {
                    $scope.TEN_PHONG_BAN = 'Quản lý phương tiện bộ và xăng dầu';
                    break;
                }
                case 4: {
                    $scope.TEN_PHONG_BAN = 'Quản lý phương tiện thủy';
                    break;
                }
                case 5: {
                    $scope.TEN_PHONG_BAN = 'Quản lý trang thiết bị, vật tư';
                    break;
                }
                case 6: {
                    $scope.TEN_PHONG_BAN = 'Quản lý vũ khí, vật liệu nổ';
                    break;
                }
                case 7: {
                    $scope.TEN_PHONG_BAN = 'Quản lý cấp phát quân trang';
                    break;
                }
                case 8: {
                    $scope.TEN_PHONG_BAN = 'Quản lý kho hàng';
                    break;
                }
                case 9: {
                    $scope.TEN_PHONG_BAN = 'Nghiệp vụ phòng vận tải';
                    break;
                }
                default: {
                    $scope.TEN_PHONG_BAN = 'Quản lý tài chính';
                }
            }
            $('#list_nhom_don_vi').modal('show');
        }

    };
    //Lưu trữ khi p1 đăng nhập
    $scope.Submit = function () {
        if ($scope.rows != "") {
            angular.forEach($scope.rows, function (data) {
                if (data.TEN_NHOM_DV == null || data.TEN_NHOM_DV == "") {
                    toastr.error("Bạn chưa nhập tên nhóm");
                    $scope.ckc = true;
                    return;
                }
                if (data.LOAI_NHOM_DV_TEXT == "--Chọn nhóm đơn vị--" ) {
                    toastr.error("Bạn chưa chọn loại nhóm");
                    $scope.ckc = true;
                    return;
                }
            })
            if ($scope.ckc == false) {
                $.ajax({
                    type: 'post',
                    url: '/NhomDonVi/Add',
                    data: { ds: $scope.rows },
                    success: function (data) {
                        if (data.Error) {
                            toastr.error(data.Title);
                        } else {
                            toastr.success(data.Title);
                            $scope.ItemShow = false;
                            $scope.rows = [];
                            $scope.pageChanged();
                        }
                        hideLoading();
                    }
                })
            }
            $scope.ckc = false;
        } else {
            $.ajax({
                type: 'post',
                url: '/NhomDonVi/Edit',
                data: { ds: $scope.GetNDV },
                success: function (data) {
                    if (data.Error) {
                        toastr.error(data.Title);
                    } else {
                        toastr.success(data.Title);
                        $scope.pageChanged();
                    }
                    hideLoading();
                }
            });
        }
    };

    //Lưu trữ các phòng ban khi chọn khu vực
    $scope.SubmitPhongBan = function () {
        $scope.listNhomDonViPB = [];
        angular.forEach($scope.listAllNhomDonVi, function (data) {
            if (data.Selected == true) {
                $scope.listNhomDonViPB.push(data);
            }
        })
        if ($scope.listNhomDonViPB.length == 0) {
            toastr.error("Chưa chọn nhóm đơn vị cấu hình");
            return;
        }
        $.ajax({
            type: 'post',
            url: '/NhomDonVi/EditPhongBan',
            data: { editPB: $scope.listNhomDonViPB, unitID: $scope.unitID },
            success: function (data) {
                if (data.Error) {
                    toastr.error('Cấu hình thất bại');
                } else {
                    toastr.success('Cấu hình thành công');
                    $scope.selectAllModal = false;
                    listAllNhomDonVi();
                    LoadPage();
                }
                hideLoading();
            }
        });
    }
    //Xóa dữ liệu
    $scope.listXoa = [];
    $scope.Delete = function () {
        angular.forEach($scope.GetNDV, function (data) {
            if (data.select == true) {
                $scope.listXoa.push(data);
            }
        });
        if ($scope.listXoa.length == 0) {
            toastr.error("Chưa chọn mục nào để xóa.");
            return;
        }
        $ngConfirm({
            title: "Thông báo",
            content: "Bạn có chắc chắn muốn xóa dữ liệu đã chọn không?",
            scope: $scope,
            buttons: {
                delete: {
                    text: "Xóa",
                    btnClass: "btn-red",
                    action: function (scope, button) {
                        $.ajax({
                            type: "post",
                            url: "/NhomDonVi/Delete",
                            data: { dt: $scope.listXoa, unitID: $scope.unitID },
                            success: function (data) {
                                toastr.warning(data.notifyTitle);
                                LoadPage();
                                listAllNhomDonVi();
                                $scope.listXoa = [];
                                $scope.selectAll = false;
                            },
                        });
                    },
                },
                close: {
                    text: "Hủy",
                    btnClass: "btn-green",
                    action: function (scope, button) { },
                },
            },
        });
    }
    //Xóa cấu hình các phòng ban
    $scope.listNhomDonViXoaCauHinh = [];
    $scope.DeletePhongBan = function () {
        angular.forEach($scope.GetNDV, function (data) {
            if (data.select == true) {
                $scope.listNhomDonViXoaCauHinh.push(data);
            }
        })
        if ($scope.listNhomDonViXoaCauHinh.length == 0) {
            toastr.error("Chưa chọn nhóm đơn vị để xóa cấu hình");
            return;
        }
        $.ajax({
            type: 'post',
            url: '/NhomDonVi/DeletePhongBan',
            data: { delPhongBan: $scope.listNhomDonViXoaCauHinh, unitID: $scope.unitID },
            success: function () {
                toastr.warning('Xóa cấu hình thành công');
                LoadPage();
                listAllNhomDonVi();
                $scope.selectAll = false;
            }
        });
    }

    //Huỷ thay đổi
    $scope.Cancel = function () {
        $scope.rows = [];
        $scope.pageChanged();
    };

    //Đóng modal
    $scope.cancel = function () {
        $('#list_nhom_don_vi').modal('hide');
        $scope.pageChanged();
    };

    //#endregion Xử lý button
    
    //#region Xử lý tiện ích

    //Check all trên bảng
    $scope.selectAll = false;
    $scope.checkAll = () => {
        angular.forEach($scope.GetNDV, function (obj) {
            obj["select"] = !$scope.selectAll;
            if (obj["select"] === true) obj["EditMode"] = true;
            else obj["EditMode"] = false;
        });
    };

    //Check all trên modal
    $scope.selectAllModal = false;
    $scope.checkAllModal = () => {
        angular.forEach($scope.listAllNhomDonVi, function (obj) {
            obj["Selected"] = !$scope.selectAllModal;
            if (obj["Selected"] === true) obj["EditMode"] = true;
            else obj["EditMode"] = false;
        })
    }   

    $scope.pageChanged = function () {
        LoadPage();
        listAllNhomDonVi();
    };

    $scope.loadTemplate = function (index) {
        if ($scope.RoleBtnSave)
            $scope.GetNDV[index].Edit = true;
    };
       

    $rootScope.stt = true;
    $rootScope.tenNhomDV = true;
    $rootScope.moTa = true;
    $rootScope.loaiNhom = true;

    //#endregion Xử lý tiện ích
});