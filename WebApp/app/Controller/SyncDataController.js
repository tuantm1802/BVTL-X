app.controller("SyncDataController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $compile) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 50;
    $scope.modelSearch.SortColumn = "Name DESC";
    $scope.ListData = [];

    var dataTableApiSync = null;
    $scope.SyncDataIdSeleted = 0;
    angular.element(document).ready(function () {

        GetBottomAction();
        $scope.LoadPage(1);
    });

    $scope.RoleBtnSyncData = false;
    $scope.RoleBtnSearch = false;
    function GetBottomAction() {
        $.ajax({
            type: 'post',
            url: '/SyncData/GetBottomAction',
            data: {},
            success: function (response) {
                if (response.Buttoms != null) {
                    angular.forEach(response.Buttoms, function (item) {
                        if (item == 'btnSyncData') {
                            $scope.RoleBtnSyncData = true;
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

    $('#dataTableApiSync').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });

    $scope.LoadPage = function (genTable) {
        showToast();
        $scope.ListData = [];
        if (genTable == 1) {
            dataTableApiSync = $('#dataTableApiSync').DataTable({
                lengthMenu: [50, 20, 30, 40, 60, 100],
                serverSide: true,
                ordering: false,
                searching: false,
                autoWidth: false,
                scrollX: true,
                ajax: function (data, callback, settings) {
                    var dataUser = [];
                    var totalItems = 0;
                    var page = ((data.start / data.length) + 1);

                    $scope.modelSearch.currentPage = page;
                    $scope.modelSearch.pageSize = data.length;

                    $.ajax({
                        type: 'post',
                        url: '/SyncData/GetAllByPage',
                        cache: false,
                        async: false,
                        data: $scope.modelSearch,
                        success: function (respone) {
                            totalItems = respone.totalItems;
                            $scope.ListData = respone.data;
                            if (respone.data != null && respone.data.length > 0) {
                                for (var i = 0; i < respone.data.length; i++) {
                                    var tmp = {
                                        STT: i + 1,
                                        Api_Code: respone.data[i].Api_Code,
                                        NameSyncdata: respone.data[i].NameSyncdata,
                                        HrefApi: respone.data[i].HrefApi,
                                        ReportId: respone.data[i].ReportId,
                                        IsActive: respone.data[i].IsActive == true ? 'Active' : 'InActive',
                                        TimeReCall: respone.data[i].TimeReCall,
                                        Api_Id: respone.data[i].Api_Id,
                                        RawOrLabel: respone.data[i].RawOrLabel,
                                        Start_Time_Sync: respone.data[i].Start_Time_Sync != null ? moment(respone.data[i].Start_Time_Sync).format("YYYY-MM-DD HH:mm:ss") : "",
                                        End_Time_Sync: respone.data[i].End_Time_Sync != null ? moment(respone.data[i].End_Time_Sync).format("YYYY-MM-DD HH:mm:ss") : "",
                                        Message: respone.data[i].Message
                                    }
                                    dataUser.push(tmp);
                                }
                            }
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
                rowId: 'Api_Id',
                //select: {
                //    info: false
                //},
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
                columns: [
                    { "data": "STT", "width": "80px" },
                    { "data": "Api_Code", "width": "150px" },
                    {
                        "data": "NameSyncdata", "width": "200px",
                        "createdCell": function (td) {
                            $(td).css('white-space', 'nowrap');
                        }
                    },
                    { "data": "HrefApi", "width": "250px" },
                    { "data": "ReportId", "width": "100px" },
                    { "data": "TimeReCall", "width": "100px" },
                    { "data": "IsActive", "width": "80px" },
                    { "data": "Start_Time_Sync", "width": "150px" },
                    { "data": "End_Time_Sync", "width": "150px" },
                    { "data": "Message", "width": "200px" },
                    { "data": "RawOrLabel", "width": "100px" },
                    {
                        "data": null,   // Không yêu cầu dữ liệu từ nguồn
                        "title": "Thao tác",
                        "width": "100px",
                        "createdCell": function (td) {
                            $(td).css('white-space', 'nowrap');
                        },
                        "render": function (data, type, full) {
                            return '<button type="button" ng-click="SyncDataRow(' + full.Api_Id + ')" class="btn btn-primary">Đồng bộ</button>'
                        }
                    }
                ],
                
                rowCallback: function (row) {
                    if (!row.compiled) {
                        $compile(angular.element(row))($scope);
                        row.compiled = true;
                    }
                },
                dom: "<'row'<'col-sm-12'f>>" +
                    "<'row'<'col-sm-12'tr>>" +
                    "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>",

                scroller: {
                    loadingIndicator: true
                },
            });
        } else {
            dataTableApiSync.ajax.reload();
        }


        hideLoading();
    };

    $scope.Refesh = function () {
        $scope.LoadPage(0);
    };

    $scope.SyncDataAPI = function () {
        var seletedRow = dataTableApiSync.rows({ selected: true });
        var count = seletedRow.count();
        if (count > 0) {
            $scope.SyncDataIdSeleted = seletedRow.data()[0].Api_Id;
        } else {
            $scope.SyncDataIdSeleted = 0;
        }

        if ($scope.SyncDataIdSeleted > 0 && $scope.SyncDataIdSeleted != undefined) {

            var name = $scope.ListData.filter(function (item) {
                return item.Api_Id === $scope.SyncDataIdSeleted;
            })[0].NameSyncdata;

            $ngConfirm({
                title: 'Thông báo',
                content: 'Bạn có chắc chắn muốn đồng bộ lại dữ liệu của tiến trình ' + name + ' không?',
                scope: $scope,
                buttons: {
                    delete: {
                        text: 'Đồng ý',
                        btnClass: 'btn-blue',
                        action: function (scope, button) {
                            $.ajax({
                                type: 'post',
                                url: '/SyncData/SyncDataFromApi',
                                data: { Id: $scope.SyncDataIdSeleted },
                                success: function (data) {
                                    if (data.Error) {
                                        toastr.error(data.Title);
                                    } else {
                                        toastr.success(data.Title);
                                        $scope.LoadPage(0);
                                    }
                                }
                            });
                        }
                    },
                    close: {
                        text: 'Hủy',
                        action: function (scope, button) {

                        }
                    }
                }
            });
        } else {
            toastr.error("Bạn chưa chọn bản ghi nào.");
        }
    };

    
    $scope.SyncDataRow = function (apiId) {
        $scope.SyncDataIdSeleted = apiId;
        if ($scope.SyncDataIdSeleted > 0 && $scope.SyncDataIdSeleted != undefined) {

            var btn = $('button[ng-click="SyncDataRow(' + apiId + ')"]');
            btn.html('Processing...').attr('disabled', true); // Đổi nội dung nút và vô hiệu hóa nút

            var name = $scope.ListData.filter(function (item) {
                return item.Api_Id === $scope.SyncDataIdSeleted;
            })[0].NameSyncdata;

            $ngConfirm({
                title: 'Thông báo',
                content: 'Bạn có chắc chắn muốn đồng bộ lại dữ liệu của tiến trình ' + name + ' không?',
                scope: $scope,
                buttons: {
                    delete: {
                        text: 'Đồng ý',
                        btnClass: 'btn-blue',
                        action: function (scope, button) {
                            $.ajax({
                                type: 'post',
                                url: '/SyncData/SyncDataFromApi',
                                data: { Id: $scope.SyncDataIdSeleted },
                                success: function (data) {
                                    if (data.Error) {
                                        btn.html('Thử lại').attr('disabled', false);
                                        toastr.error(data.Title);
                                    } else {
                                        btn.html('Hoàn thành').attr('disabled', false);
                                        toastr.success(data.Title);
                                        $scope.LoadPage(0);
                                    }
                                }
                            });
                        }
                    },
                    close: {
                        text: 'Hủy',
                        action: function (scope, button) {

                        }
                    }
                }
            });
        } else {
            toastr.error("Bạn chưa chọn bản ghi nào.");
        }
    };
});
