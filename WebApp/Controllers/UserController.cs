using Common;
using Data.Admin;
using Model.Model;
using Model.ModelExtend;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class UserController : BaseController
    {
        BaoCaoBVTLEntities db = new BaoCaoBVTLEntities();
        UserDA _userDA = new UserDA();
        SysLogDA _sysLogDA = new SysLogDA();
        BaseController _helperController = new BaseController();

        // GET: User
        [HasCredential(ControllerName = "User")]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetProfile(int Id)
        {
            var model = db.Users.Select(x => new UserPageModel
            {
                ID = x.ID,
                Name = x.Name,
                Phone = x.Phone,
                Status = x.Status,
                UserName = x.UserName,
                Address = x.Address,
                Avartar = x.Avartar,
                Email = x.Email,
                CreatedDate = x.CreatedDate,
                ROLE_DESC = x.Role.Name,
            }).FirstOrDefault(x => x.ID == Id);

            if (!string.IsNullOrEmpty(model.Avartar))
            {
                model.Avartar = GetBase64Avatar(model.Avartar);
            }

            AddLog("Lấy dữ liệu chi tiết bảng Người dùng( ID: " + Id + ") thành công.");
            return View(model);
        }

        public ActionResult ChangePassword(int Id)
        {
            return View();
        }

        public ActionResult _Add()
        {
            return PartialView("_add");
        }
        public ActionResult _Edit()
        {
            return PartialView("_edit");
        }

        public ActionResult _View()
        {
            return PartialView("_view");
        }
        [HttpPost]
        public ActionResult GetListUser(ModelSearch modelSearch)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                int totalItems = db.Users.Where(x => x.IsAdmin == false).Count();
                int skipRows = (modelSearch.currentPage - 1) * modelSearch.pageSize;
                var data = db.Users.Where(x => x.IsAdmin == false).Select(x => new
                {
                    x.ID,
                    x.Name,
                    x.Phone,
                    x.Status,
                    x.UserName,
                    Role = x.Role.Name,
                    x.Address,
                    x.Avartar,
                    x.Email,
                    x.CreatedDate
                })
                .Where(x => !string.IsNullOrEmpty(modelSearch.KeyWord) ? (x.UserName.Contains(modelSearch.KeyWord) || x.Name.Contains(modelSearch.KeyWord) || x.Phone.Contains(modelSearch.KeyWord)) : true)
                .OrderBy(x => x.Name).Skip(skipRows).Take(modelSearch.pageSize).ToList();

                AddLog("Lấy dữ liệu theo trang bảng Người dùng( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") thành công.");

                return Json(new { data = data, totalItems = totalItems, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu theo trang bảng Người dùng( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }


        [HttpPost]
        public ActionResult GetDanhMuc()
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                // danh sách nhóm quyền
                var dataRole = db.Roles.Select(x => new
                {
                    x.ID,
                    x.Name
                })
                .OrderBy(x => x.Name).ToList();

                // danh sách nhóm thu thập dữ liệu
                var dataTestGroup = db.TestGroups.Select(x => new
                {
                    x.Id,
                    x.Name
                })
                .OrderBy(x => x.Name).ToList();




                return Json(new { DataRoles = dataRole, DataTestGroup = dataTestGroup, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                return Json(obj);
            }
        }

        [HttpPost]
        public object GetItemByID(int? Id)
        {
            try
            {
                var data = db.Users.Select(x => new
                {
                    x.ID,
                    x.Name,
                    x.Phone,
                    x.Status,
                    x.UserName,
                    Role = x.Role.Name,
                    UserGroupID = x.Role.ID,
                    x.Address,
                    x.Avartar,
                    x.Email,
                    x.CreatedDate,
                    x.DateOfBirth,
                    x.Gender,
                    x.IdNumber,
                    x.Possition,
                    x.OperativeLevel,
                    x.OriginId
                }).FirstOrDefault(x => x.ID == Id);

                // Lấy danh sách id nhóm thu thập dữ liệu
                var testGroupIds = db.User_TestGroup.Where(x => x.UserId == Id).Select(x => x.TestGroupId).ToList();

                if (testGroupIds == null)
                    testGroupIds = new List<int>();

                AddLog("Lấy dữ liệu theo ID bảng Người dùng( ID: " + Id + ") thành công.");
                return Json(new { Error = false, Title = "Lấy dữ liệu thành công.", data = data, TestGroupId = testGroupIds });
            }
            catch (Exception ex)
            {
                AddLog("Lấy dữ liệu theo ID bảng Người dùng( ID: " + Id + ") lỗi: " + ex.Message);
                return Json(new { Error = true, Title = ex.Message });
            }
        }

        [HttpPost]
        public object GetViewByID(int Id)
        {
            try
            {
                var data = _userDA.GetItemById(Id);

                AddLog("Lấy dữ liệu theo ID bảng Người dùng( ID: " + Id + ") thành công.");
                return Json(new { Error = false, Title = "Lấy dữ liệu thành công.", data = data });
            }
            catch (Exception ex)
            {
                AddLog("Lấy dữ liệu theo ID bảng Người dùng( ID: " + Id + ") lỗi: " + ex.Message);
                return Json(new { Error = true, Title = ex.Message });
            }
        }


        [HttpPost]
        public ActionResult ConvertPathImageToBase64(string path)
        {
            try
            {
                var pathRoot = Server.MapPath("~/FileUpload");

                var arrayPath = path.IndexOf("FileUpload");
                string pathGet = path.Substring(0, arrayPath) + "FileUpload";
                string patttt = path.Replace(pathGet, pathRoot);

                string base64String = "";
                try
                {
                    if (!string.IsNullOrEmpty(patttt))
                    {
                        using (Image image = Image.FromFile(patttt))
                        {
                            using (MemoryStream m = new MemoryStream())
                            {
                                image.Save(m, image.RawFormat);
                                byte[] imageBytes = m.ToArray();

                                // Convert byte[] to Base64 String
                                base64String = Convert.ToBase64String(imageBytes);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                var result = Json(new { data = base64String }, JsonRequestBehavior.AllowGet);
                result.MaxJsonLength = int.MaxValue;
                return result;
            }
            catch (Exception)
            {
                return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        public string GetBase64Avatar(string path)
        {
            string base64String = "";
            var pathRoot = Server.MapPath("~/FileUpload");

            var arrayPath = path.IndexOf("FileUpload");
            if (arrayPath >= 0)
            {
                string pathGet = path.Substring(0, arrayPath) + "FileUpload";
                string patttt = path.Replace(pathGet, pathRoot);
                try
                {
                    if (!string.IsNullOrEmpty(path))
                    {
                        using (Image image = Image.FromFile(path))
                        {
                            using (MemoryStream m = new MemoryStream())
                            {
                                image.Save(m, image.RawFormat);
                                byte[] imageBytes = m.ToArray();

                                // Convert byte[] to Base64 String
                                base64String = "data:image/jpeg;base64," + Convert.ToBase64String(imageBytes);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            else
            {
                base64String = path;
            }

            return base64String;
        }

        [HttpPost]
        public object Add(User user, string fileName, List<int> testGroupId)
        {
            ObjectMessage obj = new ObjectMessage();
            obj.Error = false;
            try
            {
                var checkTrungUser = db.Users.Where(x => x.UserName == user.UserName).ToList();
                if (checkTrungUser == null || checkTrungUser.Count == 0)
                {
                    if (!string.IsNullOrEmpty(user.Avartar))
                    {
                        var pathPhoto = CoppyPhoto(user.Avartar, fileName, user.UserName, user.Name);
                        user.Avartar = pathPhoto;
                    }
                    user.Status = true;
                    var userLogin = Session["USER_SESSION"] as UserLogin;
                    user.CreatedBy = userLogin.UserID.ToString();
                    user.CreatedDate = DateTime.Now;
                    if (testGroupId == null)
                        testGroupId = new List<int>();
                    obj = _userDA.Add(user, testGroupId);
                    if (obj.Error)
                    {
                        if (!string.IsNullOrEmpty(user.Avartar))
                            DeleteAvatar(user.Avartar, user.UserName, user.Name);
                        AddLog("Thêm mới dữ liệu bảng Người dùng(UserName: " + user.UserName + ", Name: " + user.Name + ") lỗi: " + obj.Title);
                    }
                    //else
                    AddLog("Thêm mới dữ liệu bảng Người dùng(UserName: " + user.UserName + ", Name: " + user.Name + ") thành công.");
                }
                else
                {
                    obj.Error = true;
                    obj.Title = "Trùng tên đăng nhập.";
                    AddLog("Thêm mới dữ liệu bảng Người dùng(UserName: " + user.UserName + ", Name: " + user.Name + ") lỗi: Trùng tên đăng nhập.");
                }
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Thêm mới dữ liệu bảng Người dùng(UserName: " + user.UserName + ", Name: " + user.Name + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        /// <summary>
        /// Coppy ảnh lên server
        /// </summary>
        /// <param name="fileAnhCreate"></param>
        /// <returns></returns>
        public string CoppyPhoto(string base64, string fileName, string userName, string name)
        {
            string destFile = "";
            var pathFolder = Server.MapPath("~/FileUpload/Avatar");
            if (!Directory.Exists(pathFolder))
            {
                Directory.CreateDirectory(pathFolder);
            }
            var countFoleder = 0;
            var directory = System.IO.Directory.GetDirectories(pathFolder);
            if (directory != null)
                countFoleder = directory.Length + 1;
            var folder = Path.Combine(pathFolder + "\\", countFoleder.ToString());
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            try
            {
                destFile = Path.Combine(folder + "\\", fileName);
                var bytes = Convert.FromBase64String(base64);
                using (var imageFile = new FileStream(destFile, FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }
                AddLog("Coppy file ảnh của người dùng(UserName: " + userName + ", Name: " + name + ") thành công.");
            }
            catch (Exception ex)
            {
                destFile = "";
                AddLog("Coppy file ảnh của người dùng(UserName: " + userName + ", Name: " + name + ") lỗi: " + ex.Message);
            }

            var folderServer = Server.MapPath("~/FileUpload");
            if (!string.IsNullOrEmpty(destFile))
                destFile = "/FileUpload" + destFile.Replace(folderServer, "");

            return destFile;
        }

        /// <summary>
        /// Coppy ảnh lên server
        /// </summary>
        /// <param name="fileAnhCreate"></param>
        /// <returns></returns>
        public void DeleteAvatar(string path, string userName, string name)
        {
            try
            {
                var pathRoot = Server.MapPath("~/FileUpload");

                var arrayPath = path.IndexOf("FileUpload");
                string pathGet = path.Substring(0, arrayPath) + "FileUpload";
                string patttt = path.Replace(pathGet, pathRoot);

                System.IO.File.Delete(path);
                AddLog("Xóa file ảnh của người dùng(UserName: " + userName + ", Name: " + name + ") thành công.");
            }
            catch (Exception ex)
            {
                AddLog("Xóa file ảnh của người dùng(UserName: " + userName + ", Name: " + name + ") lỗi: " + ex.Message);
            }
        }


        [HttpPost]
        public object Edit(User user, string fileName, List<int> testGroupId)
        {
            ObjectMessage obj = new ObjectMessage();
            obj.Error = false;
            try
            {
                if (!string.IsNullOrEmpty(user.Avartar) && !string.IsNullOrEmpty(fileName))
                {
                    // xóa file cũ
                    var userOld = db.Users.FirstOrDefault(x => x.ID == user.ID);
                    if (!string.IsNullOrEmpty(userOld.Avartar))
                    {
                        DeleteAvatar(user.Avartar, user.UserName, user.Name);
                    }
                    var pathPhoto = CoppyPhoto(user.Avartar, fileName, user.UserName, user.Name);
                    user.Avartar = pathPhoto;
                }

                var userLogin = Session["USER_SESSION"] as UserLogin;
                user.ModifiedBy = userLogin.UserID.ToString();
                user.ModifiedDate = DateTime.Now;
                if (testGroupId == null)
                    testGroupId = new List<int>();
                obj = _userDA.Edit(user, testGroupId);
                if (obj.Error)
                {
                    if (!string.IsNullOrEmpty(user.Avartar) && !string.IsNullOrEmpty(fileName))
                        DeleteAvatar(user.Avartar, user.UserName, user.Name);
                    AddLog("Cập nhật dữ liệu bảng Người dùng(UserName: " + user.UserName + ", Name: " + user.Name + ") lỗi: " + obj.Title);
                }
                //else
                AddLog("Cập nhật dữ liệu bảng Người dùng(UserName: " + user.UserName + ", Name: " + user.Name + ") thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Cập nhật dữ liệu bảng Người dùng(UserName: " + user.UserName + ", Name: " + user.Name + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }
        [HttpPost]
        public object Delete(int Id)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                obj = _userDA.Delete(Id);
                //if (obj.Error) { }
                //    AddLog("Xóa dữ liệu bảng Người dùng(ID: " + Id + ") lỗi: " + obj.Title);
                //else
                //    AddLog("Xóa dữ liệu bảng Người dùng(ID: " + Id + ") thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Xóa dữ liệu bảng Người dùng(ID: " + Id + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        [HttpPost]
        public object PostChangePassword(string passOld, string passNew)
        {
            var user = Session["USER_SESSION"] as UserLogin;
            ObjectMessage obj = new ObjectMessage();
            obj.Error = false;
            try
            {
                obj = _userDA.ChangePassword(user.UserID, passOld, passNew);

                if (obj.Error)
                {
                    AddLog("Thay đổi mật khẩu Người dùng(UserName: " + user.UserName + ", Name: " + user.Name + ") lỗi: " + obj.Title);
                }
                else
                {
                    AddLog("Thay đổi mật khẩu Người dùng(UserName: " + user.UserName + ", Name: " + user.Name + ") thành công.");
                    Session["USER_SESSION"] = null;
                }

                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Thay đổi mật khẩu Người dùng(UserName: " + user.UserName + ", Name: " + user.Name + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        private void AddLog(string content)
        {
            var user = Session["USER_SESSION"] as UserLogin;
            _sysLogDA.Add(
                    new SysLog
                    {
                        ControllerName = "User",
                        UserName = user.UserName,
                        DateLog = DateTime.Now,
                        Content = content
                    }
                    );
        }

        [HttpPost]
        public ActionResult GetBottomAction()
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                var menu = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");
                var bottoms = _helperController.GetBottomRoleByController(controllerName, menu);
                AddLog("Lấy danh sách các botom được thực hiện trên from Người dùng thành công.");
                return Json(new { Buttoms = bottoms, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy danh sách các botom được thực hiện trên from Người dùng lỗi: " + ex.Message);
                return Json(obj);
            }
        }

    }
}