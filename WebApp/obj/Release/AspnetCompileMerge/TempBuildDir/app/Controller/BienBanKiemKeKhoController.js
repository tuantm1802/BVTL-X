app.controller("BienBanKiemKeKhoController", function ($scope, $window, $uibModal, $ngConfirm, showToast, $timeout, hideLoading, $rootScope) {
    $scope.totalRecordBB = 0;
    $scope.displayNumber = 0;
    $scope.pageSizeBB = 5;
    $scope.pageIndexBB = 1;

    $scope.danhSachKho = [];
    $scope.danhSachNhomDuLieu = [
        { 'ID': '', 'NAME': '-- Tất cả --' },
        { 'ID': 'PHUONG_TIEN_BO', 'NAME': 'Phương tiện đường bộ và xăng dầu' },
        { 'ID': 'PHUONG_TIEN_THUY', 'NAME': 'Phương tiện đường thuỷ' },
        { 'ID': 'VTU_THIET_BI', 'NAME': 'Vật tư, thiết bị kỹ thuật nghiệp vụ' },
        { 'ID': 'VU_KHI_VLN', 'NAME': 'Vũ khí, vật liệu nổ' },
        { 'ID': 'CP_QUAN_TRANG', 'NAME': 'Quân trang' }
    ];
    $scope.listBienBan = [];
    $scope.listThanhVien = [];
    $scope.danhSachHangHoa = [];
    $scope.thongTinBienBan = [];
    $scope.danhSachCanBo = [];
    var luuDanhSachHangHoa = null;

    $scope.appCodeType2 = true;
    $scope.showBtnDinhKem = false;

    var idBD = null;
    var tenKho = "";
    var ngayct = "";
    var appCode = "";
    var appCodeType = "";

    angular.element(document).ready(function () {
        $scope.GetDanhMuc();
    });

    $scope.GetDanhMuc = function () {
        $.ajax({
            type: "post",
            url: "/BienBanKiemKeKho/LayDanhMuc",
            data: {},
            success: function (response) {
                $scope.danhSachKho = response.danhSachKho;
                $scope.danhSachCanBo = response.danhSachCanBo;
                appCodeType = response.appCodeType;
                appCode = response.appCode;
                $scope.$apply();
            }
        });
    }

    $scope.getDanhSachBienBan = function () {
        var idKho = 0;
        if ($scope.modelSearch_khoKiemKe != null) {
            idKho = $scope.modelSearch_khoKiemKe;
        }
        $.ajax({
            type: "POST",
            url: "/BienBanKiemKeKho/TimKiemBienBanKiemKe",
            data: {
                idKho: idKho,
                tuNgay: $scope.modelSearch_tuNgay,
                denNgay: $scope.modelSearch_denNgay,
                trangThai: $scope.modelSearch_trangThai,
                pageSize: $scope.pageSizeBB,
                pageNumber: $scope.pageIndexBB
            },
            success: function (response) {
                if (response != null && response.length > 0) {
                    $scope.listBienBan = response;
                    $scope.selectedRow = 0;
                    $scope.displayNumber = response.length;
                    $scope.totalRecordBB = response[0].TOTAL;
                    $scope.$apply();
                } else {
                    toastr.error('Không tìm thấy kết quả.');
                }
            }
        })
    }

    $scope.LoadDetailBienBan = function (item, index) {
        idBD = item.ID_BD;
        tenKho = item.TEN_KHO;
        ngayct = item.NGAY_CT;
        $scope.danhsachanh = [];
        $.ajax({
            type: 'POST',
            url: '/BienBanKiemKeKho/GetBienBan',
            data: { idBD: idBD },
            success: function (response) {
                if (response != null) {
                    $scope.thongTinBienBan = response;
                    //console.log($scope.thongTinBienBan);
                    try {
                        GetChiTietBienBan(idBD);
                        GetHoiDongKiemKe(idBD);
                    } catch (e) {
                        console.error(e);
                    }

                    $scope.thongTinBienBan.ID_BD = idBD;
                    $scope.thongTinBienBan.ID_KHO = $scope.danhSachKho.filter(x => x.ID == response.ID_KHO)[0].ID.toString();
                    if (response.NGUOI_VIET_PHIEU != null) {
                        $scope.thongTinBienBan.NGUOI_VIET_PHIEU = $scope.danhSachCanBo.filter(x => x.ID == response.NGUOI_VIET_PHIEU)[0].ID.toString();
                    }
                    console.log($scope.thongTinBienBan.NGAY_CHOT_KIEM_KE);
                    $scope.thongTinBienBan.NGAY_CHOT_KIEM_KE = $scope.thongTinBienBan.NGAY_CHOT_KIEM_KE == null ? '' : moment($scope.thongTinBienBan.NGAY_CHOT_KIEM_KE, 'YYYYMMDD').format('DD/MM/YYYY');
                    $scope.thongTinBienBan.NHOM_DU_LIEU = appCode;
                    $scope.selectedRow = index;
                    //
                    if (response.IMG_PATH_1 != null) {
                        $scope.danhsachanh.push({
                            'src': `/BienBanKiemKeKho/Image?imgPath=${response.IMG_PATH_1}`,
                            'checked': false,
                            'IMG_PATH': response.IMG_PATH_1
                        })
                    }

                    // Check trang thai va disabled button -> TODO
                    $scope.showBtnDinhKem = true;
                    if (response.TRANG_THAI == 'D') {
                        $scope.appCodeType2 = true;
                    } else {
                        $scope.appCodeType2 = false;
                    }

                    $scope.$apply();
                }
            },
            error: function (err) {
                console.log(err);
            }
        });
    }

    GetChiTietBienBan = (id) => {
        $.ajax({
            type: 'post',
            url: '/BienBanKiemKeKho/GetChiTietBienBanByID',
            data: { idBD: id },
            success: function (response) {

                if (response != null) {
                    $scope.danhSachHangHoa = response;
                    $scope.danhSachHangHoa = $scope.danhSachHangHoa.filter(x => x.APP_CODE == appCode);
                    console.log($scope.danhSachHangHoa);

                    angular.forEach($scope.danhSachHangHoa, function (data) {
                        var chenhLechSL = 0;
                        if (data.SO_LUONG_TT != null && data.SO_LUONG_TT != "") {
                            chenhLechSL = data.SO_LUONG_TT - data.SO_LUONG_KH;

                            if (chenhLechSL > 0) {
                                data.SO_LUONG_THUA = chenhLechSL;
                                data.THANH_TIEN_THUA = data.THANH_TIEN_TT - data.THANH_TIEN_KH;
                            }

                            if (chenhLechSL < 0) {
                                data.SO_LUONG_THIEU = 0 - chenhLechSL;
                                data.THANH_TIEN_THIEU = data.THANH_TIEN_KH - data.THANH_TIEN_TT;
                            }
                        }
                    })
                    luuDanhSachHangHoa = $scope.danhSachHangHoa;
                    $scope.$apply();
                }
            }
        })
    }


    $scope.printBienBan = () => {

        //var kho = $scope.danhmuckho.filter(x => x.ID === $scope.hangDoi.Kho);
        //var tenKho = '';
        //if (kho.length > 0) {
        //    tenKho = 'Tại kho: ' + kho[0].TEN;
        //}
        $.ajax({
            url: '/BienBanKiemKeKho/InBaoCao',
            type: 'post',
            data: {
                idBD: idBD,
                ten_kho: tenKho,
                ngay_ct: ngayct
            },
            success: function (result) {

                let pdfWindow = $window.open("");
                pdfWindow.document.write("<iframe width='100%' height='100%' src='data:application/pdf;base64," + result + "'></iframe>");

            },
            error: function (xhr, status, err) {
                alert(err);
            }
        });
    }
    GetHoiDongKiemKe = (id) => {
        $.ajax({
            type: 'post',
            url: '/BienBanKiemKeKho/GetHoiDongKiemKeByID',
            data: { idBD: id },
            success: function (response) {
                if (response != null) {
                    $scope.listThanhVien = response;
                    $scope.$apply();
                }
            }
        })
    }

    $scope.copyDuLieuTheoHTToTheoTT = function () {
        //Copy dữ liệu từ cột Theo hệ thống sang cột THEO THỰC TẾ
        $scope.danhSachHangHoa.forEach(
            (x,index) => {
                (x.SO_LUONG_TT == '' || x.SO_LUONG_TT == 0 ) ? x.SO_LUONG_TT = x.SO_LUONG_KH : x.SO_LUONG_TT;
                (x.THANH_TIEN_TT == '' || x.THANH_TIEN_TT == 0) ? x.THANH_TIEN_TT = x.THANH_TIEN_KH : x.THANH_TIEN_TT;
                $scope.changeSLTT(index);
                //x.THANH_TIEN_TT = x.THANH_TIEN_KH;
        });
    }

    $scope.saveChange = function () {
        if ($scope.thongTinBienBan.NGAY_KIEM_KE == undefined || $scope.thongTinBienBan.NGAY_KIEM_KE == "") {
            toastr.Error('Chưa nhập ngày bắt đầu kiểm kê.');
            return;
        }

        if ($scope.thongTinBienBan.NGAY_CT == undefined || $scope.thongTinBienBan.NGAY_CT == ""){
            toastr.Error('Chưa nhập ngày lập biên bản.');
            return;
        }
        $scope.thongTinBienBan.NGAY_CHOT_KIEM_KE = moment($scope.thongTinBienBan.NGAY_CHOT_KIEM_KE, 'DD/MM/YYYY').format('YYYYMMDD')
        //console.log($scope.thongTinBienBan.NGAY_CHOT_KIEM_KE);

        if (idBD == null) {    
            // Thêm mới biên bản
            $.ajax({
                type: "post",
                url: "/BienBanKiemKeKho/ThemMoiBienBan",
                data: { bienBanMoi: $scope.thongTinBienBan, chiTietBienBan: $scope.danhSachHangHoa, hoiDongKiemKe: $scope.listThanhVien },
                success: function (response) {
                    if (response.Error) {
                        toastr.error(response.Title);
                    } else {
                        toastr.success(response.Title);
                        $scope.getDanhSachBienBan();
                    }
                }
            });
        } else {
            var errorFlag = false;
            // Cập nhật biên bản
            if ($scope.listThanhVien != null) {
                angular.forEach($scope.listThanhVien, function (data) {
                    console.log(data);
                    data.ID_BD_TV = data.ID_BD_TV == null ? 0 : data.ID_BD_TV;
                    if (data.TEN_TV == "" || data.CV_TV == "" || data.CB_TV == "") {
                        toastr.error('Không được để trống tên, chức vụ và vị trí của thành viên.');
                        errorFlag = true;
                    }
                })
            }

            var listChiTietBienBan = [];
            console.log($scope.danhSachHangHoa);
            angular.forEach($scope.danhSachHangHoa, function (data) {
                var tempSLLech = (data.SO_LUONG_KH - data.SO_LUONG_TT) >= 0 ? (data.SO_LUONG_KH - data.SO_LUONG_TT) : (data.SO_LUONG_TT - data.SO_LUONG_KH);
                var tempTienLech = (data.THANH_TIEN_KH - data.THANH_TIEN_TT) >= 0 ? (data.THANH_TIEN_KH - data.THANH_TIEN_TT) : (data.THANH_TIEN_TT - data.THANH_TIEN_KH);
                listChiTietBienBan.push({
                    ID_BD_CT: data.ID_BD_CT,
                    MA_HANG: data.MA_HANG,
                    TEN_HANG: data.TEN_HANG,
                    DVT: data.DVT,
                    SO_LUONG_KH: data.SO_LUONG_KH,
                    THANH_TIEN_KH: data.THANH_TIEN_KH,
                    SO_LUONG_TT: data.SO_LUONG_TT,
                    THANH_TIEN_TT: data.THANH_TIEN_TT,
                    SO_LUONG_LECH: tempSLLech,
                    THANH_TIEN_LECH: tempTienLech,
                    PHAN_LOAI_TAI_SAN: data.PHAN_LOAI_TAI_SAN,
                    GHI_CHU: data.GHI_CHU
                });
            })
            if (!errorFlag) {
                $.ajax({
                    type: "post",
                    url: "/BienBanKiemKeKho/CapNhatBienBan",
                    data: { bienBanMoi: $scope.thongTinBienBan, chiTietBienBan: listChiTietBienBan, hoiDongKiemKe: $scope.listThanhVien, idBD: idBD, imgPath: $scope.danhsachanh },
                    success: function (response) {
                        toastr.success('Lưu biên bản thành công.');
                        $scope.LoadDetailBienBan($scope.listBienBan[$scope.selectedRow], $scope.selectedRow);
                    }
                });
            }
        }
    }

    $scope.cancelUpdate = function () {
        if (idBD != null) {
            $scope.LoadDetailBienBan($scope.listBienBan[$scope.selectedRow], $scope.selectedRow);
        }
    }

    $scope.checkAll = () => {
        angular.forEach($scope.listThanhVien, function (obj) {
            obj["SELECT"] = !$scope.selectAll;
            if (obj["SELECT"] === true) obj["EditMode"] = true;
            else obj["EditMode"] = false;
        })
    }

    $scope.changeSLTT = function (index) {
        var tempSL = parseInt($scope.danhSachHangHoa[index].SO_LUONG_TT);
        if (tempSL == NaN) {
            toastr.error('Chỉ được nhập kiểu số nguyên cho Số lượng thực tế.');
            $scope.danhSachHangHoa[index].SO_LUONG_TT = "";
            $scope.danhSachHangHoa[index].THANH_TIEN_TT = "";
            $scope.danhSachHangHoa[index].SO_LUONG_THUA = "";
            $scope.danhSachHangHoa[index].SO_LUONG_THIEU = "";
            $scope.danhSachHangHoa[index].THANH_TIEN_THUA = "";
            $scope.danhSachHangHoa[index].THANH_TIEN_THIEU = "";
            return;
        }

        if ($scope.danhSachHangHoa[index].SO_LUONG_TT != '') {
            $scope.danhSachHangHoa[index].THANH_TIEN_TT = $scope.danhSachHangHoa[index].SO_LUONG_TT * $scope.danhSachHangHoa[index].THANH_TIEN_KH / $scope.danhSachHangHoa[index].SO_LUONG_KH;
            $scope.danhSachHangHoa[index].SO_LUONG_THUA = "";
            $scope.danhSachHangHoa[index].SO_LUONG_THIEU = "";
            $scope.danhSachHangHoa[index].THANH_TIEN_THUA = "";
            $scope.danhSachHangHoa[index].THANH_TIEN_THIEU = "";

            if ($scope.danhSachHangHoa[index].SO_LUONG_TT - $scope.danhSachHangHoa[index].SO_LUONG_KH > 0) {
                $scope.danhSachHangHoa[index].SO_LUONG_THUA = $scope.danhSachHangHoa[index].SO_LUONG_TT - $scope.danhSachHangHoa[index].SO_LUONG_KH;
                $scope.danhSachHangHoa[index].THANH_TIEN_THUA = $scope.danhSachHangHoa[index].THANH_TIEN_TT - $scope.danhSachHangHoa[index].THANH_TIEN_KH;
            }

            if ($scope.danhSachHangHoa[index].SO_LUONG_TT - $scope.danhSachHangHoa[index].SO_LUONG_KH < 0) {
                $scope.danhSachHangHoa[index].SO_LUONG_THIEU = $scope.danhSachHangHoa[index].SO_LUONG_KH - $scope.danhSachHangHoa[index].SO_LUONG_TT;
                $scope.danhSachHangHoa[index].THANH_TIEN_THIEU = $scope.danhSachHangHoa[index].THANH_TIEN_KH - $scope.danhSachHangHoa[index].THANH_TIEN_TT;
            }
        }
    }

    $scope.changeTien = function (index) {
        if (parseInt($scope.danhSachHangHoa[index].THANH_TIEN_TT) == NaN) {
            toastr.Error('Chỉ được nhập kiểu số nguyên cho Thành tiền thực tế.');
            $scope.danhSachHangHoa[index].THANH_TIEN_TT = 0;
            $scope.danhSachHangHoa[index].THANH_TIEN_THUA = "";
            $scope.danhSachHangHoa[index].THANH_TIEN_THIEU = "";
            return;
        }

        if ($scope.danhSachHangHoa[index].THANH_TIEN_TT != '') {
            $scope.danhSachHangHoa[index].THANH_TIEN_THUA = "";
            $scope.danhSachHangHoa[index].THANH_TIEN_THIEU = "";

            if ($scope.danhSachHangHoa[index].THANH_TIEN_TT - $scope.danhSachHangHoa[index].THANH_TIEN_KH > 0) {
                $scope.danhSachHangHoa[index].THANH_TIEN_THUA = $scope.danhSachHangHoa[index].THANH_TIEN_TT - $scope.danhSachHangHoa[index].THANH_TIEN_KH;
            }

            if ($scope.danhSachHangHoa[index].THANH_TIEN_TT - $scope.danhSachHangHoa[index].THANH_TIEN_KH < 0) {
                $scope.danhSachHangHoa[index].THANH_TIEN_THIEU = $scope.danhSachHangHoa[index].THANH_TIEN_KH - $scope.danhSachHangHoa[index].THANH_TIEN_TT;
            }
        }
    }
});