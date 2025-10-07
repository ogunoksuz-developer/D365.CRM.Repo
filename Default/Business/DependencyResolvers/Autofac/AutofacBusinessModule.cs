using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using LCW.Business.Concrete;
using LCW.Core.DataAccess.CDSWebApi;
using LCW.Core.Utilities.Interceptors.Autofac;
using LCW.Core.Utilities.Security.Jwt;
using LCW.DataAccess.Abstract;
using LCW.DataAccess.Concrete.AdoNet;
using LCW.DataAccess.Concrete.CrmWebApi;
using LCW.Interfaces.Abstract;

namespace LCW.Business.DependencyResolvers.Autofac
{
    public class AutofacBusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterType<Service>().As<IHttpClientService>();

            builder.RegisterType<StoreManager>().As<IStoreService>();
            builder.RegisterType<WAStoreDal>().As<IStoreDal>();

            builder.RegisterType<StoreScheduleSettingManager>().As<IStoreScheduleSettingService>();
            builder.RegisterType<WAStoreScheduleSettingDal>().As<IStoreScheduleSettingDal>();

            builder.RegisterType<ANStoreScheduleDal>().As<IStoreScheduleANDal>();

            builder.RegisterType<LiveApiManager>().As<ILiveApiService>();

            builder.RegisterType<Service>().As<IHttpClientService>();
         
            builder.RegisterType<JwtHelper>().As<ITokenHelper>();

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces()
                .EnableInterfaceInterceptors(new ProxyGenerationOptions()
                {
                    Selector = new AspectInterceptorSelector()
                }).SingleInstance();
        }

    }
}
