app.controller("PhieuGiaoThangQuanTrangController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, chuyenGiaSangChu) {
    DisablePanel = (type) => {
        if (type) {
            $('#sub-action *').prop('disabled', true);
        }
        else {
            $('#sub-action *').prop('disabled', false);
        }
    }
    DefaultSettings = () => {
        DisablePanel(true);
        ResetVariables();
        $('#btnAdd').prop('disabled', true);
        $('#btnEdit').prop('disabled', true);
        $('#btnSave').prop('disabled', true);
        $('#btnConfirm').prop('disabled', true);
        $('#btnRemove').prop('disabled', true);
        $('#btnPrint').prop('disabled', true);
        $('#btnThemHangHoa').prop('disabled', true);

    }
    ResetVariables = () => {
        if ($scope.hanghoa != undefined) {
            $scope.dsHangHoaChon = $scope.hanghoa.map(object => ({ ...object }));
        }
        $scope.dsHangHoaDaChon = [];
        $scope.dsChitietToanBo = [];
        $scope.thongTinKh = {};
        $scope.danhsachanh = [];
        $scope.CoKetQua = false;
        $scope.VanChuyenStatus = false;
        $scope.HsStatus = true;
        $scope.pageSize = 10;
        $scope.SoLuongKetQua = 0;
    }
    GetDanhMuc = () => {
        try {
            $.ajax({
                type: 'post',
                url: '/DanhMucKho/GetDanhMucChung',
                data: {
                },
                success: function (response) {
                    if (response.Status === 200) {
                        var kq = response.Data;
                        $scope.dmnguonkinhphi = response.DanhMucRieng.filter(x => x.LOAI === 'NGUON_KINH_PHI');
                        $scope.dmnguonhang = response.DanhMucRieng.filter(x => x.LOAI === 'NGUON_GOC_HH');
                        $scope.dmchatluong = response.DanhMucRieng.filter(x => x.LOAI === 'CAP_CHAT_LUONG');
                        $scope.dmnhomhang = response.DanhMucRieng.filter(x => x.LOAI === 'NHOM_PT_BO');
                        $scope.dmloaihang = response.DanhMucRieng.filter(x => x.LOAI === 'LOAI_PT_BO');
                        $scope.danhmuckho = response.DanhMucChung.filter(x => x.LOAI === 'KHO');
                        $scope.donvi = response.DanhMucChung.filter(x => x.LOAI === 'DON_VI');
                        $scope.dmcongty = response.DanhMucChung.filter(x => x.LOAI === 'CONG_TY');
                        $scope.danhmucloaikh = response.DanhMucChung.filter(x => x.LOAI === 'LOAI_KE_HOACH');
                        $scope.trangthai = response.DanhMucChung.filter(x => x.LOAI === 'TRANG_THAI_NHAP_KHO');
                        $scope.dmhtvanchuyen = response.DanhMucChung.filter(x => x.LOAI === 'HINH_THUC_VAN_CHUYEN');
                        $scope.canbo = response.DanhMucCanBo;
                    }
                    console.log(response);

                    $scope.$apply();
                }, error: (e) => {
                    console.log(e);
                },
            });
        } catch (e) {
            console.log(e);
        }
    }

    ResetDieuKienTimKiem = () => {
        var date = new Date();
        date.setDate(date.getDate() - 7);
        $scope.hangDoi = {
            'TuNgay': moment(date).format('DD/MM/YYYY'), 'DenNgay': moment(new Date()).format('DD/MM/YYYY'), Kho: null, DonVi: null
        }
    }
    ResetHienThiHangDoi = () => {
        $scope.pageSize = 10;
        $scope.SoLuongKetQua = 0;
        $scope.SoLuongHienThi = 0;
        $scope.CoKetQua = false;
    }
    $scope.currentPage = 1;
    DefaultSettings();
    GetDanhMuc();
    ResetDieuKienTimKiem();
    ResetHienThiHangDoi();

    $scope.LuuPhieu = () => {
        showToast();
        try {
            if ($('#main-info').valid()) {
                if ($scope.thongTinKh.NGAY_CT_VW != undefined) {
                    $scope.thongTinKh.NGAY_CT = moment($scope.thongTinKh.NGAY_CT_VW, 'DD/MM/YYYY').format('YYYYMMDD');
                }
                $scope.thongTinKh.NGUOI_KY_1 = $scope.canbo.find(x => x.ID === $scope.thongTinKh.ID_NGUOI_KY_1);
                if ($scope.NGUOI_KY_1 != undefined) {
                    $scope.thongTinKh.TEN_NGUOI_KY_1 = $scope.thongTinKh.NGUOI_KY_1.HoTen;
                    $scope.thongTinKh.CV_NGUOI_KY_1 = $scope.thongTinKh.NGUOI_KY_1.ChucVu;
                    $scope.thongTinKh.CB_NGUOI_KY_1 = $scope.thongTinKh.NGUOI_KY_1.CapBac;
                }
                $scope.thongTinKh.NGUOI_KY_2 = $scope.canbo.find(x => x.ID === $scope.thongTinKh.ID_NGUOI_KY_2);
                if ($scope.NGUOI_KY_2 != undefined) {
                    $scope.thongTinKh.TEN_NGUOI_KY_2 = $scope.thongTinKh.NGUOI_KY_2.HoTen;
                    $scope.thongTinKh.CV_NGUOI_KY_2 = $scope.thongTinKh.NGUOI_KY_2.ChucVu;
                    $scope.thongTinKh.CB_NGUOI_KY_2 = $scope.thongTinKh.NGUOI_KY_2.CapBac;
                }

                $.ajax({
                    type: 'post',
                    url: '/PhieuGiaoThangQuanTrang/UpdatePhieuXuatKho',
                    data: {
                        thongTinPhieu: $scope.thongTinKh
                    },
                    success: function (response) {
                        if (response.Status === 200) {
                            toastr.success('Cập nhật thông tin thành công!');
                        }
                        else {
                            toastr.error('Có lỗi xảy ra trong quá trình lưu!');
                        }
                        console.log(response);

                        $scope.$apply();
                    }, error: (e) => {
                        console.error('Phiếu xuất kho- Lưu phiếu : ' + e);
                        toastr.error('Có lỗi xảy ra trong quá trình lưu!');
                    },
                    complete: () => {
                        hideLoading();
                    }
                });
            }
        } catch (e) {
            toastr.error('Có lỗi xảy ra trong quá trình lưu!');
        } finally {
            hideLoading();
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
                    url: '/PhieuGiaoThangQuanTrang/DuyetPhieu',
                    data: {
                        idPhieu: $scope.thongTinKh.ID_TXN,
                    },
                    success: function (response) {
                        if (response.Status === 200) {
                            $scope.thongTinKh.TRANG_THAI === 'A';
                            $scope.dsKetQua[$scope.selectedRow].TRANG_THAI = 'A';
                            $scope.dsKetQua[$scope.selectedRow].TRANG_THAI_VW = $scope.trangthai.find(y => y.MA === 'A').TEN;
                            $scope.HsStatus = true;
                            toastr.success(response.Message);
                        }
                        else {
                            toastr.error('Có lỗi xảy ra trong quá trình duyệt phiếu!');
                        }
                        console.log(response);
                        $scope.$apply();
                    }, error: (e) => {
                        toastr.error('Có lỗi xảy ra trong quá trình duyệt phiếu!');
                    }
                });
            }
        }
        catch (e) {
            console.log(e);
            toastr.error('Có lỗi xảy ra trong quá trình duyệt phiếu!');
        }
        finally {
            hideLoading();
        }
    }

    $scope.TimKiem = () => {
        try {
            DefaultSettings();
            $.ajax({
                type: 'post',
                url: '/PhieuGiaoThangQuanTrang/TimKiemPhieu',
                data: {
                    soPhieu: $scope.hangDoi.SoPhieu,
                    soKeHoach: $scope.hangDoi.SoKeHoach,
                    tuNgay: moment($scope.hangDoi.TuNgay, 'DD/MM/YYYY').format('YYYYMMDD'),
                    denNgay: moment($scope.hangDoi.DenNgay, 'DD/MM/YYYY').add(1, 'days').format('YYYYMMDD'),
                    khoId: $scope.hangDoi.Kho === null ? null : $scope.hangDoi.Kho.ID,
                    trangThai: null,
                    donViId: $scope.hangDoi.DonVi === null ? null : $scope.hangDoi.DonVi.ID,
                    pageSize: $scope.pageSize,
                    pageNumber: $scope.currentPage
                },
                success: function (response) {
                    if (response.Status === 200) {

                        $scope.dsKetQua = response.Data;
                        $scope.dsKetQua.forEach(x => { x.TRANG_THAI_VW = $scope.trangthai.find(y => y.MA === x.TRANG_THAI).TEN, x.NGAY_TAO = moment(new Date(Number(x.NGAY_TAO.replace(/\D/g, '')))).format('DD/MM/YYYY') });
                        $scope.CoKetQua = true;
                        $scope.SoLuongKetQua = $scope.dsKetQua[0].total;
                        $scope.SoLuongHienThi = $scope.dsKetQua.length;
                        console.log(response.Data);
                        $scope.selectedRow = 0;
                        $scope.$apply();

                        $scope.ShowThongTin($scope.dsKetQua[0], 0);
                    }
                    else if (response.Status === 204) {
                        toastr.error('Không tìm thấy kết quả nào!');
                    }
                    else {
                        toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm!');
                    }
                }, error: (e) => {
                    toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm!');
                }
            });
        } catch (e) {
            console.error(e);
        }
    }
    $scope.ShowThongTin = (item, index) => {
        try {
            showToast();
            console.log('a' + $scope.currentPage);
            $scope.selectedRow = index;
            $scope.thongTinKh = {};
            $scope.thongTinKh = item;
            $scope.thongTinKh.NGUOI_KY_1 = $scope.canbo.find(x => x.ID === $scope.thongTinKh.ID_NGUOI_KY_1);
            $scope.thongTinKh.NGUOI_KY_2 = $scope.canbo.find(x => x.ID === $scope.thongTinKh.ID_NGUOI_KY_2);
            if ($scope.thongTinKh.NGAY_XUAT_DK_TU != null) {
                if ($scope.thongTinKh.NGAY_XUAT_DK_TU.length === 8) {
                    $scope.thongTinKh.NGAY_XUAT_DK_TU_VW = moment($scope.thongTinKh.NGAY_XUAT_DK_TU, 'YYYYMMDD').format('DD/MM/YYYY');
                }
            }
            if ($scope.thongTinKh.NGAY_XUAT_DK_DEN != null) {
                if ($scope.thongTinKh.NGAY_XUAT_DK_DEN.length === 8) {
                    $scope.thongTinKh.NGAY_XUAT_DK_DEN_VW = moment($scope.thongTinKh.NGAY_XUAT_DK_DEN, 'YYYYMMDD').format('DD/MM/YYYY')
                }
            }
            if ($scope.thongTinKh.NGAY_NHAN_DK != null) {
                if ($scope.thongTinKh.NGAY_NHAN_DK.length === 8) {
                    $scope.thongTinKh.NGAY_NHAN_DK_VW = moment($scope.thongTinKh.NGAY_NHAN_DK, 'YYYYMMDD').format('DD/MM/YYYY')
                }
            }
            if ($scope.thongTinKh.NGAY_CT != null) {
                if ($scope.thongTinKh.NGAY_CT.length === 8) {
                    $scope.thongTinKh.NGAY_CT_VW = moment($scope.thongTinKh.NGAY_CT, 'YYYYMMDD').format('DD/MM/YYYY')
                }
            }

            ThongTinHangHoa($scope.thongTinKh.ID_TXN);
        } catch (e) {
            console.error(e);
        }
        finally {
            hideLoading();
        }
    }
    ThongTinHangHoa = (id) => {
        $scope.dsHangHoaChon = [];
        $scope.dsHangHoaDaChon = [];
        $scope.dsChitietToanBo = [];

        $.ajax({
            type: 'post',
            url: '/PhieuGiaoThangQuanTrang/GetDsHangHoaByPhieu',
            data: {
                idPhieu: id,
                loai: 'X'
            },
            success: function (response) {
                if (response.Status === 200) {
                    $('#btnSave').prop('disabled', false);
                    $scope.dsChitietToanBo = response.Data;
                    $scope.thongTinKh.ID_NGUON_GOC = $scope.dsChitietToanBo[0].ID_NGUON_GOC;
                    $scope.thongTinKh.ID_NGUON_KP = $scope.dsChitietToanBo[0].ID_NGUON_KP;
                    $scope.dsChitietToanBo.forEach(x => {
                        if (x.MO_TA_CT1 && x.MO_TA_CT2 || x.MO_TA_CT4) {
                            x.SO_LUONG_TT = x.SO_LUONG_KH,
                            x.THANH_TIEN_TT = x.THANH_TIEN_KH,
                            x.KHO = $scope.danhmuckho.find(y => y.ID === x.ID_KHO),
                            x.TEN_KHO = x.KHO != undefined ? x.KHO.TEN : null,
                            //x.HOP_DONG = $scope.danhmuchopdong.find(y => y.ID === x.ID_HOP_DONG),
                            x.CHAT_LUONG = $scope.dmchatluong.find(y => y.ID === x.ID_CHAT_LUONG),
                            x.TEN_CHAT_LUONG = x.CHAT_LUONG != undefined ? x.CHAT_LUONG.TEN : null
                        } else {
                            x.KHO = $scope.danhmuckho.find(y => y.ID === x.ID_KHO),
                            x.TEN_KHO = x.KHO != undefined ? x.KHO.TEN : null,
                            //x.HOP_DONG = $scope.danhmuchopdong.find(y => y.ID === x.ID_HOP_DONG),
                            x.CHAT_LUONG = $scope.dmchatluong.find(y => y.ID === x.ID_CHAT_LUONG),
                            x.TEN_CHAT_LUONG = x.CHAT_LUONG != undefined ? x.CHAT_LUONG.TEN : null
                        }
                    });

                    var dsHangHoaDaChon = [...new Map($scope.dsChitietToanBo.map(item => [item['ID_HANG'], item])).values()];
                    $scope.dsHangHoaDaChon = JSON.parse(JSON.stringify(dsHangHoaDaChon)) ;
                    $scope.dsHangHoaDaChon.forEach((x, index) => {
                         x.STT = index + 1
                    });
                    $scope.ShowChiTietHangHoa(0);
                }
                else {
                    toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm!');
                }
                console.log(response);

            }, error: (e) => {
                toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm!');
            }
        });
    }
    $scope.ShowChiTietHangHoa = (index) => {
        $scope.selectedGood = index;
        $scope.id_hang_da_chon = $scope.dsHangHoaDaChon[index].ID_HANG;
        const hangHoaCu = $scope.dsChitietToanBo.some(x => x.ID_HANG === $scope.id_hang_da_chon);
        if (hangHoaCu) {
            const dsChitietTheoHangHoa = $scope.dsChitietToanBo.filter(x => x.ID_HANG === $scope.id_hang_da_chon);
            $scope.tenHangHoaDaChon = dsChitietTheoHangHoa[0].TEN_HANG;
            $scope.soLuongDaChon = dsChitietTheoHangHoa.sum('SO_LUONG_KH');
            $scope.thanhTienDaChon = dsChitietTheoHangHoa.sum('THANH_TIEN_KH');
        }
        else {
            $scope.tenHangHoaDaChon = $scope.dsHangHoaDaChon[$scope.selectedGood].TEN_HANG;
            $scope.soLuongDaChon = $scope.dsHangHoaDaChon[$scope.selectedGood].SO_LUONG;
            $scope.soLuongDaChon = null;
        }
    }
   
    $scope.ThayDoiTableChiTiet = () => {
        if ($("#table-toggle").hasClass("fa-angle-double-left")) {
            $("#table-toggle").removeClass("fa-angle-double-left").addClass("fa-angle-double-right");
        }
        else if ($("#table-toggle").hasClass("fa-angle-double-right")) {
            $("#table-toggle").removeClass("fa-angle-double-right").addClass("fa-angle-double-left");
        }
            if ($('#right-table').hasClass("column-full")) {
                $('#left-table').css("display", "block");
                //$('#left-table').removeClass('column-none').addClass('column-left');
                $('#right-table').removeClass('column-full').addClass('column-right');
            }
            else {
                $('#left-table').css("display", "none");
                //$('#right-table').removeClass('column-left').addClass('column-none');
                $('#right-table').removeClass('column-right').addClass('column-full');
            }
        
    }
       
        $scope.SuaChiTietHangHoa = (item) => {
            $scope.chitietTheoHangHoa = item;
            $('#cs_hang_hoa_theo_hd').modal({ backdrop: 'static', keyboard: false, show: true })
            $('#cs_hang_hoa_theo_hd').modal('show');
        }

        GenerateRandomNumber = () => {
            var newDate = new Date();
            var soPhieu = 'X' + moment(newDate).format('YYYYMMDDHHmm');
            return soPhieu;
        }
    });