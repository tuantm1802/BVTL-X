app.controller("KetQuaASSISTController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "kqslassist_id";

    var dataTableKetQuaASSIST = null;
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
            url: '/KetQuaASSIST/GetBottomAction',
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

    $('#dataTableKetQuaASSIST').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });

    $scope.LoadPage = function (genTable) {
        showToast();
        
        $scope.ListKetQuaASSIST = [];
        if (genTable == 1) {
            dataTableKetQuaASSIST = $('#dataTableKetQuaASSIST').DataTable({
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
                        url: '/KetQuaASSIST/GetAll',
                        cache: false,
                        async: false,
                        data: $scope.modelSearch,
                        success: function (respone) {
                            totalItems = respone.totalItems;
                            $scope.ListKetQuaASSIST = respone.data;
                            if (respone.data != null && respone.data.length > 0) {
                                for (var i = 0; i < respone.data.length; i++) {
                                    var tmp = {
                                        kqslassist_id: respone.data[i].kqslassist_id,
                                        khachhang_id: respone.data[i].khachhang_id,
                                        ngaysl: respone.data[i].ngaysl,
                                        ngaysl_date: respone.data[i].ngaysl_date,
                                        ngaysl_month: respone.data[i].ngaysl_month,
                                        ngaysl_year: respone.data[i].ngaysl_year,
                                        thuocla_diem: respone.data[i].thuocla_diem,
                                        thuocla_nguyco: respone.data[i].thuocla_nguyco,
                                        conruou_diem: respone.data[i].conruou_diem,
                                        conruou_nguyco: respone.data[i].conruou_nguyco,
                                        cansa_diem: respone.data[i].cansa_diem,
                                        cansa_nguyco: respone.data[i].cansa_nguyco,
                                        cocaine_diem: respone.data[i].cocaine_diem,
                                        cocaine_nguyco: respone.data[i].cocaine_nguyco,
                                        matuyda_diem: respone.data[i].matuyda_diem,
                                        matuyda_nguyco: respone.data[i].matuyda_nguyco,
                                        khixonghit_diem: respone.data[i].khixonghit_diem,
                                        khixonghit_nguyco: respone.data[i].khixonghit_nguyco,
                                        thuocanthan_diem: respone.data[i].thuocanthan_diem,
                                        thuocanthan_nguyco: respone.data[i].thuocanthan_nguyco,
                                        chatgayaogiac_diem: respone.data[i].chatgayaogiac_diem,
                                        chatgayaogiac_nguyco: respone.data[i].chatgayaogiac_nguyco,
                                        thuocphien_diem: respone.data[i].thuocphien_diem,
                                        thuocphien_nguyco: respone.data[i].thuocphien_nguyco,
                                        chatkhac_diem: respone.data[i].chatkhac_diem,
                                        chatkhac_nguyco: respone.data[i].chatkhac_nguyco,
                                        ghichu: respone.data[i].ghichu,
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
                rowId: 'kqslassist_id',
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
                    { "data": "thuocla_diem" },
                    { "data": "thuocla_nguyco" },
                    { "data": "conruou_diem" },
                    { "data": "conruou_nguyco" },
                    { "data": "cansa_diem" },
                    { "data": "cansa_nguyco" },
                    { "data": "cocaine_diem" },
                    { "data": "cocaine_nguyco" },
                    { "data": "matuyda_diem" },
                    { "data": "matuyda_nguyco" },
                    { "data": "khixonghit_diem" },
                    { "data": "khixonghit_nguyco" },
                    { "data": "thuocanthan_diem" },
                    { "data": "thuocanthan_nguyco" },
                    { "data": "chatgayaogiac_diem" },
                    { "data": "chatgayaogiac_nguyco" },
                    { "data": "thuocphien_diem" },
                    { "data": "thuocphien_nguyco" },
                    { "data": "chatkhac_diem" },
                    { "data": "chatkhac_nguyco" }
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
            dataTableKetQuaASSIST.ajax.reload();
        }
        hideLoading();
    };

    $scope.Refesh = function () {
        $scope.LoadPage(0);
    };
   
});
