using OnlineShop.Security.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OnlineShop.Models;

namespace OnlineShop.Security
{
    public class AccountModel
    {
        OnlineShopEntities db = new OnlineShopEntities();
        public List<Security.Account> listAccounts = new List<Security.Account>();

        public AccountViewModel ValidationLogin(AccountViewModel accountViewModel)
        {
           
           
            List<string> lstString = new List<string>();
            //AccountViewModel accountViewModel1 = new AccountViewModel();

            if (string.IsNullOrEmpty(accountViewModel.Account.Username) || string.IsNullOrEmpty(accountViewModel.Account.Password))
            {
                lstString.Add("نام کاربری یا پسور نمیتواند خالی باشد");
                //accountViewModel.lstString.Add("نام کاربری یا پسور نمیتواند خالی باشد");
            }

            var oldUser = db.Users.SingleOrDefault(q => q.Username == accountViewModel.Account.Username);
      

            

            if (oldUser != null)
            {
                if (Security.RsaEncryptDecrypt.RSACls.RSADecryption(oldUser.password, Properties.Settings.Default.PrivateKey)  != accountViewModel.Account.Password)
                {
                    if (oldUser.AccessFailedCount >= 5)
                    {
                        TimeSpan span = DateTime.Now.Subtract((DateTime)oldUser.LockoutEnd);

                        if (span.TotalMinutes > 5)
                        {
                            oldUser.LockoutEnd = DateTime.Now;
                            oldUser.IsActive = true;
                            oldUser.IsLock = true;
                            oldUser.AccessFailedCount = 0;
                            db.SaveChanges();
                            lstString.Add("نام کاربری یا پسورد اشتباه است");
                        }
                        else
                        {
                            //oldUser.LockoutEnd = DateTime.Now;
                            oldUser.IsActive = false;
                            oldUser.IsLock = false;
                            db.SaveChanges();

                            lstString.Add("یوزر شما به علت پنج بار اشتباه به مدت "+(5-Math.Round(span.TotalMinutes,0)) + " دقیقه قفل شده است");
                        }

                    }
                    else
                    {
                        oldUser.LockoutEnd = DateTime.Now;
                        oldUser.AccessFailedCount += 1;
                        db.SaveChanges();
                        lstString.Add( "بعد از پنج بار اشتباه اکانت شما قفل میشود");
                        lstString.Add(oldUser.AccessFailedCount.ToString() + " تعداد اشتباه " );

                    }
                }
                else
                {
                    if (oldUser.IsLock == false && DateTime.Now.Subtract((DateTime)oldUser.LockoutEnd).TotalMinutes <= 5)
                    {
                        lstString.Add("یوزر شما به علت پنج بار اشتباه به مدت " + (5 - Math.Round(DateTime.Now.Subtract((DateTime)oldUser.LockoutEnd).TotalMinutes, 0)) + " دقیقه قفل شده است");
                    
                    }
                    else
                    {
                        oldUser.IsActive = true;
                        oldUser.IsLock = true;
                        oldUser.AccessFailedCount = 0;
                        db.SaveChanges();

                        accountViewModel.Account.FullName= oldUser.firstName + " " + oldUser.lastName;
                        accountViewModel.Account.UserId = oldUser.id;
                       

                        return accountViewModel;
                    }
                }
            }
            else
            {
                lstString.Add("نام کاربری یا پسورد اشتباه است");
            }
            accountViewModel.lstString = lstString;
            return accountViewModel;
        }


    }
}