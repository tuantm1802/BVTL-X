app.controller("KetQuaSangLocController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "kqslace_id";

    $scope.ListCity = [];
    $scope.ListNhomTBH = [];

    $scope.DoiTuongKHs = [];
    $scope.GioiTinhs = [];
    $scope.Tuois = [];
    $scope.KetQuaHIVs = [];
    $scope.ChatGayNghien3Thangs = [];
    $scope.SoChatGayNghiens = [];
    $scope.ChatGayNghienSDThuongXuyens = [];
    $scope.DuongSDMaTuyDas = [];
    $scope.TanSuatSDMaTuyDas = [];
    $scope.LanDauSDMaTuyDas = [];
    $scope.LoaiMaTuyDaSDDauTiens = [];
    $scope.NguyCoSDMaTuyDas = [];
    $scope.ChungBKTs = [];
    $scope.NguyCoTinhDucs = [];
    $scope.DungBCSs = [];
    $scope.SDMaTuyDaKhiQHTDs = [];
    $scope.QHTDTapThes = [];
    $scope.BanDams = [];
    $scope.NhieuNguycoTinhDucs = [];
    $scope.BenhSTIs = [];
    $scope.BenhLaos = [];
    $scope.BenhVGCs = [];
    $scope.SocHeroins = [];
    $scope.SocMeths = [];
    $scope.CacLoaiChatGayNghiens = [];
    $scope.KetQuaQSTs = [];
    $scope.MucDoGapVanDeSKTTs = [];
    $scope.TuLamHaiBanThans = [];
    $scope.CoTuSats = [];
    $scope.LoanThans = [];

    var dataTableDoiTuongKHs = null;
    var dataTableGioiTinhs = null;
    var dataTableTuois = null;
    var dataTableKetQuaHIVs = null;
    var dataTableChatGayNghien3Thangs = null;
    var dataTableSoChatGayNghiens = null;
    var dataTableChatGayNghienSDThuongXuyens = null;
    var dataTableDuongSDMaTuyDas = null;
    var dataTableTanSuatSDMaTuyDas = null;
    var dataTableLanDauSDMaTuyDas = null;
    var dataTableLoaiMaTuyDaSDDauTiens = null;
    var dataTableNguyCoSDMaTuyDas = null;
    var dataTableChungBKTs = null;
    var dataTableNguyCoTinhDucs = null;
    var dataTableDungBCSs = null;
    var dataTableSDMaTuyDaKhiQHTDs = null;
    var dataTableQHTDTapThes = null;
    var dataTableBanDams = null;
    var dataTableNhieuNguycoTinhDucs = null;
    var dataTableBenhSTIs = null;
    var dataTableBenhLaos = null;
    var dataTableBenhVGCs = null;
    var dataTableSocHeroins = null;
    var dataTableSocMeths = null;
    var dataTableCacLoaiChatGayNghiens = null;
    var dataTableKetQuaQSTs = null;
    var dataTableMucDoGapVanDeSKTTs = null;
    var dataTableTuLamHaiBanThans = null;
    var dataTableCoTuSats = null;
    var dataTableLoanThans = null;
    $scope.ParamIdSeleted = 0;
    angular.element(document).ready(function () {

        var date = new Date();
        $scope.modelSearch.FromDate = date;
        $scope.modelSearch.ToDate = date;

        $scope.ListCity = [];
        $scope.ListNhomTBH = [];
        GetBottomAction();
        $scope.LoadPage(1);
    });

    $scope.RoleBtnExportExcel = false;
    $scope.RoleBtnSearch = false;

    function GetBottomAction() {
        $.ajax({
            type: 'post',
            url: '/KetQuaSangLoc/GetBottomAction',
            cache: false,
            async: false,
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

                    $scope.ListCity = response.Citis;
                    $scope.modelSearch.CityCodes = $scope.ListCity[0].Code;
                    $scope.ListNhomTBH = response.NhomTBHs;
                    $scope.modelSearch.MaNhomTBH = $scope.ListNhomTBH[0].manhom_tbh;
                }
                //$scope.$apply();
            }
        });
    }

    $('#dataTableDoiTuongKHs').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableGioiTinhs').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableTuois').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableKetQuaHIVs').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableChatGayNghien3Thangs').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableSoChatGayNghiens').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableChatGayNghienSDThuongXuyens').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableDuongSDMaTuyDas').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableTanSuatSDMaTuyDas').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableLanDauSDMaTuyDas').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableLoaiMaTuyDaSDDauTiens').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableNguyCoSDMaTuyDas').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableChungBKTs').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableNguyCoTinhDucs').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableDungBCSs').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableSDMaTuyDaKhiQHTDs').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableQHTDTapThes').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableBanDams').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableNhieuNguycoTinhDucs').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableBenhSTIs').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableBenhLaos').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableBenhVGCs').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableSocHeroins').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableSocMeths').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableCacLoaiChatGayNghiens').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableKetQuaQSTs').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableMucDoGapVanDeSKTTs').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableTuLamHaiBanThans').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableCoTuSats').on('click', 'tr', function () { $(this).toggleClass('selected'); });
    $('#dataTableLoanThans').on('click', 'tr', function () { $(this).toggleClass('selected'); });

    $scope.LoadPage = function (genTable) {

        //if (genTable == 1) {
        //    $scope.modelSearch.CityCodes = $scope.ListCity[0].Code;
        //    $scope.modelSearch.MaNhomTBH = $scope.ListNhomTBH[0].manhom_tbh;
        //}

        $scope.DoiTuongKHs = [];
        $scope.GioiTinhs = [];
        $scope.Tuois = [];
        $scope.KetQuaHIVs = [];
        $scope.ChatGayNghien3Thangs = [];
        $scope.SoChatGayNghiens = [];
        $scope.ChatGayNghienSDThuongXuyens = [];
        $scope.DuongSDMaTuyDas = [];
        $scope.TanSuatSDMaTuyDas = [];
        $scope.LanDauSDMaTuyDas = [];
        $scope.LoaiMaTuyDaSDDauTiens = [];
        $scope.NguyCoSDMaTuyDas = [];
        $scope.ChungBKTs = [];
        $scope.NguyCoTinhDucs = [];
        $scope.DungBCSs = [];
        $scope.SDMaTuyDaKhiQHTDs = [];
        $scope.QHTDTapThes = [];
        $scope.BanDams = [];
        $scope.NhieuNguycoTinhDucs = [];
        $scope.BenhSTIs = [];
        $scope.BenhLaos = [];
        $scope.BenhVGCs = [];
        $scope.SocHeroins = [];
        $scope.SocMeths = [];
        $scope.CacLoaiChatGayNghiens = [];
        $scope.KetQuaQSTs = [];
        $scope.MucDoGapVanDeSKTTs = [];
        $scope.TuLamHaiBanThans = [];
        $scope.CoTuSats = [];
        $scope.LoanThans = [];

        // Check điều kiện tìm kiếm
        if ($scope.modelSearch.FromDate == null || $scope.modelSearch.FromDate == '' || $scope.modelSearch.FromDate == undefined) {
            toastr.error("Vui lòng chọn Từ ngày!");
            return;
        }

        if ($scope.modelSearch.ToDate == null || $scope.modelSearch.ToDate == '' || $scope.modelSearch.ToDate == undefined) {
            toastr.error("Vui lòng chọn Đến ngày!");
            return;
        }

        if ($scope.modelSearch.CityCodes == null || $scope.modelSearch.CityCodes == '' || $scope.modelSearch.CityCodes == undefined) {
            toastr.error("Vui lòng chọn Tỉnh!");
            return;
        }

        if ($scope.modelSearch.MaNhomTBH == null || $scope.modelSearch.MaNhomTBH == '' || $scope.modelSearch.MaNhomTBH == undefined) {
            toastr.error("Vui lòng chọn Nhóm TBH!");
            return;
        }
        showToast();
        var inputSearch = {
            //_FromDate: $scope.modelSearch.FromDate,
            //_ToDate: $scope.modelSearch.ToDate,
            FromDate: $scope.modelSearch.FromDate.toISOString().slice(0, 10).replace(/-/g, ""),
            ToDate: $scope.modelSearch.ToDate.toISOString().slice(0, 10).replace(/-/g, ""),
            CityCodes: $scope.modelSearch.CityCodes,
            MaNhomTBH: $scope.modelSearch.MaNhomTBH,
            TuSoMaKH: null,
            DenSoMaKH: null
        };

        //var res = $scope.modelSearch.FromDate.toISOString().slice(0, 10).replace(/-/g, "");

        //var res1 = $scope.modelSearch.ToDate.toISOString().slice(0, 10).replace(/-/g, "");

        if ($scope.modelSearch.TuSoMaKH != null && $scope.modelSearch.TuSoMaKH != '') {
            inputSearch.TuSoMaKH = parseInt($scope.modelSearch.TuSoMaKH.replace($scope.modelSearch.MaNhomTBH, ''));
        } 

        if ($scope.modelSearch.DenSoMaKH != null && $scope.modelSearch.DenSoMaKH != '') {
            inputSearch.DenSoMaKH = parseInt($scope.modelSearch.DenSoMaKH.replace($scope.modelSearch.MaNhomTBH, ''));
        }

        // Lấy dữ liệu báo cáo
        $.ajax({
            type: 'post',
            url: '/KetQuaSangLoc/GetAll',
            cache: false,
            async: false,
            data: inputSearch,
            success: function (respone) {
                $scope.DoiTuongKHs = respone.DoiTuongKHs;
                $scope.GioiTinhs = respone.GioiTinhs;
                $scope.Tuois = respone.Tuois;
                $scope.KetQuaHIVs = respone.KetQuaHIVs;
                $scope.ChatGayNghien3Thangs = respone.ChatGayNghien3Thangs;
                $scope.SoChatGayNghiens = respone.SoChatGayNghiens;
                $scope.ChatGayNghienSDThuongXuyens = respone.ChatGayNghienSDThuongXuyens;
                $scope.DuongSDMaTuyDas = respone.DuongSDMaTuyDas;
                $scope.TanSuatSDMaTuyDas = respone.TanSuatSDMaTuyDas;
                $scope.LanDauSDMaTuyDas = respone.LanDauSDMaTuyDas;
                $scope.LoaiMaTuyDaSDDauTiens = respone.LoaiMaTuyDaSDDauTiens;
                $scope.NguyCoSDMaTuyDas = respone.NguyCoSDMaTuyDas;
                $scope.ChungBKTs = respone.ChungBKTs;
                $scope.NguyCoTinhDucs = respone.NguyCoTinhDucs;
                $scope.DungBCSs = respone.DungBCSs;
                $scope.SDMaTuyDaKhiQHTDs = respone.SDMaTuyDaKhiQHTDs;
                $scope.QHTDTapThes = respone.QHTDTapThes;
                $scope.BanDams = respone.BanDams;
                $scope.NhieuNguycoTinhDucs = respone.NhieuNguycoTinhDucs;
                console.log(respone.NhieuNguycoTinhDucs);
                $scope.BenhSTIs = respone.BenhSTIs;
                $scope.BenhLaos = respone.BenhLaos;
                $scope.BenhVGCs = respone.BenhVGCs;
                $scope.SocHeroins = respone.SocHeroins;
                $scope.SocMeths = respone.SocMeths;
                $scope.CacLoaiChatGayNghiens = respone.CacLoaiChatGayNghiens;
                $scope.KetQuaQSTs = respone.KetQuaQSTs;
                $scope.MucDoGapVanDeSKTTs = respone.MucDoGapVanDeSKTTs;
                $scope.TuLamHaiBanThans = respone.TuLamHaiBanThans;
                $scope.CoTuSats = respone.CoTuSats;
                $scope.LoanThans = respone.LoanThans;
            }
        });

        $scope.ListKetQuaSangLoc = [];
        if (genTable == 1 || genTable == 2) {
            if (genTable == 2) {
                DestroyTable();
            }
            // gen table
            GenTableDoiTuongKHs();
            GenTableGioiTinhs();
            GenTableTuois();
            GenTableKetQuaHIVs();
            GenTableChatGayNghien3Thangs();
            GenTableSoChatGayNghiens();
            GenTableChatGayNghienSDThuongXuyens();
            GenTableDuongSDMaTuyDas();
            GenTableTanSuatSDMaTuyDas();
            GenTableLanDauSDMaTuyDas();
            GenTableLoaiMaTuyDaSDDauTiens();
            GenTableNguyCoSDMaTuyDas();
            GenTableChungBKTs();
            GenTableNguyCoTinhDucs();
            GenTableDungBCSs();
            GenTableSDMaTuyDaKhiQHTDs();
            GenTableQHTDTapThes();
            GenTableBanDams();
            GenTableNhieuNguycoTinhDucs();
            GenTableBenhSTIs();
            GenTableBenhLaos();
            GenTableBenhVGCs();
            GenTableSocHeroins();
            GenTableSocMeths();
            GenTableCacLoaiChatGayNghiens();
            GenTableKetQuaQSTs();
            GenTableMucDoGapVanDeSKTTs();
            GenTableTuLamHaiBanThans();
            GenTableCoTuSats();
            GenTableLoanThans();

        } else {
            ReloadTable();
        }
        hideLoading();
    };

    function ReloadTable(){
        dataTableDoiTuongKHs.ajax.reload();
        dataTableGioiTinhs.ajax.reload();
        dataTableTuois.ajax.reload();
        dataTableKetQuaHIVs.ajax.reload();
        dataTableChatGayNghien3Thangs.ajax.reload();
        dataTableSoChatGayNghiens.ajax.reload();
        dataTableChatGayNghienSDThuongXuyens.ajax.reload();
        dataTableDuongSDMaTuyDas.ajax.reload();
        dataTableTanSuatSDMaTuyDas.ajax.reload();
        dataTableLanDauSDMaTuyDas.ajax.reload();
        dataTableLoaiMaTuyDaSDDauTiens.ajax.reload();
        dataTableNguyCoSDMaTuyDas.ajax.reload();
        dataTableChungBKTs.ajax.reload();
        dataTableNguyCoTinhDucs.ajax.reload();
        dataTableDungBCSs.ajax.reload();
        dataTableSDMaTuyDaKhiQHTDs.ajax.reload();
        dataTableQHTDTapThes.ajax.reload();
        dataTableBanDams.ajax.reload();
        dataTableNhieuNguycoTinhDucs.ajax.reload();
        dataTableBenhSTIs.ajax.reload();
        dataTableBenhLaos.ajax.reload();
        dataTableBenhVGCs.ajax.reload();
        dataTableSocHeroins.ajax.reload();
        dataTableSocMeths.ajax.reload();
        dataTableCacLoaiChatGayNghiens.ajax.reload();
        dataTableKetQuaQSTs.ajax.reload();
        dataTableMucDoGapVanDeSKTTs.ajax.reload();
        dataTableTuLamHaiBanThans.ajax.reload();
        dataTableCoTuSats.ajax.reload();
        dataTableLoanThans.ajax.reload();
    }

    function DestroyTable() {
        dataTableDoiTuongKHs.destroy();
        dataTableGioiTinhs.destroy();
        dataTableTuois.destroy();
        dataTableKetQuaHIVs.destroy();
        dataTableChatGayNghien3Thangs.destroy();
        dataTableSoChatGayNghiens.destroy();
        dataTableChatGayNghienSDThuongXuyens.destroy();
        dataTableDuongSDMaTuyDas.destroy();
        dataTableTanSuatSDMaTuyDas.destroy();
        dataTableLanDauSDMaTuyDas.destroy();
        dataTableLoaiMaTuyDaSDDauTiens.destroy();
        dataTableNguyCoSDMaTuyDas.destroy();
        dataTableChungBKTs.destroy();
        dataTableNguyCoTinhDucs.destroy();
        dataTableDungBCSs.destroy();
        dataTableSDMaTuyDaKhiQHTDs.destroy();
        dataTableQHTDTapThes.destroy();
        dataTableBanDams.destroy();
        dataTableNhieuNguycoTinhDucs.destroy();
        dataTableBenhSTIs.destroy();
        dataTableBenhLaos.destroy();
        dataTableBenhVGCs.destroy();
        dataTableSocHeroins.destroy();
        dataTableSocMeths.destroy();
        dataTableCacLoaiChatGayNghiens.destroy();
        dataTableKetQuaQSTs.destroy();
        dataTableMucDoGapVanDeSKTTs.destroy();
        dataTableTuLamHaiBanThans.destroy();
        dataTableCoTuSats.destroy();
        dataTableLoanThans.destroy();
    }

    function GenTableDoiTuongKHs(){
        dataTableDoiTuongKHs = $('#dataTableDoiTuongKHs').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "SoNguoi", searchBuilderType: "number" },
                { "data": "PhanTram", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableGioiTinhs() {
        dataTableGioiTinhs = $('#dataTableGioiTinhs').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "SoNguoi", searchBuilderType: "number" },
                { "data": "PhanTram", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableTuois() {
        dataTableTuois = $('#dataTableTuois').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "SoNguoi", searchBuilderType: "number" },
                { "data": "PhanTram", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableKetQuaHIVs() {
        dataTableKetQuaHIVs = $('#dataTableKetQuaHIVs').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "SoNguoi", searchBuilderType: "number" },
                { "data": "PhanTram", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableChatGayNghien3Thangs() {
        dataTableChatGayNghien3Thangs = $('#dataTableChatGayNghien3Thangs').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "SoNguoi", searchBuilderType: "number" },
                { "data": "PhanTram", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableSoChatGayNghiens() {
        dataTableSoChatGayNghiens = $('#dataTableSoChatGayNghiens').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "SoNguoi", searchBuilderType: "number" },
                { "data": "PhanTram", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableChatGayNghienSDThuongXuyens() {
        dataTableChatGayNghienSDThuongXuyens = $('#dataTableChatGayNghienSDThuongXuyens').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "SoNguoi", searchBuilderType: "number" },
                { "data": "PhanTram", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableDuongSDMaTuyDas() {
        
        dataTableDuongSDMaTuyDas = $('#dataTableDuongSDMaTuyDas').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "HutHit", searchBuilderType: "number" },
                { "data": "DangBot", searchBuilderType: "number" },
                { "data": "UongNuot", searchBuilderType: "number" },
                { "data": "TiemChich", searchBuilderType: "number" },
                { "data": "KBKTL", searchBuilderType: "number" }
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
                }
            },
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableTanSuatSDMaTuyDas() {
        dataTableTanSuatSDMaTuyDas = $('#dataTableTanSuatSDMaTuyDas').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "VaiLan1Ngay", searchBuilderType: "number" },
                { "data": "HangNgay", searchBuilderType: "number" },
                { "data": "VaiLan1Tuan", searchBuilderType: "number" },
                { "data": "VaiNgayRoiTamNghi", searchBuilderType: "number" },
                { "data": "DungCuoiTuan", searchBuilderType: "number" },
                { "data": "VaiLan1Thang", searchBuilderType: "number" },
                { "data": "ItHon1Lan1Thang", searchBuilderType: "number" },
                { "data": "KBKTL", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableLanDauSDMaTuyDas() {
        dataTableLanDauSDMaTuyDas = $('#dataTableLanDauSDMaTuyDas').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "_13", searchBuilderType: "number" },
                { "data": "_14", searchBuilderType: "number" },
                { "data": "_15", searchBuilderType: "number" },
                { "data": "_16", searchBuilderType: "number" },
                { "data": "_17", searchBuilderType: "number" },
                { "data": "_18", searchBuilderType: "number" },
                { "data": "_19", searchBuilderType: "number" },
                { "data": "_20", searchBuilderType: "number" },
                { "data": "_21", searchBuilderType: "number" },
                { "data": "_22", searchBuilderType: "number" },
                { "data": "_23", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableLoaiMaTuyDaSDDauTiens() {
        dataTableLoaiMaTuyDaSDDauTiens = $('#dataTableLoaiMaTuyDaSDDauTiens').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,
            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "Da", searchBuilderType: "number" },
                { "data": "Keo", searchBuilderType: "number" },
                { "data": "CanCo", searchBuilderType: "number" },
                { "data": "Ketamin", searchBuilderType: "number" },
                { "data": "BongCuoi", searchBuilderType: "number" },
                { "data": "Heroin", searchBuilderType: "number" },
                { "data": "CacChatHit", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableNguyCoSDMaTuyDas() {
        dataTableNguyCoSDMaTuyDas = $('#dataTableNguyCoSDMaTuyDas').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,
            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "ChuaBaoGio", searchBuilderType: "number" },
                { "data": "DaTungTiemChich", searchBuilderType: "number" },
                { "data": "VanDangTiemChich", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableChungBKTs() {
        dataTableChungBKTs = $('#dataTableChungBKTs').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "ChuaBaoGio", searchBuilderType: "number" },
                { "data": "DaTungDungChung", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableNguyCoTinhDucs() {
        dataTableNguyCoTinhDucs = $('#dataTableNguyCoTinhDucs').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "ChuaBaoGio", searchBuilderType: "number" },
                { "data": "DongGioi", searchBuilderType: "number" },
                { "data": "KhacGioi", searchBuilderType: "number" },
                { "data": "CaHai", searchBuilderType: "number" },
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableDungBCSs() {
        dataTableDungBCSs = $('#dataTableDungBCSs').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "LuonLuon", searchBuilderType: "number" },
                { "data": "ThuongXuyen", searchBuilderType: "number" },
                { "data": "ThiThoang", searchBuilderType: "number" },
                { "data": "HiemKhi", searchBuilderType: "number" },
                { "data": "KhongBaoGio", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableSDMaTuyDaKhiQHTDs() {
        dataTableSDMaTuyDaKhiQHTDs = $('#dataTableSDMaTuyDaKhiQHTDs').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "Co", searchBuilderType: "number" },
                { "data": "Khong", searchBuilderType: "number" },
                { "data": "KBKTL", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableQHTDTapThes() {
        dataTableQHTDTapThes = $('#dataTableQHTDTapThes').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "Co", searchBuilderType: "number" },
                { "data": "Khong", searchBuilderType: "number" },
                { "data": "KBKTL", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableBanDams() {
        dataTableBanDams = $('#dataTableBanDams').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "Co", searchBuilderType: "number" },
                { "data": "Khong", searchBuilderType: "number" },
                { "data": "KBKTL", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableNhieuNguycoTinhDucs() {
        dataTableNhieuNguycoTinhDucs = $('#dataTableNhieuNguycoTinhDucs').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "Mot", searchBuilderType: "number" },
                { "data": "Hai", searchBuilderType: "number" },
                { "data": "Ba", searchBuilderType: "number" },
                { "data": "BonNam", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableBenhSTIs() {
        dataTableBenhSTIs = $('#dataTableBenhSTIs').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "Lau", searchBuilderType: "number" },
                { "data": "SuiMaoGa", searchBuilderType: "number" },
                { "data": "KhongMac", searchBuilderType: "number" },
                { "data": "Khac", searchBuilderType: "number" },
                { "data": "GiangMai", searchBuilderType: "number" },
                { "data": "KBKTL", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableBenhLaos() {
        dataTableBenhLaos = $('#dataTableBenhLaos').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "HienTai_SoLuong", searchBuilderType: "number" },
                { "data": "HienTai_PhanTram", searchBuilderType: "number" },
                { "data": "QuaKhu_SoLuong", searchBuilderType: "number" },
                { "data": "QuaKhu_PhanTram", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableBenhVGCs() {
        dataTableBenhVGCs = $('#dataTableBenhVGCs').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "HienTai_SoLuong", searchBuilderType: "number" },
                { "data": "HienTai_PhanTram", searchBuilderType: "number" },
                { "data": "QuaKhu_SoLuong", searchBuilderType: "number" },
                { "data": "QuaKhu_PhanTram", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableSocHeroins() {
        dataTableSocHeroins = $('#dataTableSocHeroins').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "SoNguoi", searchBuilderType: "number" },
                { "data": "PhanTram", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableSocMeths() {
        dataTableSocMeths = $('#dataTableSocMeths').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "SoNguoi", searchBuilderType: "number" },
                { "data": "PhanTram", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableCacLoaiChatGayNghiens() {
        dataTableCacLoaiChatGayNghiens = $('#dataTableCacLoaiChatGayNghiens').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "NguyCoThap_SoLuong", searchBuilderType: "number" },
                { "data": "NguyCoThap_PhanTram", searchBuilderType: "number" },
                { "data": "NguyCoTrungBinh_SoLuong", searchBuilderType: "number" },
                { "data": "NguyCoTrungBinh_PhanTram", searchBuilderType: "number" },
                { "data": "NguyCoCao_SoLuong", searchBuilderType: "number" },
                { "data": "NguyCoCao_PhanTram", searchBuilderType: "number" },
                { "data": "Tong_SoLuong", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableKetQuaQSTs() {
        dataTableKetQuaQSTs = $('#dataTableKetQuaQSTs').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "SoNguoi", searchBuilderType: "number" },
                { "data": "PhanTram", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableMucDoGapVanDeSKTTs() {
        dataTableMucDoGapVanDeSKTTs = $('#dataTableMucDoGapVanDeSKTTs').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "KhongChutNao_SoLuong", searchBuilderType: "number" },
                { "data": "KhongChutNao_PhanTram", searchBuilderType: "number" },
                { "data": "Tu1Den7Ngay_SoLuong", searchBuilderType: "number" },
                { "data": "Tu1Den7Ngay_PhanTram", searchBuilderType: "number" },
                { "data": "Tu8NgayTroLen_SoLuong", searchBuilderType: "number" },
                { "data": "Tu8NgayTroLen_PhanTram", searchBuilderType: "number" },
                { "data": "GanNhuHangNgay_SoLuong", searchBuilderType: "number" },
                { "data": "GanNhuHangNgay_PhanTram", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableTuLamHaiBanThans() {
        dataTableTuLamHaiBanThans = $('#dataTableTuLamHaiBanThans').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "Co", searchBuilderType: "number" },
                { "data": "Khong", searchBuilderType: "number" },
                { "data": "KBKTL", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableCoTuSats() {
        dataTableCoTuSats = $('#dataTableCoTuSats').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "Co", searchBuilderType: "number" },
                { "data": "Khong", searchBuilderType: "number" },
                { "data": "KBKTL", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    function GenTableLoanThans() {
        dataTableLoanThans = $('#dataTableLoanThans').DataTable({
            lengthMenu: [10, 20, 30, 50, 60, 100],
            //serverSide: true,
            ordering: false,
            searching: true,
            processing: true,

            columns: [
                { "data": "NoiDung", searchBuilderType: "string" },
                { "data": "TheoDoiRinhRap_SoLuong", searchBuilderType: "number" },
                { "data": "TheoDoiRinhRap_PhanTram", searchBuilderType: "number" },
                { "data": "YNghi_SoLuong", searchBuilderType: "number" },
                { "data": "YNghi_PhanTram", searchBuilderType: "number" },
                { "data": "NgheThuMaNKKNT_SoLuong", searchBuilderType: "number" },
                { "data": "NgheThuMaNKKNT_PhanTram", searchBuilderType: "number" }
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
            dom:
                //"<'row'<'col-sm-8'Q>>" +
                //"<'row'<'col-sm-8'B><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" //+
            // "<'row'<'col-sm-3'i><'col-sm-3'l><'col-sm-6'p>>"
            ,
            scroller: {
                loadingIndicator: true
            }
            //,
            //columnDefs: [{
            //    searchBuilder: {
            //        defaultCondition: "="
            //    },
            //    targets: [1]
            //}]
        });
    }

    $scope.Refesh = function () {
        $scope.LoadPage(0);
    };

    $scope.ExportExcel = function () {
        window.location.href = '/KetQuaSangLoc/ExportData?keyword=' + $scope.modelSearch.KeyWord;
    }

});