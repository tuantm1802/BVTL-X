document.addEventListener('alpine:init', function () {
    Alpine.data('alpineChatGayNghien3TH', function () {
        return {
            items: [],
            totalItems: 0,
            keyword: '',
            isLoading: false,
            currentPage: 1,
            pageSize: 20,

            init: function () {
                this.loadData();
            },

            loadData: function () {
                var self = this;
                self.isLoading = true;

                $.ajax({
                    type: 'POST',
                    url: '/ChatGayNghien3TH/GetAll',
                    data: {
                        KeyWord: self.keyword || '',
                        currentPage: self.currentPage,
                        pageSize: self.pageSize
                    },
                    success: function (response) {
                        if (response && !response.Error) {
                            self.items = response.data || [];
                            self.totalItems = response.totalItems || self.items.length;
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

            search: function () {
                this.currentPage = 1;
                this.loadData();
            },

            resetSearch: function () {
                this.keyword = '';
                this.currentPage = 1;
                this.loadData();
            }
        };
    });
});
