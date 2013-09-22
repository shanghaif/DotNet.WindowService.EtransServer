using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using ET.WinService.Core.Task;

namespace ET.WinService.Task
{
    public class MutilTask : TriggerTimeTask
    {
        public MutilTask(XElement xelem) : base(xelem) { }

        public override void Execute(object state)
        {
            try
            {
                log.Info(string.Format("任务执行时间：{0}", DateTime.Now.ToString()));
            }
            catch (Exception ex)
            {
                log.Error("任务执行出错", ex);
            }
        }
    }
}
