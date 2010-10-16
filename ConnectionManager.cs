using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace PowerPointController
{
    class ConnectionManager
    {
        public ConnectionManager()
        {
            listener.Prefixes.Add("http://+:1989/");
            CacheResources();
        }

        private void CacheResources()
        {
            var asm = typeof(ConnectionManager).Assembly;
            Array.ForEach(new[] { "navigation.html" },
                x => resourceCache[x] = typeof(ConnectionManager).Assembly.GetManifestResourceStream("PowerPointController." + x).ToArray());
        }


        Dictionary<string, byte[]> resourceCache = new Dictionary<string, byte[]>();

        private HttpListener listener = new HttpListener();

        public void StartListening()
        {
            listener.Start();
            listener.BeginGetContext(DispatchRequest, null);
        }

        private void DispatchRequest(IAsyncResult result)
        {
            try {
                var requestContext = listener.EndGetContext(result);
                listener.BeginGetContext(DispatchRequest, null);
                ProcessRequest(requestContext);
            } catch { }

        }

        private void ProcessRequest(HttpListenerContext requestContext)
        {
            switch (requestContext.Request.Url.LocalPath) {
                //case "/iphonenav.js":
                //    SendResource(requestContext.Response, "iphonenav.js");
                //    break;
                //case "/iphonenav.css":
                //    SendResource(requestContext.Response, "iphonenav.css");
                //    break;
                case "/":
                    switch (requestContext.Request.QueryString["action"]) {
                        case "next":
                            InvokeTransition(TransitionType.NextSlide);
                            break;
                        case "prev":
                            InvokeTransition(TransitionType.PreviousSlide);
                            break;
                        default:
                            SendNavigation(requestContext.Response);
                            break;
                    }
                    break;
                default:
                    requestContext.Response.StatusDescription = "Not found";
                    requestContext.Response.StatusCode = 404;
                    break;
            }
            requestContext.Response.Close();
        }

        private void SendResource(HttpListenerResponse response, string name)
        {
            var resource = resourceCache[name];
            response.OutputStream.Write(resource, 0, resource.Length);
            response.OutputStream.Flush();
        }

        private void SendNavigation(HttpListenerResponse response)
        {
            response.AddHeader("Cache-Control", "No-Cache");
            SendResource(response, "navigation.html");
        }

        private void InvokeTransition(TransitionType transitionType)
        {
            try {
                if (TransitionOccurred != null)
                    TransitionOccurred(this, new TransitionEventArgs(transitionType));
            } catch { /* eat the exception */ }
        }

        public event EventHandler<TransitionEventArgs> TransitionOccurred;

        public void StopListening()
        {
            listener.Stop();
        }
    }
}
