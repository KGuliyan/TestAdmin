using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace WebApplication1.DataAccess
{
    
        public class SqlDataAccess<T>
        {
            public List<T> Result { get; set; }
            //public T Model { get; set; }
            public int rowsAffected { get; set; }

            public void Request(string sql, object param = null )
            {
                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString))
                {
                    Result = db.Query<T>(sql, param).ToList();
                }
            }

            public void ChangeInfo(string sql, object param)
            {
                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString))
                {
                        rowsAffected = db.Execute(sql, param);
                }
                return;
            }

        }
    }