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

    $scope.RoleBtnExportExcel = false;
    $scope.RoleBtnSearch = false;

    function GetBottomAction() {
        $.ajax({
            type: 'post',
            url: '/KetQuaASSIST/GetBottomAction',
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
        if (genTable == 1 || genTable == 2) {
            if (genTable == 2)
                dataTableKetQuaASSIST.destroy();
            dataTableKetQuaASSIST = $('#dataTableKetQuaASSIST').DataTable({
                lengthMenu: [10, 20, 30, 50, 60, 100],
                //serverSide: true,
                ordering: false,
                searching: true,
                processing: true,
                //ajax: function (data, callback, settings) {
                //    var dataUser = [];
                //    var totalItems = 0;
                //    var page = ((data.start / data.length) + 1);

                //    $scope.modelSearch.currentPage = page;
                //    $scope.modelSearch.pageSize = data.length;

                //    $.ajax({
                //        type: 'post',
                //        url: '/KetQuaASSIST/GetAll',
                //        cache: false,
                //        async: false,
                //        data: $scope.modelSearch,
                //        success: function (respone) {
                //            totalItems = respone.totalItems;
                //            $scope.ListKetQuaASSIST = respone.data;
                //            if (respone.data != null && respone.data.length > 0) {
                //                for (var i = 0; i < respone.data.length; i++) {
                //                    var tmp = {
                //                        kqslassist_id: respone.data[i].kqslassist_id,
                //                        khachhang_id: respone.data[i].khachhang_id,
                //                        ngaysl: respone.data[i].ngaysl,
                //                        ngaysl_date: respone.data[i].ngaysl_date,
                //                        ngaysl_month: respone.data[i].ngaysl_month,
                //                        ngaysl_year: respone.data[i].ngaysl_year,
                //                        thuocla_diem: respone.data[i].thuocla_diem,
                //                        thuocla_nguyco: respone.data[i].thuocla_nguyco,
                //                        conruou_diem: respone.data[i].conruou_diem,
                //                        conruou_nguyco: respone.data[i].conruou_nguyco,
                //                        cansa_diem: respone.data[i].cansa_diem,
                //                        cansa_nguyco: respone.data[i].cansa_nguyco,
                //                        cocaine_diem: respone.data[i].cocaine_diem,
                //                        cocaine_nguyco: respone.data[i].cocaine_nguyco,
                //                        matuyda_diem: respone.data[i].matuyda_diem,
                //                        matuyda_nguyco: respone.data[i].matuyda_nguyco,
                //                        khixonghit_diem: respone.data[i].khixonghit_diem,
                //                        khixonghit_nguyco: respone.data[i].khixonghit_nguyco,
                //                        thuocanthan_diem: respone.data[i].thuocanthan_diem,
                //                        thuocanthan_nguyco: respone.data[i].thuocanthan_nguyco,
                //                        chatgayaogiac_diem: respone.data[i].chatgayaogiac_diem,
                //                        chatgayaogiac_nguyco: respone.data[i].chatgayaogiac_nguyco,
                //                        thuocphien_diem: respone.data[i].thuocphien_diem,
                //                        thuocphien_nguyco: respone.data[i].thuocphien_nguyco,
                //                        chatkhac_diem: respone.data[i].chatkhac_diem,
                //                        chatkhac_nguyco: respone.data[i].chatkhac_nguyco,
                //                        ghichu: respone.data[i].ghichu,
                //                        manhom_tbh: respone.data[i].manhom_tbh,
                //                        city_code: respone.data[i].city_code,
                //                        hoten: respone.data[i].hoten,
                //                        makh: respone.data[i].makh,
                //                        CityName: respone.data[i].CityName,
                //                        ngaysltext: respone.data[i].ngaysltext,
                //                        tennhom_tbh: respone.data[i].tennhom_tbh
                //                    }
                //                    dataUser.push(tmp);
                //                }
                //            }
                //        }
                //    });

                //    setTimeout(function () {
                //        callback({
                //            draw: data.draw,
                //            data: dataUser,
                //            recordsTotal: totalItems,
                //            recordsFiltered: totalItems
                //        });
                //    }, 50);
                //},
                //rowId: 'kqslassist_id',
                //select: {
                //    info: false
                //},
                columns: [
                    { "data": "CityName", searchBuilderType: "string" },
                    { "data": "manhom_tbh", searchBuilderType: "string" },
                    { "data": "tennhom_tbh", searchBuilderType: "string" },
                    { "data": "makh", searchBuilderType: "string" },
                    { "data": "hoten", searchBuilderType: "string"},
                    //{ "data": "ngaysltext", searchBuilderType: "string" },
                    { "data": "ngaysl", searchBuilderType: "date" },
                    { "data": "thuocla_diem", searchBuilderType: "number" },
                    { "data": "thuocla_nguyco", searchBuilderType: "string" },
                    { "data": "conruou_diem", searchBuilderType: "number" },
                    { "data": "conruou_nguyco", searchBuilderType: "string" },
                    { "data": "cansa_diem", searchBuilderType: "number" },
                    { "data": "cansa_nguyco", searchBuilderType: "string"},
                    { "data": "cocaine_diem", searchBuilderType: "number" },
                    { "data": "cocaine_nguyco", searchBuilderType: "string" },
                    { "data": "matuyda_diem", searchBuilderType: "number" },
                    { "data": "matuyda_nguyco", searchBuilderType: "string" },
                    { "data": "khixonghit_diem", searchBuilderType: "number"},
                    { "data": "khixonghit_nguyco", searchBuilderType: "string"},
                    { "data": "thuocanthan_diem", searchBuilderType: "number" },
                    { "data": "thuocanthan_nguyco", searchBuilderType: "string"},
                    { "data": "chatgayaogiac_diem", searchBuilderType: "number" },
                    { "data": "chatgayaogiac_nguyco", searchBuilderType: "string" },
                    { "data": "thuocphien_diem", searchBuilderType: "number"},
                    { "data": "thuocphien_nguyco", searchBuilderType: "string" },
                    { "data": "chatkhac_diem", searchBuilderType: "number" },
                    { "data": "chatkhac_nguyco", searchBuilderType: "string"}
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
                    "searchBuilder": {
                        "button": 'Tìm kiếm',
                        "add": 'Thêm điều kiện',
                        "clearAll": 'Xóa tất cả',
                        "condition": 'Điều kiện',
                        "conditions": {
                            "date": {
                                "before": 'Trước',
                                "after": 'Sau',
                                "equals": 'Bằng',
                                "not": 'Khác',
                                "between": 'Giữa',
                                "notBetween": 'Không phải ở giữa',
                                "empty": 'Trống',
                                "notEmpty": 'Không trống'
                            },
                            "moment": {
                                "before": 'Trước',
                                "after": 'Sau',
                                "equals": 'Bằng',
                                "not": 'Khác',
                                "between": 'Giữa',
                                "notBetween": 'Không phải ở giữa',
                                "empty": 'Trống',
                                "notEmpty": 'Không trống'
                            },
                            "number": {
                                "equals": 'Bằng',
                                "not": 'Khác',
                                "gt": 'Tốt hơn',
                                "gte": 'Lớn hơn hoặc bằng',
                                "lt": 'Ít hơn',
                                "lte": 'Ít hơn hoặc bằng',
                                "between": 'Giữa',
                                "notBetween": 'Không phải ở giữa',
                                "empty": 'Trống',
                                "notEmpty": 'Không trống'
                            },
                            "string": {
                                "contains": 'Chứa',
                                "notContains": 'Không chứa',
                                "empty": 'Trống',
                                "notEmpty": 'Không trống',
                                "equals": 'Bằng',
                                "not": 'Khác',
                                "endsWith": 'Kết thúc bằng',
                                "startsWith": 'Bắt đầu bằng',
                                "notEndsWith": 'Không kết thúc bằng',
                                "notStartsWith": 'Không bắt đầu bằng'
                            },
                        },
                        "data": 'Cột',
                        "logicAnd": 'Và',
                        "logicOr": 'Hoặc',
                        "title": {
                            0: 'Các điều kiện tìm kiếm',
                            _: 'Đã lọc (%d)'
                        },
                        "deleteTitle": 'Xóa',
                        "leftTitle": 'Bên trái',
                        "rightTitle": 'Đúng',
                        "value": 'Giá trị',
                    }
                },
                dom:
                    "<'row'<'col-sm-8'Q>>" +
                    "<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                    "<'row'<'col-sm-12'tr>>" +
                    "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>",
                scroller: {
                    loadingIndicator: true
                },
                buttons: [
                    {
                        extend: 'excelHtml5',
                        title: 'Xuất excel'
                    }
                ]
                ,
                columnDefs: [{
                    searchBuilder: {
                        defaultCondition: "="
                    },
                    targets: [1]
                }]
            });
        } else {
            dataTableKetQuaASSIST.ajax.reload();
        }
        hideLoading();
    };

    $scope.Refesh = function () {
        $scope.LoadPage(0);
    };

    $scope.ExportExcel = function () {
        window.location.href = '/KetQuaASSIST/ExportData?keyword=' + $scope.modelSearch.KeyWord;
    }

});
