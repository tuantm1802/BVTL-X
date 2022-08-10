app.controller("DonViTinhController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $rootScope) {

    $scope.user = {};
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.TEN_DVT = '';
    $scope.ds_chua_sd = [];
    $scope.ds_dang_ssd = [];
    $scope.DanhSach = [];
    $scope.ItemShow = false;
    $scope.rows = [];
    $scope.search = [];
    $scope.sea = [];
    $scope.ck = false;
    $scope.ckc = false;

    angular.element(document).ready(function () {
        $scope.LoadPage();
        GetButtonAction();
        GetDVT_CH();
    });

    //Phân quyền các nút chắc năng
    $scope.RoleBtnCreate = false;
    $scope.RoleBtnSave = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnCancel = false;
    $scope.RoleBtnCauHinh = false;
    function GetButtonAction() {
        $.ajax({
            type: 'post',
            url: '/DonViTinh/GetButtonAction',
            data: {},
            success: function (response) {
                $scope.user = response.user;
                if (response.buttons != null) {
                    angular.forEach(response.buttons, function (item) {
                        if (item == 'btnCreate') {
                            $scope.RoleBtnCreate = true;
                        }
                        if (item == 'BtnCauHinh') {
                            $scope.RoleBtnCauHinh = true;
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

    function GetDVT_CH() {
        $scope.ds_chua_sd = [];
        $scope.ds_dang_ssd = [];
        $.ajax({
            type: 'post',
            url: '/DonViTinh/GetChauHinh',
            data: { search: $scope.KeywordCH },
            success: function (response) {
                if (response.Error == false) {
                    $scope.ds_chua_sd = response.ds_chua_sd;
                    $scope.ds_dang_ssd = response.ds_da_sd;
                }
                $scope.$apply();
            }
        });
    }

    $scope.searchDVT = function () {

        $scope.LoadPage();
    }
    $scope.searchitem = function () {
        $scope.ckc = true;

        $.ajax({
            type: 'post',
            url: '/DonViTinh/Search',
            data: { modelSearch: $scope.modelSearch },
            success: function (data) {

                $scope.DanhSach.Edit = false;
                $scope.DanhSach = data.data;
                if (data.data.length > 0) {
                    angular.forEach($scope.DanhSach, function (obj) {
                        obj["EditMode"] = false;
                    })
                    $scope.modelSearch.totalItems = data.data[0].currentPage;
                    //$scope.modelSearch.pageSize = data.data[0].pageSize;
                  
                }

                $scope.$apply();
            }
        });
    }

    $scope.selectAll = false;
    $scope.checkAll = () => {
        angular.forEach($scope.DanhSach, function (obj) {
            obj["select"] = !$scope.selectAll;
            if (obj["select"] === true) obj["EditMode"] = true;
            else obj["EditMode"] = false;
        })
    }

    $scope.Addmm = function () {
        $scope.rows.push({ ID: 0, TEN_DVT: '', MO_TA: '' });
    };

    $scope.Hiden = function () {
        $scope.rows = [];
        $scope.LoadPage();
    };



    $scope.LoadPage = function () {

        $.ajax({
            type: 'post',
            url: '/DonViTinh/Search',
            data: { modelSearch: $scope.modelSearch },
            success: function (data) {

                $scope.DanhSach.Edit = false;
                $scope.DanhSach = data.data;
                if (data.data.length > 0) {
                    angular.forEach($scope.DanhSach, function (obj) {
                        obj["EditMode"] = false;
                    })
                    $scope.modelSearch.totalItems = data.data[0].currentPage;
                    //$scope.modelSearch.pageSize = data.data[0].pageSize;
                  
                }

                $scope.$apply();
            }
        });
    };

    $scope.pageChanged = function () {
        if ($scope.ckc == false) {
            $scope.LoadPage();
        } else $scope.searchitem();
    };

    $scope.loadTemplate = function (index) {
        if ($scope.RoleBtnSave && $scope.user.UnitId == 1)
            $scope.DanhSach[index].Edit = true;
    }

    $scope.changeEdit = function (index) {
        $scope.DanhSach[index].changeEdit = true;
    }

    $scope.submit = function () {
        var check = true;
        var ds = [];
        angular.forEach($scope.rows, function (data) {
            ds.push(data);
            if (data.TEN_DVT == null || data.TEN_DVT == "") {
                check = false;
            }
        })
        angular.forEach($scope.DanhSach, function (data) {
            if (data.changeEdit == true) {
                ds.push(data);
                if (data.TEN_DVT == null || data.TEN_DVT == "") {
                    check = false;
                }
            }

        })

        if (check == false) {
            toastr.error("Bạn chưa nhập tên đơn vị tính");
            return;
        }
        if (ds.length > 0) {
            $.ajax({
                type: 'post',
                url: '/DonViTinh/Them',
                data: { ds: ds },
                success: function (data) {
                    if (data.Error) {
                        toastr.error(data.Title);
                    } else {
                        toastr.success('Cập nhật dữ liệu thành công');
                        $scope.ItemShow = false;
                        $scope.LoadPage();
                        $scope.rows = [];
                    }
                    hideLoading();
                }
            })
        }
        //   else {
        //    toastr.error("Bạn chưa nhập tên đơn vị tính");
        //}


        //} else {
        //    $.ajax({
        //        type: 'post',
        //        url: '/DonViTinh/edit',
        //        data: { ds: $scope.DanhSach },
        //        success: function (data) {
        //            if (data.Error) {
        //                toastr.error(data.Title, 'Thay đổi dữ liệu thất bại');
        //            } else {
        //                toastr.success(data.Title, 'Thay đổi dữ liệu thành công');
        //                $scope.LoadPage();
        //            }
        //            hideLoading();
        //        }
        //    });
        //}
    };

    $scope.btnCauHinh = function () {
        GetDVT_CH();
        $scope.KeywordCH = '';
        $('#modal_CH').modal('show');

    }
    function CauHinh(is_sd) {
        var ds = [];
        if (is_sd == 'Y') {
            angular.forEach($scope.ds_chua_sd, function (data) {
                if (data.SELECT == true) {
                    ds.push(data);
                }

            })
        }
        else {
            angular.forEach($scope.ds_dang_ssd, function (data) {
                if (data.SELECT == true) {
                    ds.push(data);
                }

            })
        }



        if (ds.length > 0) {
            $.ajax({
                type: 'post',
                url: '/DonViTinh/ChauHinh',
                data: {
                    ds: ds,
                    is_sd: is_sd
                },
                success: function (data) {
                    if (data.Error) {
                        toastr.error(data.Title);
                    } else {
                        toastr.success('Cấu hình thành công');
                        GetDVT_CH();
                        $scope.LoadPage();
                    }
                    hideLoading();
                }
            })
        }

    };

    $scope.CoSD = function () {
        CauHinh('Y')
    }
    $scope.KhongSD = function () {
        CauHinh('N')
    }
    $scope.SearchKeywordCH = function () {
        GetDVT_CH();
    }
    $scope.CheckAllChuaSD = function () {

        var toggleStatus = $scope.AllChuaSD;
        angular.forEach($scope.ds_chua_sd, function (itm) { itm.SELECT = toggleStatus; });

    }
    $scope.CheckChuaSD = function () {
        $scope.AllChuaSD = $scope.ds_chua_sd.every(function (itm) { return itm.SELECT; })

    }

    $scope.CheckAllDangSD = function () {

        var toggleStatus = $scope.AllDangSD;
        angular.forEach($scope.ds_dang_ssd, function (itm) { itm.SELECT = toggleStatus; });

    }
    $scope.CheckDangSD = function () {
        $scope.AllDangSD = $scope.ds_dang_ssd.every(function (itm) { return itm.SELECT; })

    }

    $scope.listXoa = [];
    $scope.delete = function () {
        angular.forEach($scope.DanhSach, function (data) {
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
            content: 'Bạn có chắc chắn muốn xóa dữ liệu đã chọn không?',
            scope: $scope,
            buttons: {
                delete: {
                    text: 'Xóa',
                    btnClass: 'btn-red',
                    action: function (scope, button) {
                        $.ajax({
                            type: 'post',
                            url: '/DonViTinh/delete',
                            data: { dt: $scope.listXoa },
                            success: function (data) {
                                toastr.warning(data.notifyTitle);
                                $scope.LoadPage();
                                $scope.selectAll = false;
                                $scope.listXoa = []
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

    $rootScope.stt = true;
    $rootScope.mdvt = true;
    $rootScope.dvt = true;
    $rootScope.Mota = true;
});