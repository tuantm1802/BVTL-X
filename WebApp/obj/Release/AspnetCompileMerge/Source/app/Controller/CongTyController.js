app.controller("CongTyController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $location, $rootScope) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    //$scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.search = '';
    $scope.DanhSach = [];
    $scope.rows = [];
    $scope.listDonVi = [];
    $scope.ItemShow = false;
    $scope.search = [];
    $scope.ckc = false;
    $scope.unit_id = 0;
    $scope.HieuLuc = false;
    angular.element(document).ready(function () {
        $scope.GetPhongQL();
        //$scope.LoadPage();
        $scope.Search();
        GetButtonAction();
        GetCTY_CH();
    });

    //Phân quyền các nút chắc năng
    $scope.RoleBtnCreate = false;
    $scope.RoleBtnSave = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnCancel = false;
    $scope.RoleBtnView = false;
    function GetButtonAction() {
        $.ajax({
            type: 'post',
            url: '/CongTy/GetButtonAction',
            data: {},
            success: function (response) {
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
                        if (item == 'btnView') {
                            $scope.RoleBtnView = true;
                        }
                    });
                }
                $scope.$apply();
            }
        });
    }
    function GetCTY_CH() {
        $scope.ds_chua_sd = [];
        $scope.ds_dang_ssd = [];
        $.ajax({
            type: 'post',
            url: '/CongTy/GetChauHinh',
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
    $scope.btnCauHinh = function () {
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
                url: '/CongTy/ChauHinh',
                data: {
                    ds: ds,
                    is_sd: is_sd
                },
                success: function (data) {
                    if (data.Error) {
                        toastr.error(data.Title);
                    } else {
                        toastr.success('Cấu hình thành công');
                        GetCTY_CH();
                        $scope.Search();
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
    $scope.getCongTy = function () {
        $scope.Search();
    }
    $scope.isAllSelected = false;

    $scope.toggleAll = function () {

        var toggleStatus = $scope.isAllSelected;
        angular.forEach($scope.DanhSach, function (itm) {
            if (itm.MNG_UNIT_ID == $scope.unit_id) {
                itm.CHECKED = !toggleStatus;
            }

        });

    }
    $scope.optionToggled = function () {
        var list = $scope.DanhSach.filter(x => x.MNG_UNIT_ID == $scope.unit_id);
        $scope.isAllSelected = list.every(function (itm) { return itm.CHECKED; })

    }
    $scope.Search = function () {
        $scope.DanhSach = [];

        $.ajax({
            type: 'POST',
            url: '/CongTy/Search',
            data: {
                page: $scope.modelSearch.currentPage,
                kyword: $scope.modelSearch.search,
                pageSize: $scope.modelSearch.pageSize
            },
            success: function (data) {
                if (data.Error == false) {
                    if (data.data != null && data.data.length > 0) {
                        $scope.modelSearch.totalItems = data.data[0].TOTAL_ROW;
                        $scope.DanhSach = data.data;
                        //$scope.modelSearch.pageSize = data.data[0].PAGE_SIZE;
                    } else {
                        $scope.modelSearch.totalItems = 0;
                        //$scope.modelSearch.pageSize = 10;
                    }
                    if (data.user == 1) {
                        $scope.HieuLuc = true;
                    }
                } else {
                    $scope.modelSearch.totalItems = 0;
                    //$scope.modelSearch.pageSize = 10;
                }

                $scope.$apply();
            }
        });
    }
    $scope.loadTemplate = function (index, item) {
        if ($scope.HieuLuc == true) {
            $scope.DanhSach[index].Edit = true;
        }

    }
    $scope.pageChanged = function () {
        $scope.Search();
    };
    $scope.GetPhongQL = function () {
        $.ajax({
            type: 'POST',
            url: '/CongTy/GetPhongQL',
            data: {},
            success: function (data) {
                $scope.listDonVi = data.data;
                $scope.unit_id = data.user.UnitId;
                $scope.$apply();
            }
        });
    };
    $scope.LoadPage = function () {
        $.ajax({
            type: 'post',
            url: '/CongTy/GetAllData',
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
        $scope.rows.push({ ID: 0, TEN_CONG_TY: '', TEN_VIET_TAT: '', MO_TA: '', DIA_CHI: '', DV_KHONG_DUNG: 'Y', HIEU_LUC: 'Y' });
    };
    $scope.Undo = function () {
        $scope.rows = [];
        $scope.Search();

    }

    $scope.changeCty = function (index) {
        $scope.DanhSach[index].CHANGE = true;
    }
    $scope.Submit = function () {
        $scope.addCty = [];
        $scope.isTenVT = true;
        $scope.isTen = true;
        angular.forEach($scope.rows, function (data) {
            if (data.TEN_VIET_TAT == null || data.TEN_VIET_TAT == "" || data.TEN_VIET_TAT == undefined) {
                $scope.isTenVT = false;
            }
            if (data.TEN_CONG_TY == null || data.TEN_CONG_TY == "") {
                $scope.isTen = false;
            }
            $scope.addCty.push(data);

        })

        angular.forEach($scope.DanhSach, function (data) {
            if (data.CHANGE == true) {
                if (data.TEN_VIET_TAT == null || data.TEN_VIET_TAT == "" || data.TEN_VIET_TAT == undefined) {
                    $scope.isTenVT = false;
                }
                if (data.TEN_CONG_TY == null || data.TEN_CONG_TY == "") {
                    $scope.isTen = false;
                }
                $scope.addCty.push(data);

            }

        })

        if ($scope.isTenVT == false) {
            toastr.error("Phải nhập Tên viết tắt công ty và là tiếng việt không dấu.");
            return;
        }
        if ($scope.isTen == false) {
            toastr.error("Chưa nhập Tên công ty.");
            return;
        }

        if ($scope.addCty != [] && $scope.addCty.length > 0) {

            $.ajax({
                type: 'post',
                url: '/CongTy/Add',
                data: { DMCongTy: $scope.addCty },
                success: function (data) {
                    console.log(data);
                    if (data.Error) {
                        toastr.error(data.Title, 'Thêm mới dữ liệu thất bại');
                    } else {
                        toastr.success(data.Title, 'Thêm mới dữ liệu thành công');
                        $scope.Search();
                        $scope.rows = [];
                    }
                    hideLoading();
                }
            })

        }
    };

    $scope.listDelete = [];
    $scope.Delete = function () {
        $scope.listDelete = [];
        angular.forEach($scope.DanhSach, function (data) {
            if (data.CHECKED == true) {
                $scope.listDelete.push(data);
            }
        });
        if ($scope.listDelete == [] || $scope.listDelete.length == 0) {
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
                            url: '/CongTy/Delete',
                            data: { dt: $scope.listDelete },
                            success: function (data) {
                                if (data.Error) {
                                    console.log(data);
                                    toastr.error(data.Title);
                                } else {
                                    toastr.success('Xóa dữ liệu thành công');
                                    $scope.isAllSelected = false;
                                    $scope.Search();

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
});