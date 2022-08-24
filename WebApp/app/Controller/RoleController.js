app.controller("RoleController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "Name DESC";

    $scope.RoleIdSeleted = "";

    $scope.ListRole = [];
    var dataTableRole = null;
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
            url: '/Role/GetBottomAction',
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
                console.log('RoleBtnUpdate');
                console.log($scope.RoleBtnUpdate);
                $scope.$apply();
            }
        });
    }

    $scope.pageChanged = function () {
        $scope.LoadPage(0);
    };

    $scope.ViewDetail = function (ID) {
        $scope.selectedRow = $scope.ListRole.findIndex(record => record.ID === ID);;
    };

    $scope.LoadPage = function (genTable) {

        showToast();
        //$.ajax({
        //    type: 'post',
        //    url: '/Role/GetAllByPage',
        //    data: $scope.modelSearch,
        //    success: function (data) {
        //        //$scope.modelSearch.totalItems = data.totalItems;
        //        $scope.ListRole = data.data;
        //        //$scope.modelSearch.pageSize = data.pageSize;

        //        $scope.$apply();
        //        if (genTable == 1) {
        //            $('#dataTableRole').DataTable({ "filter": false });
        //        }
        //         hideLoading();
        //    }
        //});
        $scope.ListRole = [];
        if (genTable == 1) {
            dataTableRole = $('#dataTableRole').DataTable({
                lengthMenu: [10, 20, 30, 50, 60, 100],
                serverSide: true,
                ordering: false,
                searching: false,
                ajax: function (data, callback, settings) {
                    var dataRole = [];
                    var totalItems = 0;
                    var page = ((data.start / data.length) + 1);

                    $scope.modelSearch.currentPage = page;
                    $scope.modelSearch.pageSize = data.length;

                    $.ajax({
                        type: 'post',
                        url: '/Role/GetAllByPage',
                        cache: false,
                        async: false,
                        data: $scope.modelSearch,
                        success: function (respone) {
                            totalItems = respone.totalItems;
                            $scope.ListRole = respone.data;
                            //dataUser = respone.data;
                            if (respone.data != null && respone.data.length > 0) {
                                for (var i = 0; i < respone.data.length; i++) {
                                    var tmp = {
                                        STT: i + 1,
                                        ID: respone.data[i].ID,
                                        Name: respone.data[i].Name,
                                        Descripttion: respone.data[i].Descripttion,
                                        IsActive: respone.data[i].IsActive == true ? 'Sử dụng' : 'Không sử dụng'
                                    }
                                    dataRole.push(tmp);
                                }
                            }
                        }
                    });

                    setTimeout(function () {
                        callback({
                            draw: data.draw,
                            data: dataRole,
                            recordsTotal: totalItems,
                            recordsFiltered: totalItems
                        });
                    }, 50);
                },
                rowId: 'ID',
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
                    { "data": "ID" },
                    { "data": "Name" },
                    { "data": "Descripttion" },
                    { "data": "IsActive" }
                    //,{
                    //    "data": "ID", "render": function (data) {

                    //        return '<button type="button" class="btn btn-sm btn-primary" title="Cập nhật" style="margin-right: 5px;" onclick="edit(' + data + ')"> <i class="fas fa-pencil-alt" ></i></button>'
                    //            + '<button type="button" class="btn btn-sm btn-danger"  title="Xóa" style="margin-right: 5px;" onclick="deleteRole(' + data + ')"><i class="fas fa-times"></i> </button>'
                    //            //+ '<button type="button" class="btn btn-sm btn-primary"  title="Xem" ng-click="ViewDetail(' + data + ')"> <i class="fas fa-eye"></i></button>'

                    //    }
                    //}
                ],

                dom: "<'row'<'col-sm-12'f>>" +
                    "<'row'<'col-sm-12'tr>>" +
                    "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>",

                // dom: '<"top"f>rt<"bottom"ilp><"clear">',
                // scrollY: 200,
                scroller: {
                    loadingIndicator: true
                },
            });

        } else {
            dataTableRole.ajax.reload();
        }
        hideLoading();
    };

    $('#dataTableRole').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });



    $scope.Refesh = function () {
        $scope.LoadPage(0);
    };

    $scope.add = function () {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/Role/_Add',
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
        var seletedRow = dataTableRole.rows({ selected: true });
        var count = seletedRow.count();
        if (count > 0) {
            $scope.RoleIdSeleted = seletedRow.data()[0].ID;
        } else {
            $scope.RoleIdSeleted = "";
        }

        if ($scope.RoleIdSeleted != null && $scope.RoleIdSeleted != '' && $scope.RoleIdSeleted != undefined) {
            var modalInstance = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: '/Role/_Edit',
                controller: 'edit',
                size: 'xl',
                backdrop: 'static',
                resolve: {
                    itemId: function () {
                        return $scope.RoleIdSeleted;
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
        var seletedRow = dataTableRole.rows({ selected: true });
        var count = seletedRow.count();
        if (count > 0) {
            $scope.RoleIdSeleted = seletedRow.data()[0].ID;
        } else {
            $scope.RoleIdSeleted = "";
        }

        if ($scope.RoleIdSeleted != null && $scope.RoleIdSeleted != '' && $scope.RoleIdSeleted != undefined) {
            var name = $scope.ListRole.filter(function (item) {
                return item.ID === $scope.RoleIdSeleted;
            })[0].Name;

            $ngConfirm({
                title: 'Thông báo',
                content: 'Bạn có chắc chắn muốn xóa nhóm quyền ' + name + ' không?',
                scope: $scope,
                buttons: {
                    delete: {
                        text: 'Xóa',
                        btnClass: 'btn-primary',
                        action: function (scope, button) {
                            $.ajax({
                                type: 'post',
                                url: '/Role/Delete',
                                data: { Id: $scope.RoleIdSeleted },
                                success: function (data) {
                                    if (data.Error) {
                                        toastr.error(data.Title);
                                    } else {
                                        toastr.success(data.Title);
                                        //$scope.cancel();
                                        $scope.LoadPage(0);
                                    }
                                }
                            });

                        }
                    },
                    close: {
                        text: 'Hủy',
                        btnClass: 'btn-secondary',
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
    $scope.ListPageMenu = [];
    angular.element(document).ready(function () {
        showToast();
        GetDanhMuc();
    });

    var setting = {
        check: {
            enable: true
        },
        data: {
            simpleData: {
                enable: true,
                idKey: "id",
                pIdKey: "pId",
                rootPId: ""
            }
        }
    };


    function GetDanhMuc() {
        $scope.ListPageMenu = [];
        $.ajax({
            type: 'post',
            url: '/Role/GetDanhMuc',
            data: {},
            success: function (data) {
                hideLoading();

                $scope.ListPageMenu = data.TreeDatas;
                $.fn.zTree.init($("#treeRole"), setting, $scope.ListPageMenu);
                var zTree = $.fn.zTree.getZTreeObj("treeRole");
                var type = { "Y": "ps", "N": "ps" };
                zTree.setting.check.chkboxType = type;

            }
        });
    }

    $scope.model = {};
    $scope.submit = function () {
        $scope.model.Status = $scope.model.StatusTemp === '1' ? true : false;
        $("#formSubmit").validate({
            rules: {
                ID: {
                    required: true,
                    maxlength: 50
                },
                Name: {
                    required: true,
                    maxlength: 50
                }

            },
            messages: {
                ID: {
                    required: "Vui lòng nhập mã",
                    maxlength: "Mã không được vượt quá 50 ký tự"
                },
                Name: {
                    required: "Vui lòng nhập tên quyền",
                    maxlength: "Tên quyền không được vượt quá 50 ký tự"
                }
            }
        });
        if ($("#formSubmit").valid()) {

            var treeObj = $.fn.zTree.getZTreeObj("treeRole");
            var nodes = treeObj.getCheckedNodes();

            angular.forEach($scope.ListPageMenu, function (pageMenu) {

                var dataSearch = nodes.filter(function (item) {
                    return item.id === pageMenu.id;
                });

                if (dataSearch !== null && dataSearch.length > 0) {
                    pageMenu.checked = true;
                }

            })


            $.ajax({
                type: 'post',
                url: '/Role/Add',
                data: { role: $scope.model, pageMenus: $scope.ListPageMenu },
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

    $scope.SelectFile = function (e) {
        $scope.model.Avartar = e.target.files[0];
        document.getElementById("pathPhoto").src = e.target.files[0];
    };
});

app.controller('edit', function ($scope, $uibModalInstance, itemId, $ngConfirm, showToast, hideLoading) {
    $scope.ListPageMenu = [];

    $scope.ListRole = [];
    var setting = {
        check: {
            enable: true
        },
        data: {
            simpleData: {
                enable: true,
                idKey: "id",
                pIdKey: "pId",
                rootPId: ""
            }
        }
    };


    $scope.model = {};
    angular.element(document).ready(function () {

        $.ajax({
            type: 'post',
            url: '/Role/GetItemByID',
            data: { Id: itemId },
            success: function (data) {
                if (data.Error) {
                    toastr.error(data.Title);
                } else {
                    $scope.model = data.data;

                    $scope.ListPageMenu = data.TreeDatas;
                    $.fn.zTree.init($("#treeRole"), setting, $scope.ListPageMenu);
                    var zTree = $.fn.zTree.getZTreeObj("treeRole");
                    var type = { "Y": "ps", "N": "ps" };
                    zTree.setting.check.chkboxType = type;
                    $scope.$apply();
                }
            }
        });
    });

    $scope.submit = function () {
        $scope.model.Status = $scope.model.StatusTemp === '1' ? true : false;
        $("#formSubmit").validate({
            rules: {
                ID: {
                    required: true,
                    maxlength: 50
                },
                Name: {
                    required: true,
                    maxlength: 50
                }

            },
            messages: {
                ID: {
                    required: "Vui lòng nhập mã",
                    maxlength: "Mã không được vượt quá 50 ký tự"
                },
                Name: {
                    required: "Vui lòng nhập tên quyền",
                    maxlength: "Tên quyền không được vượt quá 50 ký tự"
                }
            }
        });
        if ($("#formSubmit").valid()) {

            var treeObj = $.fn.zTree.getZTreeObj("treeRole");
            var nodes = treeObj.getCheckedNodes();

            angular.forEach($scope.ListPageMenu, function (pageMenu) {

                var dataSearch = nodes.filter(function (item) {
                    return item.id === pageMenu.id;
                });

                if (dataSearch !== null && dataSearch.length > 0) {
                    pageMenu.checked = true;
                }

            })

            $.ajax({
                type: 'post',
                url: '/Role/Edit',
                data: { role: $scope.model, pageMenus: $scope.ListPageMenu },
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