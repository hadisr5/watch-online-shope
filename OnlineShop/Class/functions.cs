using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Web;
using OnlineShop.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Globalization;
using System.Data.Entity.Core.EntityClient;
using System.Text;

namespace OnlineShop.Class
{
    public class functions
    {
        OnlineShopEntities db = new OnlineShopEntities();
        public Boolean dangerous(HttpRequestBase request)
        {
            var lst = new List<string>();
            lst.Add(";");
            lst.Add(",");
            lst.Add("\"");
            lst.Add("%");
            lst.Add("\'");
            lst.Add("--");
            lst.Add("_");
            lst.Add("--");
            lst.Add("/");
            lst.Add("\\");
            lst.Add("xp_");
            lst.Add("Drop");
            lst.Add("xp_");
            bool flag = false;
            if (lst.Any(s => request.Params["search[value]"].Contains(s)) || lst.Any(s => request.Params["order[0][dir]"].Contains(s)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
      
        public List<string> getColumnName(System.Web.HttpRequestBase req)
        {
            var colums = new List<string>();
            int i = 0;
            Boolean flg = false;
            while (!flg)
            {
                var newVal = req.Params["columns[" + i + "][data]"];
                i++;
                if (string.IsNullOrEmpty(newVal))
                {
                    flg = true;
                }
                else
                {
                    colums.Add(newVal);
                }
            }
            return colums;
        }

        public class Result
        {
            public string content { get; set; }
            public int total { get; set; }
        }

        public Result CreateTable(string tbName, List<string> bans, System.Web.HttpRequestBase request)
        {
            if (dangerous(request))
            {
                var dn = new Result() ;
                dn.content = "false";
                dn.total = 0;
                return dn;
            }
            int length =Convert.ToInt32( request.Params["length"]);
            int start = Convert.ToInt32( request.Params["start"]);
            int draw = Convert.ToInt32( request.Params["draw"]);
            int sort = Convert.ToInt32(request.Params["order[0][column]"]);
            string search = request.Params["search[value]"]; 
            string sortDir = request.Params["order[0][dir]"].ToUpper();
            int total = 0; 




            var columns = getColumnName(request);
            string result = string.Empty;


            try
            {
                SqlParameter sq1 = new SqlParameter();
                sq1.DbType = DbType.Int32;
                sq1.Value = 2;

            }
            catch (Exception)
            {

                throw;
            }

            var fields = DynamicListFromSql(db, "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" + tbName + "'", new Dictionary<string, object> { }).ToList();
            var fieldList = new List<string>();

            foreach (var item in fields)
            {
                foreach (var v in (item as IDictionary<string, object>))
                {
                    string key = v.Key;
                    object value = v.Value;

                    if (key == "COLUMN_NAME")
                    {
                        string val = (value.ToString());
                        fieldList.Add(val);
                    }
                }

            }

            var list = new List<Type>();

            List<dynamic> results = new List<dynamic>();
            if (string.IsNullOrEmpty(search))
            {
                total = Convert.ToInt32( DynamicListFromSql(db, "select * from " + tbName + " order by [" + columns[sort] + "] " + sortDir , new Dictionary<string, object> { }).ToList().Count);

                results = DynamicListFromSql(db, "select * from " + tbName + " order by [" + columns[sort] + "] "+ sortDir + " offset " + start + " rows FETCH NEXT " + length + " rows only", new Dictionary<string, object> { }).ToList();
            }
            else
            {
                string FeildsCmd = "";
                if (bans!=null && bans.Count != 0)
                {
                    foreach (var item in bans)
                    {
                        fieldList.Remove(item);
                    }
                }
                if (fieldList.Count != 0)
                {
                    int c = fieldList.Count; 
                    foreach (var item in fieldList)
                    {
                        c--;
                        if (c!=0)
                        {
                            FeildsCmd += item + " LIKE N'%" + search+"%' OR ";

                        }
                        else
                        {
                            FeildsCmd += item + " LIKE N'%" + search+ "%' order by ["+columns[sort]+ "] " + sortDir + " offset " + start+" rows FETCH NEXT "+length+" rows only";

                        }
                    }
                }

                total = DynamicListFromSql(db, "select * from " + tbName + " WHERE "+FeildsCmd.Replace(" offset " + start + " rows FETCH NEXT " + length + " rows only" , string.Empty), new Dictionary<string, object> { }).ToList().Count;
                results = DynamicListFromSql(db, "select * from " + tbName + " WHERE "+ FeildsCmd, new Dictionary<string, object> { }).ToList();

            }

            string jsonString = JsonConvert.SerializeObject(results);

            //results = results;
            //var omid =  JArray.FromObject(results).ToString();

            //foreach (var item in results)
            //{
            //    foreach (var v in (item as IDictionary<string, object>))
            //    {
            //        string key = v.Key;
            //        object value = v.Value;
            //    }

            //}
            Result res = new Result();
            res.content = jsonString;
            res.total = total; 
            //res.total
            return res ;
        }
        public static IEnumerable<dynamic> DynamicListFromSql(DbContext db, string Sql, Dictionary<string, object> Params)
        {
            using (var cmd = db.Database.Connection.CreateCommand())
            {
                cmd.CommandText = Sql;
                if (cmd.Connection.State != ConnectionState.Open) { cmd.Connection.Open(); }

                foreach (KeyValuePair<string, object> p in Params)
                {
                    DbParameter dbParameter = cmd.CreateParameter();
                    dbParameter.ParameterName = p.Key;
                    dbParameter.Value = p.Value;
                    cmd.Parameters.Add(dbParameter);
                }

                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        var row = new ExpandoObject() as IDictionary<string, object>;
                        for (var fieldCount = 0; fieldCount < dataReader.FieldCount; fieldCount++)
                        {
                            row.Add(dataReader.GetName(fieldCount), dataReader[fieldCount]);
                        }
                        yield return row;
                    }
                }
            }
        }

       
    }

    public static class Utility
    {

        public static string SazmanJoghrafia
        {
            get
            {
                return "سازمان جغرافیای نیروهای مسلح";
            }
        }
        public static int NumberExistInSite { get; set; }
        public static string DaysOfWeek(DayOfWeek d)
        {
            if (DayOfWeek.Saturday == d) return "شنبه";
            if (DayOfWeek.Sunday == d) return "یکشنبه";
            if (DayOfWeek.Monday == d) return "دوشنبه";
            if (DayOfWeek.Tuesday == d) return "سه شنبه";
            if (DayOfWeek.Wednesday == d) return "چهارشنبه";
            if (DayOfWeek.Thursday == d) return "پنجشنبه";
            if (DayOfWeek.Friday == d) return "جمعه";
            else return "نامشخص";
        }
        //public static ViewModels.LastPostUpdate LastUpdateData()
        //{
        //    OnlineShopEntities db = new OnlineShopEntities();
        //    //ViewModels.LastPostUpdate lastPostUpdate = new ViewModels.LastPostUpdate();


        //    var News = db.News.OrderByDescending(q => q.creationDate).FirstOrDefault();


        //    ViewModels.LastPostUpdate lastPostUpdate = new ViewModels.LastPostUpdate()
        //    {
        //        Date = /*News.creationDate.GetPersianWeekDayName() +" "+*/ convertToPersianDate(News.creationDate),
        //        Title = News.title1,
        //        LastPostUpdateId = News.id
        //    };

        //    return lastPostUpdate;
        //}
        public static DbConnection GetSqlConnection()
        {
            // Initialize the EntityConnectionStringBuilder. 
            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();

            var connectionSettings = Properties.Settings.Default.ServerConnectionString;

            string providerName = "System.Data.SqlClient";
            // Set the provider-specific connection string. 
            entityBuilder.Provider = providerName;
            entityBuilder.ProviderConnectionString = connectionSettings;

            // Set the Metadata location. 
            entityBuilder.Metadata = "res://*/Models.Model1.csdl|res://*/Models.Model1.ssdl|res://*/Models.Model1.msl";

            return new EntityConnection(entityBuilder.ToString());
        }
        public static string convertToPersianDate(DateTime? dt)
        {
            //PersianDateTime persianDateTime = new PersianDateTime(dt);
            var ressss = dt;

            //string GregorianDate = "Thursday, October 24, 2013";
            DateTime d = Convert.ToDateTime(dt);
            PersianCalendar pc = new PersianCalendar();
            var res = string.Format("{0}/{1}/{2}", pc.GetYear(d), pc.GetMonth(d), pc.GetDayOfMonth(d));


            //PersianCalendar pc = new PersianCalendar();
            DateTime thisDate = Convert.ToDateTime(dt);
            var shamsyDate = string.Format("{0},{3}/{1}/{2} {4}:{5}:{6}", DaysOfWeek(pc.GetDayOfWeek(thisDate)),
                      pc.GetMonth(thisDate),
                      pc.GetDayOfMonth(thisDate),
                      pc.GetYear(thisDate),
                      pc.GetHour(thisDate),
                      pc.GetMinute(thisDate),
                      pc.GetSecond(thisDate));
            return shamsyDate;

        }
        public static int CountVisitor(this int day)
        {
            OnlineShopEntities db1 = new OnlineShopEntities();
            var countsVisit = db1.VisitStatistics.Where(o =>
    DbFunctions.DiffDays(o.DateInput, DateTime.Now) <= day).ToList();

            return countsVisit.Count();
        }
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
        public static string DecryptPassword(string password)
        {
            try
            {
                var result = Security.RsaEncryptDecrypt.RSACls.RSADecryption(password, Properties.Settings.Default.PrivateKey);
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        
        }
        public static string EncryptPassword(string password)
        {
            try
            {
                var result = Security.RsaEncryptDecrypt.RSACls.RSAEncryption(password, Properties.Settings.Default.PublicKey);
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public static void CreateLog(Exception ex, string action = null, string controller = null)
        {
            using (var db = new OnlineShopEntities())
            {
                Models.Log log = new Log()
                {
                    Action = "",
                    Controller = controller,
                    DateCreated = DateTime.Now,
                    message = ex.Message
                };
                db.Logs.Add(log);
                db.SaveChanges();
            }
        }

    }
}