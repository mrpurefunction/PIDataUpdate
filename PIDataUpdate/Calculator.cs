using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIDataUpdate
{
    /// <summary>
    /// provide static methods to calculate total amount of date retrieving
    /// </summary>
    public class Calculator
    {
        //private static int count_unitmin_real = 1;
        //private static int count_unithour_avg = 1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointsnum"></param>
        /// <param name="st"></param>
        /// <param name="et"></param>
        /// <returns></returns>
        public static long GetRealCount(int pointsnum,DateTime st,DateTime et)
        {
            int diff = (int)(et - st).TotalMinutes + 1;
            return diff * pointsnum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointsnum"></param>
        /// <param name="st"></param>
        /// <param name="et"></param>
        /// <returns></returns>
        public static long GetAvgCount(int pointsnum, DateTime st, DateTime et)
        {
            int diff = (int)(DateTime.Parse(et.ToString("yyyy-MM-dd HH:00:00")) - DateTime.Parse(st.ToString("yyyy-MM-dd HH:00:00"))).TotalHours + 1;
            return diff * pointsnum;
        }
    }
}
