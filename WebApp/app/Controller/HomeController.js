app.controller("HomeController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.xValuesHivGioiTinh = [];
    $scope.yValuesHivGioiTinh = [];
    $scope.hivGioiTinhDB = [];
    $scope.dbTanSuatChemsex3ThangTheoDoTuoi = {};
    $scope.dbTanSuatChemsex3ThangTheoDiemAssist = {};
    $scope.dbTanSuatChemsex3ThangTheoDiemACE = {};
    $scope.dbSuDungDaChatTrongChemsexTheoDoTuoi = {};
    $scope.dbSuDungDaChatTrongChemsexTheoDoiTuongQHTD = {};
    $scope.dbSuDungDaChatTrongChemsexTheoQHTDTT = {};
    $scope.dbSuDungDaChatTrongChemsexTheoBanDam = {};
    $scope.dbSuDungDaChatTrongChemsexTheoDiemAssistMaTuyDa = {};
    $scope.dbSuDungDaChatTrongChemsexTheoDiemACE = {};
    $scope.dbSuDungDaChatTrongChemsexTheoDiemQST = {};
    $scope.dbTanSuatChemsexTrong3ThangTheoDiemQST = {};

    $scope.ListNhomTBH = [];
    $scope.ListCity = [];
    $scope.selectedNhom = null;
    $scope.selectedNhomName = null;
    $scope.selectedTinh = null;
    $scope.selectedTinhName = null;

    $scope.isLoading = true; // Bật loading khi bắt đầu
    let loadCount = 14; // Số hàm cần gọi khi trang load

    function checkLoadingComplete() {
        loadCount--;
        console.log(loadCount);

        if (loadCount === 0) {
            $scope.isLoading = false; // Tắt loading khi tất cả hàm hoàn thành
            $scope.$apply(); // Đảm bảo Angular cập nhật view
        }
    }

    angular.element(document).ready(function () {
         
        //getDBHIVGioiTinh();
        //getDBHIVTinhTrang();
        //getDBHIVDoiTuong();
        //getDBHIVDoTuoi();

        const apiCode = 'API_ALL_CD43_KHACH_HANG_TTCB';
        Promise.resolve(fetchEndTimeSync(apiCode)).finally(checkLoadingComplete);
        Promise.resolve(GetTinh()).finally(checkLoadingComplete);
        Promise.resolve(GetNhomByTinh()).finally(checkLoadingComplete);    
        Promise.resolve(GetTanSuatChemsex3ThangTheoDoTuoi()).finally(checkLoadingComplete);
        Promise.resolve(GetTanSuatChemsex3ThangTheoDiemAssist()).finally(checkLoadingComplete);
        Promise.resolve(GetTanSuatChemsex3ThangTheoDiemACE()).finally(checkLoadingComplete);
        Promise.resolve(GetSuDungDaChatTrongChemsexTheoDoTuoi()).finally(checkLoadingComplete);
        Promise.resolve(GetSuDungDaChatTrongChemsexTheoDoiTuongQHTD()).finally(checkLoadingComplete);
        Promise.resolve(GetSuDungDaChatTrongChemsexTheoQHTDTT()).finally(checkLoadingComplete);
        Promise.resolve(GetSuDungDaChatTrongChemsexTheoBanDam()).finally(checkLoadingComplete);
        Promise.resolve(GetSuDungDaChatTrongChemsexTheoDiemAssistMaTuyDa()).finally(checkLoadingComplete);
        Promise.resolve(GetSuDungDaChatTrongChemsexTheoDiemACE()).finally(checkLoadingComplete);
        Promise.resolve(GetSuDungDaChatTrongChemsexTheoDiemQST()).finally(checkLoadingComplete);
        Promise.resolve(GetTanSuatChemsexTrong3ThangTheoDiemQST()).finally(checkLoadingComplete);          
        
    });
    function GetTinh() {        
        $.ajax({
            type: 'post',
            url: '/BaoCaoCD43/GetBottomAction',
            data: {},
            success: function (response) {
                $scope.ListCity = response.Citys;    
            }
        });
    }

    function GetNhomByTinh() {
        var CityCodes = '';
        $.ajax({
            type: 'post',
            url: '/BaoCaoCD43/GetNhomTBHByMaNhomMap',
            cache: false,
            async: false,
            data: {
                CityCodes: CityCodes
            },
            success: function (respone) {
                $scope.ListNhomTBH = respone.NhomTBHs;
            }
        });
    }

    // Xử lý khi chọn nhóm
    $scope.selectNhom = function (maNhom, nhomName) {
        $scope.selectedNhom = maNhom;
        $scope.selectedNhomName = nhomName;
        Promise.resolve(GetTanSuatChemsex3ThangTheoDoTuoi()).finally(checkLoadingComplete);  
    };

    // Xử lý khi chọn tỉnh
    $scope.selectTinh = function (maTinh, tinhName) {
        $scope.selectedTinh = maTinh;
        $scope.selectedTinhName = tinhName;
        Promise.resolve(GetTanSuatChemsex3ThangTheoDoTuoi()).finally(checkLoadingComplete);
    };

    function GetTanSuatChemsex3ThangTheoDoTuoi() {
        $.ajax({
            type: 'POST',
            url: '/Home/GetTanSuatChemsex3ThangTheoDoTuoi',
            data: { maNhom: $scope.selectedNhom, maTinh: $scope.selectedTinh },
            success: function (response) {
                
                $scope.dbTanSuatChemsex3ThangTheoDoTuoi = response.data;
                $scope.$apply();

            }
        });
    }
    function GetTanSuatChemsex3ThangTheoDiemAssist() {
        $.ajax({
            type: 'POST',
            url: '/Home/GetTanSuatChemsex3ThangTheoDiemAssist',
            data: {},
            success: function (response) {
                console.log(response);
                $scope.dbTanSuatChemsex3ThangTheoDiemAssist = response.data;
                $scope.$apply();

            }
        });
    }
    
    function GetTanSuatChemsex3ThangTheoDiemACE() {
        $.ajax({
            type: 'POST',
            url: '/Home/GetTanSuatChemsex3ThangTheoDiemACE',
            data: {},
            success: function (response) {
                console.log(response);
                $scope.dbTanSuatChemsex3ThangTheoDiemACE = response.data;
                $scope.$apply();

            }
        });
    }
    
    function GetSuDungDaChatTrongChemsexTheoDoTuoi() {
        $.ajax({
            type: 'POST',
            url: '/Home/GetSuDungDaChatTrongChemsexTheoDoTuoi',
            data: {},
            success: function (response) {
                console.log(response);
                $scope.dbSuDungDaChatTrongChemsexTheoDoTuoi = response.data;
                $scope.$apply();

            }
        });
    }
    
    function GetSuDungDaChatTrongChemsexTheoDoiTuongQHTD() {
        $.ajax({
            type: 'POST',
            url: '/Home/GetSuDungDaChatTrongChemsexTheoDoiTuongQHTD',
            data: {},
            success: function (response) {
                console.log(response);
                $scope.dbSuDungDaChatTrongChemsexTheoDoiTuongQHTD = response.data;
                $scope.$apply();

            }
        });
    }
    
    function GetSuDungDaChatTrongChemsexTheoQHTDTT() {
        $.ajax({
            type: 'POST',
            url: '/Home/GetSuDungDaChatTrongChemsexTheoQHTDTT',
            data: {},
            success: function (response) {
                console.log(response);
                $scope.dbSuDungDaChatTrongChemsexTheoQHTDTT = response.data;
                $scope.$apply();

            }
        });
    }
    
    function GetSuDungDaChatTrongChemsexTheoBanDam() {
        $.ajax({
            type: 'POST',
            url: '/Home/GetSuDungDaChatTrongChemsexTheoBanDam',
            data: {},
            success: function (response) {
                console.log(response);
                $scope.dbSuDungDaChatTrongChemsexTheoBanDam = response.data;
                $scope.$apply();

            }
        });
    }
    
    function GetSuDungDaChatTrongChemsexTheoDiemAssistMaTuyDa() {
        $.ajax({
            type: 'POST',
            url: '/Home/GetSuDungDaChatTrongChemsexTheoDiemAssistMaTuyDa',
            data: {},
            success: function (response) {
                console.log(response);
                $scope.dbSuDungDaChatTrongChemsexTheoDiemAssistMaTuyDa = response.data;
                $scope.$apply();

            }
        });
    }
    
    function GetSuDungDaChatTrongChemsexTheoDiemACE() {
        $.ajax({
            type: 'POST',
            url: '/Home/GetSuDungDaChatTrongChemsexTheoDiemACE',
            data: {},
            success: function (response) {
                console.log(response);
                $scope.dbSuDungDaChatTrongChemsexTheoDiemACE = response.data;
                $scope.$apply();

            }
        });
    }
    
    function GetSuDungDaChatTrongChemsexTheoDiemQST() {
        $.ajax({
            type: 'POST',
            url: '/Home/GetSuDungDaChatTrongChemsexTheoDiemQST',
            data: {},
            success: function (response) {
                console.log(response);
                $scope.dbSuDungDaChatTrongChemsexTheoDiemQST = response.data;
                $scope.$apply();

            }
        });
    }
    
    function GetTanSuatChemsexTrong3ThangTheoDiemQST() {
        $.ajax({
            type: 'POST',
            url: '/Home/GetTanSuatChemsexTrong3ThangTheoDiemQST',
            data: {},
            success: function (response) {
                console.log(response);
                $scope.dbTanSuatChemsexTrong3ThangTheoDiemQST = response.data;
                $scope.$apply();

            }
        });
    }

    function fetchEndTimeSync(apiCode) {
        $.ajax({
            url: '/SyncData/GetEndTimeSync',
            type: 'GET',
            data: { apiCode: apiCode },
            success: function (response) {
                if (response.success) {
                    // Hiển thị thông tin ngày giờ lấy được
                    $('#endTimeSyncDisplay').text('Dữ liệu được đồng bộ lần cuối vào lúc: ' + response.endTimeSync);
                } else {
                    $('#endTimeSyncDisplay').text(response.message);
                }
            },
            error: function () {
                $('#endTimeSyncDisplay').text('Đã xảy ra lỗi khi lấy dữ liệu');
            }
        });
    }

    function getDBHIVGioiTinh() {
        $.ajax({
            type: 'post',
            url: '/KetQuaHIV/getDBHIVGioiTinh',
            data: {},
            success: function (response) {
                if (response.data != null) {
                    console.log(response);

                    var collection = response.data;

                    var xVals = [];
                    var yVals = [];
                    for (var i in collection) {
                        xVals.push(collection[i]['xValues']);
                        yVals.push(collection[i]['yValues']);
                    }
                    $scope.xValuesHivGioiTinh = xVals;
                    $scope.yValuesHivGioiTinh = yVals;
                    
                }
                var barColors = ["red", "green","black"];
                new Chart("hivGioiTinhChart", {
                    plugins: [ChartDataLabels],
                    type: "bar",
                    data: {
                        labels: $scope.xValuesHivGioiTinh,
                        datasets: [{
                            label:'Hiv',
                            backgroundColor: barColors,
                            minBarLength: 10,
                            data: $scope.yValuesHivGioiTinh
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        layout: {
                            padding: {
                                left: 10,
                                right: 10,
                                top: 20,
                                bottom: 0
                            }
                        },
                        scales: {
                            x: {
                                ticks: {
                                    maxTicksLimit: 6
                                },
                                maxBarThickness: 25,
                            },
                            y: {
                                ticks: {
                                    min: 0,
                                    max: 30000,
                                    beginAtZero: true
                                }
                            }
                        },
                        plugins: {
                            legend: { display: false },
                            title: {
                                display: false,
                                text: "Thông tin HIV theo giới tính"
                            },
                            datalabels: {
                                anchor: 'end',
                                align: 'top',
                                formatter: Math.round,
                                font: {
                                    weight: 'bold'
                                }
                            }
                        },  
                    }
                });

                $scope.$apply();
                
            }
        });
    }

    function getDBHIVTinhTrang() {
        $scope.xValuesHiv = [];
        $scope.yValuesHiv = [];

        $.ajax({
            type: 'post',
            url: '/KetQuaHIV/getDBHIVTinhTrang',
            data: {},
            success: function (response) {
                if (response.data != null) {
                    console.log(response);

                    var collection = response.data;

                    var xVals = [];
                    var yVals = [];
                    for (var i in collection) {
                        xVals.push(collection[i]['xValues']);
                        yVals.push(collection[i]['yValues']);
                    }
                    $scope.xValuesHiv = xVals;
                    $scope.yValuesHiv = yVals;

                }
                /*var barColors = ["red", "green", "blue", "orange", "brown"];*/
                var barColors = [
                    "#b91d47",
                    "#00aba9",
                    "#2b5797",
                    "#e8c3b9",
                    "#1e7145"
                ];
                new Chart("hivTinhTrangChart", {
                    plugins: [ChartDataLabels],
                    type: "doughnut",
                    data: {
                        labels: $scope.xValuesHiv,
                        datasets: [{
                            label: 'Tình trạng HIV',
                            backgroundColor: barColors,
                            data: $scope.yValuesHiv,
                        }]
                    },
                    options: {
                        maintainAspectRatio: false,
                        responsive: true,
                        plugins: {
                            legend: {
                                display: true,
                                position: 'bottom',
                            },
                            title: {
                                display: false,
                                text: 'Tình trạng HIV'
                            },
                            datalabels: {
                                anchor: 'center',
                                align: 'top',
                                formatter: Math.round,
                                font: {
                                    weight: 'bold'
                                },
                                color: '#FFFFFF'
                            }
                        },
                       
                        cutoutPercentage: 80,
                    },
                });

            }
        });
    }

   
    function getDBHIVDoiTuong() {
        $scope.xValuesHiv = [];
        $scope.yValuesHiv = [];

        $.ajax({
            type: 'post',
            url: '/KetQuaHIV/getDBHIVDoiTuong',
            data: {},
            success: function (response) {
                if (response.data != null) {
                    console.log(response);

                    var collection = response.data;

                    var xVals = [];
                    var yVals = [];
                    for (var i in collection) {
                        xVals.push(collection[i]['xValues']);
                        yVals.push(collection[i]['yValues']);
                    }
                    $scope.xValuesHiv = xVals;
                    $scope.yValuesHiv = yVals;

                }
                /*var barColors = ["red", "green", "blue", "orange", "brown"];*/
                var barColors = [
                    "#b91d47",
                    "#00aba9",
                    "#2b5797",
                    "#e8c3b9",
                    "#1e7145"
                ];
                new Chart("hivDoiTuongChart", {
                    plugins: [ChartDataLabels],
                    type: "pie",
                    data: {
                        labels: $scope.xValuesHiv,
                        datasets: [{
                            label: 'Tình trạng HIV theo Đối tượng',
                            backgroundColor: barColors,
                            data: $scope.yValuesHiv,
                        }]
                    },
                    options: {
                        maintainAspectRatio: false,
                        responsive: true,
                        plugins: {
                            legend: {
                                display: true,
                                position: 'bottom',
                            },
                            title: {
                                display: false,
                                text: 'Tình trạng HIV theo Đối tượng'
                            },
                            datalabels: {
                                anchor: 'center',
                                align: 'top',
                                formatter: Math.round,
                                font: {
                                    weight: 'bold'
                                },
                                color: '#FFFFFF'
                            }
                        }
                    },
                });

            }
        });
    }
    
    function getDBHIVDoTuoi() {
        $scope.xValuesHiv = [];
        $scope.yValuesHiv = [];

        $.ajax({
            type: 'post',
            url: '/KetQuaHIV/getDBHIVDoTuoi',
            data: {},
            success: function (response) {
                if (response.data != null) {
                    console.log(response);

                    var collection = response.data;

                    var xVals = [];
                    var yVals = [];
                    for (var i in collection) {
                        xVals.push(collection[i]['xValues']);
                        yVals.push(collection[i]['yValues']);
                    }
                    $scope.xValuesHiv = xVals;
                    $scope.yValuesHiv = yVals;

                }
                /*var barColors = ["red", "green", "blue", "orange", "brown"];*/
                var barColors = [
                    "#b91d47",
                    "#00aba9",
                    "#2b5797",
                    "#e8c3b9",
                    "#1e7145"
                ];
                new Chart("hivDoTuoiChart", {
                    plugins: [ChartDataLabels],
                    type: "bar",
                    data: {
                        labels: $scope.xValuesHiv,
                        datasets: [{
                            label: 'Độ tuổi',
                            backgroundColor: barColors,
                            minBarLength: 5,
                            data: $scope.yValuesHiv,
                        }]
                    },
                    options: {
                        responsive: false,
                        maintainAspectRatio: false,
                        layout: {
                            padding: {
                                left: 10,
                                right: 10,
                                top: 20,
                                bottom: 0
                            }
                        },
                        responsive: true,
                        plugins: {
                            legend: {
                                position: 'top',
                            },
                            title: {
                                display: false,
                                text: 'Theo Độ tuổi'
                            }
                        },
                        scales: {
                            x: {
                                
                                //gridLines: {
                                //    display: false,
                                //    drawBorder: false
                                //},
                                ticks: {
                                    maxTicksLimit: 6
                                },
                                maxBarThickness: 25,
                            },
                            y: {
                                ticks: {
                                    min: 0,
                                    max: 30000,
                                    //maxTicksLimit: 5,
                                    //padding: 10,
                                    
                                },
                                //gridLines: {
                                //    color: "rgb(234, 236, 244)",
                                //    zeroLineColor: "rgb(234, 236, 244)",
                                //    drawBorder: false,
                                //    borderDash: [2],
                                //    zeroLineBorderDash: [2]
                                //}
                            },
                        },
                        plugins: {
                            legend: { display: false },
                            datalabels: {
                                anchor: 'end',
                                align: 'top',
                                formatter: Math.round,
                                font: {
                                    weight: 'bold'
                                }
                            }
                        },
                    },
                });

            }
        });
    }

    function getDBHIVTinhTrangSuDungChat() {
        $.ajax({
            type: 'post',
            url: '/KetQuaHIV/getDBHIVTinhTrangSuDungChat',
            data: {},
            success: function (response) {
                if (response.data != null) {
                    console.log(response);

                    var collection = response.data;

                    var xVals = [];
                    var yVals = [];
                    for (var i in collection) {
                        xVals.push(collection[i]['xValues']);
                        yVals.push(collection[i]['yValues']);
                    }
                    $scope.xValuesHivGioiTinh = xVals;
                    $scope.yValuesHivGioiTinh = yVals;

                }
                var barColors = ["red", "green", "black"];
                new Chart("hivGioiTinhChart", {
                    plugins: [ChartDataLabels],
                    type: "bar",
                    data: {
                        labels: $scope.xValuesHivGioiTinh,
                        datasets: [{
                            label: 'Hiv',
                            backgroundColor: barColors,
                            minBarLength: 10,
                            data: $scope.yValuesHivGioiTinh
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        layout: {
                            padding: {
                                left: 10,
                                right: 10,
                                top: 20,
                                bottom: 0
                            }
                        },
                        scales: {
                            x: {
                                ticks: {
                                    maxTicksLimit: 6
                                },
                                maxBarThickness: 25,
                            },
                            y: {
                                ticks: {
                                    min: 0,
                                    max: 30000,
                                    beginAtZero: true
                                }
                            }
                        },
                        plugins: {
                            legend: { display: false },
                            title: {
                                display: false,
                                text: "Thông tin HIV theo giới tính"
                            },
                            datalabels: {
                                anchor: 'end',
                                align: 'top',
                                formatter: Math.round,
                                font: {
                                    weight: 'bold'
                                }
                            }
                        },
                    }
                });

                $scope.$apply();

            }
        });
    }
});
