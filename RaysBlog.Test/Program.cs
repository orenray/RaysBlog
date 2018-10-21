using RaysBlog.Repository;
using System;
using System.Linq;
using RaysBlog.Model;
using System.Collections.Generic;

namespace RaysBlog.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ArticleRepository adal = new ArticleRepository();
            //var list= dal.GetEntitiesByTag("标签A",1,4).ToList();
            //var s1 =dal.GetAsync(1).Result;
            //UserInfoRepository userDal = new UserInfoRepository();
            //var s= userDal.Get("oren", "123");
            //var s1 = userDal.GetUserInfos();
            //var s = adal.Get(1);
            //var s = dal.GetPagerByKeywords(1, 3);
            //var s2 = dal.GetPagerByTag(1, 3,"标签C");
            //int id = dal.GetMaxId()+1;
            //BlogArticle article = new BlogArticle {
            //    ArticleId=id,
            //    ArticleName = "测试保存",
            //    Body="测试保存测试保存啊",
            //    Category=new BlogCategory { CategoryId=36 },
            //    IsPublished=true,
            //    PostDate=DateTime.Now,
            //    Remark=null,
            //    Tags=new List<BlogTag>
            //    {
            //        new BlogTag { ArId=id, TagName="标签s"},
            //        new BlogTag { ArId=id, TagName="标签ss"},
            //        new BlogTag { ArId=id, TagName="标签sss"}
            //    }
            //};
            //var r= dal.Add(article);

            Console.WriteLine();
            Console.WriteLine("test.............");
            Console.ReadKey();
        }
    }
}
