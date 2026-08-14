app.controller("TestGroupController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "manhom_tbh";
    $scope.ListData = [];

    var dataTableNhomTTDL = null;
    $scope.NhomTTDLIdSeleted = "";
    angular.element(document).ready(function () {

        GetBottomAction();
        $scope.LoadPage(1);
    });

    $scope.RoleBtnCreate = false;
    $scope.RoleBtnUpdate = false;
    $scope.RoleBtnSearch = false;
    $scope.RoleBtnDelete = false;

    function GetBottomAction() {
        $.ajax({
            type: 'post',
            url: '/TestGroup/GetBottomAction',
            data: {},
            success: function (response) {
                if (response.Buttoms != null) {
                    angular.forEach(response.Buttoms, function (item) {
                        if (item == 'btnCreate') {
                            $scope.RoleBtnCreate = true;
                        }
                        if (item == 'btnUpdate') {
                            $scope.RoleBtnUpdate = true;
                        }
                        if (item == 'btnSearch') {
                            $scope.RoleBtnSearch = true;
                        }
                        if (item == 'btnDelete') {
                            $scope.RoleBtnDelete = true;
                        }
                    });
                }
                $scope.$apply();
            }
        });
    }

    $('#dataTableNhomTTDL').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });

    $scope.LoadPage = function (genTable) {
        showToast();
        //$.ajax({
        //    type: 'post',
        //    url: '/TestGroup/GetAllByPage',
        //    data: $scope.modelSearch,
        //    success: function (data) {
        //        $scope.modelSearch.totalItems = data.totalItems;
        //        $scope.ListData = data.data;
        //      //  $scope.modelSearch.pageSize = data.pageSize;
        //        $scope.$apply();
        //         hideLoading();
        //    }
        //});

        $scope.ListData = [];
        if (genTable == 1) {
            dataTableNhomTTDL = $('#dataTableNhomTTDL').DataTable({
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
                        url: '/TestGroup/GetAllByPage',
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
                                        Code: respone.data[i].manhom_tbh,
                                        Name: respone.data[i].tennhom_tbh,
                                        //IsActive: respone.data[i].IsActive == true ? 'Sử dụng' : 'Không sử dụng',
                                        CityName: respone.data[i].CityName,
                                        city_code: respone.data[i].city_code
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
                rowId: 'Code',
                select: {
                    info: false
                },
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
                    { "data": "STT", },
                    { "data": "Code" },
                    { "data": "Name" },
                    { "data": "CityName" },
                ],
                dom: "<'row'<'col-sm-12'f>>" +
                    "<'row'<'col-sm-12'tr>>" +
                    "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>",

                scroller: {
                    loadingIndicator: true
                },
            });
        } else {
            dataTableNhomTTDL.ajax.reload();
        }


        hideLoading();
    };

    $scope.Refesh = function () {
        $scope.LoadPage(0);
    };


    $scope.add = function () {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/TestGroup/_Add',
            controller: 'add',
            size: 'xl',
            backdrop: 'static'
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            $scope.LoadPage(0);
        });
    };
    $scope.edit = function (maNhomCode) {
        var targetCode = maNhomCode || $scope.NhomTTDLIdSeleted;
        if (!targetCode && dataTableNhomTTDL && typeof dataTableNhomTTDL.rows === 'function') {
            var seletedRow = dataTableNhomTTDL.rows({ selected: true });
            if (seletedRow.count() > 0) {
                targetCode = seletedRow.data()[0].Code;
            }
        }
        $scope.NhomTTDLIdSeleted = targetCode;

        if ($scope.NhomTTDLIdSeleted != null && $scope.NhomTTDLIdSeleted != '' && $scope.NhomTTDLIdSeleted != undefined) {
            var modalInstance = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: '/TestGroup/_Edit',
                controller: 'edit',
                size: 'xl',
                backdrop: 'static',
                resolve: {
                    itemId: function () {
                        return $scope.NhomTTDLIdSeleted;
                    }
                }
            });

            //kết quả trả về của modal
            modalInstance.result.then(function (response) {
                if (window.alpineTestGroupInstance) {
                    window.alpineTestGroupInstance.LoadPage(window.alpineTestGroupInstance.modelSearch.currentPage);
                } else {
                    $scope.LoadPage(0);
                }
            });
        } else {
            toastr.error("Bạn chưa chọn bản ghi nào.");
        }
    };
    $scope.delete = function (maNhomCode) {
        var targetCode = maNhomCode || $scope.NhomTTDLIdSeleted;
        if (!targetCode && dataTableNhomTTDL && typeof dataTableNhomTTDL.rows === 'function') {
            var seletedRow = dataTableNhomTTDL.rows({ selected: true });
            if (seletedRow.count() > 0) {
                targetCode = seletedRow.data()[0].Code;
            }
        }
        $scope.NhomTTDLIdSeleted = targetCode;

        if ($scope.NhomTTDLIdSeleted != null && $scope.NhomTTDLIdSeleted != '' && $scope.NhomTTDLIdSeleted != undefined) {

            var nameItem = ($scope.ListData || []).filter(function (item) {
                return item.manhom_tbh === $scope.NhomTTDLIdSeleted;
            })[0];
            var name = nameItem ? nameItem.tennhom_tbh : 'nhóm này';

            $ngConfirm({
                title: 'Thông báo',
                content: 'Bạn có chắc chắn muốn xóa nhóm thu thập dữ liệu ' + name + ' không?',
                scope: $scope,
                buttons: {
                    delete: {
                        text: 'Xóa',
                        btnClass: 'btn-blue',
                        action: function (scope, button) {
                            $.ajax({
                                type: 'post',
                                url: '/TestGroup/Delete',
                                data: { maNhom: $scope.NhomTTDLIdSeleted },
                                success: function (data) {
                                    if (data.Error) {
                                        toastr.error(data.Title);
                                    } else {
                                        toastr.success(data.Title);
                                        if (window.alpineTestGroupInstance) {
                                            window.alpineTestGroupInstance.LoadPage(window.alpineTestGroupInstance.modelSearch.currentPage);
                                        } else {
                                            $scope.LoadPage(0);
                                        }
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

app.controller('add', function ($scope, $uibModalInstance, $ngConfirm, showToast, hideLoading) {
    $scope.ListCity = [];
    $scope.ListCityCode = [];
    angular.element(document).ready(function () {
        showToast();
        GetDanhMuc();
    });

    function GetDanhMuc() {
        $scope.ListCity = [];
        $.ajax({
            type: 'post',
            url: '/TestGroup/GetDanhMuc',
            cache: false,
            async: false,
            data: {},
            success: function (data) {

                if (data.Citys != null && data.Citys.length > 0) {
                    $scope.ListCity = data.Citys;
                }

                $scope.$apply();
                hideLoading();
            }
        });
    }

    $scope.model = {};
    $scope.submit = function () {
        $("#formSubmit").validate({
            errorElement: 'span',
            errorClass: 'error invalid-feedback',
            errorPlacement: function (error, element) {
                if (element.parent('.input-group').length) {
                    error.insertAfter(element.parent());
                } else if (element.hasClass('select2-hidden-accessible')) {
                    error.insertAfter(element.next('.select2-container'));
                } else {
                    error.insertAfter(element);
                }
            },
            highlight: function (element) {
                $(element).addClass('is-invalid');
            },
            unhighlight: function (element) {
                $(element).removeClass('is-invalid');
            },
            rules: {
                manhom_tbh: {
                    required: true,
                    maxlength: 50
                },
                tennhom_tbh: {
                    required: true,
                    maxlength: 250
                },
                city_code: {
                    required: true
                }
            },
            messages: {
                manhom_tbh: {
                    required: "Vui lòng nhập mã nhóm",
                    maxlength: "Mã nhóm không được vượt quá 50 ký tự"
                },
                tennhom_tbh: {
                    required: "Vui lòng nhập tên nhóm",
                    maxlength: "Tên nhóm không được vượt quá 250 ký tự"
                },
                city_code: {
                    required: "Vui lòng chọn tỉnh thành",
                }
            }
        });
        if ($("#formSubmit").valid()) {

            $.ajax({
                type: 'post',
                url: '/TestGroup/Add',
                data: { model: $scope.model },
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

app.controller('edit', function ($scope, $uibModalInstance, itemId, $ngConfirm, showToast, hideLoading) {
    $scope.model = {};
    $scope.ListCity = [];
    $scope.ListCityCode = [];
    angular.element(document).ready(function () {
        showToast();
        GetDanhMuc();

        $.ajax({
            type: 'post',
            url: '/TestGroup/GetItemByID',
            data: { maNhom: itemId },
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

    function GetDanhMuc() {
        $scope.ListCity = [];
        $.ajax({
            type: 'post',
            url: '/TestGroup/GetDanhMuc',
            cache: false,
            async: false,
            data: {},
            success: function (data) {

                if (data.Citys != null && data.Citys.length > 0) {
                    $scope.ListCity = data.Citys;
                }

                $scope.$apply();
                hideLoading();
            }
        });
    }

    $scope.submit = function () {
        $("#formSubmit").validate({
            errorElement: 'span',
            errorClass: 'error invalid-feedback',
            errorPlacement: function (error, element) {
                if (element.parent('.input-group').length) {
                    error.insertAfter(element.parent());
                } else if (element.hasClass('select2-hidden-accessible')) {
                    error.insertAfter(element.next('.select2-container'));
                } else {
                    error.insertAfter(element);
                }
            },
            highlight: function (element) {
                $(element).addClass('is-invalid');
            },
            unhighlight: function (element) {
                $(element).removeClass('is-invalid');
            },
            rules: {
                manhom_tbh: {
                    required: true,
                    maxlength: 50
                },
                tennhom_tbh: {
                    required: true,
                    maxlength: 250
                },
                city_code: {
                    required: true
                }
            },
            messages: {
                manhom_tbh: {
                    required: "Vui lòng nhập mã nhóm",
                    maxlength: "Mã nhóm không được vượt quá 50 ký tự"
                },
                tennhom_tbh: {
                    required: "Vui lòng nhập tên nhóm",
                    maxlength: "Tên nhóm không được vượt quá 250 ký tự"
                },
                city_code: {
                    required: "Vui lòng chọn tỉnh thành",
                }
            }
        });
        if ($("#formSubmit").valid()) {

            $.ajax({
                type: 'post',
                url: '/TestGroup/Edit',
                data: { model: $scope.model },
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