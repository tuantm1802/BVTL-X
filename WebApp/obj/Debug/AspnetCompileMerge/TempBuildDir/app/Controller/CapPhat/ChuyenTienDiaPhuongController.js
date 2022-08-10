app.controller("ChuyenTienDiaPhuongController", function ($scope, $rootScope, $uibModal, $ngConfirm, showToast, hideLoading, $location, constant) {
    //#region giá trị mặc định
    $scope.IsShowRight = false;
    $scope.model = {};
    $scope.ListNam = [];
    $scope.isThem = 'N';
    $scope.selectedRow = -1;
    $scope.bieuGia_Keyword = '';
    var currYear = new Date().getFullYear();
    for (var i = currYear; i >= (currYear - 5); i--)
        $scope.ListNam.push({ Id: i, Text: i });

    $scope.NamThemMoiKH = currYear + '';
    $scope.NamKH = currYear + '';

    $scope.modelKH = {};
    angular.element(document).ready(function () {
        
    });
    //#region Biểu giá trang phục
    // Có hiển thị cột checkbox ở đầu không
    $scope.tree_Table_Checkbox_tab1_1 = false;
    // Model data
    $scope.tree_data_tab1_1 = [];
    $scope.GetDanhMuc = function () {
        $.ajax({
            type: 'get',
            url: '/ChuyenTienDiaPhuong/GetDanhMuc',
            data: { nam: $scope.NamKH },
            success: function (res) {
                if (res.Error) {
                    toastr.error(res.Title);
                } else {
                    $scope.ListKeHoach = res.keHoach;
                    $scope.$apply();
                }
            }
        })
    }
    $scope.GetDanhMuc();
    // cấu hình hiển thị cùng với icon
    $scope.expanding_property_tab1_1 = {
        field: "tenTrangPhuc",
        displayName: "Tên trang phục",
        width: "25%",
        rowspan: 1
    };
    // Cấu hình các cột sẽ hiển thị tiếp theo
    $scope.col_defs_tab1_1 = [[
        { field: "nam", displayName: "Năm", width: "15%", rowspan: 1, colspan: 1 },
        { field: "bieuGia", displayName: "Biểu giá", width: "30%", rowspan: 1, colspan: 1 },
        { field: "troGia", displayName: "% Trợ giá", width: "10%", rowspan: 1, colspan: 1 },
        { field: "moTa", displayName: "Mô tả", width: "10%", rowspan: 1, colspan: 1 },
    ]];

    $scope.body_defs_tab1_1 = [{ field: "nam" }, { field: "bieuGia" }, { field: "troGia" }, { field: "moTa" }];
    // event when change value checkbox
    $scope.ChangeTreeTableCheckbox_tab1_1 = function (item) {
        console.log(item);
    };

    // event when click choose row
    $scope.ClickTreeTableRow = function (item) {
        console.log(item);
    };
    //#endregion


    $scope.ThemKeHoach = function () {
        $.ajax({
            type: 'POST',
            url: '/ChuyenTienDiaPhuong/CheckKHTheoNam',
            data: { nam: $scope.NamKH },
            success: function (res) {
                if (res.Error) {
                    toastr.error(res.Title);
                } else {
                    if (res.check) {
                        $ngConfirm({
                            title: 'Xác nhận',
                            content: 'Đã có kế hoạch trong năm. Nếu thêm mới sẽ xóa toàn bộ dữ liệu kế hoạch đang có. Bạn có chắc chắn thêm mới hay không?',
                            scope: $scope,
                            buttons: {
                                OK: {
                                    text: 'Đồng ý',
                                    btnClass: 'btn-blue',
                                    action: function (scope, button) {
                                        $scope.IsShowRight = true;
                                        $scope.isThem = 'Y';
                                        $scope.TaoKeHoach();
                                    }
                                },
                                Hủy: function (scope, button) {
                                },
                            }
                        });
                    }
                    else {
                        $scope.IsShowRight = true;
                        $scope.TaoKeHoach();
                    }
                }
            }
        });
    }

    $scope.TaoKeHoach = function () {
        $.ajax({
            type: 'POST',
            url: '/ChuyenTienDiaPhuong/TaoKeHoach',
            data: { nam: $scope.NamKH },
            success: function (res) {
                if (res.Error) {
                    toastr.error(res.Title);
                } else {
                    $scope.modelKH.SO_KH = res.soKH;
                    $scope.modelKH.CAN_BO_ID = res.canBo.UserID;
                    $scope.modelKH.TEN_CAN_BO = res.canBo.Name;
                    $scope.modelKH.NAM = $scope.NamKH;
                    $scope.modelKH.TRANG_THAI = 'N';
                    $scope.$apply();
                }
            }
        })
    }

    $scope.LuuKeHoach = function () {
        $.ajax({
            type: 'POST',
            url: '/ChuyenTienDiaPhuong/LuuKeHoach',
            data: { cP_CTDP_KE_HOACH: $scope.modelKH, isThem: $scope.isThem },
            success: function (res) {
                if (res.Error) {
                    toastr.error(res.Title);
                } else {
                    toastr.success(res.Title);
                    $scope.isThem = 'N';
                    $scope.GetDanhMuc();
                }
            }
        })
    }
    $scope.ChangeNamKH = function () {
        $scope.IsShowRight = false;
        $scope.GetDanhMuc();
        //$scope.GetKpTx();
        //$scope.GetKPTT();
        //$rootScope.GetBieuGia($scope.NamKH);
        //$rootScope.GetDV_TroGia($scope.NamKH);
    }
    $scope.ChonKeHoach = function (item, index) {
        $scope.selectedRow = index;
        $scope.IsShowRight = true;
        $scope.isThem = 'N';
        $scope.modelKH = item;
        $scope.GetKpTx();
        $scope.GetKPTT();
        $rootScope.GetBieuGia(item.NAM);
        $rootScope.GetDV_TroGia(item.NAM);
        $rootScope.GetDSDV_UngNoTon(item.ID);
        $scope.GetDonVikhct();
    }
    $rootScope.GetBieuGia = function(nam) {
        $scope.dsBieuGia = [];
        $.ajax({
            type: 'post',
            url: '/ChuyenTienDiaPhuong/GetBieuGiaByNam',
            data: { nam: nam, search: $scope.bieuGia_Keyword },
            success: function (data) {
                if (!data.Error) {
                    $scope.dsBieuGia = data;
                } else {
                    toastr.error(data.message);
                }
                $scope.$apply();

            }
        });

    }
    $rootScope.GetDV_TroGia = function (nam) {
        $scope.dsDV_TroGia = [];
        $.ajax({
            type: 'post',
            url: '/ChuyenTienDiaPhuong/GetDonViTroGiaByNam',
            data: { nam: nam, search: '' },
            success: function (data) {
                if (!data.Error) {
                    $scope.dsDV_TroGia = data;
                } else {
                    toastr.error(data.message);
                }
                $scope.$apply();

            }
        });

    }
    $rootScope.GetDSDV_UngNoTon = function (id_kh) {
        $scope.dsDV_NoTon = [];
        $.ajax({
            type: 'post',
            url: '/ChuyenTienDiaPhuong/GetDSDV_UngNoTon',
            data: { id_kh: id_kh},
            success: function (data) {
                if (!data.Error) {
                    $scope.dsDV_NoTon = data;
             
                } else {
                    toastr.error(data.message);
                }
                $scope.$apply();

            }
        });

    }

    $scope.xoaDVTroGia = function () {
        $scope.listXoa = [];
        angular.forEach($scope.dsDV_TroGia, function (itm) {
            if (itm.CHECKEDDV == true) {
                var item = {
                    ID: itm.ID
                }
                $scope.listXoa.push(item);
             }
        });

        if ($scope.listXoa.length > 0) {
            $ngConfirm({
                title: 'Thông báo',
                content: 'Bạn có chắc chắn muốn xóa mục đã chọn?',
                scope: $scope,
                buttons: {
                    delete: {
                        text: 'Có',
                        btnClass: 'btn-blue',
                        action: function (scope, button) {
                            $.ajax({
                                type: 'post',
                                url: '/ChuyenTienDiaPhuong/XoaDonViTroGia',
                                data: {
                                    listCH: $scope.listXoa
                                },
                                success: function (data) {

                                    if (!data.Error) {
                                        toastr.success(data.Title);
                                        $rootScope.GetDV_TroGia($scope.modelKH.NAM);

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
    $scope.changeToggleAllDonVi = function () {
        var toggleStatus = $scope.toggleAllDonVi;
        angular.forEach($scope.dsDV_TroGia, function (itm) {
                itm.CHECKEDDV = toggleStatus;
            });
    }
    $scope.changeCheckDV = function () {
       
        $scope.toggleAllDonVi = $scope.dsDV_TroGia.every(function (itm) { return itm.CHECKEDDV; })
               
    }

    $scope.ThemDSDV_UngNoTon = function () {

        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/ChuyenTienDiaPhuong/_ThemMoiDV_UngNoTon',
            controller: 'ThemMoiDV_UngNoTon',
            size: 'xl',
            backdrop: 'static',
            resolve: {
                data: function () {
                    return {
                        nam: $scope.NamKH,
                        id_kh: $scope.modelKH.ID,
                        so_kh: $scope.modelKH.SO_KH
                    };
                }
            }
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            //$scope.LoadPage();
        });
    }
    $scope.xoaDV_UngNoTon = function () {
        $scope.listXoa = [];
        angular.forEach($scope.dsDV_NoTon, function (itm) {
            if (itm.CHECKEDDV == true) {
                var item = {
                    ID: itm.ID_DON_VI
                }
                $scope.listXoa.push(item);
            }
        });

        if ($scope.listXoa.length > 0) {
            $ngConfirm({
                title: 'Thông báo',
                content: 'Bạn có chắc chắn muốn xóa mục đã chọn?',
                scope: $scope,
                buttons: {
                    delete: {
                        text: 'Có',
                        btnClass: 'btn-blue',
                        action: function (scope, button) {
                            $.ajax({
                                type: 'post',
                                url: '/ChuyenTienDiaPhuong/XoaDV_UngNoTon',
                                data: {
                                    listCH: $scope.listXoa,
                                    id_KH: $scope.modelKH.ID
                                },
                                success: function (data) {

                                    if (!data.Error) {
                                        toastr.success(data.Title);
                                        $rootScope.GetDSDV_UngNoTon($scope.modelKH.ID);

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
    $scope.huyDV_UngNoTon = function () {
        $rootScope.GetDSDV_UngNoTon($scope.modelKH.ID);
    }
    $scope.luuDV_UngNoTon = function () {
        $scope.listSua = [];
        var kpdn_cb = true;
        var kpdn_pham = true;
        angular.forEach($scope.dsDV_NoTon, function (itm) {
           
            if (itm.CHECKED_SUA == true) {
                if (itm.DN_KP_CBCS < 0) {
                    kpdn_cb = false;
                }
                if (itm.DN_KP_PHAM < 0) {
                    kpdn_pham = false;
                }
                var item = {
                    ID_KH: itm.ID_KH,
                    ID_DON_VI: itm.ID_DON_VI,
                    QT_NT_CBCS: itm.QT_NT_CBCS,
                    QT_NT_PHAM: itm.QT_NT_PHAM,
                    QT_NT_TONG: itm.QT_NT_TONG,
                    DN_KP_CBCS: itm.DN_KP_CBCS,
                    DN_KP_PHAM: itm.DN_KP_PHAM,
                    DN_KP_TONG: itm.DN_KP_TONG
                }

                $scope.listSua.push(item);
            }

        });
        if (kpdn_cb == false) {
            toastr.error("Kinh phí đầu năm CBCS không được âm.");
            return;
        }
        if (kpdn_pham == false) {
            toastr.error("Kinh phí đầu năm phạm không được âm.");
            return;
        }
        if ($scope.listSua.length == 0) {
            toastr.error("Không có bản ghi nào sửa.");
            return;
        }
        $.ajax({
            type: 'post',
            url: '/ChuyenTienDiaPhuong/SuaDV_UngNoTon',
            data: {
                listCH: $scope.listSua
            },
            success: function (data) {

                if (!data.Error) {
                    toastr.success(data.Title);
                    $rootScope.GetDSDV_UngNoTon($scope.modelKH.ID);

                } else {
                    toastr.error(data.Title);

                }

                hideLoading();
                $scope.$apply();
            }
        });
    };
    $scope.changeSua_UngNoTon = function (index, type) {
        $scope.dsDV_NoTon[index].CHECKED_SUA = true;
        if (type == 'DN') {
            $scope.dsDV_NoTon[index].DN_KP_TONG = $scope.dsDV_NoTon[index].DN_KP_CBCS + $scope.dsDV_NoTon[index].DN_KP_PHAM;
        } else {
            $scope.dsDV_NoTon[index].QT_NT_TONG = $scope.dsDV_NoTon[index].QT_NT_CBCS + $scope.dsDV_NoTon[index].QT_NT_PHAM;
        }
    }
    $scope.changeToggleDV_UngNoTon = function () {
        var toggleStatus = $scope.toggleAllDV_UngNoTon;
        angular.forEach($scope.dsDV_NoTon, function (itm) {
            itm.CHECKEDDV = toggleStatus;
        });
    }
    $scope.changeCheckDV_UngNoTon = function () {

        $scope.toggleAllDV_UngNoTon = $scope.dsDV_NoTon.every(function (itm) { return itm.CHECKEDDV; })

    }

    $scope.huyBieuGia = function () {
        $rootScope.GetBieuGia($scope.NamKH);
    }
    $scope.xoaBieuGia = function () {
        $scope.stringXoa = '';
        angular.forEach($scope.dsBieuGia, function (itm) {
            angular.forEach(itm.children, function (itm2) {
                if (itm2.CHECKEDBG == true) {
                    if ($scope.stringXoa == '') {
                        $scope.stringXoa = itm2.id;
                    } else {
                        $scope.stringXoa = $scope.stringXoa + ',' + itm2.id;
                    }
                }
            });

        });

        if ($scope.stringXoa != '') {
            $ngConfirm({
                title: 'Thông báo',
                content: 'Bạn có chắc chắn muốn xóa mục đã chọn?',
                scope: $scope,
                buttons: {
                    delete: {
                        text: 'Có',
                        btnClass: 'btn-blue',
                        action: function (scope, button) {
                            $.ajax({
                                type: 'post',
                                url: '/ChuyenTienDiaPhuong/XoaBieuGiaTrangPhuc',
                                data: {
                                    idSP: $scope.stringXoa
                                },
                                success: function (data) {

                                    if (!data.Error) {
                                        toastr.success(data.Title);
                                        $rootScope.GetBieuGia($scope.NamKH);

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
    $scope.bieuGia_Search = function () {
        $rootScope.GetBieuGia($scope.NamKH);
    }
    $scope.luuBieuGia = function () {
        $scope.listSua = [];
        var checkBieuGia = true;
        var checkTroGia = true;
        angular.forEach($scope.dsBieuGia, function (itm) {
            if (itm.CHECKEDBG == true) {

                if (itm.bieu_gia == null || itm.bieu_gia < 0) {
                    checkBieuGia = false;
                }
                if (itm.phan_tram == null || itm.phan_tram < 0) {
                    itm.phan_tram = 0;
                }
                    var item = {
                        ID: itm.id,
                        BIEU_GIA: itm.bieu_gia,
                        PHAN_TRAM: itm.phan_tram
                }
    
                    $scope.listSua.push(item);
            }

        });
        if (checkBieuGia == false) {
            toastr.error("Biểu giá không được để trống và là số nguyên dương.");
            return;
        }
        if (checkTroGia == false) {
            toastr.error("Trợ giá phải là số nguyên dương.");
            return;
        }
        if ($scope.listSua.length == 0) {
            toastr.error("Không có biểu giá nào sửa.");
            return;
        }
       $.ajax({
                                type: 'post',
                                url: '/ChuyenTienDiaPhuong/SuaBieuGiaTrangPhuc',
                                data: {
                                    listBieuGia: $scope.listSua
                                },
                                success: function (data) {

                                    if (!data.Error) {
                                        toastr.success(data.Title);
                                        $rootScope.GetBieuGia($scope.NamKH);

                                    } else {
                                        toastr.error(data.Title);

                                    }

                                    hideLoading();
                                    $scope.$apply();
                                }
                            });
    };

    $scope.changeSua = function (index) {
        $scope.dsBieuGia[index].CHECKEDBG = true;
    }

    $scope.changeToggleAllCheckboxes = function () {
        var toggleStatus = $scope.toggleAllCheckboxes;
        angular.forEach($scope.dsBieuGia, function (itm) {
            angular.forEach(itm.children, function (itm2) {
                    itm2.CHECKEDBG = toggleStatus;
            });
            
        });

    }
    $scope.changeCheck = function (check) {
        var check_child = true;
        if (check == false) {
            check_child = false;
        }
        else {
            angular.forEach($scope.dsBieuGia, function (itm) {
                var check_child2 = itm.children.every(function (itm2) { return itm2.CHECKEDBG;})
                if (check_child2 == false) {
                    check_child = false;
                }
            });
        }

        $scope.toggleAllCheckboxes = check_child;
    }
    //#region Thêm mới từ danh sách
    $scope.ThemMoiTuDanhSach = function () {
       
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/ChuyenTienDiaPhuong/_ThemMoiTuDanhSachT1',
            controller: 'ThemMoiTuDanhSachT1',
            size: 'xl',
            backdrop: 'static',
            resolve: {
                data: function () {
                    return {
                        nam: $scope.NamKH
                    };
                }
            }
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            //$scope.LoadPage();
        });
    }
    //#region Thêm mới đơn vị danh sách
    $scope.ThemDVTroGia = function () {
       
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/ChuyenTienDiaPhuong/_ThemMoiDonViTroGia',
            controller: 'ThemMoiDonViTroGia',
            size: 'xl',
            backdrop: 'static',
            resolve: {
                data: function () {
                    return {
                        nam: $scope.NamKH
                    };
                }
            }
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            //$scope.LoadPage();
        });
    }

    //#endregion

    $scope.cancel = function () {
        ClickTreeTableRow_tab1_1.close();
    };
    //Kinh phí khác 
    $scope.GetKpTx = function () {
        $.ajax({
            type: 'get',
            url: '/ChuyenTienDiaPhuong/GetKinhPhiThuongXuyen',
            data: { idKH: $scope.modelKH.ID },
            success: function (data) {
                if (data.Error) {
                    toastr.error('Lấy danh sách kinh phí thường xuyên không thành công');
                } else {
                    $scope.ListKinhPhiThuongXuyen = data.data;
                    angular.forEach($scope.ListKinhPhiThuongXuyen, function (val) {
                        val.Selected = false;
                        val.Edit = false;
                    });
                    $scope.$apply();
                }
            }
        })
    }

    $rootScope.rows = [];
    $rootScope.addkptt = [];
    $scope.DSDonVi = function () {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/ChuyenTienDiaPhuong/_DanhSachDonVi',
            controller: 'DanhSachDonVi',
            size: 'xl',
            backdrop: 'static',
            resolve: {
                KeHoach: function () {
                    return $scope.modelKH;
                },
                KPTX: function () {
                    return $scope.ListKinhPhiThuongXuyen;
                }
            }
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            //$scope.LoadPage();
        });
    }
    $scope.SaveKptx = function () {
        var flag = true;
        if ($rootScope.rows.length > 0) {
            angular.forEach($rootScope.rows, function (data) {
                if (data.KP_HOA_TRANG == null || data.KP_HOA_TRANG == "") {
                    data.KPHT = true;
                    flag = false;
                    data.textht = "Bạn chưa nhập kinh phí."
                } else {
                    if (isUnicodeFunc(data.KP_HOA_TRANG) == false) {
                        data.KPHT = true;
                        flag = false;
                        data.textht = "Kinh phí nhập không đúng định dạng."
                    } else {
                        data.KPHT = false;
                    }
                }
                if (data.KP_BAO_HO_LD == null || data.KP_BAO_HO_LD == "") {
                    data.KPBH = true;
                    flag = false;
                    data.textbh = "Bạn chưa nhập kinh phí."
                } else {
                    if (isUnicodeFunc(data.KP_BAO_HO_LD) == false) {
                        data.KPBH = true;
                        flag = false;
                        data.textbh = "Kinh phí nhập không đúng định dạng."
                    } else {
                        data.KPBH = false;
                    }
                }
            });
            if (flag == true) {
                $.ajax({
                    type: 'POST',
                    url: '/ChuyenTienDiaPhuong/ThemKinhPhiThuongXuyen',
                    data: { model: $rootScope.rows },
                    success: function (res) {
                        if (res.Error) {
                            toastr.error(res.Title);
                        } else {
                            toastr.success(res.Title);
                            $rootScope.rows = [];
                            $scope.GetKpTx();

                        }
                    }
                })
            }
        } else {
            $.ajax({
                type: 'POST',
                url: '/ChuyenTienDiaPhuong/ThemKinhPhiThuongXuyen',
                data: { model: $scope.ListKinhPhiThuongXuyen },
                success: function (res) {
                    if (res.Error) {
                        toastr.error('Cập nhật kinh phí thất bại.');
                    } else {
                        toastr.success('Cập nhật kinh phí thành công.');
                        $rootScope.rows = [];
                        $scope.GetKpTx();

                    }
                }
            })
        }
    }
    $scope.CheckAllKptx = function () {
        angular.forEach($scope.ListKinhPhiThuongXuyen, function (data) {
            data.Selected = !$scope.SelectAllKptx;
        });
    }
    $scope.CheckAllKptt = function () {
        angular.forEach($scope.ListKinhPhiTangThemDT, function (data) {
            data.Selected = !$scope.SelectAllKptt;
        });
    }
    $scope.ListXoaKptx = [];
    $scope.DeleteKptx = function () {
        angular.forEach($scope.ListKinhPhiThuongXuyen, function (data) {
            if (data.Selected) {
                $scope.ListXoaKptx.push(data);
            }
        });
        if ($scope.ListXoaKptx.length > 0) {
            $ngConfirm({
                title: 'Thông báo',
                content: 'Bạn có chắc chắn muốn xóa các kinh phí đã chọn không?',
                scope: $scope,
                buttons: {
                    delete: {
                        text: 'Xóa',
                        btnClass: 'btn-blue',
                        action: function (scope, button) {
                            $.ajax({
                                type: 'POST',
                                url: '/ChuyenTienDiaPhuong/XoaKinhPhiThuongXuyen',
                                data: { model: $scope.ListXoaKptx },
                                success: function (res) {
                                    if (res.Error) {
                                        toastr.error(res.Title);
                                    } else {
                                        toastr.success(res.Title);
                                        $scope.GetKpTx();
                                        $scope.ListXoaKptx = [];
                                    }
                                }
                            })
                        }
                    },
                    close: {
                        text: 'Hủy',
                        action: function (scope, button) {

                        }
                    }
                }
            });
            
        } else toastr.error('Bạn chưa chọn kinh phí để xóa.');
    }
    function isUnicodeFunc(string) {
        let isUnicode = true;
        for (let i = 0; i < string.length; i++) {
            let code = string.charCodeAt(i);
            if (code > 0x002F && code < 0x003A) {
                isUnicode = true;
            } else {
                isUnicode = false;
                break;
            }
        }
        return isUnicode;
    }
    $scope.BackKptx = function () {
        $rootScope.rows = [];
        angular.forEach($scope.ListKinhPhiThuongXuyen, function (val) {
            val.Edit = false;
        });
        $scope.GetKpTx();
    }
    $scope.BackKptt = function () {
        $scope.ListAddKp = [];
    }
    $scope.BackKpttDt = function () {
        $rootScope.addkptt = [];
        angular.forEach($scope.ListKinhPhiTangThemDT, function (val) {
            val.Edit = false;
        });
        $scope.SelectKP($scope.modelKPTT);
    }
    $scope.ListAddKp = [];
    $scope.AddKP = function () {
        $scope.ListAddKp.push({
            TEN_KINH_PHI_TANG_THEM: '',
            NAM: $scope.modelKH.NAM,
            KPT: false
        });
    }
    $scope.ListKinhPhiTangThem = [];
    $scope.GetKPTT = function () {
        $.ajax({
            type: 'get',
            url: '/ChuyenTienDiaPhuong/GetKinhPhiTangThem',
            data: { nam: $scope.modelKH.NAM },
            success: function (data) {
                if (data.Error) {
                    toastr.error('Lấy danh sách kinh phí tăng thêm không thành công');
                } else {
                    $scope.ListKinhPhiTangThem = data.data;
                    $scope.$apply();
                }
            }
        })
    }
    $scope.SaveKpTT = function () {
        var flag = true;
        if ($scope.ListAddKp != "") {
            angular.forEach($scope.ListAddKp, function (data) {
                if (data.TEN_KINH_PHI_TANG_THEM == null || data.TEN_KINH_PHI_TANG_THEM == "") {
                    data.KPT = true;
                    flag = false;
                } else {
                    data.KPT = false;
                }
            });
            if (flag == true) {
                $.ajax({
                    type: 'post',
                    url: '/ChuyenTienDiaPhuong/AddKinhPhiTangThem',
                    data: { ds: $scope.ListAddKp },
                    success: function (data) {
                        if (data.Error) {
                            toastr.error(data.Title, 'Thêm mới dữ liệu thất bại');
                        } else {
                            toastr.success(data.Title, 'Thêm mới dữ liệu thành công');
                            $scope.ListAddKp = [];
                            $scope.GetKPTT();
                        }
                        hideLoading();
                    }
                })
            }
        }
    }
    $scope.ListXoaKptt = [];
    $scope.DeleteKptt = function () {
        angular.forEach($scope.ListKinhPhiTangThemDT, function (data) {
            if (data.Selected) {
                $scope.ListXoaKptt.push(data);
            }
        });
        if ($scope.ListXoaKptt.length > 0) {
            $ngConfirm({
                title: 'Thông báo',
                content: 'Bạn có chắc chắn muốn xóa các kinh phí đã chọn không?',
                scope: $scope,
                buttons: {
                    delete: {
                        text: 'Xóa',
                        btnClass: 'btn-blue',
                        action: function (scope, button) {
                            $.ajax({
                                type: 'POST',
                                url: '/ChuyenTienDiaPhuong/XoaKinhPhiTangThemDT',
                                data: { ds: $scope.ListXoaKptt },
                                success: function (res) {
                                    if (res.Error) {
                                        toastr.error(res.Title);
                                    } else {
                                        toastr.success(res.Title);
                                        $scope.SelectKP($scope.modelKPTT);
                                        $scope.ListXoaKptt = [];
                                    }
                                }
                            })
                        }
                    },
                    close: {
                        text: 'Hủy',
                        action: function (scope, button) {

                        }
                    }
                }
            });

        } else toastr.error('Bạn chưa chọn kinh phí để xóa.');
    }
    $scope.ListKinhPhiTangThemDT = [];
    $scope.modelKPTT = {};
    $scope.CheckKptt = false;
    $scope.TenKpTT = {};
    $scope.SelectKP = function (item) {
        $scope.modelKPTT = item;
        $scope.TenKpTT = item.TEN_KINH_PHI_TANG_THEM;
        $.ajax({
            type: 'get',
            url: '/ChuyenTienDiaPhuong/GetKinhPhiTangThemDT',
            data: { idKP: item.ID },
            success: function (data) {
                if (data.Error) {
                    toastr.error('Lấy kinh phí tăng thêm không thành công');
                } else {
                    $scope.ListKinhPhiTangThemDT = data.data;
                    $scope.CheckKptt = true;
                    $scope.$apply();
                }
            }
        })
    }
    $scope.XoaKpTT = function () {
        if ($scope.modelKPTT != undefined) {
            $ngConfirm({
                title: 'Thông báo',
                content: 'Bạn có chắc chắn muốn xóa kinh phí đã chọn không?',
                scope: $scope,
                buttons: {
                    delete: {
                        text: 'Xóa',
                        btnClass: 'btn-blue',
                        action: function (scope, button) {
                            $.ajax({
                                type: 'POST',
                                url: '/ChuyenTienDiaPhuong/XoaKinhPhiTangThem',
                                data: { id: $scope.modelKPTT.ID },
                                success: function (res) {
                                    if (res.Error) {
                                        toastr.error(res.Title);
                                    } else {
                                        toastr.success(res.Title);
                                        $scope.GetKPTT();

                                    }
                                }
                            })
                        }
                    },
                    close: {
                        text: 'Hủy',
                        action: function (scope, button) {

                        }
                    }
                }
            });
        } else toastr.error('Bạn chưa chọn kinh phí để xóa.');
    }
    $scope.AddDonViKPTT = function () {
        if ($scope.CheckKptt == true) {
            var modalInstance = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: '/ChuyenTienDiaPhuong/_DanhSachDonViKPTT',
                controller: 'DanhSachDonViKPTT',
                size: 'xl',
                backdrop: 'static',
                resolve: {
                    KeHoach: function () {
                        return $scope.modelKPTT;
                    },
                    KPTT: function () {
                        return $scope.ListKinhPhiTangThemDT;
                    }
                }
            });

            //kết quả trả về của modal
            modalInstance.result.then(function (response) {
                //$scope.LoadPage();
            });
        } else toastr.error('Bạn chưa chọn kinh phí tăng thêm');
        
    }
    $scope.loadTemplateKptx = function (index) {
        $scope.ListKinhPhiThuongXuyen[index].Edit = true;
    }
    $scope.loadTemplateKptt = function (index) {
        $scope.ListKinhPhiTangThemDT[index].Edit = true;
    }
    $scope.SaveKpTTDT = function () {
        var flag = true;
        if ($rootScope.addkptt.length > 0) {
            angular.forEach($rootScope.addkptt, function (data) {
                if (data.GIA_TRI == null || data.GIA_TRI == "") {
                    data.GT = true;
                    flag = false;
                    data.text = "Bạn chưa nhập kinh phí."
                } else {
                    if (isUnicodeFunc(data.GIA_TRI) == false) {
                        data.GT = true;
                        flag = false;
                        data.text = "Kinh phí nhập không đúng định dạng."
                    } else {
                        data.GT = false;
                    }
                }
            });
            if (flag == true) {
                $.ajax({
                    type: 'post',
                    url: '/ChuyenTienDiaPhuong/AddKinhPhiTangThemDT',
                    data: { ds: $rootScope.addkptt },
                    success: function (data) {
                        if (data.Error) {
                            toastr.error('Thêm mới dữ liệu thất bại');
                        } else {
                            toastr.success('Thêm mới dữ liệu thành công');
                            $scope.addkptt = [];
                            $scope.SelectKP($scope.modelKPTT);
                        }
                        hideLoading();
                    }
                })
            }
        } else {
            $.ajax({
                type: 'post',
                url: '/ChuyenTienDiaPhuong/EditKinhPhiTangThemDT',
                data: { ds: $scope.ListKinhPhiTangThemDT },
                success: function (data) {
                    if (data.Error) {
                        toastr.error('Cập nhật kinh phí thất bại');
                    } else {
                        toastr.success('Cập nhật kinh phí thành công');
                        $scope.addkptt = [];
                        $scope.SelectKP($scope.modelKPTT);
                    }
                    hideLoading();
                }
            })
        }
    }
    //Ke hoach chuyen tien
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.GetDonVikhct = function () {
        $scope.ListDonVikh = [];
        $.ajax({
            type: 'post',
            url: '/ChuyenTienDiaPhuong/GetDonViKh',
            data: { idKh: $scope.modelKH.ID},
            success: function (data) {
                if (data.Error) {
                    toastr.error('Lỗi lấy danh sách đơn vị');
                } else {
                    $scope.ListDonVikh = data.data;
                    $scope.$apply();
                }
            }
        })
    }
    $scope.KeHoachChuyenTienDT = function (item) {
            var modalInstance = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: '/ChuyenTienDiaPhuong/_KeHoachChuyenTienDT',
                controller: 'KeHoachChuyenTienDT',
                size: 'lg',
                backdrop: 'static',
                resolve: {
                    idKH: function () {
                        return $scope.modelKH.ID;
                    },
                    Donvi: function () {
                        return item;
                    }
                }
            });
            //kết quả trả về của modal
            modalInstance.result.then(function (response) {
                //$scope.LoadPage();
            });
    }
});
app.controller('ThemMoiDV_UngNoTon', function ($scope, $rootScope, $uibModalInstance, $ngConfirm, showToast, hideLoading, data) {

    $scope.AddSPNhan_Keyword = "";
    $scope.AddSPNhan_SanPhamChons = [];

    $scope.LoadPage = function () {
        showToast()
        $.ajax({
            type: 'POST',
            url: '/ChuyenTienDiaPhuong/GetDSDVCH_UngNoTon',
            data: {
                id_kh: data.id_kh
            },
            success: function (res) {
                if (!res.Error) {
                    $scope.DanhSachSanPham = res.data;
                    $scope.AddSPNhan_SanPhams = $scope.DanhSachSanPham;
                    $scope.$apply();
                    hideLoading();
                }
            }
        })
    }
    $scope.LoadPage();

    $scope.ChonSanPham = function () {
        $scope.SelectedTrangPhuc = [];
        angular.forEach($scope.AddSPNhan_SanPhamChons, function (val, key) {
            var item = {
                ID: val.ID
            }
            $scope.SelectedTrangPhuc.push(item);
        })

        if ($scope.SelectedTrangPhuc.length > 0) {
            $.ajax({
                type: 'POST',
                url: '/ChuyenTienDiaPhuong/ThemDV_UngNoTon',
                data: {
                    listCH: $scope.SelectedTrangPhuc,
                    nam: data.nam,
                    id_KH: data.id_kh,
                    so_KH: data.so_kh
                },
                success: function (res) {

                    if (res.Error) {
                        toastr.error(res.Title);
                    } else {
                        $rootScope.GetDSDV_UngNoTon(data.id_kh);
                        toastr.success(res.Title);
                        $uibModalInstance.close();
                    }
                }
            })
        }

    }

    $scope.AddSPNhan_SearchSanPham = function () {
        $scope.AddSPNhan_SanPhams = [];
        var sanPhams = $scope.DanhSachSanPham.filter(function (x) {
            return (x.TEN_DON_VI != null && x.TEN_DON_VI.toUpperCase().indexOf($scope.AddSPNhan_Keyword.toUpperCase()) != -1);
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

    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});
app.controller('ThemMoiDonViTroGia', function ($scope, $rootScope, $uibModalInstance, $ngConfirm, showToast, hideLoading, data) {
  
    $scope.AddSPNhan_Keyword = "";
    $scope.AddSPNhan_SanPhamChons = [];

    $scope.LoadPage = function () {
        showToast()
        $.ajax({
            type: 'POST',
            url: '/ChuyenTienDiaPhuong/GetDanhSachDonVi',
            data: {
                nam: data.nam
            },
            success: function (res) {
                if (!res.Error) {
                    $scope.DanhSachSanPham = res.data;
                    $scope.AddSPNhan_SanPhams = $scope.DanhSachSanPham;
                    $scope.$apply();
                    hideLoading();
                }
            }
        })
    }
    $scope.LoadPage();

    $scope.ChonSanPham = function () {
        $scope.SelectedTrangPhuc = [];
        angular.forEach($scope.AddSPNhan_SanPhamChons, function (val, key) {
            var item = {
                ID: val.ID
            }
            $scope.SelectedTrangPhuc.push(item);
        })

        if ($scope.SelectedTrangPhuc.length > 0) {
            $.ajax({
                type: 'POST',
                url: '/ChuyenTienDiaPhuong/CauHinhDonViTroGia',
                data: {
                    listCH: $scope.SelectedTrangPhuc,
                    nam: data.nam
                },
                success: function (res) {

                    if (res.Error) {
                        toastr.error(res.Title);
                    } else {
                        $rootScope.GetDV_TroGia(data.nam);
                        toastr.success(res.Title);
                        $uibModalInstance.close();
                    }
                }
            })
        }
        
    }

    $scope.AddSPNhan_SearchSanPham = function () {
        $scope.AddSPNhan_SanPhams = [];
        var sanPhams = $scope.DanhSachSanPham.filter(function (x) {
            return (x.TEN_DON_VI != null && x.TEN_DON_VI.toUpperCase().indexOf($scope.AddSPNhan_Keyword.toUpperCase()) != -1);
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

    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});
app.controller('ThemMoiTuDanhSachT1', function ($scope, $rootScope, $uibModalInstance, $ngConfirm, showToast, hideLoading, data) {
    $scope.model = {};
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.model = {};
    $scope.AddSPNhan_Keyword = "";
    $scope.AddSPNhan_SanPhamChons = [];
   
    $scope.LoadPage = function () {
        showToast()
        $.ajax({
            type: 'POST',
            url: '/ChuyenTienDiaPhuong/GetDanhSachSanPham',
            data: {
                nam: data.nam
            },
            success: function (res) {
                if (!res.Error) {
                    $scope.DanhSachSanPham = res.data.SanPhams;
                    $scope.AddSPNhan_SanPhams = $scope.DanhSachSanPham;
                    $scope.$apply();
                    hideLoading();
                }
            }
        })
    }
    $scope.LoadPage();

    $scope.ChonSanPham = function () {
        if ($scope.model.BIEU_GIA === null || $scope.model.BIEU_GIA === '') {
            toastr.error('Bạn chưa nhập biểu giá.');
            return;
        }
        if ($scope.formThemMoiTuDanhSachT1.BIEU_GIA.$error.pattern == true) {
            toastr.error('Biểu giá phải là số nguyên dương.');
            return;
        }
        if ($scope.model.BIEU_GIA < 0) {
            toastr.error('Biểu giá phải là số nguyên dương.');
            return;
        }
        if ($scope.formThemMoiTuDanhSachT1.PHAN_TRAM_TRO_GIA.$error.pattern == true) {
            toastr.error('Trợ giá phải là số nguyên dương.');
            return;
        }
        if ($scope.model.PHAN_TRAM_TRO_GIA < 0) {
            toastr.error('Trợ giá phải là số nguyên dương.');
            return;
        }
            $scope.SelectedTrangPhuc = [];
            $scope.ListIdSP = '';
            angular.forEach($scope.AddSPNhan_SanPhamChons, function (val, key) {
                    $scope.SelectedTrangPhuc.push(val.ID);
                    if ($scope.ListIdSP === '')
                        $scope.ListIdSP = val.ID + '';
                    else
                        $scope.ListIdSP += ',' + val.ID;
            })
            $.ajax({
                type: 'POST',
                url: '/ChuyenTienDiaPhuong/ThemBieuGiaTrangPhuc',
                data: {
                    idSP: $scope.ListIdSP,
                    NAM: data.nam,
                    BIEU_GIA: $scope.model.BIEU_GIA,
                    PHAN_TRAM_TRO_GIA: $scope.model.PHAN_TRAM_TRO_GIA,
                    MO_TA: '',
                },
                success: function (res) {
                    console.log(res);
                    if (res.Error) {
                        toastr.error(res.Title);
                    } else {
                        $rootScope.GetBieuGia(data.nam);
                        toastr.success(res.Title);
                        $uibModalInstance.close();
                    }
                }
            })

       
    }

    $scope.AddSPNhan_SearchSanPham = function () {
        $scope.AddSPNhan_SanPhams = [];
        var sanPhams = $scope.DanhSachSanPham.filter(function (x) {
            return (x.TEN_SP != null && x.TEN_SP.toUpperCase().indexOf($scope.AddSPNhan_Keyword.toUpperCase()) != -1);
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

    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});
app.controller('DanhSachDonVi', function ($scope, $rootScope, $uibModalInstance, $ngConfirm, showToast, hideLoading, KeHoach, KPTX) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    angular.element(document).ready(function () {
        $scope.GetDonVi();
    });
    $scope.ListDonVi = [];
    $scope.GetDonVi = function () {
        $.ajax({
            type: 'get',
            url: '/ChuyenTienDiaPhuong/GetDonVi',
            data: {},
            success: function (data) {
                if (data.Error) {
                    toastr.error('Lỗi lấy danh sách đơn vị');
                } else {
                    $scope.ListDonVi = data.data;
                    angular.forEach($scope.ListDonVi, function (val) {
                        var count = KPTX.filter(x => x.ID_DON_VI === val.ID).length;
                        if (count > 0) {
                            val.Selected = true;
                            val.Status = true;
                        } else {
                            val.Selected = false;
                            val.Status = false;
                        }
                    });
                    $scope.$apply();
                }
            }
        })
    }
    $scope.selectAll = false;
    $scope.checkAll = function () {
        angular.forEach($scope.ListDonVi, function (val) {
            if (val.Status != true) {
                val.Selected = !$scope.selectAll;
            }
        });

    }
    $scope.ChonDonVi = function () {
        angular.forEach($scope.ListDonVi, function (val) {
            if (val.Selected == true && val.Status != true) {
                $rootScope.rows.push({
                    ID_KH: KeHoach.ID,
                    SO_KH: KeHoach.SO_KH,
                    LOAI_KH: 'CT',
                    NAM: KeHoach.NAM,
                    KY: 'CN',
                    ID_DON_VI: val.ID,
                    TEN_DON_VI: val.TEN_DON_VI,
                    KP_HOA_TRANG: '',
                    KP_BAO_HO_LD: '',
                    KPHT: false,
                    KPBH: false,
                    textht: '',
                    textbh: '',
                });
            }
        });
        $uibModalInstance.close();
    }
    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});
app.controller('DanhSachDonViKPTT', function ($scope, $rootScope, $uibModalInstance, $ngConfirm, showToast, hideLoading, KeHoach, KPTT) {
    angular.element(document).ready(function () {
        $scope.GetDonVi();
    });
    $scope.ListDonVi = [];
    $scope.GetDonVi = function () {
        $.ajax({
            type: 'get',
            url: '/ChuyenTienDiaPhuong/GetDonVi',
            data: {},
            success: function (data) {
                if (data.Error) {
                    toastr.error('Lỗi lấy danh sách đơn vị');
                } else {
                    $scope.ListDonVi = data.data;
                    angular.forEach($scope.ListDonVi, function (val) {
                        var count = KPTT.filter(x => x.ID_DON_VI === val.ID).length;
                        if (count > 0) {
                            val.Selected = true;
                            val.Status = true;
                        } else {
                            val.Selected = false;
                            val.Status = false;
                        }
                    });
                    $scope.$apply();
                }
            }
        })
    }
    $scope.selectAll = false;
    $scope.checkAll = function () {
        angular.forEach($scope.ListDonVi, function (val) {
            if (val.Status != true) {
                val.Selected = !$scope.selectAll;
            }
        });

    }
    $scope.ChonDonVi = function () {
        angular.forEach($scope.ListDonVi, function (val) {
            if (val.Selected == true && val.Status != true) {
                $rootScope.addkptt.push({
                    ID_KP_TANG_THEM: KeHoach.ID,
                    ID_DON_VI: val.ID,
                    TEN_DON_VI: val.TEN_DON_VI,
                    GIA_TRI: '',
                    GT: false,
                    text: ''
                });
            }
        });
        $uibModalInstance.close();
    }
    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});
app.controller('KeHoachChuyenTienDT', function ($scope, $rootScope, $uibModalInstance, $ngConfirm, showToast, hideLoading, idKH, Donvi) {
    angular.element(document).ready(function () {

    //    $('#containerReportViewer').load('/ChuyenTienDiaPhuong/ChiTietKeHoachChuyenTien?SoCV=' + $scope.Socv + '&TenDV=' + Donvi.TEN_DON_VI + '&IdDV=' + Donvi.ID + '&idKH=' + idKH, function () {
    //        hideLoading();
    //    });

    });
    $scope.Socv = "";
    $scope.XemBC = function () {
        showToast();
        var url = '/ChuyenTienDiaPhuong/ChiTietKeHoachChuyenTien?SoCV=' + $scope.Socv + '&TenDV=' + encodeURIComponent(Donvi.TEN_DON_VI) + '&IdDV=' + Donvi.ID_DV + '&idKH=' + String(idKH)+'';
        $('#containerReportViewer').load(url, function () {
        //$('#containerReportViewer').load('/ChuyenTienDiaPhuong/ChiTietKeHoachChuyenTien?SoCV=' + $scope.Socv + '&TenDV=' + Donvi.TEN_DON_VI + '&IdDV=411&idKH=1', function () {
        hideLoading();
        });
        $("#modal1").modal('show');

    };
    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});


