app.controller("KetQuaACEController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "kqslace_id";

    var dataTableKetQuaACE = null;
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
            url: '/KetQuaACE/GetBottomAction',
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

    $('#dataTableKetQuaACE').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });

    $scope.LoadPage = function (genTable) {
        showToast();

        $scope.ListKetQuaACE = [];
        if (genTable == 1 || genTable == 2) {
            if (genTable == 2)
                dataTableKetQuaACE.destroy();
            dataTableKetQuaACE = $('#dataTableKetQuaACE').DataTable({
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
                        url: '/KetQuaACE/GetAll',
                        cache: false,
                        async: false,
                        data: $scope.modelSearch,
                        success: function (respone) {
                            totalItems = respone.totalItems;
                            $scope.ListKetQuaACE = respone.data;
                            if (respone.data != null && respone.data.length > 0) {
                                for (var i = 0; i < respone.data.length; i++) {
                                    var tmp = {
                                        kqslace_id: respone.data[i].kqslace_id,
                                        khachhang_id: respone.data[i].khachhang_id,
                                        ngaysl: respone.data[i].ngaysl,
                                        ngaysl_date: respone.data[i].ngaysl_date,
                                        ngaysl_month: respone.data[i].ngaysl_month,
                                        ngaysl_year: respone.data[i].ngaysl_year,
                                        tongdiem_ace: respone.data[i].tongdiem_ace,
                                        ketqua_ace: respone.data[i].ketqua_ace,
                                        manhom_tbh: respone.data[i].manhom_tbh,
                                        city_code: respone.data[i].city_code,
                                        hoten: respone.data[i].hoten,
                                        makh: respone.data[i].makh,
                                        CityName: respone.data[i].CityName,
                                        ngaysltext: respone.data[i].ngaysltext,
                                        tennhom_tbh: respone.data[i].tennhom_tbh
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
                rowId: 'kqslace_id',
                select: {
                    info: false
                },
                columns: [
                    { "data": "CityName", },
                    { "data": "manhom_tbh" },
                    { "data": "tennhom_tbh" },
                    { "data": "makh" },
                    { "data": "hoten" },
                    { "data": "ngaysltext" },
                    { "data": "tongdiem_ace" },
                    { "data": "ketqua_ace" }
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
                buttons: [
                    'colvis'
                ]
            });
        } else {
            dataTableKetQuaACE.ajax.reload();
        }
        hideLoading();
    };

    $scope.Refesh = function () {
        $scope.LoadPage(0);
    };

});