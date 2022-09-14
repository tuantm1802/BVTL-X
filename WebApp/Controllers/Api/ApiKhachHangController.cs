using DataTables;
using Model.ModelExtend;
using System.Configuration;
using System.Web.Http;

namespace WebApp.Controllers.Api
{
    public class ApiKhachHangController : ApiController
    {
        private string stringConnect = ConfigurationManager.AppSettings["ConnectionString"];
        [Route("api/khachhang")]
        [HttpGet]
        [HttpPost]
        public IHttpActionResult khachhang()
        {
            var request = System.Web.HttpContext.Current.Request;

            using (var db = new Database("sqlserver", stringConnect))
            {
                var response = new Editor(db, "BVTL_KHACH_HANG")
                    .Model<CustomerPageModel>()
                    .Field(new Field("makh")
                        .Validator(Validation.NotEmpty())
                    )
                    .Field(new Field("hoten"))
                    .Field(new Field("extn")
                        .Validator(Validation.Numeric())
                    )
                    .Field(new Field("gioitinh")
                        .Validator(Validation.Numeric())
                        .SetFormatter(Format.IfEmpty(null))
                    )
                    .Field(new Field("namsinh")
                        .Validator(Validation.Numeric())
                        .SetFormatter(Format.IfEmpty(null))
                    )
                    .Field(new Field("ngaytiepcan")
                        .Validator(Validation.DateFormat(
                            Format.DATE_ISO_8601,
                            new ValidationOpts { Message = "Please enter a date in the format yyyy-mm-dd" }
                        ))
                        .GetFormatter(Format.DateSqlToFormat(Format.DATE_ISO_8601))
                        .SetFormatter(Format.DateFormatToSql(Format.DATE_ISO_8601))
                    )
                    .Process(request)
                    .Data();

                return (IHttpActionResult)Json(response);
            }
        }
    }
}