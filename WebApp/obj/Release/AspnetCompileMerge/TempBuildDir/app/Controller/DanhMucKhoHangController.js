app.controller("DanhMucKhoHangController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {

    $scope.user = {};
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    //$scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.isAllSelectedKH = false;
    $scope.dsNhapKho = [];
    $scope.loaiDM = 'KH';
    $scope.title = '';
    $scope.dsKhoCha = [];
    $scope.rows = [];
    $scope.modelSearchKH = {
        DON_VI_CHA_ID: '',
        TEN_DON_VI: ''
    }

    $scope.listTrangThai = [
        { ID: 'A', TEN: 'Hoạt động bình thường' },
        { ID: 'L', TEN: 'Lock - Khóa' }
    ]

    $scope.listLoaiKho = [
        { ID: 'BT', TEN: 'Kho bình thường' },
        { ID: 'DP', TEN: 'Kho dự phòng chiến đấu' }
    ]

    $scope.khohang = function () {
        $scope.loaiDM = 'KH';
    }
    $scope.lienthong = function () {
        $scope.loaiDM = 'LT';
    }
    $scope.sanpham = function () {
        $scope.loaiDM = 'SP';
        nhapkhoLT();
    }
   
    angular.element(document).ready(function () {
       
        $.ajax({
            type: 'post',
            url: '/KhoHang/GetDanhMuc',
            data: {},
            success: function (response) {
                $scope.listKho = response.lstKho;
                $scope.ListSanPham = response.lstHang;
                $scope.$apply();
            }
        });

        GetAllKHuVuc();
        GetAllNhomDV();
        GetBottomAction();
        GetKho_CH()
        //khocha(0,0,0);
    });

    $scope.RoleBtnCauHinh = false;
    $scope.RoleBtnCreate = false;
    $scope.RoleBtnSave = false;
    $scope.RoleBtnSearch = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnCancel = false;

    //$scope.RoleBtnCreateLT = false;
    //$scope.RoleBtnSaveLT = false;
    //$scope.RoleBtnDeleteLT = false;
    //$scope.RoleBtnCancelLT = false;

    //$scope.RoleBtnCreateSP = false;
    //$scope.RoleBtnSaveSP = false;
    //$scope.RoleBtnSearchSP = false;
    //$scope.RoleBtnDeleteSP = false;
    //$scope.RoleBtnCancelSP = false;
    $scope.isDisable = true;
    function GetBottomAction() { 
        $.ajax({
            type: 'post',
            url: '/KhoHang/GetBottomAction',
            data: {},
            success: function (response) {
                $scope.user = response.user;
                if (response.Buttoms != null) {
                    angular.forEach(response.Buttoms, function (item) {
                        if (item == 'btnCreate') {
                            $scope.RoleBtnCreate = true;
                        }
                        if (item == 'BtnCauHinh') {
                            $scope.RoleBtnCauHinh = true;
                        }
                        if (item == 'btnSave') {
                            $scope.RoleBtnSave = true;
                            $scope.isDisable = false;
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


                        //if (item == 'btnCreateLT') {
                        //    $scope.RoleBtnCreateLT = true;
                        //}
                        //if (item == 'btnSaveLT') {
                        //    $scope.RoleBtnSaveLT = true;
                        //}
                        //if (item == 'btnDeleteLT') {
                        //    $scope.RoleBtnDeleteLT = true;
                        //}
                        //if (item == 'btnCancelLT') {
                        //    $scope.RoleBtnCancelLT = true;
                        //}

                        //if (item == 'btnCreateSP') {
                        //    $scope.RoleBtnCreateSP = true;
                        //}
                        //if (item == 'btnSaveSP') {
                        //    $scope.RoleBtnSaveSP = true;
                        //}
                        //if (item == 'btnSearchSP') {
                        //    $scope.RoleBtnSearchSP = true;
                        //}
                        //if (item == 'btnDeleteSP') {
                        //    $scope.RoleBtnDeleteSP = true;
                        //}
                        //if (item == 'btnCancelSP') {
                        //    $scope.RoleBtnCancelSP = true;
                        //}
                    });
                }
                //$scope.ListYear = response.ListYear;
                //$scope.NamBanHanh = response.ListYear[0].Value;
                $scope.$apply();
            }
        });
    }
    function GetAllKHuVuc() {
        $.ajax({
            type: 'post',
            url: '/DonVi/GetAllKHuVuc',
            data: {},
            success: function (data) {
                $scope.ListKhuVuc = data.ListKhuVuc;
                $scope.$apply();
            },
            error: function (xhr, status, error) {
                toastr.error("Lỗi lấy danh sách");
            }
        });
    }
    function GetAllNhomDV() {
        $.ajax({
            type: 'post',
            url: '/DonVi/GetAllNhomDonVi',
            data: {},
            success: function (data) {
                $scope.ListNhomDonVi = data.ListNhomDonVi;
                $scope.$apply();
            },
            error: function (xhr, status, error) {
                toastr.error("Lỗi lấy danh sách");
            }
        });
    }
    $scope.chengaKV = function (item, type) {
        khocha(item.ID, item.KHU_VUC_ID, item.NHOM_DON_VI_ID);
        if (type == 'EDIT') {
            item.CHANGE = true;
        }
    }

    $scope.changeEdit = function (index) {
        $scope.dsNhapKho[index].CHANGE = true;
    }
   
    nhapkho();
    $scope.TimKiemKH = function () {
        nhapkho();
    }

    //$scope.changeKH = function () {
    //    nhapkho();
    //}
    $scope.pageChanged = function () {
        nhapkho();
    };
    function nhapkho() {
      
        showToast();
        $scope.dsNhapKho = [];
        //$scope.rows = [];
        
        $.ajax({
            type: 'post',
            url: '/KhoHang/dsNhapKho',
            data: { nhap: $scope.modelSearchKH, page: $scope.modelSearch.currentPage, pageSize:$scope.modelSearch.pageSize},
            success: function (data) {
                if (data.Error) {
                    toastr.error(data.Title);
                } else {
                    $scope.dsNhapKho = data.data;
                    if (data.data != null && data.data.length > 0) {
                        $scope.modelSearch.totalItems = data.data[0].TOTAL_ROW;
                        //$scope.modelSearch.pageSize = data.data[0].PAGE_SIZE;
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


   
    function khocha(id, id_khuvuc, id_nhom) {
        $scope.dsKhoCha = [];
        $.ajax({
            type: 'post',
            url: '/KhoHang/dsKhoCha',
            data: {
                id: id,
                id_khu_vuc: id_khuvuc,
                id_nhom: id_nhom
            },
            success: function (data) {
                if (data.Error) {
                    toastr.error(data.Title);
                } else {
                    $scope.dsKhoCha = data.data;
                   
                }

                $scope.$apply();
            }
        });

    };


    $scope.loadTemplate = function (index) {
        if ($scope.RoleBtnSave && $scope.user.UnitId == 1)
            $scope.dsNhapKho[index].EDIT = true;
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
            NHOM_DON_VI_ID: '',
            LOAI_KHO: 'BT',
            CONTACT_ADDRESS: '',
            MO_TA: '',
            HIEU_LUC: 'Y',
            TRANG_THAI:'A',
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
        $scope.rows = [];
    }
    $scope.LuuKH = function () {
        $scope.listNhap = [];
        var ma = true;
        var ten = true;
        var kv = true;
        var nhom = true;
        //var cha = true;
        angular.forEach($scope.dsNhapKho, function (data) {
            if (data.CHANGE == true) {
                $scope.listNhap.push(data);
                if (data.PHIEN_HIEU == '' || data.PHIEN_HIEU == null || data.PHIEN_HIEU == undefined) {
                    ma = false;
                }
                if (data.TEN_DON_VI == '' || data.TEN_DON_VI == null || data.TEN_DON_VI == undefined) {
                    ten = false;
                }

                if (data.KHU_VUC_ID == '' || data.KHU_VUC_ID == null || data.KHU_VUC_ID == undefined) {
                    kv = false;
                }
                //if (data.NHOM_DON_VI_ID == '' || data.NHOM_DON_VI_ID == null || data.NHOM_DON_VI_ID == undefined) {
                //    nhom = false;
                //}
                //if (data.ID == data.DON_VI_CHA_ID ) {
                //    cha = false;
                //}
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

                if (data.KHU_VUC_ID == '' || data.KHU_VUC_ID == null || data.KHU_VUC_ID == undefined) {
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
            toastr.error('Phải nhập khu vực.');
            return;
        }

        //if (nhom == false) {
        //    toastr.error('Phải nhập nhóm đơn vị.');
        //    return;
        //}
        //if (cha == false) {
        //    toastr.error('Kho cha không được là chính nó.');
        //    return;
        //}

        if ($scope.listNhap != null && $scope.listNhap.length == 0) {
            toastr.error('Chưa thêm mới hoặc sửa kho.');
            return;
        }
        $.ajax({
            type: 'post',
            url: '/KhoHang/themMoi',
            data: {
                hang: $scope.listNhap
            },
            success: function (data) {
               
                if (!data.Error) {
                    toastr.success("Cập nhật thành công");
                    nhapkho();
                    $scope.rows = [];
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
                                url: '/KhoHang/xoaKho',
                                data: {
                                    xoa: $scope.listXoa
                                },
                                success: function (data) {

                                    if (!data.Error) {
                                        toastr.success("Xóa thành công");
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
        angular.forEach($scope.dsNhapKho, function (itm) { itm.CHECKEDKH = !toggleStatus; });
        
    }
    $scope.optionToggledKH = function () {
        $scope.isAllSelectedKH = $scope.dsNhapKho.every(function (itm) { return itm.CHECKEDKH; })
     
    }

    function GetKho_CH() {
        $scope.ds_chua_sd = [];
        $scope.ds_dang_ssd = [];
        $.ajax({
            type: 'post',
            url: '/KhoHang/GetChauHinh',
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
        GetKho_CH();
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
                url: '/KhoHang/ChauHinh',
                data: {
                    ds: ds,
                    is_sd: is_sd
                },
                success: function (data) {
                    if (data.Error) {
                        toastr.error(data.Title);
                    } else {
                        toastr.success('Cấu hình thành công');
                        GetKho_CH();
                        nhapkho();
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
        GetKho_CH();
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





    $scope.modelSearchLT = {};
    $scope.modelSearchLT.totalItems = 0;
    $scope.modelSearchLT.currentPage = 1;
    $scope.modelSearchLT.pageSize = 10;
    $scope.pageChangedLT = function () {
        nhapkhoLT();
    }
    nhapkhoLT();
    function nhapkhoLT() {
        $scope.listXoaLT = [];
        $scope.modelSearchLT.totalItems = 0;
        $scope.modelSearchLT.pageSize = 10;
        $scope.dsNhapKhoLT = [];

        $.ajax({
            type: 'post',
            url: '/KhoHang/dsKhoLienThong',
            data: { page: $scope.modelSearchLT.currentPage, pageSize: $scope.modelSearchLT.pageSize },
            success: function (data) {
                if (data.Error) {
                    toastr.error(data.Title);

                } else {
                    $scope.dsKhoLT = data.data;
                    if (data.data != null && data.data.length > 0) {
                        $scope.modelSearchLT.totalItems = data.data[0].TOTAL_ROW;
                        //$scope.modelSearchLT.pageSize = data.data[0].PAGE_SIZE;
                    } else {
                        $scope.modelSearchLT.totalItems = 0;
                        //$scope.modelSearchLT.pageSize = 10;
                    }
                }


                //hideLoading();
                $scope.$apply();
            }
        });

    };


    $scope.ThemLT = function () {
        $scope.themLT = [{
            ID: 0,
            ID_KHO_CHINH: '',
            ID_KHO_LIEN_THONG: '',
            CHECKEDLT: false
        }]

        $scope.dsKhoLT = [...$scope.dsKhoLT, ...$scope.themLT];

    }

    $scope.HuyLT = function () {
        nhapkhoLT();
    }
    $scope.LuuLT = function () {
        $scope.listNhapLT = [];
        var isKho = false;
        var khochinh = false;
        var khoLT = false;
        angular.forEach($scope.dsKhoLT, function (data) {
            //if (data.CHECKED == true) {
            $scope.listNhapLT.push(data);
            //}
            if (data.ID_KHO_CHINH == data.ID_KHO_LIEN_THONG) {
                isKho = true;
            }
            if (data.ID_KHO_CHINH == '' || data.ID_KHO_CHINH == null || data.ID_KHO_CHINH == undefined) {
                khochinh = true;
            }
            if (data.ID_KHO_LIEN_THONG == '' || data.ID_KHO_LIEN_THONG == null || data.ID_KHO_LIEN_THONG == undefined) {
                khoLT = true;
            }
        })
        if (isKho == true) {
            toastr.error('Kho liên thông không được giống kho chính.');
            return;
        }
        if (khochinh == true) {
            toastr.error('Phải nhập Kho chính.');
            return;
        }
        if (khoLT == true) {
            toastr.error('Phải nhập Kho liên thông.');
            return;
        }
        $.ajax({
            type: 'post',
            url: '/KhoHang/ThemKhoLienThong',
            data: {
                hang: $scope.listNhapLT,
                xoa: $scope.listXoaLT

            },
            success: function (data) {

                if (data.Error) {
                    toastr.success('Thêm mới thành công.');
                    nhapkho();

                } else {
                    toastr.error(data.Title);

                }

                hideLoading();
                $scope.$apply();
            }
        });
    }


    $scope.listXoaLT = [];
    $scope.XoaLT = function () {
        var countDelete = 0;
        for (var i = 0; i < $scope.dsKhoLT.length; i++) {
            if ($scope.dsKhoLT[i].CHECKEDLT == true) {
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
                            for (var i = 0; i < $scope.dsKhoLT.length; i++) {
                                if ($scope.dsKhoLT[i].CHECKEDLT == true) {
                                    if ($scope.dsKhoLT[i].ID > 0) {
                                        $scope.listXoaLT.push($scope.dsKhoLT[i]);
                                    }

                                    $scope.dsKhoLT.splice(i, 1);
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


    $scope.toggleAllLT = function () {

        var toggleStatus = $scope.isAllSelectedLT;
        angular.forEach($scope.dsKhoLT, function (itm) { itm.CHECKEDLT = !toggleStatus; });

    }
    $scope.optionToggledLT = function () {
        $scope.isAllSelectedLT = $scope.dsKhoLT.every(function (itm) { return itm.CHECKEDLT; })

    }


    $scope.modelSearchSP = {
        ID_KHO: '',
        TEN_SP: ''
    }
    $scope.TimKiemSP = function () {
        nhapkhoSP();
    }
    $scope.chonKho = -1;
    $scope.clickKhoSP = function (item) {
        $scope.chonKho = item.ID_KHO_CHINH;
        $scope.modelSearchSP.ID_KHO = item.ID_KHO_CHINH;
        $scope.modelSearchSP.ID_KHO_LT = item.ID_KHO_LIEN_THONG;
        nhapkhoSP();
    }
    $scope.changesp = function () {
        nhapkhoSP();
    }

    $scope.HuySP = function () {
        nhapkhoSP();
    }
    function nhapkhoSP() {
        $scope.listXoaSP = [];
        $scope.dsCHSP = [];
        $scope.dsSP = [];
        $.ajax({
            type: 'post',
            url: '/KhoHang/dsPSKhongLienThong',
            data: $scope.modelSearchSP,
            success: function (data) {
                if (data.Error) {
                    toastr.error(data.Title);
                } else {
                    $scope.dsSP = data.data;

                }


                //hideLoading();
                $scope.$apply();
            }
        });

    };

    $scope.LuuSP = function () {
        $scope.listNhapSP = [];
        angular.forEach($scope.dsSP, function (data) {
            //if (data.CHECKED == true) {
            $scope.listNhapSP.push(data);
            //}
        })

        $.ajax({
            type: 'post',
            url: '/KhoHang/ThemSPKhongLienThong',
            data: {
                hang: $scope.listNhapSP,
                xoa: $scope.listXoaSP

            },
            success: function (data) {

                if (data.Error) {
                    toastr.success('Thêm mới thành công.');
                    nhapkhoSP();

                } else {
                    toastr.error('Thêm mới thất bại.');

                }

                hideLoading();
                $scope.$apply();
            }
        });
    }

    $scope.XoaSP = function () {
        var countDelete = 0;
        for (var i = 0; i < $scope.dsSP.length; i++) {
            if ($scope.dsSP[i].CHECKEDSP == true) {
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
                            for (var i = 0; i < $scope.dsSP.length; i++) {
                                if ($scope.dsSP[i].CHECKEDSP == true) {
                                    if ($scope.dsSP[i].ID > 0) {
                                        $scope.listXoaSP.push($scope.dsSP[i]);
                                    }

                                    $scope.dsSP.splice(i, 1);
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

    $scope.AddSPNhan_SanPhams = [];
    $scope.AddSPNhan_SanPhamChons = [];
    $scope.ThemSP = function () {
        if ($scope.modelSearchSP.ID_KHO != null && $scope.modelSearchSP.ID_KHO > 0) {
            $scope.AddSPNhan_SanPhamChons = [];
            for (var i = 0; i < $scope.ListSanPham.length; i++) {
                $scope.ListSanPham[i].SELECT = false;
                var check = $scope.dsSP.filter(x => x.ID_SP == $scope.ListSanPham[i].ID);
                if (check.length == 0) {
                    $scope.AddSPNhan_SanPhams.push($scope.ListSanPham[i]);
                }
                
               
            }

            $scope.AddSPNhan_Keyword = "";
            $('#AddCHSPDK_SPDK').modal('show');
        }
        else {
            toastr.error("Vui lòng chọn kho liên thông!");
        }
    };

    $scope.AddSPNhan_Luu = function () {
        for (var i = 0; i < $scope.AddSPNhan_SanPhamChons.length; i++) {
            var spn = {
                ID: 0,
                ID_KHO: $scope.modelSearchSP.ID_KHO,
                ID_SP: $scope.AddSPNhan_SanPhamChons[i].ID,
                TEN_SP: $scope.AddSPNhan_SanPhamChons[i].TEN_SP,
                MA_H55: $scope.AddSPNhan_SanPhamChons[i].MA_H55,
                SELECT: false
            };
            $scope.dsSP.push(spn);

        }
        $('#AddCHSPDK_SPDK').modal('hide');
    };


    $scope.AddSPNhan_SearchSanPham = function () {
        $scope.AddSPNhan_SanPhams = [];
        var sanPhams = $scope.ListSanPham.filter(function (x) {
            return (x.TEN_SP != null && x.TEN_SP.indexOf($scope.AddSPNhan_Keyword) != -1);
        });
        if (sanPhams != null && sanPhams.length > 0) {
            $scope.AddSPNhan_SanPhams = sanPhams;
        }
    };


    $scope.AddSPNhan_ChangeCheckAllTP = function () {
        if ($scope.AddSPNhan_CheckAllTP == true) {
            $scope.AddSPNhan_CheckAllTP = false;
            for (var i = 0; i < $scope.AddSPNhan_SanPhams.length; i++) {
                $scope.AddSPNhan_SanPhams[i].SELECT = false;
                // thêm vào danh sách sản phẩm
                $scope.AddSPNhan_SanPhamChons.push($scope.AddSPNhan_SanPhams[i]);
            }
            $scope.AddSPNhan_SanPhams = [];
        }

    };

    // Bỏ Chọn tất cả sản phẩm để add vào sp nhận
    $scope.AddSPNhan_ChangeCheckAllTPChon = function () {
        if ($scope.AddSPNhan_CheckAllTPChon == true) {
            $scope.AddSPNhan_CheckAllTPChon = false;
            for (var i = 0; i < $scope.AddSPNhan_SanPhamChons.length; i++) {
                $scope.AddSPNhan_SanPhamChons[i].SELECT = false;
                // thêm vào danh sách sản phẩm
                $scope.AddSPNhan_SanPhams.push($scope.AddSPNhan_SanPhamChons[i]);
            }
            $scope.AddSPNhan_SanPhamChons = [];
        }

    };

    // Bỏ Chọn trang phục nhận
    $scope.AddSPNhan_CheckedTrangPhucChon = function (index) {
        if ($scope.AddSPNhan_SanPhamChons[index].SELECT == true) {
            $scope.AddSPNhan_SanPhamChons[index].SELECT = false;
            // thêm vào danh sách chọn
            $scope.AddSPNhan_SanPhams.push($scope.AddSPNhan_SanPhamChons[index]);

            // xóa khỏi danh sách sản phẩm
            $scope.AddSPNhan_SanPhamChons.splice(index, 1);
        }
    };

    // Chọn trang phục nhận
    $scope.AddSPNhan_CheckedTrangPhuc = function (index) {
        if ($scope.AddSPNhan_SanPhams[index].SELECT == true) {
            $scope.AddSPNhan_SanPhams[index].SELECT = false;
            // thêm vào danh sách chọn
            $scope.AddSPNhan_SanPhamChons.push($scope.AddSPNhan_SanPhams[index]);

            // xóa khỏi danh sách sản phẩm
            $scope.AddSPNhan_SanPhams.splice(index, 1);
        }
    };

    $scope.toggleAllSP = function () {

        var toggleStatus = $scope.isAllSelectedSP;
        angular.forEach($scope.dsSP, function (itm) { itm.CHECKEDSP = !toggleStatus; });

    }
    $scope.optionToggledSP = function () {
        $scope.isAllSelectedSP = $scope.dsSP.every(function (itm) { return itm.CHECKEDSP; })

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

