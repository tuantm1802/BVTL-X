app.controller("DonViController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $rootScope) {
    $scope.modelAdd = {};
    $scope.model = {
        PHIEN_HIEU: '',
        TEN_DON_VI: '',
        KHU_VUC_ID: '',
        NHOM_DON_VI_ID: '',
        IS_QUAN_LY_KHO: '',
        CONTACT_NAME: '',
        CONTACT_ID: '',
        CONTACT_TEL: '',
        CONTACT_ADDRESS: '',
        DON_VI_CHA_ID: '',
        DV_KHONG_DUNG: '',
        PHONG_QUAN_LY: '',
        PHONG_QUAN_LY1: '',
        ID: 0,
        DV_KHONG_DUNG:'',
        MNG_UNIT_ID: '',
        HIEU_LUC: '',
        KHO_DEFAULT1_ID: '',
        KHO_DEFAULT2_ID: '',
        CONG_TY_DEFAULT1_ID: '',
        CONG_TY_DEFAULT2_ID: '',
        CH_ID: ''
    };
    $scope.event = '';
    $scope.activeMenu = 1;
    $scope.UnitId = 0;
    angular.element(document).ready(function () {
        $scope.showhideForm = true;
        $scope.showhideHuy = true;
        $scope.showhideLuu = true;
        GetAllKHuVuc();
        GetAllNhomDV();
        GetAllKho();
        GetAllCongTy();
        GetAllDonVi(); 
        $scope.GetDonVi();
        GetBottomAction();
    });

    $scope.RoleBtnCreate = false;
    $scope.RoleBtnSave = false;
    $scope.RoleBtnSearch = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnCancel = false;
    $scope.DisabledHieuLuc_SuDung = true;
    function GetBottomAction() {
        $.ajax({
            type: 'post',
            url: '/DonVi/GetBottomAction',
            data: {},
            success: function (response) {
                if (response.Buttoms != null) {
                    angular.forEach(response.Buttoms, function (item) {
                        if (item == 'btnCreate') {
                            $scope.RoleBtnCreate = true;
                        }
                        if (item == 'btnSave') {
                            $scope.RoleBtnSave = true;
                        }
                        if (item == 'btnSearch') {
                            $scope.RoleBtnSearch = true;
                        }
                        if (item == 'btnDelete') {
                            $scope.RoleBtnDelete = true;
                        }
                        if (item == 'btnCancel') {
                            $scope.RoleBtnCancel = true;
                        }
                    });
                }

                if ($scope.RoleBtnCreate || $scope.RoleBtnSave || $scope.RoleBtnDelete)
                    $scope.DisabledHieuLuc_SuDung = false;

                //$scope.ListYear = response.ListYear;
                //$scope.NamBanHanh = response.ListYear[0].Value;
                $scope.$apply();
            }
        });
    }

    $scope.listPhongBan = [
        { 'MA': 'P1', 'TEN': 'Phòng 1' },
        { 'MA': 'P3', 'TEN': 'Phòng 3' },
        { 'MA': 'P4', 'TEN': 'Phòng 4' },
        { 'MA': 'P5', 'TEN': 'Phòng 5' },
        { 'MA': 'P6', 'TEN': 'Phòng 6' },
        { 'MA': 'P7', 'TEN': 'Phòng 7' },
        { 'MA': 'P8', 'TEN': 'Phòng 8' },
        { 'MA': 'P9', 'TEN': 'Phòng 9' },
        { 'MA': 'P10', 'TEN': 'Phòng 10' }
    ]
    var setting = {
        check: {
            enable: false
        },
        data: {
            simpleData: {
                enable: true,
                idKey: "id",
                pIdKey: "pId",
                rootPId: ""
            }
        },
        callback: {
            onClick: onClickTCCBCS
        }
    };

    $scope.treeNodecl = {};
    $scope.donvi_ID = 0;
    function onClickTCCBCS(event, treeId, treeNode, clickFlag) {

       
        if (event != 'THEM' && !treeNode.id.includes('_')) {
            $scope.event = clickFlag;
            $scope.showhideForm = false;
            $scope.treeNodecl = treeNode;
            $scope.donvi_ID = treeNode.id;
            $.ajax({
                type: 'post',
                url: '/DonVi/getEdit',
                data: { Id: treeNode.id },
                success: function (data) {
                    if (data.ObjectData == null || data.ObjectData.length == 0) {
                        toastr.error("Không tìm thấy đơn vị.");
                    } else {
                        console.log(data.ObjectData);
                        //$scope.model.SO_THU_TU_VUNG = data.ObjectData[0].SO_THU_TU_VUNG;
                        $scope.model.PHIEN_HIEU = data.ObjectData[0].PHIEN_HIEU;
                        $scope.model.TEN_DON_VI = data.ObjectData[0].TEN_DON_VI;
                        $scope.model.KHU_VUC_ID = data.ObjectData[0].KHU_VUC_ID;
                        $scope.model.NHOM_DON_VI_ID = data.ObjectData[0].NHOM_DON_VI_ID;
                        //$scope.model.IS_QUAN_LY_KHO = data.ObjectData[0].IS_QUAN_LY_KHO;

                        $scope.model.CONTACT_NAME = data.ObjectData[0].CONTACT_NAME;
                        $scope.model.CONTACT_ID = data.ObjectData[0].CONTACT_ID;
                        $scope.model.CONTACT_TEL = data.ObjectData[0].CONTACT_TEL;
                        $scope.model.CONTACT_ADDRESS = data.ObjectData[0].CONTACT_ADDRESS;
                        $scope.model.DON_VI_CHA_ID = data.ObjectData[0].DON_VI_CHA_ID;
                        $scope.model.MO_TA = data.ObjectData[0].MO_TA;

                        $scope.model.DV_KHONG_DUNG = data.ObjectData[0].DV_KHONG_DUNG;
                        $scope.model.MNG_UNIT_ID = data.ObjectData[0].MNG_UNIT_ID;
                        $scope.model.HIEU_LUC = data.ObjectData[0].HIEU_LUC;

                        //$scope.model.PHONG_QUAN_LY = data.ObjectData[0].PHONG_QUAN_LY;

                        //$scope.model.PHONG_QUAN_LY1 = data.ObjectData[0].PHONG_QUAN_LY != null ? data.ObjectData[0].PHONG_QUAN_LY.split(',') : null;

                        $scope.model.ID = data.ObjectData[0].ID;

                       
                        $scope.model.KHO_DEFAULT1_ID = data.ObjectData[0].KHO_DEFAULT1_ID;
                        $scope.model.KHO_DEFAULT2_ID = data.ObjectData[0].KHO_DEFAULT2_ID;
                        $scope.model.CONG_TY_DEFAULT1_ID = data.ObjectData[0].CONG_TY_DEFAULT1_ID;
                        $scope.model.CONG_TY_DEFAULT2_ID = data.ObjectData[0].CONG_TY_DEFAULT2_ID;
                        $scope.model.CH_ID = data.ObjectData[0].CH_ID;
                        
                        $scope.$apply();
                        //GetAllDonVi();
                    }
                }
            });
        } else {

           
            $scope.model.PHIEN_HIEU = '';
            $scope.model.TEN_DON_VI = '';
            $scope.model.KHU_VUC_ID = null;
            $scope.model.NHOM_DON_VI_ID = null;
            $scope.model.IS_QUAN_LY_KHO = null;


            $scope.model.CONTACT_NAME = '';
            $scope.model.CONTACT_ID = '';
            $scope.model.CONTACT_TEL = '';
            $scope.model.CONTACT_ADDRESS = '';
            $scope.model.DON_VI_CHA_ID = null;
            $scope.model.MO_TA = '';
            //$scope.model.PHONG_QUAN_LY = '';
            //$scope.model.PHONG_QUAN_LY1 = null;

            $scope.model.ID = 0;

            $scope.model.MNG_UNIT_ID = $scope.UnitId;
            $scope.model.HIEU_LUC = 'Y';
            $scope.model.DV_KHONG_DUNG = 'Y';
            

            $scope.model.KHO_DEFAULT1_ID = null;
            $scope.model.KHO_DEFAULT2_ID = null;
            $scope.model.CONG_TY_DEFAULT1_ID = null;
            $scope.model.CONG_TY_DEFAULT2_ID = null;
            $scope.model.CH_ID = 0; 

            $scope.treeNodecl = {};
          
            //$scope.$apply();
        }

    }

    $scope.changePB = function (item) {
        //console.log(item);
    }
    //$scope.chengaKV = function () {
    //    GetAllDonVi();
    //}

    //$scope.chengaNhom = function () {
    //    GetAllDonVi();
    //}

    function GetAllKHuVuc() {
        $.ajax({
            type: 'post',
            url: '/DonVi/GetAllKHuVuc',
            data: {},
            success: function (data) {
                $scope.ListKhuVuc = data.ListKhuVuc;
                $scope.$apply();
            },
            error: function (xhr, status, error) {
                toastr.error("Lỗi lấy danh sách");
            }
        });
    }
    function GetAllNhomDV() {
        $.ajax({
            type: 'post',
            url: '/DonVi/GetAllNhomDonVi',
            data: {},
            success: function (data) {
                $scope.ListNhomDonVi = data.ListNhomDonVi;
                $scope.$apply();
            },
            error: function (xhr, status, error) {
                toastr.error("Lỗi lấy danh sách");
            }
        });
    }
    function GetAllKho() {
        $.ajax({
            type: 'post',
            url: '/DonVi/GetAllKho',
            data: {},
            success: function (data) {
                $scope.ListKho = data.ListKho;
                $scope.$apply();
            },
            error: function (xhr, status, error) {
                toastr.error("Lỗi lấy danh sách");
            }
        });
    }

    function GetAllCongTy() {
        $.ajax({
            type: 'post',
            url: '/DonVi/GetAllCongTy',
            data: {},
            success: function (data) {
                $scope.ListCongTy = data.ListCongTy;
                $scope.UnitId = data.user.UnitId;
                $scope.$apply();
            },
            error: function (xhr, status, error) {
                toastr.error("Lỗi lấy danh sách");
            }
        });
    }
    function GetAllDonVi() {
        $.ajax({
            type: 'post',
            url: '/DonVi/GetAllDonVi',
            data: { id_nhom: $scope.model.NHOM_DON_VI_ID, id_kv: $scope.model.KHU_VUC_ID, id: $scope.model.ID},
            success: function (data) {
                $scope.ListDonVi = data.ListDonVi; 
                $scope.$apply();
            },
            error: function (xhr, status, error) {
                toastr.error("Lỗi lấy danh sách");
            }
        });
    }
    $scope.addDonVi = function () {
        $scope.event = 'THEM';
       
        $scope.model.DON_VI_CHA_ID = $scope.model.ID;
        $scope.donvi_ID = 0;
        $scope.showhideForm = false;
        $scope.model.PHIEN_HIEU = '';
        $scope.model.TEN_DON_VI = '';
        $scope.model.KHU_VUC_ID = $scope.model.KHU_VUC_ID = !undefined ? $scope.model.KHU_VUC_ID : null;
        $scope.model.NHOM_DON_VI_ID = $scope.model.NHOM_DON_VI_ID = !undefined ? $scope.model.NHOM_DON_VI_ID : null;
        $scope.model.IS_QUAN_LY_KHO = null;

        
        $scope.model.CONTACT_NAME = '';
        $scope.model.CONTACT_ID = '';
        $scope.model.CONTACT_TEL = '';
        $scope.model.CONTACT_ADDRESS = '';
       
        $scope.model.MO_TA = '';
        //$scope.model.PHONG_QUAN_LY = '';

        //$scope.model.PHONG_QUAN_LY1 = null;
        $scope.model.ID = 0;

        $scope.model.MNG_UNIT_ID = $scope.UnitId;
        $scope.model.HIEU_LUC = 'Y';
        $scope.model.DV_KHONG_DUNG = 'Y';

        $scope.model.KHO_DEFAULT1_ID = null;
        $scope.model.KHO_DEFAULT2_ID = null;
        $scope.model.CONG_TY_DEFAULT1_ID = null;
        $scope.model.CONG_TY_DEFAULT2_ID = null;
        $scope.model.CH_ID = 0;
        //GetAllDonVi();
    }

    $scope.cancel = function () {
        onClickTCCBCS($scope.event, '', $scope.treeNodecl, '');
        
    }

    $scope.nhomDV = function (type) {
        
        if ($scope.activeMenu != type) {
            $scope.event = 'THEM';
            $scope.model.DON_VI_CHA_ID = null;
            $scope.donvi_ID = 0;
            $scope.model.PHIEN_HIEU = '';
            $scope.model.TEN_DON_VI = '';
            $scope.model.KHU_VUC_ID = null;
            $scope.model.NHOM_DON_VI_ID = null;
            $scope.model.IS_QUAN_LY_KHO = null;

            $scope.model.CONTACT_NAME = '';
            $scope.model.CONTACT_ID = '';
            $scope.model.CONTACT_TEL = '';
            $scope.model.CONTACT_ADDRESS = '';

            $scope.model.MO_TA = '';
           
            $scope.model.ID = 0;

            $scope.model.MNG_UNIT_ID = $scope.UnitId;
            $scope.model.HIEU_LUC = 'Y';
          
            $scope.model.KHO_DEFAULT1_ID = null;
            $scope.model.KHO_DEFAULT2_ID = null;
            $scope.model.CONG_TY_DEFAULT1_ID = null;
            $scope.model.CONG_TY_DEFAULT2_ID = null;
            $scope.model.CH_ID = 0;
        }
        $scope.activeMenu = type;
        $scope.GetDonVi();
    }

    //$scope.khuVuc = function () {
    //    $scope.activeMenu = 2;
    //    $scope.GetDonVi();
    //}

    $scope.ChangeSearch = function () {
        $scope.GetDonVi();
    }
    $scope.GetDonVi = function () {
        //showToast();
        //$scope.ListPageMenu = [];
        //$scope.model = {};
        $.ajax({
            type: 'post',
            url: '/DonVi/GetDonVi',
            data: { key: $scope.searchDonVi, tree: $scope.activeMenu},
            success: function (data) {
                $scope.ListPageMenu = data.TreeDatas;
                    setting = {
                        check: {
                            enable: false
                        },
                        data: {
                            simpleData: {
                                enable: true,
                                idKey: "id",
                                pIdKey: "pId",
                                rootPId: ""
                            }
                        },
                        callback: {
                            onClick: onClickTCCBCS
                        }
                    };

                $.fn.zTree.init($("#treeDonVi"), setting, $scope.ListPageMenu);
                var zTree = $.fn.zTree.getZTreeObj("treeDonVi");
                var type = { "Y": "s", "N": "ps" };
                zTree.setting.check.chkboxType = type;
                $scope.$apply();
                hideLoading();
            },
            error: function (xhr, status, error) {
                toastr.error("Lỗi lấy danh sách lực lượng");
            }
        });
    }

    //$scope.ViewDetail = function (ID, rowIndex) {
    //    $scope.selectedRow = rowIndex;
    //    $.fn.zTree.getZTreeObj("treeDonVi").getSelectedNodes();
    //};

   
    $scope.submit = function () {
            $scope.model.PHONG_QUAN_LY = '';
     
        if ($scope.model.PHIEN_HIEU == null || $scope.model.PHIEN_HIEU == "") {
            toastr.error("Phải nhập Mã đơn vị và Mã đơn vị phải là tiếng việt không dấu");
            return;
        }

        if ($scope.model.TEN_DON_VI == null || $scope.model.TEN_DON_VI == "") {
            toastr.error("Bạn chưa nhập Tên đơn vị!");
            return;
        }
       
        if ($scope.model.NHOM_DON_VI_ID == null || $scope.model.NHOM_DON_VI_ID == "") {
            toastr.error("Bạn chưa nhập chọn Nhóm đơn vị!");
            return;
        }

        if ($scope.model.ID == $scope.model.DON_VI_CHA_ID && $scope.model.ID != 0 && $scope.model.ID != '' && $scope.model.ID != null && $scope.model.ID != undefined) {
            toastr.error("Đơn vị cha không được là chính nó!");
            return;
        }

        if ($scope.formDonVi.CONTACT_TEL.$error.pattern == true) {
            toastr.error("Số điện thoại chỉ được nhập số");
            return;
        }


        if ($scope.formDonVi.CONTACT_ID.$error.pattern == true) {
            toastr.error("Số cmt chỉ được nhập số và ký tự '-'");
            return;
        }

        if ($scope.model.KHO_DEFAULT1_ID == $scope.model.KHO_DEFAULT2_ID && $scope.model.KHO_DEFAULT1_ID != '' && $scope.model.KHO_DEFAULT1_ID != null) {
            toastr.error("Kho ưu tiên 1 và 2 không thể trùng nhau!");
            return;
        }
        if ($scope.model.CONG_TY_DEFAULT1_ID == $scope.model.CONG_TY_DEFAULT2_ID && $scope.model.CONG_TY_DEFAULT1_ID != '' && $scope.model.CONG_TY_DEFAULT1_ID != null) {
            toastr.error("Công ty ưu tiên không thể trùng nhau!");
            return;
        }
       
        $.ajax({
            type: 'post',
            url: '/DonVi/Add',
            data: { donVi: $scope.model},
            success: function (data) {
                if (data.Error) {
                    toastr.error(data.Title);
                } else {
                    //toastr.success(data.Title);
                    if ($scope.model.ID == 0) {
                        $scope.GetDonVi();
                        GetAllDonVi();
                        toastr.success('thêm mới thành công.');
                    } else {
                        GetAllDonVi();
                        toastr.success('Chỉnh sửa thành công.');
                    }

                    //$scope.model = {};
                    hideLoading();
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    };
    $scope.delete = function () {
        var obj = $.fn.zTree.getZTreeObj("treeDonVi").getSelectedNodes();
        if (obj[0].id == null || obj[0].id == "" || obj[0].id.includes('_')) {
            toastr.error("Bạn chưa chọn Đơn vị để Xóa!");
            return;
        }
        $ngConfirm({
            title: 'Thông báo',
            content: 'Bạn có chắc chắn muốn xóa dữ liệu đã chọn không?',
            scope: $scope,
            buttons: {
                delete: {
                    text: 'Xóa',
                    btnClass: 'btn-red',
                    action: function (scope, button) {
                        $.ajax({
                            type: 'post',
                            url: '/DonVi/Delete',
                            data: { Id: obj[0].id },
                            success: function (data) {
                                if (data.Error) {
                                    toastr.error(data.Title);
                                } else {
                                    toastr.success(data.Title);
                                    $scope.GetDonVi();
                                    GetAllDonVi();
                                    hideLoading();
                                }
                            }
                        });
                    }
                },
                close: {
                    text: 'Hủy',
                    btnClass: 'btn-green',
                    action: function (scope, button) {

                    }
                }
            }
        });
      
    };
   
});