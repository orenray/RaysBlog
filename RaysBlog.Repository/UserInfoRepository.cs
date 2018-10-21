using Dapper;
using RaysBlog.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace RaysBlog.Repository
{
    public class UserInfoRepository
    {
        public int GetMaxId()
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                return conn.ExecuteScalar<int>("select max(userId) from bloguserinfo");
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
        public BlogUserInfo Get(string userName,string pwd)
        {
            using (var conn=ConnectionFactory.GetOpenConnection())
            {
                //var lookup = new Dictionary<int, BlogUserInfo>();
                var result = conn.Query<BlogUserInfo, BlogPermission, BlogUserInfo>(@"SELECT bu.*,bp.* FROM bloguserinfo AS bu 
INNER JOIN bloguser_permission AS bup ON bu.UserId=bup.UserId 
INNER JOIN blogpermission AS bp ON bup.PermissionId=bp.PermissionId 
where  bu.IsEnabled=1 and  bu.Name=@userName and bu.password=@pwd",
                (bu, bp) =>
                {
                    bu.Permission = bp;
                    return bu;
                }, new { userName, pwd }, splitOn: "UserId,PermissionId");
                //return lookup.Values.FirstOrDefault();
                return result.FirstOrDefault();
            }
        }
        public async Task<BlogUserInfo> GetAsync(string userName, string pwd)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                //var lookup = new Dictionary<int, BlogUserInfo>();
                var result =await conn.QueryAsync<BlogUserInfo, BlogPermission, BlogUserInfo>(@"SELECT bu.*,bp.* FROM bloguserinfo AS bu 
INNER JOIN bloguser_permission AS bup ON bu.UserId=bup.UserId 
INNER JOIN blogpermission AS bp ON bup.PermissionId=bp.PermissionId 
where  bu.IsEnabled=1 and  bu.Name=@userName and bu.password=@pwd",
                (bu, bp) =>
                {
                    //if (!lookup.TryGetValue(bu.UserId, out var userInfo))
                    //{
                    //    lookup.Add(bu.UserId, userInfo = bu);
                    //}
                    //userInfo.Permissions = userInfo.Permissions ?? new List<BlogPermission>();
                    //userInfo.Permissions.Add(bp);
                    //return userInfo;
                    bu.Permission = bp;
                    return bu;
                }, new { userName, pwd }, splitOn: "UserId,PermissionId");
                //return lookup.Values.FirstOrDefault();
                return result.FirstOrDefault();
            }
        }
        public IEnumerable<BlogUserInfo> GetUserInfos()
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                //var lookup = new Dictionary<int,BlogUserInfo>();
                var result = conn.Query<BlogUserInfo, BlogPermission, BlogUserInfo>(@"SELECT bu.*,bp.* FROM bloguserinfo AS bu 
INNER JOIN bloguser_permission AS bup ON bu.UserId=bup.UserId 
INNER JOIN blogpermission AS bp ON bup.PermissionId=bp.PermissionId 
where  bu.IsEnabled=1 ",
                (bu, bp) =>
                {
                    //if (!lookup.TryGetValue(bu.UserId,out var userInfo))
                    //{
                    //    lookup.Add(bu.UserId, userInfo = bu);
                    //}
                    //userInfo.Permissions = userInfo.Permissions ?? new List<BlogPermission>();
                    //userInfo.Permissions.Add(bp);
                    //return userInfo;
                    bu.Permission = bp;
                    return bu;
                }, splitOn: "UserId,PermissionId");
                //return lookup.Values;
                return result;
            }
        }
        public async Task<IEnumerable<BlogUserInfo>> GetUserInfosAsync()
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                //var lookup = new Dictionary<int, BlogUserInfo>();
                var result =await conn.QueryAsync<BlogUserInfo, BlogPermission, BlogUserInfo>(@"SELECT bu.*,bp.* FROM bloguserinfo AS bu 
INNER JOIN bloguser_permission AS bup ON bu.UserId=bup.UserId 
INNER JOIN blogpermission AS bp ON bup.PermissionId=bp.PermissionId 
where  bu.IsEnabled=1 ",
                (bu, bp) =>
                {
                    //if (!lookup.TryGetValue(bu.UserId, out var userInfo))
                    //{
                    //    lookup.Add(bu.UserId, userInfo = bu);
                    //}
                    //userInfo.Permissions = userInfo.Permissions ?? new List<BlogPermission>();
                    //userInfo.Permissions.Add(bp);
                    //return userInfo;
                    bu.Permission = bp;
                    return bu;
                }, splitOn: "UserId,PermissionId");
                //return lookup.Values;
                return result;
            }
        }
        public (bool IsAdd, string msg) Add(BlogUserInfo userInfo)
        {
            using (var conn=ConnectionFactory.GetOpenConnection())
            {
                var tran = conn.BeginTransaction();
                if (userInfo==null||userInfo.Permission==null)
                {
                    return (false, "用户添加信息不完整，添加失败。");
                }
                try
                {
                    if (GetExistCount("select count(*) from bloguser_permission where userId=@uid and permissionId = @pid ", new { uid = userInfo.UserId, pid = userInfo.Permission.PermissionId }) > 0 || GetExistCount("select count(*) from blogpermission where permissionName = @pName", new { pName = userInfo.Permission.PermissionName }) <= 0 || GetExistCount("select count(*) from bloguserinfo where [Name]=@Name", new { Name = userInfo.Name }) > 0)
                    {
                        return (false, "用户相关信息已经存在");
                    }
                    var usrNum = conn.Execute("set identity_insert bloguserinfo on;insert into bloguserinfo(userId,[Name],userName,password,IsEnabled) values(@uid,@Name,@uName,@pwd,@isEnabled);set identity_insert bloguserinfo off", new { uid = userInfo.UserId, Name=userInfo.Name,uName = userInfo.UserName, pwd = userInfo.Password, isEnabled = userInfo.IsEnabled }, tran);
                    var upNum = 0;
                    upNum = conn.Execute("insert into bloguser_permission(userId,permissionId) values(@uid,@pid)", new { uid = userInfo.UserId, pid = userInfo.Permission.PermissionId }, tran);
                    if (usrNum > 0 && upNum > 0)
                    {
                        tran.Commit();
                        return (true, "添加用户成功");
                    }
                    else
                    {
                        tran.Rollback();
                        return (false, "添加用户失败");
                    }
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return (false, ex.Message);
                }
            }
        }
        public async Task<(bool IsAdd, string msg)> AddAsync(BlogUserInfo userInfo)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                var tran = conn.BeginTransaction();
                if (userInfo == null || userInfo.Permission == null)
                {
                    return (false, "用户添加信息不完整，添加失败。");
                }
                try
                {
                    if (GetExistCount("select count(*) from bloguser_permission where userId=@uid and permissionId = @pid ", new { uid = userInfo.UserId, pid = userInfo.Permission.PermissionId }) > 0 || GetExistCount("select count(*) from blogpermission where permissionName = @pName", new { pName = userInfo.Permission.PermissionName }) <= 0 || GetExistCount("select count(*) from bloguserinfo where [Name]=@Name", new { Name = userInfo.Name }) > 0)
                    {
                        return (false, "用户添加失败");
                    }
                    var usrNum =await conn.ExecuteAsync("set identity_insert bloguserinfo on;insert into bloguserinfo(userId,[Name],userName,password,IsEnabled) values(@uid,@Name,@uName,@pwd,@isEnabled);set identity_insert bloguserinfo off", new { uid = userInfo.UserId,Name=userInfo.Name, uName = userInfo.UserName, pwd = userInfo.Password, isEnabled = userInfo.IsEnabled }, tran);
                    var upNum = 0;
                    upNum = await conn.ExecuteAsync("insert into bloguser_permission(userId,permissionId) values(@uid,@pid)", new { uid = userInfo.UserId, pid = userInfo.Permission.PermissionId }, tran);
                    if (usrNum > 0 && upNum > 0)
                    {
                        tran.Commit();
                        return (true, "添加用户成功");
                    }
                    else
                    {
                        tran.Rollback();
                        return (false, "添加用户失败");
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
