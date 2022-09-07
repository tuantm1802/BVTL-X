app.controller("KetQuaHIVController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "kqxnhiv_id";

    var dataTableKetQuaHIV = null;
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
            url: '/KetQuaHIV/GetBottomAction',
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

    $('#dataTableKetQuaHIV').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });

    $scope.LoadPage = function (genTable) {
        showToast();
        
        $scope.ListKetQuaHIV = [];
        if (genTable == 1) {
            dataTableKetQuaHIV = $('#dataTableKetQuaHIV').DataTable({
                lengthMenu: [10, 20, 30, 50, 60, 100],
                serverSide: true,
                ordering: false,
                searching: true,
                ajax: function (data, callback, settings) {
                    var dataUser = [];
                    var totalItems = 0;
                    var page = ((data.start / data.length) + 1);

                    $scope.modelSearch.currentPage = page;
                    $scope.modelSearch.pageSize = data.length;
                    // Lấy điều kiện tìm kiếm
                    var input = $('.dataTables_filter input')[0];
                    $scope.modelSearch.KeyWord = input.value;

                    $.ajax({
                        type: 'post',
                        url: '/KetQuaHIV/GetAll',
                        cache: false,
                        async: false,
                        data: $scope.modelSearch,
                        success: function (respone) {
                            totalItems = respone.totalItems;
                            $scope.ListKetQuaHIV = respone.data;
                            if (respone.data != null && respone.data.length > 0) {
                                for (var i = 0; i < respone.data.length; i++) {
                                    var tmp = {
                                        kqxnhiv_id: respone.data[i].kqxnhiv_id,
                                        khachhang_id: respone.data[i].khachhang_id,
                                        ngayxn: respone.data[i].ngayxn,
                                        ngayxn_date: respone.data[i].ngayxn_date,
                                        ngayxn_month: respone.data[i].ngayxn_month,
                                        ngayxn_year: respone.data[i].ngayxn_year,
                                        ketqua: respone.data[i].ketqua,
                                        dangdieutri_hiv: respone.data[i].dangdieutri_hiv,
                                        manhom_tbh: respone.data[i].manhom_tbh,
                                        city_code: respone.data[i].city_code,
                                        hoten: respone.data[i].hoten,
                                        makh: respone.data[i].makh,
                                        CityName: respone.data[i].CityName,
                                        ngayxntext: respone.data[i].ngayxntext,
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
                rowId: 'khachhang_id',
                select: {
                    info: false
                },
                columns: [
                    { "data": "CityName", },
                    { "data": "manhom_tbh" },
                    { "data": "tennhom_tbh" },
                    { "data": "makh" },
                    { "data": "hoten" },
                    { "data": "ngayxntext" },
                    { "data": "ketqua", },
                    { "data": "dangdieutri_hiv" }
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
            dataTableKetQuaHIV.ajax.reload();
        }
        hideLoading();
    };

    $scope.Refesh = function () {
        $scope.LoadPage(0);
    };
   
   
});
