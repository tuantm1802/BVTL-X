app.controller("PhieuDieuChuyenKhoController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, chuyenGiaSangChu) {
    $scope.dsKeHoach = [];
    $scope.dsHangHoa = [];
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
        $scope.dsHangHoa = [];
        $scope.thongTinKho = {};
        $('#btnAdd').prop('disabled', false);
        $('#btnEdit').prop('disabled', true);
        $('#btnSave').prop('disabled', true);
        $('#btnPrint').prop('disabled', true);
        $('#btnThemHangHoa').prop('disabled', true);
    }
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
        hideLoading();
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

    $scope.LuuPhieu = () => {
        showToast();
        var a = $scope.dsKeHoach.find(obj => obj.SoKeHoach === $scope.thongTinKho.SO_KE_HOACH);
        if (a !== undefined) {
            $scope.thongTinKho.ID_KE_HOACH = $scope.dsKeHoach.find(obj => obj.SoKeHoach === $scope.thongTinKho.SO_KE_HOACH).IDKeHoach;
        }
        $scope.thongTinKho.NGAY_CT = moment($scope.thongTinKho.NGAY_CT_VW, 'DD/MM/YYYY').format('YYYYMMDD');
        $scope.thongTinKho.NGAY_KE_HOACH = moment($scope.thongTinKho.NGAY_KE_HOACH_VW,'DD/MM/YYYY').format('YYYYMMDD');
        $scope.thongTinKho.TONG_TIEN = $scope.dsHangHoa.sum("THANH_TIEN_KH");
        $scope.thongTinKho.TONG_TIEN_CHU = chuyenGiaSangChu($scope.thongTinKho.TONG_TIEN);
        $scope.thongTinKho.ID_NGUOI_KY_1 = $scope.thongTinKho.NGUOI_KY_1.ID;
        $scope.thongTinKho.TEN_NGUOI_KY_1 = $scope.thongTinKho.NGUOI_KY_1.HoTen;
        $scope.thongTinKho.CV_NGUOI_KY_1 = $scope.thongTinKho.NGUOI_KY_1.ChucVu;
        $scope.thongTinKho.CB_NGUOI_KY_1 = $scope.thongTinKho.NGUOI_KY_1.CapBac;
        $scope.thongTinKho.ID_NGUOI_KY_2 = $scope.thongTinKho.NGUOI_KY_2.ID;
        $scope.thongTinKho.TEN_NGUOI_KY_2 = $scope.thongTinKho.NGUOI_KY_2.HoTen;
        $scope.thongTinKho.CV_NGUOI_KY_2 = $scope.thongTinKho.NGUOI_KY_2.ChucVu;
        $scope.thongTinKho.CB_NGUOI_KY_2 = $scope.thongTinKho.NGUOI_KY_2.CapBac;
        $scope.thongTinKho.ID_KHO_XUAT = $scope.thongTinKho.KHO_XUAT.ID;
        $scope.thongTinKho.TEN_KHO_XUAT = $scope.thongTinKho.KHO_XUAT.NAME;
        $scope.thongTinKho.ID_KHO_NHAN = $scope.thongTinKho.KHO_NHAN.ID;
        $scope.thongTinKho.TEN_KHO_NHAN = $scope.thongTinKho.KHO_NHAN.NAME;

        if ($scope.dsHangHoa.length === 0) {
            toastr.error('Bạn chưa thêm hàng hóa nào!');
        } else {
            $scope.dsHangHoa.forEach((x) => { x.ID_HANG = x.HANG_HOA.ID; x.MA_HANG = x.HANG_HOA.MaHang; x.TEN_HANG = x.HANG_HOA.TenHang, x.DVT = x.DVT_VW.NAME });
            if ($scope.thongTinKho.ID_TXN != null) {
                $.ajax({
                    type: 'post',
                    url: '/PhieuDieuChuyenKho/UpdatePhieuDieuChuyen',
                    data: {
                        thongTinPhieu: $scope.thongTinKho,
                        dsHangHoaChiTiet: $scope.dsHangHoa
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
                    }
                });
            } else {
                $.ajax({
                    type: 'post',
                    url: '/PhieuDieuChuyenKho/InsertPhieuDieuChuyen',
                    data: {
                        thongTinPhieu: $scope.thongTinKho,
                        dsHangHoaChiTiet: $scope.dsHangHoa
                    },
                    success: function (response) {
                        if (response.Status === 200) {
                            toastr.success('Thêm mới hàng hóa thành công!');
                        }
                        else {
                            toastr.success('Có lỗi xảy ra trong quá trình lưu!');
                        }
                        console.log(response);

                        $scope.$apply();
                    }
                });
            }

        }
        hideLoading();
    }

    $scope.ShowThongTin = (item, indexRow) => {
        $scope.thongTinKho = {};
        $scope.dsHangHoa = [];
        $scope.selectedRow = indexRow;
        $.ajax({
            type: 'post',
            url: '/PhieuDieuChuyenKho/GetPhieuBySoKeHoach',
            data: {
                soKeHoach: item.SoKeHoach
            },
            success: function (response) {
                console.log(response);
                if (response.Data == null) {
                    $scope.thongTinKho.SO_KE_HOACH = item.SoKeHoach;
                    $scope.thongTinKho.NGAY_KE_HOACH_VW = item.NgayLap;
                    $scope.thongTinKho.SO_PHIEU = GenerateRandomNumber();

                    $('#btnAdd').prop('disabled', false);
                    $('#btnEdit').prop('disabled', true);
                    $('#btnSave').prop('disabled', false);
                    $('#btnThemHangHoa').prop('disabled', false);
                } else {
                    $scope.thongTinKho = response.Data;
                    $scope.thongTinKho.NGUOI_KY_1 = $scope.canbo.find(x => x.ID === $scope.thongTinKho.ID_NGUOI_KY_1);
                    $scope.thongTinKho.NGUOI_KY_2 = $scope.canbo.find(x => x.ID === $scope.thongTinKho.ID_NGUOI_KY_2);
                    $scope.thongTinKho.KHO_XUAT = $scope.danhmuckho.find(x => x.ID === $scope.thongTinKho.ID_KHO_XUAT);
                    $scope.thongTinKho.KHO_NHAN = $scope.danhmuckho.find(x => x.ID === $scope.thongTinKho.ID_KHO_NHAN);
                    if ($scope.thongTinKho.NGAY_CT.length === 8) {
                        $scope.thongTinKho.NGAY_CT_VW = moment($scope.thongTinKho.NGAY_CT, 'YYYYMMDD').format('DD/MM/YYYY')
                    }
                    if ($scope.thongTinKho.NGAY_KE_HOACH.length === 8) {
                        $scope.thongTinKho.NGAY_KE_HOACH_VW = moment($scope.thongTinKho.NGAY_KE_HOACH, 'YYYYMMDD').format('DD/MM/YYYY')
                    }
                    $scope.ThongTinHangHoa($scope.thongTinKho.ID_TXN);
                    $('#btnAdd').prop('disabled', false);
                    $('#btnEdit').prop('disabled', false);
                    $('#btnSave').prop('disabled', false);
                    $('#btnThemHangHoa').prop('disabled', false);
                }

                $scope.$apply();
            }
        });
    }
    $scope.ThongTinHangHoa = (id) => {
        showToast();
        $.ajax({
            type: 'post',
            url: '/PhieuDieuChuyenKho/GetDsHangHoaByPhieu',
            data: {
                phieuNKId: id
            },
            success: function (response) {
                if (response.Status === 200) {
                    $scope.dsHangHoa = response.Data;
                    //$scope.dsHangHoa.HANG_HOA = $scope.hanghoa.find(x => x.ID === $scope.dsHangHoa.ID_HANG)
                    $scope.dsHangHoa.forEach(mathang => { mathang.HANG_HOA = $scope.hanghoa.find(x => x.ID === mathang.ID_HANG), mathang.DVT_VW = $scope.donvitinh.find(x => x.NAME === mathang.DVT) });
                    //$scope.dsHangHoa.DVT = $scope.donvitinh.find(x => x.NAME === $scope.dsHangHoa.DVT)
                }
                else {
                    toastr.success('Có lỗi xảy ra trong quá trình lưu!');
                }
                console.log(response);

                $scope.$apply();
            }
        });
        hideLoading();
    }
    $scope.ThemPhieu = () => {
        DefaultSettings();
        $('input[type="text"], input[type="date"], select').removeAttr('disabled');
        $('#btnSave').prop('disabled', false);
        $('#btnPrint').prop('disabled', false);
        $('#btnXoaHangHoa').prop('disabled', false);
        $('#btnThemHangHoa').prop('disabled', false);
    }
    $scope.ThemHangHoa = () => {
        $scope.dsHangHoa.push({ 'STT': $scope.dsHangHoa.length + 1 });
    }
    $scope.XoaHangHoa = (index) => {
        if ($scope.dsHangHoa[index].HANG_HOA != null || $scope.dsHangHoa[index].DVT_VW != null || $scope.dsHangHoa[index].DVT_VW != null || $scope.dsHangHoa[index].SO_LUONG_KH != null || $scope.dsHangHoa[index].DON_GIA != null || $scope.dsHangHoa[index].GHI_CHU != null) {
            if (confirm('Bạn có chắc muốn xóa hàng hóa này?')) {
                $scope.dsHangHoa.splice(index, 1);
                $scope.dsHangHoa.forEach((x, index) => x.STT = index + 1);
            } else {
                // Do nothing!
                console.log('Không xóa gì hết');
            }
        }
        else {
            $scope.dsHangHoa.splice(index, 1);
            $scope.dsHangHoa.forEach((x, index) => x.STT = index + 1);
        }    }
    $scope.ValueCount = (item, index) => {
        if (typeof item.SO_LUONG_KH == 'number' && typeof item.DON_GIA == 'number') {
            $scope.dsHangHoa[index].THANH_TIEN_KH = item.SO_LUONG_KH * item.DON_GIA;
        };
    }
    $scope.TabThemHangHoa = (e) => {
        if (e.key == "ArrowDown") {
            e.preventDefault();
            $scope.ThemHangHoa();
        }
        if (e.key == "ArrowUp") {
            e.preventDefault();
            $scope.XoaHangHoa($scope.dsHangHoa.length - 1);
        }
    }
    GenerateRandomNumber = () => {
        var newDate = new Date();
        var soPhieu = 'DC' + moment(newDate).format('YYYYMMDDHHmm');
        return soPhieu;
    }
})