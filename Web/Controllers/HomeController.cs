using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            SelectData();

            InsertData();
            UpdateData();
            DeleteData();

            return View();
        }

        private void SelectData()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["pdam_Entities"].ConnectionString;

            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();

            NpgsqlCommand cmd = new NpgsqlCommand();

            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.Text; // BARIS INI OPTIONAL (BISA TIDAK DIPAKAI)
            cmd.CommandText = @"
select
a.group_code,
a.class_code,
a.class_desc,
a.meter_size_code

from comm.tr_class a

where 1 = 1
and a.group_code = @group_code --> MENGGUNAKAN PARAMETER
--and a.class_code = @class_code --> MENGGUNAKAN PARAMETER

order by a.group_code, a.class_code --> ORDER

--offset 20 limit 10 --> PAGING --> MULAI DARI BARIS 21 SEBANYAK 10 BARIS
offset @offset limit @limit --> PAGING --> MULAI DARI BARIS @offset + 1 SEBANYAK @limit BARIS
";
            
            /*
            NpgsqlParameter param1 = new NpgsqlParameter("@group_code", NpgsqlTypes.NpgsqlDbType.Smallint)
            {
                Value = (Int16)1 // ASSIGN A VALUE TO THE @group_code PARAMETER
            };
            cmd.Parameters.Add(param1);

            NpgsqlParameter param2 = new NpgsqlParameter("@class_code", NpgsqlTypes.NpgsqlDbType.Varchar)
            {
                Value = "1A" // ASSIGN A VALUE TO THE @class_code PARAMETER
            };
            cmd.Parameters.Add(param2);

            NpgsqlParameter param3 = new NpgsqlParameter("@offset", NpgsqlTypes.NpgsqlDbType.Integer)
            {
                Value = 0 // SET YOUR OFFSET VALUE (STARTING ROW, E.G., ROW 1)
            };
            cmd.Parameters.Add(param3);

            NpgsqlParameter param4 = new NpgsqlParameter("@limit", NpgsqlTypes.NpgsqlDbType.Integer)
            {
                Value = 10 // SET YOUR LIMIT VALUE (NUMBER OF ROWS PER PAGE)
            };
            cmd.Parameters.Add(param4);
            */

            NpgsqlParameter paramValue = null;

            paramValue = new NpgsqlParameter("@group_code", NpgsqlTypes.NpgsqlDbType.Smallint);
            paramValue.Value = (Int16)1; // ASSIGN A VALUE TO THE @group_code PARAMETER
            cmd.Parameters.Add(paramValue);

            paramValue = new NpgsqlParameter("@class_code", NpgsqlTypes.NpgsqlDbType.Varchar);
            paramValue.Value = "1A"; // ASSIGN A VALUE TO THE @class_code PARAMETER
            cmd.Parameters.Add(paramValue);

            paramValue = new NpgsqlParameter("@offset", NpgsqlTypes.NpgsqlDbType.Integer);
            paramValue.Value = 0; // SET YOUR OFFSET VALUE (STARTING ROW, E.G., ROW 1)
            cmd.Parameters.Add(paramValue);

            paramValue = new NpgsqlParameter("@limit", NpgsqlTypes.NpgsqlDbType.Integer);
            paramValue.Value = 10; // SET YOUR LIMIT VALUE (NUMBER OF ROWS PER PAGE)
            cmd.Parameters.Add(paramValue);

            try
            {
                NpgsqlDataReader sdr = cmd.ExecuteReader();

                IList<CommTrClass> lst = new List<CommTrClass>();

                while (sdr.Read())
                {
                    CommTrClass itm = new CommTrClass();

                    itm.group_code = Convert.ToInt16(sdr["group_code"]);
                    itm.class_code = sdr["class_code"].ToString();
                    itm.class_desc = sdr["class_desc"].ToString();

                    if (sdr["meter_size_code"] != DBNull.Value)
                        itm.meter_size_code = Convert.ToInt32(sdr["meter_size_code"]);
                    else
                        itm.meter_size_code = null;

                    lst.Add(itm);
                }

                // ORDER BY WITH LINQ - QUERY SYNTAX
                IList<CommTrClass> lst2 = (
                    from p in lst
                    orderby p.class_code descending
                    select p
                ).ToList();

                // ORDER BY WITH LINQ - METHOD SYNTAX
                IList<CommTrClass> lst3 = (
                    lst.OrderByDescending(p => p.class_code)
                ).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void InsertData()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["pdam_Entities"].ConnectionString;

            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();

            NpgsqlCommand cmd = new NpgsqlCommand();

            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.Text; // BARIS INI OPTIONAL (BISA TIDAK DIPAKAI)
            cmd.CommandText = @"
insert into comm.tr_class
values (@group_code, @class_code, @class_desc, @meter_size_code)
";

            /*
            NpgsqlParameter paramValue1 = new NpgsqlParameter("@group_code", NpgsqlTypes.NpgsqlDbType.Smallint)
            {
                Value = (Int16)6 // ASSIGN A VALUE TO THE @group_code PARAMETER
            };
            cmd.Parameters.Add(paramValue1);

            NpgsqlParameter paramValue2 = new NpgsqlParameter("@class_code", NpgsqlTypes.NpgsqlDbType.Varchar)
            {
                Value = "zzz" // ASSIGN A VALUE TO THE @class_code PARAMETER
            };
            cmd.Parameters.Add(paramValue2);

            NpgsqlParameter paramValue3 = new NpgsqlParameter("@class_desc", NpgsqlTypes.NpgsqlDbType.Varchar)
            {
                Value = "ZZZZZZ" // ASSIGN A VALUE TO THE @class_desc PARAMETER
            };
            cmd.Parameters.Add(paramValue3);

            NpgsqlParameter paramValue14 = new NpgsqlParameter("@meter_size_code", NpgsqlTypes.NpgsqlDbType.Smallint)
            {
                Value = null // ASSIGN A VALUE TO THE @meter_size_code PARAMETER
            };
            cmd.Parameters.Add(paramValue14);
            */

            NpgsqlParameter paramValue = null;

            paramValue = new NpgsqlParameter("@group_code", NpgsqlTypes.NpgsqlDbType.Smallint);
            paramValue.Value = (Int16)6;
            cmd.Parameters.Add(paramValue);

            paramValue = new NpgsqlParameter("@class_code", NpgsqlTypes.NpgsqlDbType.Varchar);
            paramValue.Value = "zzz";
            cmd.Parameters.Add(paramValue);

            paramValue = new NpgsqlParameter("@class_desc", NpgsqlTypes.NpgsqlDbType.Varchar);
            paramValue.Value = "ZZZZZZ";
            cmd.Parameters.Add(paramValue);

            paramValue = new NpgsqlParameter("@meter_size_code", NpgsqlTypes.NpgsqlDbType.Smallint);
            paramValue.Value = DBNull.Value;
            cmd.Parameters.Add(paramValue);

            NpgsqlTransaction trans = conn.BeginTransaction();

            try
            {
                int rowsAffected = cmd.ExecuteNonQuery();

                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
            }
        }

        private void UpdateData()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["pdam_Entities"].ConnectionString;

            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();

            NpgsqlCommand cmd = new NpgsqlCommand();

            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.Text; // BARIS INI OPTIONAL (BISA TIDAK DIPAKAI)
            cmd.CommandText = @"
update comm.tr_class
set
class_desc = @class_desc --> PARAMETER VALUE
where 1 = 1
and group_code = @group_code --> MENGGUNAKAN PARAMETER
and class_code = @class_code --> MENGGUNAKAN PARAMETER
";

            /*
            NpgsqlParameter paramCriteria1 = new NpgsqlParameter("@group_code", NpgsqlTypes.NpgsqlDbType.Smallint)
            {
                Value = (Int16)6 // ASSIGN A VALUE TO THE @group_code PARAMETER
            };
            cmd.Parameters.Add(paramCriteria1);

            NpgsqlParameter paramCriteria2 = new NpgsqlParameter("@class_code", NpgsqlTypes.NpgsqlDbType.Varchar)
            {
                Value = "zzz" // ASSIGN A VALUE TO THE @class_code PARAMETER
            };
            cmd.Parameters.Add(paramCriteria2);

            NpgsqlParameter paramValue = new NpgsqlParameter("@class_desc_value", NpgsqlTypes.NpgsqlDbType.Varchar)
            {
                Value = "XXXyyyZZZ" // ASSIGN A VALUE TO THE @class_code_value PARAMETER
            };
            cmd.Parameters.Add(paramValue);
            */

            NpgsqlParameter paramValue = null;

            paramValue = new NpgsqlParameter("@group_code", NpgsqlTypes.NpgsqlDbType.Smallint);
            paramValue.Value = (Int16)6;
            cmd.Parameters.Add(paramValue);

            paramValue = new NpgsqlParameter("@class_code", NpgsqlTypes.NpgsqlDbType.Varchar);
            paramValue.Value = "zzz";
            cmd.Parameters.Add(paramValue);

            paramValue = new NpgsqlParameter("@class_desc", NpgsqlTypes.NpgsqlDbType.Varchar);
            paramValue.Value = "XXXyyyZZZ";
            cmd.Parameters.Add(paramValue);

            paramValue = new NpgsqlParameter("@meter_size_code", NpgsqlTypes.NpgsqlDbType.Smallint);
            paramValue.Value = DBNull.Value;
            cmd.Parameters.Add(paramValue);

            NpgsqlTransaction trans = conn.BeginTransaction();

            try
            {
                int rowsAffected = cmd.ExecuteNonQuery();

                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
            }
        }

        private void DeleteData()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["pdam_Entities"].ConnectionString;

            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();

            NpgsqlCommand cmd = new NpgsqlCommand();

            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.Text; // BARIS INI OPTIONAL (BISA TIDAK DIPAKAI)
            cmd.CommandText = @"
delete from comm.tr_class
where 1 = 1
and group_code = @group_code --> MENGGUNAKAN PARAMETER
and class_code = @class_code --> MENGGUNAKAN PARAMETER
";

            /*
            NpgsqlParameter paramValue1 = new NpgsqlParameter("@group_code", NpgsqlTypes.NpgsqlDbType.Smallint)
            {
                Value = (Int16)6 // ASSIGN A VALUE TO THE @group_code PARAMETER
            };
            cmd.Parameters.Add(paramValue1);

            NpgsqlParameter paramValue2 = new NpgsqlParameter("@class_code", NpgsqlTypes.NpgsqlDbType.Varchar)
            {
                Value = "zzz" // ASSIGN A VALUE TO THE @class_code PARAMETER
            };
            cmd.Parameters.Add(paramValue2);
            */

            NpgsqlParameter paramValue = null;

            paramValue = new NpgsqlParameter("@group_code", NpgsqlTypes.NpgsqlDbType.Smallint);
            paramValue.Value = (Int16)6;
            cmd.Parameters.Add(paramValue);

            paramValue = new NpgsqlParameter("@class_code", NpgsqlTypes.NpgsqlDbType.Varchar);
            paramValue.Value = "zzz";
            cmd.Parameters.Add(paramValue);

            NpgsqlTransaction trans = conn.BeginTransaction();

            try
            {
                int rowsAffected = cmd.ExecuteNonQuery();

                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
