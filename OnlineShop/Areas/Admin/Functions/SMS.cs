using SmsIrRestful;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using OnlineShop.Models;
using Microsoft.Ajax.Utilities;

namespace OnlineShop.Areas.Admin.Functions
{
    public class SMS
    {
        OnlineShopEntities db = new OnlineShopEntities();
      
        public Boolean Send(string Mobile, string Code, int Template)
        {
            var setting = db.Settings.FirstOrDefault();
            try
            {
                string SmsTokenOne = setting.SmsToken1;
                string SmsTokenTwo = setting.SmsToken2;
                var token = new Token().GetToken(SmsTokenOne, SmsTokenTwo);

                var ultraFastSend = new UltraFastSend()
                {
                    Mobile = Convert.ToInt64(Mobile),
                    TemplateId = Template,
                    ParameterArray = new List<UltraFastParameters>()
                      {
                         new UltraFastParameters()
                            {
                                  Parameter = "VerificationCode" , ParameterValue = Code
                            }
                      }.ToArray()

                };

                UltraFastSendRespone ultraFastSendRespone = new UltraFast().Send(token, ultraFastSend);

                if (ultraFastSendRespone.IsSuccessful)
                {
                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false; 
            }
        
        }
    }
}