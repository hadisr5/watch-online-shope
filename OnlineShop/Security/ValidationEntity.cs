using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using OnlineShop.Security.ViewModels;

namespace OnlineShop.Security
{
    public class ValidationEntity
    {
       
        OnlineShopEntities db = new OnlineShopEntities();
        int _MinuteLockUser = 0;
        int _CountUnsuccessfulattempt = 0;
        public ValidationEntity()
        {
            _MinuteLockUser=int.Parse(db.UserSettings.SingleOrDefault(q => q.Key == "MinuteLockUser").value);
            _CountUnsuccessfulattempt=int.Parse(db.UserSettings.SingleOrDefault(q => q.Key == "CountUnsuccessfulattempt").value);
        }
        public List<string> ValidationRegister(User user)
        {
            List<string> lstString = new List<string>();


            if (string.IsNullOrEmpty(user.Username))
            {
                lstString.Add("نام کاربری نمیتواند خالی باشد");
            }

            if (string.IsNullOrEmpty(user.password))
                lstString.Add("پسورد نمیتواند خالی باشد");
           else
            lstString=checkPassword(user.password);
            var oldUser = db.Users.SingleOrDefault(q => q.Username == user.Username);
            if (oldUser != null)
            {
                lstString.Add("ابن نام کاربری قبلا ثبت شده است");

            }
            if (string.IsNullOrEmpty(user.firstName))
                lstString.Add("نام نمیتواند خالی باشد");
            if (string.IsNullOrEmpty(user.lastName))
                lstString.Add("نام خانوادگی نمیتواند خالی باشد");

            if (user.mobile != null)
            {
                if (user.mobile.Length != 11)
                    lstString.Add("شماره موبایل باید 11 رقم باشد");
                if (user.mobile.Substring(0, 2) != "09")
                    lstString.Add("شماره موبایل با 09 شروع میشود");
            }


            return lstString;
        }

        public List<string> checkPassword(string input)
        {
           
            int minChar = 4;
            List<string> lstString = new List<string>();
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMinimum8Chars = new Regex(@".{"+minChar+",}");

            if (!hasUpperChar.IsMatch(input))
            {
                lstString.Add("رمز عبور باید حداقل یک حروف بزرگ داشته باشد");
            }
            if (!hasNumber.IsMatch(input))
            {
                lstString.Add("رمز عبور باید حداقل یک عدد داشته باشد");
            }
            if (!hasMinimum8Chars.IsMatch(input))
            {
                lstString.Add(string.Format("رمز عبور باید حداقل {0} کارکتر داشته باشد", minChar));
            }
            // var isValidated = hasNumber.IsMatch(input) && hasUpperChar.IsMatch(input) && hasMinimum8Chars.IsMatch(input);
            return lstString;
        }

        public AccountViewModel ValidationLogin(AccountViewModel accountViewModel)
        {

             List<string> lstString = new List<string>();

            if (string.IsNullOrEmpty(accountViewModel.Account.Username) || string.IsNullOrEmpty(accountViewModel.Account.Password))
            {
                lstString.Add("نام کاربری یا پسورد نمیتواند خالی باشد");
                accountViewModel.lstString = lstString;
                db.LogUserAuthenticationDatas.Add(new LogUserAuthenticationData()
                {
                    AuthenticationLogType = (byte)AuthenticationLogType.EmptyInputs,
                    CreateDate = DateTime.Now,
                    Hostname = Utility.GetHostName(),
                    IP = Utility.GetLocalIPAddress(),
                    UserId = (accountViewModel.UserId != 0) ? accountViewModel.UserId : (int?)null,
                    UserName = accountViewModel.Username,
                    Description = "نام کاربری یا پسورد نمیتواند خالی باشد"
                });
                db.SaveChanges();
                return accountViewModel;
            }

            var oldUser = db.Users.SingleOrDefault(q => q.Username == accountViewModel.Account.Username);
            if (oldUser == null || oldUser.IsActive==false)
            {
                lstString.Add("نام کاربری یا پسورد اشتباه است");
                accountViewModel.lstString = lstString;
                db.LogUserAuthenticationDatas.Add(new LogUserAuthenticationData()
                {
                    AuthenticationLogType = (byte)AuthenticationLogType.WrongUsername,
                    CreateDate = DateTime.Now,
                    Hostname = Utility.GetHostName(),
                    IP = Utility.GetLocalIPAddress(),
                    UserId = (accountViewModel.UserId != 0) ? accountViewModel.UserId : (int?)null,
                    UserName = accountViewModel.Username,
                    Description = "نام کاربری اشتباه است"
                });
                db.SaveChanges();
                return accountViewModel;
            }

            if (oldUser.IsActive == false)
            {
                lstString.Add("نام کاربرشما غیر فعال شده است.");
                accountViewModel.lstString = lstString;
                db.LogUserAuthenticationDatas.Add(new LogUserAuthenticationData()
                {
                    AuthenticationLogType = (byte)AuthenticationLogType.InactiveUser,
                    CreateDate = DateTime.Now,
                    Hostname = Utility.GetHostName(),
                    IP = Utility.GetLocalIPAddress(),
                    UserId = (accountViewModel.UserId != 0) ? accountViewModel.UserId : (int?)null,
                    UserName = accountViewModel.Username,
                    Description = "نام کاربر غیر فعال شده است"
                });
                db.SaveChanges();
                return accountViewModel;
            }
            if (oldUser.IsResetPassword == true)
            {
                lstString.Add("پسورد شما ریست شده است لطفا پسورد جدید برای خود قرار دهید");
                accountViewModel.lstString = lstString;
                accountViewModel.IsResetPassword = true;
              HttpContext.Current.Session["UserId"] = oldUser.id;
                db.LogUserAuthenticationDatas.Add(new LogUserAuthenticationData()
                {
                    AuthenticationLogType = (byte)AuthenticationLogType.PasswordMustChange,
                    CreateDate = DateTime.Now,
                    Hostname = Utility.GetHostName(),
                    IP = Utility.GetLocalIPAddress(),
                    UserId = (accountViewModel.UserId != 0) ? accountViewModel.UserId : (int?)null,
                    UserName = accountViewModel.Username,
                    Description = "پسورد ریست شده است."
                });
                db.SaveChanges();
                return accountViewModel;
            }
            var allowedipaddresses = db.AllowedIpAddresses.Where(p => p.IsActive == true).Select(p=>p.IpAddress).ToList();
            var currentipaddress = Utility.GetLocalIPAddress();
            if (!allowedipaddresses.Contains(currentipaddress) && allowedipaddresses.Count>0)
            {
                lstString.Add("آدرس IP شما مجاز به دسترسی نیست.");
                accountViewModel.lstString = lstString;
                db.LogUserAuthenticationDatas.Add(new LogUserAuthenticationData()
                {
                    AuthenticationLogType = (byte)AuthenticationLogType.NotAllowedIpAddress,
                    CreateDate = DateTime.Now,
                    Hostname = Utility.GetHostName(),
                    IP = Utility.GetLocalIPAddress(),
                    UserId = (accountViewModel.UserId != 0) ? accountViewModel.UserId : (int?)null,
                    UserName = accountViewModel.Username,
                    Description = "آدرس IP غیر مجاز"
                });
                db.SaveChanges();
                return accountViewModel;
            }
            var setting = db.UserSettings.ToList();
           
            var Password = Security.RsaEncryptDecrypt.RSACls.RSADecryption(oldUser.password, Properties.Settings.Default.PrivateKey);
            //کاربر پسورد را اشتباه وارد کرده است
            if (oldUser != null && Password != accountViewModel.Account.Password)
            {
                FalsePassword falsePassword=new FalsePassword()
                //Models.FalsePassword logUserData = new Models.LogUserData()
                {
             FalsePass= Security.RsaEncryptDecrypt.RSACls.RSAEncryption(accountViewModel.Account.Password, Properties.Settings.Default.PublicKey),
                DateCreated = DateTime.Now,
                    UserId = oldUser.id,
                

                };
                db.FalsePasswords.Add(falsePassword);
                db.LogUserAuthenticationDatas.Add(new LogUserAuthenticationData()
                {
                    AuthenticationLogType = (byte)AuthenticationLogType.UserLocked,
                    CreateDate = DateTime.Now,
                    Hostname = Utility.GetHostName(),
                    IP = Utility.GetLocalIPAddress(),
                    UserId = (accountViewModel.UserId != 0) ? accountViewModel.UserId : (int?)null,
                    UserName = accountViewModel.Username,
                    Description = "کاربر پسورد را اشتباه وارد کرده است"
                });
                db.SaveChanges();


                var varCheckTileLockFinish = checkTimeLockFinish(oldUser, _MinuteLockUser);
                if (varCheckTileLockFinish == false)
                {
                    TimeSpan span = DateTime.Now.Subtract((DateTime)oldUser.LockoutEnd);
                    lstString.Add("یوزر شما به علت "+_CountUnsuccessfulattempt+" اشتباه به مدت " + (_MinuteLockUser - Math.Round(span.TotalMinutes, 0)) + " دقیقه قفل شده است");
                    accountViewModel.lstString = lstString;
                    db.LogUserAuthenticationDatas.Add(new LogUserAuthenticationData()
                    {
                        AuthenticationLogType = (byte)AuthenticationLogType.UserLocked,
                        CreateDate = DateTime.Now,
                        Hostname = Utility.GetHostName(),
                        IP = Utility.GetLocalIPAddress(),
                        UserId = (accountViewModel.UserId != 0) ? accountViewModel.UserId : (int?)null,
                        UserName = accountViewModel.Username,
                        Description = "یوزر شما به علت " + _CountUnsuccessfulattempt + " اشتباه به مدت " + (_MinuteLockUser - Math.Round(span.TotalMinutes, 0)) + " دقیقه قفل شده است"
                    });
                    db.SaveChanges();
                    return accountViewModel;
                }

                var varCheckFailedCount = checkFailedCount(oldUser, _CountUnsuccessfulattempt);
                if (varCheckFailedCount == true)
                {
                    LockUser(oldUser, accountViewModel);
                    return accountViewModel;
                }
                else {
                    IncreaseFault(oldUser, accountViewModel);
                    return accountViewModel;
                }
              
            }
   
            else
            {
                var varCheckTimeLockFinish = checkTimeLockFinish(oldUser, _MinuteLockUser);
                if (varCheckTimeLockFinish == false)
                {
                    TimeSpan span = DateTime.Now.Subtract((DateTime)oldUser.LockoutEnd);
                    lstString.Add("یوزر شما به علت "+_CountUnsuccessfulattempt+" اشتباه به مدت " + (_MinuteLockUser - Math.Round(span.TotalMinutes, 0)) + " دقیقه قفل شده است");
                    accountViewModel.lstString = lstString;
                    db.LogUserAuthenticationDatas.Add(new LogUserAuthenticationData()
                    {
                        AuthenticationLogType = (byte)AuthenticationLogType.UserLocked,
                        CreateDate = DateTime.Now,
                        Hostname = Utility.GetHostName(),
                        IP = Utility.GetLocalIPAddress(),
                        UserId = (accountViewModel.UserId != 0) ? accountViewModel.UserId : (int?)null,
                        UserName = accountViewModel.Username,
                        Description = "یوزر شما به علت " + _CountUnsuccessfulattempt + " اشتباه به مدت " + (_MinuteLockUser - Math.Round(span.TotalMinutes, 0)) + " دقیقه قفل شده است"
                    });
                    db.SaveChanges();
                    return accountViewModel;
                }
                ActiveUser(oldUser, accountViewModel);
                accountViewModel.UserId = oldUser.id;
               
                return accountViewModel;
            }

        }
        /// <summary>
        /// آیا زمان قفل بودن اکانت تمام شده است یا نه
        /// </summary>
        private bool checkTimeLockFinish(User user, int value)
        {
            if (user.LockoutEnd == null) return true; 
            TimeSpan span = DateTime.Now.Subtract((DateTime)user.LockoutEnd);

            if (span.TotalMinutes >= value && user.IsLock == true)
            {
                return true;
            }
            else if (user.IsLock == false)
            {
                return true;
            }
            else if (span.TotalMinutes < value && user.IsLock == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// آیا کاربر بیشتر از حد مجاز پسورد خود را اشتباه وارد نموده
        /// </summary>
        /// <param name="user"></param>
        private bool checkFailedCount(User user, int value)
        {
            if (user.AccessFailedCount >= value)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// با هر بار اشتباه یکی به تعدا اشتباهات اضافه مشود
        /// </summary>
        /// <param name="user"></param>
        private void IncreaseFault(User user, AccountViewModel accountViewModel)
        {
            List<string> lstString = new List<string>();
            user.LockoutEnd = DateTime.Now;
            user.AccessFailedCount += 1;
            db.SaveChanges();
            lstString.Add("بعد از "+_CountUnsuccessfulattempt+" اشتباه اکانت شما قفل میشود");
            lstString.Add(user.AccessFailedCount.ToString() + " تعداد اشتباه ");
            accountViewModel.lstString = lstString;
        }
        /// <summary>
        /// فعال کردن کاربر پس از مدت زمان مشخص
        /// </summary>
        /// <param name="user"></param>
        private void ActiveUser(User user, AccountViewModel accountViewModel)
        {
            //user.IsActive = true;
            user.IsLock = false;
            user.AccessFailedCount = 0;
            db.SaveChanges();
            accountViewModel.Account.FullName = user.firstName + " " + user.lastName;
            accountViewModel.Account.UserId = user.id;
        }
        /// <summary>
        /// به علت اینکه کاربر بیشتر از حد مجاز پسورد خود را اشتباه وارد کرده اکانت قفل شده است
        /// </summary>
        /// <param name="user"></param>
        /// <param name="accountViewModel"></param>
        private void LockUser(User user, AccountViewModel accountViewModel)
        {
            List<string> lstString = new List<string>();
          
            user.LockoutEnd = DateTime.Now;
            TimeSpan span = DateTime.Now.Subtract((DateTime)user.LockoutEnd);
            //user.IsActive = false;
            user.IsLock = true;
            db.SaveChanges();
            lstString.Add("یوزر شما به علت "+_CountUnsuccessfulattempt+" اشتباه به مدت " + (_MinuteLockUser - Math.Round(span.TotalMinutes, 0)) + " دقیقه قفل شده است");
            accountViewModel.lstString = lstString;
        }


        public  bool checkPasswordMustChanges()
        {
            var PasswordMustChanges = db.UserSettings.FirstOrDefault(q => q.Key == "PasswordMustChanges");
            if (PasswordMustChanges == null)
            {
                return false;
            }
            if (HttpContext.Current.Session["UserId"] == null) return false;
            var UserId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
            var oldUser = db.Users.SingleOrDefault(q => q.id == UserId);
            TimeSpan spanPasswordMustChanges = DateTime.Now.Subtract(oldUser.DatePassRefresh ?? DateTime.Now);

            int Days = Convert.ToInt32(Math.Round(spanPasswordMustChanges.TotalDays, 0));

            if (Days >= int.Parse(PasswordMustChanges.value))
            {
                return true;
            }
            return false;
        }


    }
}