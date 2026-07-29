app.controller("BaoCaoTuanController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
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
    $scope.ListDuAn = [];
    $scope.ListWeek = [];
    $scope.Week = 1;

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

        // Initialize weeks 1 to 53
        for (var w = 1; w <= 53; w++) {
            $scope.ListWeek.push({
                Id: w,
                Name: "Tuần " + w
            });
        }
        
        // Calculate current week of year
        var oneJan = new Date(date.getFullYear(), 0, 1);
        var numberOfDays = Math.floor((date - oneJan) / (24 * 60 * 60 * 1000));
        var currentWeek = Math.ceil((date.getDay() + 1 + numberOfDays) / 7);
        if (currentWeek > 53) currentWeek = 53;
        $scope.Week = currentWeek;

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
            url: '/BaoCaoTuan/GetBottomAction',
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
                if ($scope.ListDuAn != null && $scope.ListDuAn.length > 0) {
                    $scope.modelSearch.MaDuAn = $scope.ListDuAn[0].maduan;
                }
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

        if ($scope.Week == null || $scope.Week == '') {
            toastr.error("Vui lòng chọn tuần!");
            return;
        }

        $scope.modelSearch.Week = $scope.Week;
        $scope.modelSearch.CityCodes = '';
        if ($scope.ListCityCode != null && $scope.ListCityCode.length > 0) {
            for (var i = 0; i < $scope.ListCityCode.length; i++) {
                if (i == 0)
                    $scope.modelSearch.CityCodes += $scope.ListCityCode[i];
                else
                    $scope.modelSearch.CityCodes += ',' + $scope.ListCityCode[i];
            }
        }

        $scope.modelSearch.MaNhomTBH = '';
        if ($scope.ListMaNhomTBH != null && $scope.ListMaNhomTBH.length > 0) {
            for (var i = 0; i < $scope.ListMaNhomTBH.length; i++) {
                if (i == 0)
                    $scope.modelSearch.MaNhomTBH += $scope.ListMaNhomTBH[i];
                else
                    $scope.modelSearch.MaNhomTBH += ',' + $scope.ListMaNhomTBH[i];
            }
        }

        $.ajax({
            type: 'post',
            url: '/BaoCaoTuan/SearchData',
            data: { modelSearch: $scope.modelSearch },
            success: function (response) {
                if (response.Error == false) {
                    $scope.ListData = response.data;
                } else {
                    toastr.error(response.Title);
                }
                $scope.$apply();
            }
        });
    };

    $scope.Refesh = function () {
        $scope.LoadPage(1);
    };

    $scope.Changecity = function () {
        $scope.ListMaNhomTBH = [];
        $scope.ListNhomTBH = [];
        var CityCodes = '';
        if ($scope.ListCityCode != null && $scope.ListCityCode.length > 0) {
            for (var i = 0; i < $scope.ListCityCode.length; i++) {
                if (i == 0)
                    CityCodes += $scope.ListCityCode[i];
                else
                    CityCodes += ',' + $scope.ListCityCode[i];
            }
        }
        $.ajax({
            type: 'post',
            url: '/BaoCaoTuan/GetNhomTBHByCityCodes',
            data: { CityCodes: CityCodes },
            success: function (response) {
                $scope.ListNhomTBH = response.NhomTBHs;
                $scope.$apply();
            }
        });
    };

    $scope.ExportExcel = function () {
        if ($scope.modelSearch.Year == null || $scope.modelSearch.Year == 0) {
            toastr.error("Vui lòng chọn năm!");
            return;
        }

        if ($scope.modelSearch.MaDuAn == null || $scope.modelSearch.MaDuAn == '') {
            toastr.error("Vui lòng chọn dự án!");
            return;
        }

        $scope.modelSearch.CityCodes = '';
        if ($scope.ListCityCode != null && $scope.ListCityCode.length > 0) {
            for (var i = 0; i < $scope.ListCityCode.length; i++) {
                if (i == 0)
                    $scope.modelSearch.CityCodes += $scope.ListCityCode[i];
                else
                    $scope.modelSearch.CityCodes += ',' + $scope.ListCityCode[i];
            }
        }

        $scope.modelSearch.MaNhomTBH = '';
        if ($scope.ListMaNhomTBH != null && $scope.ListMaNhomTBH.length > 0) {
            for (var i = 0; i < $scope.ListMaNhomTBH.length; i++) {
                if (i == 0)
                    $scope.modelSearch.MaNhomTBH += $scope.ListMaNhomTBH[i];
                else
                    $scope.modelSearch.MaNhomTBH += ',' + $scope.ListMaNhomTBH[i];
            }
        }

        var url = '/BaoCaoTuan/ExportData?Year=' + $scope.modelSearch.Year +
            '&Week=' + $scope.Week +
            '&CityCodes=' + $scope.modelSearch.CityCodes +
            '&maNhomTBHs=' + $scope.modelSearch.MaNhomTBH +
            '&maDuAn=' + $scope.modelSearch.MaDuAn;
        window.location = url;
    };
});
