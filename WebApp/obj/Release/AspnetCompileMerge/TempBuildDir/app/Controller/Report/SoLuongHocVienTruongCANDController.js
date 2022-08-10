app.controller("SoLuongHocVienTruongCANDController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $location)
{
    $scope.select2Options = {
        data: [],
    };
    $scope.getDonVi = function ()
    {

        $.ajax({
            type: 'post',
            url: '/SoLuongHocVienTruongCAND/GetDonViByNhomDonVi',
            data: {},
            success: function (data)
            {
                if (data.Error == false)
                {
                    $scope.listDonvi = data.listDonvi;
                }

                $scope.$apply();
            },
            error: function (xhr, status, error)
            {
                toastr.error("Lỗi lấy danh sách");
            }
        });
    }

    angular.element(document).ready(function ()
    {
        $scope.getDonVi();
        showToast();
        $("#containerReportViewer").load("/SoLuongHocVienTruongCAND/GetBaoCaoSLHV?donvi=0&socongvan=" + $scope.modelSearch.socongvan + '&ngaybaocao=' + moment($scope.modelSearch.ngaybaocao, 'DD/MM/YYYY').format('YYYY-MM-DD'), function ()
        {
            hideLoading();
        });
    });

    $scope.modelSearch = {
        ngaybaocao: $('#txtngaybaocao').val(),
        socongvan: $('#txtsocongvan').val()
    }
    $scope.XemBC = function ()
    {
        showToast();
        $("#containerReportViewer").load("/SoLuongHocVienTruongCAND/GetBaoCaoSLHV?donvi=0&socongvan=" + $scope.modelSearch.socongvan + '&ngaybaocao=' + moment($scope.modelSearch.ngaybaocao, 'DD/MM/YYYY').format('YYYY-MM-DD'), function ()
        {
            hideLoading();
        });
    }

});