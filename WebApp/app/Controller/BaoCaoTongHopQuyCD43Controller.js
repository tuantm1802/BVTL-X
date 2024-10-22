app.controller("BaoCaoCD43Controller", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
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

    $scope.ListQuy = [];

    $scope.TuQuy = "I";
    $scope.DenQuy = "I";
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

        if (date.getMonth() == 1 || date.getMonth() == 2 || date.getMonth() == 3) {
            $scope.TuQuy = "I";
            $scope.DenQuy = "I";
        } else if (date.getMonth() == 4 || date.getMonth() == 5 || date.getMonth() == 6) {
            $scope.TuQuy = "II";
            $scope.DenQuy = "II";
        } else if (date.getMonth() == 7 || date.getMonth() == 8 || date.getMonth() == 9) {
            $scope.TuQuy = "III";
            $scope.DenQuy = "III";
        } else if (date.getMonth() == 10 || date.getMonth() == 11 || date.getMonth() == 12) {
            $scope.TuQuy = "IV";
            $scope.DenQuy = "IV";
        }
        $scope.modelSearch.TuNam = date.getFullYear();
        $scope.modelSearch.DenNam = date.getFullYear();
        console.log(123123);
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
            url: '/BaoCaoCD43/GetBottomAction',
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
        if ($scope.modelSearch.TuNam == null || $scope.modelSearch.TuNam == 0) {
            toastr.error("Vui lòng chọn từ năm!");
            return;
        }

        if ($scope.modelSearch.DenNam == null || $scope.modelSearch.DenNam == 0) {
            toastr.error("Vui lòng chọn đến năm!");
            return;
        }

        if ($scope.TuQuy == null || $scope.TuQuy == '') {
            toastr.error("Vui lòng chọn từ quý!");
            return;
        } else {
            if ($scope.TuQuy == 'I') {
                $scope.modelSearch.TuThang = 1;
            } else if ($scope.TuQuy == 'II') {
                $scope.modelSearch.TuThang = 4;
            } else if ($scope.TuQuy == 'III') {
                $scope.modelSearch.TuThang = 7;
            } else if ($scope.TuQuy == 'IV') {
                $scope.modelSearch.TuThang = 10;
            }
        }

        if ($scope.DenQuy == null || $scope.DenQuy == '') {
            toastr.error("Vui lòng chọn đến quý!");
            return;
        } else {
            if ($scope.DenQuy == 'I') {
                $scope.modelSearch.DenThang = 3;
            } else if ($scope.DenQuy == 'II') {
                $scope.modelSearch.DenThang = 6;
            } else if ($scope.DenQuy == 'III') {
                $scope.modelSearch.DenThang = 9;
            } else if ($scope.DenQuy == 'IV') {
                $scope.modelSearch.DenThang = 12;
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
            url: '/BaoCaoCD43/SearchDataBaoCaoHoatDong',
            cache: false,
            async: false,
            data: $scope.modelSearch,
            success: function (respone) {
                $scope.ListData = respone.data;
                if (respone.data != null && respone.data.length > 0) {
                    $scope.ListQuy = respone.data[0].ListQuy;
                }
                
            }
        });

        
        hideLoading();
    };

    $scope.Refesh = function () {
        $scope.LoadPage(0);
    };

    $scope.ExportExcel = function () {
        if ($scope.modelSearch.TuNam == null || $scope.modelSearch.TuNam == 0) {
            toastr.error("Vui lòng chọn từ năm!");
            return;
        }

        if ($scope.modelSearch.DenNam == null || $scope.modelSearch.DenNam == 0) {
            toastr.error("Vui lòng chọn đến năm!");
            return;
        }

        if ($scope.TuQuy == null || $scope.TuQuy == '') {
            toastr.error("Vui lòng chọn từ quý!");
            return;
        } else {
            if ($scope.TuQuy == 'I') {
                $scope.modelSearch.TuThang = 1;
            } else if ($scope.TuQuy == 'II') {
                $scope.modelSearch.TuThang = 4;
            } else if ($scope.TuQuy == 'III') {
                $scope.modelSearch.TuThang = 7;
            } else if ($scope.TuQuy == 'IV') {
                $scope.modelSearch.TuThang = 10;
            }
        }

        if ($scope.DenQuy == null || $scope.DenQuy == '') {
            toastr.error("Vui lòng chọn đến quý!");
            return;
        } else {
            if ($scope.DenQuy == 'I') {
                $scope.modelSearch.DenThang = 3;
            } else if ($scope.DenQuy == 'II') {
                $scope.modelSearch.DenThang = 6;
            } else if ($scope.DenQuy == 'III') {
                $scope.modelSearch.DenThang = 9;
            } else if ($scope.DenQuy == 'IV') {
                $scope.modelSearch.DenThang = 12;
            }
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

        window.location.href = '/BaoCaoCD43/ExportData?TuThang=' + $scope.modelSearch.TuThang + '&TuNam=' + $scope.modelSearch.TuNam
            + '&DenThang=' + $scope.modelSearch.DenThang + '&DenNam=' + $scope.modelSearch.DenNam
            + '&CityCodes=' + ($scope.modelSearch.CityCodes == undefined ? '' : $scope.modelSearch.CityCodes) 
            + '&maNhomTBHs=' + ($scope.modelSearch.MaNhomTBH == undefined ? '' : $scope.modelSearch.MaNhomTBH)
            //+ '&maDuAn=' + ($scope.modelSearch.MaDuAn == undefined ? '' : $scope.modelSearch.MaDuAn)
            ;
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
            url: '/BaoCaoCD43/GetNhomTBHByCityCodes',
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
