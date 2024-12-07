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
            /*
            SelectData();

            InsertData();
            UpdateData();
            DeleteData();
            */

            SelectAndCrud();

            //CrudTransaction();

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

        private void SelectAndCrud()
        {
            /*
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["pdam_Entities"].ConnectionString;

            // Inisialisasi layanan database
            var dbService = new DatabaseService(connectionString);
            */

            // Inisialisasi layanan database
            var dbService = new DatabaseService();

            // **INSERT Data**
            dbService.ExecuteNonQuery(
                @"
                INSERT INTO comm.tr_class (group_code, class_code, class_desc, meter_size_code)
                VALUES (@group_code, @class_code, @class_desc, @meter_size_code)
                ",
                cmd =>
                {
                    dbService.AddParameter(cmd, "@group_code", NpgsqlTypes.NpgsqlDbType.Smallint, 6);
                    dbService.AddParameter(cmd, "@class_code", NpgsqlTypes.NpgsqlDbType.Varchar, "zzz");
                    dbService.AddParameter(cmd, "@class_desc", NpgsqlTypes.NpgsqlDbType.Varchar, "ZZZZZZ");
                    dbService.AddParameter(cmd, "@meter_size_code", NpgsqlTypes.NpgsqlDbType.Smallint, null);  // NULL
                }
            );

            // **UPDATE Data**
            dbService.ExecuteNonQuery(
                @"
                UPDATE comm.tr_class
                SET class_desc = @class_desc
                WHERE group_code = @group_code AND class_code = @class_code
                ",
                cmd =>
                {
                    dbService.AddParameter(cmd, "@group_code", NpgsqlTypes.NpgsqlDbType.Smallint, 6);
                    dbService.AddParameter(cmd, "@class_code", NpgsqlTypes.NpgsqlDbType.Varchar, "zzz");
                    dbService.AddParameter(cmd, "@class_desc", NpgsqlTypes.NpgsqlDbType.Varchar, "Updated Description");
                }
            );

            // **DELETE Data**
            dbService.ExecuteNonQuery(
                @"
                DELETE FROM comm.tr_class
                WHERE group_code = @group_code AND class_code = @class_code
                ",
                cmd =>
                {
                    dbService.AddParameter(cmd, "@group_code", NpgsqlTypes.NpgsqlDbType.Smallint, 6);
                    dbService.AddParameter(cmd, "@class_code", NpgsqlTypes.NpgsqlDbType.Varchar, "zzz");
                }
            );

            // SELECT Data dengan ORDER BY Dinamis - ALTERNATIF ORDER BY - 1
            var dataList1 = dbService.ExecuteQuery(
                @"
                SELECT
                group_code, class_code, class_desc, meter_size_code
                
                FROM comm.tr_class
                
                WHERE 1 = 1
                and group_code = @group_code
                --and class_code = @class_code
                
                ORDER BY 
                    CASE 
                        WHEN @order_by = 'group_code' AND @order_dir = 'ASC' THEN CAST(group_code AS TEXT)
                        WHEN @order_by = 'class_code' AND @order_dir = 'ASC' THEN class_code 
                        WHEN @order_by = 'class_desc' AND @order_dir = 'ASC' THEN class_desc 
                        WHEN @order_by = 'meter_size_code' AND @order_dir = 'ASC' THEN CAST(meter_size_code AS TEXT)
                    END ASC,
                    CASE 
                        WHEN @order_by = 'group_code' AND @order_dir = 'DESC' THEN CAST(group_code AS TEXT)
                        WHEN @order_by = 'class_code' AND @order_dir = 'DESC' THEN class_code 
                        WHEN @order_by = 'class_desc' AND @order_dir = 'DESC' THEN class_desc 
                        WHEN @order_by = 'meter_size_code' AND @order_dir = 'DESC' THEN CAST(meter_size_code AS TEXT)
                    END DESC
                
                OFFSET @offset LIMIT @limit
                ",
                cmd =>
                {
                    dbService.AddParameter(cmd, "@group_code", NpgsqlTypes.NpgsqlDbType.Smallint, 6);
                    dbService.AddParameter(cmd, "@class_code", NpgsqlTypes.NpgsqlDbType.Varchar, "zzz");
                    dbService.AddParameter(cmd, "@offset", NpgsqlTypes.NpgsqlDbType.Integer, 0);
                    dbService.AddParameter(cmd, "@limit", NpgsqlTypes.NpgsqlDbType.Integer, 10);

                    // Parameter dinamis untuk ORDER BY
                    dbService.AddParameter(cmd, "@order_by", NpgsqlTypes.NpgsqlDbType.Varchar, "class_code");
                    dbService.AddParameter(cmd, "@order_dir", NpgsqlTypes.NpgsqlDbType.Varchar, "DESC");
                },
                reader => new
                {
                    GroupCode = reader.GetInt16(reader.GetOrdinal("group_code")),
                    ClassCode = reader.GetString(reader.GetOrdinal("class_code")),
                    ClassDesc = reader.GetString(reader.GetOrdinal("class_desc")),
                    MeterSizeCode = reader.IsDBNull(reader.GetOrdinal("meter_size_code")) ?
                                    (int?)null :
                                    reader.GetInt32(reader.GetOrdinal("meter_size_code"))
                }
            );

            // Cetak hasilnya
            foreach (var item in dataList1)
            {
                Console.WriteLine($"Group: {item.GroupCode}, Class: {item.ClassCode}, Desc: {item.ClassDesc}, Meter Size: {item.MeterSizeCode}");
            }

            // SELECT Data dengan ORDER BY Dinamis - ALTERNATIF ORDER BY - 2
            string orderByColumn = "group_code";
            string orderByDirection = "ASC";
            string query = $@"
                SELECT
                group_code, class_code, class_desc, meter_size_code
                
                FROM comm.tr_class
                
                WHERE 1 = 1
                and group_code = @group_code
                --and class_code = @class_code
                
                ORDER BY {orderByColumn} {orderByDirection}
                
                OFFSET @offset LIMIT @limit
            ";

            var dataList2 = dbService.ExecuteQuery(
                query,
                cmd =>
                {
                    dbService.AddParameter(cmd, "@group_code", NpgsqlTypes.NpgsqlDbType.Smallint, 6);
                    dbService.AddParameter(cmd, "@class_code", NpgsqlTypes.NpgsqlDbType.Varchar, "zzz");
                    dbService.AddParameter(cmd, "@offset", NpgsqlTypes.NpgsqlDbType.Integer, 0);
                    dbService.AddParameter(cmd, "@limit", NpgsqlTypes.NpgsqlDbType.Integer, 10);
                },
                reader => new
                {
                    GroupCode = reader.GetInt16(reader.GetOrdinal("group_code")),
                    ClassCode = reader.GetString(reader.GetOrdinal("class_code")),
                    ClassDesc = reader.GetString(reader.GetOrdinal("class_desc")),
                    MeterSizeCode = reader.IsDBNull(reader.GetOrdinal("meter_size_code")) ?
                                    (int?)null :
                                    reader.GetInt32(reader.GetOrdinal("meter_size_code"))
                }
            );

            // Cetak hasilnya
            foreach (var item in dataList2)
            {
                Console.WriteLine($"Group: {item.GroupCode}, Class: {item.ClassCode}, Desc: {item.ClassDesc}, Meter Size: {item.MeterSizeCode}");
            }






            // WHERE yang dinamis

            // Contoh filter dinamis
            var filters = new Dictionary<string, (NpgsqlTypes.NpgsqlDbType, object)>
            {
                { "group_code", (NpgsqlTypes.NpgsqlDbType.Smallint, 6) }//,
                //{ "class_code", (NpgsqlTypes.NpgsqlDbType.Varchar, "xxx") },
                //{ "class_desc", (NpgsqlTypes.NpgsqlDbType.Varchar, "xxx") }
                // Anda bisa menambahkan lebih banyak filter di sini
            };

            // CONTOH TIDAK ADA FILTER --> DATA DIAMBIL SEMUA
            //var filters = new Dictionary<string, (NpgsqlTypes.NpgsqlDbType, object)>
            //{
            //};

            // Parameter dinamis untuk ORDER BY
            string orderByColumn3 = "group_code"; // Kolom yang ingin diurutkan
            string orderByDirection3 = "ASC"; // Arah pengurutan

            // Membuat query dinamis dengan ORDER BY dan WHERE yang fleksibel
            string query3 = @"
                SELECT
                group_code, class_code, class_desc, meter_size_code
                
                FROM comm.tr_class
                
                WHERE 1 = 1
            ";

            // Menambahkan kondisi WHERE berdasarkan filter dinamis
            foreach (var filter in filters)
            {
                query3 += $" AND {filter.Key} = @{filter.Key}";
            }

            // Menambahkan ORDER BY dinamis
            string query3Full = query3 + $@"
                ORDER BY {orderByColumn3} {orderByDirection3}
                
                OFFSET @offset LIMIT @limit
            ";

            // Menjalankan query dengan parameter dinamis
            var dataList3 = dbService.ExecuteQuery(
                query3Full,
                cmd =>
                {
                    // Menambahkan parameter dinamis ke dalam command
                    foreach (var filter in filters)
                    {
                        dbService.AddParameter(cmd, $"@{filter.Key}", filter.Value.Item1, filter.Value.Item2);
                    }

                    // Menambahkan parameter offset dan limit
                    dbService.AddParameter(cmd, "@offset", NpgsqlTypes.NpgsqlDbType.Integer, 0);
                    dbService.AddParameter(cmd, "@limit", NpgsqlTypes.NpgsqlDbType.Integer, 10);
                },
                reader =>
                {
                    // Mengambil data dari reader
                    return new
                    {
                        GroupCode = reader.GetInt16(reader.GetOrdinal("group_code")),
                        ClassCode = reader.GetString(reader.GetOrdinal("class_code")),
                        ClassDesc = reader.GetString(reader.GetOrdinal("class_desc")),
                        MeterSizeCode = reader.IsDBNull(reader.GetOrdinal("meter_size_code")) ?
                                        (int?)null :
                                        reader.GetInt32(reader.GetOrdinal("meter_size_code"))
                    };
                }
            );

            // Tampilkan data hasil query (contoh)
            foreach (var item in dataList3)
            {
                Console.WriteLine($"GroupCode: {item.GroupCode}, ClassCode: {item.ClassCode}, ClassDesc: {item.ClassDesc}");
            }

            // COUNT ALL
            string query4 = @"
                SELECT
                group_code, class_code, class_desc, meter_size_code
                
                FROM comm.tr_class
                
                WHERE 1 = 1
            ";

            var countAll = dbService.ExecuteQuery<int>(
                query4,
                cmd =>
                {
                },
                reader => reader.GetInt32(0) // Mengambil nilai COUNT(*) dari kolom pertama (index 0)
            );

            Console.WriteLine($"Total count: {countAll.FirstOrDefault()}");

            // COUNT WITH FILTER
            var countFilter = dbService.ExecuteQuery<int>(
                query3,
                cmd =>
                {
                    // Menambahkan parameter dinamis ke dalam command
                    foreach (var filter in filters)
                    {
                        dbService.AddParameter(cmd, $"@{filter.Key}", filter.Value.Item1, filter.Value.Item2);
                    }
                },
                reader => reader.GetInt32(0) // Mengambil nilai COUNT(*) dari kolom pertama (index 0)
            );

            Console.WriteLine($"Total count: {countFilter.FirstOrDefault()}");
        }

        private void CrudTransaction()
        {
            var dbService = new DatabaseService();

            try
            {
                dbService.ExecuteTransaction(cmd =>
                {
                    // Operasi INSERT
                    cmd.CommandText = @"
                        INSERT INTO comm.tr_class (group_code, class_code, class_desc, meter_size_code)
                        VALUES (@group_code, @class_code, @class_desc, @meter_size_code)
                    ";

                    dbService.AddParameter(cmd, "@group_code", NpgsqlTypes.NpgsqlDbType.Smallint, 6);
                    dbService.AddParameter(cmd, "@class_code", NpgsqlTypes.NpgsqlDbType.Varchar, "zzz");
                    dbService.AddParameter(cmd, "@class_desc", NpgsqlTypes.NpgsqlDbType.Varchar, "Sample Desc");
                    dbService.AddParameter(cmd, "@meter_size_code", NpgsqlTypes.NpgsqlDbType.Smallint, DBNull.Value);

                    cmd.ExecuteNonQuery();

                    // Operasi UPDATE
                    cmd.CommandText = @"
                        UPDATE comm.tr_class
                        SET class_desc = @update_class_desc
                        WHERE group_code = @update_group_code
                        AND class_code = @update_class_code
                    ";

                    dbService.AddParameter(cmd, "@update_group_code", NpgsqlTypes.NpgsqlDbType.Smallint, 6);
                    dbService.AddParameter(cmd, "@update_class_code", NpgsqlTypes.NpgsqlDbType.Varchar, "zzz");
                    dbService.AddParameter(cmd, "@update_class_desc", NpgsqlTypes.NpgsqlDbType.Varchar, "1234567890123456789012345678901234567890"); // MAX 30 CHARS

                    cmd.ExecuteNonQuery();
                });

                Console.WriteLine("Transaction completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
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
