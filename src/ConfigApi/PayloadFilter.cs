using System.Diagnostics;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Newtonsoft.Json;

namespace ConfigApi
{
    public class Header
    {
        public long QTime { get; set; }
    }

    public class Payload
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Response { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Header Header { get; set; }
    }

    public class PayloadFilter : ActionFilterAttribute
    {
        public static readonly Task CompletedTask = Task.FromResult(false);

        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            actionContext.Request.Properties["QTime"] = Stopwatch.StartNew();

            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }

        public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            /*
            var content = (ObjectContent) actionExecutedContext.ActionContext.Response.Content;
            var wrapped =
                new ObjectContent<Payload>(new Payload {Response = content?.Value, Header = sw != null ? new Header {QTime = sw.ElapsedMilliseconds} : null},
                    content.Formatter);
                    */

            if (actionExecutedContext.Response != null)
            {
                var sw = actionExecutedContext.Request.Properties["QTime"] as Stopwatch;
                if (sw != null)
                    actionExecutedContext.Response.Headers.Add("x-yapi-qtime", sw.ElapsedMilliseconds.ToString());

                actionExecutedContext.Response.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
            }
            return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }
    }
}
