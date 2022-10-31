app.controller("BaoCaoThangController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "ParamCode DESC";
    $scope.ListYear = [];
    $scope.ListCity = [];
    $scope.ListThang = [];
    $scope.Thangs = [];
    $scope.ListCityCode = [];
    
    $scope.ParamIdSeleted = 0;
    angular.element(document).ready(function () {
        $scope.Thangs = [];
        var date = new Date();
        for (var i = date.getFullYear() - 5; i < date.getFullYear() + 5; i++) {
            var tmpYear = {
                Id: i,
                Name : i+''
            };
            $scope.ListYear.push(tmpYear);
        }
        for (var i = 1; i < 13; i++) {
            var tmpMonth = {
                Id: i,
                Name: (i + '')
            };
            $scope.ListThang.push(tmpMonth);
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
            url: '/BaoCaoThang/GetBottomAction',
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
        $scope.modelSearch.Months = '';

        console.log($scope.Thangs);
        console.log($scope.Thangs.join(','));

        if ($scope.Thangs != null && $scope.Thangs.length > 0) {
            //for (var i = 0; i < $scope.Thangs.length; i++) {
            //    if ($scope.modelSearch.Months == null || $scope.modelSearch.Months == '') {
            //        $scope.modelSearch.Months = $scope.Thangs[i].Id;
            //    } else {
            //        $scope.modelSearch.Months += ',' + $scope.Thangs[i].Id;
            //    }
            //}
            //$scope.modelSearch.Months = $scope.Thangs.map(function (obj) { return obj.Id; }).join(',');
            $scope.modelSearch.Months = $scope.Thangs.join(',');
        } else {
            toastr.error("Vui lòng chọn tháng!");
            return;
        }
        console.log($scope.modelSearch.Months);

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
        showToast();
        $scope.ListData = [];
        $.ajax({
            type: 'post',
            url: '/BaoCaoThang/SearchData',
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
        if ($scope.Thangs != null && $scope.Thangs.length > 0) {
            //for (var i = 0; i < $scope.Thangs.length; i++) {
            //    if ($scope.modelSearch.Months == null || $scope.modelSearch.Months == '') {
            //        $scope.modelSearch.Months = $scope.Thangs[i].Id;
            //    } else {
            //        $scope.modelSearch.Months += ',' + $scope.Thangs[i].Id;
            //    }
            //}
            //$scope.modelSearch.Months = $scope.Thangs.map(function (obj) { return obj.Id; }).join(',');
            $scope.modelSearch.Months = $scope.Thangs.join(',');
        } else {
            toastr.error("Vui lòng chọn tháng!");
            return;
        }

        if ($scope.ListCityCode != null && $scope.ListCityCode.length > 0) {
            for (var i = 0; i < $scope.ListCityCode.length; i++) {
                if ($scope.modelSearch.CityCodes == null || $scope.modelSearch.CityCodes == '') {
                    $scope.modelSearch.CityCodes = $scope.ListCityCode[i];
                } else {
                    $scope.modelSearch.CityCodes += ',' + $scope.ListCityCode[i];
                }
            }
            //$scope.modelSearch.CityCodes = $scope.ListCityCode.map(function (obj) { return obj.Code; }).join(',');
        }
        window.location.href = '/BaoCaoThang/ExportData?Months=' + $scope.modelSearch.Months + '&Year=' + $scope.modelSearch.Year + '&CityCodes=' + ($scope.modelSearch.CityCodes == undefined ? '' : $scope.modelSearch.CityCodes);
    }


});
