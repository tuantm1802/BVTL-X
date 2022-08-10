app.controller("BienBanKiemKeController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, chuyenGiaSangChu) {
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
    $scope.pageSize = 10;
    $scope.currentPage = 1;
    DefaultSettings = () => {
        $scope.checkAll = false;
        $scope.danhsachanh = [];
        $scope.dsKeHoach = [];
        $scope.dsHangHoa = [];
        $scope.dsCanBo = [];
        //$scope.dsCanBo = [{ 'STT': 1 }, { 'STT': 2 }, { 'STT': 3 }];
        $scope.thongTinKho = {};
        $scope.thongTinKho.NGAY_KIEM_KE_VW = moment(new Date()).format('DD/MM/YYYY');
        $('#btnAdd').prop('disabled', false);
        $('#btnEdit').prop('disabled', true);
        $('#btnSave').prop('disabled', true);
        $('#btnPrint').prop('disabled', true);
        $('#btnThemHangHoa').prop('disabled', true);
        $('#btnThemCanBo').prop('disabled', true); $scope.CoDinhKem1 = false;
        $scope.CoDinhKem2 = false;
        $scope.CoDinhKem3 = false;
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
        $scope.soLuongKetQua = $scope.dsKeHoach.length;
        $scope.selectedRow = 0;
        $.ajax({
            type: 'post',
            url: '/BienBanKiemKe/TimKiemPhieu',
            data: {
                khoId: $scope.hangDoi.Kho === null ? null : $scope.hangDoi.Kho.ID,
                trangThai: null,
                tuNgay: moment($scope.hangDoi.TuNgay, 'DD/MM/YYYY').format('YYYYMMDD'),
                denNgay: moment($scope.hangDoi.DenNgay, 'DD/MM/YYYY').add(1, 'days').format('YYYYMMDD'),
                pageSize: $scope.pageSize,
                pageNumber: $scope.currentPage
            },
            success: function (response) {
                if (response.Status === 200) {
                    $scope.dsKetQua = response.Data;
                    $scope.dsKetQua.forEach(x => x.NGAY_KIEM_KE_VW = moment(x.NGAY_KIEM_KE, 'YYYYMMDD').format('DD/MM/YYYY'));
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
                $scope.$apply();
            }
        });
        hideLoading();
    };

    $scope.LuuPhieu = () => {
        showToast();
        $scope.thongTinKho.NGAY_KIEM_KE = moment($scope.thongTinKho.NGAY_KIEM_KE_VW, 'DD/MM/YYYY').format('YYYYMMDD');
        $scope.thongTinKho.NGUOI_VIET_PHIEU = $scope.thongTinKho.NGUOI_VIET_PHIEU_VW.ID;      
        $scope.thongTinKho.ID_KHO = $scope.thongTinKho.KHO.ID;

        if ($scope.dsHangHoa.length === 0) {
            toastr.error('Bạn chưa thêm hàng hóa nào!');
        } else {
            $scope.dsHangHoa.forEach((x) => { x.ID_HANG = x.HANG_HOA.ID; x.MA_HANG = x.HANG_HOA.MaHang; x.TEN_HANG = x.HANG_HOA.TenHang, x.DVT = x.DVT_VW.NAME });
            $scope.dsCanBo.forEach((x) => { x.ID_TV = x.CAN_BO.ID});

            if ($scope.thongTinKho.ID_BD != null) {
                $.ajax({
                    type: 'post',
                    url: '/BienBanKiemKe/UpdateBBKiemKe',
                    data: {
                        thongTinPhieu: $scope.thongTinKho,
                        dsHangHoaChiTiet: $scope.dsHangHoa,
                        dsThanhVienChiTiet: $scope.dsCanBo,
                        dsDinhKem: $scope.danhsachanh
                    },
                    success: function (response) {
                        if (response.Status === 200) {
                            toastr.success('Cập nhật thông tin biên bản thành công!');
                        }
                        else {
                            toastr.error(response.Message);
                        }
                        console.log(response);

                        $scope.$apply();
                    }
                });
            } else {
                $.ajax({
                    type: 'post',
                    url: '/BienBanKiemKe/InsertBBKiemKe',
                    data: {
                        thongTinPhieu: $scope.thongTinKho,
                        dsHangHoaChiTiet: $scope.dsHangHoa,
                        dsThanhVienChiTiet: $scope.dsCanBo,
                        dsDinhKem: $scope.danhsachanh
                    },
                    success: function (response) {
                        if (response.Status === 200) {
                            toastr.success('Lưu thông tin biên bản thành công!');
                            $scope.thongTinKho.ID_BD = response.Data
                        }
                        else {
                            toastr.error(response.Message);
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
        $scope.thongTinKho = item;
        $scope.thongTinKho.KHO = $scope.danhmuckho.find(x => x.ID === item.ID_KHO);
        if ($scope.thongTinKho.NGAY_KIEM_KE.length === 8) {
            $scope.thongTinKho.NGAY_KIEM_KE_VW = moment($scope.thongTinKho.NGAY_KIEM_KE, 'YYYYMMDD').format('DD/MM/YYYY');
        }
        $scope.thongTinKho.NGUOI_VIET_PHIEU_VW = $scope.canbo.find(x => x.ID === item.NGUOI_VIET_PHIEU);
        $scope.thongTinKho = item;
        $scope.dsHangHoa = [];
        $scope.dsCanBo = [];
        $scope.selectedRow = indexRow;
      
        $.ajax({
            type: 'post',
            url: '/BienBanKiemKe/GetDsByPhieu',
            data: {
                phieuId: item.ID_BD
            },
            success: function (response) {
                console.log(response);
                if (response.Status === 200) {

                    $scope.dsHangHoa = response.DsHangHoa;
                    $scope.dsHangHoa.forEach(mathang => { mathang.HANG_HOA = $scope.hanghoa.find(x => x.ID === mathang.ID_HANG), mathang.DVT_VW = $scope.donvitinh.find(x => x.NAME === mathang.DVT) });
                    $scope.dsCanBo = response.DsCanBo;
                    $scope.dsCanBo.forEach(cb => { cb.CAN_BO = $scope.canbo.find(x => x.ID === cb.ID_TV) });
                }
                else {
                    toastr.error(response.Message);
                }
                    $('#btnAdd').prop('disabled', false);
                    $('#btnEdit').prop('disabled', false);
                    $('#btnSave').prop('disabled', false);
                    $('#btnThemHangHoa').prop('disabled', false);

                $scope.$apply();
            }
        });
        ThongTinDinhKem($scope.thongTinKho.ID_TXN);
    }
    ThongTinDinhKem = (id) => {
        $scope.danhsachanh = [];
        $.ajax({
            type: 'post',
            url: '/DinhKem/GetAnhDinhKem',
            data: {
                idPhieu: id,
                loaiPhieu: 'KK'
            },
            success: function (response) {
                if (response.Status === 200) {
                    var dsLink = response.Data;
                    dsLink.forEach(x => {
                        $scope.danhsachanh.push({
                            'src': `https://localhost:44374/DinhKem/Image?imgPath=${x.IMG_PATH}`,
                            'checked': false,
                            'IMG_PATH': x.IMG_PATH,
                            'ID_TAB': x.ID_TAB,
                            'ID_TXN_REF': x.ID_TXN_REF,
                            'LOAI_PHIEU': x.LOAI_PHIEU,
                            'TRANG_THAI': x.TRANG_THAI
                        })
                    });
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
    $scope.XoaAnhDinhKem = () => {
        var a = $scope.danhsachanh.filter(x => x.checked === true);
        if (a.length > 0) {
            try {
                $.ajax({
                    type: 'post',
                    url: '/DinhKem/DeleteMultipleImagesFTP',
                    data: {
                        lstImgPath: $scope.danhsachanh.filter(x => x.checked === true).map(a => a.IMG_PATH)
                    },
                    success: function (response) {
                        if (response.Status === 200) {
                            $scope.danhsachanh = $scope.danhsachanh.filter(x => x.checked !== true);
                            if ($scope.danhsachanh.length < 1) {
                                $('#kho_anh').modal('hide');
                            }
                        }
                        else {
                            toastr.error('Có lỗi xảy ra trong quá trình xóa ảnh!');
                        }
                        console.log(response);
                        $scope.$apply();
                    }, error: (e) => {
                        toastr.error('Có lỗi xảy ra trong quá trình xóa ảnh! ');
                        console.error(e);
                    }
                });
            } catch (e) {
                console.error(e);
                toastr.error('Có lỗi xảy ra trong quá trình xóa ảnh! ');
            }
        }
        else {
            toastr.error('Bạn chưa chọn ảnh nào cần xóa!')
        }
    }

    $scope.ChonCanBo = (index) => {
        $scope.dsCanBo[index].CV_TV = $scope.dsCanBo[index].CAN_BO.ChucVu;
        $scope.dsCanBo[index].CB_TV = $scope.dsCanBo[index].CAN_BO.CapBac;
        $scope.dsCanBo[index].TEN_TV = $scope.dsCanBo[index].CAN_BO.HoTen;
    }
    $scope.ThemPhieu = () => {
        DefaultSettings();
        $('input[type="text"], input[type="date"], select').removeAttr('disabled');
        $('#btnSave').prop('disabled', false);
        $('#btnPrint').prop('disabled', false);
        $('#btnXoaHangHoa').prop('disabled', false);
        $('#btnThemHangHoa').prop('disabled', false);
        $('#btnXoaCanBo').prop('disabled', false);
        $('#btnThemCanBo').prop('disabled', false);
    }
    $scope.ThemHangHoa = () => {
        $scope.dsHangHoa.push({ 'STT': $scope.dsHangHoa.length + 1 });
    }
    $scope.ThemCanBo = () => {
        $scope.dsCanBo.push({ 'STT': $scope.dsCanBo.length + 1 });
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
        }
    }
    $scope.XoaCanBo = (index) => {
        if ($scope.dsCanBo[index].CAN_BO != null || $scope.dsHangHoa[index].CB_TV != null || $scope.dsHangHoa[index].CV_TV != null) {
            if (confirm('Bạn có chắc muốn xóa hàng hóa này?')) {
                $scope.dsCanBo.splice(index, 1);
                $scope.dsCanBo.forEach((x, index) => x.STT = index + 1);
            } else {
                // Do nothing!
                console.log('Không xóa gì hết');
            }
        }
        else {
            $scope.dsCanBo.splice(index, 1);
            $scope.dsCanBo.forEach((x, index) => x.STT = index + 1);
        }
    }
    $scope.ValueCount = (item, index) => {
        //if (typeof item.SO_LUONG_KH == 'number' && typeof item.SO_LUONG_TT == 'number') {
        //    $scope.dsHangHoa[index].SO_LUONG_LECH = item.SO_LUONG_KH - item.SO_LUONG_TT;
        //};
        //    if (typeof item.SO_LUONG_KH == 'number' && typeof item.DON_GIA == 'number') {
        //        $scope.dsHangHoa[index].THANH_TIEN_KH = item.SO_LUONG_KH * item.DON_GIA;
        //    };
        //    if (typeof item.SO_LUONG_TT == 'number' && typeof item.DON_GIA == 'number') {
        //        $scope.dsHangHoa[index].THANH_TIEN_TT = item.SO_LUONG_TT * item.DON_GIA;
        //    };
        //    if (typeof item.SO_LUONG_LECH == 'number' && typeof item.DON_GIA == 'number') {
        //        $scope.dsHangHoa[index].THANH_TIEN_LECH = item.SO_LUONG_LECH * item.DON_GIA;
        //    };
            $scope.dsHangHoa[index].SO_LUONG_LECH = item.SO_LUONG_KH - item.SO_LUONG_TT;
        
            $scope.dsHangHoa[index].THANH_TIEN_KH = item.SO_LUONG_KH * item.DON_GIA;
        
            $scope.dsHangHoa[index].THANH_TIEN_TT = item.SO_LUONG_TT * item.DON_GIA;
        
            $scope.dsHangHoa[index].THANH_TIEN_LECH = item.SO_LUONG_LECH * item.DON_GIA;
        
    }
    $scope.ImageUpload = () => {
        $scope.stepsModel = [];
        var fileList = document.getElementById('file').files;
        if (fileList.length > 3) {
            toastr.error('Không được chọn quá 3 file!');
            return;
        }
        if (fileList.length > 0 && fileList.length < 4) {
            $scope.CoDinhKem1 = false;
            $scope.CoDinhKem2 = false;
            $scope.CoDinhKem3 = false;
            $scope.thongTinKho.IMAGE1 = $scope.thongTinKho.IMAGE2 = $scope.thongTinKho.IMAGE3 = null;
            for (let i = 0; i < fileList.length; i++) {
                //var file = document.getElementById('file').files[i];
                var file = fileList[i];
                var freader = new FileReader();
                freader.onload = $scope.imageIsLoaded;
                freader.readAsDataURL(file);
            }
        }

    }
    $scope.imageIsLoaded = function (e) {
        $scope.$apply(function () {
            $scope.danhsachanh.push({
                'src': e.target.result,
                'checked': false,
                'ImageString': e.target.result,
                'IMG_PATH': null,
                'ID_TAB': 0,
                'ID_TXN_REF': $scope.thongTinKho.ID_TXN,
                'LOAI_PHIEU': 'KK',
                'TRANG_THAI': 'O'
            });
        });
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
        var soPhieu = 'KK' + moment(newDate).format('YYYYMMDDHHmm');
        return soPhieu;
    }
})