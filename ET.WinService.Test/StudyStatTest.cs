using ET.WinService.StudyStatistics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

using ET.WinService.StudyStatistics.Entity;
using ET.WinService.Core.Utility;
using ET.WinService.StudyStatistics;
using ET.WinService.StudyStatistics.DAL;

namespace ET.WinService.Test
{

    /// <summary>
    /// 单元测试
    /// </summary>
    /// <remarks>
    /// ------------------------------------------------------------------------------
    /// Copyright:Copyright (c) 2013,广州亿程交通信息有限公司 All rights reserved.
    /// 描  述：
    /// 版本号：1.0.0.1
    /// 作  者：黄冠群 (hgq@e-trans.com.cn)
    /// 日  期：2013年1月17日
    /// 修  改：
    /// 原  因：
    /// ------------------------------------------------------------------------------
    /// </remarks>
    /// <example>
    /// [示例代码在这里写入]
    /// </example>
    [TestClass()]
    public class StudyStatTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        // 
        //编写测试时，还可使用以下特性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod]
        public void MyTest()
        {
            TestForEach();
            TestGetLisByTeachers();
            return;

            bool b1 = DateTime.Now.TimeOfDay > TimeSpan.Parse("08:00:00");
            bool b2 = DateTime.Now.TimeOfDay < TimeSpan.Parse("12:30:00");
            int dr = 4 & 4;
            int r = 1 & 4;
            int w = 2 & 4;
            int wd = 3 & 4;
            InvalidType typ = (InvalidType)Enum.Parse(typeof(InvalidType), "8192");

        }

        private void TestGetLisByTeachers()
        {
            try
            {
                StudyRecordResultDAL dal = new StudyRecordResultDAL();
                string teachers = "18,19,20,21,22,23,24,25";
                ArrayList recordIDs = dal.GetRecordIDByTeacher(teachers);
                if (recordIDs.Contains(23))
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void TestForEach()
        {
            List<TempStudyRecord> lists = new List<TempStudyRecord>(){
                new TempStudyRecord{ StudyRecordID=1,StuID=2,SubjectID=3, SubjectName="aaa"},
                new TempStudyRecord{StudyRecordID=2,StuID=3,SubjectID=3,SubjectName="bbb"}
            };

            lists.RemoveAll(o => o.StudyRecordID == 1);

           var temp= lists.Where(o => o.StudyRecordID == 2).Select(o => o.StudyRecordID);

            //这个可以起修改作用
            lists.ForEach(o =>
            {
                if (o.StuID == 2)
                {
                    o.SubjectID = 0;
                }
            });

            List<int> datas = new List<int>() { 1, 2, 3, 4 };
            //下面的不起作用
            datas.ForEach(o => o += 1);
        }
        /// <summary>
        ///StudyStat 构造函数 的测试
        ///</summary>
        [TestMethod()]
        public void StudyStatConstructorTest()
        {
            StudyStat target = new StudyStat();
            target.Execute();
        }
    }
}
