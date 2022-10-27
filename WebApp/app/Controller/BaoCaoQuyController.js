app.controller("BaoCaoQuyController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "ParamCode DESC";
    $scope.ListYear = [];
    $scope.ListCity = [];
    $scope.ListCityCode = [];
    $scope.Quy = "I";
    $scope.ParamIdSeleted = 0;
    angular.element(document).ready(function () {
        var date = new Date();
        for (var i = date.getFullYear() - 5; i < date.getFullYear() + 5; i++) {
            var tmpYear = {
                Id: i,
                Name: i + ''
            };
            $scope.ListYear.push(tmpYear);
        }

        if (date.getMonth() == 1 || date.getMonth() == 2 || date.getMonth() == 3) {
            $scope.Quy = "I";
        } else if (date.getMonth() == 4 || date.getMonth() == 5 || date.getMonth() == 6) {
            $scope.Quy = "II";
        } else if (date.getMonth() == 7 || date.getMonth() == 8 || date.getMonth() == 9) {
            $scope.Quy = "III";
        } else if (date.getMonth() == 10 || date.getMonth() == 11 || date.getMonth() == 12) {
            $scope.Quy = "IV";
        }
        $scope.modelSearch.Year = date.getFullYear();
        GetBottomAction();
        $scope.LoadPage(1);
    });

    $scope.RoleBtnUpdate = false;
    $scope.RoleBtnSearch = false;

    function GetBottomAction() {
        $scope.ListCity = [];
        $.ajax({
            type: 'post',
            url: '/BaoCaoQuy/GetBottomAction',
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

        if ($scope.Quy == null || $scope.Quy == '') {
            toastr.error("Vui lòng chọn quý!");
            return;
        } else {
            if ($scope.Quy == 'I') {
                $scope.modelSearch.Months = '1,2,3';
            } else if ($scope.Quy == 'II') {
                $scope.modelSearch.Months = '4,5,6';
            } else if ($scope.Quy == 'III') {
                $scope.modelSearch.Months = '7,8,9';
            } else if ($scope.Quy == 'IV') {
                $scope.modelSearch.Months = '10,11,12';
            }
        }
        $scope.modelSearch.CityCodes = '';
        if ($scope.ListCityCode != null && $scope.ListCityCode.length > 0) {
            for (var i = 0; i < $scope.ListCityCode.length; i++) {
                if ($scope.modelSearch.CityCodes == null || $scope.modelSearch.CityCodes == '') {
                    $scope.modelSearch.CityCodes = $scope.ListCityCode[i];
                } else {
                    $scope.modelSearch.CityCodes +=','+ $scope.ListCityCode[i];
                }
            }
            //$scope.modelSearch.CityCodes = $scope.ListCityCode.map(function (obj) { return obj.Code }).join(',');
        }

        showToast();
        $scope.ListData = [];
        $.ajax({
            type: 'post',
            url: '/BaoCaoQuy/SearchData',
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
        //var strData = '';
        //angular.forEach($scope.modelSearch.City, function (val, key) {
        //    if (val !== '') {
        //        if (strData !== '')
        //            strData += ',';
        //        strData += parseInt(val);
        //    }
        //});

        if ($scope.modelSearch.Year == null || $scope.modelSearch.Year == 0) {
            toastr.error("Vui lòng chọn năm!");
            return;
        }

        if ($scope.Quy == null || $scope.Quy == '') {
            toastr.error("Vui lòng chọn quý!");
            return;
        } else {
            if ($scope.Quy == 'I') {
                $scope.modelSearch.Months = '1,2,3';
            } else if($scope.Quy == 'II') {
                $scope.modelSearch.Months = '4,5,6';
            } else if ($scope.Quy == 'III') {
                $scope.modelSearch.Months = '7,8,9';
            } else if ($scope.Quy == 'IV') {
                $scope.modelSearch.Months = '10,11,12';
            }
        }

        if ($scope.ListCityCode != null && $scope.ListCityCode.length > 0) {
            for (var i = 0; i < $scope.ListCityCode.length; i++) {
                if ($scope.modelSearch.CityCodes == null || $scope.modelSearch.CityCodes == '') {
                    $scope.modelSearch.CityCodes = $scope.ListCityCode[i];
                } else {
                    $scope.modelSearch.CityCodes += ',' + $scope.ListCityCode[i];
                }
            }
        }
        window.location.href = '/BaoCaoQuy/ExportData?Months=' + $scope.modelSearch.Months + '&Year=' + $scope.modelSearch.Year + '&CityCodes=' + ($scope.modelSearch.CityCodes == undefined ? '' : $scope.modelSearch.CityCodes) + '&quy=' + $scope.Quy;
    }

   
});
