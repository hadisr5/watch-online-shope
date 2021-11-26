using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace OnlineShop.Security
{
    public class MessageSender //: IMessageSender
    {
        public  Task SendEmailAsync(string toEmail, string subject, string message, bool isMessageHtml = false)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                //نام فرستنده
                mail.From = new MailAddress(Properties.Settings.Default.emailSender);
                //آدرس گیرنده یا گیرندگان
                mail.To.Add(toEmail);
                //عنوان ایمیل
                mail.Subject = subject;
                //بدنه ایمیل
                mail.Body = message;
                mail.IsBodyHtml = isMessageHtml;
                //مشخص کرن پورت 
                SmtpServer.Port = 587;
                //SmtpServer.UseDefaultCredentials = true;
                SmtpServer.EnableSsl = true;
                //username : به جای این کلمه نام کاربری ایمیل خود را قرار دهید
                //password : به جای این کلمه رمز عبور ایمیل خود را قرار دهید
                SmtpServer.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.emailUsername,
                    Properties.Settings.Default.emailPassword);

                return Task.Run(() =>
                {
                    SmtpServer.Send(mail);
                });


            }
            catch (Exception ex)
            {

                
                
                return Task.Run(
                    ()=>
                    {
                       return ex.ToString();
                    }
                    );
            }



        }

    }
}