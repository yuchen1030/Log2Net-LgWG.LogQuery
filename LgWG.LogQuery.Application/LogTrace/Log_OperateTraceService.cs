using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Caching;
using LgWG.LogQuery.Authorization;
using LgWG.LogQuery.Authorization.Roles;
using LgWG.LogQuery.LogTrace.DTO;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace LgWG.LogQuery.LogTrace
{
    [AbpAuthorize(PermissionNames.Pages_LogTrace)]
    public class Log_OperateTraceService : LogQueryAppServiceBase, ILog_OperateTraceService
    {
        private readonly ILog_OperateTraceRepository _logTraceRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly ICacheManager _cacheManager;
        /// <summary>
        /// 构造函数自动注入
        /// </summary>
        /// <param name="logTraceRepository"></param>
        public Log_OperateTraceService(ILog_OperateTraceRepository logTraceRepository, IRepository<UserRole, long> userRoleRepository, IRepository<Role> roleRepository, ICacheManager cacheManager) : base(userRoleRepository, roleRepository, cacheManager)
        {
            _logTraceRepository = logTraceRepository;
        }



        public IQueryable<Log_OperateTrace> GetLogTracesQuery(GetLogTraceInput input)
        {
            var enumList = GetUserSysCategoryAccordingRoleID();
            var query = _logTraceRepository.GetAll();
            query = query.WhereIf(input.LogType != Log2Net.Models.LogType.所有, a => a.LogType == input.LogType);
            query = query.WhereIf(input.StartT != new DateTime(), a => a.LogTime >= input.StartT & a.LogTime < input.EndT);

            if (!input.SystemID.Contains(Log2Net.Models.SysCategory.ALL))
            {
                query = query.Where(a => input.SystemID.Contains(a.SystemID));
            }
            else
            {
                query = query.Where(a => enumList.Contains(a.SystemID));
            }

            query = query.WhereIf(!string.IsNullOrEmpty(input.ServerHost), a => a.ServerHost == input.ServerHost);
            query = query.WhereIf(!string.IsNullOrEmpty(input.ServerIP), a => a.ServerIP == input.ServerIP);
            query = query.WhereIf(!string.IsNullOrEmpty(input.UserName), a => a.UserName.Contains(input.UserName));
            query = query.WhereIf(!string.IsNullOrEmpty(input.ModuTable), a => a.TabOrModu.ToLower().Contains(input.ModuTable.ToLower()));
            query = query.WhereIf(!string.IsNullOrEmpty(input.KeyWord), a => a.Detail.ToLower().Contains(input.KeyWord.ToLower()));
            query = query.WhereIf(input.Express != null, input.Express);
            return query;



        }




        /// <summary>
        /// 获取轨迹日志（具体实现）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public GetLogTraceOutput GetLogTraces(GetLogTraceInput input)
        {
            var query = GetLogTracesQuery(input);
            GetLogTraceOutput getLogTraceOutput = new GetLogTraceOutput();

            var totalCount = query.Count();
            input.Sorting = input.Sorting.Replace("SysName", "SystemID").Replace("LogTypeCN", "LogType");
            var list = query.OrderBy(input.Sorting).PageBy(input).ToList();

            var listDtos = list.MapTo<List<Log_OperateTraceDto>>();
            getLogTraceOutput.Items = listDtos;
            getLogTraceOutput.TotalCount = totalCount;
            getLogTraceOutput = GetCategorySummary(input.StartT.ToShortDateString(), input.EndT.ToShortDateString(), input, getLogTraceOutput);

            return getLogTraceOutput;

        }


        //获取按服务器，系统、日志类型、用户分类的信息
        GetLogTraceOutput GetCategorySummary(string startT, string endT, GetLogTraceInput input, GetLogTraceOutput getLogTraceOutput)
        {
            var query = (from d in GetLogTracesQuery(input)
                         select new
                         {
                             ServerHost = d.ServerHost,
                             SystemID = d.SystemID,
                             LogType = d.LogType,
                             UserName = string.IsNullOrEmpty(d.UserName) ? "系统" : d.UserName
                         }).ToList();

            getLogTraceOutput.ErrLogNum = query.Where(a => a.LogType == Log2Net.Models.LogType.异常).Count();
            var serversGroup = query.GroupBy(a => a.ServerHost);
            Dictionary<string, int> dicServer = new Dictionary<string, int>();
            foreach (var item in serversGroup)
            {
                dicServer.Add(item.Key, item.Count());
            }
            NameData serverND = new NameData() { Title = "各服务器的情况", Name = dicServer.Keys.ToArray(), Data = dicServer.Values.ToArray() };

            var sysidsGroup = query.GroupBy(a => a.SystemID);
            Dictionary<string, int> dicSys = new Dictionary<string, int>();
            foreach (var item in sysidsGroup)
            {
                dicSys.Add(Configuration.SysNameMap.GetMySystemName(item.Key), item.Count());
            }
            NameData sysidND = new NameData() { Title = "各网站的情况", Name = dicSys.Keys.ToArray(), Data = dicSys.Values.ToArray() };

            var logTypesGroup = query.GroupBy(a => a.LogType);
            Dictionary<string, int> dicLogType = new Dictionary<string, int>();
            foreach (var item in logTypesGroup)
            {
                dicLogType.Add(item.Key.ToString(), item.Count());
            }
            NameData logTypeND = new NameData() { Title = "各日志类型的情况", Name = dicLogType.Keys.ToArray(), Data = dicLogType.Values.ToArray() };

            var usersGroup = query.GroupBy(a => a.UserName);
            Dictionary<string, int> dicUser = new Dictionary<string, int>();
            foreach (var item in usersGroup)
            {
                dicUser.Add(item.Key, item.Count());
            }
            NameData userND = new NameData() { Title = "各用户的情况", Name = dicUser.Keys.ToArray(), Data = dicUser.Values.ToArray() };

            List<NameData> nameDatas = new List<NameData>() { serverND, sysidND, logTypeND, userND };
            getLogTraceOutput.NameDatas = nameDatas;
            return getLogTraceOutput;
        }

    }
}
