app.controller("TonKhoController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, chuyenGiaSangChu) {
    $scope.listKho = [];
    $scope.chkNVPK = false;
    $scope.hangDoi = {
        'ThoiDiem': new Date(), 'Kho': '', 'NhomDuLieu': '', 'NhomHangHoa': '', 'LoaiHangHoa': ''
    };
    var sdate = new Date();
    var edate = new Date();
    sdate.setDate(sdate.getDate());
    edate.setDate(edate.getDate());
    $scope.hangDoi = {
        'ThoiDiem': moment(edate).format('DD/MM/YYYY'),Kho: null, NhomDuLieu: null,NhomHangHoa: null,LoaiHangHoa:null
    };
    $scope.listNhomDL = null;
    $scope.listNhomDLP3 = [
        {'CODE': 'PHUONG_TIEN_BO', 'NAME': 'Phương tiện đường bộ và xăng dầu'}
    ];
    $scope.listNhomDLP4 = [
        {'CODE': 'PHUONG_TIEN_THUY', 'NAME': 'Phương tiện đường thuỷ'}
    ];
    $scope.listNhomDLP5 = [
        {'CODE': 'VTU_THIET_BI', 'NAME': 'Vật tư, thiết bị kỹ thuật nghiệp vụ'}
    ];
    $scope.listNhomDLP6 = [
        {'CODE': 'VU_KHI_VL_NO', 'NAME': 'Vũ khí, vật liệu nổ'}
    ];
    $scope.listNhomDLP7 = [
        {'CODE': 'CP_QUAN_TRANG', 'NAME': 'Quân trang'}
    ];
    $scope.listNhomDLAll = [
        {'CODE': 'PHUONG_TIEN_BO', 'NAME': 'Phương tiện đường bộ và xăng dầu'},
        {'CODE': 'PHUONG_TIEN_THUY', 'NAME': 'Phương tiện đường thuỷ'},
        {'CODE': 'VTU_THIET_BI', 'NAME': 'Vật tư, thiết bị kỹ thuật nghiệp vụ'},
        {'CODE': 'VU_KHI_VL_NO', 'NAME': 'Vũ khí, vật liệu nổ'},
        {'CODE': 'CP_QUAN_TRANG', 'NAME': 'Quân trang'}
    ];
    $scope.listLoaiHangHoa = [];
    $scope.listNhomHangHoa = [];
    angular.element(document).ready(function(){
        $scope.GetListKho();
        $scope.GetListNhomDuLieu();
        $scope.GetListNhomHangHoa();
    });
    //Get List kho
    $scope.GetListKho = ()=>{
        $.ajax({
            type: 'POST',
            url: '/TonKho/GetListKho',
            data: {},
            success:function(response){
                if(response.Status ===200){
                    $scope.listKho = response.data;
                }
                else{
                    toastr.error(response.Message);
                }
                $scope.GetListNhomDuLieu();
                $scope.$apply();
            }
        });
    }
    $scope.unitInfo = {};
    //Get list nhóm dữ liệu
    $scope.GetListNhomDuLieu = () =>{
        $.ajax({
            type: 'POST',
            url: '/TonKho/GetListNhomDuLieu',
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
                        $scope.GetListLoaiHangHoa();
                    }
                    else{
                        $scope.chkNVPK = true;
                        $scope.listNhomDL = $scope.listNhomDLAll;
                    }
                }
                else{
                    toastr.error(data.Title);
                }
                $scope.$apply();
            }
        })
    }
    $scope.GetListLoaiHangHoa = ()=>{
        $.ajax({
            type: 'POST',
            url: '/TonKho/GetListLoaiHangHoa',
            data: {},
            success:function(response){
                if(response.Status === 200){
                    $scope.listLoaiHangHoa = response.data;
                }
                else{
                    toastr.error(response.Message);
                }
                $scope.$apply();
            }
        });
    }
    $scope.GetListNhomHangHoa = ()=>{
        $.ajax({
            type: 'POST',
            url: '/TonKho/GetListNhomHangHoa',
            data:{idLoaiHang: $scope.hangDoi.LoaiHangHoa},
            success: function(data){
                
            }
        })
    }
});