using System;
using Model.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.ModelExtend;
using Model.ModelExtend.Base;

namespace Data.InterfaceDA.Admin
{
    public interface IChatGayNghienTX3THDA
    {

        /// <summary>
        /// Lấy dữ liệu theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        BVTL_CHAT_GAY_NGHIEN_TX3TH GetItemById(int id);

        /// <summary>
        /// Lấy dữ liệu theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        List<ChatGayNghienTX3THPageModel> GetAllByPage(ModelSearch modelSearch);

        /// <summary>
        /// Lấy all dữ liệu
        /// </summary>
        /// <returns></returns>
        List<BVTL_CHAT_GAY_NGHIEN_TX3TH> GetAll();
    }
}
