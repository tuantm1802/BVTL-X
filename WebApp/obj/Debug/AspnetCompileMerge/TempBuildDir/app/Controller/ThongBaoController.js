app.controller("ThongBaoController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1; 
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.recordPerPage = 10; 
    $scope.modelSearch.loaithongbao = '0';

    angular.element(document).ready(function () {
        showToast(); 
        $scope.LoadPage();
    });
       
    $scope.LoadPage = function () {
        $.ajax({
            type: 'post',
            url: '/ThongBao/DsThongBaoPaging',
            data: $scope.modelSearch,
            success: function (data) {
                $scope.modelSearch.totalItems = data.total;
                $scope.ListThongBao = data.data;                
                $scope.$apply();
                hideLoading();
            }
        });
    }; 
    $scope.deleteThongBaoListIds = function () {
        var listIds = [];
        angular.forEach($scope.ListThongBao, function (data) {
            if (data.SELECT == true) {
                listIds.push(data.ID);
            }
        })
        if (listIds.length > 0) {
            showToast();
            $.ajax({
                type: 'post',
                url: '/ThongBao/XoaThongBaoTheoId',
                data: { lstIds: listIds },
                success: function (data) {
                    if (!data.Error) {
                        $scope.LoadPage();
                        angular.element(document.getElementById('thongbao-container')).scope().LoadThongBaoHeader();
                        toastr.success(data.Title);
                    } else {
                        toastr.error("Không thể xoá được thông báo. Vui lòng liên hệ quản trị hệ thống!");
                    }
                    hideLoading();
                }
            });
        }
    };
    $scope.deleteThongBaoListId = function (id) {
        var listIds = [];
        listIds.push(id);
        if (listIds.length > 0) {
            showToast();
            $.ajax({
                type: 'post',
                url: '/ThongBao/XoaThongBaoTheoId',
                data: { lstIds: listIds },
                success: function (data) {
                    if (!data.Error) {
                        $scope.LoadPage();
                        angular.element(document.getElementById('thongbao-container')).scope().LoadThongBaoHeader();
                        toastr.success(data.Title);
                    } else {
                        toastr.error("Không thể xoá được thông báo. Vui lòng liên hệ quản trị hệ thống!");
                    }
                    hideLoading();
                }
            });
        }
    };
    $scope.readThongBaoListIds = function () {
        var listIds = [];
        angular.forEach($scope.ListThongBao, function (data) {
            if (data.SELECT == true) {
                listIds.push(data.ID);
            }
        })
        if (listIds.length>0) {
            showToast();
            $.ajax({
                type: 'post',
                url: '/ThongBao/CapNhatTrangThai',
                data: { lstIds: listIds },
                success: function (data) {
                    if (!data.Error) {
                        $scope.LoadPage();
                        angular.element(document.getElementById('thongbao-container')).scope().LoadThongBaoHeader();
                        toastr.success(data.Title);
                    } else {
                        toastr.error("Không thể cập nhật được trạng thái đọc thông báo. Vui lòng liên hệ quản trị hệ thống!");
                    }
                    hideLoading();
                }
            });
        }
    }; 
    $scope.readThongBaoListId = function (id) {
        var listIds = [];
        listIds.push(id);
        if (listIds.length > 0) {
            showToast();
            $.ajax({
                type: 'post',
                url: '/ThongBao/CapNhatTrangThai',
                data: { lstIds: listIds },
                success: function (data) {
                    if (!data.Error) {
                        $scope.LoadPage();
                        angular.element(document.getElementById('thongbao-container')).scope().LoadThongBaoHeader();
                        toastr.success(data.Title);
                    } else {
                        toastr.error("Không thể cập nhật được trạng thái đọc thông báo. Vui lòng liên hệ quản trị hệ thống!");
                    }
                    hideLoading();
                }
            });
        }
    }; 
    $scope.checkAll = function () {
        angular.forEach($scope.ListThongBao, function (item) {
            item.SELECT = event.target.checked;
        });
    };
    $scope.fdate = function (date) {
        return moment(date).format('DD/MM/YYYY HH:mm');
    }
    $scope.fontbold = function (trangthai) {
        return trangthai != 'Y' ? 'fontbold' : '';
    }
});