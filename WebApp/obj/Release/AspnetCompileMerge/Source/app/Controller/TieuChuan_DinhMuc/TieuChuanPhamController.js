app.controller("TieuChuanPhamController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {

    $scope.index = -1;
    $scope.isAllSelected = false;
    $scope.dsNhapKho = [];
    $scope.dsGoc = [];
    $scope.namht = new Date().getFullYear();
    $scope.nambh = [
        { ID: $scope.namht, NAME: $scope.namht },
        { ID: $scope.namht - 1, NAME: $scope.namht - 1 },
        { ID: $scope.namht - 2, NAME: $scope.namht - 2 },
        { ID: $scope.namht - 3, NAME: $scope.namht - 3 },
        { ID: $scope.namht - 4, NAME: $scope.namht - 4 },
        { ID: $scope.namht - 5, NAME: $scope.namht - 5 }
    ]
    $scope.modelSearch = {
        NHOM_TIEU_CHUAN_ID: '',
        LOAI_PHAM_ID: '',
        NAM: $scope.namht,
        SP_ID: '',
        SO_LUONG_TC: 0,
        NIEN_HAN: 0,
        OrderByClause: '',
        maxSize: 5,
        pageSize: 10,
        currentPage: 1,
        totalItems: 0
    }

    $scope.them = [{
        ID: 0,
        NHOM_TIEU_CHUAN_ID: $scope.modelSearch.NHOM_TIEU_CHUAN_ID,
        LOAI_PHAM_ID: '',
        NAM: $scope.modelSearch.NAM,
        SP_ID: '',
        SO_LUONG_TC: 0,
        NIEN_HAN: 0,
        CHECKED: false
    }]

    
    $scope.model = {
        SO_KE_HOACH: '',
        SO_KE_HOACH_GAN: '',
        NGUON_TIEP_NHAN_ID: '',
        NGUOI_NHAP_NHAN_ID: '',
        NGUOI_NHAP_NHAN: '',
        NGAY_NHAP_NHAN: '',
        DON_VI_NHAP_NHAN_ID: '',
        DON_VI_NHAP_NHAN: '',
        LOAI: 1,
        GHI_CHU: ''
    }

   
   
    angular.element(document).ready(function () {
        GetBottomAction();
        $.ajax({
            type: 'post',
            url: '/TieuChuanPham/GetDanhMuc',
            data: {},
            success: function (response) {
                console.log(response);
                $scope.loaipham = response.loaipham;
                $scope.nhomtc = response.nhomTC;
                $scope.sanpham = response.dsHang;

                if ($scope.nhomtc.length > 0) {
                    $scope.modelSearch.NHOM_TIEU_CHUAN_ID = $scope.nhomtc[0].ID_NHOM;
                    nhapkho();
                }
                $scope.$apply();
            }
        });
    });

    $scope.RoleBtnCreate = false;
    $scope.RoleBtnSave = false;
    $scope.RoleBtnSearch = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnView = false;
    $scope.RoleBtnRollback = false;
    function GetBottomAction() {
        $.ajax({
            type: 'post',
            url: '/TieuChuanCBCSPham/GetBottomAction',
            data: {},
            success: function (response) {
                if (response.Buttoms != null) {
                    angular.forEach(response.Buttoms, function (item) {
                        if (item == 'btnCreate') {
                            $scope.RoleBtnCreate = true;
                        }
                        if (item == 'btnSave') {
                            $scope.RoleBtnSave = true;
                        }
                        if (item == 'btnDelete') {
                            $scope.RoleBtnDelete = true;
                        }
                        if (item == 'btnView') {
                            $scope.RoleBtnView = true;
                        }
                        if (item == 'btnRollback') {
                            $scope.RoleBtnRollback = true;
                        }
                    });
                }
                $scope.$apply();
            }
        });
    }

    
    $scope.TimKiem = function () {
        nhapkho();
    }
    $scope.pageChanged = function () {
        nhapkho();
    };
    function nhapkho() {
        $scope.listXoa = [];
        showToast();
        $scope.index = -1;
        $scope.dsNhapKho = [];
        $scope.dsGoc = [];
        $scope.stt = 0;
        $.ajax({
            type: 'post',
            url: '/TieuChuanPham/dsNhapKho',
            data: $scope.modelSearch,
            success: function (data) {
                if (data.Error) {
                    toastr.error(data.Title);
                } else {
                    $scope.dsNhapKho = data.data;
                    //console.log($scope.dsNhapKho);
                    //if (data.data.length > 0) {
                    //    $scope.modelSearch.totalItems = data.data[0].TotalRow;
                    //}
                }
                
                $scope.$apply();
                hideLoading();
            }
        });

    };

    $scope.changeNhomTC = function (id) {
        nhapkho();
    }

    $scope.changeNam = function (hang) {
        nhapkho();
    }
 
    $scope.Them = function () {
        
        $scope.them = [{
            ID: 0,
            NHOM_TIEU_CHUAN_ID: $scope.modelSearch.NHOM_TIEU_CHUAN_ID,
            LOAI_PHAM_ID: '',
            NAM: $scope.modelSearch.NAM,
            SP_ID: '',
            SO_LUONG_TC: 0,
            NIEN_HAN: 0,
            CHECKED: false
        }]
       
        $scope.dsNhapKho = [...$scope.dsNhapKho, ...$scope.them];
        //$scope.index = $scope.index - 1;
        //console.log($scope.dsNhapKho);
    }

    $scope.Huy = function () {
        nhapkho();
    }
    $scope.Luu = function () {
        $scope.listNhap = [];
        angular.forEach($scope.dsNhapKho, function (data) {
            //if (data.CHECKED == true) {
                $scope.listNhap.push(data);
            //}
        })

        $.ajax({
            type: 'post',
            url: '/TieuChuanPham/themMoi',
            data: {
                hang: $scope.listNhap,
                xoa: $scope.listXoa

            },
            success: function (data) {
               
                if (data.Error) {
                    toastr.success('Thêm mới thành công.');
                    nhapkho();
                    
                } else {
                    toastr.error('Thêm mới thất bại.');

                }
            
                hideLoading();
                $scope.$apply();
            }
        });
    }

   
    $scope.listXoa = [];
    $scope.Xoa = function () {
       
        //angular.forEach($scope.dsNhapKho, function (data) {
        //    if (data.CHECKED == true) {
        //        $scope.listXoa.push(data);
        //    }
        //})
        //console.log($scope.listXoa);
        //if ($scope.dsNhapKho.length == 0) {
        //    toastr.error('Chưa chọn hồ sơ nào để xóa.');
        //    return;
        //}
        //$ngConfirm({
        //    title: 'Thông báo',
        //    content: 'Bạn có chắc chắn muốn xóa bản ghi đã chọn không?',
        //    scope: $scope,
        //    buttons: {
        //        delete: {
        //            text: 'Xóa',
        //            btnClass: 'btn-blue',
        //            action: function (scope, button) {
        //                //angular.forEach($scope.dsNhapKho, function (data) {
        //                //    if (data.CHECKED == true) {
        //                //        $scope.listXoa.push(data);
        //                //    }
        //                //})
        //                showToast();
        //                $.ajax({
        //                    type: 'post',
        //                    url: '/TieuChuanPham/xoa',
        //                    data: { hang: $scope.listXoa },
        //                    success: function (data) {
        //                        if (data.Error) {
        //                            toastr.error(data.Title);
        //                        } else {
        //                            nhapkho();
        //                            toastr.success(data.Title);

        //                        }
        //                        hideLoading();
        //                    }
        //                });
        //            }
        //        },
        //        close: {
        //            text: 'Hủy',
        //            action: function (scope, button) {

        //            }
        //        }
        //    }
        //});

        var countDelete = 0;
        for (var i = 0; i < $scope.dsNhapKho.length; i++) {
            if ($scope.dsNhapKho[i].CHECKED == true) {
                countDelete++;
            }
           
        }

        if (countDelete != null && countDelete > 0) {
            $ngConfirm({
                title: 'Thông báo',
                content: 'Bạn có chắc chắn muốn xóa sản phẩm đã chọn?',
                scope: $scope,
                buttons: {
                    delete: {
                        text: 'Có',
                        btnClass: 'btn-blue',
                        action: function (scope, button) {
                            for (var i = 0; i < $scope.dsNhapKho.length; i++) {
                                if ($scope.dsNhapKho[i].CHECKED == true) {
                                    if ($scope.dsNhapKho[i].ID > 0) {
                                        $scope.listXoa.push($scope.dsNhapKho[i]);
                                    }
                                    
                                    $scope.dsNhapKho.splice(i, 1);
                                    i--;
                                }

                            }
                            
                            $scope.$apply();
                        }
                    },
                    close: {
                        text: 'Không',
                        action: function (scope, button) {

                        }
                    }
                }
            });
        }
        else {
            toastr.error("Chưa có sản phẩm nào được chọn!");
        }
    };


    $scope.toggleAll = function () {
       
        var toggleStatus = $scope.isAllSelected;
        angular.forEach($scope.dsNhapKho, function (itm) { itm.CHECKED = !toggleStatus; });
        console.log($scope.dsNhapKho);
    }
    $scope.optionToggled = function () {
        $scope.isAllSelected = $scope.dsNhapKho.every(function (itm) { return itm.CHECKED; })
     
    }

    //format String DD/MM/YYYY to YYYYMMDD
    function DateString(date) {
        if (date != undefined && date != "") {
            date = date.substring(6, 10) + date.substring(0, 2) + date.substring(3, 5);
        }
        return date;
    }

    //format String YYYYMMDD to DD/MM/YYYY
    function StringDate(date) {
        if (date != undefined && date != "") {
            date = date.substring(4, 6) + '/' + date.substring(6, 8) + '/' + date.substring(0, 4);
        }
        return date;
    }
});

