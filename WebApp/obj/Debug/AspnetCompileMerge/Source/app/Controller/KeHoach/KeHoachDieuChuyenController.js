app.controller("KeHoachDieuChuyenController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.NguoiDuyet = 0;
    $scope.disabledNguoiLapKH = true;
    $scope.disabledSendApprove = true;
    $scope.disabledSave = true;
    $scope.disabledRollback = true;
    $scope.disabledEdit = true;
    $scope.disabledDuyet = true;
    $scope.disabledTuChoiDuyet = true;
    $scope.disabledDelete = true;
    $scope.disabledPrint = true;
    $scope.disabledChiTietHH = true;
    $scope.disabledThongTinKH = true;
    $scope.SearchHH_TuNgay = moment(new Date()).format('DD/MM/YYYY');
    $scope.SearchHH_DenNgay = moment(new Date()).format('DD/MM/YYYY');
    $scope.showColumnDetail = false;

    $scope.danhmuchopdong = [];
    $scope.dmDonVis = [];
    $scope.nguonHangs = [];
    $scope.chatLuongs = [];
    $scope.hinhThucVanChuyens = [];
    $scope.nguonKinhPhis = [];
    $scope.nhomHangs = [];
    $scope.loaiHangs = [];
    $scope.congTys = [];

    $scope.thongTinKh = {};

    $scope.CurrentUserName = "";
    $scope.dinhkem = null;
    $scope.dsKetQua = [];
    $scope.thongTinKho = {};
    $scope.pageSize = 10;
    $scope.currentPage = 1;
    $scope.SoLuongKetQua = 0;
    $scope.SoLuongHienThi = 0;
    $scope.CoKetQua = false;
    DefaultSettings = () => {
        ResetVariables();
        $scope.dsKetQua = [];
        DisablePanel(true);
        $scope.HsStatus = true;

        $scope.disabledNguoiLapKH = true;
        $scope.disabledSendApprove = true;
        $scope.disabledSave = true;
        $scope.disabledRollback = true;
        $scope.disabledEdit = true;
        $scope.disabledDuyet = true;
        $scope.disabledTuChoiDuyet = true;
        $scope.disabledDelete = true;
        $scope.disabledPrint = true;
        $scope.disabledChiTietHH = true;
        $scope.disabledThongTinKH = true;

    }
    DisablePanel = (type) => {
        if (type) {
            $('#main-info *').prop('disabled', true);
            $('#sub-action *').prop('disabled', true);
            $('#detail-info *').prop('disabled', true);
            $scope.EditStatus = true;
        }
        else {
            $('#main-info *').prop('disabled', false);
            $('#sub-action *').prop('disabled', false);
            $('#detail-info *').prop('disabled', false);
            $scope.EditStatus = false;
        }
        $('input[name="NGUOI_LAP"]').prop('disabled', true);
        $('input[name="soPhieuTaiChinh"]').prop('disabled', true);
        $('input[name="ngayNhanThucTe"]').prop('disabled', true);
        $('textarea[name="LY_DO_TC_DUYET"]').prop('disabled', true);
    }

    ResetVariables = () => {
        $scope.dsHangHoaChon = $scope.hanghoa;
        $scope.dsHangHoaDaChon = [];
        $scope.dsChitietTheoHangHoa = [];
        $scope.dsChitietToanBo = [];
        $scope.thongTinKh = {};
        $scope.dsmodel = [];
        $scope.checkAllLeft = false;
        $scope.checkAllRight = false;
        $scope.checkAllHangHoa = false;
    }

    $scope.NgayCapNhat = "";


    $scope.pageSize = 10;
    $scope.currentPage = 1;
    $scope.SoLuongKetQua = 0;
    var date = new Date();
    date.setDate(date.getDate() - 7);
    $scope.hangDoi = {
        'TuNgay': moment(date).format('DD/MM/YYYY'), 'DenNgay': moment(new Date()).format('DD/MM/YYYY'), NoiXuat: 0, NoiNhap: 0
    }
    $scope.NgayCapNhat = moment(new Date()).format('DD/MM/YYYY');

    var thongtinChungValidate = $('#main-info').validate({
        rules: {
            KhoXuat: {
                required: true
            },
            KhoNhap: {
                required: true
            },
            canCu: {
                required: true
            }
            ,
            ngayLapKH: {
                required: true
            },

            nguoiNhan: {
                required: true
            },

            nguonHang: {
                required: true
            },

            ngayNhapDK: {
                required: true
            },

            ngayXuatDKTu: {
                required: true
            },

            ngayXuatDKDen: {
                required: true
            },

            vanChuyen: {
                required: true
            }

        },
        messages: {
            //KhoXuat: {
            //    required: 'Chọn Nơi xuất',
            //},
            //KhoNhap: {
            //    required: 'Chọn Nơi xuất',
            //},
            //canCu: {
            //    required: 'Nhập Căn cứ',
            //},
            //ngayLapKH: {
            //    required: 'Nhập Ngày lập KH',
            //},
            //nguoiNhan: {
            //    required: 'Nhập Người nhận',
            //},
            //nguonHang: {
            //    required: 'Chọn Nguồn hàng',
            //},
            //ngayNhapDK: {
            //    required: 'Nhập Ngày nhập DK',
            //},
            //ngayXuatDKTu: {
            //    required: 'Nhập Ngày xuất DK từ',
            //},
            //ngayXuatDKDen: {
            //    required: 'Nhập Ngày xuất DK đến',
            //},
            //vanChuyen: {
            //    required: 'Chọn Vận chuyển',
            //}

        }

    });

    $scope.ThemKeHoach = (user, appcode) => {

        ResetVariables();
        $scope.thongTinKh.APP_CODE = appcode;
        DisablePanel(false);
        $scope.HsStatus = false;

        $scope.disabledNguoiLapKH = true;
        $scope.disabledSendApprove = true;
        $scope.disabledRollback = true;
        $scope.disabledEdit = true;
        $scope.disabledDuyet = true;
        $scope.disabledTuChoiDuyet = true;
        $scope.disabledDelete = true;
        $scope.disabledPrint = true;
        $scope.disabledChiTietHH = true;

        $scope.disabledThongTinKH = false;
        $scope.disabledSave = false;

        $scope.thongTinKh = {
            'NGAY_KH': moment(new Date()).format('DD/MM/YYYY'),
            'NGAY_XUAT_DK_TU': moment(new Date()).format('DD/MM/YYYY'),
            'NGAY_XUAT_DK_DEN': moment(new Date()).format('DD/MM/YYYY'),
            'NGAY_NHAN_DK': moment(new Date()).format('DD/MM/YYYY')
        }

        $scope.thongTinKh.NGUOI_LAP = $scope.CurrentUserName;

        if ($scope.dmDonVis != null && $scope.dmDonVis.length > 0) {
            $scope.thongTinKh.ID_KHO_XUAT = $scope.dmDonVis[0].ID;
        }

        if ($scope.dmDonVis != null && $scope.dmDonVis.length > 0) {
            $scope.thongTinKh.ID_KHO_NHAP = $scope.dmDonVis[0].ID;
        }

        if ($scope.nguonHangs != null && $scope.nguonHangs.length > 0) {
            $scope.thongTinKh.ID_NGUON_GOC = $scope.nguonHangs[0].FLEX_VALUE_ID;
        }

        if ($scope.hinhThucVanChuyens != null && $scope.hinhThucVanChuyens.length > 0) {
            $scope.thongTinKh.HT_VAN_CHUYEN = $scope.hinhThucVanChuyens[0].FLEX_VALUE_ID;
        }

        if ($scope.NguoiKys != null && $scope.NguoiKys.length > 0) {
            $scope.thongTinKh.NGUOI_DUYET = $scope.NguoiKys[0].USER_ID;
        }

        thongtinChungValidate.resetForm();
    }

    $scope.SuaKeHoach = () => {
        if ($scope.thongTinKh.TRANG_THAI === 'I' || $scope.thongTinKh.TRANG_THAI === 'R') {
            DisablePanel(false);
            $scope.HsStatus = false;
            $scope.disabledThongTinKH = false;
            $scope.disabledSave = false;
            $scope.disabledRollback = false;
            thongtinChungValidate.resetForm();
        }
    }

    $scope.HuyThayDoi = () => {
        LayThongTinKeHoachTheoId();
    }

    LayThongTinKeHoachTheoId = () => {
        showToast();
        $.ajax({
            type: 'post',
            url: '/KeHoachDieuChuyen/LayKeHoachTheoId',
            data: {
                id: $scope.thongTinKh.ID_KE_HOACH
            },
            success: function (response) {
                hideLoading();
                if (response.Status === 200) {
                    for (var i = 0; i < $scope.dsKetQua.length; i++) {
                        if ($scope.dsKetQua[i].ID_KE_HOACH == $scope.thongTinKh.ID_KE_HOACH) {
                            $scope.dsKetQua[i].TRANG_THAI = response.Data.TRANG_THAI;
                            $scope.dsKetQua[i].TRANG_THAI_TEXT = response.Data.TRANG_THAI_TEXT;
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

    $scope.RoleBtnSearch = false;
    $scope.RoleBtnCreate = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnRollback = false;
    $scope.RoleBtnUpdate = false;
    $scope.RoleBtnSave = false;
    $scope.RoleBtnApprove = false;
    $scope.RoleBtnNotApprove = false;
    $scope.RoleBtnSendApprove = false;
    $scope.RoleBtnPrint = false;

    $scope.CurrentUserKyDuyet = "N";
    $scope.NguoiKys = [];
    GetDanhMuc = () => {
        $scope.dmDonVis = [];
        $scope.nguonHangs = [];
        $scope.chatLuongs = [];
        $scope.hinhThucVanChuyens = [];
        $scope.nguonKinhPhis = [];
        $scope.nhomHangs = [];
        $scope.loaiHangs = [];
        $scope.congTys = [];
        $scope.NguoiKys = [];
        $scope.danhmuchopdong = [];
        $scope.CurrentUserName = "";
        $scope.CurrentUserKyDuyet = "N";
        showToast();
        try {
            $.ajax({
                type: 'post',
                url: '/KeHoachDieuChuyen/GetDanhMuc',
                data: {},
                success: function (response) {
                    if (response.Status === 200) {
                        $scope.dmDonVis = response.Datas.dmDonVis;

                        $scope.nguonHangs = response.Datas.nguonHangs;
                        //$scope.chatLuongs = response.Datas.chatLuongs;
                        $scope.hinhThucVanChuyens = response.Datas.hinhThucVanChuyens;
                        //$scope.nguonKinhPhis = response.Datas.nguonKinhPhis;
                        //$scope.nhomHangs = response.Datas.nhomHangs;
                        //$scope.loaiHangs = response.Datas.loaiHangs;
                        //$scope.congTys = response.Datas.congTys;
                        //$scope.danhmuchopdong = response.dsHopDong;
                        $scope.NguoiKys = response.nguoiKyDuyets;
                        $scope.thongTinKh.NGUOI_LAP = response.userName;
                        $scope.CurrentUserName = response.userName;
                        $scope.CurrentUserKyDuyet = response.IsKyDuyet;
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
                            if (item == 'btnApprove') {
                                $scope.RoleBtnApprove = true;
                            }
                            if (item == 'btnNotApprove') {
                                $scope.RoleBtnNotApprove = true;
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
                    $scope.hangDoi.NoiXuat = 0;
                    $scope.hangDoi.NoiNhap = 0;
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
    DefaultSettings();

    $scope.HangHoa_SLTon = 0;

    $scope.ShowChiTietHangHoa = (index) => {
        $scope.HangHoa_SLTon = 0;
        //DisablePanel(true);
        $scope.selectedGood = index;
        $scope.tenHangHoaDaChon = Object.assign('', $scope.dsHangHoaDaChon[index].TEN_HANG_HOA);
        var idHang = $scope.dsHangHoaDaChon[index].ID;
        const hangHoaCu = $scope.dsChitietToanBo.some(x => x.ID_HANG === idHang);

        if (hangHoaCu) {
            $scope.dsChitietTheoHangHoa = $scope.dsChitietToanBo.filter(x => x.ID_HANG === idHang);
            $scope.tenHangHoaDaChon = $scope.dsChitietTheoHangHoa[0].TEN_HANG;
            $scope.soLuongDaChon = $scope.dsChitietTheoHangHoa.sum('SO_LUONG');
            $scope.thanhTienDaChon = $scope.dsChitietTheoHangHoa.sum('THANH_TIEN');
            //$scope.dsChitietTheoHangHoa.forEach((x, index) => x.STT = index + 1);
        } else {
            $scope.dsChitietTheoHangHoa = [];
            $scope.tenHangHoaDaChon = $scope.dsHangHoaDaChon[$scope.selectedGood].TEN_HANG_HOA;
            $scope.soLuongDaChon = $scope.dsHangHoaDaChon[$scope.selectedGood].SO_LUONG;
            $scope.soLuongDaChon = null;
        }
        $scope.GetTonKhoByHangHoa(idHang);
    }

    $scope.GetTonKhoByHangHoa = (hangHoaId) => {
        if (hangHoaId > 0) {
            $.ajax({
                type: 'post',
                url: '/KeHoachDieuChuyen/GetTonKhoByHangHoa',
                data: {
                    khoId: $scope.thongTinKh.ID_KHO_XUAT,
                    hangHoaId: hangHoaId
                },
                success: function (response) {
                    if (response.Status === 200) {
                        $scope.HangHoa_SLTon = response.Data - $scope.dsChitietTheoHangHoa.length;
                    }
                    else {
                        toastr.error('Có lỗi xảy ra trong quá trình lấy tồn kho của hàng hóa!');
                    }

                    $scope.$apply();
                }, error: (e) => {
                    toastr.error('Có lỗi xảy ra trong quá trình lấy tồn kho của hàng hóa!');
                },
                complete: () => {
                }
            });
        }
    }

    $scope.XemChiTietHangHoa = () => {
        $scope.ChiTietHangHoa = [];
        $('#chi_tiet_hang').modal('show');
    }

    $scope.ngayLapKh = moment(new Date()).format('DD/MM/YYYY');
    $scope.TimKiem1 = () => {
        $scope.currentPage = 1;
        LoadPage();

    }

    $scope.pageChanged = function () {
        LoadPage();
        
    }

    LoadPage = () => {
        $scope.dsKetQua = [];
        $scope.CoKetQua = false;
        $scope.SoLuongKetQua = 0;
        $scope.SoLuongHienThi = 0;
        showToast();
        DefaultSettings();
        $.ajax({
            type: 'POST',
            dataType: 'json',
            cache: false,
            async: false,
            url: '/KeHoachDieuChuyen/TimKiemKeHoach',
            data: {
                soKeHoach: $scope.hangDoi.SoKeHoach,
                tuNgay: moment($scope.hangDoi.TuNgay, 'DD/MM/YYYY').format('YYYYMMDD'),
                denNgay: moment($scope.hangDoi.DenNgay, 'DD/MM/YYYY').format('YYYYMMDD'),
                noiXuat: $scope.hangDoi.NoiXuat,
                noiNhap: $scope.hangDoi.NoiNhap,
                trangThai: $scope.hangDoi.TrangThai,
                pageSize: $scope.pageSize,
                pageNumber: $scope.currentPage
            },
            success: function (response) {
                hideLoading();
                if (response.Status === 200) {
                    if (response.Data != null && response.Data.length > 0) {
                        $scope.dsKetQua = response.Data;

                        $scope.CoKetQua = true;
                        $scope.SoLuongKetQua = $scope.dsKetQua[0].total;
                        $scope.SoLuongHienThi = $scope.dsKetQua.length;
                        $scope.selectedRow = 0;
                        $scope.ShowThongTin($scope.dsKetQua[0], 0);
                    }
                }

                //$scope.$apply();
            }, error: (e) => {
                hideLoading();
                toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm!');
            },
            complete: () => {
            }
        });
    }

    ThongTinHangHoa = (id) => {
        $scope.dsHangHoa = [];
        $scope.dsHangHoaDaChon = [];
        $.ajax({
            type: 'POST',
            dataType: 'json',
            cache: false,
            async: false,
            url: '/KeHoachDieuChuyen/DanhSachHangHoaTheoKeHoach',
            data: {
                idKeHoach: id
            },
            success: function (response) {
                if (response.Status === 200) {
                    $scope.dsHangHoaDaChon = response.Data;
                }
                else {
                    toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm!');
                }

               // $scope.$apply();
            }, error: (e) => {
                toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm!');
            }
        });
    }

    $scope.ShowThongTin = (item, index) => {
        showToast();
        try {
            ResetVariables();

            $scope.disabledNguoiLapKH = true;
            $scope.disabledSendApprove = true;
            $scope.disabledSave = true;
            $scope.disabledRollback = true;
            $scope.disabledEdit = true;
            $scope.disabledDuyet = true;
            $scope.disabledTuChoiDuyet = true;
            $scope.disabledDelete = true;
            $scope.disabledPrint = true;
            $scope.disabledChiTietHH = true;
            $scope.disabledThongTinKH = true;

            $scope.selectedRow = index;
            $scope.thongTinKh = item;
            if ($scope.thongTinKh.NGAY_XUAT_DK_TU.length === 8) {
                $scope.thongTinKh.NGAY_XUAT_DK_TU = moment($scope.thongTinKh.NGAY_XUAT_DK_TU, 'YYYYMMDD').format('DD/MM/YYYY')
            }
            if ($scope.thongTinKh.NGAY_XUAT_DK_DEN.length === 8) {
                $scope.thongTinKh.NGAY_XUAT_DK_DEN = moment($scope.thongTinKh.NGAY_XUAT_DK_DEN, 'YYYYMMDD').format('DD/MM/YYYY')
            }
            if ($scope.thongTinKh.NGAY_KH.length === 8) {
                $scope.thongTinKh.NGAY_KH = moment($scope.thongTinKh.NGAY_KH, 'YYYYMMDD').format('DD/MM/YYYY')
            }
            if ($scope.thongTinKh.NGAY_NHAN_DK.length === 8) {
                $scope.thongTinKh.NGAY_NHAN_DK = moment($scope.thongTinKh.NGAY_NHAN_DK, 'YYYYMMDD').format('DD/MM/YYYY')
            }

            if ($scope.thongTinKh.NGAY_NHAN_TT != null && $scope.thongTinKh.NGAY_NHAN_TT != '' && $scope.thongTinKh.NGAY_NHAN_TT.length === 8) {
                $scope.thongTinKh.NGAY_NHAN_TT = moment($scope.thongTinKh.NGAY_NHAN_TT, 'YYYYMMDD').format('DD/MM/YYYY')
            }


            $scope.disabledPrint = false;

            if ($scope.thongTinKh.TRANG_THAI === 'I' || $scope.thongTinKh.TRANG_THAI === 'R') {
                $scope.disabledEdit = false;
                $scope.disabledDelete = false;
                $scope.disabledSendApprove = false;
            }


            ThongTinHangHoa(item.ID_KE_HOACH);
        } catch (e) {
            toastr.error("Có lỗi xảy ra khi lấy thông tin " + e);
            console.error(e);
        }
        finally {
            hideLoading();
        }
    }

    $scope.IsClickDuyetKeHoach = false;
    $scope.IsClickXoaKeHoach = false;
    $scope.DuyetPhieu = () => {
        $scope.TxtPassword = "";
        $('#EnterPassword *').prop('disabled', false);
        try {
            if ($scope.thongTinKh.TRANG_THAI === 'A') {
                toastr.error('Kế hoạch đã duyệt, không thể duyệt!');
            }
            else {
                if ($scope.dsHangHoaDaChon == null || $scope.dsHangHoaDaChon.length == 0) {
                    toastr.error('Bạn chưa thêm hàng hóa!');
                } else {
                    $scope.IsClickDuyetKeHoach = true;
                    $scope.IsClickXoaKeHoach = false;
                    $('#EnterPassword').modal('show');
                }
            }
        }
        catch (e) {
            console.log(e);
            toastr.error('Có lỗi xảy ra trong quá trình duyệt kế hoạch!');
        }
        finally {
            hideLoading();
        }
    }
    $scope.TxtLyDoKhongDuyet = "";
    $scope.TuChoiDuyetPhieu = () => {

        $('#EnterLyDoKhongDuyet *').prop('disabled', false);
        if ($scope.thongTinKh.TRANG_THAI === 'A') {
            toastr.error('Kế hoạch đã duyệt, không thể từ chối duyệt!');
        }
        else {
            $scope.TxtLyDoKhongDuyet = "";
            $('#EnterLyDoKhongDuyet').modal('show');
        }
    }



    // Đồng ý từ chối duyệt phiếu
    $scope.XacNhanTuChoiDuyet = function () {
        if ($scope.TxtLyDoKhongDuyet == null || $scope.TxtLyDoKhongDuyet == '') {
            toastr.error("Vui lòng nhập lý do từ chối duyệt!");
        }
        else {
            // Kiểm tra password
            $.ajax({
                type: 'post',
                async: false,
                url: '/KeHoachDieuChuyen/TuChoiDuyetKeHoach',
                data: {
                    idKh: $scope.thongTinKh.ID_KE_HOACH,
                    lyDoTuChoiDuyet: $scope.TxtLyDoKhongDuyet
                },
                success: function (res) {
                    if (res.Status == false) {
                        $('#EnterLyDoKhongDuyet').modal('hide');
                        $scope.TxtLyDoKhongDuyet = '';
                        toastr.success(res.Message);
                        LayThongTinKeHoachTheoId();
                        //DefaultSettings();
                        //$scope.TimKiem();
                    } else {
                        toastr.error(res.Message);
                    }
                }
            });
        }

    };

    DongYDuyetKehoach = () => {
        $scope.IsClickDuyetKeHoach = false;
        $scope.IsClickXoaKeHoach = false;
        if ($scope.thongTinKh.ID_KE_HOACH > 0) {
            showToast();
            $.ajax({
                type: 'post',
                url: '/KeHoachDieuChuyen/DuyetKeHoach',
                data: {
                    idKh: $scope.thongTinKh.ID_KE_HOACH,
                },
                success: function (response) {
                    hideLoading();
                    if (response.Status === false) {
                        toastr.success(response.Message);
                        LayThongTinKeHoachTheoId();
                       // DefaultSettings();
                        //$scope.TimKiem();
                    }
                    else {
                        toastr.error('Có lỗi xảy ra trong quá trình duyệt kế hoạch!');
                    }
                    console.log(response);
                    $scope.$apply();

                }, error: (e) => {
                    hideLoading();
                    toastr.error('Có lỗi xảy ra trong quá trình duyệt kế hoạch!');
                }
            });
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


    $scope.TrinhDuyet = () => {
        $('#ChonNguoiDuyet *').prop('disabled', false);
        if ($scope.dsHangHoaDaChon == null || $scope.dsHangHoaDaChon.length == 0) {
            toastr.error('Cần thêm hàng hóa để trình duyệt!');
        } else {
            if ($scope.thongTinKh.TRANG_THAI === 'I' || $scope.thongTinKh.TRANG_THAI === 'R') {
                $('#ChonNguoiDuyet').modal('show');
            }
        }
    }

    $scope.XacNhanTrinhDuyet = () => {
        if ($scope.NguoiDuyet > 0) {
            showToast();
            $.ajax({
                type: 'post',
                url: '/KeHoachDieuChuyen/TrinhDuyetKeHoach',
                data: {
                    idKh: $scope.thongTinKh.ID_KE_HOACH,
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

    DongYXoaKehoach = () => {
        $scope.IsClickDuyetKeHoach = false;
        $scope.IsClickXoaKeHoach = false;
        showToast();
        $.ajax({
            type: 'post',
            url: '/KeHoachDieuChuyen/XoaKeHoach',
            data: {
                idKh: $scope.thongTinKh.ID_KE_HOACH,
            },
            success: function (response) {
                hideLoading();
                if (response.Status === false) {
                    $('#EnterPassword').modal('hide');
                    toastr.success(response.Message);
                    $scope.TimKiem();
                }
                else {
                    toastr.error('Có lỗi xảy ra trong quá trình xóa!');
                }
                console.log(response);
                $scope.$apply();
            }, error: (e) => {
                hideLoading();
                toastr.error('Có lỗi xảy ra trong quá trình xóa!');
            }
        });
    }

    $scope.XoaKeHoach = () => {
        showToast();
        try {
            if ($scope.thongTinKh.TRANG_THAI == 'A') {
                $scope.TxtPassword = "";
                $('#EnterPassword *').prop('disabled', false);
                $scope.IsClickDuyetKeHoach = false;
                $scope.IsClickXoaKeHoach = true;
                $('#EnterPassword').modal('show');
                //toastr.error('Kế hoạch đã duyệt, không thể xóa!');
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

    ThongTinChungValidate = () => {
        $scope.CheckThongTinChungMsg = 'Vui lòng nhập các thông tin sau:';
        $scope.CheckThongTinChung = false;
        $scope.ListMsg = [];
        if ($scope.thongTinKh.CAN_CU == null || $scope.thongTinKh.CAN_CU == '' || $scope.thongTinKh.CAN_CU == undefined) {
            $scope.CheckThongTinChung = true;
            $scope.ListMsg.push("Căn cứ");
            $scope.thongTinKh.CAN_CU == '';
        }

        if ($scope.thongTinKh.NGUOI_NHAN == null || $scope.thongTinKh.NGUOI_NHAN == '' || $scope.thongTinKh.NGUOI_NHAN == undefined) {
            $scope.CheckThongTinChung = true;
            $scope.ListMsg.push("Người nhận");
            $scope.thongTinKh.NGUOI_NHAN == '';
        }

        if ($scope.thongTinKh.LY_DO == undefined) {
            $scope.thongTinKh.LY_DO == '';
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


    $scope.CheckThongTinChung = false;
    $scope.CheckThongTinChungMsg = '';
    $scope.LuuPhieu = () => {
        showToast();
        try {
            if ($('#main-info').valid()) {

                $scope.CheckThongTinChungMsg = '';
                $scope.CheckThongTinChung = false;
                ThongTinChungValidate();
                if ($scope.CheckThongTinChung == false) {
                    if ($scope.thongTinKh.ID_KHO_XUAT == $scope.thongTinKh.ID_KHO_NHAP) {
                        toastr.error('Nơi xuất và Nơi nhập không được giống nhau!');
                    } else {

                        if ($scope.thongTinKh.NGAY_XUAT_DK_TU > $scope.thongTinKh.NGAY_NHAN_DK || $scope.thongTinKh.NGAY_XUAT_DK_TU > $scope.thongTinKh.NGAY_XUAT_DK_DEN) {
                            toastr.error('Ngày xuất dự kiến từ không được lớn hơn ngày nhập dự kiến hoặc ngày xuất dự kiến đến!');
                        }
                        else {

                            $scope.thongTinKh.NGAY_KH = moment($scope.thongTinKh.NGAY_KH, 'DD/MM/YYYY').format('YYYYMMDD');
                            $scope.thongTinKh.NGAY_NHAN_DK = moment($scope.thongTinKh.NGAY_NHAN_DK, 'DD/MM/YYYY').format('YYYYMMDD');
                            $scope.thongTinKh.NGAY_XUAT_DK_TU = moment($scope.thongTinKh.NGAY_XUAT_DK_TU, 'DD/MM/YYYY').format('YYYYMMDD');
                            $scope.thongTinKh.NGAY_XUAT_DK_DEN = moment($scope.thongTinKh.NGAY_XUAT_DK_DEN, 'DD/MM/YYYY').format('YYYYMMDD');

                            if ($scope.thongTinKh.NGAY_NHAN_TT != null && $scope.thongTinKh.NGAY_NHAN_TT != '') {
                                $scope.thongTinKh.NGAY_NHAN_TT = moment($scope.thongTinKh.NGAY_NHAN_TT, 'DD/MM/YYYY').format('YYYYMMDD');
                            }


                            $scope.thongTinKh.TRANG_THAI = 'I';
                            $scope.thongTinKh.APP_CODE = 'CP_QUAN_TRANG';

                            if ($scope.thongTinKh.ID_KE_HOACH != null) {
                                $.ajax({
                                    type: 'post',
                                    url: '/KeHoachDieuChuyen/UpdateKeHoach',
                                    data: {
                                        thongTinPhieu: $scope.thongTinKh,
                                        dsHangHoaChiTiet: $scope.dsHangHoaDaChon
                                    },
                                    success: function (response) {
                                        if (response.Status == false) {
                                            toastr.success('Cập nhật thông tin điều chuyển kho thành công!');
                                            LayThongTinKeHoachTheoId();
                                        }
                                        else {
                                            toastr.error(response.Message);
                                            if ($scope.thongTinKh.NGAY_XUAT_DK_TU.length === 8) {
                                                $scope.thongTinKh.NGAY_XUAT_DK_TU = moment($scope.thongTinKh.NGAY_XUAT_DK_TU, 'YYYYMMDD').format('DD/MM/YYYY')
                                            }
                                            if ($scope.thongTinKh.NGAY_XUAT_DK_DEN.length === 8) {
                                                $scope.thongTinKh.NGAY_XUAT_DK_DEN = moment($scope.thongTinKh.NGAY_XUAT_DK_DEN, 'YYYYMMDD').format('DD/MM/YYYY')
                                            }
                                            if ($scope.thongTinKh.NGAY_KH.length === 8) {
                                                $scope.thongTinKh.NGAY_KH = moment($scope.thongTinKh.NGAY_KH, 'YYYYMMDD').format('DD/MM/YYYY')
                                            }
                                            if ($scope.thongTinKh.NGAY_NHAN_DK.length === 8) {
                                                $scope.thongTinKh.NGAY_NHAN_DK = moment($scope.thongTinKh.NGAY_NHAN_DK, 'YYYYMMDD').format('DD/MM/YYYY')
                                            }

                                            if ($scope.thongTinKh.NGAY_NHAN_TT != null && $scope.thongTinKh.NGAY_NHAN_TT != '' && $scope.thongTinKh.NGAY_NHAN_TT.length === 8) {
                                                $scope.thongTinKh.NGAY_NHAN_TT = moment($scope.thongTinKh.NGAY_NHAN_TT, 'YYYYMMDD').format('DD/MM/YYYY')
                                            }

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
                                    url: '/KeHoachDieuChuyen/TaoKeHoach',
                                    data: {
                                        thongTinPhieu: $scope.thongTinKh,
                                        dsHangHoaChiTiet: $scope.dsHangHoaDaChon
                                    },
                                    success: function (response) {
                                        if (response.Status == false) {
                                            toastr.success('Lưu thông tin điều chuyển kho thành công!');
                                            //$scope.dsHangHoaDaChon = response.HangHoa;
                                            $scope.thongTinKh.ID_KE_HOACH = response.keHoachId;
                                            $scope.thongTinKh.SO_KH = response.soKH;
                                            $scope.disabledSendApprove = false;
                                            //LayThongTinKeHoachTheoId();
                                            LoadPage();
                                            //$scope.TimKiem();
                                        }
                                        else {
                                            toastr.error('Có lỗi xảy ra trong quá trình lưu!');
                                            if ($scope.thongTinKh.NGAY_XUAT_DK_TU.length === 8) {
                                                $scope.thongTinKh.NGAY_XUAT_DK_TU = moment($scope.thongTinKh.NGAY_XUAT_DK_TU, 'YYYYMMDD').format('DD/MM/YYYY')
                                            }
                                            if ($scope.thongTinKh.NGAY_XUAT_DK_DEN.length === 8) {
                                                $scope.thongTinKh.NGAY_XUAT_DK_DEN = moment($scope.thongTinKh.NGAY_XUAT_DK_DEN, 'YYYYMMDD').format('DD/MM/YYYY')
                                            }
                                            if ($scope.thongTinKh.NGAY_KH.length === 8) {
                                                $scope.thongTinKh.NGAY_KH = moment($scope.thongTinKh.NGAY_KH, 'YYYYMMDD').format('DD/MM/YYYY')
                                            }
                                            if ($scope.thongTinKh.NGAY_NHAN_DK.length === 8) {
                                                $scope.thongTinKh.NGAY_NHAN_DK = moment($scope.thongTinKh.NGAY_NHAN_DK, 'YYYYMMDD').format('DD/MM/YYYY')
                                            }

                                            if ($scope.thongTinKh.NGAY_NHAN_TT != null && $scope.thongTinKh.NGAY_NHAN_TT != '' && $scope.thongTinKh.NGAY_NHAN_TT.length === 8) {
                                                $scope.thongTinKh.NGAY_NHAN_TT = moment($scope.thongTinKh.NGAY_NHAN_TT, 'YYYYMMDD').format('DD/MM/YYYY')
                                            }
                                        }
                                        console.log(response);
                                        $scope.$apply();
                                    }, error: (e) => {
                                        toastr.error('Có lỗi xảy ra trong quá trình lưu!');
                                    }
                                });
                            }
                        }

                    }
                } else {
                    toastr.error($scope.CheckThongTinChungMsg);
                }
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


    CheckThongTin = () => {
        if ($scope.thongTinKho.NGAY_NK_VW == null || $scope.thongTinKho.NGUOI_KY_4 == null || $scope.thongTinKho.NGUOI_KY_5 == null || $scope.thongTinKho.NGUOI_PHU_TRACH == null || $scope.thongTinKho.NGUOI_GIAO == null /*|| $scope.thongTinKho.NGUOI_NHAN == null*/) {
            toastr.error("Chưa nhập đủ thông tin");
            return false;
        }
        //if ($scope.dsHangHoa.some(element => CheckDsHangHoa(element))){
        //    toastr.error("Chưa nhập đủ thông tin hàng hóa");
        //    return false;
        //}
        return true;
    }

    const CheckDsHangHoa = (hanghoa) => {
        if (hanghoa.HANG_HOA == null || hanghoa.DVT == null || hanghoa.SO_LUONG_TT == null) return false;
        else return true;
    }
    $scope.ChonTatCa = () => {
        $scope.danhsachanh.forEach(x => x.checked = !$scope.checkAll);
    }


    $scope.showTableHH = true;
    $scope.ThayDoiTableChiTiet = () => {
        //if ($("#img-toggle").hasClass("img-normal")) {
        //    $("#img-toggle").removeClass("img-normal").addClass("img-180-rotate");
        //}
        //else if ($("#img-toggle").hasClass("img-180-rotate")) {
        //    $("#img-toggle").removeClass("img-180-rotate").addClass("img-normal");
        //}
        //if ($('#right-table').hasClass("col-md-12")) {
        //    $('#left-table').css("display", "block").addClass('col-md-3');
        //    $('#right-table').removeClass('col-md-12').addClass('col-md-9');
        //}
        //else {
        //    $('#left-table').css("display", "none").removeClass('col-md-3');
        //    $('#right-table').removeClass('col-md-9').addClass('col-md-12');
        //}

        if ($scope.showColumnDetail) {
            $scope.showColumnDetail = false;
            $scope.showTableHH = true;
        }
        else {
            $scope.showColumnDetail = true;
            $scope.showTableHH = false;
        }
    }
    GenerateRandomNumber = () => {
        var newDate = new Date();
        var soPhieu = 'KHDCB' + moment(newDate).format('YYYYMMDDHHmm');
        return soPhieu;
    }
    $scope.LocDsTheoHangHoa = (item) => {
        return item.ID_HANG === $scope.dsHangHoaDaChon[$scope.selectedGood].ID_HANG;
    }



    var chitietValidate = $('#chi-tiet-hh-form').validate({
        rules: {
            hopdong: {
                required: true
            },
            nguonKinhPhi: {
                required: true
            },
            nhomhang: {
                required: true
            },
            loaihang: {
                required: true
            },
            soluong: {
                required: true
            },
            chatluong: {
                required: true
            }
        },
        messages: {
            hopdong: {
                required: 'Chọn Hợp đồng',
            },
            nguonKinhPhi: {
                required: 'Chọn Nguồn kinh phí',
            },
            nhomhang: {
                required: 'Chọn Nhóm hàng',
            },
            loaihang: {
                required: 'Chọn loại hàng',
            },
            soluong: {
                required: 'Nhập Số lượng',
            },
            chatluong: {
                required: 'Chọn Chất lượng',
            }
        }
    });
    $scope.XoaHangHoaDaChon = () => {
        $scope.dsHangHoaChon = $scope.dsHangHoaChon.concat($scope.dsHangHoaDaChon.filter(x => x.oSELECTED === true));

        $scope.dsHangHoaDaChon.forEach(x => {
            if (x.oSELECTED) {
                $scope.dsChitietToanBo = $scope.dsChitietToanBo.filter(y => y.ID_HANG !== x.ID_HANG)
            }

        });
        var hangHoa = $scope.dsHangHoaDaChon.filter(x => x.oSELECTED !== true);
        if (hangHoa != null && hangHoa.length > 0) {
            $scope.dsHangHoaDaChon = hangHoa;
            $scope.ShowChiTietHangHoa(0);
        }
        else {
            $scope.dsHangHoaDaChon = [];
            $scope.dsChitietTheoHangHoa = [];
        }

    }





    CheckAllHandler = () => {
        if ($scope.dsHangHoaChon.length > 0) {
            if ($scope.dsHangHoaChon.every(x => x.SELECT === true)) {
                $scope.checkAllLeft = true;
            } else {
                $scope.checkAllLeft = false;
            }
        } else {
            $scope.checkAllLeft = false;
        }

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


    ////////////////////////////////////// Thêm hàng hóa ////////////////////////////////////

    $scope.dsHangHoaChon = [];

    // Thêm hàng hóa
    $scope.ThemHangHoa = () => {
        GetDanhSachHangHoa();
        if ($scope.thongTinKh.ID_KHO_XUAT > 0) {
            $('#them_hang_hoa').modal('show');
        }
    }

    // Tìm kiếm hàng hóa
    $scope.AddHH_TimKiem = () => {
        GetDanhSachHangHoa();
    }

    GetDanhSachHangHoa = () => {
        if ($scope.thongTinKh.ID_KHO_XUAT > 0) {
            showToast();
            $.ajax({
                type: 'post',
                url: '/KeHoachDieuChuyen/DanhSachHangHoa',
                data: {
                    keyword: $scope.AddHH_SearchSanPhamText,
                    khoId: $scope.thongTinKh.ID_KHO_XUAT,
                    keHoachId: $scope.thongTinKh.ID_KE_HOACH
                },
                success: function (response) {
                    if (response.Status === 200) {
                        $scope.dsHangHoaChon = response.Data;

                        var dsHH = [];

                        // Bỏ các hàng hóa đã chọn
                        if ($scope.dsHangHoaDaChon != null && $scope.dsHangHoaDaChon.length > 0) {
                            for (var i = 0; i < $scope.dsHangHoaChon.length; i++) {
                                var hhs = $scope.dsHangHoaDaChon.filter(x => x.ID_HANG == $scope.dsHangHoaChon[i].ID_HANG);
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
            toastr.error("Vui lòng chọn kho xuất!");
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

    $scope.ChonHangHoa = () => {
        // Kiểm tra xem hàng hóa đã được nhập số lượng chưa
        var ds = $scope.dsHangHoaChon.filter(x => x.SELECT == true && (x.SO_LUONG == null || x.SO_LUONG == 0));
        if (ds != null && ds.length > 0) {
            toastr.error('Số lượng không được để trống!');
        } else {
            //$scope.dsHangHoaChon.forEach(x => x.HANG_HOA = $scope.hanghoa.find(y => y.MaHang === x.MaHang));
            $scope.dsHangHoaDaChon = $scope.dsHangHoaDaChon.concat($scope.dsHangHoaChon.filter(x => x.SELECT === true));
            $scope.dsHangHoaChon = $scope.dsHangHoaChon.filter(x => x.SELECT !== true);
            CheckAllHandler();
        }
    }
    $scope.HuyChonHangHoa = () => {
        $scope.dsHangHoaChon = $scope.dsHangHoaChon.concat($scope.dsHangHoaDaChon.filter(x => x.SELECT === true));
        $scope.dsHangHoaDaChon = $scope.dsHangHoaDaChon.filter(x => x.SELECT !== true);
        CheckAllHandler();
    }

    $scope.LuuDsHangHoa = () => {
        var a = $scope.dsHangHoaDaChon.filter(x => x.SO_LUONG == null);
        if (a.length > 0) {
            toastr.error('Số lượng không được để trống!');
        } else {
            $scope.dsHangHoaDaChon.forEach((x, index) => { x.STT = index + 1; x.SELECT = false; });
            $scope.selectedGood = 0;
            // $scope.ShowChiTietHangHoa(0);
            $('#them_hang_hoa').modal('hide');
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////

    //////////////////////////////// Thêm, sửa hàng hóa chi tiết ////////////////////////////
    $scope.TaoBanSao = false;

    $scope.SerialStatus = 1;// 1: show cả ô nhập số khung, số máy và số serial; 2: chỉ show ô nhập số khung, số máy; 3: chỉ show ô nhập số serial
    $scope.ThemMoiChiTietHangHoa = () => {
        if ($scope.dsHangHoaDaChon.length > 0) {
            //if ($('#main-info').valid()) {
            $scope.chitietTheoHangHoa = {};
            $scope.chitietTheoHangHoa.ID_NHOM_HH = $scope.dsHangHoaDaChon[$scope.selectedGood].NHOM_HH_ID;
            $scope.chitietTheoHangHoa.ID_LOAI_HH = $scope.dsHangHoaDaChon[$scope.selectedGood].LOAI_HH_ID;

            chitietValidate.resetForm();
            $('#chi-tiet-hh-form').find('input, select').removeClass('error');
            //if ($scope.dsHangHoaDaChon[$scope.selectedGood].LOAI_HANG === 'PHUONG_TIEN') {
            //    $scope.SerialStatus = false;
            //} else {
            //    $scope.SerialStatus = true;
            //}
            $scope.chitietTheoHangHoa.SO_LUONG = 1;
            $('#chi_tiet_hang_hoa_theo_hd').modal('show');
            //}

            // Lấy danh sách hợp đồng
            TimKiemHopDong();
        } else {
            toastr.error("Bạn chưa chọn hàng hóa nào");
        }
    }

    $scope.SuaChiTietHangHoa = () => {
        if ($scope.IndexChiTietHangHoa > -1) {
            //if ($('#main-info').valid()) {
            $scope.chitietTheoHangHoa = $scope.dsChitietTheoHangHoa[$scope.IndexChiTietHangHoa];
            chitietValidate.resetForm();
            $('#chi-tiet-hh-form').find('input, select').removeClass('error');
            $('#chi_tiet_hang_hoa_theo_hd').modal('show');
            // }

            // Lấy danh sách hợp đồng
            TimKiemHopDong();
        } else {
            toastr.error("Vui lòng chọn dòng chi tiết cần sửa!");
        }
    }

    $scope.ChangeSLHangHoa = (soLuong, soLuongTon, index) => {
        if (soLuong > 0) {
            if (soLuong > soLuongTon) {
                toastr.error("Vui lòng nhập Số lượng không được lớn hơn Số lượng tồn!");
                $scope.dsHangHoaChon[index].SO_LUONG = soLuongTon;
            }
        }
        else {
            toastr.error("Vui lòng nhập Số lượng lớn hơn 0!");
            $scope.dsHangHoaChon[index].SO_LUONG = 0;
        }
    }


    $scope.LuuChiTiet = () => {
        // Kiểm tra xem số khung, số máy có trùng nhau không
        if ($scope.chitietTheoHangHoa.MO_TA_CT2 != null && $scope.chitietTheoHangHoa.MO_TA_CT2 != '' && $scope.chitietTheoHangHoa.MO_TA_CT2 == $scope.chitietTheoHangHoa.MO_TA_CT3) {
            toastr.error('Vui lòng nhập Số khung và Số máy khác nhau!');
        } else {
            $scope.chitietTheoHangHoa.ID_HANG = $scope.dsHangHoaDaChon[$scope.selectedGood].ID;
            $scope.chitietTheoHangHoa.TEN_HANG = $scope.dsHangHoaDaChon[$scope.selectedGood].TEN_HANG_HOA;
            $scope.chitietTheoHangHoa.MA_HANG = $scope.dsHangHoaDaChon[$scope.selectedGood].MA_HANG_HOA;
            $scope.chitietTheoHangHoa.DVT = $scope.dsHangHoaDaChon[$scope.selectedGood].TEN_DVT;
            //$scope.chitietTheoHangHoa.ID_NGUON_KP = $scope.thongTinKh.ID_NGUON_KP;
            // $scope.chitietTheoHangHoa.ID_NGUON_GOC = $scope.thongTinKh.ID_NGUON_GOC;

            if ($scope.chitietTheoHangHoa.ID_NGUON_KP != null && $scope.chitietTheoHangHoa.ID_NGUON_KP > 0) {
                $scope.chitietTheoHangHoa.TEN_NGUON_KP = $scope.nguonKinhPhis.find(y => y.FLEX_VALUE_ID === $scope.chitietTheoHangHoa.ID_NGUON_KP).DESCRIPTION;
            }

            if ($scope.chitietTheoHangHoa.ID_NHOM_HH != null && $scope.chitietTheoHangHoa.ID_NHOM_HH > 0) {
                $scope.chitietTheoHangHoa.TEN_NHOM_HH = $scope.nhomHangs.find(y => y.FLEX_VALUE_CATEGORY_ID === $scope.chitietTheoHangHoa.ID_NHOM_HH).DESCRIPTION;
            }

            if ($scope.chitietTheoHangHoa.ID_LOAI_HH != null && $scope.chitietTheoHangHoa.ID_LOAI_HH > 0) {
                $scope.chitietTheoHangHoa.TEN_LOAI_HH = $scope.loaiHangs.find(y => y.FLEX_VALUE_CATEGORY_ID === $scope.chitietTheoHangHoa.ID_LOAI_HH).DESCRIPTION;
            }

            if ($scope.chitietTheoHangHoa.ID_CHAT_LUONG != null && $scope.chitietTheoHangHoa.ID_CHAT_LUONG > 0) {
                $scope.chitietTheoHangHoa.TEN_CHAT_LUONG = $scope.chatLuongs.find(y => y.FLEX_VALUE_ID === $scope.chitietTheoHangHoa.ID_CHAT_LUONG).DESCRIPTION;
            }

            if ($scope.chitietTheoHangHoa.ID_HOP_DONG != null && $scope.chitietTheoHangHoa.ID_HOP_DONG > 0) {
                $scope.chitietTheoHangHoa.SO_HOP_DONG = $scope.danhmuchopdong.find(y => y.ID_HOP_DONG === $scope.chitietTheoHangHoa.ID_HOP_DONG).SO_HD;
            }

            if ($('#chi-tiet-hh-form').valid()) {
                if ($scope.dsmodel.length > 0) {

                    if ($scope.dsmodel.length > $scope.chitietTheoHangHoa.SO_LUONG) {
                        $ngConfirm({
                            title: 'Thông báo',
                            content: 'Tổng số lượng serial khác số lượng đã nhập! Bạn có muốn cập nhật lại số lượng không?',
                            buttons: {
                                change: {
                                    text: 'Cập nhật',
                                    btnClass: 'btn-red',
                                    action: function () {
                                        for (var i = 0; i < $scope.dsmodel.length; i++) {
                                            var obj = Object.assign({}, $scope.chitietTheoHangHoa);
                                            obj.MO_TA_CT4 = $scope.dsmodel[i];
                                            obj.SO_LUONG = 1;
                                            obj.THANH_TIEN = obj.SO_LUONG * obj.DON_GIA;
                                            $scope.dsChitietTheoHangHoa.push(obj);
                                            $scope.dsChitietTheoHangHoa.forEach((x, index) => x.STT = index + 1);
                                            $scope.dsChitietToanBo.push($scope.dsChitietTheoHangHoa[$scope.dsChitietTheoHangHoa.length - 1]);
                                        }
                                        const chuaCoSerial = $scope.chitietTheoHangHoa.SO_LUONG - $scope.dsmodel.length;
                                        if (chuaCoSerial > 0) {
                                            //for (var j = 0; j < chuaCoSerial; j++) {
                                            var objNS = Object.assign({}, $scope.chitietTheoHangHoa);
                                            objNS.SO_LUONG = chuaCoSerial;
                                            obj.THANH_TIEN = obj.SO_LUONG * obj.DON_GIA;
                                            $scope.dsChitietTheoHangHoa.push(objNS);
                                            $scope.dsChitietTheoHangHoa.forEach((x, index) => x.STT = index + 1);
                                            $scope.dsChitietToanBo.push($scope.dsChitietTheoHangHoa[$scope.dsChitietTheoHangHoa.length - 1]);
                                            //}
                                        }

                                        if ($scope.TaoBanSao == false) {
                                            $('#chi_tiet_hang_hoa_theo_hd').modal('hide');
                                        }
                                        ThayDoiThanhTienSoLuongTheoHH($scope.dsChitietToanBo);
                                    }
                                },
                                cancel: function () {

                                }
                            }
                        });
                    } else {
                        for (var i = 0; i < $scope.dsmodel.length; i++) {
                            var obj = Object.assign({}, $scope.chitietTheoHangHoa);
                            obj.MO_TA_CT4 = $scope.dsmodel[i];
                            obj.SO_LUONG = 1;
                            obj.THANH_TIEN = obj.SO_LUONG * obj.DON_GIA;
                            $scope.dsChitietTheoHangHoa.push(obj);
                            $scope.dsChitietTheoHangHoa.forEach((x, index) => x.STT = index + 1);
                            $scope.dsChitietToanBo.push($scope.dsChitietTheoHangHoa[$scope.dsChitietTheoHangHoa.length - 1]);
                        }
                        const chuaCoSerial = $scope.chitietTheoHangHoa.SO_LUONG - $scope.dsmodel.length;
                        if (chuaCoSerial > 0) {
                            //for (var j = 0; j < chuaCoSerial; j++) {
                            var objNS = Object.assign({}, $scope.chitietTheoHangHoa);
                            objNS.SO_LUONG = chuaCoSerial;
                            obj.THANH_TIEN = obj.SO_LUONG * obj.DON_GIA;
                            $scope.dsChitietTheoHangHoa.push(objNS);
                            $scope.dsChitietTheoHangHoa.forEach((x, index) => x.STT = index + 1);
                            $scope.dsChitietToanBo.push($scope.dsChitietTheoHangHoa[$scope.dsChitietTheoHangHoa.length - 1]);
                            //}
                        }
                        if ($scope.TaoBanSao == false) {
                            $('#chi_tiet_hang_hoa_theo_hd').modal('hide');
                        }
                        ThayDoiThanhTienSoLuongTheoHH($scope.dsChitietToanBo);
                    }
                }
                else {
                    const chuaCoSerial = $scope.chitietTheoHangHoa.SO_LUONG;
                    for (var j = 0; j < chuaCoSerial; j++) {
                        var obj = Object.assign({}, $scope.chitietTheoHangHoa);
                        obj.THANH_TIEN = obj.SO_LUONG * obj.DON_GIA;
                        $scope.dsChitietTheoHangHoa.push(obj);
                        $scope.dsChitietTheoHangHoa.forEach((x, index) => x.STT = index + 1);
                        $scope.dsChitietToanBo.push($scope.dsChitietTheoHangHoa[$scope.dsChitietTheoHangHoa.length - 1]);
                    }
                    if ($scope.TaoBanSao == false) {
                        $('#chi_tiet_hang_hoa_theo_hd').modal('hide');
                    }
                    ThayDoiThanhTienSoLuongTheoHH($scope.dsChitietToanBo);
                }
            }
        }
    }

    $scope.LuuChinhSuaChiTiet = () => {
        if ($('#cs-chi-tiet-form').valid()) {

            var index = $scope.chitietTheoHangHoa.INDEX;

            $scope.chitietTheoHangHoa.KHO = $scope.danhmuckho.find(y => y.ID === $scope.chitietTheoHangHoa.ID_KHO);
            $scope.chitietTheoHangHoa.HOP_DONG = $scope.danhmuchopdong.find(y => y.ID === $scope.chitietTheoHangHoa.ID_HOP_DONG);
            $scope.chitietTheoHangHoa.CHAT_LUONG = $scope.dmchatluong.find(y => y.ID === $scope.chitietTheoHangHoa.ID_CHAT_LUONG);
            $scope.dsChitietTheoHangHoa[index] = $scope.chitietTheoHangHoa;
            //$scope.dsChitietTheoHangHoa.forEach((x, index) => x.STT = index + 1);
            var a = $scope.dsChitietToanBo.findIndex(x => x.ID_HANG === $scope.dsChitietTheoHangHoa[index].ID_HANG && x.STT === $scope.dsChitietTheoHangHoa[index].ID_HANG);

            $scope.dsChitietToanBo[a] = $scope.chitietTheoHangHoa;
            $('#cs-chi_tiet_hang_hoa_theo_hd').modal('hide');
            ThayDoiThanhTienSoLuongTheoHH($scope.dsChitietTheoHangHoa);
        }
    }
    ThayDoiThanhTienSoLuongTheoHH = (arr) => {
        var tongSoLuongDaChon = arr.sum('SO_LUONG');
        var tongThanhTienDaChon = arr.sum('THANH_TIEN');
        if (tongSoLuongDaChon > $scope.soLuongDaChon) {
            $ngConfirm({
                title: 'Thông báo',
                content: 'Tổng số lượng hàng hóa khác nhau! Bạn có muốn cập nhật?',

                buttons: {
                    change: {
                        text: 'Cập nhật',
                        btnClass: 'btn-red',
                        action: function () {
                            $scope.soLuongDaChon = tongSoLuongDaChon;
                            $scope.thanhTienDaChon = tongThanhTienDaChon;
                            $scope.dsHangHoaDaChon[$scope.selectedGood].SO_LUONG = tongSoLuongDaChon;
                            $scope.$apply();
                        }
                    },
                    cancel: function () {

                    }
                }
            });
        } else {
            $scope.soLuongDaChon = tongSoLuongDaChon;
            $scope.thanhTienDaChon = tongThanhTienDaChon;
        }
    }



    $scope.ValueCount = (index) => {
        if (typeof $scope.chitietTheoHangHoa.SO_LUONG == 'number' && typeof $scope.chitietTheoHangHoa.DON_GIA == 'number') {
            $scope.chitietTheoHangHoa.THANH_TIEN = $scope.chitietTheoHangHoa.SO_LUONG * $scope.chitietTheoHangHoa.DON_GIA;
        };
    }
    $scope.AddModel = (event) => {
        if (event.keyCode === 13) {
            // check trung serial
            var check = $scope.dsmodel.filter(x => x == $scope.chitietTheoHangHoa.SO_SERIAL);
            if (check == null || check.length == 0) {
                event.preventDefault();
                $scope.dsmodel.push($scope.chitietTheoHangHoa.SO_SERIAL);
                //$scope.FNSetShowHideSerial();
                $scope.SerialStatus = 3;
                $scope.chitietTheoHangHoa.MO_TA_CT3 = null;
                $scope.chitietTheoHangHoa.MO_TA_CT2 = null;
            }
            $scope.chitietTheoHangHoa.SO_SERIAL = null;
        }
    }
    $scope.XoaModel = (index) => {
        $scope.dsmodel.splice(index, 1);
        //$scope.FNSetShowHideSerial();
        if ($scope.dsmodel != null && $scope.dsmodel.length > 0) {
            $scope.SerialStatus = 3;
            $scope.chitietTheoHangHoa.MO_TA_CT3 = null;
            $scope.chitietTheoHangHoa.MO_TA_CT2 = null;
        } else {
            $scope.SerialStatus = 1;
        }
    }

    $scope.FNSetShowHideSerial = () => {
        $scope.SerialStatus = 1;
        if (($scope.dsmodel != null && $scope.dsmodel.length > 0)
            || ($scope.chitietTheoHangHoa.SO_SERIAL != null && $scope.chitietTheoHangHoa.SO_SERIAL != '')) {
            $scope.SerialStatus = 3;
            $scope.chitietTheoHangHoa.MO_TA_CT3 = null;
            $scope.chitietTheoHangHoa.MO_TA_CT2 = null;

            $scope.chitietTheoHangHoa.SO_LUONG = $scope.dsmodel.length;
            if (typeof $scope.chitietTheoHangHoa.SO_LUONG == 'number' && typeof $scope.chitietTheoHangHoa.DON_GIA == 'number') {
                $scope.chitietTheoHangHoa.THANH_TIEN = $scope.chitietTheoHangHoa.SO_LUONG * $scope.chitietTheoHangHoa.DON_GIA;
            };
        } else {
            if (($scope.chitietTheoHangHoa.MO_TA_CT3 != null && $scope.chitietTheoHangHoa.MO_TA_CT3 != '')
                || ($scope.chitietTheoHangHoa.MO_TA_CT2 != null && $scope.chitietTheoHangHoa.MO_TA_CT2 != '')
            ) {
                $scope.SerialStatus = 2;
                $scope.dsmodel = [];
                $scope.chitietTheoHangHoa.SO_SERIAL = "";
                $scope.chitietTheoHangHoa.SO_LUONG = 1;
                if (typeof $scope.chitietTheoHangHoa.SO_LUONG == 'number' && typeof $scope.chitietTheoHangHoa.DON_GIA == 'number') {
                    $scope.chitietTheoHangHoa.THANH_TIEN = $scope.chitietTheoHangHoa.SO_LUONG * $scope.chitietTheoHangHoa.DON_GIA;
                };
            }
        }
    }


    // Thay đổi số khung hoặc số máy
    $scope.ChangeSoKhung_SoMay = (index) => {
        $scope.FNSetShowHideSerial();
    }

    $scope.IndexChiTietHangHoa = -1;

    // Thay đổi số khung hoặc số máy
    $scope.ChooseChiTietHangHoa = (index) => {
        $scope.IndexChiTietHangHoa = index;
        $scope.disabledChiTietHH = false;
    }

    // Xóa chi tiết hàng hóa
    $scope.XoaChiTietHangHoa = () => {
        $scope.disabledChiTietHH = true;
        var chiTiet = $scope.dsChitietTheoHangHoa[$scope.IndexChiTietHangHoa];

        // Xóa ỏ danh sách tổng
        var dsChuaXoa = $scope.dsChitietToanBo.filter(x => !(x.ID_HANG == chiTiet.ID_HANG && (x.SO_SERIAL == chiTiet.SO_SERIAL || (x.MO_TA_CT3 == chiTiet.MO_TA_CT3 && x.MO_TA_CT2 == chiTiet.MO_TA_CT2))));

        if (dsChuaXoa != null && dsChuaXoa.length > 0)
            $scope.dsChitietToanBo = dsChuaXoa;
        else
            $scope.dsChitietToanBo = [];

        $scope.dsChitietTheoHangHoa.splice($scope.IndexChiTietHangHoa, 1);
    }

    ////////////////////////////// End Thêm, sửa hàng hóa chi tiết ///////////////////////////////


    ////////////////////////////// Tìm kiếm hợp đồng //////////////////////////////////////////////
    $scope.SearchHH_SoHopDong = "";
    $scope.SearchHH_CongTy = 0;
    $scope.SearchHH_HopDong = {};
    $scope.SearchHH_TuNgay = "";
    $scope.SearchHH_DenNgay = "";



    $scope.ShowPopupSearchHD = () => {
        $scope.SearchHH_TuNgay = moment(new Date()).format('DD/MM/YYYY');
        $scope.SearchHH_DenNgay = moment(new Date()).format('DD/MM/YYYY');
        $('#search-hop-dong').modal('show');
    }


    // Chọn hợp đồng
    $scope.SearchHH_SelectedHopDong = (item) => {
        $scope.SearchHH_HopDong = item;
    }

    // 
    $scope.SearchHH_ChooseHopDong = () => {
        $('#search-hop-dong').modal('hide');
        $scope.chitietTheoHangHoa.ID_HOP_DONG = $scope.SearchHH_HopDong.ID_HOP_DONG;
        // lay id nguon kinh phi
        if ($scope.SearchHH_HopDong.NGUON_KP != null && $scope.SearchHH_HopDong.NGUON_KP.length > 0) {
            var arrary = $scope.SearchHH_HopDong.NGUON_KP.split(',');
            $scope.chitietTheoHangHoa.ID_NGUON_KP = parseInt(arrary[0]);
        }
    }

    // Chọn hợp đồng
    $scope.ChangeHopDong = (item) => {
        // lay id nguon kinh phi
        if (item.NGUON_KP != null && item.NGUON_KP.length > 0) {
            var arrary = item.NGUON_KP.split(',');
            $scope.chitietTheoHangHoa.ID_NGUON_KP = parseInt(arrary[0]);
        }
    }

    $scope.LayDanhSachHopDong = () => {
        if (($scope.SearchHH_SoHopDong == null || $scope.SearchHH_SoHopDong == '')
            && ($scope.SearchHH_TuNgay == null || $scope.SearchHH_TuNgay == '')
            && ($scope.SearchHH_DenNgay == null || $scope.SearchHH_DenNgay == '')
            && ($scope.SearchHH_CongTy == null || $scope.SearchHH_CongTy == 0)) {
            toastr.error('Vui lòng nhập điều kiện tìm kiếm!');
        } else {
            TimKiemHopDong();
        }
    }

    TimKiemHopDong = () => {
        var date = new Date();
        date.setDate(date.getDate() - 30);

        var tuNgay = '';
        if ($scope.SearchHH_TuNgay != null && $scope.SearchHH_TuNgay != '') {
            tuNgay = moment($scope.SearchHH_TuNgay, 'DD/MM/YYYY').format('YYYYMMDD');
        }
        else {
            tuNgay = moment(date).format('YYYYMMDD');
        }

        var denNgay = '';
        if ($scope.SearchHH_DenNgay != null && $scope.SearchHH_DenNgay != '') {
            denNgay = moment($scope.SearchHH_DenNgay, 'DD/MM/YYYY').format('YYYYMMDD');
        } else {
            denNgay = moment(new Date()).format('YYYYMMDD');
        }

        if ($scope.SearchHH_CongTy == null || $scope.SearchHH_CongTy == '')
            $scope.SearchHH_CongTy = 0;

        showToast();
        $.ajax({
            type: 'post',
            url: '/KeHoachDieuChuyen/LayHopDong',
            data: {
                soHd: $scope.SearchHH_SoHopDong,
                tuNgay: tuNgay,
                denNgay: denNgay,
                congTyId: $scope.SearchHH_CongTy
            },
            success: function (response) {
                if (response.Status === 200) {
                    $scope.danhmuchopdong = response.Data;
                }
                else if (response.Status === 404) {
                    toastr.error('Không tìm thấy kết quả nào!');
                }
                else {
                    toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm!');
                }
                console.log(response);

                $scope.$apply();
            }, error: (e) => {
                toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm!');
            },
            complete: () => {
                hideLoading();
            }
        });
    }

    /////////////////////////////// End Tìm kiếm hợp đồng /////////////////////////////////////////

    $scope.InPhieu = () => {
        if ($scope.thongTinKh != null && $scope.thongTinKh.ID_KE_HOACH > 0) {
            $.ajax({
                url: '/KeHoachDieuChuyen/ExportToPDF',
                type: 'post',
                data: {
                    idPhieu: $scope.thongTinKh.ID_KE_HOACH
                },
                success: function (result) {

                    let pdfWindow = window.open("");
                    pdfWindow.document.write("<iframe width='100%' height='100%' src='data:application/pdf;base64," + result + "'></iframe>");

                },
                error: function (xhr, status, err) {
                    alert(err);
                }
            });
        }

        return false;
    }

});

