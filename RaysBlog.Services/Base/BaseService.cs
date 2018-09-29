using RaysBlog.Model;
using RaysBlog.Repository.Interface;
using RaysBlog.Services.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace RaysBlog.Services.Base
{
    #region MyRegion
   // public class BaseService<T> : IBaseService<T>
   //where T : class, new()
   // {
   //     public IBaseRepository<T> baseDal = new BaseRepository<T>();
   //     public dynamic AddEntitiy(T entity)
   //     {
   //         return baseDal.AddEntitiy(entity);
   //     }

   //     public bool DeleteEntitiy(T entity)
   //     {
   //         return baseDal.DeleteEntitiy(entity);
   //     }

   //     public int GetCount()
   //     {
   //         return baseDal.GetCount();
   //     }

   //     public IEnumerable<T> GetEntities(int pageIndex, int pageSize, bool ascending = true)
   //     {
   //         return baseDal.GetEntities(pageIndex, pageSize, ascending);
   //     }

   //     public T GetEntity(int id)
   //     {
   //         return baseDal.GetEntity(id);
   //     }

   //     public bool UpdateEntity(T entity)
   //     {
   //         return baseDal.UpdateEntity(entity);
   //     }
   // } 
    #endregion
}
