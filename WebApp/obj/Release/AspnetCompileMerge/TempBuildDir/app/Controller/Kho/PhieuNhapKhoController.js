app.controller("PhieuNhapKhoController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, chuyenGiaSangChu) {
    $scope.dsKeHoach = [];
    $scope.dsNhapKho = [];
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
        { 'ID': 1, 'TenHang': 'Quần áo', 'MaHang': '01', 'disable': false },
        { 'ID': 2, 'TenHang': 'Đạn dược', 'MaHang': '02', 'disable': false },
        { 'ID': 3, 'TenHang': 'Xăng dầu', 'MaHang': '03', 'disable': false },
    ]
    $scope.trangthai = [
        { 'MA': 'N', 'NAME': 'Khởi tạo' },
        { 'MA': 'A', 'NAME': 'Đã duyệt' }
    ];
   
    DefaultSettings = () => {
        $scope.dsKeHoach = [];
        $scope.dsNhapKho = [];
        $scope.dsKetQua = [];
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
        $('#btnSave').prop('disabled', false);
        $scope.soLuongKetQua = $scope.dsKeHoach.length;
        $scope.selectedRow = 0;
        $scope.ShowThongTin( $scope.dsKeHoach[0], 0);
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
    $("#formNhap").validate({
        rules: {
            sophieu: "required",
            khonhap: "required"
        }
    });
    CheckThongTin = () => {
        if ($scope.thongTinKho.NGAY_CT_VW == null || $scope.thongTinKho.NGAY_KY_HD_VW == null || $scope.thongTinKho.NGAY_HH_HD_VW == null || $scope.thongTinKho.NGUOI_KY_1 == null || $scope.thongTinKho.NGUOI_KY_2 == null || $scope.thongTinKho.NGUOI_KY_3 == null
            || $scope.thongTinKho.KHO == null || $scope.thongTinKho.NGUOI_PHU_TRACH_VW == null || $scope.thongTinKho.LY_DO_NHAP == null) {
            toastr.error("Chưa nhập đủ thông tin");
            return false;
        }
        if ($scope.dsNhapKho.some(element => (element.HANG_HOA == null || element.DVT_VW == null || element.SO_LUONG_KH == null || element.DON_GIA == null))) {
            toastr.error("Chưa nhập đủ thông tin hàng hóa");
            return false;
        }
        return true;
    }

    const CheckDsHangHoa = (hanghoa) => {
        if (hanghoa.HANG_HOA == null || hanghoa.DVT == null || hanghoa.SO_LUONG_TT == null) return false;
        else return true;
    }

    $scope.LuuPhieu = () => {
        //if ($("#formNhap").valid()) {
        try {
            if (CheckThongTin()) {
                var a = $scope.dsKeHoach.find(obj => obj.SoKeHoach === $scope.thongTinKho.SO_KE_HOACH);
                if (a !== undefined) {
                    $scope.thongTinKho.ID_KE_HOACH = $scope.dsKeHoach.find(obj => obj.SoKeHoach === $scope.thongTinKho.SO_KE_HOACH).IDKeHoach;
                }
                $scope.thongTinKho.NGAY_CT = moment($scope.thongTinKho.NGAY_CT_VW, 'DD/MM/YYYY').format('YYYYMMDD');
                $scope.thongTinKho.NGAY_KY_HD = moment($scope.thongTinKho.NGAY_KY_HD_VW, 'DD/MM/YYYY').format('YYYYMMDD');
                $scope.thongTinKho.NGAY_HH_HD = moment($scope.thongTinKho.NGAY_HH_HD_VW, 'DD/MM/YYYY').format('YYYYMMDD');
                $scope.thongTinKho.TONG_TIEN = $scope.dsNhapKho.sum("THANH_TIEN_KH");
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
                $scope.thongTinKho.ID_KHO = $scope.thongTinKho.KHO.ID;
                $scope.thongTinKho.TEN_KHO = $scope.thongTinKho.KHO.NAME;
                $scope.thongTinKho.NGUOI_PHU_TRACH = $scope.thongTinKho.NGUOI_PHU_TRACH_VW.ID;

                if ($scope.dsNhapKho.length === 0) {
                    toastr.error('Bạn chưa thêm hàng hóa nào!');
                } else {
                    $scope.dsNhapKho.forEach((x) => { x.ID_HANG = x.HANG_HOA.ID; x.MA_HANG = x.HANG_HOA.MaHang; x.TEN_HANG = x.HANG_HOA.TenHang, x.DVT = x.DVT_VW.NAME });
                    if ($scope.thongTinKho.ID_TXN != null) {
                        $.ajax({
                            type: 'post',
                            url: '/PhieuNhapKho/UpdatePhieuNhapKho',
                            data: {
                                thongTinPhieu: $scope.thongTinKho,
                                dsHangHoaChiTiet: $scope.dsNhapKho
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
                            url: '/PhieuNhapKho/InsertPhieuNhapKho',
                            data: {
                                thongTinPhieu: $scope.thongTinKho,
                                dsHangHoaChiTiet: $scope.dsNhapKho
                            },
                            success: function (response) {
                                if (response.Status === 200) {
                                    $scope.thongTinKho.ID_TXN = response.Data;
                                    toastr.success('Thêm mới hàng hóa thành công!');
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

                }
            } else {
                console.log("dmm");
            }
        } catch (e) {
            toastr.error('Có lỗi xảy ra trong quá trình lưu!');
        }
        finally {
            hideLoading();
        }
    }

    $scope.ShowThongTin = (item, indexRow) => {
        showToast();
        $scope.thongTinKho = {};
        $scope.dsNhapKho = [];
        $scope.selectedRow = indexRow;
        $scope.ngayLapKh = item.NgayLap;
        $.ajax({
            type: 'post',
            url: '/PhieuNhapKho/GetPhieuBySoKeHoach',
            data: {
                soKeHoach: item.SoKeHoach
                },
            success: function (response) {
                console.log(response);
                if (response.Data == null) {
                    $scope.thongTinKho.SO_KE_HOACH = item.SoKeHoach;
                    $scope.thongTinKho.SO_PHIEU = GenerateRandomNumber();
                    $('#btnAdd').prop('disabled', false);
                    $('#btnEdit').prop('disabled', true);
                    $('#btnThemHangHoa').prop('disabled', false);

                } else {
                    $('#btnSave').prop('disabled', false);
                    $scope.thongTinKho = response.Data;
                    $scope.thongTinKho.NGUOI_KY_1 = $scope.canbo.find(x => x.ID === $scope.thongTinKho.ID_NGUOI_KY_1);
                    $scope.thongTinKho.NGUOI_KY_2 = $scope.canbo.find(x => x.ID === $scope.thongTinKho.ID_NGUOI_KY_2);
                    $scope.thongTinKho.NGUOI_KY_3 = $scope.canbo.find(x => x.ID === $scope.thongTinKho.ID_NGUOI_KY_3);
                    $scope.thongTinKho.KHO = $scope.danhmuckho.find(x => x.ID === $scope.thongTinKho.ID_KHO);
                    $scope.thongTinKho.NGUOI_PHU_TRACH_VW = $scope.canbo.find(x => x.ID === $scope.thongTinKho.NGUOI_PHU_TRACH);
                    if ($scope.thongTinKho.NGAY_KY_HD.length === 8) {
                        $scope.thongTinKho.NGAY_KY_HD_VW = moment($scope.thongTinKho.NGAY_KY_HD, 'YYYYMMDD').format('DD/MM/YYYY')
                    }
                    if ($scope.thongTinKho.NGAY_HH_HD.length === 8) {
                        $scope.thongTinKho.NGAY_HH_HD_VW = moment($scope.thongTinKho.NGAY_HH_HD, 'YYYYMMDD').format('DD/MM/YYYY')
                    }
                    if ($scope.thongTinKho.NGAY_CT.length === 8) {
                        $scope.thongTinKho.NGAY_CT_VW = moment($scope.thongTinKho.NGAY_CT, 'YYYYMMDD').format('DD/MM/YYYY')
                    }
                    $scope.ThongTinHangHoa($scope.thongTinKho.ID_TXN);
                    $('#btnAdd').prop('disabled', false);
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
            url: '/NhapKho/GetDsHangHoaByPhieu',
            data: {
                phieuNKId: id
            },
            success: function (response) {
                if (response.Status === 200) {
                    $scope.dsNhapKho = response.Data;
                    //$scope.dsNhapKho.HANG_HOA = $scope.hanghoa.find(x => x.ID === $scope.dsNhapKho.ID_HANG)
                    $scope.dsNhapKho.forEach(mathang => { mathang.HANG_HOA = $scope.hanghoa.find(x => x.ID === mathang.ID_HANG), mathang.DVT_VW = $scope.donvitinh.find(x => x.NAME === mathang.DVT) });
                    //$scope.dsNhapKho.DVT = $scope.donvitinh.find(x => x.NAME === $scope.dsNhapKho.DVT)
                }
                else {
                    toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm!');
                }
                console.log(response);

                $scope.$apply();
            },
            error: (ex) => {
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
        //if ($scope.dsNhapKho[$scope.dsNhapKho.length - 1].HANG_HOA != null) {
        //    $scope.hanghoa[$scope.dsNhapKho[$scope.dsNhapKho.length - 1].HANG_HOA.ID - 1].disable = true;
        //    //HangHoaSelectedChange($scope.dsNhapKho[$scope.dsNhapKho.length - 1].HANG_HOA.ID)
        //}
        $scope.dsNhapKho.push({ 'STT': $scope.dsNhapKho.length + 1 });
    }
    $scope.TabThemHangHoa = (e) => {
        if (e.key == "ArrowDown") {
            e.preventDefault();
            $scope.ThemHangHoa();
        }
        if (e.key == "ArrowUp") {
            e.preventDefault();
            $scope.XoaHangHoa($scope.dsNhapKho.length-1);
        }
    }
    $scope.XoaHangHoa = (index) => {
        if ($scope.dsNhapKho[index].HANG_HOA != null || $scope.dsNhapKho[index].DVT_VW != null || $scope.dsNhapKho[index].DVT_VW != null || $scope.dsNhapKho[index].SO_LUONG_KH != null || $scope.dsNhapKho[index].DON_GIA != null || $scope.dsNhapKho[index].GHI_CHU != null) {
           if (confirm('Bạn có chắc muốn xóa hàng hóa này?')) {
                $scope.dsNhapKho.splice(index, 1);
                $scope.dsNhapKho.forEach((x, index) => x.STT = index + 1);
            } else {
                // Do nothing!
                console.log('Không xóa gì hết');
            }
        }
        else {
            $scope.dsNhapKho.splice(index, 1);
            $scope.dsNhapKho.forEach((x, index) => x.STT = index + 1);
        }     
    }
    $scope.ValueCount = (item,index) => {
        if (typeof item.SO_LUONG_KH == 'number' && typeof item.DON_GIA == 'number') {
            $scope.dsNhapKho[index].THANH_TIEN_KH = item.SO_LUONG_KH * item.DON_GIA;
        };
    }
    HangHoaSelectedChange = (item) => {
        $scope.hanghoa = $scope.hanghoa.filter(x => x.ID !== item);
    }

    GenerateRandomNumber = () => {
        var newDate = new Date();
        var soPhieu = 'N' + moment(newDate).format('YYYYMMDDHHmm');
        return soPhieu;
    }
})