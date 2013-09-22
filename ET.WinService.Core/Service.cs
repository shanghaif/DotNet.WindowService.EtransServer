using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ET.WinService.Core.Task;

namespace ET.WinService.Core
{
    public class Service
    {
        public string ServiceName { get; set; }
        public string Class { get; set; }
        public string Assembly { get; set; }
        public bool Activate { get; set; }
        public string Status { get; set; }
        public IList<TaskModel> TaskList { get; set; }
    }
}
