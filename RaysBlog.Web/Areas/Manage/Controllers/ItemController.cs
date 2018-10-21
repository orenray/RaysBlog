using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using RaysBlog.Repository;

namespace RaysBlog.Web.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Route("Ray/item")]
    [Authorize(Roles ="admin,system")]
    public class ItemController : Controller
    {
        private readonly CategoryRepository _categoryService;
        private readonly ArticleRepository _articleService;
        //private readonly UserInfoRepository  _userInfoService;
        //private readonly PermissionRepository  _permissionService;
        private readonly IHostingEnvironment hostingEnvironment;

        public ItemController(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
            _categoryService= new CategoryRepository();
            _articleService = new ArticleRepository();

        }
        public IActionResult Index(string id)
        {
            ViewBag.TypeId = id;
            ViewBag.PageIndex = 1;
            return View();
        }
        [HttpPost]
        public IActionResult Index(string id, int pageIndex, bool ascending = true)
        {
            ViewBag.TypeId = id;
            ViewBag.PageIndex = pageIndex;
            string vcName= "";   
            switch (id)
	        {
		        case "1":
                    vcName="Category";
                break;
                case "2":
                    vcName="Article";
                 break;
                case "3":
                    vcName = "Tag";
                    break;
            }

            return ViewComponent(vcName, new { id, pageIndex, ascending });
        }

        [Route("update")]
        public IActionResult Add(string typeId,string actionstr, int mid)
        {
            var vcName = string.Empty;
            (string typeId, string action, dynamic entry) partParam =(typeId, actionstr, null);
            switch (typeId)
            {
                case "1":
                    vcName = "_Add";
                    if (actionstr == "1")
                    {
                        var mc = _categoryService.Get(mid);
                        partParam = (typeId, actionstr, mc);
                    }
                    if (actionstr == "0")
                    {
                        partParam =(typeId, actionstr, null);
                    }
                    break;
                case "2":
                    vcName = "_Add";
                    if (actionstr == "1")
                    {
                        (Model.BlogArticle article, IEnumerable<Model.BlogCategory> blogCategories) mac = (_articleService.Get(mid), _categoryService.GetCategorys());
                        partParam = (typeId, actionstr, mac);
                    }
                    if (actionstr == "0")
                    {
                        (Model.BlogArticle article, IEnumerable<Model.BlogCategory> blogCategories) mac = (null, _categoryService.GetCategorys());
                        partParam = (typeId, actionstr, mac);
                    }
                    break;
            }
            return PartialView(vcName, partParam);
        }
        [Route("add")]
        [AutoValidateAntiforgeryToken]
        [HttpPost]
        //[AutoValidateAntiforgeryToken]
        public IActionResult Update()
        {
            dynamic obj = null;
            (bool, string) IsSuccess = (false, "");
            string typeId = Request.Form["typeId"].ToString(),action= Request.Form["action"].ToString();
            if (string.IsNullOrEmpty(typeId)|| string.IsNullOrEmpty(action))
            {
                obj = new { code = 0, tid = -1, msg = "请求异常" };
                return Json(obj);
            }
            switch (typeId)
            {
                #region 分类添加和修改
                case "1":
                    var caidStr = Request.Form["CategoryId"].ToString();
                    var caName = Request.Form["CategoryName"].ToString();
                    if (!string.IsNullOrEmpty(caName))
                    {
                        if (!string.IsNullOrEmpty(action))
                        {
                            if (action == "1")
                            {
                                if (!int.TryParse(caidStr, out var id))
                                {
                                    obj = new { code = 0, tid = typeId, msg = "分类Id转换错误" };
                                    return Json(obj);
                                }
                                var m = new Model.BlogCategory
                                {
                                    CategoryId = id,
                                    CategoryName = caName
                                };
                                IsSuccess = _categoryService.Update(m);
                            }
                            if (action == "0")
                            {
                                var m = new Model.BlogCategory
                                {
                                    CategoryName = caName
                                };
                                IsSuccess = _categoryService.Add(m);
                            }
                        }
                        if (IsSuccess.Item1)
                        {
                            obj = new { code = 0, tid = typeId, msg = IsSuccess.Item2 };
                        }
                        else
                        {
                            obj = new { code = 1, tid = typeId, msg = IsSuccess.Item2 };
                        }
                    }
                    else
                    {
                        obj = new { code = 2, tid = typeId, msg = "请求失败" };
                    }
                    break;
                #endregion
                #region 文章添加和修改
                case "2":
                    var artidStr = Request.Form["ArticleId"].ToString();
                    var artName = Request.Form["ArticleName"].ToString();
                    var ar_caidStr = Request.Form["CategoryId"].ToString();
                    var tagName = Request.Form["TagName"].ToString();
                    var tagidStr = Request.Form["TagId"].ToString();
                    var body = Request.Form["body"].ToString();
                    var remark = Request.Form["remark"].ToString();
                    var TitileImgPath = Request.Form["TitleImgPath"].ToString();
                    if (!string.IsNullOrEmpty(artName) && !string.IsNullOrEmpty(body) && !string.IsNullOrEmpty(artName)&&!string.IsNullOrEmpty(TitileImgPath))
                    {
                        if (!string.IsNullOrEmpty(action))
                        {
                            if (!int.TryParse(ar_caidStr, out var caid))
                            {
                                obj = new { code = 0, tid = typeId, msg = "分类Id转换错误" };
                                return Json(obj);
                            }
                            if (action == "1")
                            {
                                if (!int.TryParse(artidStr, out var artid))
                                {
                                    obj = new { code = 0, tid = typeId, msg = "文章Id转换错误" };
                                    return Json(obj);
                                }
                                Model.BlogTag tg = null;
                                if (int.TryParse(tagidStr, out var tgid))
                                {
                                    tg = new Model.BlogTag
                                    {
                                        TagId = tgid,
                                        ArId = artid,
                                        TagName = tagName ?? tagName.Trim()
                                    };
                                }
                                var m = new Model.BlogArticle
                                {
                                    ArticleId = artid,
                                    ArticleName = artName,
                                    Body = body,
                                    Remark = remark,
                                    IsPublished = true,
                                    PostDate = DateTime.Now,
                                    Category = _categoryService.Get(caid),
                                    Tag = tg,
                                    TitleImgPath= TitileImgPath
                                };
                                IsSuccess = _articleService.Update(m);
                            }
                            if (action == "0")
                            {
                                var m = new Model.BlogArticle
                                {
                                    ArticleName = artName,
                                    Body = body,
                                    Remark = remark,
                                    IsPublished = true,
                                    PostDate = DateTime.Now,
                                    Category = _categoryService.Get(caid),
                                    Tag = new Model.BlogTag { TagName = tagName },
                                    TitleImgPath = TitileImgPath
                                };
                                IsSuccess = _articleService.Add(m);
                            }
                        }
                        if (IsSuccess.Item1)
                        {
                            obj = new { code = 0, tid = typeId, msg = IsSuccess.Item2 };
                        }
                        else
                        {
                            obj = new { code = 1, tid = typeId, msg = IsSuccess.Item2 };
                        }
                    }
                    else
                    {
                        obj = new { code = 2, tid = typeId, msg = "请求失败" };
                    }
                    break;
                default:
                    obj = new { code = 2, tid = -1, msg = "请求异常" };
                    break;
                    #endregion
            }
            return Json(obj);
        }
        [Route("del")]
        [AutoValidateAntiforgeryToken] //CSRF  防跨域请求攻击
        [HttpPost]
        public IActionResult Delete()
        {
            string id = Request.Form["id"];
            string typeId = Request.Form["typeId"];
            dynamic obj = null;
            switch (typeId)
            {
                case "1":
                    if (!string.IsNullOrEmpty(id))
                    {
                        var m = _categoryService.Get(Convert.ToInt32(id));
                        var (IsDelete, msg) = _categoryService.Delete(m);
                        if (IsDelete)
                        {
                            obj = new { code = 0, msg = "删除成功" };
                        }
                        else
                        {
                            obj = new { code = 1, msg = "删除失败" };
                        }
                    }
                    else
                    {
                        obj = new { code = 2, msg = "请求失败" };
                    }
                    break;
                case "2":
                    if (!string.IsNullOrEmpty(id))
                    {
                        var m = _articleService.Get(Convert.ToInt32(id));
                        var (IsDelete, msg) = _articleService.Delete(m);
                        if (IsDelete)
                        {
                            obj = new { code = 0, msg = "删除成功" };
                        }
                        else
                        {
                            obj = new { code = 1, msg = "删除失败" };
                        }
                    }
                    else
                    {
                        obj = new { code = 2, msg = "请求失败" };
                    }
                    break;
            }
            return Json(obj);
        }

        [Route("ImgUp")]
        [HttpPost]
        public IActionResult Upload()//问题，需要处理上传图片后，不提交时，应删除之前上传图片的问题
        {
            var imgFile = Request.Form.Files[0];
            if(imgFile==null||string.IsNullOrEmpty(imgFile.FileName)) return Json(new { code = 1, msg = "上传失败", });
            long size = imgFile.Length;
            var fileName = imgFile.FileName;//文件名
            var extName = fileName.Substring(fileName.LastIndexOf("."), fileName.Length - fileName.LastIndexOf("."));//扩展名
            var fileName1 = Guid.NewGuid().ToString().Replace("-", "");
            fileName1 = fileName1 + "_" + fileName;
            string dirDate = DateTime.Now.ToString("yyyyMMdd");
            string dir1 = hostingEnvironment.WebRootPath + $@"/upload/{dirDate}";
            try
            {
                if (!System.IO.Directory.Exists(dir1))
                {
                    System.IO.Directory.CreateDirectory(dir1);
                }
                fileName = dir1 + $@"/{fileName1}";//上传后的文件全路径
                using (var fs = System.IO.File.Create(fileName))
                {
                    imgFile.CopyTo(fs);
                    fs.Flush();
                }
                var result = new
                {
                    code = 0,
                    msg = "上传成功",
                    data = new { src = $"/upload/{dirDate}/{fileName1}", title = "#" }
                };
                return Json(result);
            }
            catch (Exception)
            {
                var result = new
                {
                    code = -1,
                    msg = "上传失败",
                    data = new { src = $"/upload/error.gif", title = "错误" }
                };
                return Json(result);
            }           
            
        }
        [HttpPost]
        [Route("titleImgUp")]
        public IActionResult UpLoadForTitle()
        {
            var imgFile = Request.Form.Files[0];
            if (imgFile == null || string.IsNullOrEmpty(imgFile.FileName)) return Json(new { code = 1, msg = "上传失败", });
            long size = imgFile.Length;
            var fileName = imgFile.FileName;//文件名
            var extName = fileName.Substring(fileName.LastIndexOf("."), fileName.Length - fileName.LastIndexOf("."));//扩展名
            var fileName1 = Guid.NewGuid().ToString().Replace("-", "");
            fileName1 = fileName1 + "_" + fileName;
            string dirDate = DateTime.Now.ToString("yyyyMMdd");
            string dir1 = hostingEnvironment.WebRootPath + $@"/uploadTitle/{dirDate}";//--》"/"linux写法
            //System.IO.DirectoryInfo root = new System.IO.DirectoryInfo(hostingEnvironment.WebRootPath);
            try
            {
                if (!System.IO.Directory.Exists(dir1))
                {
                    System.IO.Directory.CreateDirectory(dir1);     
                    //directory.CreateSubdirectory(dirDate);
                }
                fileName = dir1 + $@"/{fileName1}";//上传后的文件全路径
                using (var fs = System.IO.File.Create(fileName))
                {
                    imgFile.CopyTo(fs);
                    fs.Flush();
                }
                TitileImgPath = $"/uploadTitle/{dirDate}/{fileName1}";
                var result = new
                {
                    code = 0,
                    msg = "上传成功",
                    data = new { src = $"/uploadTitle/{dirDate}/{fileName1}", title = "#" }
                };
                return Json(result);
            }
            catch (Exception)
            {
                var result = new
                {
                    code = -1,
                    msg = "上传失败",
                    data = new { src = $"/upload/error.gif", title = "错误" }
                };
                return Json(result);
            }

        }
        private static string TitileImgPath=string.Empty;
    }
}