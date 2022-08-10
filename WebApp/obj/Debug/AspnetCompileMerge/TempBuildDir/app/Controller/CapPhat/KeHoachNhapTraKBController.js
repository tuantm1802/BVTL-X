app.controller("KeHoachNhapTraKBController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
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
        url: '/KeHoachNhapTraKB/GetTreeData',
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
                url: '/KeHoachNhapTraKB/GetDanhMuc',
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
            url: '/KeHoachNhapTraKB/GetTreeData',
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
            url: '/KeHoachNhapTraKB/GetTreeData',
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
                url: '/KeHoachNhapTraKB/TimKiemKeHoach',
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

    // Thêm hàng hóa
    $scope.ThemHangHoa = () => {
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
                url: '/KeHoachNhapTraKB/DanhSachHangHoa',
                data: {
                    keyword: $scope.AddHH_SearchSanPhamText,
                    donViId: $scope.DonViId,
                    nam: $scope.cbbNam
                },
                success: function (response) {
                    if (response.Status === 200) {
                        $scope.dsHangHoaChon = response.Data;

                        var dsHH = [];

                        // Bỏ các hàng hóa đã chọn
                        if ($scope.dsHangHoaDaChon != null && $scope.dsHangHoaDaChon.length > 0) {
                            for (var i = 0; i < $scope.dsHangHoaChon.length; i++) {
                                var hhs = $scope.dsHangHoaDaChon.filter(x => x.ID_SP == $scope.dsHangHoaChon[i].ID_SP);
                                if (hhs == null || hhs.length == 0) {
                                    dsHH.push($scope.dsHangHoaChon[i]);
                                }
                            }
                        }

                        if (dsHH != null && dsHH.length > 0) {
                            $scope.dsHangHoaChon = dsHH;
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

    // Kiểm tra xem số lượng trả có lớn hơn số lượng kế hoạch không
    $scope.ChangeSLTra = (slkh, slTra, index) => {
        if (slkh < slTra) {
            toastr.error("Vui lòng nhập SL trả không lớn hơn SL kế hoạch!");
            $scope.dsHangHoaChon[index].SO_LUONG_CON_LAI = $scope.dsHangHoaChon[index].SL_TONG;
        }
    }

    $scope.ChonTatCaHangHoa = (type) => {
        if (type === 0) {
            $scope.dsHangHoaChon.forEach(x => x.SELECT = !$scope.checkAllLeft);
        }
        if (type === 1) {
            $scope.dsHangHoaDaChon.forEach(x => x.SELECT = !$scope.checkAllRight);
        }
        if (type === 2) {
            $scope.dsHangHoaDaChon.forEach(x => x.SELECT = !$scope.checkAllHangHoa);
        }
    }

    $scope.TichChonHangHoa = (type, index) => {
        if (type === 0) {
            $scope.dsHangHoaChon[index].SELECT = !$scope.dsHangHoaChon[index].SELECT;
            if ($scope.dsHangHoaChon.length > 0) {
                if ($scope.dsHangHoaChon.every(x => x.SELECT === true)) {
                    $scope.checkAllLeft = true;
                } else {
                    $scope.checkAllLeft = false;
                }
            } else {
                $scope.checkAllLeft = false;
            }
        }
        if (type === 1) {
            $scope.dsHangHoaDaChon[index].SELECT = !$scope.dsHangHoaDaChon[index].SELECT;
            if ($scope.dsHangHoaDaChon.length > 0) {
                if ($scope.dsHangHoaDaChon.every(x => x.SELECT === true)) {
                    $scope.checkAllRight = true;
                } else {
                    $scope.checkAllRight = false;
                }
            }
            else {
                $scope.checkAllRight = false;
            }
        }
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

    // Chọn hàng hóa
    $scope.ChonHangHoa = () => {
        // Kiểm tra xem hàng hóa đã được nhập số lượng chưa
        var ds = $scope.dsHangHoaChon.filter(x => x.SELECT == true && (x.SO_LUONG_CON_LAI == null || x.SO_LUONG_CON_LAI == 0));
        if (ds != null && ds.length > 0) {
            toastr.error('Số lượng trả không được để trống!');
        } else {
            //$scope.dsHangHoaChon.forEach(x => x.HANG_HOA = $scope.hanghoa.find(y => y.MaHang === x.MaHang));
            $scope.dsHangHoaDaChon = $scope.dsHangHoaDaChon.concat($scope.dsHangHoaChon.filter(x => x.SELECT === true));
            $scope.dsHangHoaChon = $scope.dsHangHoaChon.filter(x => x.SELECT !== true);
        }
    }

    // Bỏ chọn hàng hóa
    $scope.HuyChonHangHoa = () => {
        $scope.dsHangHoaChon = $scope.dsHangHoaChon.concat($scope.dsHangHoaDaChon.filter(x => x.SELECT === true));
        $scope.dsHangHoaDaChon = $scope.dsHangHoaDaChon.filter(x => x.SELECT !== true);
    }

    // Lưu chọn hàng hóa
    $scope.LuuDsHangHoa = () => {
        var a = $scope.dsHangHoaDaChon.filter(x => x.SO_LUONG_CON_LAI == null);
        if (a.length > 0) {
            toastr.error('Số lượng trả không được để trống!');
        } else {
            $scope.dsHangHoaDaChon.forEach((x, index) => { x.STT = index + 1; x.SELECT = false; });
            $scope.selectedGood = 0;
            $('#them_hang_hoa').modal('hide');
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
                        url: '/KeHoachNhapTraKB/UpdateKeHoach',
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
                        url: '/KeHoachNhapTraKB/TaoKeHoach',
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
            url: '/KeHoachNhapTraKB/LayKeHoachTheoId',
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
            url: '/KeHoachNhapTraKB/DanhSachHangHoaTheoKeHoach',
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
                url: '/KeHoachNhapTraKB/TrinhDuyetKeHoach',
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
            url: '/KeHoachNhapTraKB/XoaKeHoach',
            data: {
                idKh: $scope.thongTinKh.ID_KH
            },
            success: function (response) {
                hideLoading();
                if (response.Status === false) {
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



});

