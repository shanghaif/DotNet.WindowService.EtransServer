using ET.WinService.StudyStatistics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Dynamic;

using ET.WinService.StudyStatistics.Entity;
using ET.WinService.Core.Utility;
using ET.WinService.StudyStatistics;
using ET.WinService.StudyStatistics.DAL;
using ET.WinService.Core.Extension;

namespace ET.Winservice.Test
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
            double d1 = Math.Round(3.64);
            DateTime dt = DateTime.Now;
            DateTime dt2 = dt.AddSeconds(-1);
            testcDatediff();
            TimeSpan ts = TimeSpan.Parse("08:00");
            TestLinqUpdate();
            TestDAL();
            TestBase();

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

        private void testcDatediff()
        {
            //select DATEDIFF(ss,'2013-03-15 12:20:23','2013-03-15 12:30:23')
            DateTime dt1=Convert.ToDateTime("2013-03-15 12:20:23");
            DateTime dt2=Convert.ToDateTime("2013-03-15 12:30:23");
            TimeSpan ts1 = new TimeSpan(dt2.Ticks - dt1.Ticks);
            long ss = dt1.DateDiff(DateInterval.Second, dt2); //(long)ts1.TotalSeconds;
        }

        private void TestDAL()
        {
            VehMapDAL dal = new VehMapDAL();
            DataSourceDAL dDal = new DataSourceDAL();
            List<VehMap> list = dal.GetLisByDeviceNos("HB_JP_GPSDB","'80005447','80000524','80000871'");
            string dbName = dDal.GetDbNameByCode("GPSDB");
        }

        private void TestBase()
        {
            /*
             * declare @t NUMERIC(12,4)
                select @t=ROUND(2.64536,0,1)
                print @t
                select @t=ROUND(2.64536,0,0)
                print @t
             * */
            //Math.Round采用的是国际通行的是 Banker 舍入法.
           double d1= Math.Round(3.44, 1); //Returns 3.4.  四舍

           double d2= Math.Round(3.451, 1); //Returns 3.5  五后非零就进一
           double d3 = Math.Round(3.45, 1); //Returns 3.4. 五后皆零看奇偶, 五前为偶应舍 去

           double d4 = Math.Round(3.75, 1);  //Returns 3.8  五后皆零看奇偶,五前为奇要进一
            double d5 = Math.Round(3.46, 1); //Returns 3.5. 六入

            double t1 = Math.Round((50 - 20) / 60.0);
            double t2 = Math.Round((70 - 20) / 60.0);

            DateTime snow = DateTime.Now;
            DateTime ss = snow.AddSeconds(1);
        }

        private void TestObject()
        {
            dynamic obj = new ExpandoObject();
            obj.Firstname = "bolo";
            obj.Lastname = "rock";
            
        }

        private void TestLinqUpdate()
        {
            var people = new List<Person> {
                new Person{Name="aaa", Salary=15000, isHip=false}
                ,new Person{Name="bbb", Salary=20000, isHip=false}
                ,new Person{Name="aaa3", Salary=15000, isHip=false}
                ,new Person{Name="ccc", Salary=25000, isHip=false}
                ,new Person{Name="ddd", Salary=30000, isHip=false}
                ,new Person{Name="eee", Salary=35000, isHip=false}
            };

            people.RemoveAll(o => o.Salary > 20000);

            var pl = new List<Test> {
                new Test{Name="aaa", Salary=15000, isHip=false,Type=1}
                ,new Test{Name="bbb", Salary=20000, isHip=false,Type=0}
                ,new Test{Name="c", Salary=20000, isHip=false,Type=1}
                ,new Test{Name="c", Salary=25000, isHip=false,Type=1}
                ,new Test{Name="dd", Salary=30000, isHip=false,Type=1}
                ,new Test{Name="eee", Salary=35000, isHip=false,Type=1}
            };

            (from t1 in people
             join t2 in pl
             on t1.Name equals t2.Name
            // where t2.isHip == false && (t2.Type == 1 || t2.Type == 0)
            where !(t2.Salary<20000)
             select new { t1, gg=t2.Type })
                      .Update(item =>
                          {
                              if (item.gg == 1)
                              {
                                  item.t1.Salary = 1;
                              }
                              if (item.gg == 0)
                              {
                                  item.t1.Salary = 0;
                              }
                          });


            var tese = people.Skip(1);

            var list = people.Where(o => o.Salary < 25000).GroupBy(o => o.Name);


            var tp = people.Where(p => p.Salary < 3000);
            if (tp.Count() > 0)
            {
                tp.Update(p => p.isHip = true);
            }
            else
            {
                people.Add(new Person
                {
                    Name="bolo",
                    Salary=12000,
                    isHip=true
                });
            }

            foreach (var p in people)
            {
                Console.WriteLine("{0} - {1}", p.Name, p.isHip);
            }
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

            var temp = lists.Where(o => o.StudyRecordID == 2).Select(o => o.StudyRecordID);

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
           // StudyStat target = new StudyStat();
           // target.Execute();
        }
    }
}
