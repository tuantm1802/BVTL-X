app.controller("KeHoach_XuatKhoController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading, $location) {
    //#region giá trị mặc định
    $scope.IsShowRight = false;
    $scope.IsSavePhamNhan = false;
    $scope.SetSavePhamNhan = function (data) {
        if (data === '7')
            $scope.IsSavePhamNhan = true;
        else
            $scope.IsSavePhamNhan = false;
    }
    $scope.ListNam = [];
    var currentYear = new Date().getFullYear();
    for (var i = currentYear; i > currentYear - 7; i--) {
        $scope.ListNam.push({ Id: i, Name: i });
    }
    $scope.NAM = currentYear.toString();
    $scope.KY = '1';

    angular.element(document).ready(function () {
        $scope.CurrentDate = new Date();

        GetBottomAction();
    });

    //#endregion
    //#region mở popup
    $scope.AddFileChotQuanSoDauKy = function (donViId, ky, nam) {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/KeHoach_XuatKho/_AddFileChotQuanSoDauKy',
            controller: 'addFileChotQuanSoDauKy',
            size: 'xl',
            backdrop: 'static',
            resolve: {
                data: function () {
                    return {
                        donViId: donViId,
                        ky: ky,
                        nam: nam
                    };
                }
            }
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            //$scope.LoadPage();
        });
    };
    $scope.AddFileChotQuanSoTrongKy = function () {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/KeHoach_XuatKho/_AddFileChotQuanSoTrongKy',
            controller: 'addFileChotQuanSoTrongKy',
            size: 'xl',
            backdrop: 'static'
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            //$scope.LoadPage();
        });
    };
    $scope.DanhSachLoi = function () {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/KeHoach_XuatKho/_DanhSachFileLoiGanNhat',
            controller: 'danhSachFileLoiGanNhat',
            size: 'xs',
            backdrop: 'static'
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            //$scope.LoadPage();
        });
    };
    //#endregion
    //#region tạo treeview
    function CreateItem(data, chilldTreeLevel, parentID) {
        angular.forEach(data.filter(x => x.DON_VI_CHA_ID === parentID), function (val, key) {
            chilldTreeLevel[val.ID] = {};
            chilldTreeLevel[val.ID]['text'] = val.TEN_DON_VI;
            if (data.filter(x => x.DON_VI_CHA_ID === val.ID).length === 0) {
                chilldTreeLevel[val.ID]['type'] = 'item';
                chilldTreeLevel[val.ID]['id'] = val.ID;
            }
            else {
                chilldTreeLevel[val.ID]['type'] = 'folder';
                chilldTreeLevel[val.ID]['additionalParameters'] = {};
                chilldTreeLevel[val.ID]['additionalParameters']['children'] = {};
                var chilldTree = chilldTreeLevel[val.ID]['additionalParameters']['children'];
                CreateItem(data, chilldTree, val.ID);
            }
        });
    }

    $scope.treeData = {};
    $scope.treeData.data = {};
    $scope.ListDonVi;
    $.ajax({
        type: 'get',
        async: false,
        url: '/KeHoach_XuatKho/GetTreeData',
        success: function (res) {
            if (!res.Error) {
                $scope.ListDonVi = res;
            }
        }
    })
    $scope.CreateTreeView = function (data) {
        var res = data;
        var tree_data = {};
        angular.forEach(res.data.ItemTreeKhoi, function (val, key) {
            var childrenLevel1 = res.data.ItemTreeDonViNhom.filter(x => x.LOAI_NHOM_DV === val.ID);
            tree_data[val.ID] = {};
            tree_data[val.ID]['text'] = val.TEN_KHOI;
            if (childrenLevel1.length === 0)
                tree_data[val.ID]['type'] = 'item';
            else {
                tree_data[val.ID]['type'] = 'folder';
                tree_data[val.ID]['additionalParameters'] = {};
                tree_data[val.ID]['additionalParameters']['children'] = {};
            }
            var chilldTreeLevel1 = tree_data[val.ID]['additionalParameters']['children'];
            angular.forEach(childrenLevel1, function (valNhom, keyNhom) {
                var childrenLevel2 = res.data.ItemTreeDonVi.filter(x => x.NHOM_DON_VI_ID === valNhom.ID);

                chilldTreeLevel1[valNhom.ID] = {};
                chilldTreeLevel1[valNhom.ID]['text'] = valNhom.TEN_NHOM_DV;

                if (childrenLevel2.length === 0)
                    chilldTreeLevel1[valNhom.ID]['type'] = 'item';
                else {
                    chilldTreeLevel1[valNhom.ID]['type'] = 'folder';
                    chilldTreeLevel1[valNhom.ID]['additionalParameters'] = {};
                    chilldTreeLevel1[valNhom.ID]['additionalParameters']['children'] = {};
                }
                var chilldTreeLevel2 = chilldTreeLevel1[valNhom.ID]['additionalParameters']['children'];
                angular.forEach(childrenLevel2, function (valDonVi, keyDonVi) {
                    var childrenLevel3 = res.data.ItemTreeDonVi.filter(x => x.DON_VI_CHA_ID === valDonVi.ID);

                    chilldTreeLevel2[valDonVi.ID] = {};
                    chilldTreeLevel2[valDonVi.ID]['text'] = valDonVi.TEN_DON_VI;

                    if (childrenLevel3.length === 0) {
                        chilldTreeLevel2[valDonVi.ID]['id'] = valDonVi.ID;
                        chilldTreeLevel2[valDonVi.ID]['type'] = 'item';
                    }
                    else {
                        chilldTreeLevel2[valDonVi.ID]['type'] = 'folder';
                        chilldTreeLevel2[valDonVi.ID]['additionalParameters'] = {};
                        chilldTreeLevel2[valDonVi.ID]['additionalParameters']['children'] = {};
                        var chilldTreeLevel3 = chilldTreeLevel2[valDonVi.ID]['additionalParameters']['children'];
                        CreateItem(res.data.ItemTreeDonVi, chilldTreeLevel3, valDonVi.ID);
                    }
                });
            });
        });
        $scope.treeData.data = tree_data;
    }
    $scope.CreateTreeView($scope.ListDonVi);
    $('#cat-tree').on('selected.fu.tree', function (e) {
        var dataSelected = $('#cat-tree').tree('selectedItems');
        if (dataSelected !== undefined && dataSelected[0]['id'] !== undefined) {
            showToast();
            $scope.TEN_DON_VI = dataSelected[0]['text'];
            $scope.DON_VI_ID = dataSelected[0]['id'];
            $scope.IsShowRight = true;
            $scope.LoadDataCBCongTac($scope.DON_VI_ID);
            $scope.LoadDataCBChoHuu($scope.DON_VI_ID);
            $scope.LoadDataCBBietPhai($scope.DON_VI_ID);
            $scope.LoadDataCBDiHoc($scope.DON_VI_ID);
            $scope.LoadDataCSNghiaVu($scope.DON_VI_ID);
            $scope.LoadDataCBTuyenMoi($scope.DON_VI_ID);
            $scope.LoadDataPhamNhan($scope.DON_VI_ID)
            $scope.$apply();
            hideLoading();
        }
    });
    //#endregion tạo treeview
    //#region validate dữ liệu từ bảng temp
    $scope.ValidateSoHieu = function (data, key) {
        var lstError = [];
        if (data.SO_HIEU.indexOf('-') === -1 || data.SO_HIEU.length !== 7) {
            lstError.push({ col: 1, row: key, id: data.ID });
        } else {
            if (data.SO_HIEU.split('-')[0].length !== 3 || data.SO_HIEU.split('-')[1].length !== 3) {
                lstError.push({ col: 1, row: key, id: data.ID });
            }
        }
        return lstError;
    }
    //#endregion
    //#region Tìm kiếm đơn vị tree
    $scope.SearchDonVi = function () {
        console.log($scope.TenDonVi);
    }
    //#endregion
    //#region Lấy tnxId
    $scope.txnId = 0;
    $scope.txnId = getSearchParams('txnid');
    function getSearchParams(k) {
        var p = {};
        location.search.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (s, k, v) { p[k] = v })
        return k ? p[k] : p;
    }
    //#endregion

    //#region Danh sách Cán bộ công tác
    $scope.JxlCbCongTac = {};
    $scope.JxlCbCongTac.data = [];
    $scope.ID_NHAP_XUAT = 0; //dữ liệu demo
    $scope.ListErrorCT = [];
    $scope.LoadDataCBCongTac = function (donViId) {
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoach_XuatKho/DanhSachTp_CbCongTac',
            data: { donViId: donViId },
            success: function (response) {
                if (response.Error) {
                    toastr.error(response.Title);
                } else {
                    angular.forEach(response.data, function (val, key) {
                        $scope.ItemJexcel = [val.ID, val.SO_HIEU, val.GIOI_TINH, val.CAP_BAC_ID, val.LOAI_HAM_ID, val.CHUC_VU, val.CHUC_VU, val.LUC_LUONG_ID, val.CO_MU, val.CO_GIAY, val.CO_QA, val.DANG_KY_HV]
                        $scope.JxlCbCongTac.data.push($scope.ItemJexcel);
                        $scope.tblCBCongTac.setData($scope.Json2Arrary($scope.JxlCbCongTac.data));
                        $scope.ListErrorCT = $scope.ValidateSoHieu(val, key);
                    });
                }
            }
        });
    }
    $scope.JxlCbCongTac.column = [
        { type: 'text', width: '0', title: '' },
        { type: 'text', width: '100', title: 'A' },
        { type: 'dropdown', width: '100', source: [{ 'id': 'F', 'name': 'Nữ' }, { 'id': 'M', 'name': 'Nam' }] },
        { type: 'dropdown', autocomplete: true, width: '120', title: 'C', url: '/KeHoach_XuatKho/GetDMCapBacJexcel' },
        { type: 'dropdown', autocomplete: true, width: '120', title: 'D', url: '/KeHoach_XuatKho/GetDMLoaiHamJexcel' },
        { type: 'text', width: '120', title: 'E' },
        { type: 'text', width: '120', title: 'F' },
        { type: 'dropdown', autocomplete: true, width: '120', title: 'G', url: '/KeHoach_XuatKho/GetDMLucLuongJexcel' },
        { type: 'text', width: '50', title: 'H' },
        { type: 'text', width: '50', title: 'I' },
        { type: 'text', width: '50', title: 'J' },
        { type: 'text', width: '100', title: 'K' }
    ];
    $scope.JxlCbCongTac.nestedHeaders = [
        [
            { title: '', rowspan: '2' },
            { title: 'Số hiệu CAND ', rowspan: '2' },
            { title: 'Giới tính ', rowspan: '2' },
            { title: 'Cấp bậc - loại hàm (hiện tại) ', colspan: '2' },
            { title: 'Chức vụ - đơn vị công tác - lực lượng (hiện tại)', colspan: '3' },
            { title: 'Cỡ giầy ', rowspan: '2' },
            { title: 'Cỡ mũ  ', rowspan: '2' },
            { title: 'Cỡ QA ', rowspan: '2' },
            { title: 'Không nhận hiện vật ', rowspan: '2' }

        ], [
            { title: 'Cấp bậc ' },
            { title: 'Loại hàm' },
            { title: 'Chức vụ' },
            { title: 'Vị trí công tác' },
            { title: 'Lực lượng' },
        ]
    ];

    $scope.JxlCbCongTac.footers = [['', 'Tổng', '', '', '', '', '', '', '', '', '', '']];
    $scope.JxlCbCongTac.changed = function (instance, cell, x, y, value) {
        var cellName = jexcel.getColumnNameFromId([x, y]);
        var cellA = jexcel.getColumnNameFromId([0, y]);
        $scope.DataUpdate = {};
        var rowData = $scope.tblCBCongTac.getRowData(y);
        $scope.DataUpdate.ID = rowData['0'];
        $scope.DataUpdate.SO_HIEU = rowData['1'];
        $scope.DataUpdate.GIOI_TINH = rowData['2'];
        $scope.DataUpdate.CAP_BAC_ID = rowData['3'];
        $scope.DataUpdate.LOAI_HAM_ID = rowData['4'];
        $scope.DataUpdate.CHUC_VU = rowData['5'];
        $scope.DataUpdate.LUC_LUONG_ID = rowData['7'];
        $scope.DataUpdate.CO_GIAY = rowData['8'];
        $scope.DataUpdate.CO_MU = rowData['9'];
        $scope.DataUpdate.CO_QA = rowData['10'];
        $scope.DataUpdate.DANG_KY_HV = rowData['11'];
        $scope.UpdateCBCongTac($scope.DataUpdate);
    };
    $scope.JxlCbCongTac.updateTable = function (instance, cell, col, row, val, label, cellName) {
        angular.forEach($scope.ListErrorCT, function (v, k) {
            if (v.col === col && v.row === row) {
                cell.className = '';
                cell.style.backgroundColor = '#f46e42';
                cell.style.color = '#ffffff';
            }
        });

    };

    $scope.CBCongTacCallBack = function (data) {
        $scope.tblCBCongTac = data;
    }
    $scope.UpdateCBCongTac = function (data) {
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoach_XuatKho/UpdateCBCongTac',
            data: { tP_CB_CONG_TAC: data },
            success: function (response) {
                if (response.Error) {
                    toastr.error(response.Title);
                } else {
                    angular.forEach(response.data, function (val, key) {
                        $scope.ItemJexcel = [val.ID, val.SO_HIEU, val.GIOI_TINH, val.CAP_BAC_ID, val.LOAI_HAM_ID, val.CHUC_VU, val.CHUC_VU, val.LUC_LUONG_ID, val.CO_GIAY, val.CO_MU, val.CO_QA, val.DANG_KY_HV]
                        $scope.JxlCbChoHuu.data.push($scope.ItemJexcel);
                        $scope.ListErrorCH = $scope.ValidateSoHieu(val, key);
                    });
                }
                //$scope.$apply();
            }
        });
    }
    //#endregion 
    //#region Danh sách cán bộ chờ hưu 
    $scope.JxlCbChoHuu = {};
    $scope.JxlCbChoHuu.data = [];
    $scope.ListErrorCH = [];
    $scope.LoadDataCBChoHuu = function (donViId) {
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoach_XuatKho/DanhSachTp_CbChoHuu',
            data: { donViId: donViId },
            success: function (response) {
                if (response.Error) {
                    toastr.error(response.Title);
                } else {
                    angular.forEach(response.data, function (val, key) {
                        $scope.ItemJexcel = [val.ID, val.SO_HIEU, val.GIOI_TINH, val.CAP_BAC_ID, val.LOAI_HAM_ID, val.CHUC_VU, val.CHUC_VU, val.LUC_LUONG_ID, val.CO_GIAY, val.CO_MU, val.CO_QA, val.DANG_KY_HV]
                        $scope.JxlCbChoHuu.data.push($scope.ItemJexcel);
                        $scope.tblCBChoHuu.setData($scope.JxlCbChoHuu.data);
                        $scope.ListErrorCH = $scope.ValidateSoHieu(val, key);
                    });
                }
            }
        });
    }

    $scope.JxlCbChoHuu.colHeaders = ['', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K'];
    $scope.JxlCbChoHuu.column = [
        { type: 'text', width: '0' },
        { type: 'text', width: '100' },
        { type: 'dropdown', width: '100', source: [{ 'id': 'F', 'name': 'Nữ' }, { 'id': 'M', 'name': 'Nam' }] },
        { type: 'dropdown', autocomplete: true, width: '120', title: 'C', url: '/KeHoach_XuatKho/GetDMCapBacJexcel' },
        { type: 'dropdown', autocomplete: true, width: '120', title: 'D', url: '/KeHoach_XuatKho/GetDMLoaiHamJexcel' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'dropdown', autocomplete: true, width: '120', title: 'H', url: '/KeHoach_XuatKho/GetDMLucLuongJexcel' },
        { type: 'text', width: '50' },
        { type: 'text', width: '50' },
        { type: 'text', width: '50' },
        { type: 'text', width: '120' }
    ];
    $scope.JxlCbChoHuu.nestedHeaders = [
        [
            { title: '', rowspan: '2' },
            { title: 'Số hiệu CAND ', rowspan: '2' },
            { title: 'Giới tính ', rowspan: '2' },
            { title: 'Cấp bậc - loại hàm ', colspan: '2' },
            { title: 'Chức vụ - đơn vị - lực lượng', colspan: '3' },
            { title: 'Cỡ giầy ', rowspan: '2' },
            { title: 'Cỡ mũ  ', rowspan: '2' },
            { title: 'Cỡ QA ', rowspan: '2' },
            { title: 'Nhận hiện vật ', rowspan: '2' }
        ], [
            { title: ' ' },
            { title: ' ' },
            { title: ' ' },
            { title: 'Cấp bậc ' },
            { title: 'Loại hàm' },
            { title: 'Chức vụ' },
            { title: 'Vị trí công tác' },
            { title: 'Lực lượng' },
            { title: ' ' },
            { title: ' ' },
            { title: ' ' },
            { title: ' ' },
        ]
    ];

    $scope.JxlCbChoHuu.footers = [['', 'Tổng', '', '', '', '', '', '', '', '', '', '', '']];

    $scope.JxlCbChoHuu.changed = function (instance, cell, x, y, value) {
        var cellName = jexcel.getColumnNameFromId([5, 12]);
        //var cellVal = jexcel.current.options.getData(true);
        var sumcol1 = SUMCOL(jexcel.current, 12)
        jexcel('setComments', 'A1', 'This is the comments from A1');
    };
    $scope.JxlCbChoHuu.updateTable = function (instance, cell, col, row, val, label, cellName) {
        angular.forEach($scope.ListErrorCH, function (v, k) {
            if (v.col === col && v.row === row) {
                cell.className = '';
                cell.style.backgroundColor = '#f46e42';
                cell.style.color = '#ffffff';
            }
        });

    };
    $scope.CBChoHuuCallBack = function (data) {
        $scope.tblCBChoHuu = data;
    }
    //#endregion
    //#region Danh sách cán bộ biệt phái    
    $scope.JxlBietPhai = {};
    $scope.JxlBietPhai.data = [];
    $scope.ListErrorBP = [];
    $scope.LoadDataCBBietPhai = function (donViId) {
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoach_XuatKho/DanhSachTp_BietPhai',
            data: { donViId: donViId },
            success: function (response) {
                if (response.Error) {
                    toastr.error(response.Title);
                } else {
                    angular.forEach(response.data, function (val, key) {
                        $scope.ItemJexcel = [val.ID, val.SO_HIEU, val.GIOI_TINH, val.CAP_BAC_ID, val.LOAI_HAM_ID, val.CHUC_VU, val.CHUC_VU, val.LUC_LUONG_ID, val.CO_GIAY, val.CO_MU, val.CO_QA, val.DANG_KY_HV]
                        $scope.JxlBietPhai.data.push($scope.ItemJexcel);
                        $scope.tblCBBietPhai.setData($scope.Json2Arrary($scope.JxlBietPhai.data));
                        $scope.ListErrorBP = $scope.ValidateSoHieu(val, key);
                    });
                }
            }
        });
    }

    $scope.JxlBietPhai.colHeaders = ['', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K'];
    $scope.JxlBietPhai.column = [
        { type: 'text', width: '0' },
        { type: 'text', width: '100' },
        { type: 'dropdown', width: '100', source: [{ 'id': 'F', 'name': 'Nữ' }, { 'id': 'M', 'name': 'Nam' }] },
        { type: 'dropdown', autocomplete: true, width: '120', title: 'C', url: '/KeHoach_XuatKho/GetDMCapBacJexcel' },
        { type: 'dropdown', autocomplete: true, width: '120', title: 'D', url: '/KeHoach_XuatKho/GetDMLoaiHamJexcel' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'dropdown', autocomplete: true, width: '120', title: 'H', url: '/KeHoach_XuatKho/GetDMLucLuongJexcel' },
        { type: 'text', width: '50' },
        { type: 'text', width: '50' },
        { type: 'text', width: '50' },
        { type: 'text', width: '120' }
    ];
    $scope.JxlBietPhai.nestedHeaders = [
        [
            { title: '', rowspan: '2' },
            { title: 'Số hiệu CAND ', rowspan: '2' },
            { title: 'Giới tính ', rowspan: '2' },
            { title: 'Cấp bậc - loại hàm ', colspan: '2' },
            { title: 'Chức vụ - đơn vị - lực lượng', colspan: '3' },
            { title: 'Cỡ giầy ', rowspan: '2' },
            { title: 'Cỡ mũ  ', rowspan: '2' },
            { title: 'Cỡ QA ', rowspan: '2' },
            { title: 'Nhận hiện vật ', rowspan: '2' }
        ], [
            { title: ' ' },
            { title: ' ' },
            { title: ' ' },
            { title: 'Cấp bậc ' },
            { title: 'Loại hàm' },
            { title: 'Chức vụ' },
            { title: 'Vị trí công tác' },
            { title: 'Lực lượng' },
            { title: ' ' },
            { title: ' ' },
            { title: ' ' },
            { title: ' ' },
        ]
    ];

    $scope.JxlBietPhai.footers = [['', 'Tổng', '', '', '', '', '', '', '', '', '', '', '']];


    $scope.JxlBietPhai.changed = function (instance, cell, x, y, value) {
        var cellName = jexcel.getColumnNameFromId([5, 12]);
        //var cellVal = jexcel.current.options.getData(true);
        var sumcol1 = SUMCOL(jexcel.current, 12)
    };
    $scope.JxlBietPhai.updateTable = function (instance, cell, col, row, val, label, cellName) {
        angular.forEach($scope.ListErrorBP, function (v, k) {
            if (v.col === col && v.row === row) {
                cell.className = '';
                cell.style.backgroundColor = '#f46e42';
                cell.style.color = '#ffffff';
            }
        });

    };
    $scope.CBBietPhaiCallBack = function (data) {
        $scope.tblCBBietPhai = data;
    }
    //#endregion
    //#region Danh sách cán bộ đi học 
    $scope.JxlCbDiHoc = {};
    $scope.JxlCbDiHoc.data = [];
    $scope.ListErrorDH = [];
    $scope.LoadDataCBDiHoc = function (donViId) {
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoach_XuatKho/DanhSachTp_CbDiHoc',
            data: { donViId: donViId },
            success: function (response) {
                if (response.Error) {
                    toastr.error(response.Title);
                } else {
                    angular.forEach(response.data, function (val, key) {
                        $scope.ItemJexcel = [val.ID, val.SO_HIEU, val.GIOI_TINH, val.CAP_BAC_ID, val.LOAI_HAM_ID, val.CHUC_VU, val.CHUC_VU, val.LUC_LUONG_ID, val.CO_GIAY, val.CO_MU, val.CO_QA, val.DANG_KY_HV]
                        $scope.JxlCbDiHoc.data.push($scope.ItemJexcel);
                        $scope.tblCBDiHoc.setData($scope.Json2Arrary($scope.JxlCbDiHoc.data));
                        $scope.ListErrorDH = $scope.ValidateSoHieu(val, key);
                    });
                }
            }
        });
    }

    $scope.JxlCbDiHoc.colHeaders = ['', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K'];
    $scope.JxlCbDiHoc.column = [
        { type: 'text', width: '0' },
        { type: 'text', width: '100' },
        { type: 'dropdown', width: '100', source: [{ 'id': 'F', 'name': 'Nữ' }, { 'id': 'M', 'name': 'Nam' }] },
        { type: 'dropdown', autocomplete: true, width: '120', title: 'C', url: '/KeHoach_XuatKho/GetDMCapBacJexcel' },
        { type: 'dropdown', autocomplete: true, width: '120', title: 'D', url: '/KeHoach_XuatKho/GetDMLoaiHamJexcel' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'dropdown', autocomplete: true, width: '120', title: 'H', url: '/KeHoach_XuatKho/GetDMLucLuongJexcel' },
        { type: 'text', width: '50' },
        { type: 'text', width: '50' },
        { type: 'text', width: '50' },
        { type: 'text', width: '120' }
    ];
    $scope.JxlCbDiHoc.nestedHeaders = [
        [
            { title: '', rowspan: '2' },
            { title: 'Số hiệu CAND ', rowspan: '2' },
            { title: 'Giới tính ', rowspan: '2' },
            { title: 'Cấp bậc - loại hàm ', colspan: '2' },
            { title: 'Chức vụ - đơn vị - lực lượng', colspan: '3' },
            { title: 'Cỡ giầy ', rowspan: '2' },
            { title: 'Cỡ mũ  ', rowspan: '2' },
            { title: 'Cỡ QA ', rowspan: '2' },
            { title: 'Nhận hiện vật ', rowspan: '2' }
        ], [
            { title: ' ' },
            { title: ' ' },
            { title: ' ' },
            { title: 'Cấp bậc ' },
            { title: 'Loại hàm' },
            { title: 'Chức vụ' },
            { title: 'Vị trí công tác' },
            { title: 'Lực lượng' },
            { title: ' ' },
            { title: ' ' },
            { title: ' ' },
            { title: ' ' },
        ]
    ];

    $scope.JxlCbDiHoc.footers = [['', 'Tổng', '', '', '', '', '', '', '', '', '', '', '']];

    $scope.JxlCbDiHoc.changed = function (instance, cell, x, y, value) {
        var cellName = jexcel.getColumnNameFromId([5, 12]);
        //var cellVal = jexcel.current.options.getData(true);
        var sumcol1 = SUMCOL(jexcel.current, 12)
    };
    $scope.JxlCbDiHoc.updateTable = function (instance, cell, col, row, val, label, cellName) {
        angular.forEach($scope.ListErrorDH, function (v, k) {
            if (v.col === col && v.row === row) {
                cell.className = '';
                cell.style.backgroundColor = '#f46e42';
                cell.style.color = '#ffffff';
            }
        });

    };
    $scope.CBDiHocCallBack = function (data) {
        $scope.tblCBDiHoc = data;
    }
    //#endregion
    //#region Danh sách chiến sỹ nghĩa vụ
    $scope.JxlCsNghiaVu = {};
    $scope.JxlCsNghiaVu.data = [];
    $scope.ListErrorNV = [];
    $scope.LoadDataCSNghiaVu = function (donViId) {
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoach_XuatKho/DanhSachTp_CsNghiaVu',
            data: { donViId: donViId },
            success: function (response) {
                if (response.Error) {
                    toastr.error(response.Title);
                } else {
                    angular.forEach(response.data, function (val, key) {
                        $scope.ItemJexcel = [val.ID, val.SO_HIEU, val.GIOI_TINH, val.CAP_BAC_ID, val.LOAI_HAM_ID, val.CHUC_VU, val.CHUC_VU, val.LUC_LUONG_ID, val.CO_GIAY, val.CO_MU, val.CO_QA]
                        $scope.JxlCsNghiaVu.data.push($scope.ItemJexcel);
                        $scope.tblCSNghiaVu.setData($scope.Json2Arrary($scope.JxlCsNghiaVu.data));
                        $scope.ListErrorNV = $scope.ValidateSoHieu(val, key);

                    });
                }
            }
        });
    }

    $scope.JxlCsNghiaVu.colHeaders = ['', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J'];
    $scope.JxlCsNghiaVu.column = [
        { type: 'text', width: '0' },
        { type: 'text', width: '100' },
        { type: 'dropdown', width: '100', source: [{ 'id': 'F', 'name': 'Nữ' }, { 'id': 'M', 'name': 'Nam' }] },
        { type: 'dropdown', autocomplete: true, width: '120', url: '/KeHoach_XuatKho/GetDMCapBacJexcel' },
        { type: 'dropdown', autocomplete: true, width: '120', url: '/KeHoach_XuatKho/GetDMLoaiHamJexcel' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'dropdown', autocomplete: true, width: '120', url: '/KeHoach_XuatKho/GetDMLucLuongJexcel' },
        { type: 'text', width: '120' },
        { type: 'text', width: '50' },
        { type: 'text', width: '50' },
    ];
    $scope.JxlCsNghiaVu.nestedHeaders = [
        [
            { title: '', rowspan: '2' },
            { title: 'Số hiệu CAND ', rowspan: '2' },
            { title: 'Giới tính ', rowspan: '2' },
            { title: 'Cấp bậc - loại hàm (hiện tại) ', colspan: '2' },
            { title: 'Chức vụ - đơn vị công tác - lực lượng (hiện tại)', colspan: '3' },
            { title: 'Cỡ giầy ', rowspan: '2' },
            { title: 'Cỡ mũ  ', rowspan: '2' },
            { title: 'Cỡ QA ', rowspan: '2' }

        ], [
            { title: 'Cấp bậc ' },
            { title: 'Loại hàm' },
            { title: 'Chức vụ' },
            { title: 'Vị trí công tác' },
            { title: 'Lực lượng' },
        ]
    ];

    $scope.JxlCsNghiaVu.footers = [['', 'Tổng', '', '', '', '', '', '', '', '', '']];
    $scope.JxlCsNghiaVu.changed = function (instance, cell, x, y, value) {
        var cellName = jexcel.getColumnNameFromId([5, 12]);
        //var cellVal = jexcel.current.options.getData(true);
        var sumcol1 = SUMCOL(jexcel.current, 12)
    };
    $scope.JxlCsNghiaVu.updateTable = function (instance, cell, col, row, val, label, cellName) {
        angular.forEach($scope.ListErrorNV, function (v, k) {
            if (v.col === col && v.row === row) {
                cell.className = '';
                cell.style.backgroundColor = '#f46e42';
                cell.style.color = '#ffffff';
            }
        });

    };
    $scope.CSNghiaVuCallBack = function (data) {
        $scope.tblCSNghiaVu = data;
    }
    //#endregion
    //#region Danh sách tuyển mới, phong hàm
    $scope.JxlTuyenMoiPhHam = {};
    $scope.JxlTuyenMoiPhHam.data = [];
    $scope.ListErrorTM = [];
    $scope.LoadDataCBTuyenMoi = function (donViId) {
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoach_XuatKho/DanhSachTp_TuyenMoi',
            data: { donViId: donViId },
            success: function (response) {
                if (response.Error) {
                    toastr.error(response.Title);
                } else {
                    angular.forEach(response.data, function (val, key) {
                        $scope.ItemJexcel = [val.ID, val.SO_HIEU, val.GIOI_TINH, val.CAP_BAC_ID, val.LOAI_HAM_ID, val.CHUC_VU, val.CHUC_VU, val.LUC_LUONG_ID, val.CO_GIAY, val.CO_MU, val.CO_QA, val.NGANH_NGOAI, val.TRUONG_CAND, val.TBINH_TUYEN_LAI]
                        $scope.JxlTuyenMoiPhHam.data.push($scope.ItemJexcel);
                        $scope.tblCTuyenMoi.setData($scope.Json2Arrary($scope.JxlTuyenMoiPhHam.data));
                        $scope.ListErrorTM = $scope.ValidateSoHieu(val, key);
                    });
                }
                //$scope.$apply();
            }
        });
    }
    $scope.JxlTuyenMoiPhHam.colHeaders = ['', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M'];
    $scope.JxlTuyenMoiPhHam.column = [
        { type: 'text', width: '0' },
        { type: 'text', width: '100' },
        { type: 'dropdown', width: '100', source: [{ 'id': 'F', 'name': 'Nữ' }, { 'id': 'M', 'name': 'Nam' }] },
        { type: 'dropdown', autocomplete: true, width: '120', url: '/KeHoach_XuatKho/GetDMCapBacJexcel' },
        { type: 'dropdown', autocomplete: true, width: '120', url: '/KeHoach_XuatKho/GetDMLoaiHamJexcel' },
        { type: 'text', width: '120' },
        { type: 'text', width: '120' },
        { type: 'dropdown', autocomplete: true, width: '120', url: '/KeHoach_XuatKho/GetDMLucLuongJexcel' },
        { type: 'text', width: '50' },
        { type: 'text', width: '50' },
        { type: 'text', width: '50' },
        { type: 'text', width: '100' },
        { type: 'text', width: '100' },
        { type: 'text', width: '100' }
    ];
    $scope.JxlTuyenMoiPhHam.nestedHeaders = [
        [
            { title: '', rowspan: '2' },
            { title: 'Số hiệu CAND ', rowspan: '2' },
            { title: 'Giới tính ', rowspan: '2' },
            { title: 'Cấp bậc - loại hàm (hiện tại) ', colspan: '2' },
            { title: 'Chức vụ - đơn vị công tác - lực lượng (hiện tại)', colspan: '3' },
            { title: 'Cỡ giầy ', rowspan: '2' },
            { title: 'Cỡ mũ  ', rowspan: '2' },
            { title: 'Cỡ QA ', rowspan: '2' },
            { title: 'Tuyển mới CBCS (phong hàm lần đầu) ', colspan: '3' }
        ], [
            { title: ' ' },
            { title: ' ' },
            { title: ' ' },
            { title: 'Cấp bậc ' },
            { title: 'Loại hàm' },
            { title: 'Chức vụ' },
            { title: 'Vị trí công tác' },
            { title: 'Lực lượng' },
            { title: ' ' },
            { title: ' ' },
            { title: ' ' },
            { title: 'Ngành ngoài' },
            { title: 'Trường CAND' },
            { title: 'T.binh tuyển lại' }
        ]
    ];

    $scope.JxlTuyenMoiPhHam.footers = [['', 'Tổng', '', '', '', '', '', '', '', '', '', '', '', '']];
    $scope.JxlTuyenMoiPhHam.changed = function (instance, cell, x, y, value) {
        var cellName = jexcel.getColumnNameFromId([5, 12]);
        //var cellVal = jexcel.current.options.getData(true);
        var sumcol1 = SUMCOL(jexcel.current, 12)
        alert(value)
    };
    $scope.JxlTuyenMoiPhHam.updateTable = function (instance, cell, col, row, val, label, cellName) {
        angular.forEach($scope.ListErrorTM, function (v, k) {
            if (v.col === col && v.row === row) {
                cell.className = '';
                cell.style.backgroundColor = '#f46e42';
                cell.style.color = '#ffffff';
            }
        });

    };
    $scope.CBTuyenMoiCallBack = function (data) {
        $scope.tblCTuyenMoi = data;
    }
    //#endregion
    //#region Danh sách Phạm nhân
    $scope.tblPhamNhan;
    $scope.JxlPhamNhan = {};
    $scope.JxlPhamNhan.data = [];
    $scope.ListErrorPN = [];
    $scope.LoadDataPhamNhan = function (donViId) {
        $.ajax({
            type: 'post',
            async: false,
            url: '/KeHoach_XuatKho/GetDanhSachPhamNhan',
            data: { donViId: donViId },
            success: function (response) {
                if (response.Error) {
                    toastr.error(response.Title);
                } else {
                    if (response.data.length > 0) {
                        $scope.JxlPhamNhan.data = [];
                        angular.forEach(response.data, function (val, key) {
                            $scope.ItemJexcel = [val.ID, val.LOAI_PHAM_ID, val.SL_NAM, val.SL_NU, val.TONG]
                            $scope.JxlPhamNhan.data.push($scope.ItemJexcel);
                            $scope.tblPhamNhan.setData($scope.Json2Arrary($scope.JxlPhamNhan.data));
                            //$scope.ListErrorTM = $scope.ValidateSoHieu(val, key);
                        });
                    }
                    else {
                        $scope.JxlPhamNhan.data = [];
                        $scope.ItemJexcel = ['', '', '', '', '']
                        $scope.JxlPhamNhan.data.push($scope.ItemJexcel);
                        $scope.tblPhamNhan.setData($scope.Json2Arrary($scope.JxlPhamNhan.data));
                    }
                    
                }
            }
        });
    }
    
    $scope.JxlPhamNhan.column = [
        { type: 'text', width: '0', title: '' },
        { type: 'dropdown', width: '400', url: '/KeHoach_XuatKho/GetLoaiPham', autocomplete: true, title: 'Loại phạm' },
        { type: 'number', width: '100', title: 'Nam' },
        { type: 'number', width: '100', title: 'Nữ' },
        { type: 'number', width: '120', title: 'Tổng cộng' },
    ];
    $scope.JxlPhamNhan.footers = [['', 'Tổng', '', '', '']];
 
    var SUMCOL = function (instance, columnId) {
        var total = 0;
        for (var j = 0; j < instance.options.data.length; j++) {
            if (Number(instance.records[j][columnId - 1].innerHTML)) {
                total += Number(instance.records[j][columnId - 1].innerHTML.replace('.', ''));
            }
        }
        return total;
    }

    $scope.ExportTemplate = function () {
        window.location.href = '/KeHoach_XuatKho/ExportTemplate';
    }

    $scope.SavePhamNhan = function () {
        $scope.ListInsert = [];
        $scope.ListUpdate = [];
        $scope.ListDelete = [];
        angular.forEach($scope.tblPhamNhan.getJson(), function (val, key) {
            $scope.ItemData = {};
            $scope.ItemData.ID = val[0];
            $scope.ItemData.DON_VI_ID = $scope.DON_VI_ID;
            $scope.ItemData.LOAI_PHAM_ID = val[1];
            $scope.ItemData.NAM = $scope.NAM;
            $scope.ItemData.KY = $scope.KY;
            $scope.ItemData.SL_NAM = val[2];
            $scope.ItemData.SL_NU = val[3];
            $scope.ItemData.TONG = val[4];
            $scope.ItemData.NGAY_TAO = new Date();
            if (val[0] === '' || val[0] === 0)
                $scope.ListInsert.push($scope.ItemData);
            else
                $scope.ListUpdate.push($scope.ItemData);
        });
        $.ajax({
            type: 'post',
            url: '/KeHoach_XuatKho/PhamNhanUpdate',
            data: { listThem: $scope.ListInsert, listSua: $scope.ListUpdate, listXoa: $scope.ListDelete },
            success: function (res) {
                console.log(res);
            }
        })
    }

    
    $scope.PhamNhanCallBack = function (data) {
        $scope.tblPhamNhan = data;
    }
    //#endregion
    $scope.Json2Arrary = function (data) {
        var lstJson = [];
        angular.forEach(data, function (val, key) {
            var itemData = {};
            angular.forEach(val, function (v, k) {
                itemData[k] = v;
            });
            lstJson.push(itemData);
        });
        return lstJson;
    }
});
app.controller('addFileChotQuanSoDauKy', function ($scope, $uibModalInstance, $ngConfirm, showToast, hideLoading, data) {
    $scope.model = {};

    $scope.Upload = function () {
        showToast();
        var fileUpload = $("#file-input").get(0);
        var files = fileUpload.files;
        var fileData = new FormData();
        fileData.append(files[0].name, files[0]);
        fileData.append('donViId', data.donViId);
        fileData.append('ky', data.ky);
        fileData.append('nam', data.nam);
        $.ajax({
            url: '/KeHoach_XuatKho/ImportFileExel',
            type: "POST",
            dataType: 'json',
            contentType: false, // Not to set any content header
            processData: false, // Not to process data
            data: fileData,
            success: function (result) {
                if (result.Error) {
                    toastr.error(result.Title);
                } else {
                    toastr.success(result.Title);
                    $scope.cancel();
                    hideLoading();
                    window.location.href = '/KeHoach_XuatKho/Index?txnid=' + result.txId
                }
            },
            error: function (err) {
                toastr.error("Lỗi nhập dữ liệu.")
            }
        });
    }
    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});
app.controller('addFileChotQuanSoTrongKy', function ($scope, $uibModalInstance, $ngConfirm, showToast, hideLoading) {
    $scope.model = {};

    $scope.Upload = function () {
        showToast();
        var fileUpload = $("#file-input").get(0);
        var files = fileUpload.files;
        var fileData = new FormData();
        fileData.append(files[0].name, files[0]);
        $.ajax({
            url: '/KeHoach_XuatKho/ImportFileExel',
            type: "POST",
            dataType: 'json',
            contentType: false, // Not to set any content header
            processData: false, // Not to process data
            data: fileData,
            success: function (result) {
                if (result.Error) {
                    toastr.error(result.Title);
                } else {
                    toastr.success(result.Title);
                    $scope.cancel();
                    hideLoading();
                    window.location.href = '/KeHoach_XuatKho/Index?txnid=' + result.txId
                }
            },
            error: function (err) {
                toastr.error("Lỗi nhập dữ liệu.")
            }
        });
    }
    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});
app.controller('danhSachFileLoiGanNhat', function ($scope, $uibModalInstance, $ngConfirm, showToast, hideLoading) {
    $scope.model = {};

    $.ajax({
        type: 'get',
        data: {},
        url: '/KeHoach_XuatKho/GetDanhSachLoiGanNhat',
        success: function (res) {
            $scope.ListCBCongTac = res.cbCongTac;
            $scope.ListCBBietPhai = res.cbBietPhai;
            $scope.ListCBChoHuu = res.cbChoHuu;
            $scope.ListCBDiHoc = res.cbDiHoc;
            $scope.ListCBTuyenMoi = res.cbTuyenMoi;
            $scope.ListCSNgiaVu = res.csNgiaVu;
            $scope.$apply();
        }
    })
    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});