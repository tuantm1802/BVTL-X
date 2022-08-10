app.controller("DieuChuyenKhoController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
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
    $scope.dinhkem = null;
    $scope.dsKetQua = [];
    $scope.thongTinKho = {};
    $scope.pageSize = 10;
    $scope.currentPage = 1;
    $scope.SoLuongKetQua = 0;

    DefaultSettings = () => {
        $scope.danhsachanh = [];
        $scope.dsKeHoach = [];
        $scope.dsHangHoa = [];
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
        showToast();
        DefaultSettings();
        console.log($scope.hangDoi);
        $.ajax({
            type: 'post',
            url: '/DieuChuyenKho/TimKiemPhieu',
            data: {
                soPhieu: $scope.hangDoi.SoPhieu,
                soKeHoach: $scope.hangDoi.SoKeHoach,
                tuNgay: moment($scope.hangDoi.TuNgay, 'DD/MM/YYYY').format('YYYYMMDD'),
                denNgay: moment($scope.hangDoi.DenNgay, 'DD/MM/YYYY').add(1, 'days').format('YYYYMMDD'),
                khoId: $scope.hangDoi.Kho === null ? null : $scope.hangDoi.Kho.ID,
                trangThai: null,
                donViId: $scope.hangDoi.DonVi === null ? null : $scope.hangDoi.DonVi.ID,
                pageSize: 10,
                pageNumber: $scope.currentPage
            },
            success: function (response) {
                if (response.Status === 200) {
                    $scope.dsKetQua = response.Data;
                    $scope.SoLuongKetQua = response.Quantity;
                    $scope.dsKetQua.forEach(x => x.NGAY_TAO = moment(new Date(Number(x.NGAY_TAO.replace(/\D/g, '')))).format('DD/MM/YYYY'));
                    $scope.dsKetQua.forEach(x => {
                        if (x.TRANG_THAI === 'U') {
                            x.TRANG_THAI = 'Chờ nhập'
                        }
                        if (x.TRANG_THAI === 'A') {
                            x.TRANG_THAI = 'Đã nhập'
                        }
                        if (x.TRANG_THAI === 'D') {
                            x.TRANG_THAI = 'Đã xóa'
                        }
                    });
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
            url: '/PhieuDieuChuyenKho/GetDsHangHoaByPhieu',
            data: {
                phieuNKId: id
            },
            success: function (response) {
                if (response.Status === 200) {
                    $('#btnSave').prop('disabled', false);
                    $scope.dsHangHoa = response.Data;
                    //$scope.dsHangHoa.HANG_HOA = $scope.hanghoa.find(x => x.ID === $scope.dsHangHoa.ID_HANG)
                    $scope.dsHangHoa.forEach(mathang => { mathang.HANG_HOA = $scope.hanghoa.find(x => x.ID === mathang.ID_HANG), mathang.DVT_VW = $scope.donvitinh.find(x => x.NAME === mathang.DVT) });
                    //$scope.dsHangHoa.DVT = $scope.donvitinh.find(x => x.NAME === $scope.dsHangHoa.DVT)
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
        $scope.selectedRow = index;
        $scope.thongTinKho = {};
        $scope.thongTinKho = item;
        $scope.thongTinKho.NGUOI_PHU_TRACH_VW = $scope.canbo.find(x => x.ID === $scope.thongTinKho.NGUOI_PHU_TRACH);
        $scope.thongTinKho.NGUOI_KY_1 = $scope.canbo.find(x => x.ID === $scope.thongTinKho.ID_NGUOI_KY_1);
        $scope.thongTinKho.NGUOI_KY_2 = $scope.canbo.find(x => x.ID === $scope.thongTinKho.ID_NGUOI_KY_2);
        $scope.thongTinKho.NGUOI_KY_3 = $scope.canbo.find(x => x.ID === $scope.thongTinKho.ID_NGUOI_KY_3);
        $scope.thongTinKho.NGUOI_KY_4 = $scope.canbo.find(x => x.ID === $scope.thongTinKho.ID_NGUOI_KY_4);
        $scope.thongTinKho.NGUOI_GIAO = $scope.canbo.find(x => x.ID === item.ID_NGUOI_GIAO);
        $scope.thongTinKho.NGUOI_NHAN = $scope.canbo.find(x => x.ID === item.ID_NGUOI_NHAN);
        $scope.thongTinKho.KHO_XUAT = $scope.danhmuckho.find(x => x.ID === $scope.thongTinKho.ID_KHO_XUAT);
        $scope.thongTinKho.KHO_NHAN = $scope.danhmuckho.find(x => x.ID === $scope.thongTinKho.ID_KHO_NHAN);
        if ($scope.thongTinKho.NGAY_CT.length === 8) {
            $scope.thongTinKho.NGAY_CT_VW = moment($scope.thongTinKho.NGAY_CT, 'YYYYMMDD').format('DD/MM/YYYY')
        }
        if ($scope.thongTinKho.NGAY_CHUYEN != null) {
            if ($scope.thongTinKho.NGAY_CHUYEN.length === 8) {
                $scope.thongTinKho.NGAY_CHUYEN_VW = moment($scope.thongTinKho.NGAY_CHUYEN, 'YYYYMMDD').format('DD/MM/YYYY')
            }
        }
        if ($scope.thongTinKho.NGAY_KE_HOACH != null) {
            if ($scope.thongTinKho.NGAY_KE_HOACH.length === 8) {
                $scope.thongTinKho.NGAY_KE_HOACH_VW = moment($scope.thongTinKho.NGAY_KE_HOACH, 'YYYYMMDD').format('DD/MM/YYYY')
            }
        }
        if ($scope.thongTinKho.TRANG_THAI === 'U') {
            $scope.thongTinKho.TRANG_THAI = 'Chờ nhập'
        }
        if ($scope.thongTinKho.TRANG_THAI === 'A') {
            $scope.thongTinKho.TRANG_THAI = 'Đã nhập'
        }
        if ($scope.thongTinKho.TRANG_THAI === 'D') {
            $scope.thongTinKho.TRANG_THAI = 'Đã xóa'
        }
        ThongTinDinhKem($scope.thongTinKho.ID_TXN);
        ThongTinHangHoa($scope.thongTinKho.ID_TXN);
        hideLoading();
    }
    ThongTinDinhKem = (id) => {
        $scope.danhsachanh = [];
        $.ajax({
            type: 'post',
            url: '/DinhKem/GetAnhDinhKem',
            data: {
                idPhieu: id,
                loaiPhieu: 'DC'
            },
            success: function (response) {
                if (response.Status === 200) {
                    var dsLink = response.Data;
                    dsLink.forEach(x => {
                        $scope.danhsachanh.push({
                            'src': `/DinhKem/Image?imgPath=${x.IMG_PATH}`,
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
                    toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm file đính kèm!');
                }
                console.log(response);

                $scope.$apply();
            }, error: (e) => {
                toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm file đính kèm!');
            }
        });
    }
    $scope.XoaAnhDinhKem = () => {
        var a = $scope.danhsachanh.filter(x => x.checked === true);
        try {
            $.ajax({
                type: 'post',
                url: '/DinhKem/DeleteMultipleImagesFTP',
                data: {
                    lstImgPath: $scope.danhsachanh.filter(x => x.checked === true).map(a => a.path)
                },
                success: function (response) {
                    if (response.Status === 200) {
                        $scope.danhsachanh = $scope.danhsachanh.filter(x => x.checked !== true);

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
    CheckThongTin = () => {
        if ($scope.thongTinKho.NGAY_CHUYEN_VW == null || $scope.thongTinKho.NGUOI_KY_4 == null || $scope.thongTinKho.NGUOI_KY_3 == null || $scope.thongTinKho.NGUOI_GIAO == null || $scope.thongTinKho.NGUOI_NHAN == null) {
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
                $scope.thongTinKho.ID_NGUOI_KY_4 = $scope.thongTinKho.NGUOI_KY_4.ID;
                $scope.thongTinKho.TEN_NGUOI_KY_4 = $scope.thongTinKho.NGUOI_KY_4.HoTen;
                $scope.thongTinKho.CV_NGUOI_KY_4 = $scope.thongTinKho.NGUOI_KY_4.ChucVu;
                $scope.thongTinKho.CB_NGUOI_KY_4 = $scope.thongTinKho.NGUOI_KY_4.CapBac;
                $scope.thongTinKho.ID_NGUOI_KY_3 = $scope.thongTinKho.NGUOI_KY_3.ID;
                $scope.thongTinKho.TEN_NGUOI_KY_3 = $scope.thongTinKho.NGUOI_KY_3.HoTen;
                $scope.thongTinKho.CV_NGUOI_KY_3 = $scope.thongTinKho.NGUOI_KY_3.ChucVu;
                $scope.thongTinKho.CB_NGUOI_KY_3 = $scope.thongTinKho.NGUOI_KY_3.CapBac;
                $scope.thongTinKho.ID_NGUOI_GIAO = $scope.thongTinKho.NGUOI_GIAO.ID;
                $scope.thongTinKho.TEN_NGUOI_GIAO = $scope.thongTinKho.NGUOI_GIAO.HoTen;
                $scope.thongTinKho.ID_NGUOI_NHAN = $scope.thongTinKho.NGUOI_NHAN.ID;
                $scope.thongTinKho.TEN_NGUOI_NHAN = $scope.thongTinKho.NGUOI_NHAN.HoTen;
                $scope.thongTinKho.NGAY_CHUYEN = moment($scope.thongTinKho.NGAY_CHUYEN_VW, 'DD/MM/YYYY').format('YYYYMMDD');
                if ($scope.dsHangHoa.length === 0) {
                    toastr.error('Bạn chưa thêm hàng hóa nào!');
                } else {
                    $scope.dsHangHoa.forEach((x) => { x.ID_HANG = x.HANG_HOA.ID; x.MA_HANG = x.HANG_HOA.MaHang; x.TEN_HANG = x.HANG_HOA.TenHang, x.DVT = x.DVT_VW.NAME, x.ID_NK_CT = x.ID_DC_CT });
                    $.ajax({
                        type: 'post',
                        url: '/DieuChuyenKho/UpdatedDieuChuyenKho',
                        data: {
                            thongTinPhieu: $scope.thongTinKho,
                            dsHangHoaChiTiet: $scope.dsHangHoa,
                            dsDinhKem: $scope.danhsachanh
                        },
                        success: function (response) {
                            if (response.Status === 200) {
                                toastr.success('Lưu thông tin nhập kho thành công!');
                            }
                            else {
                                toastr.error('Có lỗi xảy ra trong quá trình lưu!');
                            }
                            console.log(response);

                            $scope.$apply();
                        }, error: (e) => {
                            toastr.error('Có lỗi xảy ra trong quá trình lưu! ');
                        }
                    });
                }
            }
        } catch (e) {
            toastr.error('Có lỗi xảy ra trong quá trình lưu! ');
        }
        finally {
            hideLoading();
        }
    }
    $scope.ChonTatCa = () => {
        $scope.danhsachanh.forEach(x => x.checked = !$scope.checkAll);
    }
    $scope.GetAnhPhieu1 = () => {
        if ($scope.thongTinKho.IMAGE1 == null && $scope.thongTinKho.IMG_PATH_1 != null) {
            $.ajax({
                type: 'post',
                url: '/DieuChuyenKho/GetImageFTP',
                data: {
                    Id: parseInt($scope.thongTinKho.ID_TXN),
                    thuTu: 1
                },
                success: function (response) {
                    if (response.Status === 200) {
                        $scope.thongTinKho.IMAGE1 = response.Data;
                        $('#hiddenImg1').attr('src', 'data:image/jpeg;base64,' + $scope.thongTinKho.IMAGE1);
                    }
                    else {
                        toastr.error('Có lỗi xảy ra trong quá trình lưu!');
                    }
                    console.log(response);
                    $scope.$apply();
                }, error: (e) => {
                    toastr.error('Có lỗi xảy ra trong quá trình lưu! ');
                },
                complete: () => {
                    hideLoading();
                }
            });
        }
        if ($scope.thongTinKho.IMAGE1 != null) {
            $('#hiddenImg1').attr('src', 'data:image/jpeg;base64,' + $scope.thongTinKho.IMAGE1);
        }
    }

    $scope.GetAnhPhieu2 = () => {
        if ($scope.thongTinKho.IMAGE2 == null && $scope.thongTinKho.IMG_PATH_2 != null) {
            $.ajax({
                type: 'post',
                url: '/DieuChuyenKho/GetImageFTP',
                data: {
                    Id: parseInt($scope.thongTinKho.ID_TXN),
                    thuTu: 2
                },
                success: function (response) {
                    if (response.Status === 200) {
                        $scope.thongTinKho.IMAGE2 = response.Data;
                        $('#hiddenImg2').attr('src', 'data:image/jpeg;base64,' + $scope.thongTinKho.IMAGE2);
                    }
                    else {
                        toastr.error('Có lỗi xảy ra trong quá trình lưu!');
                    }
                    console.log(response);
                    $scope.$apply();
                }, error: (e) => {
                    toastr.error('Có lỗi xảy ra trong quá trình lưu! ');
                },
                complete: () => {
                    hideLoading();
                }
            });
        }
        if ($scope.thongTinKho.IMAGE2 != null) {
            $('#hiddenImg2').attr('src', 'data: image / jpeg; base64,' + $scope.thongTinKho.IMAGE2);
        }
    }

    $scope.GetAnhPhieu3 = () => {
        if ($scope.thongTinKho.IMAGE3 == null && $scope.thongTinKho.IMG_PATH_3 != null) {
            $.ajax({
                type: 'post',
                url: '/DieuChuyenKho/GetImageFTP',
                data: {
                    Id: parseInt($scope.thongTinKho.ID_TXN),
                    thuTu: 3
                },
                success: function (response) {
                    if (response.Status === 200) {
                        $scope.thongTinKho.IMAGE3 = response.Data;
                        $('#hiddenImg3').attr('src', 'data:image/jpeg;base64,' + $scope.thongTinKho.IMAGE3);
                    }
                    else {
                        toastr.error('Có lỗi xảy ra trong quá trình lưu!');
                    }
                    console.log(response);
                    $scope.$apply();
                }, error: (e) => {
                    toastr.error('Có lỗi xảy ra trong quá trình lưu! ');
                },
                complete: () => {
                    hideLoading();
                }
            });
        }
        if ($scope.thongTinKho.IMAGE3 != null) {
            $('#hiddenImg3').attr('src', 'data:image/jpeg;base64,' + $scope.thongTinKho.IMAGE3);
        }
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
            $scope.lstB64 = [];
            $scope.danhsachanh.push({
                'src': e.target.result,
                'checked': false,
                'ImageString': e.target.result,
                'IMG_PATH': null,
                'ID_TAB': 0,
                'ID_TXN_REF': $scope.thongTinKho.ID_TXN,
                'LOAI_PHIEU': 'DC',
                'TRANG_THAI': 'O'
            });
        });
    }
    $scope.ThemPhieu = () => {
        $('input[type="text"], input[type="date"], select').removeAttr('disabled');
    }
    $scope.ValueCount = (item, index) => {
        if (typeof item.SO_LUONG_TT == 'number' && typeof item.DON_GIA == 'number') {
            $scope.dsHangHoa[index].THANH_TIEN_TT = item.SO_LUONG_TT * item.DON_GIA;
        };
    }

});

