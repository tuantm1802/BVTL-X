app.controller("SyncDataController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
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

    $scope.LoadPage = function (genTable) {
        showToast();
        $scope.ListData = [];
        if (genTable == 1) {
            dataTableApiSync = $('#dataTableApiSync').DataTable({
                lengthMenu: [10, 20, 30, 50, 60, 100],
                serverSide: true,
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
                                        TableNameSaveData: respone.data[i].TableNameSaveData,
                                        IsActive: respone.data[i].IsActive == true ? 'Sử dụng' : 'Không sử dụng',
                                        TimeReCall: respone.data[i].TimeReCall,
                                        Api_Id: respone.data[i].Api_Id
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
                select: {
                    info: false
                },
                columns: [
                    { "data": "STT", },
                    { "data": "Api_Code" },
                    { "data": "NameSyncdata" },
                    { "data": "HrefApi" },
                    { "data": "TableNameSaveData" },
                    { "data": "TimeReCall" },
                    { "data": "IsActive" },
                ],
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
});
