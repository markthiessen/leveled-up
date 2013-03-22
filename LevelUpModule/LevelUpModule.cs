using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace LevelUp
{
    public class LevelUpModule : IHttpModule
    {
        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.PostRequestHandlerExecute += context_BeginRequest;
        }

        void context_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;
            if (app.Context.CurrentHandler is MvcHandler)
            {
                var handler = app.Context.CurrentHandler as MvcHandler;
                handler.RequestContext.HttpContext.Response.Filter = new AddScriptFilter(handler.RequestContext.HttpContext.Response.Filter);
            }
        }

    }
}
