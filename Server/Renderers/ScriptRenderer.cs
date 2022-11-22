/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Bam.Net.Web;
using Bam.Net.Presentation.Html;
using System.Reflection;
using System.Collections.Concurrent;
using Bam.Net.Server.PathHandlers;
using Bam.Net.Server.ServiceProxy;

namespace Bam.Net.Server.Renderers
{
    public class ScriptRenderer: ContentRenderer
    {
        ConcurrentDictionary<string, byte[]> _cache;
        ConcurrentDictionary<string, byte[]> _minCache;
        public ScriptRenderer(ServiceProxyInvocation request, ContentResponder content)
            : base(request, content, "application/javascript", Extensions)
        {
            this._cache = new ConcurrentDictionary<string, byte[]>();
            this._minCache = new ConcurrentDictionary<string,byte[]>();
            string path = request.Request.Url.AbsolutePath;

/*            if (!request.WasExecuted)
            {
                request.Execute();
            }*/

            if (request.Result is AppMetaResult result)
            {
                request.Result = result.Data;
            }

            //HandlePrependAndPostpend();

            string script = request.Result as string;
            if (script == null)
            {
                string type = "null";
                
                if (request.Result != null)
                {
                    type = request.Result.GetType().Name;
                    request.Result = script;
                }

                script = ";\r\nalert('expected a script but was ({0}) instead');"._Format(type);
            }
            Task.Run(() => content.SetScriptCache(path, script));
            SetResult();
        }

        public static new string[] Extensions { get; } = new string[] { ".js", ".jsonp", ".min" };

        private void SetResult()
        {
            string path = ServiceProxyInvocation.Request.Url.AbsolutePath;
            ServiceProxyInvocation.Result = _cache[path];
            /*
            if (!string.IsNullOrEmpty(ServiceProxyInvocation.Ext) && ServiceProxyInvocation.Ext.Equals(".min"))
            {
                ServiceProxyInvocation.Result = _minCache[path];
            }
            else
            {
                ServiceProxyInvocation.Result = _cache[path];
            }*/
        }

/*        protected virtual void HandlePrependAndPostpend()
        {
            *//*string ext = ServiceProxyInvocation.Ext;
            // if ext is jsonp
            if (!string.IsNullOrEmpty(ext) && ext.ToLowerInvariant().Equals(".jsonp"))
            {
                HandleJsonp();
            }
            else *//*
            if (ServiceProxyInvocation.HasCallback)
            {
                Postpend("\r\n;{0}"._Format(ServiceProxyInvocation.Callback));
            }
        }*/
/*
        protected virtual void HandleJsonp()
        {
            string callBack = ServiceProxyInvocation.HasCallback ? ServiceProxyInvocation.Callback : "alert";
            Prepend("{0}('"._Format(callBack));
            Postpend("');\r\n");
        }*/
/*
        protected virtual void Prepend(string prepend)
        {
            StringBuilder newResult = new StringBuilder();
            newResult.Append(prepend);
            newResult.Append(ServiceProxyInvocation.Result);
            
            ServiceProxyInvocation.Result = newResult.ToString();
        }*/

/*        protected virtual void Postpend(string postpend)
        {
            StringBuilder newResult = new StringBuilder();
            newResult.Append(ServiceProxyInvocation.Result);
            newResult.Append(postpend);

            ServiceProxyInvocation.Result = newResult.ToString();
        }*/

        public override void Render(object toRender, Stream output)
        {
            Expect.AreSame(ServiceProxyInvocation.Result, toRender, "Attempt to render unexpected value");
            byte[] data = toRender as byte[];
            if (data == null)
            {
                string renderString = toRender as string;
                data = Encoding.UTF8.GetBytes(renderString);
            }

            output.Write(data, 0, data.Length);
        }
    }
}
