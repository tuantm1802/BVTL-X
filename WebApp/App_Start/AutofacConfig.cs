using Autofac;
using Autofac.Integration.Mvc;
using System.Reflection;
using System.Web.Mvc;
using WebApp.Service;

namespace WebApp.App_Start
{
    public class AutofacConfig
    {
        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            // ÄÄƒng kĂ½ táº¥t cáº£ cĂ¡c Controllers
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            // ÄÄƒng kĂ½ Services
            builder.RegisterType<ExcelReportService>().As<IExcelReportService>().InstancePerRequest();

            // ÄÄƒng kĂ½ thá»§ cĂ´ng toĂ n bá»™ Data Access (DA) Ä‘á»ƒ trĂ¡nh lá»—i Assembly Load Context
            builder.RegisterType<Data.Admin.ApiDA>().As<Data.InterfaceDA.Admin.IApiDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.BaoCaoCD45DA>().As<Data.InterfaceDA.IBaoCaoCD45DA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.CD45KhachHangDA>().As<Data.InterfaceDA.Admin.ICD45KhachHangDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.CD45NhomTcvDA>().As<Data.InterfaceDA.Admin.ICD45NhomTcvDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.BaoCaoTongHopDA>().As<Data.InterfaceDA.Admin.IBaoCaoTongHopDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.BVTL_NHOM_TBHDA>().As<Data.InterfaceDA.Admin.IBVTL_NHOM_TBHDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.ChatGayNghien3THDA>().As<Data.InterfaceDA.Admin.IChatGayNghien3THDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.ChatGayNghienTX3THDA>().As<Data.InterfaceDA.Admin.IChatGayNghienTX3THDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.ChuyenGuiDichVuDA>().As<Data.InterfaceDA.Admin.IChuyenGuiDichVuDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.CityDA>().As<Data.InterfaceDA.Admin.ICityDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.CustomerDA>().As<Data.InterfaceDA.Admin.ICustomerDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.DGHLVIIVDA>().As<Data.InterfaceDA.Admin.IDGHLVIIVDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.DuAnDA>().As<Data.InterfaceDA.Admin.IDuAnDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.DuongSuDungDA>().As<Data.InterfaceDA.Admin.IDuongSuDungDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.KetQuaACEDA>().As<Data.InterfaceDA.Admin.IKetQuaACEDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.KetQuaASSISTDA>().As<Data.InterfaceDA.Admin.IKetQuaASSISTDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.KetQuaBHDGDA>().As<Data.InterfaceDA.Admin.IKetQuaBHDGDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.KetQuaHIVDA>().As<Data.InterfaceDA.Admin.IKetQuaHIVDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.KetQuaSKTTDA>().As<Data.InterfaceDA.Admin.IKetQuaSKTTDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.KetQuaXNNTDA>().As<Data.InterfaceDA.Admin.IKetQuaXNNTDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.KhoVatPhamDA>().As<Data.InterfaceDA.Admin.IKhoVatPhamDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.LoaiDoiTuongDA>().As<Data.InterfaceDA.Admin.ILoaiDoiTuongDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.PageActionDA>().As<Data.InterfaceDA.Admin.IPageActionDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.PageMenuDA>().As<Data.InterfaceDA.Admin.IPageMenuDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.PhieuTuVanDA>().As<Data.InterfaceDA.Admin.IPhieuTuVanDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.RoleDA>().As<Data.InterfaceDA.Admin.IRoleDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.RolePageDA>().As<Data.InterfaceDA.Admin.IRolePageDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.SyncDataDA>().As<Data.InterfaceDA.Admin.ISyncDataDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.SysLogDA>().As<Data.InterfaceDA.Admin.ISysLogDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.SysParameterDA>().As<Data.InterfaceDA.Admin.ISysParameterDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.TrainingDataCollVIIVDA>().As<Data.InterfaceDA.Admin.ITrainingDataCollVIIVDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.TTKHMaDaVIIVDA>().As<Data.InterfaceDA.Admin.ITTKHMaDaVIIVDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.TTTruyenThongVIIVDA>().As<Data.InterfaceDA.Admin.ITTTruyenThongVIIVDA>().InstancePerRequest();
            builder.RegisterType<Data.Admin.UserDA>().As<Data.InterfaceDA.Admin.IUserDA>().InstancePerRequest();
            builder.RegisterType<Data.API.InsertDataDA>().As<Data.InterfaceDA.API.IInsertDataDA>().InstancePerRequest();
            builder.RegisterType<Data.API.GetDataFromAPI>().As<Data.InterfaceDA.API.IGetDataFromAPI>().InstancePerRequest();
            builder.RegisterType<Data.API.SyncDataFromApi_SaveToDB>().As<Data.InterfaceDA.API.ISyncDataFromApi_SaveToDB>().InstancePerRequest();

            // Khá»Ÿi táº¡o container
            var container = builder.Build();

            // GĂ¡n container cho MVC Dependency Resolver
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}

