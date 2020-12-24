using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Classes
{
    public static class Values
    {
        public static string CheckFields(string[] sql, string frstValue, string scndValue, string thrdValue)
        {
            string str = " ";

            str = sql[0];


            if (!String.IsNullOrEmpty(frstValue))
            {
                str += sql[1] + frstValue;

            }


            if (!String.IsNullOrEmpty(scndValue) && String.IsNullOrEmpty(frstValue))
            {
                str += sql[2] + scndValue;

            }
            else if (!String.IsNullOrEmpty(scndValue) && !String.IsNullOrEmpty(frstValue))
            {
                str += "," + sql[2] +  scndValue;

            }


            if (!String.IsNullOrEmpty(thrdValue) && String.IsNullOrEmpty(scndValue) && String.IsNullOrEmpty(frstValue))
            {
                str += sql[3] + thrdValue;

            }
            else if (!String.IsNullOrEmpty(thrdValue) && (!String.IsNullOrEmpty(scndValue) || !String.IsNullOrEmpty(frstValue)))
            {
                str += ", " + sql[3] + thrdValue;

            }

            str += sql[4];

            return str;
        }
    }
}