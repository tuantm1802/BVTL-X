app.controller("DanhMucDonViSuDungController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.pageSize = 10;
    $scope.isAllSelectedKH = false;
    $scope.UnitId = 0;
    $scope.dsNhapKho = [];
    $scope.title = '';
    $scope.dsKhoCha = [];
    $scope.rows = [];
    $scope.modelSearchKH = {
        DON_VI_CHA_ID: '',
        TEN_DON_VI: ''
    }
    $scope.ListNhomDonVi = [
        { ID: 'TT', TEN_NHOM_DV: 'CA cấp tỉnh, thành phố' },
        { ID: 'QH', TEN_NHOM_DV: 'CA cấp quận, huyện' },
        { ID: 'XP', TEN_NHOM_DV: 'CA xã, phường, đồn' }
    ]

   
    angular.element(document).ready(function () {
       
        $.ajax({
            type: 'post',
            url: '/DonViSuDung/GetDanhMuc',
            data: {},
            success: function (response) {
                $scope.listKho = response.lstKho;
                $scope.UnitId = response.user.UnitId;
                $scope.$apply();
            }
        });
        GetBottomAction();
    });
    $scope.RoleBtnCreate = false;
    $scope.RoleBtnSave = false;
    $scope.RoleBtnSearch = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnCancel = false;

    function GetBottomAction() {
        $.ajax({
            type: 'post',
            url: '/DonViSuDung/GetBottomAction',
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
                        if (item == 'btnSearch') {
                            $scope.RoleBtnSearch = true;
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
    $scope.changeEdit = function (index) {
        if ($scope.dsNhapKho[index].MNG_UNIT_ID == $scope.UnitId && $scope.dsNhapKho[index].IS_DON_VI_SU_DUNG == 'Y') {
            $scope.dsNhapKho[index].CHANGE = true;
        }
       
    }
   
    nhapkho();
    $scope.TimKiemKH = function () {
        nhapkho();
    }

    $scope.pageChanged = function () {
        nhapkho();
    };
    function nhapkho() {
      
        showToast();
        $scope.dsNhapKho = [];
        $scope.rows = [];
        
        $.ajax({
            type: 'post',
            url: '/DonViSuDung/dsNhapKho',
            data: { nhap: $scope.modelSearchKH, page: $scope.modelSearch.currentPage},
            success: function (data) {
                if (data.Error) {
                    toastr.error(data.Title);
                } else {
                    $scope.dsNhapKho = data.data;
                    if (data.data != null && data.data.length > 0) {
                        $scope.modelSearch.totalItems = data.data[0].TOTAL_ROW;
                        $scope.modelSearch.pageSize = data.data[0].PAGE_SIZE;
                    }
                    //var kho = data.data.filter(x => x.ID_KHO_CHINH == $scope.modelSearchSP.ID_KHO && x.ID_KHO_LIEN_THONG == $scope.modelSearchSP.ID_KHO_LT);
                    //if (kho == null && kho == undefined && kho.length == 0) {
                    //    $scope.modelSearchSP.ID_KHO == '';
                    //    $scope.modelSearchSP.ID_KHO_LT = '';
                    //    $scope.chonKho = -1;
                    //}
                }
                
               
                hideLoading();
                $scope.$apply();
            }
        });

    };

    $scope.loadTemplate = function (index) {
        //console.log($scope.UnitId);
        //console.log($scope.dsNhapKho[index].MNG_UNIT_ID);
        if ($scope.dsNhapKho[index].MNG_UNIT_ID == $scope.UnitId && $scope.dsNhapKho[index].IS_DON_VI_SU_DUNG == 'Y') {
            $scope.dsNhapKho[index].EDIT = true;
        }
       
    }

    $scope.changeKhoCha = function () {
        nhapkho();
    }

    $scope.ThemKH = function () {
        $scope.themKH = [{
            ID: 0,
            PHIEN_HIEU: '',
            TEN_DON_VI: '',
            KHU_VUC_ID: '',
            NHOM_DV_SU_DUNG: 'QH',
            DON_VI_CHA_ID: null,
            CONTACT_ADDRESS: '',
            MO_TA: '',
            HIEU_LUC: 'Y',
            DV_KHONG_DUNG: 'Y',
            CHECKEDKH: false
        }]
       
        $scope.rows = [...$scope.rows, ...$scope.themKH];
        //$scope.title = 'Thêm mới kho';
        //$scope.model = {
        //    PHIEN_HIEU: '',
        //    TEN_DON_VI: '',
        //    KHU_VUC_ID: '',
        //    NHOM_DON_VI_ID: '',
        //    CONTACT_NAME: '',
        //    CONTACT_ID: '',
        //    CONTACT_TEL: '',
        //    CONTACT_ADDRESS: '',
        //    DON_VI_CHA_ID: '',
        //    ID: 0,
        //    HIEU_LUC: '',
        //};
        //$('#AddKho').modal('show');
    }

    $scope.HuyKH = function () {
        nhapkho();
    }
    $scope.LuuKH = function () {
        $scope.listNhap = [];
        var ma = true;
        var ten = true;
        var kv = true;
        var nhom = true;
        var cha = true;
        angular.forEach($scope.dsNhapKho, function (data) {
            if (data.CHANGE == true) {
                $scope.listNhap.push(data);
                if (data.PHIEN_HIEU == '' || data.PHIEN_HIEU == null || data.PHIEN_HIEU == undefined) {
                    ma = false;
                }
                if (data.TEN_DON_VI == '' || data.TEN_DON_VI == null || data.TEN_DON_VI == undefined) {
                    ten = false;
                }

                if (data.DON_VI_CHA_ID == '' || data.DON_VI_CHA_ID == null || data.DON_VI_CHA_ID == undefined) {
                    kv = false;
                }
                //if (data.NHOM_DON_VI_ID == '' || data.NHOM_DON_VI_ID == null || data.NHOM_DON_VI_ID == undefined) {
                //    nhom = false;
                //}
                if (data.ID == data.DON_VI_CHA_ID ) {
                    cha = false;
                }
            }
           
        })

        angular.forEach($scope.rows, function (data) {
            
                $scope.listNhap.push(data);
                if (data.PHIEN_HIEU == '' || data.PHIEN_HIEU == null || data.PHIEN_HIEU == undefined) {
                    ma = false;
                }
                if (data.TEN_DON_VI == '' || data.TEN_DON_VI == null || data.TEN_DON_VI == undefined) {
                    ten = false;
                }

            if (data.DON_VI_CHA_ID == '' || data.DON_VI_CHA_ID == null || data.DON_VI_CHA_ID == undefined) {
                    kv = false;
                }
                //if (data.NHOM_DON_VI_ID == '' || data.NHOM_DON_VI_ID == null || data.NHOM_DON_VI_ID == undefined) {
                //    nhom = false;
                //}
                
        })

        if (ma == false) {
            toastr.error('Phải nhập Mã kho và Mã kho là tiếng việt không dấu.');
            return;
        }

        if (ten == false) {
            toastr.error('Phải nhập Tên kho.');
            return;
        }

        if (kv == false) {
            toastr.error('Phải nhập đơn vị cha.');
            return;
        }

        //if (nhom == false) {
        //    toastr.error('Phải nhập nhóm đơn vị.');
        //    return;
        //}
        if (cha == false) {
            toastr.error('Kho cha không được là chính nó.');
            return;
        }

        if ($scope.listNhap != null && $scope.listNhap.length == 0) {
            toastr.error('Chưa thêm mới hoặc sửa kho.');
            return;
        }
        $.ajax({
            type: 'post',
            url: '/DonViSuDung/themMoi',
            data: {
                hang: $scope.listNhap
            },
            success: function (data) {
               
                if (!data.Error) {
                    toastr.success(data.Title);
                    nhapkho();
                    
                } else {
                    toastr.error(data.Title);

                }
            
                hideLoading();
                $scope.$apply();
            }
        });
    }

    $scope.listXoa = [];
    $scope.XoaKH = function () {
        $scope.listXoa = [];
        var countDelete = 0;
        for (var i = 0; i < $scope.dsNhapKho.length; i++) {
            if ($scope.dsNhapKho[i].CHECKEDKH == true) {
                countDelete++;
                $scope.listXoa.push($scope.dsNhapKho[i]);
            }
           
        }
        for (var i = 0; i < $scope.rows.length; i++) {
            if ($scope.rows[i].CHECKEDKH == true) {
                $scope.rows.splice(i, 1);
                i--;
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
                            $.ajax({
                                type: 'post',
                                url: '/DonViSuDung/xoaKho',
                                data: {
                                    xoa: $scope.listXoa
                                },
                                success: function (data) {

                                    if (!data.Error) {
                                        toastr.success(data.Title);
                                        nhapkho();

                                    } else {
                                        toastr.error(data.Title);

                                    }

                                    hideLoading();
                                    $scope.$apply();
                                }
                            });
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

    $scope.toggleAllKH = function () {
       
        var toggleStatus = $scope.isAllSelectedKH;
        angular.forEach($scope.dsNhapKho, function (itm) {
            if (itm.MNG_UNIT_ID == $scope.UnitId && itm.IS_DON_VI_SU_DUNG == 'Y') {
                itm.CHECKEDKH = !toggleStatus;
            }
            
        });
        
    }
    $scope.optionToggledKH = function () {
        $scope.isAllSelectedKH = $scope.dsNhapKho.every(function (itm) { return itm.CHECKEDKH; })
     
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

