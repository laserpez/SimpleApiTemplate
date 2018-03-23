using System.Configuration;
using System.Web.Http;
using Ninject;
using Owin;
using WebApiContrib.IoC.Ninject;

namespace ConfigApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var configStorageDb = ConfigurationManager.ConnectionStrings["ConfigStorageDb"].ConnectionString;

            var kernel = new StandardKernel();
            kernel.Bind<IStorage>()
                .ToConstructor(ca => new ConfigurationStorage(configStorageDb))
                .InSingletonScope();

            var httpConfiguration = new HttpConfiguration {DependencyResolver = new NinjectResolver(kernel)};
            httpConfiguration.Filters.Add(new PayloadFilter());
            WebApiConfig.Register(httpConfiguration);
            app.UseWebApi(httpConfiguration);
        }
    }
}
