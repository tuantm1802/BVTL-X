app.controller("TheKhoController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, chuyenGiaSangChu) {
    $scope.listKho = [];
    $scope.hangDoi = {
        'TuNgay': new Date(), 'DenNgay': new Date(), 'Kho': '', 'NhomDuLieu': '','NhomHangHoa':'','LoaiHangHoa':''
    };
    $scope.listNhomDL = null;
    $scope.listNhomDLP3 = [
        {'CODE': 'PHUONG_TIEN_BO','NAME': 'Phương tiện đường bộ và xăng dầu'}
    ];
    $scope.listNhomDLP4 = [
        {'CODE': 'PHUONG_TIEN_THUY','NAME': 'Phương tiện đường thuỷ'}
    ];
    $scope.listNhomDLP5 = [
        {'CODE': 'VTU_THIET_BI','NAME': 'Vật tư, thiết bị kỹ thuật nghiệp vụ' }
    ];
    $scope.listNhomDLP6 = [
        {'CODE': 'VU_KHI_VL_NO','NAME':'Vũ khí, vật liệu nổ'}
    ];
    $scope.listNhomDLP7 = [
        {'CODE': 'CP_QUAN_TRANG','NAME':'Quân trang'}
    ];
    $scope.listNhomDLAll = [
        {'CODE': 'PHUONG_TIEN_BO','NAME': 'Phương tiện đường bộ và xăng dầu'},
        {'CODE': 'PHUONG_TIEN_THUY','NAME': 'Phương tiện đường thuỷ'},
        {'CODE': 'VTU_THIET_BI','NAME': 'Vật tư, thiết bị kỹ thuật nghiệp vụ' },
        {'CODE': 'VU_KHI_VL_NO','NAME':'Vũ khí, vật liệu nổ'},
        {'CODE': 'CP_QUAN_TRANG','NAME':'Quân trang'}
    ];
    $scope.unitInfo = {};
    $scope.GetListNhomDuLieu = () =>{
        $.ajax({
            type: 'POST',
            url: '/TheKho/GetListNhomDuLieu',
            data: {},
            success: function(data){
                if(data.Status === 200){
                    $scope.unitInfo = data.data;
                    if($scope.unitInfo[0].UNIT_CODE ==="P3") {
                        $scope.listNhomDL = $scope.listNhomDLP3;
                        $scope.hangDoi.NhomDuLieu = $scope.listNhomDLP3[0].CODE;
                    } else if($scope.unitInfo[0].UNIT_CODE ==="P4") {
                        $scope.listNhomDL = $scope.listNhomDLP4;
                        $scope.hangDoi.NhomDuLieu = $scope.listNhomDLP4[0].CODE;
                    }
                    else if($scope.unitInfo[0].UNIT_CODE ==="P5") {
                        $scope.listNhomDL = $scope.listNhomDLP5;
                        $scope.hangDoi.NhomDuLieu = $scope.listNhomDLP5[0].CODE;
                    }
                    else if($scope.unitInfo[0].UNIT_CODE ==="P6"){
                        $scope.listNhomDL = $scope.listNhomDLP6;
                        $scope.hangDoi.NhomDuLieu = $scope.listNhomDLP6[0].CODE;
                    }
                    else if($scope.unitInfo[0].UNIT_CODE ==="P7") {
                        $scope.listNhomDL = $scope.listNhomDLP7;
                        $scope.hangDoi.NhomDuLieu = $scope.listNhomDLP7[0].CODE;
                        console.log($scope.hangDoi.NhomDuLieu);
                    }
                    else{
                        $scope.chkNVPK = true;
                        $scope.listNhomDL = $scope.listNhomDLAll;
                    }
                }
                else{
                    toastr.error(data.Title);
                }
                console.log($scope.listNhomDL);
                console.log($scope.hangDoi.NhomDuLieu);
                $scope.$apply();
            }
        })
    }
    $scope.chkNVPK =false;
    $scope.currentPage = 0;
    $scope.dsKetQua = [];
    $scope.thongTinKho = {};
    $scope.hangDoi = {
        'TuNgay': new Date(), 'DenNgay': new Date(), 'Kho': '', 'NhomDuLieu': '','MaHangHoa':'','TenHangHoa':''
    };
    DefaultSettings = () => {
        $scope.dsTheKho = [];
        $scope.thongTinKho = {};
        $('#btnAdd').prop('disabled', true);
        $('#btnEdit').prop('disabled', true);
        $('#btnSave').prop('disabled', true);
        $('#btnPrint').prop('disabled', true);
        $('#btnThemHangHoa').prop('disabled', true);
        $scope.CoDinhKem1 = false;
        $scope.CoDinhKem2 = false;
        $scope.CoDinhKem3 = false;
        $scope.DuDauKy = 0;
        $scope.DuCuoiKy = 0;
    }
    $scope.pageSize = 10;
    $scope.currentPage = 1;
    $scope.SoLuongKetQua = 0;
    angular.element(document).ready(function(){
        $scope.GetListKho();
        $scope.GetListNhomDuLieu();
    });
    $scope.GetListKho = () =>{
        $.ajax({
            type: 'POST',
            url: '/TheKho/GetListKho',
            data: {},
            success: function(response){
                if(response.Status === 200){
                    $scope.listKho = response.data;
                }
                else{
                    toastr.error(response.Message);
                }
                console.log(response);
            }
        });
    }
    DefaultSettings();
    var sdate = new Date();
    var edate = new Date();
    sdate.setDate(sdate.getDate());
    edate.setDate(edate.getDate());
    $scope.hangDoi = {
        'TuNgay': moment(sdate).format('DD/MM/YYYY'), 'DenNgay': moment(edate).format('DD/MM/YYYY'),Kho: null, NhomDuLieu: null,MaHangHoa:'',TenHangHoa:''
    }
    $scope.TimKiem = () => {
        if(($scope.hangDoi.MaHangHoa === '' ||$scope.hangDoi.MaHangHoa === null) &&($scope.hangDoi.TenHangHoa === '' || $scope.hangDoi.TenHangHoa === null)) {
            toastr.error("Cần nhập mã hàng hoá hoặc tên hàng hoá để tìm kiếm");
            return;
        }
        else if($scope.hangDoi.Kho === null || $scope.hangDoi.Kho === ''){
            toastr.error("Chưa chọn kho hàng hoá");
            return;
        }
        else {
            $.ajax({
                type: 'POST',
                url: '/TheKho/GetListHangHoa',
                data: {
                    ID_KHO: $scope.hangDoi.Kho,
                    APP_CODE: $scope.hangDoi.NhomDuLieu,
                    CODE: $scope.hangDoi.MaHangHoa,
                    NAME: $scope.hangDoi.TenHangHoa,
                    sDate: $scope.hangDoi.TuNgay,
                    eDate: $scope.hangDoi.DenNgay
                },
                success: function (response) {
                    if (response.Status === 200) {
                        $scope.dsKetQua = response.data;
                    } else {
                        toastr.error(response.Title);
                    }
                    console.log(response);
                    $scope.$apply();
                }
            });
        }
    }
    $scope.ShowThongTin = (item, index) => {
        $scope.selectedRow = index;
        $scope.TongTang = 0;
        $scope.TongGiam = 0;
        $scope.dsTheKho = [];
        $scope.DuDauKy = 0;
        $scope.DuCuoiKy = 0;
        $.ajax({
            type:'POST',
            url: '/TheKho/GetDuDauKy',
            data: {
                APP_CODE:$scope.hangDoi.NhomDuLieu,
                ID_KHO: $scope.hangDoi.Kho,
                ID_HANG: item.ID_HANG,
                StartDate:$scope.hangDoi.TuNgay
            },
            success: function(data){
                if(data.Status === 200){
                    $scope.DuDauKy = data.data[0].SO_LUONG_TON;
                }
                $scope.$apply();
            }
        });
        $.ajax({
            type: 'post',
            url: '/TheKho/GetTheKho',
            data: {
                APP_CODE:$scope.hangDoi.NhomDuLieu,
                ID_KHO: $scope.hangDoi.Kho,
                ID_HANG: item.ID_HANG,
                StartDate:$scope.hangDoi.TuNgay,
                EndDate:$scope.hangDoi.DenNgay
            },
            success: function (response) {
                if (response.Status === 200) {
                    $scope.dsTheKho = response.Data;
                    $scope.dsTheKho.forEach(x => x.NGAY_CT_VW = moment(x.NGAY_CT, 'YYYYMMDD').format('DD/MM/YYYY'));
                    var a = $scope.dsTheKho.filter(x => x.SL_TANG > 0);
                    $scope.TongTang = $scope.dsTheKho.filter(x => x.SL_TANG > 0).sum("SL_TANG");
                    $scope.TongGiam = $scope.dsTheKho.filter(x => x.SL_GIAM > 0).sum("SL_GIAM");
                    $scope.DuCuoiKy = $scope.DuDauKy + $scope.TongTang - $scope.TongGiam;
                    //$scope.dsTongHop.forEach(x => x.DVT = "C");
                }
                else {
                    toastr.error(response.Message);
                }
                console.log(response);
                $scope.DuCuoiKy = $scope.DuDauKy + $scope.TongTang - $scope.TongGiam;
                $scope.$apply();
            }
        });
    }
});

