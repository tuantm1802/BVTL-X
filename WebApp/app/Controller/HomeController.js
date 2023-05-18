app.controller("HomeController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.xValuesHivGioiTinh = [];
    $scope.yValuesHivGioiTinh = [];
    $scope.hivGioiTinhDB = [];

    angular.element(document).ready(function () {
               
        getDBHIVGioiTinh();
        getDBHIVTinhTrang();
        getDBHIVDoiTuong();
        getDBHIVDoTuoi();

        
       
    });
    
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
