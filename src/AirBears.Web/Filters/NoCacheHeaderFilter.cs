using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AirBears.Web.Filters
{
    public sealed class NoCacheHeaderFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext?.HttpContext?.Response == null) { return; }
            if (filterContext?.HttpContext?.Request?.Method != HttpMethod.Get.Method) { return; }

            filterContext.HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate"); // HTTP 1.1.
            filterContext.HttpContext.Response.Headers.Add("Pragma", "no-cache"); // HTTP 1.0.
            filterContext.HttpContext.Response.Headers.Add("Expires", "-1"); // Proxies.

            base.OnResultExecuting(filterContext);
        }
    }
}
