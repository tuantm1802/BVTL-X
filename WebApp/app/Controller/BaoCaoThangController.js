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
    $scope.ListMaNhomTBH = [];
    $scope.ListNhomTBH = [];
    
    $scope.ParamIdSeleted = 0;
    angular.element(document).ready(function () {
        $scope.ListMaNhomTBH = [];
        $scope.ListNhomTBH = [];
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
        $scope.Changecity();
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

        if ($scope.Thangs != null && $scope.Thangs.length > 0) {
            for (var i = 0; i < $scope.Thangs.length; i++) {
                if ($scope.modelSearch.Months == null || $scope.modelSearch.Months == '') {
                    $scope.modelSearch.Months = $scope.Thangs[i];
                } else {
                    $scope.modelSearch.Months += ',' + $scope.Thangs[i];
                }
            }
            //$scope.modelSearch.Months = $scope.Thangs.map(function (obj) { return obj.Id; }).join(',');
        } else {
            toastr.error("Vui lòng chọn tháng!");
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
            for (var i = 0; i < $scope.Thangs.length; i++) {
                if ($scope.modelSearch.Months == null || $scope.modelSearch.Months == '') {
                    $scope.modelSearch.Months = $scope.Thangs[i];
                } else {
                    $scope.modelSearch.Months += ',' + $scope.Thangs[i];
                }
            }
            //$scope.modelSearch.Months = $scope.Thangs.map(function (obj) { return obj.Id; }).join(',');
        } else {
            toastr.error("Vui lòng chọn tháng!");
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
        window.location.href = '/BaoCaoThang/ExportData?Months=' + $scope.modelSearch.Months + '&Year=' + $scope.modelSearch.Year
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
            url: '/BaoCaoThang/GetNhomTBHByCityCodes',
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
