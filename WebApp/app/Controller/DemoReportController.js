app.controller("DemoReportController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "ParamCode DESC";

    var dataTableReport = null;
    $scope.ParamIdSeleted = 0;
    angular.element(document).ready(function () {
        
        GetBottomAction();
        $scope.LoadPage(1);
    });

    $scope.RoleBtnUpdate = false;
    $scope.RoleBtnSearch = false;

    function GetBottomAction() {
        $.ajax({
            type: 'post',
            url: '/DemoReport/GetBottomAction',
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
                $scope.$apply();
            }
        });
    }

    $scope.LoadPage = function (genTable) {
        showToast();
        $scope.ListData = [];
        $.ajax({
            type: 'post',
            url: '/DemoReport/GetAll',
            cache: false,
            async: false,
            data: $scope.modelSearch,
            success: function (respone) {
                totalItems = respone.totalItems;
                $scope.ListData = respone.data;
            }
        });

        //dataTableReport = $('#dataTableReport').DataTable({});
        
        if (genTable == 1) {
            dataTableReport = $('#dataTableReport').DataTable({
                paging: false,
                ordering: false,
                searching: false,
                ajax: function (data, callback, settings) {
                    var dataUser = [];
                    var totalItems = 0;
                    var page = ((data.start / data.length) + 1);

                    $scope.modelSearch.currentPage = page;
                    $scope.modelSearch.pageSize = data.length;

                    $.ajax({
                        type: 'post',
                        url: '/DemoReport/GetAll',
                        cache: false,
                        async: false,
                        data: $scope.modelSearch,
                        success: function (respone) {
                            totalItems = respone.totalItems;
                            $scope.ListData = respone.data;
                            dataUser = respone.data;
                        }
                    });

                    setTimeout(function () {
                        callback({
                            draw: data.draw,
                            data: dataUser,
                            recordsTotal: totalItems,
                            recordsFiltered: totalItems
                        });
                    }, 50);
                },
                "columns": [
                    { "data": "STT" },
                    { "data": "ThongTinBC" },
                    { "data": "ThongTinBC_Them" },
                    { "data": "MSM" },
                    { "data": "PUD" },
                    { "data": "SW" },
                    { "data": "Nam" },
                    { "data": "Nu" },
                    { "data": "ChuyenGioi" },
                    { "data": "Tong" }
                ],
                //dom: "<'row'<'col-sm-12'B>>"
                //    + "<'row'<'col-sm-12'f>>" 
                //    + "<'row'<'col-sm-12'tr>>"
                //        //+
                //    //"<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
                //,
                //'rowsGroup': [1],
                //colsGroup: [
                //    'ThongTinBC'
                //],
                dom: 'Bfrtip',
                'rowsGroup': [1],
                'createdRow': function (row, data, dataIndex) {
                    console.log("Data");
                    console.log(row);
                    console.log(data);
                    console.log(dataIndex);

                    // Update cell data
                    //this.api().cell($('td:eq(1)', row)).data('N/A');

                    // as an indication that grouping with COLSPAN is needed
                    if (data.Colpan > 1) {
                        // Add COLSPAN attribute
                        $('td:eq(1)', row).attr('colspan', data.Colpan);

                        // next to the cell with COLSPAN attribute
                        $('td:eq(2)', row).css('display', 'none');
                    }
                    //if (data.Rowpan > 1) {
                    //    // Add COLSPAN attribute
                    //    $('td:eq(1)', row).attr('rowspan', data.Rowpan);

                    //}
                },
                buttons: [
                    {
                        extend: 'excelHtml5',
                        title: 'Xuất excel'
                    },
                    {
                        extend: 'pdfHtml5',
                        title: 'Xuất PDF'
                    },
                    'colvis'
                ],
                scroller: {
                    loadingIndicator: true
                },
            });

            $('#example').DataTable({
                dom: 'Bfrtip',
                buttons: [
                    {
                        extend: 'copyHtml5',
                        exportOptions: {
                            columns: [0, ':visible']
                        }
                    },
                    {
                        extend: 'excelHtml5',
                        exportOptions: {
                            columns: ':visible'
                        }
                    },
                    {
                        extend: 'pdfHtml5',
                        exportOptions: {
                            columns: [0, 1, 2, 5]
                        }
                    },
                    'colvis'
                ]
            });
        } else {
            dataTableReport.ajax.reload();
        }
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

        //var strDataUnit = '';
        //angular.forEach($scope.modelSearch.Unit, function (val, key) {
        //    if (val !== '') {
        //        if (strDataUnit !== '')
        //            strDataUnit += ',';
        //        strDataUnit += parseInt(val);
        //    }
        //});

        //window.location.href = '/ReportExplosiveByCareer/ExportData?listCities=' + strData + '&fromDate=' + moment($scope.modelSearch.SearchFromDate).format('YYYYMMDD') + '&toDate=' + moment($scope.modelSearch.SearchToDate).format('YYYYMMDD') + '&listUnitId=' + strDataUnit + "&isThucTe=" + parseInt($scope.modelSearch.isThucTe);;

        window.location.href = '/DemoReport/ExportExcel';
    }

   
});
