app.controller("DuongSuDungController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "hoten";

    var dataTableDuongSuDung = null;
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
            url: '/DuongSuDung/GetBottomAction',
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

    $('#dataTableDuongSuDung').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });

    $scope.LoadPage = function (genTable) {
        showToast();
       
        $scope.ListDuongSuDung = [];
        if (genTable == 1 || genTable == 2) {
            if (genTable == 2)
                dataTableDuongSuDung.destroy();
            dataTableDuongSuDung = $('#dataTableDuongSuDung').DataTable({
                lengthMenu: [10, 20, 30, 50, 60, 100],
                serverSide: true,
                ordering: false,
                searching: true,
                processing: true,
                ajax: function (data, callback, settings) {
                    var dataUser = [];
                    var totalItems = 0;
                    var page = ((data.start / data.length) + 1);

                    $scope.modelSearch.currentPage = page;
                    $scope.modelSearch.pageSize = data.length;

                    $.ajax({
                        type: 'post',
                        url: '/DuongSuDung/GetAll',
                        cache: false,
                        async: false,
                        data: $scope.modelSearch,
                        success: function (respone) {
                            totalItems = respone.totalItems;
                            $scope.ListDuongSuDung = respone.data;
                            if (respone.data != null && respone.data.length > 0) {
                                for (var i = 0; i < respone.data.length; i++) {
                                    var tmp = {
                                        STT: i + 1,
                                        mota: respone.data[i].mota,
                                        id: respone.data[i].id
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
                rowId: 'id',
                select: {
                    info: false
                },
                columns: [
                    { "data": "STT", },
                    { "data": "mota" }
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
                dom: "<'row'<'col-sm-12'tr>>" +
                    "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>",
                scroller: {
                    loadingIndicator: true
                },
            });
        } else {
            dataTableDuongSuDung.ajax.reload();
        }
        hideLoading();
    };

    $scope.Refesh = function () {
        $scope.LoadPage(0);
    };
   
    $scope.edit = function () {
        var seletedRow = dataTableDuongSuDung.rows({ selected: true });
        var count = seletedRow.count();
        if (count > 0) {
            $scope.ParamIdSeleted = seletedRow.data()[0].ID;
        } else {
            $scope.ParamIdSeleted = 0;
        }

        if ($scope.ParamIdSeleted > 0 && $scope.ParamIdSeleted != undefined) {
            var modalInstance = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: '/DuongSuDung/_Edit',
                controller: 'edit',
                size: 'xl',
                backdrop: 'static',
                resolve: {
                    itemId: function () {
                        return $scope.ParamIdSeleted;
                    }
                }
            });

            //kết quả trả về của modal
            modalInstance.result.then(function (response) {
                $scope.LoadPage(0);
            });
        } else {
            toastr.error("Bạn chưa chọn bản ghi nào.");
        }
        
    };
   
});
