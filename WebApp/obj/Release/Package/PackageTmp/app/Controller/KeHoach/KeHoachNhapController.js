app.controller("KeHoachNhapController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.canbo = [
        { 'ID': 1, 'HoTen': 'Nguyễn Tùng Dương', 'ChucVu': 'Lao công', 'CapBac': 'Hạ Sĩ' },
        { 'ID': 2, 'HoTen': 'Thái Minh Đức', 'ChucVu': 'Tạp vụ', 'CapBac': 'Hạ Sĩ' },
        { 'ID': 3, 'HoTen': 'Lê Quang Dũng', 'ChucVu': 'Bảo vệ', 'CapBac': 'Hạ Sĩ' },
        { 'ID': 4, 'HoTen': 'Lê Đức Anh', 'ChucVu': 'Bảo kê', 'CapBac': 'Hạ Sĩ' },
        { 'ID': 5, 'HoTen': 'Lê Việt Bách', 'ChucVu': 'Tổng quản', 'CapBac': 'Đại Tướng' },
        { 'ID': 6, 'HoTen': 'Phạm Ngô Đức', 'ChucVu': 'Chủ tịch', 'CapBac': 'Đại Tướng' }
    ];
    $scope.donvitinh = [
        { 'ID': 1, 'NAME': 'Chiếc' },
        { 'ID': 2, 'NAME': 'Đôi' },
        { 'ID': 3, 'NAME': 'Bộ' },
        { 'ID': 3, 'NAME': 'Kg' },
        { 'ID': 3, 'NAME': 'Lít' },
    ];

    $scope.danhmuchopdong = [
        { 'ID': 1, 'NAME': 'HD01' },
        { 'ID': 2, 'NAME': 'HD02' },
        { 'ID': 3, 'NAME': 'HD03' }
    ];
    $scope.dmcongty = [
        { 'ID': 1, 'TEN_CONG_TY': 'TECAPRO' },
        { 'ID': 2, 'TEN_CONG_TY': 'INFOMED' },
        { 'ID': 3, 'TEN_CONG_TY': 'ARMEPHACO' }
    ];
    $scope.dmnhomhang = [
        { 'ID': 1, 'TEN_NHOM_HANG': 'Xe ô tô chuyên dùng' },
        { 'ID': 2, 'TEN_NHOM_HANG': 'Xe ô tô bán tải' },
        { 'ID': 3, 'TEN_NHOM_HANG': 'Xe ô tô lội nước' },
    ];
    $scope.dmloaihang = [
        { 'ID': 1, 'TEN_LOAI_HANG': 'Xe ô tô 4-5 chỗ' },
        { 'ID': 2, 'TEN_LOAI_HANG': 'Xe ô tô 7 chỗ' },
        { 'ID': 3, 'TEN_LOAI_HANG': 'Xe ô tô 29 chỗ' },
    ];

    $scope.dmhtvanchuyen = [
        { 'ID': 1, 'TEN_HINH_THUC': 'Đơn vị tự vận chuyển' },
        { 'ID': 2, 'TEN_HINH_THUC': 'H03 vận chuyển' },
    ];

        
    $scope.danhmuckho = [
        { 'ID': 1, 'NAME': 'Kho Hà Đông' },
        { 'ID': 2, 'NAME': 'Kho Đại Mỗ' },
        { 'ID': 3, 'NAME': 'Kho Duy Tân' },
        { 'ID': 4, 'NAME': 'Kho Phan Bội Châu' },
    ];

    $scope.nhomhang = [
        { 'ID': 1, 'NAME': 'Phương tiện đường bộ' },
        { 'ID': 2, 'NAME': 'Phương tiện đường thủy' },
        { 'ID': 3, 'NAME': 'Vật tư, thiết bị kỹ thuật nghiệp vụ' },
        { 'ID': 4, 'NAME': 'Vũ khí' },
        { 'ID': 5, 'NAME': 'Quân trang' },
    ];
    $scope.hanghoa = [
        { 'ID_HANG': 1, 'TEN_HANG': 'Áo lót nam', 'MA_HANG': '01', 'Selected': false,'LOAI_HANG':'LINH_TINH' },
        { 'ID_HANG': 2, 'TEN_HANG': 'Áo lót nữ', 'MA_HANG': '02', 'Selected': false, 'LOAI_HANG': 'LINH_TINH'  },
        { 'ID_HANG': 3, 'TEN_HANG': 'Quần xịp nữ', 'MA_HANG': '03', 'Selected': false, 'LOAI_HANG': 'LINH_TINH'  },
        { 'ID_HANG': 4, 'TEN_HANG': 'Dầu ăn', 'MA_HANG': '04', 'Selected': false, 'LOAI_HANG': 'LINH_TINH'  },
        { 'ID_HANG': 5, 'TEN_HANG': 'Đạn 6.2mm', 'MA_HANG': '05', 'Selected': false, 'LOAI_HANG': 'LINH_TINH'  },
        { 'ID_HANG': 6, 'TEN_HANG': 'Xe tăng', 'MA_HANG': '05', 'Selected': false, 'LOAI_HANG': 'PHUONG_TIEN'  },
    ];
    
    $scope.trangthai = [
        { 'MA': 'U', 'NAME': 'Chưa duyệt' },
        { 'MA': 'A', 'NAME': 'Đã duyệt' },
        { 'MA': 'D', 'NAME': 'Đã bị húy' }
    ];
    $scope.donvi = [
        { 'ID': 1, 'NAME': 'CA Thái Bình' },
        { 'ID': 2, 'NAME': 'CA Hà Nội' },
        { 'ID': 3, 'NAME': 'CA Bắc Ninh' },
        { 'ID': 3, 'NAME': 'CA TP Hồ Chí Minh' },
        { 'ID': 3, 'NAME': 'CA Đà Nẵng' },
    ]
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
    }
    
    ResetVariables = () => {
        $scope.dsHangHoaChon = $scope.hanghoa.map(object => ({ ...object }));
        $scope.dsHangHoaDaChon = [];
        $scope.dsChitietTheoHangHoa = [];
        $scope.dsChitietToanBo = [];
        $scope.thongTinKh = {};
        $scope.dsmodel = [];
        $scope.checkAllLeft = false;
        $scope.checkAllRight = false;
        $scope.checkAllHangHoa = false;
    }
    $scope.pageSize = 10;
    $scope.currentPage = 1;
    $scope.SoLuongKetQua = 0;
    var date = new Date();
    date.setDate(date.getDate() - 7);
    $scope.hangDoi = {
        'TuNgay': moment(date).format('DD/MM/YYYY'), 'DenNgay': moment(new Date()).format('DD/MM/YYYY'), Kho: null, DonVi: null
    }

    $scope.ThemKeHoach = (user,appCode) => {
        ResetVariables();
        $scope.thongTinKh.NGUOI_LAP = user;
        $scope.thongTinKh.APP_CODE = appCode;
        DisablePanel(false);
        $scope.HsStatus = false;
    }

    $scope.SuaKeHoach = () => {
        DisablePanel(false);
        $scope.HsStatus = false;
    }
    GetDanhMuc = () => {
        showToast();
        try {
            $.ajax({
                type: 'post',
                url: '/KeHoachNhap/GetDanhMucByAppCode',
                data: {},
                success: function (response) {
                    if (response.Status === 200) {
                        var kq = response.Data;
                        $scope.dmnguonkinhphi = response.Data.filter(x => x.LOAI === 'NGUON_KINH_PHI');
                        $scope.dmnguonhang = response.Data.filter(x => x.LOAI === 'NGUON_GOC_HH');
                        $scope.dmchatluong = response.Data.filter(x => x.LOAI === 'CAP_CHAT_LUONG');
                    }
                    console.log(response);

                    $scope.$apply();
                }, error: (e) => {
                    console.log(e);
                },
                complete: () => {
                    hideLoading();
                }
            });
        } catch (e) {
            console.log(e);
        }
    }
    GetDanhMuc();
    DefaultSettings();

    $scope.ShowChiTietHangHoa = (index) => {
        //DisablePanel(true);
        $scope.selectedGood = index;
        $scope.tenHangHoaDaChon = Object.assign('',$scope.dsHangHoaDaChon[index].TEN_HANG);
        var idHang = $scope.dsHangHoaDaChon[index].ID_HANG;
        const hangHoaCu = $scope.dsChitietToanBo.some(x => x.ID_HANG === idHang);
        if (!hangHoaCu) {
            $scope.dsChitietTheoHangHoa = $scope.dsChitietToanBo.filter(x => x.ID_HANG === idHang);
            $scope.tenHangHoaDaChon = $scope.dsChitietTheoHangHoa[0].TEN_HANG;
            $scope.soLuongDaChon = $scope.dsChitietTheoHangHoa.sum('SO_LUONG');
            $scope.thanhTienDaChon = $scope.dsChitietTheoHangHoa.sum('THANH_TIEN');
            //$scope.dsChitietTheoHangHoa.forEach((x, index) => x.STT = index + 1);
        } else {
            $scope.dsChitietTheoHangHoa = [];
            $scope.tenHangHoaDaChon = $scope.dsHangHoaDaChon[$scope.selectedGood].TEN_HANG;
            $scope.soLuongDaChon = $scope.dsHangHoaDaChon[$scope.selectedGood].SO_LUONG;
            $scope.soLuongDaChon = null;
        }
    }
   
    $scope.LuuDsHangHoa = () => {
        var a = $scope.dsHangHoaDaChon.filter(x => x.SO_LUONG == null);
        if (a.length > 0) {
            toastr.error('Số lượng không được để trống!');
        } else {
            $scope.dsHangHoaDaChon.forEach((x, index) => { x.STT = index + 1; x.Selected = false; });
            $scope.selectedGood = 0;
            $scope.ShowChiTietHangHoa(0);
            $('#them_hang_hoa').modal('hide');
        }
    }
    $scope.XemChiTietHangHoa = () => {
        $scope.ChiTietHangHoa = [];
        $('#chi_tiet_hang').modal('show');
    }

    $scope.ngayLapKh = moment(new Date()).format('DD/MM/YYYY');

    $scope.TimKiem = (num) => {
        showToast();
        DefaultSettings();
        console.log($scope.hangDoi);
        $.ajax({
            type: 'post',
            url: '/KeHoachNhap/TimKiemKeHoach',
            data: {
                soKeHoach: $scope.hangDoi.SoKeHoach,
                tuNgay: moment($scope.hangDoi.TuNgay, 'DD/MM/YYYY').format('YYYYMMDD'),
                denNgay: moment($scope.hangDoi.DenNgay, 'DD/MM/YYYY').add(1, 'days').format('YYYYMMDD'),
                trangThai: null,
                pageSize: $scope.pageSize,
                pageNumber: $scope.currentPage
            },
            success: function (response) {
                if (response.Status === 200) {
                    $scope.dsKetQua = response.Data;
                    var a = $scope.trangthai.find(y => y.MA === 'U').NAME
                    $scope.dsKetQua.forEach(x => { x.TRANG_THAI_VW = $scope.trangthai.find(y => y.MA === x.TRANG_THAI).NAME, x.NGAY_TAO = moment(new Date(Number(x.NGAY_TAO.replace(/\D/g, '')))).format('DD/MM/YYYY') });
                    $scope.CoKetQua = true;
                    $scope.SoLuongKetQua = $scope.dsKetQua.length;
                    $scope.SoLuongHienThi = $scope.dsKetQua.length;
                    console.log(response.Data);
                    $scope.selectedRow = 0;
                    $scope.ShowThongTin($scope.dsKetQua[0], 0);
                }
                else if (response.Status === 400) {
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

    ThongTinHangHoa = (id) => {
        $scope.dsHangHoa = [];
        $.ajax({
            type: 'post',
            url: '/KeHoachNhap/DanhSachHangHoaTheoKeHoach',
            data: {
                idKeHoach: id
            },
            success: function (response) {
                if (response.Status === 200) {
                    $scope.thongTinKh.ID_NGUON_GOC = response.Data[0].ID_NGUON_GOC;
                    $scope.thongTinKh.ID_NGUON_KP = response.Data[0].ID_NGUON_KP;
                    $('#btnSave').prop('disabled', false);
                    $('#btnConfirm').prop('disabled', false);
                    $('#btnRemove').prop('disabled', false);
                    $scope.dsChitietToanBo = response.Data;
                    $scope.dsChitietToanBo.forEach(x => {
                        x.KHO = $scope.danhmuckho.find(y => y.ID === x.ID_KHO),
                        x.HOP_DONG = $scope.danhmuchopdong.find(y => y.ID === x.ID_HOP_DONG),
                        x.CHAT_LUONG = $scope.dmchatluong.find(y => y.ID === x.ID_CHAT_LUONG)
                    });
                    const distinctHangHoa = [...new Set($scope.dsChitietToanBo.map(item => item.ID_HANG))];
                    $scope.dsHangHoaDaChon = $scope.dsHangHoaChon.filter(x => distinctHangHoa.includes(x.ID_HANG));
                    $scope.dsHangHoaDaChon.forEach((x, index) => { x.STT = index + 1, x.SO_LUONG = $scope.dsChitietToanBo.filter(y => y.ID_HANG === x.ID_HANG).sum('SO_LUONG') });
                    var b = new Set($scope.dsHangHoaDaChon);
                    $scope.dsHangHoaChon = [...$scope.dsHangHoaChon].filter(x => !b.has(x));
                    $scope.ShowChiTietHangHoa(0);
                }
                else {
                    toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm!');
                }
                console.log(response);

                $scope.$apply();
            }, error: (e) => {
                toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm!');
            }
        });
    }

    $scope.ShowThongTin = (item, index) => {
        showToast();
        try {
            ResetVariables();
            var a = $scope.hanghoa;
            $scope.selectedRow = index;
            $scope.thongTinKh = item;
            if ($scope.thongTinKh.NGAY_NHAP_DK.length === 8) {
                $scope.thongTinKh.NGAY_NHAP_DK_VW = moment($scope.thongTinKh.NGAY_NHAP_DK, 'YYYYMMDD').format('DD/MM/YYYY')
            }
            if ($scope.thongTinKh.NGAY_KH.length === 8) {
                $scope.thongTinKh.NGAY_KH_VW = moment($scope.thongTinKh.NGAY_KH, 'YYYYMMDD').format('DD/MM/YYYY')
            }
            if ($scope.thongTinKh.TRANG_THAI === 'A') {
                $scope.HsStatus = true;
            } else {
                $scope.HsStatus = false;
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
    $scope.ThemMoiChiTietHangHoa = () => {
        if ($('#main-info').valid()) {
            $scope.chitietTheoHangHoa = {};
            chitietValidate.resetForm();
            $('#chi-tiet-form').find('input, select').removeClass('error');
            if ($scope.dsHangHoaDaChon[$scope.selectedGood].LOAI_HANG === 'PHUONG_TIEN') {
                $scope.SerialStatus = false;
            } else {
                $scope.SerialStatus = true;
            }
            $scope.chitietTheoHangHoa.SO_LUONG = 1;
            $('#hang_hoa_theo_hd').modal('show');
        }
    }

    $scope.SuaChiTietHangHoa = (index) => {
        if ($('#main-info').valid()) {
            $scope.chitietTheoHangHoa = $scope.dsChitietTheoHangHoa[index];
            chitietValidate.resetForm();
            $('#cs-chi-tiet-form').find('input, select').removeClass('error');
            $('#cs-hang_hoa_theo_hd').modal('show');
        }
    }

    $scope.LuuChiTiet = () => {
        $scope.chitietTheoHangHoa.ID_HANG = $scope.dsHangHoaDaChon[$scope.selectedGood].ID_HANG;
        $scope.chitietTheoHangHoa.TEN_HANG = $scope.dsHangHoaDaChon[$scope.selectedGood].TEN_HANG;
        $scope.chitietTheoHangHoa.MA_HANG = $scope.dsHangHoaDaChon[$scope.selectedGood].MA_HANG;
        $scope.chitietTheoHangHoa.TEN_NHOM_HH = $scope.dmnhomhang.find(x => x.ID === $scope.chitietTheoHangHoa.ID_NHOM_HH);
        $scope.chitietTheoHangHoa.TEN_LOAI_HH = $scope.dmloaihang.find(x => x.ID === $scope.chitietTheoHangHoa.ID_LOAI_HH);
        $scope.chitietTheoHangHoa.DVT = $scope.dsHangHoaDaChon[$scope.selectedGood].DVT;
        $scope.chitietTheoHangHoa.ID_NGUON_KP = $scope.thongTinKh.ID_NGUON_KP;
        $scope.chitietTheoHangHoa.ID_NGUON_GOC = $scope.thongTinKh.ID_NGUON_GOC;
        console.log($scope.chitietTheoHangHoa);
        $scope.chitietTheoHangHoa.KHO = $scope.danhmuckho.find(y => y.ID === $scope.chitietTheoHangHoa.ID_KHO);
        $scope.chitietTheoHangHoa.HOP_DONG = $scope.danhmuchopdong.find(y => y.ID === $scope.chitietTheoHangHoa.ID_HOP_DONG);
        $scope.chitietTheoHangHoa.CHAT_LUONG = $scope.danhmuckho.find(y => y.ID === $scope.chitietTheoHangHoa.ID_CHAT_LUONG);
        if ($('#chi-tiet-form').valid()) {
            if ($scope.dsmodel.length > 0) {
                for (var i = 0; i < $scope.dsmodel.length; i++) {
                    var obj = Object.assign({}, $scope.chitietTheoHangHoa);
                    obj.MO_TA_CT4 = $scope.dsmodel[i];
                    obj.SO_LUONG = 1;
                    $scope.dsChitietTheoHangHoa.push(obj);
                    $scope.dsChitietTheoHangHoa.forEach((x, index) => x.STT = index + 1);
                    $scope.dsChitietToanBo.push($scope.dsChitietTheoHangHoa[$scope.dsChitietTheoHangHoa.length - 1]);
                }
                const chuaCoSerial = $scope.chitietTheoHangHoa.SO_LUONG - $scope.dsmodel.length;
                for (var j = 0; j < chuaCoSerial; j++) {
                    var obj = Object.assign({}, $scope.chitietTheoHangHoa);
                    obj.SO_LUONG = 1;
                    $scope.dsChitietTheoHangHoa.push(obj);
                    $scope.dsChitietTheoHangHoa.forEach((x, index) => x.STT = index + 1);
                    $scope.dsChitietToanBo.push($scope.dsChitietTheoHangHoa[$scope.dsChitietTheoHangHoa.length - 1]);
                }
                $('#hang_hoa_theo_hd').modal('hide');
                ThayDoiThanhTienSoLuongTheoHH($scope.dsChitietTheoHangHoa);
            }
            else {
                const chuaCoSerial = $scope.chitietTheoHangHoa.SO_LUONG;
                for (var j = 0; j < chuaCoSerial; j++) {
                    var obj = Object.assign({}, $scope.chitietTheoHangHoa);
                    $scope.dsChitietTheoHangHoa.push(obj);
                    $scope.dsChitietTheoHangHoa.forEach((x, index) => x.STT = index + 1);
                    $scope.dsChitietToanBo.push($scope.dsChitietTheoHangHoa[$scope.dsChitietTheoHangHoa.length - 1]);
                }
                $('#hang_hoa_theo_hd').modal('hide');
                ThayDoiThanhTienSoLuongTheoHH($scope.dsChitietTheoHangHoa);
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
            $('#cs-hang_hoa_theo_hd').modal('hide');
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

    $scope.DuyetPhieu = () => {
        showToast();
        try {
            if ($scope.thongTinKh.TRANG_THAI === 'A') {
                toastr.error('Hồ sơ đã duyệt, không thể duyệt!');
            }
            else {
                $.ajax({
                    type: 'post',
                    url: '/KeHoachNhap/DuyetKeHoach',
                    data: {
                        idKh: $scope.thongTinKh.ID_KE_HOACH,
                    },
                    success: function (response) {
                        if (response.Status === 200) {
                            $scope.thongTinKh.TRANG_THAI === 'A';
                            $scope.dsKetQua[$scope.selectedRow].TRANG_THAI = 'A';
                            $scope.dsKetQua[$scope.selectedRow].TRANG_THAI_VW = $scope.trangthai.find(y => y.MA === 'A').NAME;
                            $scope.HsStatus = true;
                            toastr.success(response.Message);
                        }
                        else {
                            toastr.error('Có lỗi xảy ra trong quá trình duyệt kế hoạch!');
                        }
                        console.log(response);
                        $scope.$apply();
                    }, error: (e) => {
                        toastr.error('Có lỗi xảy ra trong quá trình duyệt kế hoạch!');
                    }
                });
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


    $scope.XoaKeHoach = () => {
        showToast();
        try {
            if ($scope.thongTinKh.TRANG_THAI === 'A') {
                toastr.error('Hồ sơ đã duyệt, không thể xóa!');
            }
            else {
                $.ajax({
                    type: 'post',
                    url: '/KeHoachNhap/XoaKeHoach',
                    data: {
                        idKh: $scope.thongTinKh.ID_KE_HOACH,
                    },
                    success: function (response) {
                        if (response.Status === 200) {
                            $scope.dsKetQua.splice($scope.selectedRow, 1);
                            ResetVariables();
                            if ($scope.dsKetQua.length > 0) {
                                $scope.ShowThongTin($scope.dsKetQua[0], 0);
                            }
                            toastr.success(response.Message);
                        }
                        else {
                            toastr.error('Có lỗi xảy ra trong quá trình xóa!');
                        }
                        console.log(response);
                        $scope.$apply();
                    }, error: (e) => {
                        toastr.error('Có lỗi xảy ra trong quá trình cập nhật!');
                    }
                });
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
    $scope.LuuPhieu = () => {
        showToast();
        try {
            $scope.thongTinKh.NGAY_KH = moment($scope.thongTinKh.NGAY_KH_VW, 'DD/MM/YYYY').format('YYYYMMDD');
            $scope.thongTinKh.TRANG_THAI = 'U';
            $scope.thongTinKh.NGAY_NHAP_DK = moment($scope.thongTinKh.NGAY_NHAP_DK_VW, 'DD/MM/YYYY').format('YYYYMMDD');
            $scope.dsChitietToanBo.forEach(x =>
            {
                    x.ID_NGUON_KP = $scope.thongTinKh.ID_NGUON_KP,
                    x.ID_NGUON_GOC = $scope.thongTinKh.ID_NGUON_GOC,
                    x.ID_KE_HOACH = $scope.thongTinKh.ID_KE_HOACH
            });

            if ($scope.thongTinKh.ID_KE_HOACH != null) {
                if ($('#main-info').valid()) {
                    if ($scope.dsChitietToanBo.length > 0) {

                        $.ajax({
                            type: 'post',
                            url: '/KeHoachNhap/UpdateKeHoach',
                            data: {
                                thongTinPhieu: $scope.thongTinKh,
                                dsHangHoaChiTiet: $scope.dsChitietToanBo,
                            },
                            success: function (response) {
                                if (response.Status === 200) {
                                    toastr.success('Cập nhật thông tin nhập kho thành công!');
                                }
                                else {
                                    toastr.error('Có lỗi xảy ra trong quá trình cập nhật!');
                                }
                                console.log(response);
                                $scope.$apply();
                            }, error: (e) => {
                                toastr.error('Có lỗi xảy ra trong quá trình cập nhật!');
                            }
                        });
                    } else {
                        toastr.error('Bạn chưa chọn chi tiết cho từng mặt hàng!');
                    }
                }
            } else {
                if ($('#main-info').valid()) {
                    if ($scope.dsChitietToanBo.length > 0) {
                        $scope.thongTinKh.SO_KH = GenerateRandomNumber();

                        $.ajax({
                            type: 'post',
                            url: '/KeHoachNhap/TaoKeHoach',
                            data: {
                                thongTinPhieu: $scope.thongTinKh,
                                dsHangHoaChiTiet: $scope.dsChitietToanBo,
                            },
                            success: function (response) {
                                if (response.Status === 200) {
                                    toastr.success('Lưu thông tin nhập kho thành công!');
                                    $scope.dsChitietToanBo = response.HangHoa;
                                    $scope.thongTinKh.ID_KE_HOACH = response.KeHoach
                                }
                                else {
                                    toastr.error('Có lỗi xảy ra trong quá trình lưu!');
                                }
                                console.log(response);
                                $scope.$apply();
                            }, error: (e) => {
                                toastr.error('Có lỗi xảy ra trong quá trình lưu!');
                            }
                        });
                    } else {
                        toastr.error('Bạn chưa chọn chi tiết cho từng mặt hàng!');
                    }
                }
            }
        }
        catch(e){
            console.log(e);
            toastr.error('Có lỗi xảy ra trong quá trình lưu!');
        }
        finally {
            hideLoading();
        }

    }
    CheckThongTin = () => {
        if ($scope.thongTinKho.NGAY_NK_VW == null || $scope.thongTinKho.NGUOI_KY_4 == null || $scope.thongTinKho.NGUOI_KY_5 == null || $scope.thongTinKho.NGUOI_PHU_TRACH == null || $scope.thongTinKho.NGUOI_GIAO == null || $scope.thongTinKho.NGUOI_NHAN == null) {
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


    $scope.ValueCount = (index) => {
        if (typeof $scope.chitietTheoHangHoa.SO_LUONG == 'number' && typeof $scope.chitietTheoHangHoa.DON_GIA == 'number') {
            $scope.chitietTheoHangHoa.THANH_TIEN = $scope.chitietTheoHangHoa.SO_LUONG * $scope.chitietTheoHangHoa.DON_GIA;
        };
    }
    $scope.ThayDoiTableChiTiet = () => {
        if ($("#img-toggle").hasClass("img-normal")) {
            $("#img-toggle").removeClass("img-normal").addClass("img-180-rotate");
        }
        else if ($("#img-toggle").hasClass("img-180-rotate")) {
            $("#img-toggle").removeClass("img-180-rotate").addClass("img-normal");
        }
        if ($('#right-table').hasClass("col-md-12")) {
            $('#left-table').css("display", "block");
            $('#right-table').removeClass('col-md-12').addClass('col-md-9');
        }
        else {
            $('#left-table').css("display", "none");
            $('#right-table').removeClass('col-md-9').addClass('col-md-12');
        }
    }
    GenerateRandomNumber = () => {
        var newDate = new Date();
        var soPhieu = 'KHN' + moment(newDate).format('YYYYMMDDHHmm');
        return soPhieu;
    }
    $scope.LocDsTheoHangHoa = (item) => {
        return item.ID_HANG === $scope.dsHangHoaDaChon[$scope.selectedGood].ID_HANG;
    }

    var thongtinChungValidate = $('#main-info').validate({
        rules: {
        },
        messages: {
        }
    });

       var chitietValidate =  $('#chi-tiet-form').validate({
            rules: {
                khoxuat: {
                    required: true
                },
                soluong: {
                    required: true
                },
            },
            messages: {
                khoxuat: {
                    required: 'Chọn 1 giá trị đê',
                },
                soluong: {
                    required: 'Nhập vàoooo',
                },
                action: 'Nhập dữ liệu vào xem nào! Hay nhỉ'
            }
       });
    $scope.XoaHangHoaDaChon = () => {
        $scope.dsHangHoaChon = $scope.dsHangHoaChon.concat($scope.dsHangHoaDaChon.filter(x => x.oSELECTED === true));

        $scope.dsHangHoaDaChon.forEach(x => {
            if (x.oSELECTED) {
                $scope.dsChitietToanBo = $scope.dsChitietToanBo.filter(y => y.ID_HANG !== x.ID_HANG)
            }
           
        });
        console.log($scope.dsChitietToanBo);

        $scope.dsHangHoaDaChon = $scope.dsHangHoaDaChon.filter(x => x.oSELECTED !== true);
    }

    $scope.ChonTatCaHangHoa = (type) => {
        if (type===0) {
            $scope.dsHangHoaChon.forEach(x => x.Selected = !$scope.checkAllLeft);
        }
        if (type === 1) {
            $scope.dsHangHoaDaChon.forEach(x => x.Selected = !$scope.checkAllRight);
        }
        if (type === 2) {
            $scope.dsHangHoaDaChon.forEach(x => x.oSELECTED = !$scope.checkAllHangHoa);
        }
    }
    $scope.ChonHangHoa = () => {
        //$scope.dsHangHoaChon.forEach(x => x.HANG_HOA = $scope.hanghoa.find(y => y.MaHang === x.MaHang));
        $scope.dsHangHoaDaChon = $scope.dsHangHoaDaChon.concat($scope.dsHangHoaChon.filter(x => x.Selected === true));
        $scope.dsHangHoaChon = $scope.dsHangHoaChon.filter(x => x.Selected !== true);
        CheckAllHandler();
    }
    $scope.HuyChonHangHoa = () => {
        $scope.dsHangHoaChon = $scope.dsHangHoaChon.concat($scope.dsHangHoaDaChon.filter(x => x.Selected === true));
        $scope.dsHangHoaDaChon = $scope.dsHangHoaDaChon.filter(x => x.Selected !== true);
        CheckAllHandler();
    }
    $scope.TichChonHangHoa = (type,index) => {
        if (type === 0) {
            $scope.dsHangHoaChon[index].Selected = !$scope.dsHangHoaChon[index].Selected;
            if ($scope.dsHangHoaChon.length > 0) {
                if ($scope.dsHangHoaChon.every(x => x.Selected === true)) {
                    $scope.checkAllLeft = true;
                } else {
                    $scope.checkAllLeft = false;
                }
            } else {
                $scope.checkAllLeft = false;
            }
        }
        if (type === 1) {
            $scope.dsHangHoaDaChon[index].Selected = !$scope.dsHangHoaDaChon[index].Selected;
            if ($scope.dsHangHoaDaChon.length > 0) {
                if ($scope.dsHangHoaDaChon.every(x => x.Selected === true)) {
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
            $scope.dsHangHoaDaChon[index].oSELECTED = !$scope.dsHangHoaDaChon[index].oSELECTED;
            if ($scope.dsHangHoaDaChon.length > 0) {
                if ($scope.dsHangHoaDaChon.every(x => x.oSELECTED === true)) {
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

    CheckAllHandler = () => {
        if ($scope.dsHangHoaChon.length > 0) {
            if ($scope.dsHangHoaChon.every(x => x.Selected === true)) {
                $scope.checkAllLeft = true;
            } else {
                $scope.checkAllLeft = false;
            }
        } else {
            $scope.checkAllLeft = false;
        }

        if ($scope.dsHangHoaDaChon.length > 0) {
            if ($scope.dsHangHoaDaChon.every(x => x.Selected === true)) {
                $scope.checkAllRight = true;
            } else {
                $scope.checkAllRight = false;
            }
        }
        else {
            $scope.checkAllRight = false;
        }
    }
    $scope.AddModel = (event) => {
        if (event.keyCode === 13) {
            if ($scope.dsmodel.length + 1 > $scope.chitietTheoHangHoa.SO_LUONG) {
                toastr.error('Số lượng serial vượt quá số lượng đã nhập!');
            }
            else {
                event.preventDefault();
                $scope.dsmodel.push($scope.chitietTheoHangHoa.MO_TA_CT4);
                $scope.chitietTheoHangHoa.MO_TA_CT4 = null;
            }
        }
        
    }
    $scope.XoaModel = (index) => {
        $scope.dsmodel.splice(index, 1);
    }
});

