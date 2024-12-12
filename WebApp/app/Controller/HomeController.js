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
    let loadCount = 0; // Số hàm cần gọi khi trang load

    function checkLoadingComplete() {
        loadCount--;
        console.log(loadCount);

        if (loadCount === 0) {
            $scope.isLoading = false; // Tắt loading khi tất cả hàm hoàn thành
            $scope.$apply(); // Đảm bảo Angular cập nhật view
        }
    }
    function fetchData(url, data = {}) {
        return new Promise((resolve, reject) => {
            $.ajax({
                type: 'POST',
                url: url,
                data: data,
                success: function (response) {
                    resolve(response);
                },
                error: function (error) {
                    reject(error);
                }
            });
        });
    }

    function initializeData() {
        angular.element(document).ready(function () {
            const apiCode = 'API_ALL_CD43_KHACH_HANG_TTCB';
            $scope.isLoading = true;
            loadCount = 14; 

            // Gọi hàm Promise và đợi từng hàm hoàn tất
            fetchEndTimeSync(apiCode).then(checkLoadingComplete).catch(checkLoadingComplete);
            GetTinh().then(checkLoadingComplete).catch(checkLoadingComplete);
            GetNhomByTinh().then(checkLoadingComplete).catch(checkLoadingComplete);
            GetTanSuatChemsex3ThangTheoDoTuoi().then(checkLoadingComplete).catch(checkLoadingComplete);
            GetTanSuatChemsex3ThangTheoDiemAssist().then(checkLoadingComplete).catch(checkLoadingComplete);
            GetTanSuatChemsex3ThangTheoDiemACE().then(checkLoadingComplete).catch(checkLoadingComplete);
            GetSuDungDaChatTrongChemsexTheoDoTuoi().then(checkLoadingComplete).catch(checkLoadingComplete);
            GetSuDungDaChatTrongChemsexTheoDoiTuongQHTD().then(checkLoadingComplete).catch(checkLoadingComplete);
            GetSuDungDaChatTrongChemsexTheoQHTDTT().then(checkLoadingComplete).catch(checkLoadingComplete);
            GetSuDungDaChatTrongChemsexTheoBanDam().then(checkLoadingComplete).catch(checkLoadingComplete);
            GetSuDungDaChatTrongChemsexTheoDiemAssistMaTuyDa().then(checkLoadingComplete).catch(checkLoadingComplete);
            GetSuDungDaChatTrongChemsexTheoDiemACE().then(checkLoadingComplete).catch(checkLoadingComplete);
            GetSuDungDaChatTrongChemsexTheoDiemQST().then(checkLoadingComplete).catch(checkLoadingComplete);
            GetTanSuatChemsexTrong3ThangTheoDiemQST().then(checkLoadingComplete).catch(checkLoadingComplete);
        });
    }

    function fetchEndTimeSync(apiCode) {
        return fetchData('/SyncData/GetEndTimeSync', { apiCode }).then(response => {
            if (response.success) {
                $('#endTimeSyncDisplay').text('Dữ liệu được đồng bộ lần cuối vào lúc: ' + response.endTimeSync);
            } else {
                $('#endTimeSyncDisplay').text(response.message);
            }
        }).catch(() => {
            $('#endTimeSyncDisplay').text('Đã xảy ra lỗi khi lấy dữ liệu');
        });
    }


    function GetTinh() {
        return fetchData('/BaoCaoCD43/GetBottomAction').then(response => {
            $scope.ListCity = response.Citys;
            $scope.$apply();
        });
    }

    function GetNhomByTinh() {
        return fetchData('/BaoCaoCD43/GetNhomTBHByMaNhomMap', { CityCodes: '' }).then(response => {
            $scope.ListNhomTBH = response.NhomTBHs;
            $scope.$apply();
        });
    }

    // Xử lý khi chọn nhóm
    $scope.selectNhom = function (maNhom, nhomName) {
        $scope.selectedNhom = maNhom;
        $scope.selectedNhomName = nhomName;
        //Promise.resolve(GetTanSuatChemsex3ThangTheoDoTuoi()).finally(checkLoadingComplete);  
        initializeData();
    };

    // Xử lý khi chọn tỉnh
    $scope.selectTinh = function (maTinh, tinhName) {
        $scope.selectedTinh = maTinh;
        $scope.selectedTinhName = tinhName;
        //Promise.resolve(GetTanSuatChemsex3ThangTheoDoTuoi()).finally(checkLoadingComplete);
        initializeData();
    };
    
    function GetTanSuatChemsex3ThangTheoDoTuoi() {
        return fetchData('/Home/GetTanSuatChemsex3ThangTheoDoTuoi', { maNhom: $scope.selectedNhom, maTinh: $scope.selectedTinh }).then(response => {
            $scope.dbTanSuatChemsex3ThangTheoDoTuoi = response.data;
            $scope.$apply();
        });
    }
    function GetTanSuatChemsex3ThangTheoDiemAssist() {
        return fetchData('/Home/GetTanSuatChemsex3ThangTheoDiemAssist', { maNhom: $scope.selectedNhom, maTinh: $scope.selectedTinh }).then(response => {
            $scope.dbTanSuatChemsex3ThangTheoDiemAssist = response.data;
            console.log($scope.dbTanSuatChemsex3ThangTheoDiemAssist);
            $scope.$apply();
        });
    }
    function GetTanSuatChemsex3ThangTheoDiemACE() {
        return fetchData('/Home/GetTanSuatChemsex3ThangTheoDiemACE', { maNhom: $scope.selectedNhom, maTinh: $scope.selectedTinh }).then(response => {
            $scope.dbTanSuatChemsex3ThangTheoDiemACE = response.data;            
            $scope.$apply();
        });
    }    
    function GetSuDungDaChatTrongChemsexTheoDoTuoi() {
        return fetchData('/Home/GetSuDungDaChatTrongChemsexTheoDoTuoi', { maNhom: $scope.selectedNhom, maTinh: $scope.selectedTinh }).then(response => {
            $scope.dbSuDungDaChatTrongChemsexTheoDoTuoi = response.data;
            $scope.$apply();
        });
    }  
    function GetSuDungDaChatTrongChemsexTheoDoiTuongQHTD() {
        return fetchData('/Home/GetSuDungDaChatTrongChemsexTheoDoiTuongQHTD', { maNhom: $scope.selectedNhom, maTinh: $scope.selectedTinh }).then(response => {
            $scope.dbSuDungDaChatTrongChemsexTheoDoiTuongQHTD = response.data;
            $scope.$apply();
        });
    }  
    function GetSuDungDaChatTrongChemsexTheoQHTDTT() {
        return fetchData('/Home/GetSuDungDaChatTrongChemsexTheoQHTDTT', { maNhom: $scope.selectedNhom, maTinh: $scope.selectedTinh }).then(response => {
            $scope.dbSuDungDaChatTrongChemsexTheoQHTDTT = response.data;
            $scope.$apply();
        });
    }  
    function GetSuDungDaChatTrongChemsexTheoBanDam() {
        return fetchData('/Home/GetSuDungDaChatTrongChemsexTheoBanDam', { maNhom: $scope.selectedNhom, maTinh: $scope.selectedTinh }).then(response => {
            $scope.dbSuDungDaChatTrongChemsexTheoBanDam = response.data;
            $scope.$apply();
        });
    }  
    function GetSuDungDaChatTrongChemsexTheoDiemAssistMaTuyDa() {
        return fetchData('/Home/GetSuDungDaChatTrongChemsexTheoDiemAssistMaTuyDa', { maNhom: $scope.selectedNhom, maTinh: $scope.selectedTinh }).then(response => {
            $scope.dbSuDungDaChatTrongChemsexTheoDiemAssistMaTuyDa = response.data;
            $scope.$apply();
        });
    }  
    function GetSuDungDaChatTrongChemsexTheoDiemACE() {
        return fetchData('/Home/GetSuDungDaChatTrongChemsexTheoDiemACE', { maNhom: $scope.selectedNhom, maTinh: $scope.selectedTinh }).then(response => {
            $scope.dbSuDungDaChatTrongChemsexTheoDiemACE = response.data;
            $scope.$apply();
        });
    }  
    function GetSuDungDaChatTrongChemsexTheoDiemQST() {
        return fetchData('/Home/GetSuDungDaChatTrongChemsexTheoDiemQST', { maNhom: $scope.selectedNhom, maTinh: $scope.selectedTinh }).then(response => {
            $scope.dbSuDungDaChatTrongChemsexTheoDiemQST = response.data;
            $scope.$apply();
        });
    }  
    function GetTanSuatChemsexTrong3ThangTheoDiemQST() {
        return fetchData('/Home/GetTanSuatChemsexTrong3ThangTheoDiemQST', { maNhom: $scope.selectedNhom, maTinh: $scope.selectedTinh }).then(response => {
            $scope.dbTanSuatChemsexTrong3ThangTheoDiemQST = response.data;
            $scope.$apply();
        });
    }  

    initializeData();

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
