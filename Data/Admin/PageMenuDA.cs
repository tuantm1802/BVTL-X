using log4net;
using Model.Model;
using Model.ModelExtend;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Data.Admin
{
    public class PageMenuDA
    {
        BaoCaoBVTLEntities db = new BaoCaoBVTLEntities();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public PageMenu GetItemByName(string name)
        {
            return db.PageMenus.FirstOrDefault(x => x.NAME == name);
        }

        public List<PageMenu> GetAllByPage(ModelSearch modelSearch)
        {
            var pageMenuAlls = db.PageMenus.Where(x => x.IS_ACTIVE == true).ToList();
            if (pageMenuAlls != null && pageMenuAlls.Count > 0)
            {
                if (!string.IsNullOrEmpty(modelSearch.KeyWord))
                    pageMenuAlls.Where(x => x.NAME.Contains(modelSearch.KeyWord) || x.DESCRIPTION.Contains(modelSearch.KeyWord)).ToList();
            }

            if (pageMenuAlls != null && pageMenuAlls.Count > 0)
            {
                return pageMenuAlls;
            }
            else
                return new List<PageMenu>();

        }

        public List<PageMenu> GetAll()
        {
            return db.PageMenus.Where(x => x.IS_ACTIVE == true).ToList();
        }

        public List<PageMenu> GetAllByAppCode()
        {
            return db.PageMenus.Where(x => x.IS_ACTIVE == true).ToList();
        }

        /// <summary>
        /// Lấy danh sách action trên 1 page menu
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public string GetAllByPageMenu(int page)
        {
            var result = "";
            try
            {
                var actions = db.PageActions.Where(x => x.PAGE_ID == page).Select(x => x.CONTROL_NAME).ToArray();
                if (actions != null && actions.Length > 0)
                {
                    result = string.Join("|", actions);
                }
            }
            catch (Exception ex)
            {
                log.Error("PageMenuDA - Lấy danh sách action trên 1 page menu(" + page + ") lỗi: " + ex.Message);
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
                var user = db.Users.FirstOrDefault(x => x.ID == userID);

                // Lấy danh sách menu
                result = (from rp in db.RolePages
                          join pm in db.PageMenus on rp.PageID equals pm.ID
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
                              Actions = rp.CONTROL_STRING
                          }).ToList();

            }
            catch (Exception ex)
            {
                db.SysLogs.Add(
                    new SysLog
                    {
                        ControllerName = "PageMenuDA",
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

        public ObjectMessage Add(PageMenu pageMenu, List<PageAction> pageFunctions)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                using (BaoCaoBVTLEntities context = new BaoCaoBVTLEntities())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        if (pageMenu.PARENT_PAGE_ID == null)
                            pageMenu.PARENT_PAGE_ID = 0;
                        context.PageMenus.Add(pageMenu);
                        context.SaveChanges();

                        if (pageFunctions != null && pageFunctions.Count > 0)
                        {
                            // Thêm biện button action
                            for (int i = 0; i < pageFunctions.Count; i++)
                            {
                                pageFunctions[i].PAGE_ID = pageMenu.ID;
                                context.PageActions.Add(pageFunctions[i]);
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
                log.Error("PageMenuDA - Thêm mới menu(HREF_URL: " + pageMenu.HREF_URL + ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
        public ObjectMessage Edit(PageMenu pageMenu, List<PageAction> pageFunctions)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                using (BaoCaoBVTLEntities context = new BaoCaoBVTLEntities())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        var data = context.PageMenus.FirstOrDefault(x => x.ID == pageMenu.ID);

                        if (pageMenu.PARENT_PAGE_ID == null)
                            pageMenu.PARENT_PAGE_ID = 0;

                        data.NAME = pageMenu.NAME;
                        data.DESCRIPTION = pageMenu.DESCRIPTION;
                        data.IS_ACTIVE = pageMenu.IS_ACTIVE;
                        data.ORDER_BY = pageMenu.ORDER_BY;
                        data.CONTROLLER_NAME = pageMenu.CONTROLLER_NAME;
                        data.HREF_URL = pageMenu.HREF_URL;
                        data.PARENT_PAGE_ID = pageMenu.PARENT_PAGE_ID;
                        data.IS_SYSTEM_ROLE = pageMenu.IS_SYSTEM_ROLE;
                        context.SaveChanges();

                        var deletes = context.PageActions.Where(x => x.PAGE_ID == data.ID).ToList();
                        if (deletes != null && deletes.Count > 0)
                        {
                            foreach (var item in deletes)
                            {
                                context.PageActions.Remove(item);
                            }
                            context.SaveChanges();
                        }


                        if (pageFunctions != null && pageFunctions.Count > 0)
                        {
                            // Thêm biện button action
                            for (int i = 0; i < pageFunctions.Count; i++)
                            {
                                pageFunctions[i].PAGE_ID = pageMenu.ID;
                                context.PageActions.Add(pageFunctions[i]);
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
                log.Error("PageMenuDA - Sửa menu(HREF_URL: " + pageMenu.HREF_URL + ") lỗi: " + ex.Message);
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
                var data = db.PageMenus.FirstOrDefault(x => x.ID == Id);
                data.IS_ACTIVE = false;

                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Xóa thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("PageMenuDA - Xóa menu(Id: " + Id + ", updateBy: " + updateBy + ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
    }
}
