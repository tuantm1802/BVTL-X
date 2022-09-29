app.controller("BaoCaoTongHopController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "record_id";

    var dataTableBaoCaoTongHop = null;
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
            url: '/BaoCaoTongHop/GetBottomAction',
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

    $('#dataTableBaoCaoTongHop').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });

    $scope.LoadPage = function (genTable) {
        showToast();

        $scope.ListBaoCaoTongHop = [];
        if (genTable == 1 || genTable == 2) {
            if (genTable == 2)
                dataTableBaoCaoTongHop.destroy();
            dataTableBaoCaoTongHop = $('#dataTableBaoCaoTongHop').DataTable({
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
                //        url: '/BaoCaoTongHop/GetAll',
                //        cache: false,
                //        async: false,
                //        data: $scope.modelSearch,
                //        success: function (respone) {
                //            totalItems = respone.totalItems;
                //            $scope.ListBaoCaoTongHop = respone.data;
                //            if (respone.data != null && respone.data.length > 0) {
                //                for (var i = 0; i < respone.data.length; i++) {
                //                    var tmp = {
                //                        kqslace_id: respone.data[i].kqslace_id,
                //                        khachhang_id: respone.data[i].khachhang_id,
                //                        ngaysl: respone.data[i].ngaysl,
                //                        ngaysl_date: respone.data[i].ngaysl_date,
                //                        ngaysl_month: respone.data[i].ngaysl_month,
                //                        ngaysl_year: respone.data[i].ngaysl_year,
                //                        tongdiem_ace: respone.data[i].tongdiem_ace,
                //                        ketqua_ace: respone.data[i].ketqua_ace,
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
                //rowId: 'kqslace_id',
                //select: {
                //    info: false
                //},
                columns: [
                    { "data": "CityName", searchBuilderType: "string" },
                    { "data": "manhom_tbh", searchBuilderType: "string"},
                    { "data": "tennhom_tbh", searchBuilderType: "string"},
                    { "data": "makh", searchBuilderType: "string"},
                    { "data": "hoten", searchBuilderType: "string"},
                    { "data": "ngaysl", searchBuilderType: "date" },
                    { "data": "record_id", searchBuilderType: "number" },
                    { "data": "hanhvinguyco_timestamp", searchBuilderType: "date" },
                    { "data": "chatgaynghien", searchBuilderType: "string" },
                    { "data": "loaikhac", searchBuilderType: "string" },
                    { "data": "chatgaynghien_2", searchBuilderType: "string" },
                    { "data": "loaikhac_2", searchBuilderType: "string" },
                    { "data": "duongsd", searchBuilderType: "string" },
                    { "data": "sdheroin", searchBuilderType: "string" },
                    { "data": "tansuatda", searchBuilderType: "string" },
                    { "data": "tansuat_heroin", searchBuilderType: "string" },
                    { "data": "tuoi", searchBuilderType: "number" },
                    { "data": "matuydautien", searchBuilderType: "string" },
                    { "data": "tiemchich", searchBuilderType: "string" },
                    { "data": "dungchung", searchBuilderType: "string" },
                    { "data": "qhtd", searchBuilderType: "string" },
                    { "data": "qhtd_2", searchBuilderType: "string" },
                    { "data": "sdmatuy", searchBuilderType: "string" },
                    { "data": "qhtdtt", searchBuilderType: "string" },
                    { "data": "bandam", searchBuilderType: "string" },
                    { "data": "sti", searchBuilderType: "string" },
                    { "data": "sti1", searchBuilderType: "string" },
                    { "data": "loaikhac_3", searchBuilderType: "string" },
                    { "data": "quakhu", searchBuilderType: "string" },
                    { "data": "hientai", searchBuilderType: "string" },
                    { "data": "hientai_2", searchBuilderType: "string" },
                    { "data": "quakhu2", searchBuilderType: "string" },
                    { "data": "hientai_4", searchBuilderType: "string" },
                    { "data": "hientai_3", searchBuilderType: "string" },
                    { "data": "quakhu_3", searchBuilderType: "string" },
                    { "data": "hientai_5", searchBuilderType: "string" },
                    { "data": "hientai_6", searchBuilderType: "string" },
                    { "data": "trieuchung", searchBuilderType: "string" },
                    { "data": "trieuchung_2", searchBuilderType: "string" },
                    { "data": "hiv_timestamp", searchBuilderType: "date" },
                    { "data": "cau1", searchBuilderType: "string" },
                    { "data": "cau2", searchBuilderType: "string" },
                    { "data": "cau3", searchBuilderType: "string" },
                    { "data": "cau4", searchBuilderType: "string" },
                    { "data": "cau5", searchBuilderType: "string" },
                    { "data": "cau6", searchBuilderType: "string" },
                    { "data": "cau7", searchBuilderType: "string" },
                    { "data": "cau8", searchBuilderType: "string" },
                    { "data": "cau9", searchBuilderType: "string" },
                    { "data": "cau10", searchBuilderType: "string" },
                    { "data": "cau11", searchBuilderType: "string" },
                    { "data": "cau12", searchBuilderType: "string" },
                    { "data": "cau13", searchBuilderType: "string" },
                    { "data": "cau14", searchBuilderType: "string" },
                    { "data": "cau15", searchBuilderType: "string" },
                    { "data": "cau16", searchBuilderType: "string" },
                    { "data": "cau17", searchBuilderType: "string" },
                    { "data": "cau18", searchBuilderType: "string" },
                    { "data": "cau19", searchBuilderType: "string" },
                    { "data": "cau20", searchBuilderType: "string" },
                    { "data": "cau21", searchBuilderType: "string" },
                    { "data": "cau22", searchBuilderType: "string" },
                    { "data": "cau23", searchBuilderType: "string" },
                    { "data": "cau24", searchBuilderType: "string" },
                    { "data": "cau25", searchBuilderType: "string" },
                    { "data": "cau26", searchBuilderType: "string" },
                    { "data": "cau27", searchBuilderType: "string" },
                    { "data": "assist_timestamp", searchBuilderType: "date" },
                    { "data": "thuocla", searchBuilderType: "string" },
                    { "data": "thucuong", searchBuilderType: "string" },
                    { "data": "cansa", searchBuilderType: "string" },
                    { "data": "cocain", searchBuilderType: "string" },
                    { "data": "chatkichthich", searchBuilderType: "string" },
                    { "data": "khixong", searchBuilderType: "string" },
                    { "data": "thuocanthan", searchBuilderType: "string" },
                    { "data": "chatgayaogiac", searchBuilderType: "string" },
                    { "data": "thuocphien", searchBuilderType: "string" },
                    { "data": "chatkhac", searchBuilderType: "string" },
                    { "data": "cacchatkhac", searchBuilderType: "string" },
                    { "data": "lucdihoc", searchBuilderType: "string" },
                    { "data": "thuocla1", searchBuilderType: "string" },
                    { "data": "thucuong1", searchBuilderType: "string" },
                    { "data": "cansa1", searchBuilderType: "string" },
                    { "data": "coca1", searchBuilderType: "string" },
                    { "data": "chatkichthich1", searchBuilderType: "string" },
                    { "data": "khixong1", searchBuilderType: "string" },
                    { "data": "thuocanthan1", searchBuilderType: "string" },
                    { "data": "chatgayaogiac1", searchBuilderType: "string" },
                    { "data": "chatthuocphien1", searchBuilderType: "string" },
                    { "data": "chatkhac1", searchBuilderType: "string" },
                    { "data": "thuocla2", searchBuilderType: "string" },
                    { "data": "thucuong2", searchBuilderType: "string" },
                    { "data": "cansa2", searchBuilderType: "string" },
                    { "data": "coca2", searchBuilderType: "string" },
                    { "data": "chatkichthich2", searchBuilderType: "string" },
                    { "data": "khixong2", searchBuilderType: "string" },
                    { "data": "thuocanthan2", searchBuilderType: "string" },
                    { "data": "chatgayaogiac2", searchBuilderType: "string" },
                    { "data": "chatthuocphien2", searchBuilderType: "string" },
                    { "data": "chatkhac2", searchBuilderType: "string" },
                    { "data": "thuocla3", searchBuilderType: "string" },
                    { "data": "thucuong3", searchBuilderType: "string" },
                    { "data": "cansa3", searchBuilderType: "string" },
                    { "data": "coca3", searchBuilderType: "string" },
                    { "data": "chatkichthich3", searchBuilderType: "string" },
                    { "data": "khixong3", searchBuilderType: "string" },
                    { "data": "thuocanthan3", searchBuilderType: "string" },
                    { "data": "chatgayaogiac3", searchBuilderType: "string" },
                    { "data": "chatthuocphien3", searchBuilderType: "string" },
                    { "data": "chatkhac3", searchBuilderType: "string" },
                    { "data": "thucuong4", searchBuilderType: "string" },
                    { "data": "cansa4", searchBuilderType: "string" },
                    { "data": "coca4", searchBuilderType: "string" },
                    { "data": "chatkichthich4", searchBuilderType: "string" },
                    { "data": "khixong4", searchBuilderType: "string" },
                    { "data": "thuocanthan4", searchBuilderType: "string" },
                    { "data": "chatgayaogiac4", searchBuilderType: "string" },
                    { "data": "chatthuocphien4", searchBuilderType: "string" },
                    { "data": "chatkhac4", searchBuilderType: "string" },
                    { "data": "thuocla5", searchBuilderType: "string" },
                    { "data": "thucuong5", searchBuilderType: "string" },
                    { "data": "cansa5", searchBuilderType: "string" },
                    { "data": "coca5", searchBuilderType: "string" },
                    { "data": "chatkichthich5", searchBuilderType: "string" },
                    { "data": "khixong5", searchBuilderType: "string" },
                    { "data": "thuocanthan5", searchBuilderType: "string" },
                    { "data": "chatgayaogiac5", searchBuilderType: "string" },
                    { "data": "chatthuocphien5", searchBuilderType: "string" },
                    { "data": "chatkhac5", searchBuilderType: "string" },
                    { "data": "thuocla6", searchBuilderType: "string" },
                    { "data": "thucuong6", searchBuilderType: "string" },
                    { "data": "cansa6", searchBuilderType: "string" },
                    { "data": "coca6", searchBuilderType: "string" },
                    { "data": "chatkichthich6", searchBuilderType: "string" },
                    { "data": "khixong6", searchBuilderType: "string" },
                    { "data": "thuocanthan6", searchBuilderType: "string" },
                    { "data": "chatgayaogiac6", searchBuilderType: "string" },
                    { "data": "chatthuocphien6", searchBuilderType: "string" },
                    { "data": "chatkhac6", searchBuilderType: "string" },
                    { "data": "cau_8", searchBuilderType: "string" },
                    { "data": "diemthuocla", searchBuilderType: "string" },
                    { "data": "diemthucuong", searchBuilderType: "string" },
                    { "data": "diemcansa", searchBuilderType: "string" },
                    { "data": "diemcoca", searchBuilderType: "string" },
                    { "data": "diemchatkichthich", searchBuilderType: "string" },
                    { "data": "diemkhixong", searchBuilderType: "string" },
                    { "data": "diemchatanthan", searchBuilderType: "string" },
                    { "data": "diemchatgayaogiac", searchBuilderType: "string" },
                    { "data": "diemchatthuocphien", searchBuilderType: "string" },
                    { "data": "qst_timestamp", searchBuilderType: "date" },
                    { "data": "diemchatkhac", searchBuilderType: "string" },
                    { "data": "c_1a", searchBuilderType: "string" },
                    { "data": "c_1b", searchBuilderType: "string" },
                    { "data": "c_1c", searchBuilderType: "string" },
                    { "data": "c_1d", searchBuilderType: "string" },
                    { "data": "c_2", searchBuilderType: "string" },
                    { "data": "c_3", searchBuilderType: "string" },
                    { "data": "c_4a", searchBuilderType: "string" },
                    { "data": "c_4b", searchBuilderType: "string" },
                    { "data": "c_4c", searchBuilderType: "string" },
                    { "data": "tongdiem", searchBuilderType: "number" }
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
            dataTableBaoCaoTongHop.ajax.reload();
        }
        hideLoading();
    };

    $scope.Refesh = function () {
        $scope.LoadPage(0);
    };

    $scope.ExportExcel = function () {
        window.location.href = '/BaoCaoTongHop/ExportData?keyword=' + $scope.modelSearch.KeyWord;
    }

});