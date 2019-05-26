using System.Linq;
using System.Reflection;
using System.Web.Http;
using Abp.Application.Services;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.WebApi;
using Swashbuckle.Application;
using System.Linq;
using Abp.Configuration.Startup;
using Swashbuckle.Application;
using System.IO;
using Newtonsoft.Json.Serialization;

namespace LgWG.LogQuery.Api
{
    [DependsOn(typeof(AbpWebApiModule), typeof(LogQueryApplicationModule))]
    public class LogQueryWebApiModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            Configuration.Modules.AbpWebApi().DynamicApiControllerBuilder
                .ForAll<IApplicationService>(typeof(LogQueryApplicationModule).Assembly, "app")
                .Build();

            Configuration.Modules.AbpWebApi().HttpConfiguration.Filters.Add(new HostAuthenticationFilter("Bearer"));

            //配置输出json时不使用骆驼式命名法，按对象属性原名输出
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new DefaultContractResolver();
            //集成Swagger UI
            ConfigureSwaggerUi();

        }

        private void ConfigureSwaggerUi()
        {
            var xmlFile = string.Format("{0}/bin/{1}.Application.xml", System.AppDomain.CurrentDomain.BaseDirectory, this.GetType().Namespace);
            xmlFile = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"bin\LgWG.LogQuery.Application.xml");
            if (System.IO.File.Exists(xmlFile))
            {

            }
            Configuration.Modules.AbpWebApi().HttpConfiguration
            .EnableSwagger(c =>
            {
                c.SingleApiVersion("v1", this.GetType().Namespace);
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                if (System.IO.File.Exists(xmlFile)) { c.IncludeXmlComments(xmlFile); }
            })
            .EnableSwaggerUi();
        }

    }



}

