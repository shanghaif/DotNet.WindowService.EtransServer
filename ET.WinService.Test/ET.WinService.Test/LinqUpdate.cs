using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.Winservice.Test
{
    public static class LinqUpdates
    {

        public static void Update<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

    }

    public class Person
    {

        public string Name { get; set; }
        public double Salary { get; set; }
        public bool isHip { get; set; }
    }

    public class Test
    {

        public string Name { get; set; }
        public double Salary { get; set; }
        public bool isHip { get; set; }
        public int Type { get; set; }
    }
}
