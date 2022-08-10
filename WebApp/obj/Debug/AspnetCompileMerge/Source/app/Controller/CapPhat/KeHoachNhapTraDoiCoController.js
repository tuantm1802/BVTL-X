app.controller("KeHoachNhapTraDoiCoController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.listTrangPhucAddView = [];
    $scope.ListKeHoach = [];
    $scope.ListKeHoachChoXl = [];
    $scope.selectedRowKeHoach = -1;
    $scope.DonViId = 0;
    $scope.TenDonVi = "";
    $scope.ViewKeHoach = false;
    //Thông tin cấp phát  
    $scope.txtDonVi = "";
    $scope.txtNam = "";
    $scope.txtLyDoCap = "";
    $scope.rdoDonVi = 1;
    $scope.listPhuongThucVanChuyen = [];
    $scope.dsHangHoaDaChon = [];

    $scope.ListNam = [];
    var currentYear = new Date().getFullYear();
    for (var i = currentYear; i > currentYear - 7; i--) {
        $scope.ListNam.push({ Id: i, Name: i });
    }
    $scope.cbbNam = currentYear.toString();
    $scope.LoaiSanPham = 'NHAP';

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
            $scope.ViewKeHoach = false;
            $scope.DonViId = $scope.DON_VI_ID;
            $scope.TenDonVi = $scope.TEN_DON_VI;
            //Giảm hiệu ứng đơ khi cho chọn đơn vị rồi mới load dữ liệu

            //Danh sách kế hoạch cấp phát
            $scope.LoadDsKeHoach($scope.DON_VI_ID, $scope.cbbNam, $scope.cbbKy);

            //Disable các nút chức năng khi đã duyệt KH
            $scope.disableKHDuyet = false;

            if ($scope.ListKeHoach.length > 0) {
                $scope.tempKeHoach = $scope.ListKeHoach.filter(x => x.TRANG_THAI === 'U' || x.TRANG_THAI === 'I' || x.TRANG_THAI === 'R');
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
        url: '/KeHoachNhapTraDoiCo/GetTreeData',
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


    $scope.disabledDelete = true;
    $scope.disabledRollback = true;
    $scope.disabledEdit = true;
    $scope.disabledSave = true;
    $scope.disabledSendApprove = true;
    $scope.disabledPrint = true;

    $scope.RoleBtnCreate = false;
    $scope.RoleBtnSearch = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnRollback = false;
    $scope.RoleBtnUpdate = false;
    $scope.RoleBtnSave = false;
    $scope.RoleBtnSendApprove = false;
    $scope.RoleBtnPrint = false;
    $scope.dmKhos = [];
    $scope.HsStatus = true;
    $scope.hinhThucVanChuyens = [];
    $scope.NguoiKys = [];
    GetDanhMuc = () => {
        $('input[name="NGUOI_LAP"]').prop('disabled', true);
        $scope.dmKhos = [];
        $scope.NguoiKys = [];
        $scope.hinhThucVanChuyens = [];
        $scope.CurrentUserName = "";
        showToast();
        try {
            $.ajax({
                type: 'post',
                url: '/KeHoachNhapTraDoiCo/GetDanhMuc',
                data: {},
                success: function (response) {
                    if (response.Status === 200) {
                        $scope.dmKhos = response.Khos;
                        $scope.hinhThucVanChuyens = response.HinhThucVanChuyens;
                        $scope.NguoiKys = response.nguoiKyDuyets;
                        $scope.CurrentUserName = response.userName;
                    }

                    if (response.capPhatDVId > 0) {
                        $scope.TenDonVi = response.capPhatDVTen;
                        $scope.DonViId = response.capPhatDVId;
                        $scope.IsShowRight = true;
                        $scope.IsVisibleTab = true;
                        $scope.ViewKeHoach = true;
                    }

                    if (response.Buttoms != null) {
                        angular.forEach(response.Buttoms, function (item) {
                            if (item == 'btnSearch') {
                                $scope.RoleBtnSearch = true;
                            }
                            if (item == 'btnCreate') {
                                $scope.RoleBtnCreate = true;
                            }
                            if (item == 'btnDelete') {
                                $scope.RoleBtnDelete = true;
                            }
                            if (item == 'btnRollback') {
                                $scope.RoleBtnRollback = true;
                            }
                            if (item == 'btnUpdate') {
                                $scope.RoleBtnUpdate = true;
                            }
                            if (item == 'btnSave') {
                                $scope.RoleBtnSave = true;
                            }
                            if (item == 'btnSendApprove') {
                                $scope.RoleBtnSendApprove = true;
                            }
                            if (item == 'btnPrint') {
                                $scope.RoleBtnPrint = true;
                            }
                        });
                    }

                    $scope.$apply();
                }, error: (e) => {
                    console.log(e);
                },
                complete: () => {
                    hideLoading();
                }
            });

            $scope.TimKiem();
        } catch (e) {
            console.log(e);
        }
    }
    GetDanhMuc();

    //#endregion
    //Check kiểu hiển thị tree Đơn vị
    $scope.ChangeTreeViewDmDonVi = function (type) {
        $.ajax({
            type: 'get',
            async: false,
            url: '/KeHoachNhapTraDoiCo/GetTreeData',
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
        $scope.ListKeHoach = [];
        $.ajax({
            type: 'get',
            async: false,
            url: '/KeHoachNhapTraDoiCo/GetTreeData',
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


    $scope.idx_ChangeKHTheoNam = function () {
        $scope.LoadDsKeHoach();
    }

    //Danh sách kế hoạch theo ĐV, Năm, Kỳ
    $scope.LoadDsKeHoach = function () {
        $scope.ListKeHoach = [];
        if ($scope.DonViId > 0) {
            $scope.selectedRowKeHoach = -1;
            $.ajax({
                type: 'post',
                async: false,
                url: '/KeHoachNhapTraDoiCo/TimKiemKeHoach',
                data: { donViId: $scope.DonViId, nam: parseInt($scope.cbbNam), pageSize: 1000000, pageNumber: 1 },
                success: function (data) {
                    hideLoading();
                    if (data.Status == 200) {
                        if (data.Data.length > 0) {
                            $scope.ListKeHoach = data.Data;
                        }
                        else {
                            $scope.ListKeHoach = [];
                        }

                    }
                }
            });
        }
        else {
            toastr.error("Vui lòng chọn đơn vị!");
        }
    };



    $scope.ThemKeHoach = () => {
        $scope.HsStatus = false;

        $scope.ViewKeHoach = true;

        $scope.disabledDelete = true;
        $scope.disabledRollback = true;
        $scope.disabledEdit = true;
        $scope.disabledSendApprove = true;
        $scope.disabledPrint = true;

        $scope.disabledSave = false;
        $scope.dsHangHoaDaChon = [];
        $scope.dsPhieuNhapKho = [];
        $scope.dsPhieuXuatKho = [];
        $scope.dsHangHoaNoKH = [];
        $scope.thongTinKh = {};
        $scope.thongTinKh = {
            'NGAY_TAO': moment(new Date()).format('DD/MM/YYYY'),
            TEN_DON_VI: $scope.TenDonVi,
            ID_DON_VI: $scope.DonViId
        }

        if ($scope.dmKhos != null && $scope.dmKhos.length > 0) {
            $scope.thongTinKh.ID_KHO_NHAP = $scope.dmKhos[0].ID;
        }

        if ($scope.hinhThucVanChuyens != null && $scope.hinhThucVanChuyens.length > 0) {
            $scope.thongTinKh.PHUONG_THUC_VAN_CHUYEN = $scope.hinhThucVanChuyens[0].FLEX_VALUE_ID;
        }

    }


    ////////////////////////////////////// Thêm hàng hóa ////////////////////////////////////

    $scope.dsHangHoaChon = [];
    $scope.dsHangHoaNhapDaChon = [];
    $scope.dsHangHoaXuatDaChon = [];
    $scope.SanPhamNhapId = 0;
    $scope.SanPhamXuatId = 0;

    // Thêm hàng hóa
    $scope.ThemHangHoa = () => {
        $scope.SoLuong = 0;
        GetDanhSachHangHoa();
        $('#them_hang_hoa').modal('show');
    }

    // Tìm kiếm hàng hóa
    $scope.AddHH_TimKiem = () => {
        GetDanhSachHangHoa();
    }

    // Lấy danh sách hàng hóa của kế hoạch cấp phát theo đơn vị
    GetDanhSachHangHoa = () => {
        if ($scope.DonViId > 0) {
            showToast();
            $.ajax({
                type: 'post',
                url: '/KeHoachNhapTraDoiCo/DanhSachHangHoa',
                data: {
                    keyword: $scope.AddHH_SearchSanPhamText,
                    donViId: $scope.DonViId,
                    nam: $scope.cbbNam
                },
                success: function (response) {
                    if (response.Status === 200) {
                        $scope.dsHangHoaNhapChon = response.Data;
                        $scope.dsHangHoaXuatChon = response.SPXuats;

                        var dsHHN = [];
                        var dsHHX = [];

                        // Bỏ các hàng hóa đã chọn
                        if ($scope.dsHangHoaDaChon != null && $scope.dsHangHoaDaChon.length > 0) {
                            for (var i = 0; i < $scope.dsHangHoaNhapChon.length; i++) {
                                var hhs = $scope.dsHangHoaDaChon.filter(x => x.ID_SP_NHAP == $scope.dsHangHoaNhapChon[i].ID_SP);
                                if (hhs == null || hhs.length == 0) {
                                    dsHHN.push($scope.dsHangHoaNhapChon[i]);
                                }
                            }

                            for (var i = 0; i < $scope.dsHangHoaXuatChon.length; i++) {
                                var hhs = $scope.dsHangHoaDaChon.filter(x => x.ID_SP_XUAT == $scope.dsHangHoaXuatChon[i].ID_SP);
                                if (hhs == null || hhs.length == 0) {
                                    dsHHX.push($scope.dsHangHoaXuatChon[i]);
                                }
                            }

                        }

                        if (dsHHN != null && dsHHN.length > 0) {
                            $scope.dsHangHoaNhapChon = dsHHN;
                        }

                        if (dsHHX != null && dsHHX.length > 0) {
                            $scope.dsHangHoaXuatChon = dsHHX;
                        }

                    }
                    $scope.$apply();
                }, error: (e) => {
                    console.log(e);
                },
                complete: () => {
                    hideLoading();
                }
            });
        }
        else {
            toastr.error("Vui lòng chọn đơn vị!");
        }
    }

    $scope.ChonTatCaHangHoa = (type) => {
        
        if (type === 2) {
            $scope.dsHangHoaDaChon.forEach(x => x.SELECT = !$scope.checkAllHangHoa);
        }
    }

    $scope.TichChonHangHoa = (type, index) => {
        
        if (type === 2) {
            $scope.dsHangHoaDaChon[index].SELECT = !$scope.dsHangHoaDaChon[index].SELECT;
            if ($scope.dsHangHoaDaChon.length > 0) {
                if ($scope.dsHangHoaDaChon.every(x => x.SELECT === true)) {
                    $scope.checkAllHangHoa = true;
                } else {
                    $scope.checkAllHangHoa = false;
                }
            }
            else {
                $scope.checkAllHangHoa = false;
            }
        }
    }

    // Xóa hàng hóa đã chọn
    $scope.XoaHangHoaDaChon = () => {
        var hangHoa = $scope.dsHangHoaDaChon.filter(x => x.SELECT !== true);
        if (hangHoa != null && hangHoa.length > 0) {
            $scope.dsHangHoaDaChon = hangHoa;
        }
        else {
            $scope.dsHangHoaDaChon = [];
        }

    }


    // Lưu chọn hàng hóa
    $scope.LuuDsHangHoa = () => {
        if ($scope.SanPhamNhapId > 0 && $scope.SanPhamXuatId > 0) {
            if ($scope.SanPhamNhapId == $scope.SanPhamXuatId) {
                toastr.error('Sản phẩm nhập đổi cỡ không được giống Sản phẩm xuất đổi cỡ!');
            } else {
                if ($scope.SoLuong > 0) {
                    var hhNhap = $scope.dsHangHoaNhapChon.filter(x => x.ID_SP === $scope.SanPhamNhapId);
                    var hhXuat = $scope.dsHangHoaXuatChon.filter(x => x.ID_SP === $scope.SanPhamXuatId);
                    var hangHoa = {
                        TEN_DVT: hhNhap[0].TEN_DVT,
                        ID_KH: 0,
                        ID_SP_NHAP: $scope.SanPhamNhapId,
                        TEN_SP_NHAP: hhNhap[0].TEN_SP,
                        ID_SP_XUAT: $scope.SanPhamXuatId,
                        TEN_SP_XUAT: hhXuat[0].TEN_SP,
                        ID_SP_NHAP_CHA: hhNhap[0].ID_SP_CHA,
                        ID_SP_XUAT_CHA: hhXuat[0].ID_SP_CHA,
                        SL_KE_HOACH: $scope.SoLuong,
                        SELECT: false
                    };
                    $scope.dsHangHoaDaChon.push(hangHoa);
                    $('#them_hang_hoa').modal('hide');
                } else {
                    toastr.error('Vui lòng nhập số lượng!');
                }
            }
        } else {
            toastr.error('Vui lòng chọn Sản phẩm nhập đổi cỡ hoặc Sản phẩm xuất đổi cỡ!');
        }
    }

    // Check dữ liệu thông tin chung khi thêm, sửa kế hoạch
    ThongTinChungValidate = () => {
        $scope.CheckThongTinChungMsg = 'Vui lòng nhập các thông tin sau:';
        $scope.CheckThongTinChung = false;
        $scope.ListMsg = [];
        if ($scope.thongTinKh.ID_KHO_NHAP == null || $scope.thongTinKh.ID_KHO_NHAP == 0 || $scope.thongTinKh.ID_KHO_NHAP == undefined) {
            $scope.CheckThongTinChung = true;
            $scope.ListMsg.push("Kho nhập");
            $scope.thongTinKh.ID_KHO_NHAP == 0;
        }

        if ($scope.thongTinKh.NGAY_TAO == null || $scope.thongTinKh.NGAY_TAO == '' || $scope.thongTinKh.NGAY_TAO == undefined) {
            $scope.CheckThongTinChung = true;
            $scope.ListMsg.push("Ngày lập kế hoạch");
            $scope.thongTinKh.NGAY_TAO == '';
        }

        if ($scope.thongTinKh.PHUONG_THUC_VAN_CHUYEN == null || $scope.thongTinKh.PHUONG_THUC_VAN_CHUYEN == '' || $scope.thongTinKh.PHUONG_THUC_VAN_CHUYEN == undefined) {
            $scope.CheckThongTinChung = true;
            $scope.ListMsg.push("Hình thức vận chuyển");
            $scope.thongTinKh.PHUONG_THUC_VAN_CHUYEN == 0;
        }

        if ($scope.thongTinKh.MO_TA == undefined) {
            $scope.thongTinKh.MO_TA == '';
        }

        if ($scope.ListMsg != null && $scope.ListMsg.length > 0) {
            for (var i = 0; i < $scope.ListMsg.length; i++) {
                if (i == 0) {
                    $scope.CheckThongTinChungMsg += ' ' + $scope.ListMsg[i];
                } else {
                    $scope.CheckThongTinChungMsg += ' ;' + $scope.ListMsg[i];
                }
            }
        }
    }


    // Thêm mói hoặc nhập kế hoạch
    $scope.LuuPhieu = () => {
        showToast();
        try {
            $scope.CheckThongTinChungMsg = '';
            $scope.CheckThongTinChung = false;
            ThongTinChungValidate();
            if ($scope.CheckThongTinChung == false) {

                $scope.thongTinKh.NGAY_TAO_TEXT = moment($scope.thongTinKh.NGAY_TAO, 'DD/MM/YYYY').format('DD/MM/YYYY');
                if ($scope.thongTinKh.NGAY_NHAP_TT_W != null && $scope.thongTinKh.NGAY_NHAP_TT_W != '')
                    $scope.thongTinKh.NGAY_NHAP_TT = moment($scope.thongTinKh.NGAY_NHAP_TT_W, 'DD/MM/YYYY').format('YYYYMMDD');

                $scope.thongTinKh.TRANG_THAI = 'I';
                $scope.thongTinKh.ID_DON_VI = $scope.DonViId;
                $scope.thongTinKh.NAM = $scope.cbbNam;

                if ($scope.thongTinKh.ID_KH != null && $scope.thongTinKh.ID_KH > 0) {
                    $.ajax({
                        type: 'post',
                        url: '/KeHoachNhapTraDoiCo/UpdateKeHoach',
                        data: {
                            thongTinPhieu: $scope.thongTinKh,
                            dsHangHoa: $scope.dsHangHoaDaChon
                        },
                        success: function (response) {
                            if (response.Status === false) {
                                toastr.success('Cập nhật thông tin kế hoạch thành công!');
                                LayThongTinKeHoachTheoId();
                            }
                            else {
                                toastr.error(response.Message);
                            }
                            console.log(response);
                            $scope.$apply();
                        }, error: (e) => {
                            toastr.error('Có lỗi xảy ra trong quá trình cập nhật!');
                        }
                    });
                } else {
                    $.ajax({
                        type: 'post',
                        url: '/KeHoachNhapTraDoiCo/TaoKeHoach',
                        data: {
                            thongTinPhieu: $scope.thongTinKh,
                            dsHangHoa: $scope.dsHangHoaDaChon
                        },
                        success: function (response) {
                            if (response.Status == false) {
                                toastr.success('Lưu thông tin kế hoạch thành công!');
                                $scope.thongTinKh.SO_KH = response.soKH;
                                $scope.thongTinKh.ID_KH = response.keHoachId;
                                $scope.LoadDsKeHoach();
                            }
                            else {

                                toastr.error(response.Message);
                            }
                            console.log(response);
                            $scope.$apply();
                        }, error: (e) => {
                            toastr.error('Có lỗi xảy ra trong quá trình lưu!');
                        }
                    });
                }
            } else {
                toastr.error($scope.CheckThongTinChungMsg);
            }
        }
        catch (e) {
            console.log(e);
            toastr.error('Có lỗi xảy ra trong quá trình lưu!');
        }
        finally {
            hideLoading();
        }
    }

    $scope.SuaKeHoach = () => {
        if ($scope.thongTinKh.TRANG_THAI === 'I' || $scope.thongTinKh.TRANG_THAI === 'R') {
            $scope.HsStatus = false;
            $scope.disabledSave = false;
            $scope.disabledRollback = false;
        }
    }


    $scope.HuyThayDoi = () => {
        LayThongTinKeHoachTheoId();
    }

    LayThongTinKeHoachTheoId = () => {
        showToast();
        $.ajax({
            type: 'post',
            url: '/KeHoachNhapTraDoiCo/LayKeHoachTheoId',
            data: {
                id: $scope.thongTinKh.ID_KH
            },
            success: function (response) {
                hideLoading();
                if (response.Status === 200) {
                    for (var i = 0; i < $scope.ListKeHoach.length; i++) {
                        if ($scope.ListKeHoach[i].ID_KH == $scope.thongTinKh.ID_KH) {
                            $scope.ListKeHoach[i].TRANG_THAI = response.Data.TRANG_THAI;
                            $scope.ListKeHoach[i].TRANG_THAI_TEXT = response.Data.TRANG_THAI_TEXT;
                            $scope.ShowThongTin(response.Data, i);
                        }
                    }
                }

                $scope.$apply();
            }, error: (e) => {
                toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm!');
            },
            complete: () => {
                hideLoading();
            }
        });
    }


    $scope.ShowThongTin = (item, index) => {
        showToast();
        try {

            $scope.HsStatus = true;
            $scope.ViewKeHoach = true;
            $scope.disabledDelete = true;
            $scope.disabledRollback = true;
            $scope.disabledEdit = true;
            $scope.disabledSendApprove = true;
            $scope.disabledSave = true;

            $scope.selectedRowKeHoach = index;
            $scope.thongTinKh = item;
            if ($scope.thongTinKh.NGAY_TAO_TEXT != null && $scope.thongTinKh.NGAY_TAO_TEXT != '') {
                $scope.thongTinKh.NGAY_TAO = $scope.thongTinKh.NGAY_TAO_TEXT;
            }
            if ($scope.thongTinKh.NGAY_NHAP_TT != null && $scope.thongTinKh.NGAY_NHAP_TT.length === 8) {
                $scope.thongTinKh.NGAY_NHAP_TT_W = moment($scope.thongTinKh.NGAY_NHAP_TT, 'YYYYMMDD').format('DD/MM/YYYY');
            }

            $scope.disabledPrint = false;

            if ($scope.thongTinKh.TRANG_THAI === 'I' || $scope.thongTinKh.TRANG_THAI === 'R') {
                $scope.disabledEdit = false;
                $scope.disabledDelete = false;
                $scope.disabledSendApprove = false;
            }

            ThongTinHangHoa(item.ID_KH);
            LayDanhSachPhieuXuatKho();
        } catch (e) {
            toastr.error("Có lỗi xảy ra khi lấy thông tin " + e);
            console.error(e);
        }
        finally {
            hideLoading();
        }
    }


    ThongTinHangHoa = (id) => {
        $scope.dsHangHoa = [];
        $.ajax({
            type: 'POST',
            dataType: 'json',
            cache: false,
            async: false,
            url: '/KeHoachNhapTraDoiCo/DanhSachHangHoaTheoKeHoach',
            data: {
                idKeHoach: id
            },
            success: function (response) {
                if (response.Status === 200) {
                    $scope.dsHangHoaDaChon = response.Data;
                }

                //$scope.$apply();
            }, error: (e) => {
                toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm!');
            }
        });
    }

    // Trình duyệt kế hoạch
    $scope.TrinhDuyet = () => {
        if ($scope.thongTinKh.ID_KH > 0) {
            $('#ChonNguoiDuyet *').prop('disabled', false);
            if ($scope.dsHangHoaDaChon == null || $scope.dsHangHoaDaChon.length == 0) {
                toastr.error('Cần thêm hàng hóa để trình duyệt!');
            } else {
                if ($scope.thongTinKh.TRANG_THAI === 'I' || $scope.thongTinKh.TRANG_THAI === 'R') {
                    $('#ChonNguoiDuyet').modal('show');
                }
            }
        }
        else {
            toastr.error('Vui lòng chọn kế hoạch cần trình duyệt!');
        }
    }

    $scope.XacNhanTrinhDuyet = () => {
        if ($scope.NguoiDuyet > 0) {
            showToast();
            $.ajax({
                type: 'post',
                url: '/KeHoachNhapTraDoiCo/TrinhDuyetKeHoach',
                data: {
                    idKh: $scope.thongTinKh.ID_KH,
                    nguoiDuyet: $scope.NguoiDuyet
                },
                success: function (response) {
                    hideLoading();
                    if (response.Status === false) {
                        $('#ChonNguoiDuyet').modal('hide');
                        toastr.success(response.Message);
                        LayThongTinKeHoachTheoId();
                    }
                    else {
                        toastr.error(response.Message);
                    }
                    console.log(response);
                    $scope.$apply();
                }, error: (e) => {
                    hideLoading();
                    toastr.error('Có lỗi xảy ra trong quá trình trình duyệt!');
                }
            });
        } else {
            toastr.error('Vui lòng chọn Người duyệt!');
        }
    }

    // Đồng ý xóa hoặc duyệt
    $scope.CheckPassword = function () {

        // Kiểm tra password
        $.ajax({
            type: 'post',
            async: false,
            url: '/Login/CheckPassword',
            data: { password: $scope.TxtPassword },
            success: function (res) {
                if (res.Error == false) {
                    $('#EnterPassword').modal('hide');
                    $scope.MessagePassword = '';
                    // Duyệt
                    if ($scope.IsClickDuyetKeHoach) {
                        DongYDuyetKehoach();
                    }

                    // Xóa
                    if ($scope.IsClickXoaKeHoach) {
                        DongYXoaKehoach();
                    }
                } else {
                    $scope.MessagePassword = res.Title;
                }
            }
        });
    };

    DongYXoaKehoach = () => {
        $scope.IsClickDuyetKeHoach = false;
        $scope.IsClickXoaKeHoach = false;
        showToast();
        $.ajax({
            type: 'POST',
            dataType: 'json',
            cache: false,
            async: false,
            url: '/KeHoachNhapTraDoiCo/XoaKeHoach',
            data: {
                idKh: $scope.thongTinKh.ID_KH
            },
            success: function (response) {
                hideLoading();
                if (response.Status == false) {
                    toastr.success(response.Message);
                    LayThongTinKeHoachTheoId();
                } else {
                    toastr.error(response.Message);
                }
                //$scope.$apply();
            }, error: (e) => {
                hideLoading();
                toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm!');
            }
        });
    }

    // xóa kế hoạch
    $scope.XoaKeHoach = () => {
        if ($scope.thongTinKh.ID_KH > 0) {
            showToast();
            try {
                if ($scope.thongTinKh.TRANG_THAI == 'A') {
                    $scope.TxtPassword = "";
                    $('#EnterPassword *').prop('disabled', false);
                    $scope.IsClickDuyetKeHoach = false;
                    $scope.IsClickXoaKeHoach = true;
                    $('#EnterPassword').modal('show');
                }
                else {
                    DongYXoaKehoach();
                }
            }
            catch (e) {
                console.log(e);
                toastr.error('Có lỗi xảy ra trong quá trình xóa!');
            }
            finally {
                hideLoading();
            }
        }
        else {
            toastr.error('Vui lòng chọn kế hoạch cần xóa!');
        }
    }


    /////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////// Lập phiếu xuất, nhập kho //////////////////////////////////////
    // View chi tiết phiếu
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
            $scope.LPXK_TitlePopupChiTiet = "Phiếu xuất kho từ: kho " + item.TEN_KHO;
        }
        else {
            $scope.LPXK_TitlePopupChiTiet = "Phiếu nhập kho vào: kho " + item.TEN_KHO;
        }

        $scope.LPXK_ListPXK_Detail = [];
        showToast();
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoachNhapTraDoiCo/LPXK_GetListPhieuXuatKhoChiTiet',
            data: { phieuXuatKhoId: item.ID_PHIEU, isXK: isXuatKho , keyword: $scope.LPXK_TenHangText },
            success: function (data) {
                hideLoading();
                $('#LPXK_ChiTietPXK').modal('show');
                if (data.data != null && data.data.length > 0) {
                    $scope.LPXK_ListPXK_Detail = data.data;
                }
            }
        });
    }

    $scope.LPXK_ListPhieuXuatKho = [];
    $scope.LPXK_ListPXK_Detail = [];
    $scope.LPXK_ListGKHSX = [];
    $scope.LPXK_ListSPD = [];
    $scope.LPXK_Message = 'Xuất từ kho 2 (trong trường hợp kho ưu tiên hết hàng)';

    /// Xuất kho ưu tiên
    $scope.LPXK_XuatKhoUuTien = function () {
        // kiểm tra xem đã chọn kế hoạch chưa
        if ($scope.thongTinKh.ID_KH > 0) {
            showToast();
            $.ajax({
                type: 'post',
                async: false,
                url: '/KeHoachNhapTraDoiCo/LPXK_XuatKhoUuTien',
                data: { idKeHoach: $scope.thongTinKh.ID_KH, donViId: $scope.DonViId },
                success: function (data) {
                    hideLoading();
                    if (data.obj.Error == false) {
                        toastr.success(data.obj.Title);
                        LayDanhSachPhieuXuatKho();
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

    // Lấy danh sách phiếu xuất kho
    function LayDanhSachPhieuXuatKho() {
        $scope.dsPhieuNhapKho = [];
        $scope.dsPhieuXuatKho = [];
        showToast();
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoachNhapTraDoiCo/LPXK_GetListPhieuXuatKho',
            data: { idKeHoach: $scope.thongTinKh.ID_KH },
            success: function (data) {
                hideLoading();
                if (data.PhieuXuats != null && data.PhieuXuats.length > 0) {
                    $scope.dsPhieuXuatKho = data.PhieuXuats;
                }
                if (data.PhieuNhaps != null && data.PhieuNhaps.length > 0) {
                    $scope.dsPhieuNhapKho = data.PhieuNhaps;
                }
            }
        });
        LaySanPhamNoKeHoach();
    }
    $scope.dsHangHoaNoKH = [];
    // Lấy danh sách hàng hóa nợ kế hoạch
    function LaySanPhamNoKeHoach() {
        $scope.dsHangHoaNoKH = [];
        showToast();
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoachNhapTraDoiCo/LaySanPhamNoKeHoach',
            data: { idKeHoach: $scope.thongTinKh.ID_KH },
            success: function (data) {
                hideLoading();
                if (data.HangHoas != null && data.HangHoas.length > 0) {
                    $scope.dsHangHoaNoKH = data.HangHoas;
                }
            }
        });

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
            url: '/KeHoachNhapTraDoiCo/LPXK_GetListKhoKhac',
            data: { keyword: $scope.LPXK_SearchKhoText, donViId: $scope.DonViId },
            success: function (data) {
                hideLoading();
                if (data.data != null && data.data.length > 0) {
                    $scope.LPXK_ListKho = data.data;
                }
            }
        });
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
            url: '/KeHoachNhapTraDoiCo/LPXK_LayDSHopDong',
            data: { donViId: $scope.DonViId, keHoachId: $scope.thongTinKh.ID_KH, keyword: $scope.LPXK_SearchKhoText, isXuatThangTuHD: isXuatThangTuHD },
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
    //////////////////////////////// Lập phiếu xuất, nhập kho /////////////////////////////////

});

