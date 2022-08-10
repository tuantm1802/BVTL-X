app.controller("ControlProjectController", function ($scope, $ngConfirm, showToast, hideLoading) {

    angular.element(document).ready(function () {
        
    });

    ////////////////////////// Tree table /////////////////////////////////////////

    // Có hiển thị cột checkbox ở đầu không
    $scope.tree_Table_Checkbox = true;
   // Model data
    $scope.tree_data = [
        {
            "url": "",
            "expanded": false,
            "label": "Reports",
            "last_modified": "2014-09-28T11:19:49.000Z",
            "createed": "2014-09-28T11:19:49.000Z",
            "type": 2,
            "checkbox": true,
            "selected": false,
            "children": [
                {
                    "url": "",
                    "expanded": false,
                    "label": "2014",
                    "last_modified": "2014-09-28T11:19:49.000Z",
                    "createed": "2014-09-28T11:19:49.000Z",
                    "type": 2,
                    "checkbox": true,
                    "selected": false,
                    "children": [
                        {
                            "url": "",
                            "expanded": false,
                            "label": "Passive",
                            "last_modified": "2014-09-28T11:19:49.000Z",
                            "createed": "2014-09-28T11:19:49.000Z",
                            "type": 2,
                            "checkbox": true,
                            "selected": false,
                            "children": [
                                {
                                    "url": "foo.pdf",
                                    "expanded": false,
                                    "label": "E14288-Passive-40085-2014_09_26.pdf",
                                    "last_modified": "2014-09-28T11:19:49.000Z",
                                    "createed": "2014-09-28T11:19:49.000Z",
                                    "type": 1,
                                    "checkbox": true,
                                    "selected": false,
                                    "size": 60929
                                }
                            ],
                            "size": 0
                        }
                    ],
                    "size": 0
                }
            ],
            "size": 0
        },
        {
            "url": "",
            "expanded": false,
            "label": "photos",
            "last_modified": "2014-10-23T10:27:54.000Z",
            "createed": "2014-09-28T11:19:49.000Z",
            "type": 2,
            "checkbox": false,
            "selected": false,
            "children": [
                {
                    "url": "foo.pdf",
                    "expanded": false,
                    "label": "property_front.jpg",
                    "last_modified": "2014-10-23T22:59:14.000Z",
                    "createed": "2014-09-28T11:19:49.000Z",
                    "type": 1,
                    "checkbox": false,
                    "selected": false,
                    "size": 121661
                }
            ],
            "size": 0
        }
    ];

    // cấu hình hiển thị cùng với icon
    $scope.expanding_property = {
        field: "label",
        displayName: "Name",
        width: "55%"
    };
    // Cấu hình các cột sẽ hiển thị tiếp theo
    $scope.col_defs = [

        { field: "size", displayName: "Size New Template", width: "15%" },
        { field: "last_modified", displayName: "Last modified New Template", width: "30%" },
        { field: "createed", displayName: "Created New Template", width: "10%" }
    ];

    // event when change value checkbox
    $scope.ChangeTreeTableCheckbox = function (item) {
        console.log(item);
    };

    // event when click choose row
    $scope.ClickTreeTableRow = function (item) {
        console.log(item);
    };
    ////////////////////////// End Tree table /////////////////////////////////////////

    
});
