app.controller("TestGroupController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "Name DESC";
    $scope.ListData = [];

    var dataTableNhomTTDL = null;
    $scope.NhomTTDLIdSeleted = 0;
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
                                        Code: respone.data[i].Code,
                                        Name: respone.data[i].Name,
                                        IsActive: respone.data[i].IsActive == true ? 'Sử dụng' : 'Không sử dụng',
                                        Id: respone.data[i].Id
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
                rowId: 'Id',
                select: {
                    info: false
                },
                columns: [
                    { "data": "STT", },
                    { "data": "Code" },
                    { "data": "Name" },
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
    $scope.edit = function () {
        var seletedRow = dataTableNhomTTDL.rows({ selected: true });
        var count = seletedRow.count();
        if (count > 0) {
            $scope.NhomTTDLIdSeleted = seletedRow.data()[0].Id;
        } else {
            $scope.NhomTTDLIdSeleted = 0;
        }

        if ($scope.NhomTTDLIdSeleted > 0 && $scope.NhomTTDLIdSeleted != undefined) {
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
                $scope.LoadPage(0);
            });
        } else {
            toastr.error("Bạn chưa chọn bản ghi nào.");
        }
    };
    $scope.delete = function () {
        var seletedRow = dataTableNhomTTDL.rows({ selected: true });
        var count = seletedRow.count();
        if (count > 0) {
            $scope.NhomTTDLIdSeleted = seletedRow.data()[0].Id;
        } else {
            $scope.NhomTTDLIdSeleted = 0;
        }

        if ($scope.NhomTTDLIdSeleted > 0 && $scope.NhomTTDLIdSeleted != undefined) {

            var name = $scope.ListData.filter(function (item) {
                return item.Id === $scope.NhomTTDLIdSeleted;
            })[0].Name;

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
                                data: { Id: $scope.NhomTTDLIdSeleted },
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

app.controller('add', function ($scope, $uibModalInstance, $ngConfirm, showToast, hideLoading) {
    angular.element(document).ready(function () {

    });

    $scope.model = {};
    $scope.submit = function () {
        $("#formSubmit").validate({
            rules: {
                Code: {
                    required: true,
                    maxlength: 50
                },
                Name: {
                    required: true,
                    maxlength: 250
                }
            },
            messages: {
                Code: {
                    required: "Vui lòng nhập mã nhóm",
                    maxlength: "Mã nhóm không được vượt quá 50 ký tự"
                },
                Name: {
                    required: "Vui lòng nhập tên nhóm",
                    maxlength: "Tên nhóm không được vượt quá 250 ký tự"
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
    angular.element(document).ready(function () {
        $.ajax({
            type: 'post',
            url: '/TestGroup/GetItemByID',
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

    $scope.submit = function () {
        $("#formSubmit").validate({
            rules: {
                Code: {
                    required: true,
                    maxlength: 50
                },
                Name: {
                    required: true,
                    maxlength: 250
                }
            },
            messages: {
                Code: {
                    required: "Vui lòng nhập mã nhóm",
                    maxlength: "Mã nhóm không được vượt quá 50 ký tự"
                },
                Name: {
                    required: "Vui lòng nhập tên nhóm",
                    maxlength: "Tên nhóm không được vượt quá 250 ký tự"
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