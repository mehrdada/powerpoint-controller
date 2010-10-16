using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerPointController
{
    public class TransitionEventArgs : EventArgs
    {
        public TransitionType Transition { get; private set; }
        public TransitionEventArgs(TransitionType type)
        {
            Transition = type;
        }
    }
}
