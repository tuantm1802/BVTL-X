app.controller("BaoCaoNamController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "ParamCode DESC";
    $scope.ListYear = [];
    $scope.ListCity = [];
    $scope.ListCityCode = [];
    $scope.ListMaNhomTBH = [];
    $scope.ListNhomTBH = [];
   
    $scope.ParamIdSeleted = 0;
    angular.element(document).ready(function () {
        $scope.ListMaNhomTBH = [];
    $scope.ListNhomTBH = [];
        var date = new Date();
        for (var i = date.getFullYear() - 5; i < date.getFullYear() + 5; i++) {
            var tmpYear = {
                Id: i,
                Name: i + ''
            };
            $scope.ListYear.push(tmpYear);
        }
        $scope.modelSearch.Year = date.getFullYear();
    GetBottomAction();
    $scope.Changecity();
        $scope.LoadPage(1);
    });

    $scope.RoleBtnUpdate = false;
    $scope.RoleBtnSearch = false;

    function GetBottomAction() {
        $scope.ListCity = [];
        $.ajax({
            type: 'post',
            url: '/BaoCaoNam/GetBottomAction',
            data: {},
            success: function (response) {
                if (response.Buttoms != null) {
                    angular.forEach(response.Buttoms, function (item) {
                        if (item == 'btnUpdate') {
                            $scope.RoleBtnUpdate = true;
                        }
                        if (item == 'btnSearch') {
                            $scope.RoleBtnSearch = true;
                        }
                    });
                }

                $scope.ListCity = response.Citys;
                $scope.$apply();
            }
        });
    }


    $scope.LoadPage = function (genTable) {
        if ($scope.modelSearch.Year == null || $scope.modelSearch.Year == 0) {
            toastr.error("Vui lòng chọn năm!");
            return;
        }
        $scope.modelSearch.CityCodes = '';
        if ($scope.ListCityCode != null && $scope.ListCityCode.length > 0) {
            for (var i = 0; i < $scope.ListCityCode.length; i++) {
                if ($scope.modelSearch.CityCodes == null || $scope.modelSearch.CityCodes == '') {
                    $scope.modelSearch.CityCodes = $scope.ListCityCode[i];
                } else {
                    $scope.modelSearch.CityCodes += ',' + $scope.ListCityCode[i];
                }
            }
            //$scope.modelSearch.CityCodes = $scope.ListCityCode.map(function (obj) { return obj.Code }).join(',');
        }

        $scope.modelSearch.MaNhomTBH = '';
        if ($scope.ListMaNhomTBH != null && $scope.ListMaNhomTBH.length > 0) {
            for (var i = 0; i < $scope.ListMaNhomTBH.length; i++) {
                if ($scope.modelSearch.MaNhomTBH == null || $scope.modelSearch.MaNhomTBH == '') {
                    $scope.modelSearch.MaNhomTBH = $scope.ListMaNhomTBH[i];
                } else {
                    $scope.modelSearch.MaNhomTBH += ',' + $scope.ListMaNhomTBH[i];
                }
            }
        }


        showToast();
        $scope.ListData = [];
        $.ajax({
            type: 'post',
            url: '/BaoCaoNam/SearchData',
            cache: false,
            async: false,
            data: $scope.modelSearch,
            success: function (respone) {
                $scope.ListData = respone.data;
            }
        });
        
        hideLoading();
    };

    $scope.Refesh = function () {
        $scope.LoadPage(0);
    };

    $scope.ExportExcel = function () {
        if ($scope.modelSearch.Year == null || $scope.modelSearch.Year == 0) {
            toastr.error("Vui lòng chọn năm!");
            return;
        }
        $scope.modelSearch.CityCodes = '';
        if ($scope.ListCityCode != null && $scope.ListCityCode.length > 0) {
            for (var i = 0; i < $scope.ListCityCode.length; i++) {
                if ($scope.modelSearch.CityCodes == null || $scope.modelSearch.CityCodes == '') {
                    $scope.modelSearch.CityCodes = $scope.ListCityCode[i];
                } else {
                    $scope.modelSearch.CityCodes += ',' + $scope.ListCityCode[i];
                }
            }
        }

        $scope.modelSearch.MaNhomTBH = '';
        if ($scope.ListMaNhomTBH != null && $scope.ListMaNhomTBH.length > 0) {
            for (var i = 0; i < $scope.ListMaNhomTBH.length; i++) {
                if ($scope.modelSearch.MaNhomTBH == null || $scope.modelSearch.MaNhomTBH == '') {
                    $scope.modelSearch.MaNhomTBH = $scope.ListMaNhomTBH[i];
                } else {
                    $scope.modelSearch.MaNhomTBH += ',' + $scope.ListMaNhomTBH[i];
                }
            }
        }

        window.location.href = '/BaoCaoNam/ExportData?Year=' + $scope.modelSearch.Year
            + '&CityCodes=' + ($scope.modelSearch.CityCodes == undefined ? '' : $scope.modelSearch.CityCodes)
            + '&maNhomTBHs=' + ($scope.modelSearch.MaNhomTBH == undefined ? '' : $scope.modelSearch.MaNhomTBH);
}

// Lấy danh sách Nhóm TBH theo tỉnh
$scope.Changecity = function () {

    var CityCodes = '';
    if ($scope.ListCityCode != null && $scope.ListCityCode.length > 0) {
        for (var i = 0; i < $scope.ListCityCode.length; i++) {
            if (CityCodes == null || CityCodes == '') {
                CityCodes = $scope.ListCityCode[i];
            } else {
                CityCodes += ',' + $scope.ListCityCode[i];
            }
        }
    }
    $scope.ListNhomTBH = [];
    $scope.ListMaNhomTBH = [];
    $.ajax({
        type: 'post',
        url: '/BaoCaoNam/GetNhomTBHByCityCodes',
        cache: false,
        async: false,
        data: {
            CityCodes: CityCodes
        },
        success: function (respone) {
            $scope.ListNhomTBH = respone.NhomTBHs;
        }
    });

};
});
