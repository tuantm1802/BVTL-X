app.controller("TongHopXuatNhapTonController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, chuyenGiaSangChu) {
    $scope.LoadThongTinUser = (user, appCode, unitId) => {
        $scope.appCode = appCode;
        $scope.user = user;
        $scope.unitId = unitId;
    }
    GetDanhMuc = () => {
        showToast();
        try {
            $.ajax({
                type: 'post',
                url: '/TongHopXuatNhapTon/GetDanhMucChung',
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
                        $scope.hanghoa = response.HangHoa;
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
    $scope.currentPage = 0;
    $scope.dsKetQua = [];
    $scope.thongTinKho = {};
    $scope.hangDoi = {
        'TuNgay': new Date(), 'DenNgay': new Date(), 'Kho': '', 'DonVi': ''
    };
    DefaultSettings = () => {
        $scope.dsKeHoach = [];
        $scope.dsNhapKho = [];
        $scope.thongTinKho = {};
        $('#btnAdd').prop('disabled', true);
        $('#btnEdit').prop('disabled', true);
        $('#btnSave').prop('disabled', true);
        $('#btnPrint').prop('disabled', true);
        $('#btnThemHangHoa').prop('disabled', true);
        $scope.CoDinhKem1 = false;
        $scope.CoDinhKem2 = false;
        $scope.CoDinhKem3 = false;
    }
    $scope.pageSize = 10;
    $scope.currentPage = 1;
    $scope.SoLuongKetQua = 0;
    DefaultSettings();
    GetDanhMuc();
    var date = new Date();
    date.setDate(date.getDate() - 7);
    $scope.hangDoi = {
        'TuNgay': moment(date).format('DD/MM/YYYY'), 'DenNgay': moment(new Date()).format('DD/MM/YYYY'), Kho: null, DonVi: null
    }

    $scope.TimKiem = () => {
        $scope.dsKetQua = $scope.danhmuckho;
        $scope.selectedRow = 0;
        $scope.ShowThongTin($scope.dsKetQua[0],0);
    }

    $scope.ShowThongTin = (item,index) => {
        $scope.selectedRow = index;
        $scope.dsTongHop = [];
        $.ajax({
            type: 'post',
            url: '/TongHopXuatNhapton/GetTongHopXNT',
            data: {
                khoId: item.ID,
                appCode: $scope.appCode,
                tuNgay: moment($scope.hangDoi.TuNgay, 'DD/MM/YYYY').format('YYYYMMDD'),
                denNgay: moment($scope.hangDoi.DenNgay, 'DD/MM/YYYY').add(1, 'days').format('YYYYMMDD')
            },
            success: function (response) {
                if (response.Status === 200) {
                    $scope.dsTongHop = response.Data;
                    $scope.dsTongHop.forEach(x => x.HANG_HOA = $scope.hanghoa.find(y => y.ID === x.ID_HANG));
                }
                else {
                    toastr.error(response.Message);
                }
                console.log(response);

                $scope.$apply();
            }
        });
    }
});

