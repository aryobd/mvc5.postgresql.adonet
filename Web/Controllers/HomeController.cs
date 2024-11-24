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
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["pdam_Entities"].ConnectionString;

            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();

            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"
select
a.group_code,
a.class_code,
a.class_desc,
a.meter_size_code

from comm.tr_class a

where a.group_code = @group_code --> MENGGUNAKAN PARAMETER

order by a.group_code, a.class_code --> ORDER

--offset 20 limit 10 --> PAGING --> MULAI DARI BARIS 21 SEBANYAK 10 BARIS
";
            //var param = new NpgsqlParameter("@group_code", NpgsqlTypes.NpgsqlDbType.Smallint) { Value = (Int16)1 };
            //cmd.Parameters.Add(param);

            NpgsqlParameter param1 = new NpgsqlParameter("@group_code", NpgsqlTypes.NpgsqlDbType.Smallint) { Value = (Int16)1 };
            cmd.Parameters.Add(param1);

            //NpgsqlTransaction trans = conn.BeginTransaction();

            try
            {
                NpgsqlDataReader sdr = cmd.ExecuteReader();
                List<CommTrClass> lst = new List<CommTrClass>();

                while (sdr.Read())
                {
                    CommTrClass itm = new CommTrClass();
                    itm.group_code = Convert.ToInt32(sdr["group_code"]);
                    itm.class_code = sdr["class_code"].ToString();
                    itm.class_desc = sdr["class_desc"].ToString();

                    if (sdr["meter_size_code"] != DBNull.Value)
                        itm.meter_size_code = Convert.ToInt32(sdr["meter_size_code"]);
                    else
                        itm.meter_size_code = null;

                    lst.Add(itm);
                }

                //trans.Commit();
            }
            catch (Exception ex)
            {
                //trans.Rollback();

                return View(ex.Message);
            }

            return View();
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
