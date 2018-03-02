using System.Web.Http;
using Ninject;
using Owin;
using WebApiContrib.IoC.Ninject;

namespace ProjectAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var kernel = new StandardKernel();
            kernel.Bind<IMessageGenerator>().ToConstant(new FakeMessageGenerator());

            var httpConfiguration = new HttpConfiguration {DependencyResolver = new NinjectResolver(kernel)};

            WebApiConfig.Register(httpConfiguration);
            app.UseWebApi(httpConfiguration);
        }
    }
}
