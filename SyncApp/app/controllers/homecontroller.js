(function (app) {
    app.controller('homecontroller', HomeController);
    HomeController.$inject = ['$scope', '$filter', 'apiservices', '$rootScope', '$window', '$timeout', '$rootScope'];
    function HomeController($scope, $filter, apiservices, $rootScope, $window, $timeout, $rootScope) {
        $scope.listProcess = [];
        $scope.process = {};
        $scope.interval4Log = null;
        $scope.taskName = '';
        $scope.danhSachHoSo = [];

        angular.element(document).ready(function () {
            setInterval(() => {
                var date = new Date();
                var hours = date.getHours();
                if (hours == 0) {
                    location.reload();
                }
            }, 60 * 60 * 1000);
        });

        function getListProcess() {
            $.ajax({
                url: '/Home/GetListProcess',
                type: "GET",
                dataType: "json",
                success: function (response) {
                    if (response.code == "200") {
                        $scope.listProcess = response.data
                        $scope.$apply()
                    } else {
                        toastr.error(response.message)
                    }
                },
                error: function (xhr) {

                    console.log('error');
                }
            });
        }

        getListProcess()

        function getLogJob() {
            $scope.interval4Log = setTimeout(function () {
                getLog();
            }, 5000);

        }

        function getLog() {
            $.ajax({
                type: "Post",
                url: "/Home/ShowConfig",
                data: {
                    name: $scope.process.Code
                },
                success: function (rs) {
                    $scope.logFile = rs.message;
                    $scope.$apply();

                    getLogJob();
                }
            });
        }

        $scope.selectProcess = function (process) {
            $scope.process = { ...process }
            getLog();
        }

        $scope.updateProcess = function () {
            $.ajax({
                url: '/Home/UpdateProcess',
                type: "POST",
                data: $scope.process,
                dataType: "json",
                success: function (response) {
                    console.log(response);
                    if (response.code == "200") {
                        $scope.listProcess = response.data
                        $scope.$apply()
                        toastr.success("Thành công")
                    } else {
                        toastr.error(response.message)
                    }
                },
                error: function (xhr) {

                    console.log('error');
                }
            });
        }

       

        $scope.onchangeDataType = function (value) {
            $scope.taskName = value;
        }

    }
})(angular.module('e-app'));
