app.controller("KetQuaSangLocCD43Controller", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "kqslace_id";

    $scope.ListCity = [];
    $scope.ListNhomTBH = [];
    $scope.ListDuAn = [];

    $scope.KetQuaTTCBHanhViNguyCos = [];
    $scope.CacLoaiChatGayNghienAssists = [];
    $scope.KetQuaQSTs = [];
    $scope.KetQuaACEs = [];
    
    var dataTableKetQuaTTCBHanhViNguyCos = null;
    
    $scope.ParamIdSeleted = 0;
    angular.element(document).ready(function () {

        var date = new Date();
        $scope.modelSearch.FromDate = date;
        $scope.modelSearch.ToDate = date;

        $scope.ListCity = [];
        $scope.ListNhomTBH = [];
        
        GetBottomAction();
        $scope.LoadPage(1);
        
    });

    $scope.RoleBtnExportExcel = false;
    $scope.RoleBtnSearch = false;

    function GetBottomAction() {
        $.ajax({
            type: 'post',
            url: '/KetQuaSangLoc/GetBottomAction',
            cache: false,
            async: false,
            data: {},
            success: function (response) {
                if (response.Buttoms != null) {
                    angular.forEach(response.Buttoms, function (item) {
                        if (item == 'btnExportExcel') {
                            $scope.RoleBtnExportExcel = true;
                        }
                        if (item == 'btnSearch') {
                            $scope.RoleBtnSearch = true;
                        }
                    });

                    $scope.ListCity = response.Citis;
                    //$scope.modelSearch.CityCodes = $scope.ListCity[0].Code;
                    $scope.ListNhomTBH = response.NhomTBHs;
                    $scope.ListDuAn = response.DuAns;
                    //$scope.modelSearch.MaDuAn = $scope.ListDuAn[0].maduan;
                    //$scope.modelSearch.MaNhomTBH = $scope.ListNhomTBH[0].manhom_tbh;
                    $("#CityCodes").select2({
                        placeholder: "Chọn tỉnh",
                        //allowClear: true
                    });
                }
                //$scope.$apply();
            }
        });
    }
    
    $('#dataTableKetQuaTTCBHanhViNguyCos').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    
    $scope.LoadPage = function (genTable) {

        //if (genTable == 1) {
        //    $scope.modelSearch.CityCodes = $scope.ListCity[0].Code;
        //    $scope.modelSearch.MaNhomTBH = $scope.ListNhomTBH[0].manhom_tbh;
        //}

       
        $scope.KetQuaTTCBHanhViNguyCos = [];
        $scope.CacLoaiChatGayNghienAssists = [];
        $scope.KetQuaQSTs = [];
        $scope.KetQuaACEs = [];

        console.log($scope.modelSearch.FromDate);

        // Check điều kiện tìm kiếm
        if ($scope.modelSearch.FromDate == null || $scope.modelSearch.FromDate == '' || $scope.modelSearch.FromDate == undefined) {
            toastr.error("Vui lòng chọn Từ ngày!");
            return;
        }

        if ($scope.modelSearch.ToDate == null || $scope.modelSearch.ToDate == '' || $scope.modelSearch.ToDate == undefined) {
            toastr.error("Vui lòng chọn Đến ngày!");
            return;
        }

        //if ($scope.modelSearch.CityCodes == null || $scope.modelSearch.CityCodes == '' || $scope.modelSearch.CityCodes == undefined) {
        //    toastr.error("Vui lòng chọn Tỉnh!");
        //    return;
        //}

        //if ($scope.modelSearch.MaNhomTBH == null || $scope.modelSearch.MaNhomTBH == '' || $scope.modelSearch.MaNhomTBH == undefined) {
        //    toastr.error("Vui lòng chọn Nhóm TBH!");
        //    return;
        //}
        showToast();
        var inputSearch = {
            //_FromDate: $scope.modelSearch.FromDate,
            //_ToDate: $scope.modelSearch.ToDate,
            FromDate: $scope.modelSearch.FromDate.toISOString().slice(0, 10).replace(/-/g, ""),
            ToDate: $scope.modelSearch.ToDate.toISOString().slice(0, 10).replace(/-/g, ""),
            CityCodes: $scope.modelSearch.CityCodes,
            MaNhomTBH: $scope.modelSearch.MaNhomTBH,
            MaDuAn: $scope.modelSearch.MaDuAn,
            TuSoMaKH: null,
            DenSoMaKH: null
        };

        console.log(inputSearch);
        //var res = $scope.modelSearch.FromDate.toISOString().slice(0, 10).replace(/-/g, "");

        //var res1 = $scope.modelSearch.ToDate.toISOString().slice(0, 10).replace(/-/g, "");

        //if ($scope.modelSearch.TuSoMaKH != null && $scope.modelSearch.TuSoMaKH != '') {
        //    inputSearch.TuSoMaKH = parseInt($scope.modelSearch.TuSoMaKH.replace($scope.modelSearch.MaNhomTBH, ''));
        //} 

        //if ($scope.modelSearch.DenSoMaKH != null && $scope.modelSearch.DenSoMaKH != '') {
        //    inputSearch.DenSoMaKH = parseInt($scope.modelSearch.DenSoMaKH.replace($scope.modelSearch.MaNhomTBH, ''));
        //}

        // Lấy dữ liệu báo cáo
        $.ajax({
            type: 'post',
            url: '/KetQuaSangLoc/GetAllCD43',
            cache: false,
            async: false,
            data: inputSearch,
            success: function (respone) {
                
                $scope.KetQuaTTCBHanhViNguyCos = respone.KetQuaTTCBHanhViNguyCos;
                $scope.CacLoaiChatGayNghienAssists = respone.CacLoaiChatGayNghienAssists;
                $scope.KetQuaQSTs = respone.KetQuaQSTs;
                $scope.KetQuaACEs = respone.KetQuaACEs;
                console.log($scope.KetQuaTTCBHanhViNguyCos);
            }
        });

        $scope.ListKetQuaSangLoc = [];
        if (genTable == 1 || genTable == 2) {
            if (genTable == 2) {
                DestroyTable();
            }
            // gen table
            
            GenTableKetQuaTTCBHanhViNguyCos();            

        } else {
            ReloadTable();
        }
        hideLoading();
    };

    function ReloadTable(){
        
        dataTableKetQuaTTCBHanhViNguyCos.ajax.reload();
       
    }

    function DestroyTable() {
        
        dataTableKetQuaTTCBHanhViNguyCos.destroy();
        
    }
    function GenTableKetQuaTTCBHanhViNguyCos() {
        dataTableKetQuaTTCBHanhViNguyCos = $('#dataTableKetQuaTTCBHanhViNguyCos').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "SoNguoi", searchBuilderType: "number" },
                { "data": "PhanTram", searchBuilderType: "number" }
            ],
            "language": {
                "emptyTable": "Không có dữ liệu trong bản",
                "info": "Hiển thị _START_ đến _END_ của _TOTAL_ bản ghi",
                "infoEmpty": "Hiển thị 0 đến 0 của 0 bản ghi",
                "infoFiltered": "(lọc từ _MAX_ tổng bản ghi)",
                "lengthMenu": "Hiển thị _MENU_ bản ghi",
                "loadingRecords": "Đang tải...",
                "search": "Tìm kiếm:",
                "zeroRecords": "Không tìm thấy kết quả",
                "paginate": {
                    "first": "<<",
                    "last": ">>",
                    "next": ">",
                    "previous": "<"
                },
            },
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }
        
 
    $scope.Refesh = function () {
        $scope.LoadPage(0);
    };

    
    $scope.ExportExcel = function () {
        // Check điều kiện tìm kiếm
        if ($scope.modelSearch.FromDate == null || $scope.modelSearch.FromDate == '' || $scope.modelSearch.FromDate == undefined) {
            toastr.error("Vui lòng chọn Từ ngày!");
            return;
        }

        if ($scope.modelSearch.ToDate == null || $scope.modelSearch.ToDate == '' || $scope.modelSearch.ToDate == undefined) {
            toastr.error("Vui lòng chọn Đến ngày!");
            return;
        }

        //if ($scope.modelSearch.CityCodes == null || $scope.modelSearch.CityCodes == '' || $scope.modelSearch.CityCodes == undefined) {
        //    toastr.error("Vui lòng chọn Tỉnh!");
        //    return;
        //}

       
        if ($scope.ListCityCode != null && $scope.ListCityCode.length > 0) {
            for (var i = 0; i < $scope.ListCityCode.length; i++) {
                if ($scope.modelSearch.CityCodes == null || $scope.modelSearch.CityCodes == '') {
                    $scope.modelSearch.CityCodes = $scope.ListCityCode[i];
                } else {
                    $scope.modelSearch.CityCodes += ',' + $scope.ListCityCode[i];
                }
            }
        }

        console.log($scope.ListMaNhomTBH);
        console.log($scope.modelSearch.MaNhomTBH);

        //$scope.modelSearch.MaNhomTBH = '';
        if ($scope.ListMaNhomTBH != null && $scope.ListMaNhomTBH.length > 0) {
            for (var i = 0; i < $scope.ListMaNhomTBH.length; i++) {
                if ($scope.modelSearch.MaNhomTBH == null || $scope.modelSearch.MaNhomTBH == '') {
                    $scope.modelSearch.MaNhomTBH = $scope.ListMaNhomTBH[i];
                } else {
                    $scope.modelSearch.MaNhomTBH += ',' + $scope.ListMaNhomTBH[i];
                }
            }
        }

        window.location.href = '/KetQuaSangLoc/ExportData?FromDate=' + $scope.modelSearch.FromDate.toISOString().slice(0, 10).replace(/-/g, "")
            + '&ToDate=' + $scope.modelSearch.ToDate.toISOString().slice(0, 10).replace(/-/g, "")
            + '&CityCodes=' + ($scope.modelSearch.CityCodes == undefined ? '' : $scope.modelSearch.CityCodes)
            + '&maNhomTBHs=' + ($scope.modelSearch.MaNhomTBH == undefined ? '' : $scope.modelSearch.MaNhomTBH);
    }
    $scope.ExportDataSangLoc = function () {
        // Check điều kiện tìm kiếm
        if ($scope.modelSearch.FromDate == null || $scope.modelSearch.FromDate == '' || $scope.modelSearch.FromDate == undefined) {
            toastr.error("Vui lòng chọn Từ ngày!");
            return;
        }

        if ($scope.modelSearch.ToDate == null || $scope.modelSearch.ToDate == '' || $scope.modelSearch.ToDate == undefined) {
            toastr.error("Vui lòng chọn Đến ngày!");
            return;
        }

        //if ($scope.modelSearch.CityCodes == null || $scope.modelSearch.CityCodes == '' || $scope.modelSearch.CityCodes == undefined) {
        //    toastr.error("Vui lòng chọn Tỉnh!");
        //    return;
        //}

       
        if ($scope.ListCityCode != null && $scope.ListCityCode.length > 0) {
            for (var i = 0; i < $scope.ListCityCode.length; i++) {
                if ($scope.modelSearch.CityCodes == null || $scope.modelSearch.CityCodes == '') {
                    $scope.modelSearch.CityCodes = $scope.ListCityCode[i];
                } else {
                    $scope.modelSearch.CityCodes += ',' + $scope.ListCityCode[i];
                }
            }
        }

        console.log($scope.ListMaNhomTBH);
        console.log($scope.modelSearch.MaNhomTBH);

        //$scope.modelSearch.MaNhomTBH = '';
        if ($scope.ListMaNhomTBH != null && $scope.ListMaNhomTBH.length > 0) {
            for (var i = 0; i < $scope.ListMaNhomTBH.length; i++) {
                if ($scope.modelSearch.MaNhomTBH == null || $scope.modelSearch.MaNhomTBH == '') {
                    $scope.modelSearch.MaNhomTBH = $scope.ListMaNhomTBH[i];
                } else {
                    $scope.modelSearch.MaNhomTBH += ',' + $scope.ListMaNhomTBH[i];
                }
            }
        }

        window.location.href = '/KetQuaSangLoc/ExportDataSangLoc?FromDate=' + $scope.modelSearch.FromDate.toISOString().slice(0, 10).replace(/-/g, "")
            + '&ToDate=' + $scope.modelSearch.ToDate.toISOString().slice(0, 10).replace(/-/g, "")
            + '&CityCodes=' + ($scope.modelSearch.CityCodes == undefined ? '' : $scope.modelSearch.CityCodes)
            + '&maNhomTBHs=' + ($scope.modelSearch.MaNhomTBH == undefined ? '' : $scope.modelSearch.MaNhomTBH);
    }

});