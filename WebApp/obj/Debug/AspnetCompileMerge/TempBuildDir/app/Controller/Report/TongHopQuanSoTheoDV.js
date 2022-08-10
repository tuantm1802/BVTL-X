app.controller("TongHopQuanSoTheoDVController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $location) {
    $scope.select2Options = {
        data: [],
    };
    $scope.getDonVi = function () {
        $.ajax({
            type: 'POST',
            url: '/TongHopQuanSoTheoDV/GetDonViSelect2Tree',
            async: false,
            success: function (data) {
                angular.element("#slDonVi").select2({
                    data: data
                });
                $("#slDonVi").val($scope.modelSearch.donvi).change();
                $scope.$apply();
            }
        });
    }
    
    angular.element(document).ready(function () {
        $scope.getDonVi();
    });

    $scope.modelSearch = {
        donvi: $('#txtdonvi').val(),
        ngaybaocao: $('#txtngaybaocao').val(),
        socongvan: $('#txtsocongvan').val(),
        type:''
    }
    $scope.XemBC = function () {
        showToast();
        $("#containerReportViewer").load("/TongHopQuanSoTheoDV/GetChiTietLoạiBaoCao?type=" + $scope.modelSearch.type + "&donvi=" + $("#slDonVi").val() + '&socongvan=' + $scope.modelSearch.socongvan + '&ngaybaocao=' + moment($scope.modelSearch.ngaybaocao, 'DD/MM/YYYY').format('YYYY-MM-DD'), function () {
            hideLoading();
        });
    }

});