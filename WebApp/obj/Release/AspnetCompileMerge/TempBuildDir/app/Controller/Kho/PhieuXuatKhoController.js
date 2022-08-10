app.controller("PhieuXuatKhoController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, chuyenGiaSangChu) {
    $scope.dsKeHoach = [];
    $scope.dsXuatKho = [];
    $scope.thongTinKho = {};
    $scope.thongTinKho.NGUOI_KY_1 = null;
    $scope.thongTinKho.NGUOI_KY_2 = null;
    $scope.thongTinKho.NGUOI_KY_3 = null;
    $scope.thongTinKho.NGUOI_KY_4 = null;
    $scope.thongTinKho.NGUOI_KY_5 = null;
    $scope.canbo = [
        { 'ID': 1, 'HoTen': 'Nguyễn Tùng Dương', 'ChucVu': 'Lao công', 'CapBac': 'Hạ Sĩ' },
        { 'ID': 2, 'HoTen': 'Thái Minh Đức', 'ChucVu': 'Tạp vụ', 'CapBac': 'Hạ Sĩ' },
        { 'ID': 3, 'HoTen': 'Lê Quang Dũng', 'ChucVu': 'Bảo vệ', 'CapBac': 'Hạ Sĩ' },
        { 'ID': 4, 'HoTen': 'Lê Đức Anh', 'ChucVu': 'Bảo kê', 'CapBac': 'Hạ Sĩ' },
        { 'ID': 5, 'HoTen': 'Lê Việt Bách', 'ChucVu': 'Tổng quản', 'CapBac': 'Đại Tướng' },
        { 'ID': 6, 'HoTen': 'Phạm Ngô Đức', 'ChucVu': 'Chủ tịch', 'CapBac': 'Đại Tướng' }
    ];
    $scope.donvi = [
        { 'ID': 1, 'NAME': 'CA Thái Bình' },
        { 'ID': 2, 'NAME': 'CA Hà Nội' },
        { 'ID': 3, 'NAME': 'CA Bắc Ninh' },
        { 'ID': 3, 'NAME': 'CA TP Hồ Chí Minh' },
        { 'ID': 3, 'NAME': 'CA Đà Nẵng' },
    ]
    $scope.donvitinh = [
        { 'ID': 1, 'NAME': 'Chiếc' },
        { 'ID': 2, 'NAME': 'Đôi' },
        { 'ID': 3, 'NAME': 'Bộ' },
        { 'ID': 3, 'NAME': 'Kg' },
        { 'ID': 3, 'NAME': 'Lít' },
    ]

    $scope.danhmuckho = [
        { 'ID': 1, 'NAME': 'Kho Hà Đông' },
        { 'ID': 2, 'NAME': 'Kho Đại Mỗ' },
        { 'ID': 3, 'NAME': 'Kho Duy Tân' },
        { 'ID': 4, 'NAME': 'Kho Phan Bội Châu' },
    ]

    $scope.loaihang = [
        { 'ID': 1, 'NAME': 'Loại 1' },
        { 'ID': 2, 'NAME': 'Loại 2' },
        { 'ID': 3, 'NAME': 'Loại 3' },
    ]
    $scope.hanghoa = [
        { 'ID': 1, 'TenHang': 'Quần áo', 'MaHang': '01' },
        { 'ID': 2, 'TenHang': 'Đạn dược', 'MaHang': '02' },
        { 'ID': 3, 'TenHang': 'Xăng dầu', 'MaHang': '03' },
    ]
    $scope.trangthai = [
        { 'MA': 'N', 'NAME': 'Khởi tạo' },
        { 'MA': 'A', 'NAME': 'Đã duyệt' }
    ];

    DefaultSettings = () => {
        $scope.dsKeHoach = [];
        $scope.dsXuatKho = [];
        $scope.thongTinKho = {};
        $('#btnAdd').prop('disabled', false);
        $('#btnEdit').prop('disabled', true);
        $('#btnSave').prop('disabled', true);
        $('#btnPrint').prop('disabled', true);
        $('#btnThemHangHoa').prop('disabled', true);
       
    }
    $scope.pageSize = 10;
    $scope.currentPage = 1;
    $scope.SoLuongKetQua = 0;      
    DefaultSettings();
    var date = new Date();
    date.setDate(date.getDate() - 7);
    $scope.hangDoi = {
        'TuNgay': moment(date).format('DD/MM/YYYY'), 'DenNgay': moment(new Date()).format('DD/MM/YYYY'), Kho: null, DonVi: null
    }
    $scope.TimKiem = () => {
        showToast();
        DefaultSettings();
        $scope.dsKeHoach = [
            { 'IDKeHoach': 1, 'SoKeHoach': 'AXN123', 'NgayLap': '05/02/2021' },
            { 'IDKeHoach': 2, 'SoKeHoach': 'BXN123', 'NgayLap': '15/02/2021' },
            { 'IDKeHoach': 3, 'SoKeHoach': 'CXN123', 'NgayLap': '25/02/2021' },
            { 'IDKeHoach': 4, 'SoKeHoach': 'DXN123', 'NgayLap': '30/02/2021' },
            { 'IDKeHoach': 5, 'SoKeHoach': 'EXN123', 'NgayLap': '01/03/2021' },
            { 'IDKeHoach': 6, 'SoKeHoach': 'FXN123', 'NgayLap': '05/03/2021' }
        ];
        $scope.soLuongKetQua = $scope.dsKeHoach.length;
        $scope.selectedRow = 0;
        $scope.ShowThongTin($scope.dsKeHoach[0], 0);
        //$.ajax({
        //    type: 'post',
        //    url: '/PhieuNhapKho/TimKiemKeHoach',
        //    data: {},
        //    success: function (response) {
        //        console.log(response);
        //        if (response.Error) {
        //            toastr.error(response.Title);
        //        } else {

        //        }

        //        $scope.$apply();
        //    }
        //});   
    };

    CheckThongTin = () => {
        if ($scope.thongTinKho.SO_PHIEU == null || $scope.thongTinKho.KHO == null || $scope.thongTinKho.NGAY_CT_VW == null || $scope.thongTinKho.SO_KE_HOACH == null || $scope.thongTinKho.NGUOI_PHU_TRACH_VW == null || $scope.thongTinKho.DON_VI_NHAN == null
            || $scope.thongTinKho.NGUOI_NHAN == null || $scope.thongTinKho.NGUOI_KY_3 == null || $scope.thongTinKho.NGUOI_KY_2 == null || $scope.thongTinKho.NGUOI_KY_1 == null) {
            toastr.error("Chưa nhập đủ thông tin");
            return false;
        }
        //if ($scope.dsNhapKho.every(CheckHangHoa)) {
        //    toastr.error("Chưa nhập đủ thông tin hàng hóa");
        //    return false;
        //}
        return true;
    }

    CheckHangHoa = (hangHoa) => {
        return (hangHoa.HANG_HOA != null && hangHoa.DVT_VW != null && hangHoa.DVT_VW != null && hangHoa.SO_LUONG_KH != null && hangHoa.DON_GIA != null && hangHoa.GHI_CHU != null)
    }

    $scope.LuuPhieu = () => {
        showToast();
        try {
            if (CheckThongTin()) {
                var a = $scope.dsKeHoach.find(obj => obj.SoKeHoach === $scope.thongTinKho.SO_KE_HOACH);
                if (a !== undefined) {
                    $scope.thongTinKho.ID_KE_HOACH = $scope.dsKeHoach.find(obj => obj.SoKeHoach === $scope.thongTinKho.SO_KE_HOACH).IDKeHoach;
                }
                $scope.thongTinKho.NGAY_CT = moment($scope.thongTinKho.NGAY_CT_VW, 'DD/MM/YYYY').format('YYYYMMDD');
                $scope.thongTinKho.NGAY_KE_HOACH = $scope.thongTinKho.NGAY_KE_HOACH_VW == null ? null : moment($scope.thongTinKho.NGAY_KE_HOACH_VW, 'DD/MM/YYYY').format('YYYYMMDD');
                $scope.thongTinKho.TONG_TIEN = $scope.dsXuatKho.sum("THANH_TIEN_KH");
                $scope.thongTinKho.TONG_TIEN_CHU = chuyenGiaSangChu($scope.thongTinKho.TONG_TIEN);
                $scope.thongTinKho.ID_NGUOI_KY_1 = $scope.thongTinKho.NGUOI_KY_1.ID;
                $scope.thongTinKho.TEN_NGUOI_KY_1 = $scope.thongTinKho.NGUOI_KY_1.HoTen;
                $scope.thongTinKho.CV_NGUOI_KY_1 = $scope.thongTinKho.NGUOI_KY_1.ChucVu;
                $scope.thongTinKho.CB_NGUOI_KY_1 = $scope.thongTinKho.NGUOI_KY_1.CapBac;
                $scope.thongTinKho.ID_NGUOI_KY_2 = $scope.thongTinKho.NGUOI_KY_2.ID;
                $scope.thongTinKho.TEN_NGUOI_KY_2 = $scope.thongTinKho.NGUOI_KY_2.HoTen;
                $scope.thongTinKho.CV_NGUOI_KY_2 = $scope.thongTinKho.NGUOI_KY_2.ChucVu;
                $scope.thongTinKho.CB_NGUOI_KY_2 = $scope.thongTinKho.NGUOI_KY_2.CapBac;
                $scope.thongTinKho.ID_NGUOI_KY_3 = $scope.thongTinKho.NGUOI_KY_3.ID;
                $scope.thongTinKho.TEN_NGUOI_KY_3 = $scope.thongTinKho.NGUOI_KY_3.HoTen;
                $scope.thongTinKho.CV_NGUOI_KY_3 = $scope.thongTinKho.NGUOI_KY_3.ChucVu;
                $scope.thongTinKho.CB_NGUOI_KY_3 = $scope.thongTinKho.NGUOI_KY_3.CapBac;
                $scope.thongTinKho.NGUOI_PHU_TRACH = $scope.thongTinKho.NGUOI_PHU_TRACH_VW.ID;
                $scope.thongTinKho.ID_KHO = $scope.thongTinKho.KHO.ID;
                $scope.thongTinKho.TEN_KHO = $scope.thongTinKho.KHO.NAME;
                $scope.thongTinKho.ID_DON_VI_NHAN = $scope.thongTinKho.DON_VI_NHAN.ID;
                $scope.thongTinKho.TEN_DON_VI_NHAN = $scope.thongTinKho.DON_VI_NHAN.NAME;
                $scope.thongTinKho.ID_NGUOI_NHAN = $scope.thongTinKho.NGUOI_NHAN.ID;
                $scope.thongTinKho.TEN_NGUOI_NHAN = $scope.thongTinKho.NGUOI_NHAN.HoTen;

                if ($scope.dsXuatKho.length === 0) {
                    toastr.error('Bạn chưa thêm hàng hóa nào!');
                } else {
                    $scope.dsXuatKho.forEach((x) => { x.ID_HANG = x.HANG_HOA.ID; x.MA_HANG = x.HANG_HOA.MaHang; x.TEN_HANG = x.HANG_HOA.TenHang, x.DVT = x.DVT_VW.NAME });
                    if ($scope.thongTinKho.ID_TXN != null) {
                        $.ajax({
                            type: 'post',
                            url: '/PhieuXuatKho/UpdatePhieuXuatKho',
                            data: {
                                thongTinPhieu: $scope.thongTinKho,
                                dsHangHoaChiTiet: $scope.dsXuatKho
                            },
                            success: function (response) {
                                if (response.Status === 200) {
                                    toastr.success('Thêm mới hàng hóa thành công!');
                                }
                                else {
                                    toastr.error('Có lỗi xảy ra trong quá trình lưu!');
                                }
                                console.log(response);

                                $scope.$apply();
                            }, error: (e) => {
                                toastr.error('Có lỗi xảy ra trong quá trình lưu!');
                            },
                            complete: () => {
                                hideLoading();
                            }
                        });
                    } else {
                        $.ajax({
                            type: 'post',
                            url: '/PhieuXuatKho/InsertPhieuXuatKho',
                            data: {
                                thongTinPhieu: $scope.thongTinKho,
                                dsHangHoaChiTiet: $scope.dsXuatKho
                            },
                            success: function (response) {
                                if (response.Status === 200) {
                                    $scope.thongTinKho.ID_TXN = response.Data;
                                    toastr.success('Thêm mới hàng hóa thành công!');
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
                    }

                }
            }
        } catch (e) {
            toastr.error('Có lỗi xảy ra trong quá trình lưu!');
        } finally {
            hideLoading();
        }
    }

    $scope.ShowThongTin = (item, index) => {
        $scope.thongTinKho = {};
        $scope.dsXuatKho = [];
        $scope.selectedRow = index;
        $.ajax({
            type: 'post',
            url: '/PhieuXuatKho/GetPhieuBySoKeHoach',
            dataType: 'json',
            data: {
                soKeHoach: item.SoKeHoach
            },
            success: function (response) {
                console.log(response);
                if (response.Data == null) {
                    $scope.thongTinKho.SO_KE_HOACH = item.SoKeHoach;
                    $scope.thongTinKho.SO_PHIEU = GenerateRandomNumber();
                    $scope.thongTinKho.NGAY_KE_HOACH_VW = item.NgayLap;
                    $('#btnAdd').prop('disabled', false);
                    $('#btnEdit').prop('disabled', true);
                    $('#btnThemHangHoa').prop('disabled', false);
                } else {
                    $scope.thongTinKho = response.Data;
                    $scope.thongTinKho.KHO = $scope.danhmuckho.find(x => x.ID === $scope.thongTinKho.ID_KHO);
                    $scope.thongTinKho.DON_VI_NHAN = $scope.donvi.find(x => x.ID === $scope.thongTinKho.ID_DON_VI_NHAN);
                    $scope.thongTinKho.NGUOI_NHAN = $scope.canbo.find(x => x.ID === $scope.thongTinKho.ID_NGUOI_NHAN);
                    $scope.thongTinKho.NGUOI_PHU_TRACH_VW = $scope.canbo.find(x => x.ID === $scope.thongTinKho.NGUOI_PHU_TRACH);
                    $scope.thongTinKho.NGUOI_KY_1 = $scope.canbo.find(x => x.ID === $scope.thongTinKho.ID_NGUOI_KY_1);
                    $scope.thongTinKho.NGUOI_KY_2 = $scope.canbo.find(x => x.ID === $scope.thongTinKho.ID_NGUOI_KY_2);
                    $scope.thongTinKho.NGUOI_KY_3 = $scope.canbo.find(x => x.ID === $scope.thongTinKho.ID_NGUOI_KY_3);
                    if ($scope.thongTinKho.NGAY_KE_HOACH.length === 8) {
                        $scope.thongTinKho.NGAY_KE_HOACH_VW = new Date($scope.thongTinKho.NGAY_KE_HOACH.replace(/(\d{4})(\d{2})(\d{2})/g, '$1-$2-$3'));
                    }
                    if ($scope.thongTinKho.NGAY_CT.length === 8) {
                        $scope.thongTinKho.NGAY_CT_VW = moment($scope.thongTinKho.NGAY_CT, 'YYYYMMDD').format('DD/MM/YYYY')
                    }
                    $scope.ThongTinHangHoa($scope.thongTinKho.ID_TXN);
                    $('#btnEdit').prop('disabled', false);
                    $('#btnThemHangHoa').prop('disabled', false);
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

    $scope.ThongTinHangHoa = (id) => {
        $.ajax({
            type: 'post',
            url: '/PhieuXuatKho/GetDsHangHoaByPhieu',
            data: {
                phieuNKId: id
            },
            success: function (response) {
                if (response.Status === 200) {
                    $scope.dsXuatKho = response.Data;
                    //$scope.dsXuatKho.HANG_HOA = $scope.hanghoa.find(x => x.ID === $scope.dsXuatKho.ID_HANG)
                    $scope.dsXuatKho.forEach(mathang => { mathang.HANG_HOA = $scope.hanghoa.find(x => x.ID === mathang.ID_HANG), mathang.DVT_VW = $scope.donvitinh.find(x => x.NAME === mathang.DVT) });
                    //$scope.dsXuatKho.DVT = $scope.donvitinh.find(x => x.NAME === $scope.dsXuatKho.DVT)
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
    $scope.ThemPhieu = () => {
        //$('input[type="text"], input[type="date"], select').removeAttr('disabled');
        //$('#btnXoaHangHoa').prop('disabled', false);
        //$('#btnThemHangHoa').prop('disabled', false);
        DefaultSettings();
        $('#btnThemHangHoa').prop('disabled', false);
        $('#btnSave').prop('disabled', false);
    }
    $scope.ThemHangHoa = () => {
        $scope.dsXuatKho.push({ 'STT': $scope.dsXuatKho.length + 1 });
    }
    $scope.XoaHangHoa = (index) => {
        if ($scope.dsXuatKho[index].HANG_HOA != null || $scope.dsXuatKho[index].DVT_VW != null || $scope.dsXuatKho[index].DVT_VW != null || $scope.dsXuatKho[index].SO_LUONG_KH != null || $scope.dsXuatKho[index].DON_GIA != null || $scope.dsXuatKho[index].GHI_CHU != null) {
            if (confirm('Bạn có chắc muốn xóa hàng hóa này?')) {
                $scope.dsXuatKho.splice(index, 1);
                $scope.dsXuatKho.forEach((x, index) => x.STT = index + 1);
            } else {
                // Do nothing!
                console.log('Không xóa gì hết');
            }
        }
        else {
            $scope.dsXuatKho.splice(index, 1);
            $scope.dsXuatKho.forEach((x, index) => x.STT = index + 1);
        }         }
    $scope.ValueCount = (item, index) => {
        if (typeof item.SO_LUONG_KH == 'number' && typeof item.DON_GIA == 'number') {
            $scope.dsXuatKho[index].THANH_TIEN_KH = item.SO_LUONG_KH * item.DON_GIA;
        };
    }
    $scope.TabThemHangHoa = (e) => {
        if (e.key == "ArrowDown") {
            e.preventDefault();
            $scope.ThemHangHoa();
        }
        if (e.key == "ArrowUp") {
            e.preventDefault();
            $scope.XoaHangHoa($scope.dsXuatKho.length - 1);
        }
    }
    GenerateRandomNumber = () => {
        var newDate = new Date();
        var soPhieu = 'X' + moment(newDate).format('YYYYMMDDHHmm');
        return soPhieu;
    }
})