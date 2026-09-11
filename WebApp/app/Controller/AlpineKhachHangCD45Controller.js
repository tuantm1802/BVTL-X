(function () {

    var DICT_HOSPITAL = {
        "1": "Hà Nội - Bệnh viện Lão khoa",
        "2": "Hưng Yên - BV SKTT Thái Bình",
        "3": "Hưng Yên - PK Meheal",
        "4": "Hà Nội - Phòng khám Dr Phi",
        "5": "Ninh Bình - BV SKTT Ninh Bình",
        "6": "Bệnh viện tâm thần Nghệ An",
        "7": "Bệnh viện SKTT Hải Phòng",
        "8": "Bệnh viện tâm thần TP.HCM",
        "9": "Bệnh viện Thủ Đức"
    };

    var DICT_DOCTOR = {
        "1": "Phạm Thị Phương",
        "2": "Nguyễn Thị Hòa",
        "3": "Đặng Thu Thảo",
        "4": "Nguyễn Văn Phi",
        "5": "Đỗ Thị Hằng",
        "6": "Phạm Đặng Duy",
        "7": "Bùi Thị Hè",
        "8": "Phạm Thị Anh",
        "9": "Nguyễn Thị Lan Anh",
        "10": "Hồ Thu Nga",
        "11": "Phạm Thị Huyền",
        "12": "Đặng Tiến Thành",
        "13": "Đỗ Phương Linh",
        "14": "Nguyễn Văn Cảnh",
        "15": "Lê Minh Ngọc",
        "16": "Phạm Thị Mừng",
        "17": "Diệp Lê Tuấn",
        "18": "Nguyễn Phi Yến",
        "19": "Trương Đặng Anh Vân",
        "20": "Nguyễn Trọng Hiến",
        "21": "Nguyễn Thu Hà",
        "22": "Bùi Xuân Đạt",
        "23": "Nguyễn Thị Hà Trang"
    };

    var DICT_SYMPTOM = {
        "1": "Lo âu",
        "2": "Trầm cảm",
        "3": "Mất ngủ",
        "4": "Rối loạn hành vi",
        "5": "Loạn thần",
        "6": "Ý định/hành vi tự sát",
        "7": "Rối loạn stress sau sang chấn",
        "8": "Khác"
    };

    var DICT_TREATMENT_F6 = {
        "1": "Ngoại trú",
        "2": "Nội trú",
        "3": "Tư vấn tâm lý",
        "4": "Khác"
    };

    var DICT_TREATMENT_F5 = {
        "1": "Điều trị ngoại trú",
        "2": "Điều trị nội trú",
        "3": "Tư vấn tâm lý",
        "4": "Không có chỉ định điều trị",
        "5": "Khác"
    };

    var DICT_LOCATION = {
        "1": "Văn phòng nhóm",
        "2": "Ngoài cộng đồng",
        "3": "Khác"
    };

    var DICT_MENTAL_F7 = {
        "1": "Lo âu",
        "2": "Căng thẳng",
        "3": "Trầm cảm",
        "4": "Mất ngủ",
        "5": "Khác"
    };

    var DICT_MENTAL_F8 = {
        "1": "Ổn định",
        "2": "Căng thẳng",
        "3": "Lo âu",
        "4": "Buồn chán",
        "5": "Khác"
    };

    var DICT_DIAGNOSE = {
        "1": "F00- Sa sút trí tuệ (bệnh Alzheimer, sa sút trí tuệ do mạch máu, sa sút trí tuệ trong các bệnh lý khác)",
        "2": "F10- Rối loạn tâm thần và hành vi do sử dụng rượu",
        "3": "F10.0- Rối loạn tâm thần và hành vi do sử dụng rượu (Nhiễm độc cấp)",
        "4": "F10.1- Rối loạn tâm thần và hành vi do sử dụng rượu (Sử dụng gây hại)",
        "5": "F10.2- Rối loạn tâm thần và hành vi do sử dụng rượu (Hội chứng nghiện)",
        "6": "F10.3- Rối loạn tâm thần và hành vi do sử dụng rượu (Trạng thái cai)",
        "7": "F10.4- Rối loạn tâm thần và hành vi do sử dụng rượu (Trạng thái cai với mê sảng)",
        "8": "F10.5- Rối loạn tâm thần và hành vi do sử dụng rượu (Rối loạn tâm thần)",
        "9": "F10.6- Rối loạn tâm thần và hành vi do sử dụng rượu (Hội chứng quên)",
        "10": "F10.7- Rối loạn tâm thần và hành vi do sử dụng rượu (Rối loạn loạn thần di chứng và khởi phát muộn)",
        "11": "F10.8- Rối loạn tâm thần và hành vi do sử dụng rượu (Rối loạn tâm thần và hành vi khác)",
        "12": "F10.9- Rối loạn tâm thần và hành vi do sử dụng rượu (Rối loạn tâm thần và hành vi không biệt định)",
        "13": "F11- Rối loạn tâm thần và hành vi do sử dụng các dạng thuốc phiện",
        "14": "F11.0- Rối loạn tâm thần và hành vi do sử dụng các dạng thuốc phiện (Nhiễm độc cấp)",
        "15": "F11.1- Rối loạn tâm thần và hành vi do sử dụng các dạng thuốc phiện (Sử dụng gây hại)",
        "16": "F11.2- Rối loạn tâm thần và hành vi do sử dụng các dạng thuốc phiện (Hội chứng nghiện)",
        "17": "F11.3- Rối loạn tâm thần và hành vi do sử dụng các dạng thuốc phiện (Trạng thái cai)",
        "18": "F11.4- Rối loạn tâm thần và hành vi do sử dụng các dạng thuốc phiện (Trạng thái cai với mê sảng)",
        "19": "F11.5- Rối loạn tâm thần và hành vi do sử dụng các dạng thuốc phiện (Rối loạn tâm thần)",
        "20": "F11.6- Rối loạn tâm thần và hành vi do sử dụng các dạng thuốc phiện (Hội chứng quên)",
        "21": "F11.7- Rối loạn tâm thần và hành vi do sử dụng các dạng thuốc phiện (Rối loạn loạn thần di chứng và khởi phát muộn)",
        "22": "F11.8- Rối loạn tâm thần và hành vi do sử dụng các dạng thuốc phiện (Rối loạn tâm thần và hành vi khác)",
        "23": "F11.9- Rối loạn tâm thần và hành vi do sử dụng các dạng thuốc phiện (Rối loạn tâm thần và hành vi không biệt định)",
        "24": "F12- Rối loạn tâm thần và hành vi do sử dụng cần sa",
        "25": "F12.0- Rối loạn tâm thần và hành vi do sử dụng cần sa (Nhiễm độc cấp)",
        "26": "F12.1- Rối loạn tâm thần và hành vi do sử dụng cần sa (Sử dụng gây hại)",
        "27": "F12.2- Rối loạn tâm thần và hành vi do sử dụng cần sa (Hội chứng nghiện)",
        "28": "F12.3- Rối loạn tâm thần và hành vi do sử dụng cần sa (Trạng thái cai)",
        "29": "F12.4- Rối loạn tâm thần và hành vi do sử dụng cần sa (Trạng thái cai với mê sảng)",
        "30": "F12.5- Rối loạn tâm thần và hành vi do sử dụng cần sa (Rối loạn tâm thần)",
        "31": "F12.6- Rối loạn tâm thần và hành vi do sử dụng cần sa (Hội chứng quên)",
        "32": "F12.7- Rối loạn tâm thần và hành vi do sử dụng cần sa (Rối loạn loạn thần di chứng và khởi phát muộn)",
        "33": "F12.8- Rối loạn tâm thần và hành vi do sử dụng cần sa (Rối loạn tâm thần và hành vi khác)",
        "34": "F12.9- Rối loạn tâm thần và hành vi do sử dụng cần sa (Rối loạn tâm thần và hành vi không biệt định)",
        "35": "F13- Rối loạn tâm thần và hành vi do sử dụng các chất an dịu hoặc các thuốc ngủ",
        "36": "F13.0- Rối loạn tâm thần và hành vi do sử dụng các chất an thần hoặc các thuốc ngủ (Nhiễm độc cấp)",
        "37": "F13.1- Rối loạn tâm thần và hành vi do sử dụng các chất an thần hoặc các thuốc ngủ (Sử dụng gây hại)",
        "38": "F13.2- Rối loạn tâm thần và hành vi do sử dụng các chất an thần hoặc các thuốc ngủ (Hội chứng nghiện)",
        "39": "F13.3- Rối loạn tâm thần và hành vi do sử dụng các chất an thần hoặc các thuốc ngủ (Trạng thái cai)",
        "40": "F13.4- Rối loạn tâm thần và hành vi do sử dụng các chất an thần hoặc các thuốc ngủ (Trạng thái cai với mê sảng)",
        "41": "F13.5- Rối loạn tâm thần và hành vi do sử dụng các chất an thần hoặc các thuốc ngủ (Rối loạn tâm thần)",
        "42": "F13.6- Rối loạn tâm thần và hành vi do sử dụng các chất an thần hoặc các thuốc ngủ (Hội chứng quên)",
        "43": "F13.7- Rối loạn tâm thần và hành vi do sử dụng các chất an thần hoặc các thuốc ngủ (Rối loạn loạn thần di chứng và khởi phát muộn)",
        "44": "F13.8- Rối loạn tâm thần và hành vi do sử dụng các chất an thần hoặc các thuốc ngủ (Rối loạn tâm thần và hành vi khác)",
        "45": "F13.9- Rối loạn tâm thần và hành vi do sử dụng các chất an thần hoặc các thuốc ngủ (Rối loạn tâm thần và hành vi không biệt định)",
        "46": "F14- Rối loạn tâm thần và hành vi do sử dụng cocain",
        "47": "F14.0- Rối loạn tâm thần và hành vi do sử dụng cocain (Nhiễm độc cấp)",
        "48": "F14.1- Rối loạn tâm thần và hành vi do sử dụng cocain (Sử dụng gây hại)",
        "49": "F14.2- Rối loạn tâm thần và hành vi do sử dụng cocain (Hội chứng nghiện)",
        "50": "F14.3- Rối loạn tâm thần và hành vi do sử dụng cocain (Trạng thái cai)",
        "51": "F14.4- Rối loạn tâm thần và hành vi do sử dụng cocain (Trạng thái cai với mê sảng)",
        "52": "F14.5- Rối loạn tâm thần và hành vi do sử dụng cocain (Rối loạn tâm thần)",
        "53": "F14.6- Rối loạn tâm thần và hành vi do sử dụng cocain (Hội chứng quên)",
        "54": "F14.7- Rối loạn tâm thần và hành vi do sử dụng cocain (Rối loạn loạn thần di chứng và khởi phát muộn)",
        "55": "F14.8- Rối loạn tâm thần và hành vi do sử dụng cocain (Rối loạn tâm thần và hành vi khác)",
        "56": "F14.9- Rối loạn tâm thần và hành vi do sử dụng cocain (Rối loạn tâm thần và hành vi không biệt định)",
        "57": "F15- Rối loạn tâm thần và hành vi do sử dụng chất kích thích khác, bao gồm cả caffein",
        "58": "F15.0- Rối loạn tâm thần và hành vi do sử dụng chất kích thích khác, bao gồm cả caffein (Nhiễm độc cấp)",
        "59": "F15.1- Rối loạn tâm thần và hành vi do sử dụng chất kích thích khác, bao gồm cả caffein (Sử dụng gây hại)",
        "60": "F15.2- Rối loạn tâm thần và hành vi do sử dụng chất kích thích khác, bao gồm cả caffein (Hội chứng nghiện)",
        "61": "F15.3- Rối loạn tâm thần và hành vi do sử dụng chất kích thích khác, bao gồm cả caffein (Trạng thái cai)",
        "62": "F15.4- Rối loạn tâm thần và hành vi do sử dụng chất kích thích khác, bao gồm cả caffein (Trạng thái cai với mê sảng)",
        "63": "F15.5- Rối loạn tâm thần và hành vi do sử dụng chất kích thích khác, bao gồm cả caffein (Rối loạn tâm thần)",
        "64": "F15.6- Rối loạn tâm thần và hành vi do sử dụng chất kích thích khác, bao gồm cả caffein (Hội chứng quên)",
        "65": "F15.7- Rối loạn tâm thần và hành vi do sử dụng chất kích thích khác, bao gồm cả caffein (Rối loạn loạn thần di chứng và khởi phát muộn)",
        "66": "F15.8- Rối loạn tâm thần và hành vi do sử dụng chất kích thích khác, bao gồm cả caffein (Rối loạn tâm thần và hành vi khác)",
        "67": "F15.9- Rối loạn tâm thần và hành vi do sử dụng chất kích thích khác, bao gồm cả caffein (Rối loạn tâm thần và hành vi không biệt định)",
        "68": "F16- Rối loạn tâm thần và hành vi do sử dụng các chất gây ảo giác",
        "69": "F16.0- Rối loạn tâm thần và hành vi do sử dụng các chất gây ảo giác (Nhiễm độc cấp)",
        "70": "F16.1- Rối loạn tâm thần và hành vi do sử dụng các chất gây ảo giác (Sử dụng gây hại)",
        "71": "F16.2- Rối loạn tâm thần và hành vi do sử dụng các chất gây ảo giác (Hội chứng nghiện)",
        "72": "F16.3- Rối loạn tâm thần và hành vi do sử dụng các chất gây ảo giác (Trạng thái cai)",
        "73": "F16.4- Rối loạn tâm thần và hành vi do sử dụng các chất gây ảo giác (Trạng thái cai với mê sảng)",
        "74": "F16.5- Rối loạn tâm thần và hành vi do sử dụng các chất gây ảo giác (Rối loạn tâm thần)",
        "75": "F16.6- Rối loạn tâm thần và hành vi do sử dụng các chất gây ảo giác (Hội chứng quên)",
        "76": "F16.7- Rối loạn tâm thần và hành vi do sử dụng các chất gây ảo giác (Rối loạn loạn thần di chứng và khởi phát muộn)",
        "77": "F16.8- Rối loạn tâm thần và hành vi do sử dụng các chất gây ảo giác (Rối loạn tâm thần và hành vi khác)",
        "78": "F16.9- Rối loạn tâm thần và hành vi do sử dụng các chất gây ảo giác (Rối loạn tâm thần và hành vi không biệt định)",
        "79": "F18- Rối loạn tâm thần và hành vi do sử dụng dung môi dễ bay hơi",
        "80": "F18.0- Rối loạn tâm thần và hành vi do sử dụng dung môi dễ bay hơi (Nhiễm độc cấp)",
        "81": "F18.1- Rối loạn tâm thần và hành vi do sử dụng dung môi dễ bay hơi (Sử dụng gây hại)",
        "82": "F18.2- Rối loạn tâm thần và hành vi do sử dụng dung môi dễ bay hơi (Hội chứng nghiện)",
        "83": "F18.3- Rối loạn tâm thần và hành vi do sử dụng dung môi dễ bay hơi (Trạng thái cai)",
        "84": "F18.4- Rối loạn tâm thần và hành vi do sử dụng dung môi dễ bay hơi (Trạng thái cai với mê sảng)",
        "85": "F18.5- Rối loạn tâm thần và hành vi do sử dụng dung môi dễ bay hơi (Rối loạn tâm thần)",
        "86": "F18.6- Rối loạn tâm thần và hành vi do sử dụng dung môi dễ bay hơi (Hội chứng quên)",
        "87": "F18.7- Rối loạn tâm thần và hành vi do sử dụng dung môi dễ bay hơi (Rối loạn loạn thần di chứng và khởi phát muộn)",
        "88": "F18.8- Rối loạn tâm thần và hành vi do sử dụng dung môi dễ bay hơi (Rối loạn tâm thần và hành vi khác)",
        "89": "F18.9- Rối loạn tâm thần và hành vi do sử dụng dung môi dễ bay hơi (Rối loạn tâm thần và hành vi không biệt định)",
        "90": "F19- Rối loạn tâm thần và hành vi do sử dụng nhiều loại ma túy và chất tác động tâm thần khác",
        "91": "F19.0- Rối loạn tâm thần và hành vi do sử dụng nhiều loại ma túy và chất tác động tâm thần khác (Nhiễm độc cấp)",
        "92": "F19.1- Rối loạn tâm thần và hành vi do sử dụng nhiều loại ma túy và chất tác động tâm thần khác (Sử dụng gây hại)",
        "93": "F19.2- Rối loạn tâm thần và hành vi do sử dụng nhiều loại ma túy và chất tác động tâm thần khác (Hội chứng nghiện)",
        "94": "F19.3- Rối loạn tâm thần và hành vi do sử dụng nhiều loại ma túy và chất tác động tâm thần khác (Trạng thái cai)",
        "95": "F19.4- Rối loạn tâm thần và hành vi do sử dụng nhiều loại ma túy và chất tác động tâm thần khác (Trạng thái cai với mê sảng)",
        "96": "F19.5- Rối loạn tâm thần và hành vi do sử dụng nhiều loại ma túy và chất tác động tâm thần khác (Rối loạn tâm thần)",
        "97": "F19.6- Rối loạn tâm thần và hành vi do sử dụng nhiều loại ma túy và chất tác động tâm thần khác (Hội chứng quên)",
        "98": "F19.7- Rối loạn tâm thần và hành vi do sử dụng nhiều loại ma túy và chất tác động tâm thần khác (Rối loạn loạn thần di chứng và khởi phát muộn)",
        "99": "F19.8- Rối loạn tâm thần và hành vi do sử dụng nhiều loại ma túy và chất tác động tâm thần khác (Rối loạn tâm thần và hành vi khác)",
        "100": "F19.9- Rối loạn tâm thần và hành vi do sử dụng nhiều loại ma túy và chất tác động tâm thần khác (Rối loạn tâm thần và hành vi không biệt định)",
        "101": "F20- Tâm thần phân liệt",
        "102": "F21- Rối loạn loại phân liệt",
        "103": "F22- Rối loạn hoang tưởng dai dẳng",
        "104": "F23- Rối loạn loạn thần cấp và nhất thời",
        "105": "F24- Rối loạn hoang tưởng cảm ứng",
        "106": "F25- Rối loạn phân liệt cảm xúc",
        "107": "F29- Loạn thần không thực tổn không biệt định",
        "108": "F30- Giai đoạn hưng cảm",
        "109": "F30.0- Hưng cảm nhẹ",
        "110": "F30.1- Hưng cảm không có các triệu chứng loạn thần",
        "111": "F30.2- Hưng cảm với các triệu chứng loạn thần",
        "112": "F31- Rối loạn cảm xúc lưỡng cực",
        "113": "F32- Giai đoạn trầm cảm",
        "114": "F33- Rối loạn trầm cảm tái diễn",
        "115": "F34- Rối loạn khí sắc [cảm xúc] dai dẳng",
        "116": "F38- Rối loạn khí sắc [cảm xúc] khác",
        "117": "F39- Rối loạn khí sắc (cảm xúc) biệt định",
        "118": "F40- Rối loạn lo âu ám ảnh sợ hãi",
        "119": "F41- Các rối loạn lo âu khác",
        "120": "F42- Rối loạn ám ảnh nghi thức",
        "121": "F43- Phản ứng với stress trầm trọng và rối loạn sự thích ứng",
        "122": "F43.1- Rối loạn stress sau sang chấn",
        "123": "F44- Các rối loạn phân ly [chuyển di]",
        "124": "F45- Rối loạn dạng cơ thể",
        "125": "F50- Các rối loạn ăn uống",
        "126": "F51- Rối loạn giấc ngủ không thực tổn",
        "127": "F52- Loạn chức năng tình dục, không do rối loạn hoặc bệnh thực tổn",
        "128": "F60- Rối loạn nhân cách đặc hiệu",
        "129": "F70- Chậm phát triển tâm thần nhẹ",
        "130": "F90- Các rối loạn tăng động",
        "131": "F41.2- Rối loạn hỗn hợp lo âu và trầm cảm",
        "132": "Khác",
    };
    function registerController() {
        if (typeof Alpine === 'undefined') return;

        Alpine.data('alpineKhachHangCD45', function () {
            return {
                items: [],
                totalRecords: 0,
                isLoading: false,
                isDetailLoading: false,
                listCities: [],
                listNhoms: [],
                filteredNhoms: [],
                selectedCustomer: null,

                filter: {
                    Keyword: '',
                    CityCode: '',
                    MaNhom: '',
                    DoiTuong: '',
                    CoBHYT: '',
                    CoCCCD: '',
                    FromDate: '',
                    ToDate: '',
                    PageIndex: 1,
                    PageSize: 25
                },

                init: function () {
                    var self = this;
                    self.loadDanhMuc();
                },

                get totalPages() {
                    var self = this;
                    if (!self.totalRecords || self.totalRecords <= 0) return 1;
                    return Math.ceil(self.totalRecords / self.filter.PageSize);
                },

                get visiblePages() {
                    var self = this;
                    var total = self.totalPages;
                    var current = self.filter.PageIndex;
                    var pages = [];
                    var start = Math.max(1, current - 2);
                    var end = Math.min(total, current + 2);
                    for (var i = start; i <= end; i++) {
                        pages.push(i);
                    }
                    return pages;
                },

                loadDanhMuc: function () {
                    var self = this;
                    $.ajax({
                        type: 'POST',
                        url: '/KhachHangCD45/GetFilterData',
                        success: function (res) {
                            if (res.Success) {
                                self.listCities = res.Cities || [];
                                self.listNhoms = res.Nhoms || [];
                                self.filteredNhoms = self.listNhoms;
                            }
                            self.searchCustomers(1);
                        },
                        error: function () {
                            self.searchCustomers(1);
                        }
                    });
                },

                onCityChange: function () {
                    var self = this;
                    if (!self.filter.CityCode) {
                        self.filteredNhoms = self.listNhoms;
                    } else {
                        self.filteredNhoms = self.listNhoms.filter(function (x) {
                            return x.CityCode === self.filter.CityCode;
                        });
                    }
                    self.filter.MaNhom = '';
                    self.searchCustomers(1);
                },

                resetFilters: function () {
                    var self = this;
                    self.filter.Keyword = '';
                    self.filter.CityCode = '';
                    self.filter.MaNhom = '';
                    self.filter.DoiTuong = '';
                    self.filter.CoBHYT = '';
                    self.filter.CoCCCD = '';
                    self.filter.FromDate = '';
                    self.filter.ToDate = '';
                    self.filter.PageIndex = 1;
                    self.filteredNhoms = self.listNhoms;
                    self.searchCustomers(1);
                },

                searchCustomers: function (pageIndex) {
                    var self = this;
                    if (pageIndex) self.filter.PageIndex = pageIndex;
                    self.isLoading = true;

                    var reqData = {
                        Keyword: self.filter.Keyword || '',
                        CityCode: self.filter.CityCode || '',
                        MaNhom: self.filter.MaNhom || '',
                        DoiTuong: self.filter.DoiTuong ? parseInt(self.filter.DoiTuong) : null,
                        CoBHYT: self.filter.CoBHYT === '' ? null : (self.filter.CoBHYT === 'true'),
                        CoCCCD: self.filter.CoCCCD === '' ? null : (self.filter.CoCCCD === 'true'),
                        FromDate: self.filter.FromDate || '',
                        ToDate: self.filter.ToDate || '',
                        PageIndex: self.filter.PageIndex,
                        PageSize: parseInt(self.filter.PageSize) || 25
                    };

                    $.ajax({
                        type: 'POST',
                        url: '/KhachHangCD45/SearchCustomers',
                        data: reqData,
                        success: function (res) {
                            self.isLoading = false;
                            if (res.Success) {
                                self.items = res.Data || [];
                                self.totalRecords = res.Total || 0;
                            } else {
                                if (window.toastr) toastr.error(res.Message);
                            }
                        },
                        error: function (xhr, status, error) {
                            self.isLoading = false;
                            console.error('Loi khi tim kiem khach hang:', error);
                            if (window.toastr) toastr.error('Có lỗi xảy ra khi tải danh sách khách hàng!');
                        }
                    });
                },

                activeDetailTab: 'tab-thongtin',

                switchDetailTab: function (tabId) {
                    this.activeDetailTab = tabId;
                },

                copyRecordId: function (recordId) {
                    if (!recordId) return;
                    if (navigator.clipboard && window.isSecureContext) {
                        navigator.clipboard.writeText(recordId).then(function () {
                            if (window.toastr) toastr.success('Đã sao chép mã hồ sơ: ' + recordId);
                        });
                    } else {
                        var textArea = document.createElement("textarea");
                        textArea.value = recordId;
                        document.body.appendChild(textArea);
                        textArea.select();
                        try {
                            document.execCommand('copy');
                            if (window.toastr) toastr.success('Đã sao chép mã hồ sơ: ' + recordId);
                        } catch (err) {
                            if (window.toastr) toastr.info('Mã hồ sơ: ' + recordId);
                        }
                        document.body.removeChild(textArea);
                    }
                },

                openDetailModal: function (recordId) {
                    var self = this;
                    if (!recordId) return;

                    self.selectedCustomer = null;
                    self.isDetailLoading = true;
                    self.activeDetailTab = 'tab-thongtin';

                    var modalEl = document.getElementById('modalCustomerDetail');
                    if (window.bootstrap && bootstrap.Modal) {
                        var modal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
                        modal.show();
                    } else if (window.jQuery) {
                        window.jQuery(modalEl).modal('show');
                    }

                    // Kích hoạt lại tab đầu tiên nếu có
                    var firstTabBtn = document.querySelector('#modalCustomerDetail button[data-bs-target="#tab-thongtin"]');
                    if (firstTabBtn && window.bootstrap && bootstrap.Tab) {
                        var tabInstance = bootstrap.Tab.getInstance(firstTabBtn) || new bootstrap.Tab(firstTabBtn);
                        tabInstance.show();
                    }

                    $.ajax({
                        type: 'GET',
                        url: '/KhachHangCD45/GetCustomerDetail?recordId=' + encodeURIComponent(recordId),
                        success: function (res) {
                            self.isDetailLoading = false;
                            if (res.Success) {
                                self.selectedCustomer = res.Data;
                            } else {
                                if (window.toastr) toastr.error(res.Message);
                            }
                        },
                        error: function (xhr, status, error) {
                            self.isDetailLoading = false;
                            console.error('Loi khi lay chi tiet:', error);
                            if (window.toastr) toastr.error('Có lỗi khi tải thông tin hồ sơ!');
                        }
                    });
                },

                exportExcel: function () {
                    var self = this;
                    var p = self.filter;
                    var url = '/KhachHangCD45/ExportExcel?keyword=' + encodeURIComponent(p.Keyword || '') +
                        '&cityCode=' + encodeURIComponent(p.CityCode || '') +
                        '&maNhom=' + encodeURIComponent(p.MaNhom || '') +
                        '&doiTuong=' + encodeURIComponent(p.DoiTuong || '') +
                        '&coBHYT=' + encodeURIComponent(p.CoBHYT || '') +
                        '&coCCCD=' + encodeURIComponent(p.CoCCCD || '') +
                        '&fromDate=' + encodeURIComponent(p.FromDate || '') +
                        '&toDate=' + encodeURIComponent(p.ToDate || '');
                    window.location.href = url;
                },

                parseDate: function (val) {
                    if (!val) return null;
                    if (typeof val === 'number') return new Date(val);
                    if (typeof val === 'string') {
                        var matches = val.match(/\d+/);
                        if (matches && val.indexOf('/Date(') !== -1) {
                            return new Date(parseInt(matches[0], 10));
                        }
                        var d = new Date(val);
                        if (!isNaN(d.getTime())) return d;
                    }
                    return null;
                },

                formatDate: function (val) {
                    if (!val) return '-';
                    var d = this.parseDate(val);
                    if (!d || isNaN(d.getTime())) return '-';
                    var day = ('0' + d.getDate()).slice(-2);
                    var month = ('0' + (d.getMonth() + 1)).slice(-2);
                    return day + '/' + month + '/' + d.getFullYear();
                },

                formatDateTime: function (val) {
                    if (!val) return '-';
                    var d = this.parseDate(val);
                    if (!d || isNaN(d.getTime())) return '-';
                    var day = ('0' + d.getDate()).slice(-2);
                    var month = ('0' + (d.getMonth() + 1)).slice(-2);
                    var hour = ('0' + d.getHours()).slice(-2);
                    var min = ('0' + d.getMinutes()).slice(-2);
                    return day + '/' + month + '/' + d.getFullYear() + ' ' + hour + ':' + min;
                },

                getBadgeClassDoiTuong: function (doiTuong) {
                    switch (parseInt(doiTuong)) {
                        case 1: return 'badge bg-success-subtle text-success border border-success'; // PUD
                        case 2: return 'badge bg-danger-subtle text-danger border border-danger';   // PLHIV
                        case 3: return 'badge bg-info-subtle text-info border border-info';          // TG
                        case 4: return 'badge bg-primary-subtle text-primary border border-primary'; // MSM
                        case 5: return 'badge bg-warning-subtle text-warning-emphasis border border-warning'; // SW
                        default: return 'badge bg-secondary-subtle text-secondary border';
                    }
                },

                getBadgeClassQst: function (muc) {
                    switch (parseInt(muc)) {
                        case 1: return 'badge bg-danger-subtle text-danger border border-danger'; // Nguy cơ rất cao
                        case 2: return 'badge bg-warning-subtle text-warning-emphasis border border-warning'; // Nguy cơ cao
                        case 3: return 'badge bg-info-subtle text-info border border-info'; // Nguy cơ trung bình
                        case 4: return 'badge bg-success-subtle text-success border border-success'; // Nguy cơ thấp
                        default: return 'badge bg-secondary-subtle text-secondary border';
                    }
                },

                formatKetQuaHivText: function (val) {
                    if (val === 1 || val === '1') return 'Âm tính';
                    if (val === 2 || val === '2') return 'Dương tính';
                    if (val === 3 || val === '3') return 'Không rõ / Chờ KQ';
                    return '-';
                },

                getBadgeClassHiv: function (val) {
                    if (val === 1 || val === '1') return 'badge bg-success-subtle text-success border border-success';
                    if (val === 2 || val === '2') return 'badge bg-danger-subtle text-danger border border-danger fw-bold';
                    if (val === 3 || val === '3') return 'badge bg-warning-subtle text-warning-emphasis border border-warning';
                    return 'badge bg-secondary-subtle text-secondary border';
                },

                formatHospital: function (val) {
                    if (!val) return '-';
                    var k = String(val).trim();
                    return DICT_HOSPITAL[k] || val;
                },

                formatDoctor: function (val) {
                    if (!val) return '-';
                    var k = String(val).trim();
                    return DICT_DOCTOR[k] || val;
                },

                formatSymptomF6: function (val) {
                    if (!val) return '-';
                    return String(val).split(',').map(function (s) {
                        var k = s.trim();
                        return DICT_SYMPTOM[k] || k;
                    }).join(', ');
                },

                formatDiagnoseF6: function (val) {
                    if (!val) return '-';
                    var k = String(val).trim();
                    return DICT_DIAGNOSE[k] || val;
                },

                formatTreatmentF6: function (val) {
                    if (!val) return '-';
                    return String(val).split(',').map(function (s) {
                        var k = s.trim();
                        return DICT_TREATMENT_F6[k] || k;
                    }).join(', ');
                },

                formatTreatmentF5: function (val) {
                    if (!val) return '-';
                    return String(val).split(',').map(function (s) {
                        var k = s.trim();
                        return DICT_TREATMENT_F5[k] || k;
                    }).join(', ');
                },

                formatAdherenceF5: function (val) {
                    if (!val) return '-';
                    var k = String(val).trim();
                    if (k === '1') return 'Tuân thủ tốt';
                    if (k === '2') return 'Không tuân thủ';
                    return val;
                },

                formatLocation: function (val) {
                    if (!val) return '-';
                    var k = String(val).trim();
                    return DICT_LOCATION[k] || val;
                },

                formatMentalStatusF7: function (val) {
                    if (!val) return '-';
                    return String(val).split(',').map(function (s) {
                        var k = s.trim();
                        return DICT_MENTAL_F7[k] || k;
                    }).join(', ');
                },

                formatMentalStatusF8: function (val) {
                    if (!val) return '-';
                    return String(val).split(',').map(function (s) {
                        var k = s.trim();
                        return DICT_MENTAL_F8[k] || k;
                    }).join(', ');
                },

                formatTuVanContent: function (tv) {
                    if (!tv) return '-';
                    if (tv.LanTuVan === 1) {
                        var ms = this.formatMentalStatusF7(tv.TINH_TRANG_SKTT);
                        return ms || tv.NHU_CAU_HO_TRO || '-';
                    } else {
                        var ms2 = this.formatMentalStatusF8(tv.DANH_GIA_HIEN_TAI);
                        return ms2 || tv.CAN_THIEP_AP_DUNG || '-';
                    }
                },
            };
        });
    }

    if (window.Alpine) {
        registerController();
    } else {
        document.addEventListener('alpine:init', registerController);
    }
})();