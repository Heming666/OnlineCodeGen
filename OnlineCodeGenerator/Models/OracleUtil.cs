using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Oracle.ManagedDataAccess.Client;

namespace OnlineCodeGenerator.Models
{
    public class OracleUtil
    {
        private static readonly OracleUtil util = new OracleUtil();
        private static string connStr = "";
        public static OracleUtil Util
        {
            get
            {
                return util;
            }
        }

        public void GetDatabases(DBModel db)
        {

            BuildConnStr(db);
            string sql = "select a.TABLE_NAME, B.COMMENTS from user_tables a left join USER_TAB_COMMENTS b on a.TABLE_NAME=b.TABLE_NAME Order by a.TABLE_NAME asc ";
            db.Database = db.IP + "@" + db.UserName;
            db.DbList.Add(new SelectListItem { Value = db.Database, Text = db.Database, Selected = true });
            DataTable dt  =ExcuteDataTable(sql);
            if (dt.Rows != null && dt.Rows.Count > 0)
            {
                var enumers = dt.Rows.GetEnumerator();
                while (enumers.MoveNext())
                {
                    DataRow dr = enumers.Current as DataRow;
                    db.TableList.Add(new SelectListItem
                    {
                        Value = dr["TABLE_NAME"].ToString(),
                        Text = dr["COMMENTS"] is DBNull ? dr["TABLE_NAME"].ToString() : dr["TABLE_NAME"].ToString() + $"({dr["COMMENTS"].ToString()})",
                        Selected = string.Equals(db.Database, dr["TABLE_NAME"].ToString(), System.StringComparison.OrdinalIgnoreCase)
                    });
                }
            }
        }

        public Poco BuildPoco(DBModel db)
        {
            BuildConnStr(db);
            object obj = ExcuteScalar($"SELECT COMMENTS FROM USER_TAB_COMMENTS WHERE TABLE_NAME='{db.Table}'");
            Poco poco = new Poco
            {
                NameSpace = db.CodeNameSpace,
                ClassName = db.Table,
                ClassComment = obj==null ? "" : obj.ToString()
            };

            string sql = $@"select A.TABLE_NAME,A.COLUMN_NAME,B.COMMENTS,DATA_TYPE,NULLABLE from user_tab_columns
                                    A LEFT JOIN USER_COL_COMMENTS B ON A.COLUMN_NAME=B.COLUMN_NAME where A.table_name='{db.Table}' AND B.TABLE_NAME='{db.Table}'";

            DataTable dt = ExcuteDataTable(sql);
            if (dt.Rows !=null && dt.Rows.Count >0 )
            {
                var enumter = dt.Rows.GetEnumerator();
                while (enumter.MoveNext())
                {
                    DataRow current = enumter.Current as DataRow;
                    PocoProperty property = new PocoProperty();
                    string propertyType = "string";
                    switch (current["DATA_TYPE"].ToString().ToUpper())//数据类型
                    {
                        case "CHAR":
                        case "NCHAR":
                        case "VARCHAR":
                        case "VARCHAR2":
                        case "NVARCHAR2":
                        case "CLOB":
                        case "NCLOB":
                        case "BLOB":
                        case "BFILE":
                        case "LONG":
                            propertyType = "string";
                        
                            break;
                        case "NUMBER":
                        case "BINARY_FLOAT":
                        case "BINARY_DOUBLE":
                        case "FLOAT":
                            propertyType = "decimal";
                            if (current["NULLABLE"].ToString() == "Y")
                            {
                                propertyType += "?";
                            }
                            break;
                        case "DATE":
                            propertyType = "DateTime";
                            if (current["NULLABLE"].ToString() == "Y")
                            {
                                propertyType += "?";
                            }
                            break;
                        case "INTEGER":
                        case "INT":
                            propertyType = "int";
                            if (current["NULLABLE"].ToString() == "Y")
                            {
                                propertyType += "?";
                            }
                            break;
                        default:
                            if (current["DATA_TYPE"].ToString().Contains("TIMESTAMP"))
                            {
                                propertyType = "DateTime";
                                if (current["NULLABLE"].ToString() == "Y")
                                {
                                    propertyType += "?";
                                }
                            }
                            break;
                    }

                    property.Comment = current["COMMENTS"] is DBNull ? string.Empty : current["COMMENTS"].ToString();
                    property.Name = current["COLUMN_NAME"].ToString();
                    property.TypeName = propertyType;
                    poco.Properties.Add(property);
                }
            }
            return poco;
        }



        private void BuildConnStr(DBModel db)
        {
            if (string.IsNullOrWhiteSpace(db.ServerName)) throw new Exception("服务名称不能为空");
            if (string.IsNullOrWhiteSpace(db.UserName)) throw new Exception("用户名不能为空");
            if (string.IsNullOrWhiteSpace(db.Password)) throw new Exception("密码不能为空");
            if (string.IsNullOrWhiteSpace(db.IP)) throw new Exception("IP不能为空");
            connStr = $"User Id={db.UserName};Password={db.Password};Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={db.IP})(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME={db.ServerName})))";
        }

        #region  数据库处理类
        private DataTable ExcuteDataTable(string sql, params OracleParameter[] parameter)
        {
            DataTable dt = new DataTable();
            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();
                using (OracleCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Parameters.AddRange(parameter);
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    da.Fill(dt);

                }
            }
            return dt;
        }

        private DataTable ExcuteDataTable(string sql)
        {
            DataTable dt = new DataTable();
            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();
                using (OracleCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    da.Fill(dt);

                }
            }
            return dt;
        }

        private object ExcuteScalar(string sql, params OracleParameter[] parameter)
        {
            DataTable dt = new DataTable();
            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();
                using (OracleCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Parameters.AddRange(parameter);
                    var a = cmd.ExecuteScalar();
                    object reuslt = cmd.ExecuteScalarAsync().Result;
                    return reuslt;
                }
            }
        }

        private object ExcuteScalar(string sql)
        {
            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();
                using (OracleCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    object reuslt = cmd.ExecuteScalarAsync().Result;
                    return reuslt;
                }
            }
        }
        #endregion
    }
}
