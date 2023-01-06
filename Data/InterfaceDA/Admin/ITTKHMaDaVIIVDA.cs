using System;
using Model.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using Model.ModelExtend.API;

namespace Data.InterfaceDA.Admin
{
    public interface ITTKHMaDaVIIVDA
    {

        /// <summary>
        /// Lấy kết quả Thông tin KH Mất dấu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TTKHMaDaVIIVPageModel GetItemById(int id);

        /// <summary>
        /// Lấy kết quả Thông tin KH Mất dấu
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<TTKHMaDaVIIVPageModel> GetAllByPage(ModelSearch modelSearch);
    }
}
