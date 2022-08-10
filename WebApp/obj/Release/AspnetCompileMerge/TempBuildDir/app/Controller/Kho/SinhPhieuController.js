app.controller("SinhPhieuController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {

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
    ]

    $scope.danhmuckho = [
        { 'ID': 1, 'NAME': 'Kho Hà Đông' },
        { 'ID': 2, 'NAME': 'Kho Đại Mỗ' },
        { 'ID': 3, 'NAME': 'Kho Duy Tân' },
        { 'ID': 4, 'NAME': 'Kho Phan Bội Châu' },
    ]

    $scope.nhomhang = [
        { 'ID': 1, 'NAME': 'Phương tiện đường bộ' },
        { 'ID': 2, 'NAME': 'Phương tiện đường thủy' },
        { 'ID': 3, 'NAME': 'Vật tư, thiết bị kỹ thuật nghiệp vụ' },
        { 'ID': 4, 'NAME': 'Vũ khí' },
        { 'ID': 5, 'NAME': 'Quân trang' },
    ]
    $scope.hanghoa = [
        { 'ID': 1, 'TenHang': 'Áo lót nam', 'MaHang': '01' },
        { 'ID': 2, 'TenHang': 'Áo lót nữ', 'MaHang': '02' },
        { 'ID': 3, 'TenHang': 'Quần xịp nữ', 'MaHang': '03' },
        { 'ID': 3, 'TenHang': 'Dầu ăn', 'MaHang': '04' },
        { 'ID': 3, 'TenHang': 'Đạn 6.2mm', 'MaHang': '05' },
    ]
    $scope.trangthai = [
        { 'MA': 'U', 'NAME': 'Chở nhập' },
        { 'MA': 'A', 'NAME': 'Hoàn thành' },
        { 'MA': 'D', 'NAME': 'Hủy phiếu' }
    ];
    $scope.donvi = [
        { 'ID': 1, 'NAME': 'CA Thái Bình' },
        { 'ID': 2, 'NAME': 'CA Hà Nội' },
        { 'ID': 3, 'NAME': 'CA Bắc Ninh' },
        { 'ID': 3, 'NAME': 'CA TP Hồ Chí Minh' },
        { 'ID': 3, 'NAME': 'CA Đà Nẵng' },
    ]
    $scope.danhsachkehoach = [
        { 'ID_KE_HOACH':3,'SOKEHOACH': 'AXN1234', 'NGAYLAPKH': '08/04/2021', 'CANCU': 'QD1234', 'NGUOILAPKH': 'Nguyễn Tùng Dương', 'NGUONTIEPNHAN': 'Cục trang bị', 'NGAYXUAT': '09/09/2021', 'VANCHUYEN': 'Đơn vị tự lên lấy hàng','LOAIKEHOACH':'X','LYDOXUAT':'Lý do xuất' },
        { 'ID_KE_HOACH': 4, 'SOKEHOACH': 'AXN1235', 'NGAYLAPKH': '08/04/2021', 'CANCU': 'QD1234', 'NGUOILAPKH': 'Nguyễn Tùng Dương', 'NGUONTIEPNHAN': 'Cục trang bị', 'NGAYXUAT': '09/09/2021', 'VANCHUYEN': 'Đơn vị tự lên lấy hàng', 'LOAIKEHOACH': 'N', 'LYDOXUAT': 'Lý do xuất' }

    ]
    $scope.danhsachphieu = [
        {
            'CB_NGUOI_KY_2': 'Hạ Sĩ',
            'CB_NGUOI_KY_3': 'Hạ Sĩ',
            'CB_NGUOI_KY_1': 'Hạ Sĩ',
            'CB_NGUOI_KY_4': 'Hạ Sĩ',
            'CB_NGUOI_KY_5': 'Hạ Sĩ',
            'CV_NGUOI_KY_1': 'Bảo vệ',
            'CV_NGUOI_KY_2': 'Lao công',
            'CV_NGUOI_KY_3': 'Lao công',
            'CV_NGUOI_KY_4': 'Tạp vụ',
            'CV_NGUOI_KY_5': 'Tạp vụ',
            'ID_DON_VI': 1,
            'ID_DON_VI_NHAN': 2,
            'ID_HOP_DONG': null,
            'ID_KE_HOACH': 3,
            'ID_KHO': 2,
            'ID_NGUOI_CAP_NHAT': 2,
            'ID_NGUOI_GIAO': 3,
            'ID_NGUOI_KY_1': 3,
            'ID_NGUOI_KY_2': 1,
            'ID_NGUOI_KY_3': 1,
            'ID_NGUOI_KY_4': 2,
            'ID_NGUOI_KY_5': 2,
            'ID_NGUOI_NHAN': 2,
            'ID_NGUOI_TAO': 1,
            'ID_NHA_CC': null,
            'ID_TXN': 3,
            'IMG_PATH_1': null,
            'IMG_PATH_2': null,
            'IMG_PATH_3': null,
            'LOAI_NK': null,
            'LY_DO_NHAP': 'F',
            'NGAY_CT': '20210301',
            'NGAY_HH_HD': '20210301',
            'NGAY_KY_HD': '20210305',
            'NGAY_NK': '20210303',
            'NGUOI_PHU_TRACH': 3,
            'NGUON_HANG': null,
            'NGUON_KINH_PHI': null,
            'NHAP_TRA_DOI_CO': 'N',
            'SO_HOP_DONG': '1',
            'SO_KE_HOACH': 'AXN1234',
            'SO_PHIEU': null,
            'TEN_DON_VI': null,
            'TEN_KHO': 'Kho Hà Đông',
            'TEN_NGUOI_GIAO': 'Lê Quang Dũng',
            'TEN_NGUOI_KY_1': 'Lê Quang Dũng',
            'TEN_NGUOI_KY_2': 'Nguyễn Tùng Dương',
            'TEN_NGUOI_KY_3': 'Nguyễn Tùng Dương',
            'TEN_NGUOI_KY_4': 'Thái Minh Đức',
            'TEN_NGUOI_KY_5': 'Thái Minh Đức',
            'TEN_NGUOI_NHAN': 'Thái Minh Đức',
            'TEN_NHA_CC': null,
            'TONG_TIEN': 20000,
            'TONG_TIEN_CHU': 'Hai  mươi nghìn',
            'TRANG_THAI': 'U'
        },
        {
            'CB_NGUOI_KY_1': 'Hạ Sĩ',
            'CB_NGUOI_KY_2': 'Hạ Sĩ',
            'CB_NGUOI_KY_3': 'Hạ Sĩ',
            'CB_NGUOI_KY_4': 'Hạ Sĩ',
            'CB_NGUOI_KY_5': 'Hạ Sĩ',
            'CV_NGUOI_KY_1': 'Bảo vệ',
            'CV_NGUOI_KY_2': 'Lao công',
            'CV_NGUOI_KY_3': 'Lao công',
            'CV_NGUOI_KY_4': 'Tạp vụ',
            'CV_NGUOI_KY_5': 'Tạp vụ',
            'ID_DON_VI': 1,
            'ID_DON_VI_NHAN': 2,
            'ID_HOP_DONG': null,
            'ID_KE_HOACH': 3,
            'ID_KHO': 1,
            'ID_NGUOI_CAP_NHAT': 2,
            'ID_NGUOI_GIAO': 3,
            'ID_NGUOI_KY_1': 3,
            'ID_NGUOI_KY_2': 1,
            'ID_NGUOI_KY_3': 1,
            'ID_NGUOI_KY_4': 2,
            'ID_NGUOI_KY_5': 2,
            'ID_NGUOI_NHAN': 2,
            'ID_NGUOI_TAO': 1,
            'ID_NHA_CC': null,
            'ID_TXN': 3,
            'IMG_PATH_1': null,
            'IMG_PATH_2': null,
            'IMG_PATH_3': null,
            'LOAI_NK': null,
            'LY_DO_NHAP': 'F',
            'NGAY_CT': '20210301',
            'NGAY_HH_HD': '20210301',
            'NGAY_KY_HD': '20210305',
            'NGAY_NK': '20210303',
            'NGUOI_PHU_TRACH': 3,
            'NGUON_HANG': null,
            'NGUON_KINH_PHI': null,
            'NHAP_TRA_DOI_CO': 'N',
            'SO_HOP_DONG': '1',
            'SO_KE_HOACH': 'AXN1234',
            'SO_PHIEU': null,
            'TEN_DON_VI': null,
            'TEN_KHO': 'Kho Hà Đông',
            'TEN_NGUOI_GIAO': 'Lê Quang Dũng',
            'TEN_NGUOI_KY_1': 'Lê Quang Dũng',
            'TEN_NGUOI_KY_2': 'Nguyễn Tùng Dương',
            'TEN_NGUOI_KY_3': 'Nguyễn Tùng Dương',
            'TEN_NGUOI_KY_4': 'Thái Minh Đức',
            'TEN_NGUOI_KY_5': 'Thái Minh Đức',
            'TEN_NGUOI_NHAN': 'Thái Minh Đức',
            'TEN_NHA_CC': null,
            'TONG_TIEN': 200000,
            'TONG_TIEN_CHU': 'Hai  trăm nghìn',
            'TRANG_THAI': 'U'
        }       
    ];

    $scope.danhsachhanghoa = [
        {
            'SO_PHIEU': null,
            'ID_KHO': 2,
            'KHO': 'Đại Mỗ',
            'DON_GIA': 10000,
            'DVT': 'Chiếc',
            'GHI_CHU': 'pHIẾU 1',
            'ID_HANG': 1,
            'ID_NK_CT': 5,
            'ID_TXN': 3,
            'MA_HANG': '01',
            'SO_LUONG_KH': 1,
            'SO_LUONG_TT': 1,
            'STT': 1,
            'TEN_HANG': 'Áo lót nam',
            'NAM_SX': '1999',
            'CHAT_LUONG':'Tốt',
            'THANH_TIEN_KH': 10000,
            'THANH_TIEN_TT': 10000,
        }, {
            'SO_PHIEU': null,
            'ID_KHO': 2,
            'KHO': 'Đại Mỗ',
            'DON_GIA': 10000,
            'DVT': 'Chiếc',
            'GHI_CHU': 'pHIẾU 1',
            'ID_HANG': 2,
            'ID_NK_CT': 5,
            'ID_TXN': 3,
            'MA_HANG': '01',
            'SO_LUONG_KH': 1,
            'SO_LUONG_TT': 1,
            'STT': 2,
            'TEN_HANG': 'Áo lót nữ',
            'NAM_SX': '1999',
            'CHAT_LUONG': 'Tốt',
            'THANH_TIEN_KH': 10000,
            'THANH_TIEN_TT': 10000,
        },
        {
            'SO_PHIEU': null,
            'ID_KHO': 1,
           'KHO': 'Hà Đông',
           'DON_GIA': 10000,
           'DVT': 'Chiếc',
           'GHI_CHU': 'pHIẾU 2',
           'ID_HANG': 1,
           'ID_NK_CT': 5,
           'ID_TXN': 3,
           'MA_HANG': '01',
           'SO_LUONG_KH': 1,
           'SO_LUONG_TT': 1,
           'STT': 1,
            'TEN_HANG': 'Áo lót nam',
            'NAM_SX': '1999',
            'CHAT_LUONG': 'Tốt',
           'THANH_TIEN_KH': 10000,
           'THANH_TIEN_TT': 10000,
        }, {
            'SO_PHIEU': null,
            'ID_KHO': 1,
            'KHO': 'Hà Đông',
            'DON_GIA': 10000,
            'DVT': 'Chiếc',
            'GHI_CHU': 'pHIẾU 2',
            'ID_HANG': 2,
            'ID_NK_CT': 5,
            'ID_TXN': 3,
            'MA_HANG': '01',
            'SO_LUONG_KH': 1,
            'SO_LUONG_TT': 1,
            'STT': 2,
            'TEN_HANG': 'Áo lót nữ',
            'NAM_SX': '1999',
            'CHAT_LUONG': 'Tốt',
            'THANH_TIEN_KH': 10000,
            'THANH_TIEN_TT': 10000,
        },
    ];


    $scope.dinhkem = null;
    $scope.dsKetQua = [];
    $scope.thongTinKehoach = [];
    $scope.thongTinKho = {};
    $scope.pageSize = 10;
    $scope.currentPage = 1;
    $scope.SoLuongKetQua = 0;

    DefaultSettings = () => {
        $scope.stepsModel = [];
        $scope.lstB64 = [];
        $scope.dsKeHoach = [];
        $scope.dsHangHoa = [];
        $scope.thongTinKehoach = {};
        $scope.thongTinKho = {};
        $('#btnAdd').prop('disabled', true);
        $('#btnEdit').prop('disabled', true);
        $('#btnSave').prop('disabled', true);
        $('#btnConfirm').prop('disabled', true);
        $('#btnRemove').prop('disabled', true);
        $('#btnPrint').prop('disabled', true);
    }
    var date = new Date();
    date.setDate(date.getDate() - 7);
    $scope.hangDoi = {
        'TuNgay': moment(date).format('DD/MM/YYYY'), 'DenNgay': moment(new Date()).format('DD/MM/YYYY'), Kho: null, DonVi: null
    }
    DefaultSettings();
    $scope.ngayLapKh = moment(new Date()).format('DD/MM/YYYY');

    $scope.TimKiem = (num) => {
        DefaultSettings();
        $scope.dsKetQua = $scope.danhsachkehoach;
        $scope.selectedRow = 0;
        $scope.ShowThongTin($scope.dsKetQua[0], 0);
        //$.ajax({
        //    type: 'post',
        //    url: '/DieuChuyenKho/TimKiemPhieu',
        //    data: {
        //        tuNgay: moment($scope.hangDoi.TuNgay, 'DD/MM/YYYY').format('YYYYMMDD'),
        //        denNgay: moment($scope.hangDoi.DenNgay, 'DD/MM/YYYY').add(1, 'days').format('YYYYMMDD'),
        //        khoId: $scope.hangDoi.Kho === null ? null : $scope.hangDoi.Kho.ID,
        //        trangThai: null,
        //        donViId: $scope.hangDoi.DonVi === null ? null : $scope.hangDoi.DonVi.ID,
        //        pageSize: 10,
        //        pageNumber: $scope.currentPage
        //    },
        //    success: function (response) {
        //        if (response.Status === 200) {
        //            $scope.dsKetQua = response.Data;
        //            $scope.SoLuongKetQua = response.Quantity;
        //            $scope.dsKetQua.forEach(x => x.NGAY_TAO = moment(new Date(Number(x.NGAY_TAO.replace(/\D/g, '')))).format('DD/MM/YYYY'));
        //            $scope.dsKetQua.forEach(x => {
        //                if (x.TRANG_THAI === 'U') {
        //                    x.TRANG_THAI = 'Chờ nhập'
        //                }
        //                if (x.TRANG_THAI === 'A') {
        //                    x.TRANG_THAI = 'Đã nhập'
        //                }
        //                if (x.TRANG_THAI === 'D') {
        //                    x.TRANG_THAI = 'Đã xóa'
        //                }
        //            });
        //            console.log(response.Data);
        //            $scope.selectedRow = 0;
        //            $scope.ShowThongTin($scope.dsKetQua[0], 0);
        //        }
        //        else if (response.Status === 400) {
        //            toastr.error('Không tìm thấy kết quả nào!');
        //        }
        //        else {
        //            toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm!');
        //        }
        //        console.log(response);

        //        $scope.$apply();
        //    }, error: (e) => {
        //        toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm!');
        //    },
        //    complete: () => {
        //        hideLoading();
        //    }
        //});
    }
    $scope.ShowThongTin = (item, index) => {
        showToast();
        $scope.selectedRow = index;
        $scope.thongTinKehoach = item;
        $scope.thongtinHangHoa = $scope.danhsachhanghoa;
        //$scope.thongtinHangHoa.forEach(mathang => { mathang.HANG_HOA = $scope.hanghoa.find(x => x.ID === mathang.ID_HANG), mathang.DVT_VW = $scope.donvitinh.find(x => x.NAME === mathang.DVT) });
        hideLoading();
    }
    $scope.TaoPhieu = () => {
        var urlapi = 'N';// sua o day, dang de mac dinh nhap de test
        if ($scope.thongTinKehoach.LOAIKEHOACH === 'N') {
            urlapi = '/PhieuNhapKho/SinhPhieuNhapTuKeHoach';
            $scope.danhsachphieu.forEach((x,index) => x.SO_PHIEU = GenerateRandomNumber('N')+index);
            $scope.danhsachhanghoa.forEach(x => x.SO_PHIEU = $scope.danhsachphieu.find(y => y.ID_KHO === x.ID_KHO).SO_PHIEU);
        }
        if ($scope.thongTinKehoach.LOAIKEHOACH === 'X') {
            urlapi = '/PhieuXuatKho/SinhPhieuXuatTuKeHoach';
            $scope.danhsachphieu.forEach((x, index) => x.SO_PHIEU = GenerateRandomNumber('X') + index);
            $scope.danhsachhanghoa.forEach(x => x.SO_PHIEU = $scope.danhsachphieu.find(y => y.ID_KHO === x.ID_KHO).SO_PHIEU);
        }
        if ($scope.thongTinKehoach.LOAIKEHOACH === 'DC') {
            urlapi = '/PhieuDieuChuyenKho/SinhPhieuDcTuKeHoach';
            $scope.danhsachphieu.forEach((x, index) => x.SO_PHIEU = GenerateRandomNumber('DC') + index);
            $scope.danhsachhanghoa.forEach(x => x.SO_PHIEU = $scope.danhsachphieu.find(y => y.ID_KHO === x.ID_KHO).SO_PHIEU);
        }
        $.ajax({
            type: 'post',
            url: urlapi,
            data: {
                lstPhieu: $scope.danhsachphieu,
                lstHangHoa: $scope.danhsachhanghoa
            },
            success:(response)=> {
                console.log(response.Data);
                toastr.success('Tạo phiếu thành công! ');

            }, error:(err)=> {
                toastr.error('Có lỗi xảy ra trong quá trình tạo! ' + err);
            },
            complete: {

            }
        });
    };
    GenerateRandomNumber = (loai) => {
        var newDate = new Date();
        var soPhieu = loai + moment(newDate).format('YYYYMMDDHHmmss');
        return soPhieu;
    };

});