/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Bam.Net.ServiceProxy;
using Bam.Net.Presentation;
using Bam.Net.Server.ServiceProxy;

namespace Bam.Net.Server.Renderers
{
    public abstract class WebRenderer: IWebRenderer
    {
        public WebRenderer(string contentType, params string[] extensions)
        {
            this.Extensions = extensions;
            this.ContentType = contentType;
            this.OutputStream = new MemoryStream();
        }

        public virtual ContentResponder ContentResponder
        {
            get;
            set;
        }

        public string[] Extensions
        {
            get;
            set;
        }

        public string ContentType
        {
            get;
            set;
        }

        public virtual void SetContentType(IResponse response)
        {
            response.ContentType = ContentType;
        }

        public Stream OutputStream { get; set; }

        public virtual void Render(string templateName, object toRender, Stream output)
        {
            // This implementation discards the templateName; required to implement IRenderToStream
            Render(toRender, output);
        }

        public virtual void Render(object toRender)
        {
            Render(toRender, OutputStream);
        }

        public abstract void Render(object toRender, Stream output);

        /// <summary>
        /// Sets the content type then calls Render(request.Result, request.Response.OutputStream);
        /// </summary>
        /// <param name="request"></param>
        public virtual void Respond(ServiceProxyInvocation request)
        {
            IResponse response = request.Response;
            object toRender = request.Result;
            SetContentType(response);
            Render(toRender, response.OutputStream);
        }

        public virtual void EnsureDefaultTemplate(Type type) { }        
        
        public string Render(string templateName, object data)
        {
            MemoryStream memoryStream = new MemoryStream();
            Render(templateName, data, memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream.ReadToEnd();
        }

    }
}
