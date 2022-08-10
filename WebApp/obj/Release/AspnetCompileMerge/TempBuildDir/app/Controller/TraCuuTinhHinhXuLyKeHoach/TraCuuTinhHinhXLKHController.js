app.controller("TraCuuTinhHinhXLKHController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $location) {
    $scope.searchModel = {};

    $scope.modelKH = {};
    $scope.modelKH.pageSize = 10;
    $scope.modelKH.currentPage = 1;
    $scope.modelKH.totalItems = 0;
    $scope.modelKH.maxSize = 5;

    $scope.tabCheck = 'NK';

    angular.element(document).ready(function () {
        var date = new Date();
        $scope.searchModel.TuNgay = moment(new Date(date.getFullYear(), date.getMonth(), 1)).format('DD/MM/YYYY');
        $scope.searchModel.DenNgay = moment(new Date()).format('DD/MM/YYYY');
        $scope.$apply();
        
    });
    $scope.CreateSearchNgay = function (date) {
        var item = date.split('/');
        return item[2] + item[1] + item[0];
    }
    $scope.TimKiemKeHoach = function () {
        $.ajax({
            type: 'post',
            url: '/TraCuuTinhHinhXuLyKeHoach/TimKiemKeHoach',
            data: {
                soKH: $scope.searchModel.SoKH,
                loaiKH: $scope.searchModel.LoaiKH,
                tuNgay: $scope.CreateSearchNgay($scope.searchModel.TuNgay),
                denNgay: $scope.CreateSearchNgay($scope.searchModel.DenNgay),
                trangThai: $scope.searchModel.TrangThai,
                pageSize: $scope.modelKH.pageSize,
                currentPage: $scope.modelKH.currentPage
            },
            success: function (res) {
                if (res.Error) {
                    toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm!');
                } else {
                    $scope.DanhSachKeHoach = res.data;
                    if(res.data.length > 0)
                        $scope.modelKH.totalItems = res.data[0].TOTAL;
                    else
                        $scope.modelKH.totalItems = 0;
                    $scope.$apply();
                }
            }
        })
    }
    $scope.pageKHChanged = function () {
        $scope.TimKiemKeHoach();
    }
    $scope.FormatDate = function (date) {
        return moment(date).format('DD/MM/YYYY');
    }
    $scope.ChonKeHoach = function (item, index) {
        if (item.LOAI_KH === 'Nhập kho')
            $scope.tabCheck = 'NK';
        if (item.LOAI_KH === 'Xuất kho')
            $scope.tabCheck = 'XK';
        if (item.LOAI_KH === 'Điều chuyển')
            $scope.tabCheck = 'DC';
        if (item.LOAI_KH === 'Giao thẳng')
            $scope.tabCheck = 'GT';

        $scope.selectedItem = index;
        $.ajax({
            type: 'post',
            url: '/TraCuuTinhHinhXuLyKeHoach/DanhSachChungTuTheoSoKeHoach',
            data: { soKH: item.SO_KH, loai: $scope.tabCheck},
            success: function (res) {
                console.log(res);
                if (res.Error) {
                    toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm!');
                } else {
                    $scope.DanhSachChungTu = res.data;
                    $scope.$apply();
                }
            }
        })
    }

    $scope.GetTrangThaiText = function (item) {
        var trangThaiText = '';
        if (item === 'I')
            trangThaiText = 'Khởi tạo';
        if (item === 'U')
            trangThaiText = 'Chờ duyệt';
        if (item === 'A')
            trangThaiText = 'Đã duyệt';
        if (item === 'R')
            trangThaiText = 'Từ chối duyệt';
        if (item === 'D')
            trangThaiText = 'Đã hủy';
        if (item === 'P')
            trangThaiText = 'Tạm dừng';
        if (item === 'C')
            trangThaiText = '';
        return trangThaiText;
    }

    $scope.XemChungTuNhapKho = function (item) {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/TraCuuTinhHinhXuLyKeHoach/_ChiTietChungTuNhapKho',
            controller: 'ChiTietChungTuNhapKho',
            size: 'xl',
            backdrop: 'show',
            resolve: {
                data: function () {
                    return {
                        IdPhieu: item.ID
                    };
                }
            }
        });

        //kết quả trả về của modal
        modalInstance.result.then(function () {
        });
    }
    $scope.XemChungTuXuatKho = function (item) {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/TraCuuTinhHinhXuLyKeHoach/_ChiTietChungTuXuatKho',
            controller: 'ChiTietChungTuXuatKho',
            size: 'xl',
            backdrop: 'show',
            resolve: {
                data: function () {
                    return {
                        IdPhieu: item.ID
                    };
                }
            }
        });

        //kết quả trả về của modal
        modalInstance.result.then(function () {
        });
    }
    $scope.XemChungTuDieuChuyen = function (item) {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/TraCuuTinhHinhXuLyKeHoach/_ChiTietChungTuDieuChuyen',
            controller: 'ChiTietChungTuDieuChuyen',
            size: 'xl',
            backdrop: 'show',
            resolve: {
                data: function () {
                    return {
                        IdPhieu: item.ID
                    };
                }
            }
        });

        //kết quả trả về của modal
        modalInstance.result.then(function () {
        });
    }
    $scope.XemChungTuGiaoThang = function (item) {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/TraCuuTinhHinhXuLyKeHoach/_ChiTietPhieuGiaoThang',
            controller: 'ChiTietChungTuGiaoThang',
            size: 'xl',
            backdrop: 'show',
            resolve: {
                data: function () {
                    return {
                        IdPhieu: item.ID
                    };
                }
            }
        });

        //kết quả trả về của modal
        modalInstance.result.then(function () {
        });
    }
});
app.controller("ChiTietChungTuNhapKho", function ($scope, $uibModal, $uibModalInstance, $ngConfirm, showToast, hideLoading, $location, data) {
    angular.element(document).ready(function () {
        $.ajax({
            type: 'post',
            url: '/TraCuuTinhHinhXuLyKeHoach/LayChiTietPhieuNhap',
            data: { idPhieu: data.IdPhieu },
            success: function (res) {
                if (res.Error) {
                    toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm!');
                } else {
                    $scope.ChiTietPhieuNhap = res.data;
                    $scope.DanhSachHangHoaChiTiet = res.hangHoa;
                    $scope.DanhSachHangHoa = [...new Map(res.hangHoa.map(item => [item['ID_HANG'], item])).values()];
                    angular.forEach($scope.DanhSachHangHoa, function (val, key) {
                        $scope.DanhSachHangHoa[key].SO_LUONG_TT = $scope.DanhSachHangHoaChiTiet.filter(x => x.ID_HANG === val.ID_HANG).reduce((s, item) => s + parseInt(item.SO_LUONG_TT), 0);
                    });
                    $scope.idHangHoaDaChon = 0;
                    $scope.$apply();
                }
            }
        })
    });

    $scope.ChonHangNhapKho = function (item, index) {
        $scope.selectedItem = index;
        $scope.idHangHoaDaChon = item.ID_HANG;
        $scope.tenHangDaChon = item.TEN_HANG;
        $scope.soLuongDaChon = item.SO_LUONG_TT;
    }
  
    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});
app.controller("ChiTietChungTuXuatKho", function ($scope, $uibModal, $uibModalInstance, $ngConfirm, showToast, hideLoading, $location, data) {
    angular.element(document).ready(function () {
        $.ajax({
            type: 'post',
            url: '/TraCuuTinhHinhXuLyKeHoach/LayChiTietPhieuXuat',
            data: { idPhieu: data.IdPhieu },
            success: function (res) {
                if (res.Error) {
                    toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm!');
                } else {
                    $scope.ChiTietPhieuXuat = res.data;
                    $scope.DanhSachHangHoaPhieuXuatChiTiet = res.hangHoa;
                    $scope.DanhSachHangHoaPhieuXuat = [...new Map(res.hangHoa.map(item => [item['ID_HANG'], item])).values()];
                    angular.forEach($scope.DanhSachHangHoaPhieuXuat, function (val, key) {
                        $scope.DanhSachHangHoaPhieuXuat[key].SO_LUONG_TT = $scope.DanhSachHangHoaPhieuXuatChiTiet.filter(x => x.ID_HANG === val.ID_HANG).reduce((s, item) => s + parseInt(item.SO_LUONG_TT), 0);
                    });
                    $scope.idHangHoaDaChon = 0;
                    $scope.$apply();
                }
            }
        })
    });

    $scope.ChonHangXuatKho = function (item, index) {
        $scope.selectedItem = index;
        $scope.idHangHoaDaChon = item.ID_HANG;
        $scope.tenHangDaChon = item.TEN_HANG;
        $scope.soLuongDaChon = item.SO_LUONG_TT;
    }
    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});
app.controller("ChiTietChungTuDieuChuyen", function ($scope, $uibModal, $uibModalInstance, $ngConfirm, showToast, hideLoading, $location, data) {
    angular.element(document).ready(function () {
        $.ajax({
            type: 'post',
            url: '/TraCuuTinhHinhXuLyKeHoach/LayChiTietDieuChuyen',
            data: { idPhieu: data.IdPhieu },
            success: function (res) {
                if (res.Error) {
                    toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm!');
                } else {
                    $scope.ChiTietPhieuDieuChuyen = res.data;
                    $scope.DanhSachHangHoaDieuChuyenChiTiet = res.hangHoa;
                    $scope.DanhSachHangHoaDieuChuyen = [...new Map(res.hangHoa.map(item => [item['ID_HANG'], item])).values()];
                    angular.forEach($scope.DanhSachHangHoaDieuChuyen, function (val, key) {
                        $scope.DanhSachHangHoaDieuChuyen[key].SO_LUONG_TT = $scope.DanhSachHangHoaDieuChuyenChiTiet.filter(x => x.ID_HANG === val.ID_HANG).reduce((s, item) => s + parseInt(item.SO_LUONG_TT), 0);
                    });
                    $scope.idHangHoaDaChon = 0;
                    $scope.$apply();
                }
            }
        })
    });

    $scope.ChonHangDieuChuyen = function (item, index) {
        $scope.selectedItem = index;
        $scope.idHangHoaDaChon = item.ID_HANG;
        $scope.tenHangDaChon = item.TEN_HANG;
        $scope.soLuongDaChon = item.SO_LUONG_TT;
    }
    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});

app.controller("ChiTietChungTuGiaoThang", function ($scope, $uibModal, $uibModalInstance, $ngConfirm, showToast, hideLoading, $location, data) {
    $scope.ChiTietGiaoThangNhap = [];
    angular.element(document).ready(function () {
        $.ajax({
            type: 'post',
            url: '/TraCuuTinhHinhXuLyKeHoach/GetDanhMucChung',
            success: function (res) {
                if (res.Error) {
                    toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm!');
                } else {
                    $scope
                }
            }
        })
        $.ajax({
            type: 'post',
            url: '/TraCuuTinhHinhXuLyKeHoach/LayChiTietGiaoThang',
            data: { idPhieu: data.IdPhieu },
            success: function (res) {
                if (res.Error) {
                    toastr.error('Có lỗi xảy ra trong quá trình tìm kiếm!');
                } else {
                    $scope.ChiTietGiaoThang = res.data;
                    $scope.DanhSachHangHoaGiaoThangChiTiet = res.hangHoa;
                    $scope.DanhSachHangHoaGiaoThang = [...new Map(res.hangHoa.map(item => [item['ID_HANG'], item])).values()];
                    angular.forEach($scope.DanhSachHangHoaGiaoThang, function (val, key) {
                        $scope.DanhSachHangHoaGiaoThang[key].SO_LUONG_TT = $scope.DanhSachHangHoaGiaoThangChiTiet.filter(x => x.ID_HANG === val.ID_HANG).reduce((s, item) => s + parseInt(item.SO_LUONG_TT), 0);
                    });
                    $scope.idHangHoaDaChon = 0;
                    $scope.$apply();

                }
            }
        })
    });

    $scope.ChonHangGiaoThang = function (item, index) {
        $scope.selectedItem = index;
        $scope.idHangHoaDaChon = item.ID_HANG;
        $scope.tenHangDaChon = item.TEN_HANG;
        $scope.soLuongDaChon = item.SO_LUONG_TT;
    }

    $scope.capNhatGiaoThang = function () {
        var fileUpload = $("#file-input").get(0);
        var files = fileUpload.files;
        var fileData = new FormData();
        fileData.append(files[0].name, files[0]);
        fileData.append('NGAY_NHAN_DK', $scope.ChiTietGiaoThangNhap.NGAY_NHAN_DK);
        fileData.append('CAN_BO_NHAN_HANG', $scope.ChiTietGiaoThangNhap.CAN_BO_NHAN_HANG);
        fileData.append('LANH_DAO_DON_VI_NHAN_HANG', $scope.ChiTietGiaoThangNhap.LANH_DAO_DON_VI_NHAN_HANG);
        fileData.append('CAN_BO_VAN_CHUYEN', $scope.ChiTietGiaoThangNhap.CAN_BO_VAN_CHUYEN);
        fileData.append('CAN_BO_DON_VI_SAN_XUAT', $scope.ChiTietGiaoThangNhap.CAN_BO_DON_VI_SAN_XUAT);
        fileData.append('ID_PHIEU', data.IdPhieu);
        $.ajax({
            type: 'post',
            url: '/TraCuuTinhHinhXuLyKeHoach/CapNhatGiaoThang',
            data: { idPhieu: data.IdPhieu },
            success: function (res) {
                if (res.Error) {
                    toastr.error(res.Title);
                } else {
                    toastr.success(res.Title);
                }
            }
        })
    }

    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});