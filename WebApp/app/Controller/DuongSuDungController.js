app.controller("DuongSuDungController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "hoten DESC";

    var dataTableCustomer = null;
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
            url: '/Customer/GetBottomAction',
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

    $('#dataTableCustomer').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });

    $scope.LoadPage = function (genTable) {
        showToast();
        //$.ajax({
        //    type: 'post',
        //    url: '/Customer/GetAll',
        //    data: $scope.modelSearch,
        //    success: function (data) {
        //        $scope.modelSearch.totalItems = data.totalItems;
        //        $scope.ListCustomer = data.data;
        //        $scope.modelSearch.pageSize = data.pageSize;
        //        $scope.$apply();
        //        hideLoading();
        //    }
        //});

        $scope.ListCustomer = [];
        if (genTable == 1) {
            dataTableCustomer = $('#dataTableCustomer').DataTable({
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
                        url: '/Customer/GetAll',
                        cache: false,
                        async: false,
                        data: $scope.modelSearch,
                        success: function (respone) {
                            totalItems = respone.totalItems;
                            $scope.ListCustomer = respone.data;
                            if (respone.data != null && respone.data.length > 0) {
                                for (var i = 0; i < respone.data.length; i++) {
                                    var tmp = {
                                        STT: i + 1,
                                        makh: respone.data[i].makh,
                                        hoten: respone.data[i].hoten,
                                        gioitinh: respone.data[i].gioitinh,
                                        namsinh: respone.data[i].namsinh,
                                        city_code: respone.data[i].city_code,
                                        LoaiDoiTuong: respone.data[i].LoaiDoiTuong,
                                        ngaytiepcantext: respone.data[i].ngaytiepcantext,
                                        CityName: respone.data[i].CityName,
                                        GioiTinhText: respone.data[i].GioiTinhText,
                                        sodienthoai: respone.data[i].sodienthoai,
                                        khachhang_id: respone.data[i].khachhang_id,
                                        diachi: respone.data[i].diachi
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
                    { "data": "STT", },
                    { "data": "makh" },
                    { "data": "hoten" },
                    { "data": "GioiTinhText" },
                    { "data": "namsinh" },
                    { "data": "LoaiDoiTuong" },
                    { "data": "ngaytiepcantext" },
                    { "data": "sodienthoai" },
                    { "data": "CityName" },
                    { "data": "diachi" }
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
   
});

app.controller('edit', function ($scope, $uibModalInstance, itemId, $ngConfirm, showToast, hideLoading) {

    $scope.model = {};
    angular.element(document).ready(function () {
        $.ajax({
            type: 'post',
            url: '/Customer/GetItemByID',
            data: { Id: itemId },
            success: function (data) {
                if (data.Error) {
                    toastr.error(data.Title);
                } else {
                    $scope.model = data.data;
                    $scope.$apply();
                }
            }
        });
    });
    $scope.ListValueType = [{ value: 'STRING', text: 'Kiểu chữ' }, { value: 'NUMBER', text: 'Kiểu số' }];
    $scope.submit = function () {
        $("#formSubmit").validate({
            rules: {
                ParamCode: {
                    required: true,
                    maxlength: 50
                },
                ParamValue: {
                    required: true,
                    maxlength: 500
                },
                ParamValueType: {
                    required: true
                },
                Desctiption: {
                    maxlength: 1000
                }
            },
            messages: {
                ParamCode: {
                    required: "Vui lòng nhập mã",
                    maxlength: "Mã không được vượt quá 50 ký tự"
                },
                ParamValue: {
                    required: "Vui lòng nhập giá trị",
                    maxlength: "Mã không được vượt quá 500 ký tự"
                },
                ParamValueType: {
                    required: "Vui lòng chọn kiểu dữ liệu"
                }
            }
        });
        if ($("#formSubmit").valid()) {
            // kiểm tra xem có nhập đúng kiểu dữ liệu không
            if ($scope.model.ParamValueType == 'NUMBER' && !(!isNaN($scope.model.ParamValue) && angular.isNumber(+$scope.model.ParamValue))) {
                toastr.error("Bạn nhập không đúng kiểu số.");
                return;
            }

            $.ajax({
                type: 'post',
                url: '/Customer/Edit',
                data: $scope.model,
                success: function (data) {
                    if (data.Error) {
                        toastr.error(data.Title);
                    } else {
                        toastr.success(data.Title);
                        $scope.cancel();
                    }
                }
            });
        }
    };
    
    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});