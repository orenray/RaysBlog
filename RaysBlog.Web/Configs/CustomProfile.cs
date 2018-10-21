using RaysBlog.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaysBlog.Web.Configs
{
    //public class CustomProfile:Profile
    //{
    //    public CustomProfile()
    //    {
    //        CreateMap<BlogArticle, ArticleDTO>().ForMember(dest=>dest.CategoryName,opts=>opts.MapFrom(src=>src.Category.CategoryName)).ForMember(dest=>dest.CategoryId,opts=>opts.MapFrom(src=>src.Category.CategoryId)).ForMember(dest => dest.TagId, opts => opts.MapFrom(src => src.Tag.TagId)).ForMember(dest => dest.TagName, opts => opts.MapFrom(src => src.Tag.TagName)).ForMember(dest => dest.PostDate, opts => opts.MapFrom(src => src.PostDate.ToString("yyyy-MM-dd"))).ForMember(dest => dest.Remark, opts => opts.MapFrom(src => src.Remark??""));
    //        CreateMap<ArticleDTO, BlogArticle>().ForMember(dest=>dest.Category,opts=>opts.MapFrom(src=>new BlogCategory {  CategoryId=src.CategoryId, CategoryName=src.CategoryName})).ForMember(dest => dest.Tag, opts => opts.MapFrom(src => new BlogTag {  ArId=src.ArticleId, TagId=src.TagId,TagName=src.TagName})).ForMember(dest => dest.PostDate, opts => opts.MapFrom(src =>DateTime.Now));
    //    }
    //}
}
