using Skyclient.JsonParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skyclient.Utilities
{
    public class EventUtils
    {
        public delegate void RepoItemSelectedStateChangeEventHandler(RepoItem item, EventArgs e);

        public static event RepoItemSelectedStateChangeEventHandler RepoItemSelectedStateChange;
        public static void RaiseRepoItemChangeSelectedStateEvent(RepoItem item)
        {
            RepoItemSelectedStateChange?.Invoke(item, new EventArgs());
        }
    }
}
