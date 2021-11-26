using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Security
{
    public static class SettingApp
    {
      /// <summary>
      /// زمانی که کاربر ادمین برای مدت لاگین بودن کاربر مشخص میکند
      /// </summary>
        public static int _TimeoutLogin { get; set; }
        

    }
}