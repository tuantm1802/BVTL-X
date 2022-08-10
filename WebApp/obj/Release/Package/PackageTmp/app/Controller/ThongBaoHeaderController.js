app.controller("ThongBaoHeaderController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    
    $scope.TotalThongBao = 0;  
    $scope.ListThongBao = [];

    angular.element(document).ready(function () {
        $scope.IsVisibleNf = false;
        $scope.LoadThongBaoHeader();
    });
        
    $scope.LoadThongBaoHeader = function () {
        $.ajax({
            type: 'post', 
            url: '/ThongBao/ThongBao_Header',
            data: {},
            success: function (data) { 
                if (!data.Error) {
                    if (data.total > 0)
                        $scope.IsVisibleNf = true;
                    else
                        $scope.IsVisibleNf = false;
                    $scope.TotalThongBao = data.total;
                    $scope.ListThongBao = data.data;
                    $scope.$apply();
                } 
            }
        });
    }; 
    $scope.fdate = function (date) {
        return moment(date).format('DD/MM/YYYY HH:mm');
    }
    $scope.CapNhatTrangThaiTB = function (item) {
        var lstIds = [];
        lstIds.push(item.ID);
        $.ajax({
            type: 'post',
            url: '/ThongBao/CapNhatTrangThai',
            data: { lstIds: lstIds},
            success: function (data) {
                $scope.LoadThongBaoHeader();
            }
        });
    };
    $scope.fontbold = function (trangthai) {
        return trangthai != 'Y' ? 'fontbold' : '';
    }
});