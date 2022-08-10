app.controller("DuyetKeHoachController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $location, $rootScope, constant) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
     
    $scope.DsLoaiKeHoach = [
        { code: '', name: 'Tất cả', Selected: true },
        { code: 'CP', name: 'Cấp phát' },
        { code: 'XK', name: 'Xuất kho' },
        { code: 'NK', name: 'Nhập kho' },
        { code: 'GT', name: 'Giao thẳng' },
        { code: 'DC', name: 'Điều chuyển' }
    ]; 
    $scope.cbbLoaiKeHoach = $scope.DsLoaiKeHoach[0];

    $scope.ListTrangThai = [
        { code: '', name: 'Tất cả', Selected: true },
        { code: 'U', name: 'Chờ duyệt' },
        { code: 'A', name: 'Đã duyệt' },
        { code: 'R', name: 'Từ chối duyệt' }
    ];
    $scope.cbbTrangThai = $scope.ListTrangThai[0];

    function getSearchParams(k) {
        var p = {};
        location.search.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (s, k, v) { p[k] = v })
        return k ? p[k] : p;
    }
    angular.element(document).ready(function () {
        
    });
    //#region ///////////////////////// Danh sách kế hoạch /////////////////////////////////////////////////
    $scope.DanhSachDuyetKH = [];
    $scope.LoadPage = function () { 
        showToast();
        $scope.starDate = '';
        $scope.endDate = '';
        if ($scope.TU_NGAY != undefined) {
            $scope.starDate = moment($scope.TU_NGAY, 'DD/MM/YYYY').format('MM/DD/YYYY');
        } 
        if ($scope.DEN_NGAY != undefined) {
            $scope.endDate = moment($scope.DEN_NGAY, 'DD/MM/YYYY').format('MM/DD/YYYY');
        }
            
        $.ajax({
            type: 'post',
            url: '/DuyetKeHoach/DuyetKeHoachDanhSach',
            data: {
                currentPage: $scope.modelSearch.currentPage,
                recordPerPage: $scope.modelSearch.pageSize,
                soKh: $scope.SO_KH,
                tuNgay: $scope.starDate,
                denNgay: $scope.endDate,
                loaiKh: $scope.cbbLoaiKeHoach.code,
                trangThai: $scope.cbbTrangThai.code
            },
            success: function (response) {
                if (response.Error) {
                    toastr.error(response.Title);
                }
                else {
                    
                    $scope.DanhSachDuyetKH = response.data;
                    $scope.modelSearch.totalItems = response.total;
                    $scope.$apply(); 
                }
                hideLoading();
            }
        });
    }
    $scope.LoadPage(); 
    $scope.pageChanged = function () {
        $scope.LoadPage();
    }
    $scope.btnSearchKeHoach = function () {
        $scope.LoadPage();
    }
    $scope.checkAll = function () {
        angular.forEach($scope.DanhSachDuyetKH, function (item) {
            item.Selected = event.target.checked;
        });
    };

    $scope.btnPheDuyetKeHoach = function () {
        $scope.ListKeHoach = [];
         
        angular.forEach($scope.DanhSachDuyetKH, function (model) {
            if (!!model.Selected) {  
                if (model.TRANG_THAI === 'U') {
                    $scope.itemModel = {};
                    $scope.itemModel.ID_KH = model.ID_KH;
                    $scope.itemModel.LOAI_KH = model.LOAI_KH;
                    $scope.ListKeHoach.push($scope.itemModel);
                } 
            } 
        })
        if ($scope.ListKeHoach.length > 0) {
            var url = '/DuyetKeHoach/DuyetKeHoach';
            //if (type === 'N') {
            //    url = '/DuyetKeHoach/TuchoiDuyetKeHoach';
            //}
            $.ajax({
                type: 'post',
                url: url,
                data: { listKeHoach: $scope.ListKeHoach },
                success: function (data) {
                    if (data.Error)
                        toastr.error(data.Title);
                    else {
                        toastr.success(data.Title);
                        $scope.LoadPage();
                    }
                }
            })  
        }
        else {
            toastr.error("Xin vui lòng chọn kế hoạch để xử lý!");
        }
    }
    $scope.btnTuChoiDuyetKH = function () {
      
        var dataSubmit = $scope.DanhSachDuyetKH.find(x => x.Selected === true);

        if (dataSubmit !== undefined) {
            $('#box_ykien').modal('show');
        } else {
            toastr.error("Xin vui lòng chọn kế hoạch để xử lý!");
        } 
    }
    $scope.btnTuChoiKH = function () {
        $scope.ListKeHoachTC = [];
        if ($scope.txtLyDo.length <= 0) {
            toastr.error("Bạn chưa nhập lý do!");
        }
        else {
            angular.forEach($scope.DanhSachDuyetKH, function (model) {
                if (!!model.Selected) { 
                    if (model.TRANG_THAI === 'U') {
                        $scope.itemModel = {};
                        $scope.itemModel.ID_KH = model.ID_KH;
                        $scope.itemModel.LOAI_KH = model.LOAI_KH;
                        $scope.ListKeHoachTC.push($scope.itemModel);
                    }  
                }
            })
            if ($scope.ListKeHoachTC.length > 0) {
                var url = '/DuyetKeHoach/TuchoiDuyetKeHoach';
                $.ajax({
                    type: 'post',
                    url: url,
                    data: { listKeHoach: $scope.ListKeHoachTC, lyDo: $scope.txtLyDo },
                    success: function (data) {
                        if (data.Error)
                            toastr.error(data.Title);
                        else {
                            toastr.success(data.Title);
                            $scope.LoadPage();
                            $('#box_ykien').modal('hide');
                        }
                    }
                })
            }
            else {
                toastr.error("Xin vui lòng chọn kế hoạch để xử lý!");
            }
        } 
    }
    $scope.XemChiTietKh = function (item) {        
        showToast() 
        $scope.Loai = item.LOAI_KH.trim();
        if ($scope.Loai == "KH" || $scope.Loai == "BX" || $scope.Loai == "UBX") {
            $scope.InphieuCapPhat(item);
        }
        else if ($scope.Loai == "HD-VTSX-1" || $scope.Loai == "HD-KHD-2" || $scope.Loai == "HD-VTSX-3" || $scope.Loai == "HD-VTSX-4" || $scope.Loai == "HD-VTSX-5") {
            $scope.InphieuGiaoVtChinh(item);
        }
        else if ($scope.Loai == "HD-NKB-TRA" || $scope.Loai == "HD-NKB-NHAP") {
            $scope.InphieuNhapKho(item);
        }
        else if ($scope.Loai == "KHO-KHDC") {
            $scope.InphieuKhDieuChuyen(item);
        } 
    }
    $scope.InphieuCapPhat = function (keHoach) {        
        if (keHoach.ID_KH > 0) {
            hideLoading();
            window.location.href = '/KeHoachCapPhat/ExportGiaiTrinhKeHoach?soKeHoach=' + keHoach.SO_KH;
        }
        else {
            hideLoading();
            toastr.error("Vui lòng chọn kế hoạch!");
        }         
    }
    $scope.InphieuGiaoVtChinh = function (keHoach) { 
        $scope.Loai = keHoach.LOAI_KH.trim();
        var type = 1;
        var id = keHoach.ID_KH;
        switch ($scope.Loai) {
            case 'HD-VTSX-1': 
                type = 1;
                break;
            case 'HD-KHD-2':
                type = 2;
                break;
            case 'HD-VTSX-3':
                type = 3;
                break;
            case 'HD-VTSX-4':
                type = 4;
                break;
            case 'HD-VTSX-5':
                type = 5;
                break;
            default:
                break;
        } 
        $.ajax({
            url: '/QuanLyHopDong/InPhieuVatTuChinh',
            type: 'post',
            data: {
                id: id, type: type
            },
            success: function (result) {
                hideLoading();
                let pdfWindow = window.open("");
                pdfWindow.document.write("<iframe width='100%' height='100%' src='data:application/pdf;base64," + result + "'></iframe>");

            },
            error: function (xhr, status, err) {
                alert(err);
                hideLoading();
            }
        });
    }
    $scope.InphieuNhapKho = function (keHoach) { 
        $.ajax({
            url: '/QuanLyHopDong/InPhieuNhapXuatKho',
            type: 'post',
            data: {
                id: keHoach.ID_KH
            },
            success: function (result) {
                hideLoading();
                let pdfWindow = window.open("");
                pdfWindow.document.write("<iframe width='100%' height='100%' src='data:application/pdf;base64," + result + "'></iframe>");
            },
            error: function (xhr, status, err) {
                hideLoading();
            }
        });
    }
    $scope.InphieuKhDieuChuyen = function (keHoach) { 
        $.ajax({
            url: '/KeHoachDieuChuyen/ExportToPDF',
            type: 'post',
            data: {
                idPhieu: keHoach.ID_KH
            },
            success: function (result) {

                let pdfWindow = window.open("");
                pdfWindow.document.write("<iframe width='100%' height='100%' src='data:application/pdf;base64," + result + "'></iframe>");

            },
            error: function (xhr, status, err) {
                alert(err);
            }
        });
    }
    //#endregion ////////////////////////////////////////////////////////////////////////////////////////////
}); 