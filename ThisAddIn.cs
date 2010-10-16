using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Office = Microsoft.Office.Core;

namespace PowerPointController
{
    public partial class ThisAddIn
    {
        private ConnectionManager connectionManager;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            connectionManager = new ConnectionManager();
            connectionManager.TransitionOccurred += new EventHandler<TransitionEventArgs>(ProcessTransition);
            connectionManager.StartListening();
        }

        private readonly object lc = new object();
        private void ProcessTransition(object sender, TransitionEventArgs e)
        {
            lock (lc) {
                try {
                    var x = this.Application.SlideShowWindows[1];
                    if (x != null)
                        if (e.Transition == TransitionType.NextSlide)
                            x.View.Next();
                        else
                            x.View.Previous();
                } catch {
                    var p =((PowerPoint.Slide)this.Application.ActiveWindow.View.Slide).SlideIndex;
                    if (e.Transition == TransitionType.NextSlide)
                        p++;
                    else
                        p--;
                    this.Application.ActiveWindow.View.GotoSlide(p);
                }
            }
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            try {
                if (connectionManager != null)
                    connectionManager.StopListening();
            } catch { }
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
