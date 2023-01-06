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
    public interface ITrainingDataCollVIIVDA
    {

        /// <summary>
        /// Lấy kết quả Training Data Collection
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TrainingDataCollVIIVPageModel GetItemById(int id);

        /// <summary>
        /// Lấy kết quả Training Data Collection
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<TrainingDataCollVIIVPageModel> GetAllByPage(ModelSearch modelSearch);
    }
}
