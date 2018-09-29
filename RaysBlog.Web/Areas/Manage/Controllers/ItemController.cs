﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RaysBlog.Repository;
using RaysBlog.Web.Models.DTO;

namespace RaysBlog.Web.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Route("Ray/item")]
    public class ItemController : Controller
    {
        private readonly CategoryRepository _categoryService;
        private readonly ArticleRepository _articleService;
        public ItemController()
        {
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

            return ViewComponent("Category", new { id, pageIndex, ascending });
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
                    if (!string.IsNullOrEmpty(artName) && !string.IsNullOrEmpty(body) && !string.IsNullOrEmpty(artName) && !string.IsNullOrEmpty(remark))
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
                                    Tag = tg
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
                                    Tag = new Model.BlogTag { TagName = tagName }
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
    }
}