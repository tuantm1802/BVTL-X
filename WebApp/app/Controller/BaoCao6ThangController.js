app.controller("BaoCao6ThangController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "ParamCode DESC";
    $scope.ListYear = [];
    $scope.ListCity = [];
    $scope.List6Thang = [];
    $scope.SauThang = 1;
    $scope.ListCityCode = [];
    $scope.ListMaNhomTBH = [];
    $scope.ListNhomTBH = [];
    $scope.ListDuAn = [];
    
    $scope.ParamIdSeleted = 0;
    angular.element(document).ready(function () {
        $scope.ListMaNhomTBH = [];
        $scope.ListNhomTBH = [];
        $scope.SauThang = 1;
        var date = new Date();
        for (var i = date.getFullYear() - 5; i < date.getFullYear() + 5; i++) {
            var tmpYear = {
                Id: i,
                Name : i+''
            };
            $scope.ListYear.push(tmpYear);
        }

        var tmpMonth = {
            Id: 1,
            Name: '6 tháng đầu năm'
        };
        $scope.List6Thang.push(tmpMonth);

        var tmpMonth2 = {
            Id: 2,
            Name: '6 tháng cuối năm'
        };
        $scope.List6Thang.push(tmpMonth2);

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
            url: '/BaoCao6Thang/GetBottomAction',
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
                $scope.ListDuAn = response.DuAns;
                $scope.modelSearch.MaDuAn = $scope.ListDuAn[0].maduan;
                $scope.$apply();
            }
        });
    }

    $scope.LoadPage = function (genTable) {
        if ($scope.modelSearch.Year == null || $scope.modelSearch.Year == 0) {
            toastr.error("Vui lòng chọn năm!");
            return;
        }

        if ($scope.modelSearch.MaDuAn == null || $scope.modelSearch.MaDuAn == '') {
            toastr.error("Vui lòng chọn dự án!");
            return;
        }

        $scope.modelSearch.Months = '';
        if ($scope.SauThang > 0) {
            if ($scope.SauThang == 1) {
                $scope.modelSearch.Months ='1,2,3,4,5,6';
            } else {
                $scope.modelSearch.Months = '7,8,9,10,11,12';
            }
            
        } else {
            toastr.error("Vui lòng chọn kỳ báo cáo!");
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
            url: '/BaoCao6Thang/SearchData',
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

        if ($scope.modelSearch.MaDuAn == null || $scope.modelSearch.MaDuAn == '') {
            toastr.error("Vui lòng chọn dự án!");
            return;
        }
        $scope.modelSearch.Months = '';
        if ($scope.SauThang > 0) {
            if ($scope.SauThang == 1) {
                $scope.modelSearch.Months = '1,2,3,4,5,6';
            } else {
                $scope.modelSearch.Months = '7,8,9,10,11,12';
            }

        } else {
            toastr.error("Vui lòng chọn kỳ báo cáo!");
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

        window.location.href = '/BaoCao6Thang/ExportData?Months=' + $scope.modelSearch.Months + '&Year=' + $scope.modelSearch.Year
            + '&CityCodes=' + ($scope.modelSearch.CityCodes == undefined ? '' : $scope.modelSearch.CityCodes)
            + '&maNhomTBHs=' + ($scope.modelSearch.MaNhomTBH == undefined ? '' : $scope.modelSearch.MaNhomTBH)
            + '&maDuAn=' + ($scope.modelSearch.MaDuAn == undefined ? '' : $scope.modelSearch.MaDuAn);
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
            url: '/BaoCao6Thang/GetNhomTBHByCityCodes',
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
