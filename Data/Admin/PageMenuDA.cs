using Data.InterfaceDA.Admin;
using log4net;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Data.Admin
{
    public class PageMenuDA: IPageMenuDA
    {
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public BVTL_QT_PAGE_MENU GetItemByName(string name)
        {
            return db.BVTL_QT_PAGE_MENU.FirstOrDefault(x => x.NAME == name);
        }

        public List<BVTL_QT_PAGE_MENU> GetAllByPage(ModelSearch modelSearch)
        {
            var BVTL_QT_PAGE_MENUAlls = db.BVTL_QT_PAGE_MENU.Where(x => x.IS_ACTIVE == true).ToList();
            if (BVTL_QT_PAGE_MENUAlls != null && BVTL_QT_PAGE_MENUAlls.Count > 0)
            {
                if (!string.IsNullOrEmpty(modelSearch.KeyWord))
                    BVTL_QT_PAGE_MENUAlls.Where(x => x.NAME.Contains(modelSearch.KeyWord) || x.DESCRIPTION.Contains(modelSearch.KeyWord)).ToList();
            }

            if (BVTL_QT_PAGE_MENUAlls != null && BVTL_QT_PAGE_MENUAlls.Count > 0)
            {
                return BVTL_QT_PAGE_MENUAlls;
            }
            else
                return new List<BVTL_QT_PAGE_MENU>();

        }

        public List<BVTL_QT_PAGE_MENU> GetAll()
        {
            return db.BVTL_QT_PAGE_MENU.Where(x => x.IS_ACTIVE == true).ToList();
        }

        public List<BVTL_QT_PAGE_MENU> GetAllByAppCode()
        {
            return db.BVTL_QT_PAGE_MENU.Where(x => x.IS_ACTIVE == true).ToList();
        }

        /// <summary>
        /// Lấy danh sách action trên 1 page menu
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public string GetAllByBVTL_QT_PAGE_MENU(int page)
        {
            var result = "";
            try
            {
                var actions = db.BVTL_QT_PAGE_ACTION.Where(x => x.PAGE_ID == page).Select(x => x.CONTROL_NAME).ToArray();
                if (actions != null && actions.Length > 0)
                {
                    result = string.Join("|", actions);
                }
            }
            catch (Exception ex)
            {
                log.Error("BVTL_QT_PAGE_MENUDA - Lấy danh sách action trên 1 page menu(" + page + ") lỗi: " + ex.Message);
                result = "";
            }
            return result;
        }
        /// <summary>
        /// Lấy danh sách menu theo user ID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<MenuModel> GetMenuByUser(long userID)
        {
            var result = new List<MenuModel>();
            try
            {
                // lấy thông tin user
                var user = db.BVTL_QT_NGUOI_DUNG.FirstOrDefault(x => x.ID == userID);

                // Lấy danh sách menu
                result = (from rp in db.BVTL_QT_QUYEN_PAGE
                          join pm in db.BVTL_QT_PAGE_MENU on rp.PageID equals pm.ID
                          where rp.RoleID == user.GroupID && pm.IS_ACTIVE == true && rp.IS_ACTIVE == true
                          select new MenuModel
                          {
                              ID = pm.ID,
                              NAME = pm.NAME,
                              DESCRIPTION = pm.DESCRIPTION,
                              IS_ACTIVE = pm.IS_ACTIVE,
                              ORDER_BY = pm.ORDER_BY,
                              CONTROLLER_NAME = pm.CONTROLLER_NAME,
                              HREF_URL = pm.HREF_URL,
                              PARENT_PAGE_ID = pm.PARENT_PAGE_ID,
                              IS_SYSTEM_ROLE = pm.IS_SYSTEM_ROLE,
                              Actions = rp.CONTROL_STRING,
                              TEN_DU_AN = pm.TEN_DU_AN
                          }).ToList();

            }
            catch (Exception ex)
            {
                db.BVTL_QT_LOG.Add(
                    new BVTL_QT_LOG
                    {
                        ControllerName = "BVTL_QT_PAGE_MENUDA",
                        UserName = "",
                        DateLog = DateTime.Now,
                        Content = "Lấy danh sách menu lỗi: " + ex.Message
                    }
                    );
                db.SaveChanges();
                result = new List<MenuModel>();
            }
            return result;
        }

        public ObjectMessage Add(BVTL_QT_PAGE_MENU modelPage, List<BVTL_QT_PAGE_ACTION> pageFunctions)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                using (BVTL_REPORTINGEntities context = new BVTL_REPORTINGEntities())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        if (modelPage.PARENT_PAGE_ID == null)
                            modelPage.PARENT_PAGE_ID = 0;
                        context.BVTL_QT_PAGE_MENU.Add(modelPage);
                        context.SaveChanges();

                        if (pageFunctions != null && pageFunctions.Count > 0)
                        {
                            // Thêm biện button action
                            for (int i = 0; i < pageFunctions.Count; i++)
                            {
                                pageFunctions[i].PAGE_ID = modelPage.ID;
                                context.BVTL_QT_PAGE_ACTION.Add(pageFunctions[i]);
                                context.SaveChanges();
                            }
                        }

                        dbContextTransaction.Commit();
                    }
                }

                obj.Error = false;
                obj.Title = "Thêm mới thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("BVTL_QT_PAGE_MENUDA - Thêm mới menu(HREF_URL: " + modelPage.HREF_URL + ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
        public ObjectMessage Edit(BVTL_QT_PAGE_MENU modelPage, List<BVTL_QT_PAGE_ACTION> pageFunctions)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                using (BVTL_REPORTINGEntities context = new BVTL_REPORTINGEntities())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        var data = context.BVTL_QT_PAGE_MENU.FirstOrDefault(x => x.ID == modelPage.ID);

                        if (modelPage.PARENT_PAGE_ID == null)
                            modelPage.PARENT_PAGE_ID = 0;

                        data.NAME = modelPage.NAME;
                        data.DESCRIPTION = modelPage.DESCRIPTION;
                        data.IS_ACTIVE = modelPage.IS_ACTIVE;
                        data.ORDER_BY = modelPage.ORDER_BY;
                        data.CONTROLLER_NAME = modelPage.CONTROLLER_NAME;
                        data.HREF_URL = modelPage.HREF_URL;
                        data.PARENT_PAGE_ID = modelPage.PARENT_PAGE_ID;
                        data.IS_SYSTEM_ROLE = modelPage.IS_SYSTEM_ROLE;
                        context.SaveChanges();

                        var deletes = context.BVTL_QT_PAGE_ACTION.Where(x => x.PAGE_ID == data.ID).ToList();
                        if (deletes != null && deletes.Count > 0)
                        {
                            foreach (var item in deletes)
                            {
                                context.BVTL_QT_PAGE_ACTION.Remove(item);
                            }
                            context.SaveChanges();
                        }


                        if (pageFunctions != null && pageFunctions.Count > 0)
                        {
                            // Thêm biện button action
                            for (int i = 0; i < pageFunctions.Count; i++)
                            {
                                pageFunctions[i].PAGE_ID = modelPage.ID;
                                context.BVTL_QT_PAGE_ACTION.Add(pageFunctions[i]);
                                context.SaveChanges();
                            }

                        }

                        dbContextTransaction.Commit();
                    }
                }

                obj.Error = false;
                obj.Title = "Cập nhật thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("BVTL_QT_PAGE_MENUDA - Sửa menu(HREF_URL: " + modelPage.HREF_URL + ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
        public ObjectMessage Delete(int Id, int updateBy)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.BVTL_QT_PAGE_MENU.FirstOrDefault(x => x.ID == Id);
                data.IS_ACTIVE = false;

                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Xóa thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("BVTL_QT_PAGE_MENUDA - Xóa menu(Id: " + Id + ", updateBy: " + updateBy + ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
    }
}
