using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Dapper;

namespace RaysBlog.Repository.Interface
{
    public class BaseRepository<T> :IBaseRepository<T> 
        where T:class,new()
    {
        /// <summary>
        /// 计算页数
        /// </summary>
        /// <param name="totalRecords"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        protected int GetPagesCount(int totalRecords, int pageSize)
        {
            return totalRecords / pageSize + (totalRecords % pageSize) > 0 ? 1 : 0;
        }
        public T GetEntity(int id)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                return conn.Get<T>(id);
            }
        }

        public bool UpdateEntity(T entity)
        {
            DbTransaction tran = null;
            bool result = false;
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                try
                {
                    tran = conn.BeginTransaction();
                    result = conn.Update<T>(entity, tran);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    result = false;
                    //throw new Exception(ex.Message);
                }
            }
            return result;
        }
        public dynamic AddEntitiy(T entity)
        {
            DbTransaction tran = null;
            dynamic result = false;
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                try
                {
                    tran = conn.BeginTransaction();
                    result = conn.Insert(entity, transaction: tran);
                    tran?.Commit();
                }
                catch (Exception ex)
                {
                    tran?.Rollback();
                    result = null;
                    //throw new Exception(ex.Message);
                }
            }
            return result;
        }

        public bool DeleteEntitiy(T entity)
        {
            DbTransaction tran = null;
            bool result = false;
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                try
                {
                    tran = conn.BeginTransaction();
                    result = conn.Delete(entity, transaction: tran);
                    tran?.Commit();

                }
                catch (Exception ex)
                {
                    tran?.Rollback();
                    result = false;
                    //throw new Exception(ex.Message);
                }
            }
            return result;
        }

        public int GetTotalCount()
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                return conn.Count<T>(null);
            }
        }

        public virtual IEnumerable<T> GetEntities(int pageIndex, int pageSize, bool ascending = true)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                var list = conn.GetPage<T>(null, null, pageIndex, pageSize);
                return list;
            }
        }
    }
}
