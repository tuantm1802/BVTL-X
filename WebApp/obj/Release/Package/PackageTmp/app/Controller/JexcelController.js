app.controller("JexcelController", function ($scope) {
    $scope.JexcelInit = {};
    $scope.JexcelInit.data = [
        ['', '', '', '', true, 0, 0, '=F1*G1', '#777700'],
        ['', '', '', '', true, 0, 0, '=F2*G2', '#007777'],
        ['', '', '', '', true, 0, 0, '=F3*G3', '#777700'],
        ['', '', '', '', true, 0, 0, '=F4*G4', '#007777'],
        ['', '', '', '', true, 0, 0, '=F5*G5', '#777700'],
        ['', '', '', '', true, 0, 0, '=F6*G6', '#007777'],
        ['', '', '', '', true, 0, 0, '=F7*G7', '#777700'],
        ['', '', '', '', true, 0, 0, '=F8*G8', '#007777'],
        ['', '', '', '', true, 0, 0, '=F9*G9', '#777700'],
        ['', '', '', '', true, 0, 0, '=F10*G10', '#007777'],
        ['', '', '', '', true, 0, 0, '=F11*G11', '#777700'],
        ['', '', '', '', true, 0, 0, '=F12*G12', '#007777'],
    ];
    var ListUnit = [];
    $.ajax({
        type: 'post',
        url: '/User/GetDanhMuc',
        data: {},
        success: function (data) {
            angular.forEach(data.Units, function (val, key) {
                ListUnit.push({ id: val.UNIT_ID, name: val.UNIT_NAME });
            });
        }
    });
    $scope.JexcelInit.column = [
        { type: 'text', title: 'Vật tư (A)', width: 120 },
        { type: 'dropdown', title: 'Phòng ban (B)', width: 200, source: ListUnit },
        { type: 'calendar', title: 'Ngày nhập (C)', width: 200 },
        { type: 'image', title: 'Ảnh (D)', width: 120 },
        { type: 'checkbox', title: 'Sử dụng (E)', width: 80 },
        { type: 'numeric', title: 'Đơn giá (F)', width: 100, mask: '#,##.00', decimal: '.' },
        { type: 'numeric', title: 'Số lượng (G)', width: 100, mask: '#,##.00', decimal: '.' },
        { type: 'numeric', title: 'Thành tiền (H)', width: 100, mask: '#,##.00', decimal: '.' },
        { type: 'color', title: 'Màu sắc (I)', width: 100, render: 'square', }
    ];
    $scope.JexcelInit.footers = [['Tổng', '', '', '', '', '', '', '', '']];
    $scope.JexcelInit.changed = function (instance, cell, x, y, value) {
        var cellName = jexcel.getColumnNameFromId([5, 12]);
        //var cellVal = jexcel.current.options.getData(true);
        var sumcol1 = SUMCOL(jexcel.current, 12)
    };
    var SUMCOL = function (instance, columnId) {
        var total = 0;
        for (var j = 0; j < instance.options.data.length; j++) {
            if (Number(instance.records[j][columnId - 1].innerHTML)) {
                total += Number(instance.records[j][columnId - 1].innerHTML.replace('.', ''));
            }
        }
        return total;
    }
    $scope.export = function () {
        window.location.href = "/Home/ExportExcelCustom";
    }
});