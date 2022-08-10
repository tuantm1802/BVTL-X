app.controller("KeHoachCapPhatController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $location, $rootScope) {
    $scope.listTrangPhucAddView = [];
    $scope.ListKHCapPhat = [];
    $scope.ListKHCapPhatChoXl = [];
    $rootScope.dsSPDaChonQuyetToan = [];
    $scope.selectedRowKeHoach = -1;
    $rootScope.DonViId = 0;
    $rootScope.TenDonVi = "";
    $rootScope.ViewKeHoach = false;
    //Thông tin cấp phát  
    $rootScope.txtDonVi = "";
    $rootScope.txtNam = "";
    $rootScope.txtKy = "";
    $rootScope.txtLyDoCap = "";
    $scope.rdoDonVi = 1;
    $scope.listPhuongThucVanChuyen = [];
    $scope.DataDmKeHoach = [];

    $scope.ListNam = [];
    var currentYear = new Date().getFullYear();
    for (var i = currentYear; i > currentYear - 7; i--) {
        $scope.ListNam.push({ Id: i, Name: i });
    }
    $scope.cbbNam = currentYear.toString();
    $scope.cbbKy = 'XH';

    //#region =================================Danh schs OpenTabNTDoiTC đổi tiêu chuẩn==================================

    // Có hiển thị cột checkbox ở đầu không
    $scope.tree_Table_Checkbox_nt = false;
    // Model data
    $scope.tree_data_nt = [];
    $scope.GetDsNhanTien = function (khId, dvId, nam, ky) {
        debugger;
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoachCapPhat/TreeDsNhanTien',
            data: { khId: khId, dvId: dvId, nam: nam, ky: ky },
            success: function (data) {
                if (data.status) {
                    toastr.error(data.message);
                } else {
                    $scope.tree_data_nt = data.data;
                }
                hideLoading();
            }
        });
    };

    // cấu hình hiển thị cùng với icon
    $scope.expanding_property_nt = {
        field: "TreeName",
        displayName: "Trang phục",
        width: "60%",
        rowspan: 1
    };
    // Cấu hình các cột sẽ hiển thị tiếp theo
    $scope.col_defs_nt = [[
        { field: "TreeSoLuong", displayName: "SL cấp ", width: "20%", rowspan: 1, colspan: 1 },
        { field: "TreeSoLuongNhanTien", displayName: "SL nhận tiền", width: "20%", rowspan: 1, colspan: 1 }
    ]];
    $scope.body_defs_nt = [{ field: "TreeSoLuong" }, { field: "TreeSoLuongNhanTien" }];
    // event when change value checkbox
    $scope.ChangeTreeTableCheckbox_nt = function (item) {
    };
    // event when click choose row
    $scope.ClickTreeTableRow_nt = function (item) {
        //console.log(item);
    };

    //#endregion  ======================================================

    $scope.btnTinhLoaiKHTuDong = function () {
        showToast();
        //Tính toán lại số liệu kế hoạch tự động
        if ($scope.CP_SelectKeHoachItem.LOAI_KH.trim() === 'KH' && $scope.CP_SelectKeHoachItem.TRANG_THAI === 'I') {
            var modalInstance = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: '/KeHoachCapPhat/_DSLoaiTieuChuan',
                controller: 'DSLoaiTieuChuan',
                size: 'lg',
                backdrop: 'static',
                resolve: {
                    datamodel: function () {
                        return {
                            nam: $scope.CP_SelectKeHoachItem.NAM,
                            ky: $scope.CP_SelectKeHoachItem.KY,
                            donViId: $scope.CP_SelectKeHoachItem.ID_DON_VI,
                            moTa: $scope.CP_SelectKeHoachItem.MO_TA,
                            khId: $scope.CP_SelectKeHoachItem.ID_KH
                        }
                    }
                }
            });
            modalInstance.result.then(function (response) {
                $scope.CP_SelectKeHoachItem = response;
                $uibModalInstance.close($scope.CP_SelectKeHoachItem);
            });
        }
        else {
            hideLoading();
            toastr.error('Chỉ áp dụng cho loại kế hoạch tự động mới khởi tạo!');
        }
    };
    //#region tạo treeview
    var setting = {
        check: {
            enable: false
        },
        data: {
            simpleData: {
                enable: true,
                idKey: "id",
                pIdKey: "pId",
                rootPId: ""
            }
        },
        callback: {
            onClick: onClickTCCBCS
        }
    };
    function onClickTCCBCS(event, treeId, treeNode, clickFlag) {

        showToast();
        if (treeNode !== undefined && treeNode.id !== undefined && treeNode.isParent === false) {
            $scope.TEN_DON_VI = treeNode.name;
            $scope.DON_VI_ID = treeNode.id.split('_')[1];
            $scope.IsShowRight = true;
            $scope.IsVisibleTab = true;
            $rootScope.ViewKeHoach = false;
            $rootScope.DonViId = $scope.DON_VI_ID;
            $rootScope.TenDonVi = $scope.TEN_DON_VI;
            //Giảm hiệu ứng đơ khi cho chọn đơn vị rồi mới load dữ liệu

            //Danh sách kế hoạch cấp phát
            $scope.LoadDsKeHoach($scope.DON_VI_ID, $scope.cbbNam, $scope.cbbKy);

            //Disable các nút chức năng khi đã duyệt KH
            $scope.disableKHDuyet = false;

            if ($scope.ListKHCapPhat.length > 0) {
                $scope.tempKeHoach = $scope.ListKHCapPhat.filter(x => x.TRANG_THAI === 'U' || x.TRANG_THAI === 'I' || x.TRANG_THAI === 'R');
                if ($scope.tempKeHoach.length > 0)
                    $scope.IsVisibleThemMoiKH = false;
                else
                    $scope.IsVisibleThemMoiKH = true;
            }
            else {
                $scope.IsVisibleThemMoiKH = true;
            }
            $scope.$apply();
        }
        else {
            hideLoading();
            $scope.IsVisibleTab = false;
            $scope.$apply();
        }
    }
    $scope.txtSearchDmDonVi = "";
    $scope.treeData = {};
    $scope.treeData.data = {};
    $scope.ListDonVi;
    $.ajax({
        type: 'get',
        async: false,
        url: '/KeHoachCapPhat/GetTreeData',
        data: {
            keyword: $scope.txtSearchDmDonVi,
            isOpen: false
        },
        success: function (res) {
            if (!res.Error) {
                $scope.ListDonVi = res;
                $.fn.zTree.init($("#treeRole"), setting, $scope.ListDonVi.data);
                var zTree = $.fn.zTree.getZTreeObj("treeRole");
                var type = { "Y": "ps", "N": "ps" };
                zTree.setting.check.chkboxType = type;
            }
        }
    });
    //#endregion
    //Check kiểu hiển thị tree Đơn vị
    $scope.ChangeTreeViewDmDonVi = function (type) {
        var api = '/KeHoachCapPhat/GetTreeData';
        if (type === 1) {
            $scope.rdoDonVi = type;
            api = '/KeHoachCapPhat/GetTreeData';
        }
        else {
            $scope.rdoDonVi = type;
            api = '/KeHoachCapPhat/GetTreeDataKhuVuc';
        }
        $.ajax({
            type: 'get',
            async: false,
            url: api,
            data: {
                keyword: $scope.txtSearchDmDonVi,
                isOpen: false
            },
            success: function (res) {
                if (!res.Error) {
                    $scope.ListDonVi = res;
                    $.fn.zTree.init($("#treeRole"), setting, $scope.ListDonVi.data);
                    var zTree = $.fn.zTree.getZTreeObj("treeRole");
                    var type = { "Y": "ps", "N": "ps" };
                    zTree.setting.check.chkboxType = type;
                }
            }
        });
    }

    $scope.btnSearchDmDonVi = function () {
        var api = '/KeHoachCapPhat/GetTreeData';
        if ($scope.rdoDonVi === 1) {
            api = '/KeHoachCapPhat/GetTreeData';
        }
        else {
            api = '/KeHoachCapPhat/GetTreeDataKhuVuc';
        }
        $.ajax({
            type: 'get',
            async: false,
            url: api,
            data: {
                keyword: $scope.txtSearchDmDonVi,
                isOpen: true
            },
            success: function (res) {
                if (!res.Error) {
                    $scope.ListDonVi = res;
                    $.fn.zTree.init($("#treeRole"), setting, $scope.ListDonVi.data);
                    var zTree = $.fn.zTree.getZTreeObj("treeRole");
                    var type = { "Y": "ps", "N": "ps" };
                    zTree.setting.check.chkboxType = type;
                }
            }
        });
    }

    //#region Giải trình cấp phát
    $scope.GiaiTrinhCapPhat = function () {
        if ($scope.CP_SelectKeHoachItem != null && $scope.CP_SelectKeHoachItem.ID_KH > 0) {
            //window.location.href = '/KeHoachCapPhat/ExportGiaiTrinhKeHoach?soKeHoach=KH-BTh-2020-03-0001';
            window.location.href = '/KeHoachCapPhat/ExportGiaiTrinhKeHoach?soKeHoach=' + $scope.CP_SelectKeHoachItem.SO_KH;
        }
        else {
            toastr.error("Vui lòng chọn kế hoạch!");
        }

    }
    $scope.FnDoiTrangThaiKh = function (item, status) {
        debugger;
        if (item.TRANG_THAI !== 'A') {
            if (status === 'P')
                $rootScope.ViewKeHoach = false;
            $.ajax({
                type: 'post',
                url: '/KeHoachCapPhat/DoiTrangThaiKHCP',
                data: {
                    id: item.ID_KH,
                    trangThai: status
                },
                success: function (data) {
                    hideLoading();
                    if (data.Error == true) {
                        toastr.error(data.Title);
                    } else {
                        toastr.success(data.Title);
                        $scope.LoadDsKeHoach($scope.DON_VI_ID, $scope.cbbNam, $scope.cbbKy);
                        //Kiểm tra KH chưa duyệt 
                        $scope.tempKeHoach = $scope.ListKHCapPhat.filter(x => x.TRANG_THAI === 'U' || x.TRANG_THAI === 'I' || x.TRANG_THAI === 'R');
                        if ($scope.tempKeHoach.length > 0) {
                            $scope.IsVisibleThemMoiKH = false;
                        }
                        else {
                            $scope.IsVisibleThemMoiKH = true;
                        }
                        $scope.$apply();
                    }
                }
            });
        }
        else {
            toastr.error("Kế hoạch đã duyệt không thể chuyển!");
            return false;
        }
    }

    // Xóa kế hoạch chờ xử lý

    $scope.XoaKeHoachChoXL = function (idkh) {
        debugger;
        showToast();
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoachCapPhat/XoaKeHoachChoXL',
            data: {
                khId: idkh
            },
            success: function (data) {
                hideLoading();
                if (data.Error == false) {
                    toastr.success(data.Title);
                    $scope.LoadDsKeHoach($scope.DON_VI_ID, $scope.cbbNam, $scope.cbbKy);
                } else {
                    toastr.error(data.Title);
                }
            }
        });
    }
    $scope.btnKeHoachQuyetToan = function () {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/KeHoachCapPhat/_KeHoachQuyetToan',
            controller: 'KeHoachQuyetToan',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                data: function () {
                    return {
                        nam: $scope.cbbNam,
                        ky: $scope.cbbKy
                    }
                }
            }
        });
        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            //$scope.LoadPage();
        });
    }

    //Quyết toán Trừ nợ tồn đơn vị - #truno #tondonvi
    $scope.btnQuyetToanTruNoTonDV = function () {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/KeHoachCapPhat/_QuyetToanTruNoTonDV',
            controller: 'QuyetToanTruNoTonDV',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                data: function () {
                    return {
                        nam: $scope.cbbNam,
                        ky: $scope.cbbKy
                    }
                }
            }
        });
        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            //$scope.LoadPage();
        });
    }
    //Thêm mới Trừ nợ tồn đơn vị - #truno #tondonvi
    $scope.btnThemMoiQTTruNoTonDV = function () {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/KeHoachCapPhat/_ThemMoiQTTruNoTonDV',
            controller: 'ThemMoiQTTruNoTonDV',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                data: function () {
                    return {
                        nam: $scope.cbbNam,
                        ky: $scope.cbbKy
                    }
                }
            }
        });
        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            //$scope.LoadPage();
        });
    }


    $scope.btnQtPhieuHoiVe = function () {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/KeHoachCapPhat/_QuyetToanPhieuHoiVe',
            controller: 'QuyetToanPhieuHoiVe',
            size: 'lg',
            backdrop: 'static'
        });
        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            //$scope.LoadPage();
        });
    }
    //#endregion

    //#region Thêm mới kế hoạch 
    $scope.btnKHThemMoi = function () {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/KeHoachCapPhat/_ThemMoiKeHoach',
            controller: 'ThemMoiKeHoach',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                data: function () {
                    return {
                        nam: $scope.cbbNam,
                        ky: $scope.cbbKy,
                        dataKhCap: $scope.ListKHCapPhat,
                        dataKhChoXl: $scope.ListKHCapPhatChoXl
                    }
                }
            }
        });
        //kết quả trả về của modal khi gọi sự kiện đóng poup.
        modalInstance.result.then(function (khchitiet) {
            if (khchitiet !== undefined) {
                $scope.CP_SelectKeHoachItem = khchitiet;
                $scope.cbbNam = khchitiet.NAM;
                $scope.cbbKy = khchitiet.KY;

                $.ajax({
                    type: 'post',
                    async: false,
                    url: '/KeHoachCapPhat/GetDsKeHoachCapPhat',
                    data: { dvId: $scope.DON_VI_ID, iNam: $scope.cbbNam, iKy: $scope.cbbKy },
                    success: function (data) {
                        hideLoading();
                        if (data.status) {
                            $scope.ListKHCapPhat = data.data.filter(x => x.TRANG_THAI !== 'P');
                            $scope.ListKHCapPhatChoXl = data.data.filter(x => x.TRANG_THAI === 'P');
                            $scope.selectedRowKeHoach = $scope.ListKHCapPhat.ID_KH;
                            //$scope.$apply();
                        }
                    }
                });
                $rootScope.ViewKeHoach = true;
                $scope.IsVisibleThemMoiKH = false;
            }
        });
    };
    //Danh sách kế hoạch theo ĐV, Năm, Kỳ
    $scope.LoadDsKeHoach = function (dvID, nam, ky) {
        $scope.selectedRowKeHoach = -1;
        $scope.ListKHCapPhat = [];
        $scope.ListKHCapPhatChoXl = [];
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoachCapPhat/GetDsKeHoachCapPhat',
            data: { dvId: dvID, iNam: nam, iKy: ky },
            success: function (data) {
                hideLoading();
                if (data.status) {
                    if (data.data.length > 0) {
                        $scope.ListKHCapPhat = data.data.filter(x => x.TRANG_THAI !== 'P');
                        $scope.ListKHCapPhatChoXl = data.data.filter(x => x.TRANG_THAI === 'P');
                        //$scope.$apply(); 
                    }
                    else {
                        $scope.ListKHCapPhat = [];
                        $scope.ListKHCapPhatChoXl = [];
                    }

                }
            }
        });
    };


    //Xem kế hoạch chi tiết
    $scope.CP_SelectKeHoachItem = {};
    $rootScope.CP_KeHoachDaChon = {};
    $scope.id_dv = 0;
    $scope.CP_SelectKeHoach = function (item, index) {
        showToast();
        $scope.LPXK_ListPhieuXuatKho = [];
        $scope.LPXK_ListGKHSX = [];
        $scope.id_dv = item.ID_DON_VI;
        $scope.selectedRowKeHoach = item.ID_KH;
        $scope.ListDoiTC = [];
        $scope.GetSpLoaiTCCapPhatTD(item.ID_KH, item.ID_DON_VI, item.NAM, item.KY);
        //Nút chức năng 
        $scope.hdChonLanhDaoPheDuyetKH = false;
        $scope.hdDuyetKeHoach = false;
        $scope.hdTuChoiDuyetKH = false;

        if (item.TRANG_THAI !== 'P') {
            $rootScope.ViewKeHoach = true;
            $scope.CP_SelectKeHoachItem = item;
            $rootScope.CP_KeHoachDaChon = item;
            $scope.LoadDsCapUngTieuChuan(item.ID_KH, item.ID_DON_VI, item.NAM, item.KY);
            $scope.LoadDsCapBoSungTC(item.ID_KH, item.ID_DON_VI, item.NAM, item.KY);

            //Load Quyết toán Trả Nợ
            //$scope.LoadDsDaQuyetToanByKeHoach(item.ID_KH);
            $scope.LoadDsDaQuyetToanByKeHoach(item.ID_DON_VI, item.NAM);
            //Load Quyết toán Nợ Tồn Đơn vị #trunoton
            //$scope.LoadDsDaQuyetToanTruNoTonDVByKeHoach(item.ID_KH);
            $scope.LoadDsDaQuyetToanTruNoTonDVByKeHoach(item.ID_DON_VI, item.NAM);

            LayDanhSachPhieuXuatKho();
            if ((item.TRANG_THAI === 'I' || item.TRANG_THAI === 'R') && item.CheckBtnNguoiGuiPheDuyet) {
                $scope.hdChonLanhDaoPheDuyetKH = true;
                $scope.hdDuyetKeHoach = false;
                $scope.hdTuChoiDuyetKH = false;
            }
            else if (item.TRANG_THAI === 'U') {

                if (item.CheckBtnNguoiDuyetKH) {
                    $scope.hdDuyetKeHoach = true;
                    $scope.hdTuChoiDuyetKH = true;
                }
                $scope.hdChonLanhDaoPheDuyetKH = false;
            }
            else {
                $scope.hdDuyetKeHoach = false;
                $scope.hdTuChoiDuyetKH = false;
                $scope.hdChonLanhDaoPheDuyetKH = false;
                //Disable các nút chức năng khi đã duyệt KH
                $scope.disableKHDuyet = true;
            }
        }

        else {
            toastr.error("Xin vui lòng đổi lại trạng thái để xử lý tiếp!");
        }

    };

    /** #quyettoan
     * Load danh sách ĐÃ quyết toán theo Kế hoạch
     * */
    $rootScope.dsQuyetToanNoKeHoachTruoc = [];
    $scope.LoadDsDaQuyetToanByKeHoach = function (idDonVi, iNam) {
        $scope.dsSPDaChonQuyetToan = [];

        $.ajax({
            type: 'GET',
            async: false,
            url: '/KeHoachCapPhat/GetDaQuyetToanByKeHoach',
            data: { idDonVi: idDonVi, iNam: iNam },
            //data: { idKeHoach: idKeHoach },
            success: function (res) {
                hideLoading();
                if (res.status) {
                    //console.log(res.data);
                    if (res.data.length > 0) {
                        $scope.dsSPDaChonQuyetToan = res.data;
                        $rootScope.dsQuyetToanNoKeHoachTruoc = res.data;
                        $scope.disableQuyetToanTraNo = false;
                    }
                    else {
                        $scope.disableQuyetToanTraNo = true;
                        $scope.dsSPDaChonQuyetToan = [];
                    }
                    //$scope.$apply();
                }
            }
        });
    }

    /**
     * Load danh sách Đã Quyết toán Trừ Nợ Tồn Đơn vị theo Kế hoạch #trunoton
     **/
    $scope.LoadDsDaQuyetToanTruNoTonDVByKeHoach = function (idDonVi, iNam) {
        //console.log(idKeHoach);
        $.ajax({
            type: 'GET',
            async: false,
            url: '/KeHoachCapPhat/GetDsDaQuyetToanTruNoTonDVByKeHoach',
            data: { idDonVi: idDonVi, iNam: iNam },
            success: function (res) {
                hideLoading();
                if (res.status) {
                    //console.log(res.data);
                    if (res.data.length > 0) {
                        $scope.dsSPQuyetToanNoTonDV = res.data;
                        //$rootScope.dsQuyetToanNoKeHoachTruoc = res.data;
                        //$scope.disableQuyetToanTraNo = false;
                    }
                    else {
                        //$scope.disableQuyetToanTraNo = true;
                        $scope.dsSPQuyetToanNoTonDV = [];
                    }
                    //$scope.$apply();
                }
            }
        });
    }

    $scope.idx_ChangeKHTheoNam = function () {
        $rootScope.ViewKeHoach = false;
        //Danh sách kế hoạch cấp phát
        $scope.LoadDsKeHoach($scope.DON_VI_ID, $scope.cbbNam, $scope.cbbKy);
    }

    $scope.idx_ChangeKHTheoKy = function () {
        $rootScope.ViewKeHoach = false;
        //Danh sách kế hoạch cấp phát
        $scope.LoadDsKeHoach($scope.DON_VI_ID, $scope.cbbNam, $scope.cbbKy);
    }
    //Cấp ứng TC
    $scope.LoadDsCapUngTieuChuan = function (khId, dvId, nam, ky) {
        $scope.ListCapUngTC = [];
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoachCapPhat/GetDsCapUngTieuChuan',
            data: { khId: khId, dvId: dvId, iNam: nam, iKy: ky, loaiKh: 'BX' },
            success: function (data) {
                hideLoading();
                if (data.status) {
                    angular.forEach(data.data, function (val, key) {
                        $scope.ListCapUngTC.push({ ID_KH_CT: val.ID_KH_CT, TrangPhuc: val.ID_SP, SoLuong: val.SL_TONG });
                    })
                    //$scope.$apply();
                }
            }
        });
    };
    //Cấp bổ sung TC
    $scope.LoadDsCapBoSungTC = function (khId, dvId, nam, ky) {
        $scope.ListCapBoSungTC = [];
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoachCapPhat/GetDsCapUngTieuChuan',
            data: { khId: khId, dvId: dvId, iNam: nam, iKy: ky, loaiKh: 'UBX' },
            success: function (data) {
                hideLoading();
                if (data.status) {
                    angular.forEach(data.data, function (val, key) {
                        $scope.ListCapBoSungTC.push({ ID_KH_CT: val.ID_KH_CT, TrangPhuc: val.ID_SP, SoLuong: val.SL_TONG });
                    })
                    //$scope.$apply();
                }
            }
        });
    };
    //#endregion 
    $scope.btnDmCapUngBoXung = function () {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/KeHoachCapPhat/_BoxDmCapUngBoXung',
            controller: 'BoxDmCapUngBoXung',
            size: 'lg',
            backdrop: 'static'
        });
        modalInstance.result.then(function (response) {
            //$scope.LoadPage();
        });
    };
    $scope.btnQtNoKeHoachTruoc = function () {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/KeHoachCapPhat/_BoxDmQuyetToan',
            controller: 'BoxDmQuyetToan',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                data: function () {
                    return {
                        nam: $scope.cbbNam,
                        ky: $scope.cbbKy
                    }
                }
            }
        });
        modalInstance.result.then(function (response) {
            //$scope.LoadPage();
        });
    };
    //#region Cấp ứng tiêu chuẩn /////////////////////////////////////////////////////////
    $scope.ListCapUngTC = [];
    $scope.ListCapBoSungTC = [];
    $rootScope.listSanPhamTruNoModal = [];

    angular.element(document).ready(function () {
        $.ajax({
            type: 'GET',
            url: '/KeHoachCapPhat/GetDanhMucTrangPhuc',
            success: function (data) {
                if (data.status) {
                    $scope.listSanPhamModal = data.data;
                    $rootScope.listSanPhamTruNoModal = data.data;
                } else {
                    toastr.error(data.message);
                }
                $scope.$apply();
                hideLoading();
            }
        });
        //Get phương thức vận chuyển
        $scope.callAPI('GET', 'GetDanhMucPhuongThucVanChuyen', {}, $scope.phuongThucVanChuyenListResponse);
    })

    $scope.phuongThucVanChuyenListResponse = (data) => {
        $scope.listPhuongThucVanChuyen = data.data;
        $scope.listPhuongThucVanChuyen.unshift({ FLEX_VALUE_ID: "", DESCRIPTION: "--Chọn phương thức --" })
    }

    // #region Ultil
    $scope.callAPI = (type, url, data, cb, opts) => {
        $.ajax({
            type: type,
            url: `/KeHoachCapPhat/${url}`,
            data: data,
            success: function (data) {
                if (data.status) {
                    cb(data)
                } else {
                    toastr.error(data.message);
                }
                if (opts && opts.hideLoading) {
                    hideLoading();
                }
                $scope.$apply();
            }
        });
    }

    $scope.btnPhuongThucVC = function (khId, ptcId, lyDo) {
        debugger;
        if (ptcId == undefined || ptcId <= 0) {
            toastr.error("Bạn chưa chọn phương thức vận chuyển!");
            return false;
        }
        if (lyDo == undefined || lyDo == '') {
            toastr.error("Bạn chưa nhập lý do cấp!");
            return false;
        }
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoachCapPhat/AddLyDoPhuongPhucVC',
            data: { khId: khId, ptcId: ptcId, lyDo: lyDo },
            success: function (data) {
                hideLoading();
                if (data.status) {
                    toastr.success(data.message);
                }
                else {
                    toastr.error(data.message);
                }
            }
        });
    };

    $scope.btnThemMoiCUTC = function () {
        $scope.ListCapUngTC.push({ ID_KH_CT: 0, TrangPhuc: undefined, SoLuong: 0 });
    }
    $scope.btnThemMoiTheoDs = function (loaiTC) {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/KeHoachCapPhat/_BoxDmTrangPhuc',
            controller: 'BoxDmTrangPhuc',
            size: 'lg',
            backdrop: 'static'
        });
        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            if (response !== undefined) {
                if (loaiTC === 'BX') {
                    angular.forEach(response, function (val, key) {
                        $scope.ListCapUngTC.push({ ID_KH_CT: 0, TrangPhuc: val.ID, SoLuong: 0 });
                    })
                }
                else {
                    angular.forEach(response, function (val, key) {
                        $scope.ListCapBoSungTC.push({ ID_KH_CT: 0, TrangPhuc: val.ID, SoLuong: 0 });
                    })
                }
            }
        });
    };




    //Đăng ký nhận tiền
    $scope.btnDSDKNhanTien = function () {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/KeHoachCapPhat/_BoxDangKyNhanTien',
            controller: 'BoxDangKyNhanTien',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                data: function () {
                    return {
                        khId: $scope.selectedRowKeHoach,
                        nam: $scope.cbbNam,
                        ky: $scope.cbbKy,
                        dvId: $scope.DON_VI_ID
                    }
                }
            }
        });
        //kết quả trả về của modal
        modalInstance.result.then(function () {

        });
    };
    //Luu thông tin sản phẩm cấp ứng
    $scope.LuuCapUngTieuChuan = function (loaiTC) {
        if (loaiTC === 'BX') {
            var dataSubmit = $scope.ListCapUngTC;
            if (dataSubmit.length > 0) {
                $scope.ListInsert = [];
                $scope.ListUpdate = [];
                angular.forEach(dataSubmit, function (value, key) {
                    var k = key + 1;
                    $scope.itemModel = {};
                    $scope.itemModel.ID_KH_CT = value.ID_KH_CT ?? 0;
                    $scope.itemModel.SP_ID = value.TrangPhuc;
                    $scope.itemModel.SL_TONG = value.SoLuong ?? 0;

                    if (value.ID_KH_CT > 0)
                        $scope.ListUpdate.push($scope.itemModel);
                    else
                        $scope.ListInsert.push($scope.itemModel);
                });
                if ($scope.ListUpdate.length > 0 || $scope.ListInsert.length > 0) {
                    $.ajax({
                        type: 'post',
                        url: '/KeHoachCapPhat/ThemCapBoXungTC',
                        data: { listThem: $scope.ListInsert, listSua: $scope.ListUpdate, khId: $scope.selectedRowKeHoach, loaiKh: loaiTC },
                        success: function (res) {
                            if (res.Error) {
                                toastr.error(res.Title);
                            } else {
                                toastr.success(res.Title);
                            }
                        }
                    })
                }
                else {
                    toastr.error("Danh sách trống xin vui lòng kiểm tra lại!");
                }
            }
        }
        else if (loaiTC === 'UBX') {
            var dataCb = $scope.ListCapBoSungTC;
            if (dataCb.length > 0) {
                $scope.ListInsert = [];
                $scope.ListUpdate = [];
                angular.forEach(dataCb, function (value, key) {
                    var k = key + 1;
                    $scope.itemModel = {};
                    $scope.itemModel.ID_KH_CT = value.ID_KH_CT ?? 0;
                    $scope.itemModel.SP_ID = value.TrangPhuc;
                    $scope.itemModel.SL_TONG = value.SoLuong ?? 0;

                    if (value.ID_KH_CT > 0)
                        $scope.ListUpdate.push($scope.itemModel);
                    else
                        $scope.ListInsert.push($scope.itemModel);
                });
                if ($scope.ListUpdate.length > 0 || $scope.ListInsert.length > 0) {
                    $.ajax({
                        type: 'post',
                        url: '/KeHoachCapPhat/ThemCapBoXungTC',
                        data: { listThem: $scope.ListInsert, listSua: $scope.ListUpdate, khId: $scope.selectedRowKeHoach, loaiKh: loaiTC },
                        success: function (res) {
                            if (res.Error) {
                                toastr.error(res.Title);
                            } else {
                                toastr.success(res.Title);
                            }
                        }
                    })
                }
                else {
                    toastr.error("Danh sách trống xin vui lòng kiểm tra lại!");
                }
            }
        }
        else {
            toastr.error("Lưu thất bại!");
        }
    }
    $scope.HuyThayDoiCPUTC = function (loaiTC) {
        if (loaiTC === 'BX')
            $scope.LoadDsCapUngTieuChuan($scope.selectedRowKeHoach, $scope.DON_VI_ID, $scope.cbbNam, $scope.cbbKy);
        else
            $scope.LoadDsCapBoSungTC($scope.selectedRowKeHoach, $scope.DON_VI_ID, $scope.cbbNam, $scope.cbbKy);
    }
    $scope.CheckAllCapUng = function () {
        angular.forEach($scope.ListCapUngTC, function (item) {
            item.Selected = event.target.checked;
        });
    };
    $scope.CheckAllCapBoSungTC = function () {
        angular.forEach($scope.ListCapBoSungTC, function (item) {
            item.Selected = event.target.checked;
        });
    };
    $scope.XoaThayDoiCPUTC = function (loaiTC) {
        $scope.ListDelete = [];
        if ($scope.ListCapUngTC.length > 0 || $scope.ListCapBoSungTC.length > 0) {

            angular.forEach($scope.ListCapUngTC, function (item) {
                if (item.Selected === true) {
                    $scope.ListDelete.push(item.ID_KH_CT);
                }
            });
            angular.forEach($scope.ListCapBoSungTC, function (item) {
                if (item.Selected === true) {
                    $scope.ListDelete.push(item.ID_KH_CT);
                }
            });

            if ($scope.ListDelete.length > 0) {
                $.ajax({
                    type: 'post',
                    url: '/KeHoachCapPhat/XoaCapBoXungTC',
                    data: { listXoa: $scope.ListDelete },
                    success: function (res) {
                        if (res.Error) {
                            toastr.error(res.Title);
                        } else {
                            toastr.success(res.Title);
                            if (loaiTC === 'BX')
                                $scope.LoadDsCapUngTieuChuan($scope.selectedRowKeHoach, $scope.DON_VI_ID, $scope.cbbNam, $scope.cbbKy);
                            else
                                $scope.LoadDsCapBoSungTC($scope.selectedRowKeHoach, $scope.DON_VI_ID, $scope.cbbNam, $scope.cbbKy);
                            $scope.$apply();
                        }
                    }
                });
            }
            else
                toastr.error("Bạn chưa chọn trang phục cần xóa.");
        }
    };


    //Tab đổi tiêu chuẩn


    $scope.OpenTabNTDoiTC = function () {
        $scope.ListDoiTC = [];
        $scope.ListSanPhamDich = [];
        //Danh sách đổi tiêu chuẩn
        $scope.GetDSDoiTieuChuan($scope.selectedRowKeHoach, $scope.DON_VI_ID, $scope.cbbNam, $scope.cbbKy);
        //Danh sách nhận tiền
        $scope.GetDsNhanTien($scope.selectedRowKeHoach, $scope.DON_VI_ID, $scope.cbbNam, $scope.cbbKy);
    }

    $scope.GetDSDoiTieuChuan = function (khId, dvId, nam, ky) {
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoachCapPhat/GetDsDoiTieuChuan',
            data: { khId: khId, dvId: dvId, iNam: nam, iKy: ky },
            success: function (data) {
                hideLoading();
                if (data.status) {
                    if (data.data !== undefined) {
                        $scope.ListDoiTC = data.data;
                    }
                }
            }
        });
    }
    //Danh sách được nhận (SP đích)
    $scope.GetSanPhamDoiCT = function (spId) {
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoachCapPhat/DS_SPDoi_ChiTiet',
            data: { spNguonId: spId },
            success: function (data) {
                hideLoading();
                if (data.status && data.data !== undefined) {
                    $scope.ListSanPhamDich = data.data;
                    angular.forEach($scope.ListSanPhamDich, function (val) {
                        $scope.SLSanPhamDoi = val.SL_DOI;
                    });

                }
            }
        });
    }

    $scope.LuuDanhSachDoiTC = function () {
        if ($scope.ListDoiTC.length > 0) {
            var dataSubmit = $scope.ListDoiTC;
            if (dataSubmit.length > 0) {
                $scope.ListInsert = [];
                $scope.ListUpdate = [];
                angular.forEach(dataSubmit, function (value, key) {
                    var k = key + 1;
                    $scope.itemModel = {};
                    $scope.itemModel.ID_KH_TH = value.ID_KH_TH ?? 0;
                    $scope.itemModel.ID_KH = value.ID_KH ?? $scope.selectedRowKeHoach;
                    $scope.itemModel.ID_SP = value.ID_SP;
                    $scope.itemModel.TEN_SP = value.TEN_SP;
                    $scope.itemModel.IS_SP_CO_SO = 'N';
                    $scope.itemModel.ID_SP_CHA = value.ID_SP_CHA ?? 0;
                    $scope.itemModel.NAM = value.NAM;
                    $scope.itemModel.KY = value.KY;
                    $scope.itemModel.SL_KE_HOACH = value.SL_KE_HOACH ?? 0;
                    $scope.itemModel.SL_NO_KY_TRUOC = value.SL_NO_KY_TRUOC ?? 0;
                    $scope.itemModel.SL_KY_NAY = value.SL_DOI ?? 0;
                    $scope.itemModel.SL_NHAN_TIEN = value.SL_NHAN_TIEN ?? 0;
                    $scope.itemModel.IS_MAY_DO = value.IS_MAY_DO;

                    if (value.ID_KH_TH > 0)
                        $scope.ListUpdate.push($scope.itemModel);
                    else
                        $scope.ListInsert.push($scope.itemModel);
                });
                if ($scope.ListInsert.length > 0 || $scope.ListUpdate.length > 0) {
                    $.ajax({
                        type: 'post',
                        url: '/KeHoachCapPhat/LuuDanhSachDoiTC',
                        data: { listThem: $scope.ListInsert, listSua: $scope.ListUpdate },
                        success: function (res) {
                            if (res.Error) {
                                toastr.error(res.Title);
                            } else {
                                toastr.success(res.Title);
                            }
                        }
                    })
                }
                else {
                    toastr.error("Danh sách đã được cập nhật!");
                }
            }
        }
        else {
            toastr.error("Danh sách trống xin vui lòng kiểm tra lại!");
        }
    }

    $scope.HuyDanhSachDoiTC = function () {
        $scope.ListDoiTC = [];
        $scope.ListSanPhamDich = [];
        //Danh sách đổi tiêu chuẩn
        $scope.GetDSDoiTieuChuan($scope.selectedRowKeHoach, $scope.DON_VI_ID, $scope.cbbNam, $scope.cbbKy)
    }

    $scope.DanhSachSPDoiTC = function () {
        $scope.strSanPhamId = '';
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/KeHoachCapPhat/_BoxDanhSachSPDoiTC',
            controller: 'BoxDanhSachSPDoiTC',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                datamodel: function () {
                    return {
                        khId: $scope.selectedRowKeHoach,
                        nam: $scope.cbbNam,
                        ky: $scope.cbbKy,
                        dvId: $scope.DON_VI_ID
                    }
                }
            }
        });
        modalInstance.result.then(function (response) {
            if (response !== undefined) {
                angular.forEach(response, function (val, key) {
                    if ($scope.ListDoiTC.filter(x => x.ID_SP === val.ID_SP).length <= 0)
                        $scope.ListDoiTC.push({ ID_KH_TH: 0, TEN_SP: val.TEN_SP, SL_KE_HOACH: val.SL_KE_HOACH, SL_DOI: val.SL_DOI, ID_SP: val.ID_SP });
                    //$scope.strSanPhamId += val.ID_SP + ', ';
                })
            }
            $scope.ListSanPhamDich = [];
            if ($scope.ListDoiTC.length > 0) {

                // Lấy danh sách sp nhận
                angular.forEach($scope.ListDoiTC, function (val, key) {
                    $scope.GetSanPhamDoiCTAll(val.ID_SP);
                })
                if ($scope.ListSanPhamDich.length > 0) {
                    angular.forEach($scope.ListSanPhamDich, function (val) {
                        $scope.SLSanPhamDoi = val.SL_DOI;
                    });
                }
            }
        });
    }

    $scope.GetSanPhamDoiCTAll = function (spId) {
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoachCapPhat/DS_SPDoi_ChiTiet',
            data: { spNguonId: spId },
            success: function (data) {
                hideLoading();
                if (data.status && data.data !== undefined) {
                    angular.forEach(data.data, function (val, key) {

                        var checkSP = $scope.ListSanPhamDich.filter(x => x.ID_SP === val.ID_SP);
                        if (checkSP != null && checkSP.length > 0) {
                            angular.forEach($scope.ListSanPhamDich, function (spn, key) {
                                if (spn.ID_SP == val.ID_SP) {
                                    spn.SL_DOI += val.SL_DOI;
                                }
                            });
                        } else {
                            $scope.ListSanPhamDich.push(val);
                        }
                    })
                }
            }
        });
    }

    $scope.SLSanPhamDoi = 0;
    $scope.CP_SelectSpDoi = function (item, index) {
        //Lấy danh sách SP đổi 
        if (item.ID_SP > 0) {
            $scope.GetSanPhamDoiCT(item.ID_SP);
        }
    }
    $scope.CP_DoiTCChange = function (slDoi) {
        angular.forEach($scope.ListSanPhamDich, function (val, key) {
            val.SL_DOI = slDoi * $scope.SLSanPhamDoi;
        });
    }

    //#endregion  End cấp ứng tiêu chuẩn
    //#region =================================Danh sách SP loại cấp phát tự động==================================

    // Có hiển thị cột checkbox ở đầu không
    $scope.tree_Table_Checkbox = false;
    // Model data
    //$scope.tree_data = [];
    $rootScope.tree_data = [];

    $scope.GetSpLoaiTCCapPhatTD = function (khId, dvId, nam, ky) {
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoachCapPhat/GetSpLoaiTCCapPhatTD',
            data: { khId: khId, dvId: dvId, iNam: nam, iKy: ky },
            success: function (data) {
                if (data.status) {
                    //$scope.tree_data = data.data;
                    $rootScope.tree_data = data.data;
                }
                //else {
                //    toastr.error(data.message);
                //}
                hideLoading();
            }
        });
    };

    // cấu hình hiển thị cùng với icon
    $scope.expanding_property = {
        field: "TreeName",
        displayName: "Trang phục",
        width: "60%",
        rowspan: 1
    };
    // Cấu hình các cột sẽ hiển thị tiếp theo
    $scope.col_defs = [[
        { field: "TreeSoLuong", displayName: "Số lượng cấp ", width: "40%", rowspan: 1, colspan: 1 },
    ]];
    $scope.body_defs = [{ field: "TreeSoLuong" }];
    // event when change value checkbox
    $scope.ChangeTreeTableCheckbox = function (item) {
    };
    // event when click choose row
    $scope.ClickTreeTableRow = function (item) {
        console.log(item);
    };

    //#endregion  ===========================Danh sách SP loại cấp phát tự động===========================

    //#region //////////////////////Gửi phê duyệt/////////////////////////////////
    $scope.ChonLanhDaoPheDuyetKH = function () {
        $scope.strSanPhamId = '';
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/KeHoachCapPhat/_BoxLanhDaoPheDuyet',
            controller: 'BoxLanhDaoPheDuyet',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                datamodel: function () {
                    return {
                        khId: $scope.selectedRowKeHoach,
                        nam: $scope.cbbNam,
                        ky: $scope.cbbKy,
                        dvId: $scope.DON_VI_ID
                    }
                }
            }
        });
        modalInstance.result.then(function (response) {
            if (response === 1) {
                $scope.hdChonLanhDaoPheDuyetKH = false;
                $scope.LoadDsKeHoach($scope.DON_VI_ID, $scope.cbbNam, $scope.cbbKy);
            }
        });
    }
    $scope.DuyetKeHoach = function () {
        if ($scope.selectedRowKeHoach > 0) {
            $.ajax({
                type: 'post',
                async: false,
                url: '/KeHoachCapPhat/DuyetKeHoach',
                data: { khId: $scope.selectedRowKeHoach },
                success: function (data) {
                    if (data.status) {
                        toastr.success(data.message);
                        $scope.hdDuyetKeHoach = false;
                        $scope.hdTuChoiDuyetKH = false;
                        $scope.LoadDsKeHoach($scope.DON_VI_ID, $scope.cbbNam, $scope.cbbKy);
                    } else {
                        toastr.error(data.message);
                    }
                    hideLoading();
                }
            });
        }
        else {
            toastr.error("Kế hoạch duyệt không hợp lệ. Xin vui lòng kiểm tra lại!");
        }
    }
    $scope.TuChoiDuyetKeHoach = function () {
        if ($scope.selectedRowKeHoach > 0) {
            $.ajax({
                type: 'post',
                async: false,
                url: '/KeHoachCapPhat/TuChoiPheDuyetKH',
                data: { khId: $scope.selectedRowKeHoach },
                success: function (data) {
                    if (data.status) {
                        toastr.success(data.message);
                        $scope.hdDuyetKeHoach = false;
                        $scope.hdTuChoiDuyetKH = false;
                        $scope.LoadDsKeHoach($scope.DON_VI_ID, $scope.cbbNam, $scope.cbbKy);
                    } else {
                        toastr.error(data.message);
                    }
                    hideLoading();
                }
            });
        }
        else {
            toastr.error("Kế hoạch hủy không hợp lệ. Xin vui lòng kiểm tra lại!");
        }
    }
    /////////////////////////////////// Lập phiếu xuất kho //////////////////////////////
    $scope.LPXK_ListPhieuXuatKho = [];
    $scope.LPXK_ListPXK_Detail = [];
    $scope.LPXK_ListGKHSX = [];
    $scope.LPXK_ListSPD = [];
    $scope.LPXK_Message = 'Xuất từ kho 2 (trong trường hợp kho ưu tiên hết hàng)';

    /// Xuất kho ưu tiên
    $scope.LPXK_XuatKhoUuTien = function () {
        // kiểm tra xem đã chọn kế hoạch chưa
        if ($scope.CP_SelectKeHoachItem != null && $scope.CP_SelectKeHoachItem.ID_KH > 0) {
            showToast();
            $.ajax({
                type: 'post',
                async: false,
                url: '/KeHoachCapPhat/LPXK_XuatKhoUuTien',
                data: { idKeHoach: $scope.CP_SelectKeHoachItem.ID_KH, donViId: $scope.DON_VI_ID },
                success: function (data) {
                    hideLoading();
                    if (data.obj.Error == false) {
                        toastr.success(data.obj.Title);
                    } else {
                        toastr.error(data.obj.Title);
                    }
                    LayDanhSachPhieuXuatKho();
                }
            });
        }
        else {
            toastr.error("Vui lòng chọn kế hoạch!");
        }
    }

    // Chọn tab laaph phiếu xuất kho

    $scope.ChooseTabLapPXK = function () {
        // kiểm tra xem đã chọn kế hoạch chưa
        if ($scope.CP_SelectKeHoachItem != null && $scope.CP_SelectKeHoachItem.ID_KH > 0) {
            LayDanhSachPhieuXuatKho();
        }
        else {
            toastr.error("Vui lòng chọn kế hoạch!");
        }
    }

    $scope.LPXK_ListGKHSX = [];
    $scope.LPXK_ListSPNKH = [];
    // Lấy danh sách phiếu xuất kho
    function LayDanhSachPhieuXuatKho() {
        $scope.LPXK_ListPhieuXuatKho = [];
        $scope.LPXK_ListPXK_Detail = [];
        $scope.LPXK_ListGKHSX = [];
        $scope.LPXK_ListSPNKH = [];
        showToast();
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoachCapPhat/LPXK_GetListPhieuXuatKho',
            data: { idKeHoach: $scope.CP_SelectKeHoachItem.ID_KH, donViId: $scope.DON_VI_ID },
            success: function (data) {
                hideLoading();
                if (data.pxTuKho != null && data.pxTuKho.length > 0) {
                    $scope.LPXK_ListPhieuXuatKho = data.pxTuKho;
                }
                if (data.pxTuHopDong != null && data.pxTuHopDong.length > 0) {
                    $scope.LPXK_ListGKHSX = data.pxTuHopDong;
                }

                if (data.SanPhamNoKH != null && data.SanPhamNoKH.length > 0) {
                    $scope.LPXK_ListSPNKH = data.SanPhamNoKH;
                }
            }
        });
    }
    $scope.LPXK_ListPXK_File = [];
    function LPXK_FNGetDetailPXK(pxkId) {
        $scope.LPXK_ListPXK_Detail = [];
        $scope.LPXK_ListPXK_File = [];
        showToast();
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoachCapPhat/LPXK_GetListPhieuXuatKhoChiTiet',
            data: { phieuXuatKhoId: pxkId, keyword: $scope.LPXK_TenHangText },
            success: function (data) {
                hideLoading();
                $('#LPXK_ChiTietPXK').modal('show');
                if (data.data != null && data.data.length > 0) {
                    $scope.LPXK_ListPXK_Detail = data.data;
                }
                if (data.files != null && data.files.length > 0) {
                    $scope.LPXK_ListPXK_File = data.files;
                    data.files.forEach(x => {
                        $scope.LPXK_ListPXK_File.push({
                            'src': `/KeHoachCapPhat/Image?imgPath=${x.IMG_PATH}`,
                            'checked': false,
                            'IMG_PATH': x.IMG_PATH,
                            'ID_TAB': x.ID_TAB,
                            'ID_TXN_REF': x.ID_TXN_REF,
                            'LOAI_PHIEU': x.LOAI_PHIEU,
                            'TRANG_THAI': x.TRANG_THAI
                        })
                    });
                }
            }
        });
    }

    $scope.LPXK_ShowDinhKem = (id) => {
        $('#kho_anh').modal('show');
    }

    function LPXK_FNGetDetailGiaoKHSX(pxkId) {
        $scope.LPXK_ListPXK_Detail = [];
        showToast();
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoachCapPhat/LPXK_GetListGiaoKHSXChiTiet',
            data: { phieuXuatKhoId: pxkId, keyword: $scope.LPXK_TenHangText },
            success: function (data) {
                hideLoading();
                $('#LPXK_ChiTietGiaoKHSX').modal('show');
                if (data.data != null && data.data.length > 0) {
                    $scope.LPXK_ListPXK_Detail = data.data;
                }
            }
        });
    }


    $scope.LPXK_TenHangText = "";
    $scope.LPXK_PhieuXuatKhoId = 0;
    $scope.LPXK_PhieuXuatKho = {};
    $scope.LPXK_ChiTietPXK = -1;
    $scope.LPXK_TitlePopupChiTiet = "";
    // Lấy danh sách chi tiết phiếu xuất kho
    $scope.LPXK_GetDetailPXK = function (isXuatKho, item) {
        $scope.LPXK_ChiTietPXK = isXuatKho;
        $scope.LPXK_PhieuXuatKho = {};
        $scope.LPXK_PhieuXuatKho = item;
        $scope.LPXK_PhieuXuatKhoId = item.ID;
        if (isXuatKho == 1) {
            $scope.LPXK_TitlePopupChiTiet = "Phiếu xuất kho từ: kho " + item.TEN_KHO_XUAT;
            LPXK_FNGetDetailPXK(item.ID);
        }
        else {
            $scope.LPXK_TitlePopupChiTiet = "Giao thẳng từ: HĐ số " + item.SO_HD;
            LPXK_FNGetDetailGiaoKHSX(item.ID);
        }


    }

    // Tìm kiếm hàng hóa
    $scope.LPXK_GetSearchDetailPXK = function () {
        if ($scope.LPXK_PhieuXuatKhoId > 0) {
            LPXK_FNGetDetailPXK($scope.LPXK_PhieuXuatKhoId);
        }
    }

    /// Xuất kho khác
    $scope.LPXK_XuatKhoKhac = function () {
        LayDanhSachKhoKhac();
        $('#LPXK_ChonKhoKhac').modal('show');
    }

    // Lấy danh sách kho khác
    function LayDanhSachKhoKhac() {
        $scope.LPXK_ListKho = [];
        showToast();
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoachCapPhat/LPXK_GetListKhoKhac',
            data: { keyword: $scope.LPXK_SearchKhoText, donViId: $scope.DON_VI_ID },
            success: function (data) {
                hideLoading();
                if (data.data != null && data.data.length > 0) {
                    $scope.LPXK_ListKho = data.data;
                }
            }
        });
    }

    $scope.LPXK_CheckAllKho = false;

    // Chọn chọn all kho
    $scope.LPXK_ChangeCheckAllKho = function () {
        for (var i = 0; i < $scope.LPXK_ListKho.length; i++) {
            $scope.LPXK_ListKho[i].SELECTED = $scope.LPXK_CheckAllKho;
        }
    };

    // Chọn kho
    $scope.LPXK_CheckedKho = function (index) {
        $scope.LPXK_CheckAllKho = false;
        var khoChons = $scope.LPXK_ListKho.filter(function (x) {
            return (x.SELECTED == true);
        });

        if (khoChons != null && khoChons.length == $scope.LPXK_ListKho.length) {
            $scope.LPXK_CheckAllKho = true;
        }
    };

    // Tạo xuất kho khác
    $scope.LPXK_TaoPhieuXuatKhoKhac = function () {
        // Lấy danh sách kho chọn
        var khoChons = $scope.LPXK_ListKho.filter(function (x) {
            return (x.SELECTED == true);
        });

        // kiểm tra xem đã chọn kế hoạch chưa
        if (khoChons != null && khoChons.length > 0) {
            if ($scope.CP_SelectKeHoachItem != null && $scope.CP_SelectKeHoachItem.ID_KH > 0) {
                showToast();
                $.ajax({
                    type: 'post',
                    async: false,
                    url: '/KeHoachCapPhat/LPXK_XuatKhoKhongUuTien',
                    data: { idKeHoach: $scope.CP_SelectKeHoachItem.ID_KH, donViId: $scope.DON_VI_ID, khos: khoChons },
                    success: function (data) {
                        hideLoading();
                        if (data.Error == false) {
                            toastr.success("Thêm phiếu xuất kho thành công!");
                            LayDanhSachPhieuXuatKho();
                        } else {
                            toastr.error(data.Title);
                        }
                    }
                });
            }
            else {
                toastr.error("Vui lòng chọn kế hoạch!");
            }

        }
        else {
            toastr.error("Vui lòng chọn kho!");
        }
    }
    $scope.LPXK_IsXuatThangTuHD = -1;

    /// Xuất giao tay
    $scope.LPXK_XuatGiaoTay = function () {
        $scope.LPXK_IsXuatThangTuHD = 0;
        LayDanhSachHopDong(0);
    }

    /// Xuất thẳng từ hợp đồng
    $scope.LPXK_XuatThangTuHD = function () {
        $scope.LPXK_IsXuatThangTuHD = 1;
        LayDanhSachHopDong(1);
    }

    // Lấy danh sách kho khác
    function LayDanhSachHopDong(isXuatThangTuHD) {
        $scope.LPXK_ListHopDong = [];
        showToast();
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoachCapPhat/LPXK_LayDSHopDong',
            data: { donViId: $scope.DON_VI_ID, keHoachId: $scope.CP_SelectKeHoachItem.ID_KH, keyword: $scope.LPXK_SearchKhoText, isXuatThangTuHD: isXuatThangTuHD },
            success: function (data) {
                hideLoading();
                if (data.data != null && data.data.length > 0) {
                    $scope.LPXK_ListHopDong = data.data;
                    $('#LPXK_ChonHopDong').modal('show');
                } else {
                    toastr.error("Không có hợp đồng nào phù hợp hoặc trang phục đã được xuất kho hết!");
                }
            }
        });
    }

    // Xuất kho từ hợp đồng
    $scope.LPXK_XuatGiaoThangTuHD = function () {
        // Kiểm tra xem đã chọn hợp đồng chưa
        var check = false;
        for (var i = 0; i < $scope.LPXK_ListHopDong.length; i++) {
            for (var j = 0; j < $scope.LPXK_ListHopDong[i].children.length; j++) {
                if ($scope.LPXK_ListHopDong[i].children[j].selected) {
                    check = true;
                    break;
                }
            }
            if (check) {
                break;
            }
        }
        if (check) {
            if ($scope.CP_SelectKeHoachItem != null && $scope.CP_SelectKeHoachItem.ID_KH > 0) {
                showToast();
                $.ajax({
                    type: 'post',
                    async: false,
                    url: '/KeHoachCapPhat/LPXK_XuatKhoTheoHopDong',
                    data: { idKeHoach: $scope.CP_SelectKeHoachItem.ID_KH, donViId: $scope.DON_VI_ID, hopDongs: $scope.LPXK_ListHopDong, isXuatThangTuHD: $scope.LPXK_IsXuatThangTuHD },
                    success: function (data) {
                        hideLoading();
                        if (data.Error == false) {
                            toastr.success(data.Title);
                            $('#LPXK_ChonHopDong').modal('hide');
                            LayDanhSachPhieuXuatKho();
                        } else {
                            toastr.error(data.Title);
                        }
                    }
                });
            }
            else {
                toastr.error("Vui lòng chọn kế hoạch!");
            }
        }
        else {
            toastr.error("Vui lòng chọn hợp đồng!");
        }
    }

    /////////////////////////////////// End Lập phiếu xuất kho //////////////////////////////


    ////////////////////////////////////| Duyệt xuất kho //////////////////////////////////////

    /// Duyệt Xuất kho ưu tiên
    $scope.DuyetKeHoachXuatKkho = function () {
        // kiểm tra xem đã chọn kế hoạch chưa
        if ($scope.CP_SelectKeHoachItem != null && $scope.CP_SelectKeHoachItem.ID_KH > 0) {
            showToast();
            $.ajax({
                type: 'post',
                async: false,
                url: '/KeHoachCapPhat/LPXK_DuyetXuatKho',
                data: { idKeHoach: $scope.CP_SelectKeHoachItem.ID_KH },
                success: function (data) {
                    hideLoading();
                    if (data.obj.Error == false) {
                        toastr.success(data.obj.Title);
                    } else {
                        toastr.error(data.obj.Title);
                    }
                }
            });
        }
        else {
            toastr.error("Vui lòng chọn kế hoạch!");
        }
    }




    ////////////////////////////////////// End Duyệt xuất kho ////////////////////////////////////////


    var settingLKH_LTC_NTC = {
        check: {
            enable: false
        },
        data: {
            simpleData: {
                enable: true,
                idKey: "id",
                pIdKey: "pId",
                rootPId: ""
            }
        },
        callback: {
            onClick: onClickLKH_LTC_NTC
        }
    };
    $scope.NH_LoaiKHId = 0;
    $scope.NH_LoaiTCId = 0;
    $scope.NH_NhomTCId = 0;
    $scope.NH_LucLuongId = 0;
    function onClickLKH_LTC_NTC(event, treeId, treeNode, clickFlag) {
        $scope.NH_LoaiKHId = 0;
        $scope.NH_LoaiTCId = 0;
        $scope.NH_NhomTCId = 0;
        // Lấy danh sách tiêu chuẩn của nhóm tiêu chuẩn đã chọn
        if (treeNode != null) {
            if (treeNode.name_control == "NHOMTC") {
                $scope.NH_LoaiTCId = parseInt(treeNode.LoaiTCId);
                $scope.NH_NhomTCId = parseInt(treeNode.NhomTCId);
                TimKiemNienHan();
            }

            if (treeNode.name_control == "LOAITC") {
                $scope.NH_LoaiTCId = parseInt(treeNode.LoaiTCId);
                $scope.NH_NhomTCId = 0;
                TimKiemNienHan();
            }
        }
        $scope.$apply();
    }

    function TimKiemNienHan() {
        if ($scope.NH_LoaiTCId > 0 || $scope.NH_NhomTCId > 0 || $scope.NH_LucLuongId > 0) {
            showToast();
            $scope.dsTieuChuan = [];
            $.ajax({
                type: 'post',
                url: '/KeHoachCapPhat/TimKiemNienHan',
                data: {
                    id_dv: $scope.DON_VI_ID,
                    nam: parseInt($scope.cbbNam),
                    loaiTCId: $scope.NH_LoaiTCId,
                    nhomTCId: $scope.NH_NhomTCId,
                    lucLuongId: $scope.NH_LucLuongId
                },
                success: function (data) {
                    hideLoading();
                    $scope.dsTieuChuan = data.nienhan;
                    $scope.$apply();
                }
            });
        }

    }

    $scope.ListLKH_LTC_NTC = [];
    var treeLucLuong;

    var treeLucLuongSource = [];

    // cấu hình niên hạn đơn vị
    $scope.btnNienHan = function () {
        $scope.dsSuaNienHan = [];
        $scope.dsTieuChuan = [];
        $.ajax({
            type: 'post',
            //headers: {
            //    'Access-Control-Allow-Origin': '*',
            //    'Content-Type': 'application/json'
            //},
            url: '/KeHoachCapPhat/GetNienHan',
            data: { id_dv: $scope.DON_VI_ID, nam: parseInt($scope.cbbNam) },
            success: function (data) {
                if (!data.Error) {
                    $scope.dsTieuChuan = data.nienhan;
                    $('#modelCHNienHan').modal('show');
                    $scope.ListLKH_LTC_NTC = data.treeLKH_LTC_NTC;
                    $.fn.zTree.init($("#treeKeHoach_TC_NTC"), settingLKH_LTC_NTC, $scope.ListLKH_LTC_NTC);
                    var zTree = $.fn.zTree.getZTreeObj("treeKeHoach_TC_NTC");
                    var type = { "Y": "ps", "N": "ps" };
                    zTree.setting.check.chkboxType = type;

                    treeLucLuongSource = [];
                    if (data.lucLuongs != null && data.lucLuongs.length > 0) {
                        var lucLuong0s = data.lucLuongs.filter(function (x) {
                            return (x.LL_CHA_ID == 0);
                        });
                        if (lucLuong0s != null && lucLuong0s.length > 0) {
                            for (var i = 0; i < lucLuong0s.length; i++) {

                                var comboTree = { id: lucLuong0s[i].ID, title: lucLuong0s[i].TEN_LL /*'Lực lượng'*/ };
                                // kiểm tra có con không
                                var childs = data.lucLuongs.filter(function (x) {
                                    return (x.LL_CHA_ID == lucLuong0s[i].ID);
                                });

                                if (childs != null && childs.length > 0) {
                                    ConvertTreeLucLuong(comboTree, data.lucLuongs, lucLuong0s[i].ID);
                                }
                                treeLucLuongSource.push(comboTree);

                            }
                        }
                    }

                    treeLucLuong = $('#TreeComboLucluong').comboTree({
                        source: treeLucLuongSource,
                        isMultiple: false
                    });

                    
                } else {
                    toastr.error(data.message);
                }
                $scope.$apply();

            }
        });

    }


    function ConvertTreeLucLuong(lstTreeModel, lucLuongs, Id) {
        var lstPageMenu = lucLuongs.filter(function (x) {
            return (x.LL_CHA_ID == Id);
        });

        if (lstPageMenu != null && lstPageMenu.length > 0) {
            lstTreeModel.subs = [];
            for (var i = 0; i < lstPageMenu.length; i++) {
                var tree =
                {
                    id: lstPageMenu[i].ID,
                    title: lstPageMenu[i].TEN_LL
                };
                // Kiểm tra xem có con không
                var lucLuongChilds = lucLuongs.filter(function (x) {
                    return (x.LL_CHA_ID == lstPageMenu[i].ID);
                });
                if (lucLuongChilds != null && lucLuongChilds.length > 0) {
                    ConvertTreeLucLuong(tree, lucLuongs, lstPageMenu[i].ID);
                }
                lstTreeModel.subs.push(tree);
            }

        }
    }

    $scope.NH_LucLuongId = 0;
    $scope.ChangeLucLuong = function () {
        $scope.LucLuongName = treeLucLuong.getSelectedNames();
        var id = treeLucLuong.getSelectedIds();
        if (id > 0) {
            $scope.NH_LucLuongId = id;
        } else {
            $scope.NH_LucLuongId = 0;
        }
        TimKiemNienHan();
    };


    $scope.changeNH = function (item) {
        if ($scope.dsSuaNienHan.indexOf(item) === -1) {
            $scope.dsSuaNienHan.push(item);
        }

    }

    $scope.LuuCHNienHan = function () {
        var nam = true;
        for (var i = 0; i < $scope.dsSuaNienHan.length; i++) {
            if ($scope.dsSuaNienHan[i].nam_nien_han == undefined) {
                nam = false;
            }
        }

        if (nam == false) {
            toastr.error('Năm niên hạn không được lớn hơn niên hạn hoặc nhỏ hơn 0.');
            return;
        }
        showToast();
        $.ajax({
            type: 'post',
            url: '/KeHoachCapPhat/SuaNienHan',
            data: {
                dsNienHan: $scope.dsSuaNienHan
            },
            success: function (data) {

                if (data.Error == false) {
                    toastr.error(data.Title);
                } else {
                    toastr.success('Cập nhật niên hạn thành công.');

                }
                hideLoading();
            }
        });
    }



    ///////////////////////////////// Đăng ký cỡ số - Giao kế hoạch sản xuất /////////////////////////
    // Chọn tab Đăng ký cỡ số - Giao kế hoạch sản xuất
    // Lấy danh sách trang phục
    $scope.ChooseTabDangKyCoSo = function () {
        // kiểm tra xem đã chọn kế hoạch chưa
        if ($scope.CP_SelectKeHoachItem != null && $scope.CP_SelectKeHoachItem.ID_KH > 0) {
            DKCS_LayDanhSachTrangPhuc();
        }
        else {
            toastr.error("Vui lòng chọn kế hoạch!");
        }
    }

    // thay đổi số lượng đăng ký của Trang phục khác
    $scope.TabDKCSGKHMD_TPKChangeSoLuongDK = function (item, index) {
        if (item.SO_LUONG_DK > item.SO_LUONG_KH) {
            toastr.error("Số lượng đăng ký không được lớn hơn số lượng kế hoạch!");
            $scope.TabDKCSGKHMD_TrangPhucKhacs[index].SO_LUONG_DK = 0;
        }
    }

    // Thay đổi số lượng kế hoạch của sản phẩm con(cỡ số)
    $scope.TabDKCSGKHMD_ChangeSoLuongKeHoach = function (item) {
        if (item.parent == false) {
            //if (item.SO_LUONG_TK > item.SO_LUONG_KH) {
            var spCon_SLKH = 0;
            for (var i = 0; i < $scope.TabDKCSGKHMD_DangKyCoSos.length; i++) {
                spCon_SLKH = 0;
                if ($scope.TabDKCSGKHMD_DangKyCoSos[i].children != null && $scope.TabDKCSGKHMD_DangKyCoSos[i].children.length > 0) {
                    for (var j = 0; j < $scope.TabDKCSGKHMD_DangKyCoSos[i].children.length; j++) {
                        spCon_SLKH += $scope.TabDKCSGKHMD_DangKyCoSos[i].children[j].SO_LUONG_KH;
                    }
                }

                // Kiểm tra xem tổng số lượng kế hoạch của sp con có lớn hơn số lượng kế hoạch của sp cha
                if (spCon_SLKH > $scope.TabDKCSGKHMD_DangKyCoSos[i].SO_LUONG_KH) {
                    for (var j = 0; j < $scope.TabDKCSGKHMD_DangKyCoSos[i].children.length; j++) {
                        if ($scope.TabDKCSGKHMD_DangKyCoSos[i].children[j].id = item.id) {
                            $scope.TabDKCSGKHMD_DangKyCoSos[i].children[j].SO_LUONG_KH = 0;
                        }
                    }
                    toastr.error("Tổng Số lượng kế hoạch của cỡ số không được lớn hơn Số lượng kế hoạch của sản phẩm!");
                } else {
                    $scope.TabDKCSGKHMD_DangKyCoSos[i].SO_LUONG_DK = $scope.TabDKCSGKHMD_DangKyCoSos[i].SO_LUONG_KH - spCon_SLKH;
                }
            }
            //}
            //else {
            //    for (var i = 0; i < $scope.TabDKCSGKHMD_DangKyCoSos.length; i++) {
            //        for (var j = 0; j < $scope.TabDKCSGKHMD_DangKyCoSos[i].children.length; j++) {
            //            if ($scope.TabDKCSGKHMD_DangKyCoSos[i].children[j].id = item.id) {
            //                $scope.TabDKCSGKHMD_DangKyCoSos[i].children[j].SO_LUONG_KH = 0;
            //            }
            //        }
            //    }
            //    toastr.error("Số lượng kế hoạch không được lớn hơn Tồn kho bộ!");
            //}
        }
    }

    $scope.TabDKCSGKHMD_TrangPhucKhacs = [];
    $scope.TabDKCSGKHMD_DangKyCoSos = [];
    $scope.TabDKCSGKHMD_GiaoKHMayDos = [];

    $scope.TabDKCSGKHMD_CheckAllTPDKCS = false;
    $scope.TabDKCSGKHMD_IsShowCtyUT1 = false;
    $scope.TabDKCSGKHMD_IsShowCtyUT2 = false;
    $scope.TabDKCSGKHMD_TenCtyUT1 = "";
    $scope.TabDKCSGKHMD_TenCtyUT2 = "";
    // Lấy danh sách trang phục
    function DKCS_LayDanhSachTrangPhuc() {
        $scope.TabDKCSGKHMD_TrangPhucKhacs = [];
        $scope.TabDKCSGKHMD_DangKyCoSos = [];
        $scope.TabDKCSGKHMD_GiaoKHMayDos = [];
        $scope.TabDKCSGKHMD_IsShowCtyUT1 = false;
        $scope.TabDKCSGKHMD_IsShowCtyUT2 = false;
        $scope.TabDKCSGKHMD_TenCtyUT1 = "";
        $scope.TabDKCSGKHMD_TenCtyUT2 = "";
        showToast();
        $.ajax({
            type: 'POST',
            dataType: 'json',
            async: true,
            cache: false,
            url: '/KeHoachCapPhat/DKCS_GKHMD_LayDanhSachTrangPhuc',
            data: { idKeHoach: $scope.CP_SelectKeHoachItem.ID_KH, donViId: $scope.DON_VI_ID, keyword: $scope.TabDKCSGKHMD_KeywordTrangPhuc },
            success: function (data) {
                hideLoading();
                if (data.TrangPhucKhacs != null && data.TrangPhucKhacs.length > 0) {
                    $scope.TabDKCSGKHMD_TrangPhucKhacs = data.TrangPhucKhacs;
                }
                if (data.TrangPhucCSs != null && data.TrangPhucCSs.length > 0) {
                    $scope.TabDKCSGKHMD_DangKyCoSos = data.TrangPhucCSs;
                }

                if (data.TrangPhucMayDos != null && data.TrangPhucMayDos.length > 0) {
                    $scope.TabDKCSGKHMD_GiaoKHMayDos = data.TrangPhucMayDos;
                    //console.log("SMMayDo");
                    //console.log($scope.TabDKCSGKHMD_GiaoKHMayDos);
                    $scope.TabDKCSGKHMD_TenCtyUT1 = $scope.TabDKCSGKHMD_GiaoKHMayDos[0].CONG_TY_DEFAULT1_TEN;
                    $scope.TabDKCSGKHMD_TenCtyUT2 = $scope.TabDKCSGKHMD_GiaoKHMayDos[0].CONG_TY_DEFAULT2_TEN;
                    if ($scope.TabDKCSGKHMD_TenCtyUT1 != null && $scope.TabDKCSGKHMD_TenCtyUT1.length > 0) {
                        $scope.TabDKCSGKHMD_IsShowCtyUT1 = true;
                    }
                    if ($scope.TabDKCSGKHMD_TenCtyUT2 != null && $scope.TabDKCSGKHMD_TenCtyUT2.length > 0) {
                        $scope.TabDKCSGKHMD_IsShowCtyUT2 = true;
                    }
                }
                $scope.$apply();
            },
            error: function (xhr) {
                hideLoading();
            }
        });
    }

    // Tìm kiếm trang phục
    $scope.TabDKCSGKHMD_SearchTrangPhuc = function () {
        DKCS_LayDanhSachTrangPhuc();
    }

    // Lưu đăng ký cỡ số
    $scope.TabDKCSGKHMD_Luu = function () {
        showToast();
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoachCapPhat/DKCS_GKHMD_LuuDangKyCoSo',
            data: {
                idKeHoach: $scope.CP_SelectKeHoachItem.ID_KH,
                donViId: $scope.DON_VI_ID,
                trangPhucKhacs: $scope.TabDKCSGKHMD_TrangPhucKhacs,
                tpDangKyCoSos: $scope.TabDKCSGKHMD_DangKyCoSos,
                tpDangKyMayDos: $scope.TabDKCSGKHMD_GiaoKHMayDos,
            },
            success: function (data) {
                hideLoading();
                if (data.obj.Error == false) {
                    toastr.success(data.obj.Title);
                } else {
                    toastr.error(data.obj.Title);
                }
            }
        });
    }



    // Chọn tất cả
    $scope.toggleAllCheckboxes = function ($event) {
        var i,
            item,
            len,
            ref,
            results,
            selected;
        selected = $event.target.checked;
        ref = $scope.TabDKCSGKHMD_DangKyCoSos;
        results = [];
        for (i = 0, len = ref.length; i < len; i++) {
            item = ref[i];
            item.selected = selected;
            if (item.children != null) {
                results.push($scope.$broadcast('changeChildren',
                    item));
            } else {
                results.push(void 0);
            }
        }
        return results;
    };
    $scope.initCheckbox = function (item, parentItem) {
        return item.selected = parentItem && parentItem.selected || item.selected || false;
    };
    $scope.TabDKCSGKHMD_ChangeCheckbox = function (item, parentScope) {
        if (item.children != null) {
            $scope.$broadcast('changeChildren', item);

            for (var j = 0; j < item.children.length; j++) {
                item.children[j].selected = item.selected;
            }
        }
        if (parentScope.item != null) {
            return $scope.$emit('changeParent', parentScope);
        }
    };
    $scope.$on('changeChildren',
        function (event,
            parentItem) {
            var child,
                i,
                len,
                ref,
                results;
            ref = parentItem.children;
            results = [];
            for (i = 0, len = ref.length; i < len; i++) {
                child = ref[i];
                child.selected = parentItem.selected;
                if (child.children != null) {
                    results.push($scope.$broadcast('changeChildren',
                        child));
                } else {
                    results.push(void 0);
                }
            }

            return results;
        });
    return $scope.$on('changeParent',
        function (event,
            parentScope) {
            var children;
            children = parentScope.item.children;
            parentScope.item.selected = $filter('selected')(children).length === children.length;
            parentScope = parentScope.$parent.$parent;
            if (parentScope.item != null) {
                return $scope.$broadcast('changeParent',
                    parentScope);
            }
        });


    // Tab giao kế hoạch may đo - Thay đổi số lượng giao sản xuất
    $scope.TabDKCSGKHMD_ChangeSoLuongGiaoSX = function (index) {
        $scope.TabDKCSGKHMD_GiaoKHMayDos[index].SL_GIAO_SX_CTUT1 = 0;
        $scope.TabDKCSGKHMD_GiaoKHMayDos[index].SL_GIAO_SX_CTUT2 = 0;
    }

    // Tab giao kế hoạch may đo - Thay đổi số lượng của từng cty
    $scope.TabDKCSGKHMD_ChangeSoLuongCty = function (index, item, type) {
        var soLuongGiaoSX = item.TONG_SL_GIAO_SX;
        var soLuongCty1 = item.SL_GIAO_SX_CTUT1;
        var soLuongCty2 = item.SL_GIAO_SX_CTUT2;
        if (soLuongGiaoSX < (soLuongCty1 + soLuongCty2)) {
            toastr.error("Tổng số lượng giao cho Công ty không được lớn hơn SL giao SX.");

            if (type == 1) {
                $scope.TabDKCSGKHMD_GiaoKHMayDos[index].SL_GIAO_SX_CTUT1 = 0;
            }
            if (type == 2) {
                $scope.TabDKCSGKHMD_GiaoKHMayDos[index].SL_GIAO_SX_CTUT2 = 0;
            }
        }
    }

    ///////////////////////////////// End Đăng ký cỡ số - Giao kế hoạch sản xuất /////////////////////




});
app.controller('ThemMoiKeHoach', function ($scope, $rootScope, $uibModalInstance, $uibModal, $ngConfirm, showToast, hideLoading, data) {
    $scope.ListNamNew = [];
    $scope.DataDmKeHoach = [];
    $scope.DsKeHoach = [];
    $scope.cbbKeHoach = {};
    var currentYear = new Date().getFullYear();
    for (var i = currentYear; i > currentYear - 7; i--) {
        $scope.ListNamNew.push({ Id: i, Name: i });
    }

    $scope.cbbKyNew = data.ky;
    $scope.cbbNamNew = data.nam;

    $scope.DON_VI_ID = $rootScope.DonViId;
    //Khởi tạo danh sách kế hoạch
    $.ajax({
        type: 'GET',
        url: '/KeHoachCapPhat/GetDanhMucKeHoach',
        success: function (data) {
            if (data.status) {
                hideLoading();
                if (data.data !== undefined) {
                    $scope.DataDmKeHoach = data.data;
                    angular.forEach(data.data, function (item) {
                        if (item.ID_LOAI_KH_CHA <= 0) {
                            let khSelected = false;
                            if (item.MA_LOAI_KH === 'KH') {
                                khSelected = true;
                            }
                            $scope.DsKeHoach.push({ id: item.ID_LOAI_KH, code: item.MA_LOAI_KH, name: item.TEN_LOAI_KH, type: item.TYPE_LOAI_KH, Selected: khSelected });
                        }
                    });
                    $scope.cbbKeHoach = $scope.DsKeHoach[0];
                    $scope.txtarLyDoCap = $scope.cbbKeHoach.name;
                    $scope.$apply();
                }
                else {
                    toastr.error("Danh sách kế hoạch trống. Xin vui lòng cập nhật trước khi tạo kế hoạch!");
                    return false;
                }
            } else {
                hideLoading();
                toastr.error(data.message);
                return false;
            }
        }
    });

    //$scope.DsKeHoach = [{ code: 'KH', name: 'Lập kế hoạch tự động', Selected: true },
    //{ code: 'BXU', name: 'Cấp ngoài tiêu chuẩn' },
    //{ code: 'NTDC', name: 'Nhập trả - Đổi cỡ' }
    //];

    //Loại cấp ứng
    //$scope.DsLoaiKeHoachBXU = [
    //    { code: 'BSTC', name: 'Bổ sung tiêu chuẩn', Selected: true },
    //    { code: 'UTTC', name: 'Ứng trừ tiêu chuẩn' },
    //    { code: 'BSUTTC', name: '"Bổ sung - Ứng trừ" tiêu chuẩn' },
    //    { code: 'PCLB', name: 'Phòng chống lụt bão' },
    //    { code: 'DTHT', name: 'Diễn tập - Hội thao' },
    //    { code: 'BVHN', name: 'Bảo vệ "Hội nghị - Ngày lễ", chống bạo động' }
    //];
    //Loại  Nhập-Trả-Đổi
    //$scope.DsLoaiKeHoachNTD = [
    //    { code: 'NTKB', name: 'Nhập trả kho bộ', Selected: true },
    //    { code: 'NTDC', name: 'Nhập trả - Đổi cỡ' }
    //];

    $scope.DsLoaiKeHoach = [];
    $scope.cbbLoaiKeHoach = $scope.DsLoaiKeHoach[0];
    $scope.MaLoaiKH = '';
    $scope.MaKH = '';

    hideLoading();
    $scope.cancel = function () {
        $uibModalInstance.close();
    };

    $scope.CP_ChangeKeHoach = function () {
        $scope.DsLoaiKeHoach = [];
        //Kế hoạch nhập tay
        //if ($scope.cbbKeHoach.code === 'BXU') { 
        //    $scope.DsLoaiKeHoach = $scope.DsLoaiKeHoachBXU; 
        //    $scope.showloaiTC = true;
        //}
        //else if ($scope.cbbKeHoach.code === 'KH') {
        //    $scope.showloaiTC = false;
        //    $scope.txtarLyDoCap = $scope.cbbKeHoach.name;
        //}
        //else {
        //    $scope.DsLoaiKeHoach = $scope.DsLoaiKeHoachNTD;
        //    $scope.showloaiTC = true;
        //}

        if ($scope.cbbKeHoach.code === 'KH')
            $scope.showloaiTC = false;
        else {
            $scope.showloaiTC = true;
            var daTaKh = $scope.DataDmKeHoach.filter(x => x.ID_LOAI_KH_CHA == $scope.cbbKeHoach.id);
            angular.forEach(daTaKh, function (item) {
                $scope.DsLoaiKeHoach.push({ id: item.ID_LOAI_KH, code: item.MA_LOAI_KH, name: item.TEN_LOAI_KH, type: item.TYPE_LOAI_KH });
            });
        }
    };

    $scope.CP_ChangeLoaiKeHoach = function () {
        $scope.MaLoaiKH = $scope.cbbLoaiKeHoach.code;
        $scope.txtarLyDoCap = $scope.cbbLoaiKeHoach.name;
    };

    $scope.CP_LuuKHCapPhat = function () {
        showToast();
        if ($scope.txtarLyDoCap == null || $scope.txtarLyDoCap == '') {
            hideLoading();
            toastr.error('Bạn chưa nhập lý do cấp phát!');
        } else {
            //Tạo kế hoạch tự động 
            if ($scope.cbbKeHoach.code === 'KH') {
                $scope.LstCheckKhTuDong = [];
                if (data.dataKhChoXl !== undefined && data.dataKhChoXl.length > 0) {
                    var lstKhChoXl = data.dataKhChoXl.filter(x => x.TRANG_THAI !== 'A' && x.TRANG_THAI !== 'D' && x.LOAI_KH === 'KH' && x.NAM === parseInt($scope.cbbNamNew));
                    if (lstKhChoXl !== undefined && lstKhChoXl.length > 0) {
                        angular.forEach(lstKhChoXl, function (val, key) {
                            $scope.LstCheckKhTuDong.push({ TRANG_THAI: val.TRANG_THAI, LOAI_KH: val.LOAI_KH, NAM: val.NAM });
                        });
                    }
                }
                if (data.dataKhCap !== undefined && data.dataKhCap.length > 0) {
                    var lstKhCap = data.dataKhCap.filter(x => x.TRANG_THAI !== 'A' && x.TRANG_THAI !== 'D' && x.LOAI_KH === 'KH' && x.NAM === parseInt($scope.cbbNamNew));
                    if (lstKhCap !== undefined && lstKhCap.length > 0) {
                        angular.forEach(lstKhCap, function (val, key) {
                            $scope.LstCheckKhTuDong.push({ TRANG_THAI: val.TRANG_THAI, LOAI_KH: val.LOAI_KH, NAM: val.NAM });
                        });
                    }
                }
                if ($scope.LstCheckKhTuDong === undefined || $scope.LstCheckKhTuDong.length <= 0) { //Nếu tồn tại một KH tự động chưa duyệt thì ko cho phép tạo KH mới
                    //Chọn loại tiêu chuẩn
                    hideLoading();
                    var modalInstance = $uibModal.open({
                        animation: $scope.animationsEnabled,
                        templateUrl: '/KeHoachCapPhat/_DSLoaiTieuChuan',
                        controller: 'DSLoaiTieuChuan',
                        size: 'lg',
                        backdrop: 'static',
                        resolve: {
                            datamodel: function () {
                                return {
                                    nam: $scope.cbbNamNew,
                                    ky: $scope.cbbKyNew,
                                    donViId: $scope.DON_VI_ID,
                                    moTa: $scope.txtarLyDoCap,
                                    khId: 0
                                }
                            }
                        }
                    });
                    modalInstance.result.then(function (response) {
                        $scope.CP_SelectKeHoachItem = response;
                        $uibModalInstance.close($scope.CP_SelectKeHoachItem);
                    });
                }
                else {
                    hideLoading();
                    toastr.error('Đã tồn tại kế hoạch tự động chưa duyệt!');
                    return false;
                }

            }
            else if ($scope.MaLoaiKH === 'NTKB' || $scope.MaLoaiKH === 'NTDC') {
                $scope.LstCheckNhapTra = [];
                if (data.dataKhCap !== undefined && data.dataKhCap.length > 0) {
                    var lstKhCap = data.dataKhCap.filter(x => x.TRANG_THAI !== 'A' && x.TRANG_THAI !== 'D' && x.TRANG_THAI !== 'P' && x.LOAI_KH === $scope.cbbKeHoach.code && x.NAM === parseInt($scope.cbbNamNew));
                    if (lstKhCap !== undefined && lstKhCap.length > 0) {
                        angular.forEach(lstKhCap, function (val, key) {
                            $scope.LstCheckNhapTra.push({ TRANG_THAI: val.TRANG_THAI, LOAI_KH: val.LOAI_KH, NAM: val.NAM });
                        });
                    }
                }
                if ($scope.LstCheckNhapTra === undefined || $scope.LstCheckNhapTra.length <= 0) {
                    $.ajax({
                        type: 'post',
                        url: '/KeHoachCapPhat/ThemMoiKHCapPhat',
                        data: {
                            dvId: $scope.DON_VI_ID,
                            iNam: $scope.cbbNamNew,
                            iKy: $scope.cbbKyNew,
                            loaiCp: $scope.cbbKeHoach.code,
                            lyDo: $scope.txtarLyDoCap,
                            maloaikh: $scope.MaLoaiKH
                        },
                        success: function (data) {
                            hideLoading();
                            if (data.Error == true) {
                                toastr.error(data.message);
                            } else {
                                if ($scope.MaLoaiKH === 'NTDC')
                                    window.location.href = '/KeHoachNhapTraDoiCo/Index';
                                else
                                    window.location.href = '/KeHoachNhapTraKB/Index';
                            }
                        }
                    });
                }
                else {
                    hideLoading();
                    toastr.error('Đã tồn tại một KH cùng loại chưa được duyệt ở Tab kế hoạch cấp, cần chuyển KH đó sang Tab kế hoạch chờ xử lý trước khi tạo mới!');
                    return false;
                }
            }
            else {
                if ($scope.MaLoaiKH === undefined || $scope.MaLoaiKH === '') {
                    hideLoading();
                    toastr.error('Bạn chưa chọn loại KH.');
                    return false;
                }
                $scope.LstCheckNhapTay = [];
                if (data.dataKhCap !== undefined && data.dataKhCap.length > 0) {
                    var lstKhCap = data.dataKhCap.filter(x => x.TRANG_THAI !== 'A' && x.TRANG_THAI !== 'D' && x.TRANG_THAI !== 'P' && x.LOAI_KH === $scope.cbbKeHoach.code && x.NAM === parseInt($scope.cbbNamNew));
                    if (lstKhCap !== undefined && lstKhCap.length > 0) {
                        angular.forEach(lstKhCap, function (val, key) {
                            $scope.LstCheckNhapTay.push({ TRANG_THAI: val.TRANG_THAI, LOAI_KH: val.LOAI_KH, NAM: val.NAM });
                        });
                    }
                }
                if ($scope.LstCheckNhapTay === undefined || $scope.LstCheckNhapTay.length <= 0) {
                    //Tạo kế hoạch bằng tay
                    $.ajax({
                        type: 'post',
                        url: '/KeHoachCapPhat/ThemMoiKHCapPhat',
                        data: {
                            dvId: $scope.DON_VI_ID,
                            iNam: $scope.cbbNamNew,
                            iKy: $scope.cbbKyNew,
                            loaiCp: $scope.cbbKeHoach.code,
                            lyDo: $scope.txtarLyDoCap,
                            maloaikh: $scope.MaLoaiKH
                        },
                        success: function (data) {
                            hideLoading();
                            if (data.Error == true) {
                                toastr.error(data.message);
                            } else {
                                toastr.success(data.message);
                                $scope.CP_SelectKeHoachItem = data.data;
                                $uibModalInstance.close($scope.CP_SelectKeHoachItem);
                            }
                        }
                    });
                }
                else {
                    hideLoading();
                    toastr.error('Đã tồn tại một KH cùng loại chưa được duyệt ở Tab kế hoạch cấp, cần chuyển KH đó sang Tab kế hoạch chờ xử lý trước khi tạo mới!');
                    return false;
                }

            }
        }
    };
});

app.controller('DSLoaiTieuChuan', function ($scope, $rootScope, $uibModalInstance, $ngConfirm, showToast, hideLoading, datamodel) {
    $scope.model = {};
    $scope.listTieuChuanModal = [];
    $scope.strListLoaiTC = '';
    showToast();
    $scope.changeCheckAllTCModal = function () {
        angular.forEach($scope.listTieuChuanModal, function (item) {
            item.Selected = event.target.checked;
        });
    };
    //#region Chọn loại tiêu chuẩn
    // Có hiển thị cột checkbox ở đầu không
    $scope.tree_Table_Checkbox = true;
    // Model data
    $scope.tree_data = [];
    $scope.LoadData = function (donVi) {
        $.ajax({
            type: 'GET',
            url: '/KeHoachCapPhat/GetDanhMucLoaiTC',
            success: function (data) {
                if (data.status) {
                    $scope.tree_data = data.data;
                } else {
                    toastr.error(data.message);
                }
                $scope.$apply();
                hideLoading();
            }
        });
    }
    $scope.LoadData();
    // cấu hình hiển thị cùng với icon
    $scope.expanding_property = {
        field: "STT",
        displayName: "STT",
        width: "7%",
        rowspan: 1
    };
    // Cấu hình các cột sẽ hiển thị tiếp theo
    $scope.col_defs = [[
        { field: "TEN_LOAI", displayName: "Tên tiêu chuẩn", width: "75%", rowspan: 1, colspan: 1 },
    ]];
    $scope.body_defs = [{ field: "TEN_LOAI" }];
    // event when change value checkbox
    $scope.ChangeTreeTableCheckbox = function (item) {
        if (item.type != 1) {
            var isBreak = false;
            if (item.type == 2) {
                // cấp 1
                for (var i = 0; i < $scope.tree_data.length; i++) {
                    if ($scope.tree_data[i].children != null && $scope.tree_data[i].children.length > 0) {
                        // cấp 2
                        for (var j = 0; j < $scope.tree_data[i].children.length; j++) {
                            if ($scope.tree_data[i].children[j].ID == item.ID) {
                                if ($scope.tree_data[i].children[j].children != null && $scope.tree_data[i].children[j].children.length > 0) {
                                    // cấp 3
                                    for (var k = 0; k < $scope.tree_data[i].children[j].children.length; k++) {
                                        $scope.tree_data[i].children[j].children[k].selected = $scope.tree_data[i].children[j].selected;
                                    }
                                }
                                isBreak = true;
                                break;
                            }
                        }
                    }
                    if (isBreak)
                        break;
                }
            } else {
                var countSelected = 0;
                // Cấp 1
                for (var i = 0; i < $scope.tree_data.length; i++) {
                    if ($scope.tree_data[i].children != null && $scope.tree_data[i].children.length > 0) {
                        // Cấp 2
                        for (var j = 0; j < $scope.tree_data[i].children.length; j++) {
                            if ($scope.tree_data[i].children[j].ID == item.LOAI_KH_ID) {
                                if ($scope.tree_data[i].children[j].children != null && $scope.tree_data[i].children[j].children.length > 0) {
                                    // cấp 3
                                    countSelected = 0;
                                    for (var k = 0; k < $scope.tree_data[i].children[j].children.length; k++) {
                                        if ($scope.tree_data[i].children[j].children[k].selected) {
                                            countSelected++;
                                        }
                                    }
                                    $scope.tree_data[i].children[j].selected = false;
                                    if (countSelected == $scope.tree_data[i].children[j].children.length) {
                                        $scope.tree_data[i].children[j].selected = true;
                                    }
                                }
                                isBreak = true;
                                break;
                            }
                        }
                    }
                }
            }
        } else {
            $scope.strListLoaiTC += item.ID + ',';
        }
    };
    // event when click choose row
    $scope.ClickTreeTableRow = function (item) {
        console.log(item);
    };
    //#endregion

    $scope.LapKeHoachTuDong = function () {
        showToast();

        $rootScope.CP_SelectKeHoachitem = {};

        var loaiKHId = "";
        var nhomTCId = "";
        // Cấp 1
        for (var i = 0; i < $scope.tree_data.length; i++) {
            if ($scope.tree_data[i].selected) {
                loaiKHId = $scope.tree_data[i].ID + ",";
            }
            if ($scope.tree_data[i].children != null && $scope.tree_data[i].children.length > 0) {
                // Cấp 2
                for (var j = 0; j < $scope.tree_data[i].children.length; j++) {
                    if ($scope.tree_data[i].children[j].selected) {
                        loaiKHId = $scope.tree_data[i].children[j].ID + ",";
                    }
                    if ($scope.tree_data[i].children[j].children != null && $scope.tree_data[i].children[j].children.length > 0) {
                        // cấp 3
                        for (var k = 0; k < $scope.tree_data[i].children[j].children.length; k++) {
                            if ($scope.tree_data[i].children[j].children[k].selected) {
                                nhomTCId = $scope.tree_data[i].children[j].children[k].ID + ",";
                            }
                        }
                    }
                }
            }
        }

        if ((nhomTCId == null || nhomTCId == '') && (loaiKHId == null || loaiKHId == '')) {
            hideLoading();
            toastr.error('Bạn chưa chọn loại tiêu chuẩn!');
            return false;
        }
        if (datamodel.nam === undefined || datamodel.ky === undefined || datamodel.donViId === undefined) {
            hideLoading();
            toastr.error('Xin vui lòng kiểm tra lại các thông tin: Đơn vị, Năm, Kỳ!');
            return false;
        } else {
            //Tạo kế hoạch tự động
            $.ajax({
                type: 'post',
                url: '/KeHoachCapPhat/CapUngTieuChuanTuDong',
                data: {
                    iNam: datamodel.nam,
                    ky: datamodel.ky,
                    loaiDT: $scope.strListLoaiTC,
                    loaiKHId: loaiKHId,
                    nhomTCId: nhomTCId,
                    iDonViId: datamodel.donViId,
                    moTa: datamodel.moTa,
                    khId: datamodel.khId
                },
                success: function (data) {
                    hideLoading();
                    if (data.status) {
                        toastr.error(data.message);
                    } else {
                        toastr.success(data.message);
                        //Chi tiết cấp phát mới tạo   
                        $uibModalInstance.close(data.data);

                    }
                }
            });
        }

        $scope.cancel = function () {
            $uibModalInstance.close();
        };

    };
    //Danh sách kế hoạch theo ĐV, Năm, Kỳ
    $scope.LoadDsKeHoach = function (dvID, nam, ky) {
        $scope.selectedRowKeHoach = -1;
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoachCapPhat/GetDsKeHoachCapPhat',
            data: { dvId: dvID, iNam: nam, iKy: ky },
            success: function (data) {
                hideLoading();
                if (data.status) {
                    $scope.ListKHCapPhat = data.data.filter(x => x.TRANG_THAI !== 'P');
                    $scope.ListKHCapPhatChoXl = data.data.filter(x => x.TRANG_THAI === 'P');
                    $scope.$apply();
                }
            }
        });
    };
    $scope.cancel = function () {
        $uibModalInstance.close();
    };

});

app.controller('BoxDmTrangPhuc', function ($scope, $rootScope, $uibModalInstance, $ngConfirm, showToast, hideLoading) {
    $scope.model = {};
    $scope.listTrangPhuc = [];
    $scope.listSanPhamModal = [];
    hideLoading();
    $.ajax({
        type: 'GET',
        url: '/KeHoachCapPhat/GetDanhMucTrangPhuc',
        success: function (data) {
            if (data.status) {
                $scope.listTrangPhuc = data.data;
                $scope.listSanPhamModal = data.data;
            } else {
                toastr.error(data.message);
            }
            $scope.$apply();
            hideLoading();
        }
    });
    $scope.searchSanPham = function () {
        var sp = $scope.txtSearchSanPham;
        if (sp !== null) {
            $scope.listSanPhamModal = $scope.listTrangPhuc.filter(x => (x.MA_H55 + '').includes(sp) || (x.TEN_SP + '').includes(sp))
        } else {
            $scope.listSanPhamModal = $scope.listTrangPhuc;
        }
    }

    $scope.changeCheckAllSPModal = function () {
        angular.forEach($scope.listSanPhamModal, function (val, key) {
            val.checked = $scope.CheckAll;
        });
    };

    $scope.chooseSanPham = function () {
        $scope.itemSelected = [];
        angular.forEach($scope.listSanPhamModal, function (val, key) {
            if (val.checked)
                $scope.itemSelected.push(val);
        })
        $uibModalInstance.close($scope.itemSelected);
    }
    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});
app.controller('BoxDmCapUngBoXung', function ($scope, $rootScope, $uibModalInstance, $ngConfirm, showToast, hideLoading) {
    $scope.model = {};
    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});

app.controller('BoxDmQuyetToan', function ($scope, $rootScope, $uibModalInstance, $ngConfirm, showToast, hideLoading, data) {
    $scope.model = {};
    hideLoading();
    $scope.idDonVi = $rootScope.DonViId;
    $scope.iNam = data.nam;
    $scope.QtoanType = 1;
    $scope.dsQuyetToanTruNoTonDonViFull = [];

    $scope.ChonTabQuyetToan = function (tabNo) {
        $scope.QtoanType = tabNo;
        if ($scope.QtoanType == 1) {
            //đã call lúc load page
        }
        if ($scope.QtoanType == 2) {
            //
        }
        if ($scope.QtoanType == 3) {
            $scope.GetDsQuyetToanTruNoTonDonVi();
        }
    }
    //Lấy thông tin chi Tiết các loại Quyết toán: 
    // 1 - Quyết toán Nợ KH Trước
    // 2 - Quyết toán Phiếu Hồi Về
    // 3 - Quyết toán Trừ Nợ Tồn Đơn vị
    /*
    $scope.GetDsQuyetToanTraNoKHTruoc = function () {
        $.ajax({
            type: 'GET',
            async: false,
            url: '/KeHoachCapPhat/GetDsDaQuyetToanTruNoTonDVByKeHoach',
            data: { idDonVi: $scope.idDonVi, iNam: $scope.iNam },
            success: function (res) {
                hideLoading();
                if (res.status) {
                    console.log(res.data);
                    if (res.data.length > 0) {
                        //$scope.dsSPQuyetToanNoTonDV = res.data;
                        ////$rootScope.dsQuyetToanNoKeHoachTruoc = res.data;
                        ////$scope.disableQuyetToanTraNo = false;
                    }
                    else {
                        //$scope.disableQuyetToanTraNo = true;
                        //$scope.dsSPQuyetToanNoTonDV = [];
                    }
                    //$scope.$apply();
                }
            }
        });
    }
    */

    $scope.GetDsQuyetToanTruNoTonDonVi = function () {
        $.ajax({
            type: 'GET',
            async: false,
            url: '/KeHoachCapPhat/GetDsDaQuyetToanTruNoTonDVByKeHoach',
            data: { idDonVi: $scope.idDonVi, iNam: $scope.iNam },
            success: function (res) {
                hideLoading();
                if (res.status) {
                    //console.log(res.data);
                    if (res.data.length > 0) {
                        $scope.dsQuyetToanTruNoTonDonVi = res.data;
                        $scope.dsQuyetToanTruNoTonDonViFull = res.data;
                        ////$rootScope.dsQuyetToanNoKeHoachTruoc = res.data;
                        ////$scope.disableQuyetToanTraNo = false;
                    }
                    else {
                        //$scope.disableQuyetToanTraNo = true;
                        //$scope.dsSPQuyetToanNoTonDV = [];
                    }
                    //$scope.$apply();
                }
            }
        });
    }


    $scope.searchSanPham = function () {
        var sp = $scope.txtSearchSanPham;

        if ($scope.QtoanType === 1) {
            if (sp !== null) {
                $scope.dsQuyetToanNoKeHoachTruoc = $rootScope.dsQuyetToanNoKeHoachTruoc.filter(x => (x.TEN_SP + '').toLowerCase().includes(sp.toLowerCase()) || (x.MA_H55 + '').includes(sp));
            } else {
                $scope.dsQuyetToanNoKeHoachTruoc = $rootScope.dsQuyetToanNoKeHoachTruoc;
            }
        } else if ($scope.QtoanType === 3) {
            if (sp !== null) {
                $scope.dsQuyetToanTruNoTonDonVi = $scope.dsQuyetToanTruNoTonDonViFull.filter(x => (x.TEN_SP + '').toLowerCase().includes(sp.toLowerCase()) || (x.MA_H55 + '').includes(sp));
            } else {
                $scope.dsQuyetToanTruNoTonDonVi = $scope.dsQuyetToanTruNoTonDonViFull;
            }
        }

    }

    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});

app.controller('KeHoachQuyetToan', function ($scope, $rootScope, $uibModalInstance, $ngConfirm, showToast, hideLoading, data) {
    //$scope.model = {};

    $scope.cbbKyNew = data.ky;
    $scope.cbbNamNew = data.nam;
    $scope.DON_VI_ID = $rootScope.DonViId;
    $scope.dsIdKeHoachDaChon = [];

    /** Quyết toán Nợ Kế hoạch trước - #quyettoan
    * LẤY DANH SÁCH KẾ HOẠCH CẤP PHÁT TRƯỚC CÒN NỢ
    */
    LoadDsKeHoachCPConNo = function (dvID, nam, ky) {
        //$scope.selectedRowKeHoach = -1;
        $scope.alertMessage = false;
        $.ajax({
            type: 'GET',
            async: false,
            url: '/KeHoachCapPhat/GetDsKeHoachCapPhatConNo',
            data: { dvId: dvID, iNam: nam, iKy: ky },
            success: function (data) {
                hideLoading();
                if (data.status) {
                    if (data.data.length > 0) {
                        $scope.ListKHCapPhatConNo = data.data.filter(x => x.TRANG_THAI !== 'P');
                        //$scope.$apply(); 
                    }
                    else {
                        $scope.ListKHCapPhatConNo = [];
                        //Hiển thị thông báo
                        $scope.alertMessage = true;
                        //$scope.$apply(); 
                    }

                }
            }
        });
    };

    LoadDsKeHoachCPConNo($scope.DON_VI_ID, $scope.cbbNamNew, $scope.cbbKyNew);
    /** 
    * LẤY DANH SÁCH SẢN PHẨM CÒN NỢ THEO KẾ HOẠCH
    */

    $scope.ShowThongTinDSSanPhamByKeHoach = function (idKeHoach) {

        $scope.dsIdKeHoachDaChon = $scope.ListKHCapPhatConNo.filter(x => x.Selected == true);

        if ($scope.dsIdKeHoachDaChon.length == 0) {
            $scope.dsIdKeHoachDaChon = $scope.ListKHCapPhatConNo.filter(x => x.ID_KH == idKeHoach);
        }
        //console.log($scope.dsIdKeHoachDaChon);

        $.ajax({
            type: 'POST',
            async: false,
            url: '/KeHoachCapPhat/GetDsSanPhamConNoByKeHoach',
            //data: { idKeHoach: idKeHoach },
            data: { dsIdKeHoach: $scope.dsIdKeHoachDaChon },
            success: function (res) {
                hideLoading();
                if (res.status) {
                    if (res.data.length > 0) {
                        $scope.ListDsSanPhamConNoByKeHoach = res.data;
                    }
                    else {
                        $scope.ListKHCapPhatConNo = [];
                    }
                    //$scope.$apply();
                }
            }
        });
    };
    /**
     * Lấy danh sách SP CÒn nợ theo Danh sách kế hoạch đã chọn
     */
    $scope.ShowThongTinDSSanPhamByDsKeHoach = function (dsIdKeHoach) {
        //danh sách ID_KH đã chọn
        $scope.dsKeHoachDaChon = $scope.ListKHCapPhatConNo.filter(x => x.Selected == true);


    }

    // Thực hiện Quyết Toán Kế hoạch Trước #quyettoan #kehoachtruoc
    $scope.btnQuyetToan = () => {

        $rootScope.dsSPDaChonQuyetToan = $scope.ListDsSanPhamConNoByKeHoach.filter(x => x.Selected == true);
        //console.log($scope.dsSPDaChonQuyetToan);
        //console.log($scope.CP_KeHoachDaChon.ID_KH);
        $rootScope.dsSPDaChonQuyetToan.forEach(x => {
            x.ID_KH_TRA_NO = $scope.CP_KeHoachDaChon.ID_KH,
                x.SO_KH_TRA_NO = $scope.CP_KeHoachDaChon.SO_KH
        });
        //console.log($rootScope.tree_data);
        //Lưu Thông tin Quyết toán vào bảng TRA_NO_QUYET_TOAN
        //console.log($scope.dsSPDaChonQuyetToan);
        $scope.LuuQuyetToanTraNoKHTruoc($rootScope.dsSPDaChonQuyetToan);

        $uibModalInstance.close();


    };

    $scope.showHideMessage = function () {
        $scope.alertMessage = false;
    }
    //Check all các Kế hoạch Nợ
    $scope.checkAllDsKeHoach = function () {
        if ($scope.ListKHCapPhatConNo.length > 0) {
            angular.forEach($scope.ListKHCapPhatConNo, function (item) {
                item.Selected = event.target.checked;
            });
        }
    };
    //Xử lý Check All khi chọn từng Kế hoạch dưới danh sách sản phẩm nợ
    $scope.checkValDsKeHoachNo = function (index) {

        $scope.ListKHCapPhatConNo[index].Selected = !$scope.ListKHCapPhatConNo[index].Selected;

        if ($scope.ListKHCapPhatConNo.length > 0) {
            if ($scope.ListKHCapPhatConNo.every(x => x.Selected === true)) {
                $scope.selectAllKeHoach = true;
            } else {
                $scope.selectAllKeHoach = false;
            }
        }
        else {
            $scope.selectAllKeHoach = false;
        }

    };


    //Xử lý Check All khi chọn từng Sản phẩm dưới danh sách sản phẩm nợ
    $scope.checkValDsSanPhamNo = function (index) {

        $scope.ListDsSanPhamConNoByKeHoach[index].Selected = !$scope.ListDsSanPhamConNoByKeHoach[index].Selected;

        if ($scope.ListDsSanPhamConNoByKeHoach.length > 0) {
            if ($scope.ListDsSanPhamConNoByKeHoach.every(x => x.Selected === true)) {
                $scope.selectAllSP = true;
            } else {
                $scope.selectAllSP = false;
            }
        }
        else {
            $scope.selectAllSP = false;
        }

    };

    //Check all các sản phẩm nợ
    $scope.checkAllDsSanPhamNo = function () {
        if ($scope.ListDsSanPhamConNoByKeHoach.length > 0) {
            angular.forEach($scope.ListDsSanPhamConNoByKeHoach, function (item) {
                item.Selected = event.target.checked;
            });
        }
    };

    $scope.LuuQuyetToanTraNoKHTruoc = function (dsSPQuyetToan) {
        //console.log(dsSPQuyetToan);
        showToast();
        $.ajax({
            type: 'POST',
            async: false,
            url: '/KeHoachCapPhat/LuuQuyetToanTraNoKHTruoc',
            data: { listQuyetToan: dsSPQuyetToan },
            success: function (res) {
                hideLoading();
                if (data.obj.Error == false) {
                    toastr.success(data.obj.Title);
                } else {
                    toastr.error(data.obj.Title);
                }
            }, error: (e) => {
                console.log(e);
            }
        });
    };

    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});

//Quyết toàn Trừ nợ tồn đơn vị #quyettoantruno #tondonvi
app.controller('QuyetToanTruNoTonDV', function ($scope, $rootScope, $uibModalInstance, $ngConfirm, showToast, hideLoading, data) {
    $scope.model = {};
    $scope.cbbKyNew = data.ky;
    $scope.cbbNamNew = data.nam;
    $scope.DON_VI_ID = $rootScope.DonViId;
    $scope.thongTinQT = {};

    //Load Danh sách Trang phục Quyết toán - Kế hoạch theo KẾ HOẠCH NỢ TỒN ĐƠN VỊ

    $scope.ShowDSSanPhamByKeHoach = function (idKeHoach) {
        //console.log(idKeHoach);
        $.ajax({
            type: 'GET',
            async: false,
            url: '/KeHoachCapPhat/GetDsSanPhamTruNoTonDV',
            data: { idKeHoach: idKeHoach },
            success: function (data) {
                hideLoading();
                if (data.status) {

                    if (data.data.length > 0) {
                        $scope.ListDsSanPhamTruNoDV = data.data;
                        toastr.success("Lấy danh sách Trang phục thành công");
                        //console.log($scope.ListDsSanPhamTruNoDV);

                    }
                    else {
                        $scope.ListDsSanPhamTruNoDV = [];

                    }

                }
            }
        });
    }

    //Lấy danh sách Quyết toán trừ nợ tồn đơn vị    
    $scope.LoadDsKeHoachTruNoTonDV = function (dvID, nam, ky) {
        $.ajax({
            type: 'GET',
            async: false,
            url: '/KeHoachCapPhat/GetDsKeHoachTruNoTonDV',
            data: { dvId: dvID, iNam: nam, iKy: ky },
            success: function (data) {
                hideLoading();
                if (data.status) {
                    if (data.data.length > 0) {
                        $scope.dsQuyetToanTruNoTonDV = data.data.filter(x => x.TRANG_THAI !== 'P');
                        $scope.selectedRow = 0;
                        //console.log($scope.dsQuyetToanTruNoTonDV[0].ID_KH);
                        $scope.thongTinQT.ID_KH = $scope.dsQuyetToanTruNoTonDV[0].ID_KH;
                        $scope.thongTinQT.NGAY_TAO_TEXT = $scope.dsQuyetToanTruNoTonDV[0].NGAY_TAO_TEXT;
                        $scope.thongTinQT.NAM = $scope.dsQuyetToanTruNoTonDV[0].NAM;

                        $scope.ShowDSSanPhamByKeHoach($scope.thongTinQT.ID_KH);
                    }
                    else {
                        $scope.dsQuyetToanTruNoTonDV = [];
                        $scope.thongTinQT = {};
                    }

                }
            }
        });
    };

    $scope.LoadDsKeHoachTruNoTonDV($scope.DON_VI_ID, $scope.cbbNamNew, $scope.cbbKyNew);

    $scope.showChiTietKHQT = function (idKH) {
        var objTT = $scope.dsQuyetToanTruNoTonDV.filter(x => x.ID_KH === idKH);
        $scope.thongTinQT.NGAY_TAO_TEXT = objTT[0].NGAY_TAO_TEXT;
        $scope.thongTinQT.NAM = objTT[0].NAM;
        $scope.ShowDSSanPhamByKeHoach(idKH);
    }

    //Xử lý Check All khi chọn từng Sản phẩm dưới danh sách
    $scope.checkValDsSanPhamTruNoDV = function (index) {

        $scope.ListDsSanPhamTruNoDV[index].Selected = !$scope.ListDsSanPhamTruNoDV[index].Selected;

        if ($scope.ListDsSanPhamTruNoDV.length > 0) {
            if ($scope.ListDsSanPhamTruNoDV.every(x => x.Selected === true)) {
                $scope.thongTinQT.selectAllSP = true;
            } else {
                $scope.thongTinQT.selectAllSP = false;
            }
        }
        else {
            $scope.thongTinQT.selectAllSP = false;
        }

    };

    //Check all
    $scope.checkAllDsSanPhamTruNoDV = function () {
        if ($scope.ListDsSanPhamTruNoDV.length > 0) {
            angular.forEach($scope.ListDsSanPhamTruNoDV, function (item) {
                item.Selected = event.target.checked;
            });
        }
    };

    $scope.btnQuyetToanTruNoTonDV = function () {

        $scope.dsSanPhamQuyetToan = $scope.ListDsSanPhamTruNoDV.filter(x => x.Selected == true);

        //console.log($scope.dsSanPhamQuyetToan);
        //console.log($scope.thongTinQT.ID_KH);//ID_KH DUOC TRU NO
        //console.log($scope.CP_KeHoachDaChon.ID_KH);//ID_KH TRU NO
        //console.log($scope.dsQuyetToanTruNoTonDV);

        if ($scope.dsSanPhamQuyetToan.length > 0) {
            $scope.dsSanPhamQuyetToan.forEach(x => {
                //x.ID_KH_DUOC_TRA_NO = $scope.CP_KeHoachDaChon.ID_KH,
                //x.SO_KH_DUOC_TRA_NO = $scope.CP_KeHoachDaChon.SO_KH

                x.ID_KH_DUOC_TRA_NO = $scope.thongTinQT.ID_KH,
                    x.SO_KH_DUOC_TRA_NO = $scope.dsQuyetToanTruNoTonDV.filter(x => x.ID_KH = $scope.thongTinQT.ID_KH).SO_KH
            });

            //Lưu Thông tin Quyết toán vào bảng TRA_NO_QUYET_TOAN
            //console.log($scope.dsSanPhamQuyetToan);
            $scope.LuuQuyetToanTruNoTonDV($scope.dsSanPhamQuyetToan);
        } else {
            toastr.error('Chưa chọn Trang phục quyết toán');
        }
        //$uibModalInstance.close();

    };
    $scope.LuuQuyetToanTruNoTonDV = function (dsSPQuyetToan) {
        showToast();
        $.ajax({
            type: 'POST',
            async: false,
            url: '/KeHoachCapPhat/LuuQuyetToanTruTonDV',
            data: { listQuyetToan: dsSPQuyetToan },
            success: function (data) {
                hideLoading();

                //console.log(data);

                if (data.Error == false) {
                    toastr.success(data.Title);
                } else {
                    toastr.error(data.Title);
                }
            }, error: (e) => {
                console.log(e);
            }
        });
    };

    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});

app.filter('unsafe', function ($sce) { return $sce.trustAsHtml; });
// Thêm mới Quyết toán Trừ Nợ Tồn đơn vị #trunoton
app.controller('ThemMoiQTTruNoTonDV', function ($scope, $rootScope, $uibModal, $uibModalInstance, $ngConfirm, showToast, hideLoading, data) {
    $scope.model = {};
    $scope.disableThemMoi = false;
    $scope.disableLuu = true;
    $scope.disableHuy = true;
    $scope.disableXoa = true;
    //$rootScope.listSanPhamTruNoModal = [];

    $scope.disabledTrangPhuc = true;

    $scope.cbbKyNew = data.ky;
    $scope.cbbNamNew = data.nam;
    $scope.DON_VI_ID = $rootScope.DonViId;
    $scope.LOAI_KH = 'NO';
    $scope.MaLoaiKH = 'NO';
    $scope.txtarLyDoCap = 'Trừ Nợ Tồn đơn vị';
    $scope.ListTrangPhucTruNoTonDV = [];

    $scope.showHideMessage = function () {
        $scope.showMessage = false;
    }

    $scope.btnThemMoiQTTruNoTonDV = function () {
        $scope.disableLuu = false;
        $scope.disableHuy = false;
        $scope.disableXoa = false;
        $scope.disableThemMoi = true;
        //console.log($scope.CP_KeHoachDaChon.ID_KH);//ID_KH TRU NO

        //Tạo kế hoạch bằng tay
        $.ajax({
            type: 'post',
            url: '/KeHoachCapPhat/ThemMoiKHTraNoTonDV',
            data: {
                dvId: $scope.DON_VI_ID,
                iNam: $scope.cbbNamNew,
                iKy: $scope.cbbKyNew,
                loaiCp: $scope.LOAI_KH,
                lyDo: $scope.txtarLyDoCap,
                maloaikh: $scope.MaLoaiKH,
                idKHCha: $scope.CP_KeHoachDaChon.ID_KH
            },
            success: function (data) {
                hideLoading();
                //console.log(data.data);
                if (data.Error == true) {
                    toastr.error(data.message);
                } else {
                    toastr.success(data.message);
                    $scope.CP_SelectKeHoachItem = data.data;
                    $scope.LoadDsKeHoachTruNoTonDV($scope.DON_VI_ID, $scope.cbbNamNew, $scope.cbbKyNew);
                    //$uibModalInstance.close($scope.CP_SelectKeHoachItem);
                    //$uibModalInstance.close($scope.DON_VI_ID, $scope.Nam_Hien_Tai, $scope.cbbKyNew, data.data);
                }
            }
        });
    };

    //Load Danh sách Trang phục Quyết toán - Kế hoạch theo KẾ HOẠCH NỢ TỒN ĐƠN VỊ
    $scope.SelectedID_KH = '';
    $scope.ShowDSSanPhamByKeHoach = function (idKeHoach, index) {

        $scope.selectedRow = index;
        $scope.SelectedID_KH = idKeHoach;

        //console.log(idKeHoach);
        $.ajax({
            type: 'GET',
            async: false,
            url: '/KeHoachCapPhat/GetDsSanPhamTruNoTonDV',
            data: { idKeHoach: idKeHoach },
            success: function (data) {
                hideLoading();
                if (data.status) {

                    if (data.data.length > 0) {
                        $scope.ListTrangPhucTruNoTonDV = data.data;
                        toastr.success("Lấy danh sách Trang phục thành công");
                        //console.log($scope.ListTrangPhucTruNoTonDV);

                    }
                    else {
                        $scope.ListTrangPhucTruNoTonDV = [];
                        //Hiển thị thông báo
                        //$scope.alertMessage = true;
                        //$scope.$apply(); 
                    }

                }
            }
        });
    };


    /** Trừ Nợ Tồn đơn vị #trunoton 
    * LẤY DANH SÁCH KẾ HOẠCH Trừ Nợ Tồn đơn vị : LOAI_KH = 'NO'
    */
    $scope.LoadDsKeHoachTruNoTonDV = function (dvID, nam, ky) {
        $scope.showMessage = false;
        $.ajax({
            type: 'GET',
            async: false,
            url: '/KeHoachCapPhat/GetDsKeHoachTruNoTonDV',
            data: { dvId: dvID, iNam: nam, iKy: ky },
            success: function (data) {
                hideLoading();
                if (data.status) {

                    if (data.data.length > 0) {
                        $scope.ListQTTruNoTonDV = data.data.filter(x => x.TRANG_THAI !== 'P');
                        $scope.selectedRow = 0;
                        $scope.ShowDSSanPhamByKeHoach($scope.ListQTTruNoTonDV[0].ID_KH, 0);
                    }
                    else {
                        $scope.ListKHCapPhatConNo = [];
                        //Hiển thị thông báo
                        $scope.showMessage = true;
                        //$scope.$apply(); 
                    }
                    //$scope.$apply(); 
                    $scope.$evalAsync();

                }
            }
        });
    };

    $scope.LoadDsKeHoachTruNoTonDV($scope.DON_VI_ID, $scope.cbbNamNew, $scope.cbbKyNew);

    $scope.btnThemMoiTheoDs = function (loaiTC) {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/KeHoachCapPhat/_BoxDmTrangPhuc',
            controller: 'BoxDmTrangPhuc',
            size: 'lg',
            backdrop: 'static'
        });
        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            if (response !== undefined) {

                if (loaiTC === 'BX') {
                    angular.forEach(response, function (val, key) {
                        $scope.ListTrangPhucTruNoTonDV.push({ ID_KH_CT: 0, ID_SP: val.ID, SL_NO_CON_LAI: 0 });
                    });

                    //console.log($scope.ListTrangPhucTruNoTonDV);
                }
                else {
                    angular.forEach(response, function (val, key) {
                        $scope.ListTrangPhucTruNoTonDV.push({ ID_KH_CT: 0, ID_SP: val.ID, SL_NO_CON_LAI: 0 });
                    })
                }
            }
        });
    };


    //Lưu Kế Hoạch - Quyết Toán #trunoton
    $scope.LuuKHQuyetToanTrangPhuc = function () {
        //console.log('ID KH Đã chọn: ' + $scope.SelectedID_KH);
        //console.log($scope.ListTrangPhucTruNoTonDV);
        var dataSubmit = $scope.ListTrangPhucTruNoTonDV;

        if (dataSubmit.length > 0) {
            $scope.ListInsert = [];
            angular.forEach(dataSubmit, function (value, key) {
                $scope.itemModel = {};
                //$scope.itemModel.ID_KH_TH = value.ID_KH_TH ?? 0;
                $scope.itemModel.ID = value.ID ?? 0;
                $scope.itemModel.ID_KH = value.ID_KH ?? $scope.SelectedID_KH;
                $scope.itemModel.NAM = $scope.cbbNamNew;
                $scope.itemModel.KY = $scope.cbbKyNew;
                $scope.itemModel.ID_DON_VI = $scope.DON_VI_ID;
                $scope.itemModel.ID_SP = value.ID_SP;
                $scope.itemModel.MA_H55 = value.MA_H55 ?? '';
                $scope.itemModel.TEN_SP = value.TEN_SP ?? '';
                //$scope.itemModel.SL_KE_HOACH = value.SL_KE_HOACH ?? '';
                $scope.itemModel.SL_KE_HOACH = value.SL_NO_CON_LAI ?? 0;
                $scope.itemModel.SL_NO_KH = value.SL_NO_KH ?? 0;
                $scope.itemModel.SL_CAP_DU = value.SL_CAP_DU ?? 0;
                $scope.itemModel.SL_NO_PHEU_HV = value.SL_NO_PHEU_HV ?? 0;
                $scope.itemModel.SL_NO_CON_LAI = value.SL_NO_CON_LAI ?? 0;
                $scope.itemModel.SL_TRA_NO = value.SL_TRA_NO ?? 0;
                $scope.itemModel.SO_KH_DUOC_TRA_NO = value.SO_KH_DUOC_TRA_NO ?? '';
                $scope.itemModel.ID_KH_TRA_NO = value.ID_KH_TRA_NO ?? '';
                $scope.itemModel.SO_KH_TRA_NO = value.SO_KH_TRA_NO ?? '';
                $scope.ListInsert.push($scope.itemModel);
            });
        }
        //console.log($scope.ListInsert);

        if ($scope.ListInsert.length > 0) {
            $.ajax({
                type: 'post',
                url: '/KeHoachCapPhat/LuuKHQuyetToanTrangPhuc',
                data: { listTrangPhucTruNoTonDV: $scope.ListInsert },
                success: function (data) {
                    hideLoading();
                    //console.log(data);
                    if (data.Error == true) {
                        toastr.error(data.Title);
                    } else {
                        toastr.success("Lưu thông tin Quyết toán thành công!");
                        //$uibModalInstance.close($scope.CP_SelectKeHoachItem);
                        //$uibModalInstance.close($scope.DON_VI_ID, $scope.Nam_Hien_Tai, $scope.cbbKyNew, data.data);
                    }
                }
            });
        }

    };
    $scope.enableOrDisableXoa = function (listChecked) {
        if (listChecked.filter(x => x.Selected == true).length > 0)
            return false;
        else return true;
    }
    //Xử lý Check All khi chọn từng Kế hoạch dưới danh sách
    $scope.checkValDsKeHoach = function (index) {

        $scope.ListQTTruNoTonDV[index].Selected = !$scope.ListQTTruNoTonDV[index].Selected;
        if ($scope.ListQTTruNoTonDV.length > 0) {
            if ($scope.ListQTTruNoTonDV.every(x => x.Selected === true)) {
                $scope.selectAllKeHoach = true;
            } else {
                $scope.selectAllKeHoach = false;
            }
        }
        else {
            $scope.selectAllKeHoach = false;
        }
        $scope.disableXoa = $scope.enableOrDisableXoa($scope.ListQTTruNoTonDV);
        $scope.disableHuy = $scope.enableOrDisableXoa($scope.ListQTTruNoTonDV);
    };

    //Check all - Kế hoạch Trừ Nợ Tồn đơn vị
    $scope.checkAllDsKeHoach = function () {
        if ($scope.ListQTTruNoTonDV.length > 0) {
            angular.forEach($scope.ListQTTruNoTonDV, function (item) {
                item.Selected = event.target.checked;
            });
        }

        $scope.disableXoa = $scope.enableOrDisableXoa($scope.ListQTTruNoTonDV);
        $scope.disableHuy = $scope.enableOrDisableXoa($scope.ListQTTruNoTonDV);

    };

    $scope.HuyThayDoiQTTruNoTonDV = function () {
        //Bỏ lựa chọn
        if ($scope.ListQTTruNoTonDV.length > 0) {
            angular.forEach($scope.ListQTTruNoTonDV, function (item) {
                item.Selected = false;
            });
        }
        $scope.disableXoa = true;
        $scope.disableHuy = true;
        $scope.selectAllKeHoach = false;
    };

    //Check all - Trang Phục
    $scope.CheckAllTrangPhuc = function () {

        if ($scope.ListTrangPhucTruNoTonDV.length > 0) {
            angular.forEach($scope.ListTrangPhucTruNoTonDV, function (item) {
                item.Selected = event.target.checked;
            });
        }
        //$scope.disableLuuTrangPhuc = $scope.enableOrDisableXoa($scope.ListTrangPhucTruNoTonDV);
        //$scope.disableXoaTrangPhuc = $scope.enableOrDisableXoa($scope.ListTrangPhucTruNoTonDV);
        //$scope.disableHuyTrangPhuc = $scope.enableOrDisableXoa($scope.ListTrangPhucTruNoTonDV);
        $scope.disabledTrangPhuc = $scope.enableOrDisableXoa($scope.ListTrangPhucTruNoTonDV);
    };
    $scope.checkValDsTrangPhuc = function (index) {
        $scope.ListTrangPhucTruNoTonDV[index].Selected = !$scope.ListTrangPhucTruNoTonDV[index].Selected;
        if ($scope.ListTrangPhucTruNoTonDV.length > 0) {
            if ($scope.ListTrangPhucTruNoTonDV.every(x => x.Selected === true)) {
                $scope.selectAllTrangPhuc = true;
            } else {
                $scope.selectAllTrangPhuc = false;
            }
        }
        else {
            $scope.selectAllTrangPhuc = false;
        }

        $scope.disabledTrangPhuc = $scope.enableOrDisableXoa($scope.ListTrangPhucTruNoTonDV);
    }

    $scope.HuyKHQuyetToanTrangPhuc = function () {

        $scope.disabledTrangPhuc = true;
        //Bỏ lựa chọn
        if ($scope.ListTrangPhucTruNoTonDV.length > 0) {
            angular.forEach($scope.ListTrangPhucTruNoTonDV, function (item) {
                item.Selected = false;
            });
        }
        $scope.selectAllTrangPhuc = false;
    }

    $scope.ChangeEnableTrangPhuc = function () {
        $scope.disabledTrangPhuc = false;
    }
    //Xóa Kế hoạch Quyết toán Trừ nợ tồn đơn vị #xoaquyettoan #xoatrunotondv
    $scope.XoaQTTruNoTonDV = function () {
        //console.log('Xóa Quyết toán - Kế hoạch');
        $scope.dsKeHoachDaChonXoa = $scope.ListQTTruNoTonDV.filter(x => x.Selected == true);
        $scope.dsKeHoachDaChonXoa.forEach(x => {
            x.SO_KH_DUOC_TRA_NO = x.SO_KH
        });
        //console.log($scope.dsKeHoachDaChonXoa);
        if ($scope.dsKeHoachDaChonXoa.length > 0) {
            $.ajax({
                type: 'post',
                url: '/KeHoachCapPhat/XoaKHQuyetToanTrangPhuc',
                data: { dsKeHoachDaChonXoa: $scope.dsKeHoachDaChonXoa },
                success: function (data) {
                    hideLoading();
                    //console.log(data);
                    if (data.Error == true) {
                        $scope.showMessage = true;
                        $scope.messageContent = data.Title;
                        $scope.$apply();
                        toastr.error(data.Title);
                    } else {
                        $scope.LoadDsKeHoachTruNoTonDV($scope.DON_VI_ID, $scope.cbbNamNew, $scope.cbbKyNew);
                        toastr.success("Xóa Quyết toán thành công!");
                    }
                }
            });
        }
    };
    //Xóa Trang Phục của Kế hoạch-Quyết toán #xoatrangphuc
    $scope.XoaKHQuyetToanTrangPhuc = function () {
        $scope.dsTrangPhucDaChonXoa = $scope.ListTrangPhucTruNoTonDV.filter(x => x.Selected == true);

        if ($scope.dsTrangPhucDaChonXoa.length > 0) {
            $.ajax({
                type: 'post',
                url: '/KeHoachCapPhat/XoaTrangPhucKeHoachQT',
                data: { dsTrangPhucDaChonXoa: $scope.dsTrangPhucDaChonXoa },
                success: function (data) {
                    hideLoading();
                    //console.log(data);
                    if (data.Error == true) {
                        $scope.showMessageTP = true;
                        $scope.messageContentTP = data.Title;
                        toastr.error(data.Title);
                    } else {
                        toastr.success("Xóa Quyết toán thành công!");
                        $scope.ListTrangPhucTruNoTonDV = $scope.ListTrangPhucTruNoTonDV.filter(x => x.Selected != true)
                    }
                    $scope.$apply();

                }
            });
        }
    }

    $scope.showHideMessageTP = function () {
        $scope.showMessageTP = false;
    }

    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});

app.controller('QuyetToanPhieuHoiVe', function ($scope, $rootScope, $uibModalInstance, $ngConfirm, showToast, hideLoading) {
    $scope.model = {};
    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});
app.controller('BoxDangKyNhanTien', function ($scope, $rootScope, $uibModalInstance, $uibModal, $ngConfirm, showToast, hideLoading, data) {

    // Có hiển thị cột checkbox ở đầu không
    $scope.tree_Table_Checkbox_dknt = false;
    // Model data
    $scope.tree_data_dknt = [];
    $.ajax({
        type: 'post',
        async: false,
        url: '/KeHoachCapPhat/SLDKNhanTienVaHienVat',
        data: { khId: data.khId, dvId: data.dvId, iNam: data.nam, iKy: data.ky },
        success: function (data) {
            if (!data.status) {
                //console.log(data.data);
                $scope.tree_data_dknt = data.data;
            } else {
                toastr.error(data.message);
            }
            hideLoading();
        }
    });

    // cấu hình hiển thị cùng với icon
    $scope.expanding_property_dknt = {
        field: "TreeThuTu",
        displayName: "TT",
        width: "5%",
        rowspan: 2
    };
    // Cấu hình các cột sẽ hiển thị tiếp theo
    $scope.col_defs_dknt = [[
        { field: "TreeName", displayName: "Đối tượng", width: "60%", rowspan: 2 },
        { field: "TreeDonViTinh", displayName: "ĐVT", width: "5%", rowspan: 2 },
        { field: "TreeNtNam", displayName: "Nhận tiền", colspan: 2 },
        { field: "TreeNtNu", displayName: "Hiện vật", colspan: 2 },
    ],
    [
        { field: "TreeNtNam", displayName: "Nam", width: "5%" },
        { field: "TreeNtNu", displayName: "Nữ", width: "5%" },
        { field: "TreeHvNam", displayName: "Nam", width: "5%" },
        { field: "TreeHvNu", displayName: "Nữ", width: "5%" }
    ]
    ];
    $scope.body_defs_dknt = [{ field: "TreeName" }, { field: "TreeDonViTinh" }, { field: "TreeNtNam" }, { field: "TreeNtNu" }, { field: "TreeHvNam" }, { field: "TreeHvNu" }];
    // event when change value checkbox
    $scope.ChangeTreeTableCheckbox_dknt = function (item) {
    };
    // event when click choose row
    $scope.ClickTreeTableRow_dknt = function (item) {
        console.log(item);
    };

    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});
app.controller('BoxDanhSachSPDoiTC', function ($scope, $rootScope, $uibModalInstance, $ngConfirm, showToast, hideLoading, datamodel) {
    showToast();
    $scope.TempDanhSachSPDoiTC = [];
    hideLoading();
    $.ajax({
        type: 'post',
        async: false,
        url: '/KeHoachCapPhat/GetDsDoiTieuChuanChuaDuyet',
        data: { khId: datamodel.khId, dvId: datamodel.dvId, iNam: datamodel.nam, iKy: datamodel.ky },
        success: function (data) {
            if (data.status) {
                $scope.TempDanhSachSPDoiTC = data.data;
                //$scope.$apply();
            } else {
                toastr.error(data.message);
            }
            hideLoading();
        }
    });
    $scope.checkAllSPDoiTC = function () {
        $scope.ListSanPhamNhan = [];
        if ($scope.TempDanhSachSPDoiTC.length > 0) {
            angular.forEach($scope.TempDanhSachSPDoiTC, function (item) {
                item.Selected = event.target.checked;
                if ($scope.selectAll)
                    $scope.GetSanPhamDoiCT(item.ID_SP);
            });
        }
    };
    $scope.ChonSanPhamDoiTC = function () {
        $scope.itemSelected = [];
        if ($scope.TempDanhSachSPDoiTC.length > 0) {
            angular.forEach($scope.TempDanhSachSPDoiTC, function (val, key) {
                if (val.Selected)
                    $scope.itemSelected.push(val);
            })
            $uibModalInstance.close($scope.itemSelected);
        }
        else {
            toastr.error("Không có sản phẩm nào được chọn!");
        }
    }

    $scope.CheckSPDoi = function () {
        var countChecked = 0;
        $scope.selectAll = false;
        $scope.ListSanPhamNhan = [];
        if ($scope.TempDanhSachSPDoiTC.length > 0) {
            angular.forEach($scope.TempDanhSachSPDoiTC, function (val, key) {
                if (val.Selected) {
                    countChecked++;
                    $scope.GetSanPhamDoiCT(val.ID_SP);
                }
            })

            if (countChecked == $scope.TempDanhSachSPDoiTC.length)
                $scope.selectAll = true;
        }

    }

    $scope.ListSanPhamNhan = [];
    $scope.GetSanPhamDoiCT = function (spId) {
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoachCapPhat/DS_SPDoi_ChiTiet',
            data: { spNguonId: spId },
            success: function (data) {
                hideLoading();
                if (data.status && data.data !== undefined) {
                    angular.forEach(data.data, function (val, key) {

                        var checkSP = $scope.ListSanPhamNhan.filter(x => x.ID_SP === val.ID_SP);
                        if (checkSP != null && checkSP.length > 0) {
                            angular.forEach($scope.ListSanPhamNhan, function (spn, key) {
                                if (spn.ID_SP == val.ID_SP) {
                                    spn.SL_DOI += val.SL_DOI;
                                }
                            });
                        } else {
                            $scope.ListSanPhamNhan.push(val);
                        }
                    })
                }
            }
        });
    }

    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});
app.controller('BoxLanhDaoPheDuyet', function ($scope, $rootScope, $uibModalInstance, $ngConfirm, showToast, hideLoading, datamodel) {
    showToast();
    $scope.DsLanhDaoKyDuyet = [{ USER_ID: 0, FULL_NAME: '- Chọn lãnh đạo duyệt kế hoạch', Selected: true }];
    $scope.cbbDsLanhDaoKyDuyet = '0';
    hideLoading();

    $.ajax({
        type: 'post',
        async: false,
        url: '/KeHoachCapPhat/GetDsLanhDaoPheDuyet',
        data: {},
        success: function (data) {
            if (data.status) {
                angular.forEach(data.data, function (val, key) {
                    $scope.DsLanhDaoKyDuyet.push({ USER_ID: val.USER_ID, UNIT_NAME: val.UNIT_NAME, FULL_NAME: val.FULL_NAME });
                })
            } else {
                toastr.error(data.message);
            }
            hideLoading();
        }
    });

    $scope.CP_GuiPheDuyet = function () {
        if ($scope.cbbDsLanhDaoKyDuyet > 0) {
            $.ajax({
                type: 'post',
                async: false,
                url: '/KeHoachCapPhat/GuiPheDuyetKH',
                data: { khId: datamodel.khId, ngDuyetId: $scope.cbbDsLanhDaoKyDuyet, noiDung: $scope.txtarNoiDungTrinh },
                success: function (data) {
                    if (data.status) {
                        toastr.success(data.message);
                        $uibModalInstance.close(1);
                    } else {
                        toastr.error(data.message);
                    }
                    hideLoading();
                }
            });
        }
        else {
            toastr.error("Bạn chưa chọn lãnh đạo ký duyệt!");
        }
    }

    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});
