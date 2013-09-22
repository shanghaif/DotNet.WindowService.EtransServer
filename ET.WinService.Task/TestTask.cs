using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using ET.WinService.Core.Task;
using ET.WinService.Core.Utility;

namespace ET.WinService.Task
{
    public class TestTask:TriggerTimeTask
    {
        public TestTask(XElement xelem) : base(xelem) { }

        public override void Execute(object state)
        {
            try
            {
                log.Info(string.Format("任务执行时间：{0}",DateTime.Now.ToString()));
                ET.WinService.StudyStatistics.StudyStat st = new StudyStatistics.StudyStat();
                st.Execute();
            }
            catch (Exception ex)
            {
                log.Error("任务执行出错！", ex);
            }
        }

        public override string SetLoggerName()
        {
            return "StudyStat";
        }
    }
}
