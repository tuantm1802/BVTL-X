app.controller("tieuchuantheonamtruoccontroller", function ($scope, $ngConfirm, showToast, hideLoading) {

   
    $scope.thongbao_tcnt = '';
    $scope.namht = new Date().getFullYear();

    $scope.nhap = {
        NAM: $scope.namht,
        TTCB: 'Y',
        TCP: 'Y'
    }

    angular.element(document).ready(function () {
        $scope.nambh = [];
        $.ajax({
            type: 'post',
            url: '/TieuChuanPham/GetDanhMucTCNT',
            data: {},
            success: function (response) {
                $scope.nambh = response.nambh;
                $scope.$apply();
            }
        });
    });


    //$scope.nambh = [
    //    { ID: $scope.namht, NAME: $scope.namht },
    //    { ID: $scope.namht - 1, NAME: $scope.namht - 1 },
    //    { ID: $scope.namht - 2, NAME: $scope.namht - 2 },
    //    { ID: $scope.namht - 3, NAME: $scope.namht - 3 },
    //    { ID: $scope.namht - 4, NAME: $scope.namht - 4 },
    //    { ID: $scope.namht - 5, NAME: $scope.namht - 5 }
    //]

    $scope.submittctm = function () {
        $scope.thongbao_tcnt = '';
        if ($scope.nhap.NAM == '' || $scope.nhap.NAM == undefined) {
            toastr.error('Chưa chọn năm ban hành.');
            return;
        }
        if ($scope.nhap.TTCB == 'N' && $scope.nhap.TCP == 'N') {
            toastr.error('Phải chọn ít nhất 1 tiêu chuẩn.');
            return;
        }
        showToast();
        $.ajax({
            type: 'post',
            url: '/TieuChuanPham/themTcTheoNamTruoc',
            data: $scope.nhap,
            success: function (data) {
                $scope.thongbao_tcnt = data.Title;
                $('#modelTieuChuanTheoNam').modal('hide');
                $('#modelThongBaoTCNT').modal('show');
                //if (data.Error) {
                //    //toastr.error(data.Title);
                   
                //} else {
                    
                //    //toastr.success(data.Title);
                   
                //}

                hideLoading();
                $scope.$apply();
            }
        });
        //console.log($scope.nhap);
    }
   
});

