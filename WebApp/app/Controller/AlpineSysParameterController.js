document.addEventListener('alpine:init', function () {
    Alpine.data('alpineSysParameter', function () {
        return {
            items: [],
            totalItems: 0,
            keyword: '',
            isLoading: false,
            currentPage: 1,
            pageSize: 20,

            // Stats
            stats: {
                total: 0,
                activeCount: 0,
                stringTypeCount: 0,
                numberTypeCount: 0
            },

            init: function () {
                window.alpineSysParameterInstance = this;
                this.loadData();
            },

            loadData: function () {
                var self = this;
                self.isLoading = true;

                $.ajax({
                    type: 'POST',
                    url: '/SysParameter/GetAll',
                    data: {
                        KeyWord: self.keyword || '',
                        currentPage: self.currentPage,
                        pageSize: self.pageSize
                    },
                    success: function (response) {
                        if (response && !response.Error) {
                            self.items = response.data || [];
                            self.totalItems = response.totalItems || self.items.length;
                            self.calculateStats();
                        } else {
                            self.items = [];
                            self.totalItems = 0;
                        }
                    },
                    error: function () {
                        self.items = [];
                        self.totalItems = 0;
                    },
                    complete: function () {
                        self.isLoading = false;
                    }
                });
            },

            calculateStats: function () {
                var self = this;
                self.stats.total = self.totalItems;
                self.stats.activeCount = self.items.filter(function (x) { return x.IsActive === true; }).length;
                self.stats.stringTypeCount = self.items.filter(function (x) { return x.ParamValueType === 'STRING'; }).length;
                self.stats.numberTypeCount = self.items.filter(function (x) { return x.ParamValueType === 'NUMBER'; }).length;
            },

            search: function () {
                this.currentPage = 1;
                this.loadData();
            },

            resetSearch: function () {
                this.keyword = '';
                this.currentPage = 1;
                this.loadData();
            },

            openEditModal: function (itemId) {
                var angularScope = angular.element(document.querySelector('[ng-controller="SysParameterController"]')).scope();
                if (angularScope && typeof angularScope.editDirect === 'function') {
                    angularScope.editDirect(itemId);
                } else if (angularScope && typeof angularScope.edit === 'function') {
                    angularScope.ParamIdSeleted = itemId;
                    angularScope.edit();
                }
            }
        };
    });
});
