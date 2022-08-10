app.controller("CPQuanTrangTongHopController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $location) {
    $scope.modelSearch = {
        tu_ngay: '',
        den_ngay: ''
    }
    $scope.XemBC = function () {
        showToast();
        $('#containerReportViewer').load('/CPQuanTrangTongHop/ChiTietBaoCao?tu_ngay=' + moment($scope.modelSearch.tu_ngay, 'DD/MM/YYYY').format('YYYY-MM-DD') + '&den_ngay=' + moment($scope.modelSearch.den_ngay, 'DD/MM/YYYY').format('YYYY-MM-DD'), function () {
            hideLoading();
        });

       // window.location.href = '/CPQuanTrangTongHop/Index?tu_ngay=' + moment($scope.modelSearch.tu_ngay, 'DD/MM/YYYY').format('YYYY-MM-DD') + '&den_ngay=' + moment($scope.modelSearch.den_ngay, 'DD/MM/YYYY').format('YYYY-MM-DD');
    }

});
