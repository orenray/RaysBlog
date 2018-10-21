using Dapper;
using RaysBlog.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RaysBlog.Repository
{
    public class PermissionRepository
    {
        public int GetMaxId()
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                return conn.ExecuteScalar<int>("select max(permissionId) from blogpermission");
            }
        }
        private int GetExistCount(string sql, object parameter)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                var result = conn.ExecuteScalar<int>(sql, parameter);
                return result;
            }
        }
        public (bool IsAdd, string msg) Add(BlogPermission permission)
        {
            using (var conn=ConnectionFactory.GetOpenConnection())
            {
                var tran = conn.BeginTransaction();
                try
                {
                    if (GetExistCount("select count(*) from blogpermission where permissionId=@pid", new { pid = permission.PermissionId }) > 0)
                    {
                        return (false, "角色已经存在");
                    }
                    var pNum = conn.Execute("insert into blogpermission(PermissionName) values(@pName)", new { pName = permission.PermissionName });
                    if (pNum > 0)
                    {
                        tran.Commit();
                        return (true, "角色添加成功");
                    }
                    else
                    {
                        tran.Rollback();
                        return (false, "角色添加失败");
                    }
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return (false, ex.Message);
                }
            }
        }
        public async Task<(bool IsAdd, string msg)> AddAsync(BlogPermission permission)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                var tran = conn.BeginTransaction();
                try
                {
                    if (GetExistCount("select count(*) from blogpermission where permissionId=@pid", new { pid = permission.PermissionId }) > 0)
                    {
                        return (false, "角色已经存在");
                    }
                    var pNum =await conn.ExecuteAsync("insert into blogpermission(PermissionName) values(@pName)", new { pName = permission.PermissionName });
                    if (pNum > 0)
                    {
                        tran.Commit();
                        return (true, "角色添加成功");
                    }
                    else
                    {
                        tran.Rollback();
                        return (false, "角色添加失败");
                    }
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return (false, ex.Message);
                }
            }
        }
    }
}
