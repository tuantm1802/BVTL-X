app.controller("CustomerController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "hoten";
    var editor;
    var dataTableCustomer = null;
    $scope.ParamIdSeleted = 0;
    angular.element(document).ready(function () {

        //editor = new $.fn.dataTable.Editor({
        //    "ajax": "http://localhost:54888/api/staff",
        //    "table": "#example",
        //    "fields": [{
        //        "label": "First name:",
        //        "name": "first_name"
        //    }, {
        //        "label": "Last name:",
        //        "name": "last_name"
        //    }, {
        //        "label": "Position:",
        //        "name": "position"
        //    }, {
        //        "label": "Office:",
        //        "name": "office"
        //    }, {
        //        "label": "Extension:",
        //        "name": "extn"
        //    }, {
        //        "label": "Start date:",
        //        "name": "start_date",
        //        "type": "datetime"
        //    }, {
        //        "label": "Salary:",
        //        "name": "salary"
        //    }
        //    ]
        //});

        GetBottomAction();
        $scope.LoadPage(1);

        $('#example').DataTable({
            dom: 'Qlfrtip'
        });

        //$('#example').DataTable({
        //    dom: "QBfrtip",
        //    ajax: {
        //        url: "http://localhost:54888/api/staff",
        //        type: "POST"
        //    },
        //    serverSide: true,
        //    columns: [
        //        { data: "first_name" },
        //        { data: "last_name" },
        //        { data: "position" },
        //        { data: "office" },
        //        { data: "start_date" },
        //        { data: "salary", render: $.fn.dataTable.render.number(',', '.', 0, '$') }
        //    ],
        //    select: true,
        //    buttons: [
        //        { extend: "create", editor: editor },
        //        { extend: "edit", editor: editor },
        //        { extend: "remove", editor: editor }
        //    ]
        //});
    });

    $scope.RoleBtnExportExcel = false;
    $scope.RoleBtnSearch = false;

    function GetBottomAction() {
        $.ajax({
            type: 'post',
            url: '/Customer/GetBottomAction',
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

    $('#dataTableCustomer').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });
    $scope.ListCustomer1 = [];
    $scope.LoadPage = function (genTable) {
        showToast();
        //$scope.ListCustomer1 = [];
        //$.ajax({
        //    type: 'post',
        //    url: '/Customer/GetAll',
        //    cache: false,
        //    async: false,
        //    data: $scope.modelSearch,
        //    success: function (respone) {
        //        $scope.modelSearch.totalItems = respone.totalItems;
        //        $scope.ListCustomer = respone.data;
        //        if (respone.data != null && respone.data.length > 0) {
        //            for (var i = 0; i < respone.data.length; i++) {
        //                var tmp = {
        //                    STT: i + 1,
        //                    makh: respone.data[i].makh,
        //                    hoten: respone.data[i].hoten,
        //                    //gioitinh: respone.data[i].gioitinh,
        //                    namsinh: respone.data[i].namsinh,
        //                    //city_code: respone.data[i].city_code,
        //                    LoaiDoiTuong: respone.data[i].LoaiDoiTuong,
        //                    ngaytiepcantext: respone.data[i].ngaytiepcantext,
        //                    CityName: respone.data[i].CityName,
        //                    GioiTinhText: respone.data[i].GioiTinhText,
        //                    sodienthoai: respone.data[i].sodienthoai,
        //                    // khachhang_id: respone.data[i].khachhang_id,
        //                    diachi: respone.data[i].diachi
        //                }
        //                $scope.ListCustomer1.push(tmp);
        //            }
        //        }
        //    }
        //});

        $scope.ListCustomer = [];
        if (genTable == 1 || genTable == 2) {
            if (genTable == 2)
                dataTableCustomer.destroy();

            dataTableCustomer = $('#dataTableCustomer').DataTable({
                lengthMenu: [10, 20, 30, 50, 60, 100],
               // serverSide: true,
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
                //        url: '/Customer/GetAll',
                //        cache: false,
                //        async: false,
                //        data: $scope.modelSearch,
                //        success: function (respone) {
                //            totalItems = respone.totalItems;
                //            $scope.ListCustomer = respone.data;
                //            if (respone.data != null && respone.data.length > 0) {
                //                for (var i = 0; i < respone.data.length; i++) {
                //                    var tmp = {
                //                        STT: i + 1,
                //                        makh: respone.data[i].makh,
                //                        hoten: respone.data[i].hoten,
                //                        gioitinh: respone.data[i].gioitinh,
                //                        namsinh: respone.data[i].namsinh,
                //                        city_code: respone.data[i].city_code,
                //                        LoaiDoiTuong: respone.data[i].LoaiDoiTuong,
                //                        ngaytiepcantext: respone.data[i].ngaytiepcantext,
                //                        CityName: respone.data[i].CityName,
                //                        GioiTinhText: respone.data[i].GioiTinhText,
                //                        sodienthoai: respone.data[i].sodienthoai,
                //                        khachhang_id: respone.data[i].khachhang_id,
                //                        diachi: respone.data[i].diachi
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
                //rowId: 'khachhang_id',
                //select: {
                //    info: false
                //},
                columns: [
                    { "data": "STT", searchBuilderType: "number" },
                    { "data": "makh", searchBuilderType: "string" },
                    { "data": "hoten", searchBuilderType: "string" },
                    { "data": "GioiTinhText", searchBuilderType: "string" },
                    { "data": "namsinh", searchBuilderType: "string" },
                    { "data": "LoaiDoiTuong", searchBuilderType: "string" },
                    { "data": "ngaytiepcantext", searchBuilderType: "string" },
                    { "data": "sodienthoai", searchBuilderType: "string" },
                    { "data": "CityName", searchBuilderType: "string" },
                    { "data": "diachi", searchBuilderType: "string" }
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
                        "logicAnd": 'Hủy bỏ',
                        "logicOr": 'OU',
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
            dataTableCustomer.ajax.reload();
        }
        hideLoading();
    };

    $scope.Refesh = function () {
        $scope.LoadPage(0);
    };

    $scope.edit = function () {
        var seletedRow = dataTableCustomer.rows({ selected: true });
        var count = seletedRow.count();
        if (count > 0) {
            $scope.ParamIdSeleted = seletedRow.data()[0].ID;
        } else {
            $scope.ParamIdSeleted = 0;
        }

        if ($scope.ParamIdSeleted > 0 && $scope.ParamIdSeleted != undefined) {
            var modalInstance = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: '/Customer/_Edit',
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

    $scope.ExportExcel = function () {
        window.location.href = '/Customer/ExportData?keyword=' + $scope.modelSearch.KeyWord;
    }

});
