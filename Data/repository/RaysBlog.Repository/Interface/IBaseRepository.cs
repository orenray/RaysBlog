using RaysBlog.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace RaysBlog.Repository.Interface
{
    public interface IBaseRepository<T> 
        where T:class,new()
    {
        IEnumerable<T> GetEntities(int pageIndex,int pageSize,bool ascending=true);
        int GetTotalCount();
        T GetEntity(int id);
        bool UpdateEntity(T entity);
        dynamic AddEntitiy(T entity);
        bool DeleteEntitiy(T entity);
    }
}
