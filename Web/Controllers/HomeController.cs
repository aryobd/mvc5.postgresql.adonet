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
        string _qrySELECT = @"
SELECT
group_code, class_code, class_desc, meter_size_code
FROM comm.tr_class
WHERE 1 = 1
";
        string _qryINSERT = @"
INSERT INTO comm.tr_class (group_code, class_code, class_desc, meter_size_code)
VALUES (@insert_group_code, @insert_class_code, @insert_class_desc, @insert_meter_size_code)
";
        string _qryUPDATE = @"
UPDATE comm.tr_class
SET
class_desc = @update_class_desc
WHERE 1 = 1
AND group_code = @update_group_code
AND class_code = @update_class_code
--AND 1 = 2 --> SENGAJA DIBIKIN TIDAK BERHASIL..!!
";
        string _qryDELETE = @"
DELETE FROM comm.tr_class
WHERE 1 = 1
AND group_code = @delete_group_code
AND class_code = @delete_class_code
";

        public ActionResult Index()
        {
            SelectData();
            CrudTransaction();
            CrudTransaction2();

            return View();
        }

        private void SelectData()
        {
            // INISIALISASI LAYANAN DATABASE
            var dbService = new DatabaseService();

            // CONTOH FILTER / "WHERE" YANG DINAMIS
            var filters = new Dictionary<string, (NpgsqlTypes.NpgsqlDbType, object)>
            {
                { "group_code", (NpgsqlTypes.NpgsqlDbType.Smallint, 6) }//,
                //{ "class_code", (NpgsqlTypes.NpgsqlDbType.Varchar, "xxx") },
                //{ "class_desc", (NpgsqlTypes.NpgsqlDbType.Varchar, "xxx") }
                // ANDA BISA MENAMBAHKAN LEBIH BANYAK FILTER DI SINI
            };

            // CONTOH TIDAK ADA FILTER = DATA DIAMBIL SEMUA
            //var filters = new Dictionary<string, (NpgsqlTypes.NpgsqlDbType, object)>
            //{
            //};

            // MEMBUAT QUERY DINAMIS DENGAN "WHERE" YANG FLEKSIBEL/DINAMIS
            string qryWHERE = "";

            // MENAMBAHKAN KONDISI "WHERE" BERDASARKAN FILTER DINAMIS
            foreach (var filter in filters)
            {
                qryWHERE += $@"
AND {filter.Key} = @{filter.Key}
";
            }

            // PARAMETER DINAMIS UNTUK "ORDER BY"
            string orderByCol = "group_code"; // COLUMN/KOLOM YANG INGIN DIURUTKAN
            string orderByDir = "ASC"; // DIRECTION/ARAH PENGURUTAN

            // MENAMBAHKAN "ORDER BY" DINAMIS
            string qryORDER = $@"
ORDER BY {orderByCol} {orderByDir}
";

            // MENAMBAHKAN "PAGING"
            int offset = 0; // SET YOUR OFFSET VALUE (STARTING ROW, E.G., ROW 1)
            int limit = 10; // SET YOUR LIMIT VALUE (NUMBER OF ROWS PER PAGE)
            string qryPAGING = $@"
OFFSET @offset LIMIT @limit
";

            // MENJALANKAN QUERY DENGAN PARAMETER DINAMIS
            var dataList = dbService.ExecuteQuery(
                _qrySELECT + qryWHERE + qryORDER + qryPAGING,
                cmd =>
                {
                    // MENAMBAHKAN PARAMETER DINAMIS KE DALAM COMMAND
                    foreach (var filter in filters)
                    {
                        dbService.AddParameter(cmd, $"@{filter.Key}", filter.Value.Item1, filter.Value.Item2);
                    }

                    // MENAMBAHKAN PARAMETER PAGING (OFFSET DAN LIMIT)
                    dbService.AddParameter(cmd, "@offset", NpgsqlTypes.NpgsqlDbType.Integer, offset); // SET YOUR OFFSET VALUE (STARTING ROW, E.G., ROW 1)
                    dbService.AddParameter(cmd, "@limit", NpgsqlTypes.NpgsqlDbType.Integer, limit); // SET YOUR LIMIT VALUE (NUMBER OF ROWS PER PAGE)
                },
                reader =>
                {
                    // MENGAMBIL DATA DARI READER
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

            // TAMPILKAN DATA HASIL QUERY (CONTOH)
            foreach (var item in dataList)
            {
                Console.WriteLine($"GroupCode: {item.GroupCode}, ClassCode: {item.ClassCode}, ClassDesc: {item.ClassDesc}");
            }

            // ROW COUNT ALL
            var countALL = dbService.ExecuteQuery<int>(
                _qrySELECT,
                cmd =>
                {
                },
                reader => reader.GetInt32(0) // MENGAMBIL NILAI COUNT(*) DARI KOLOM PERTAMA (INDEX 0)
            );

            Console.WriteLine($"Total count: {countALL.FirstOrDefault()}");

            // ROW COUNT WITH FILTER
            var countFILTER = dbService.ExecuteQuery<int>(
                _qrySELECT + qryWHERE,
                cmd =>
                {
                    // MENAMBAHKAN PARAMETER DINAMIS KE DALAM COMMAND
                    foreach (var filter in filters)
                    {
                        dbService.AddParameter(cmd, $"@{filter.Key}", filter.Value.Item1, filter.Value.Item2);
                    }
                },
                reader => reader.GetInt32(0) // MENGAMBIL NILAI COUNT(*) DARI KOLOM PERTAMA (INDEX 0)
            );

            Console.WriteLine($"Total count: {countFILTER.FirstOrDefault()}");
        }

        private void CrudTransaction()
        {
            var dbService = new DatabaseService();

            dbService.ExecuteTransaction(
                cmd =>
                {
                    // OPERASI "INSERT"
                    cmd.CommandText = _qryINSERT;

                    dbService.AddParameter(cmd, "@insert_group_code", NpgsqlTypes.NpgsqlDbType.Smallint, 6);
                    dbService.AddParameter(cmd, "@insert_class_code", NpgsqlTypes.NpgsqlDbType.Varchar, "1234"); // MAX 4 CHARS
                    dbService.AddParameter(cmd, "@insert_class_desc", NpgsqlTypes.NpgsqlDbType.Varchar, "123456789012345678901234567890"); // MAX 30 CHARS
                    dbService.AddParameter(cmd, "@insert_meter_size_code", NpgsqlTypes.NpgsqlDbType.Smallint, DBNull.Value); // NULL

                    cmd.ExecuteNonQuery();

                    // OPERASI "UPDATE"
                    cmd.CommandText = _qryUPDATE;

                    dbService.AddParameter(cmd, "@update_group_code", NpgsqlTypes.NpgsqlDbType.Smallint, 6);
                    dbService.AddParameter(cmd, "@update_class_code", NpgsqlTypes.NpgsqlDbType.Varchar, "1234"); // MAX 4 CHARS
                    dbService.AddParameter(cmd, "@update_class_desc", NpgsqlTypes.NpgsqlDbType.Varchar, "123456789012345678901234567890"); // MAX 30 CHARS

                    cmd.ExecuteNonQuery();

                    // OPERASI "DELETE"
                    cmd.CommandText = _qryDELETE;

                    dbService.AddParameter(cmd, "@delete_group_code", NpgsqlTypes.NpgsqlDbType.Smallint, 6);
                    dbService.AddParameter(cmd, "@delete_class_code", NpgsqlTypes.NpgsqlDbType.Varchar, "1234"); // MAX 4 CHARS

                    cmd.ExecuteNonQuery();
                }
            );
        }

        private void CrudTransaction2()
        {
            var dbService = new DatabaseService();

            var actions = new List<(string query, Action<NpgsqlCommand> configureParams)>
            {
                (
                    _qryINSERT,
                    cmd =>
                    {
                        dbService.AddParameter(cmd, "@insert_group_code", NpgsqlTypes.NpgsqlDbType.Smallint, 6);
                        dbService.AddParameter(cmd, "@insert_class_code", NpgsqlTypes.NpgsqlDbType.Varchar, "1234");// MAX 4 CHARS
                        dbService.AddParameter(cmd, "@insert_class_desc", NpgsqlTypes.NpgsqlDbType.Varchar, "123456789012345678901234567890"); // MAX 30 CHARS
                        dbService.AddParameter(cmd, "@insert_meter_size_code", NpgsqlTypes.NpgsqlDbType.Smallint, null); // NULL
                    }
                ),
                (
                    _qryUPDATE,
                    cmd =>
                    {
                        dbService.AddParameter(cmd, "@update_group_code", NpgsqlTypes.NpgsqlDbType.Smallint, 6);
                        dbService.AddParameter(cmd, "@update_class_code", NpgsqlTypes.NpgsqlDbType.Varchar, "1234");// MAX 4 CHARS
                        dbService.AddParameter(cmd, "@update_class_desc", NpgsqlTypes.NpgsqlDbType.Varchar, "123456789012345678901234567890"); // MAX 30 CHARS
                    }
                ),
                (
                    _qryDELETE,
                    cmd =>
                    {
                        dbService.AddParameter(cmd, "@delete_group_code", NpgsqlTypes.NpgsqlDbType.Smallint, 6);
                        dbService.AddParameter(cmd, "@delete_class_code", NpgsqlTypes.NpgsqlDbType.Varchar, "1234");// MAX 4 CHARS
                    }
                )
            };

            dbService.ExecuteTransaction(actions);
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
