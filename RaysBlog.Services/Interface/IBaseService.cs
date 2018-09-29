using RaysBlog.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace RaysBlog.Services.Interface
{
    public interface IBaseService<T>
     where T : class, new()
    {
        IEnumerable<T> GetEntities(int pageIndex, int pageSize, bool ascending = true);
        int GetCount();
        T GetEntity(int id);
        bool UpdateEntity(T entity);
        dynamic AddEntitiy(T entity);
        bool DeleteEntitiy(T entity);
    }
}