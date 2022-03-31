using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace rhcon.Models.ViewModel
{
    public class EmailViewModel
    {

        //Envio de email al encargado de la empresa
        private string EmailORigen = "rhstackcode@gmail.com";
        private string pass = "stackcode1.";
      
      
        
        
        public bool SendEmail(string body, string destino, string asunto )
        {

            MailMessage EmailMess = new MailMessage(
                this.EmailORigen,
                destino,
                asunto,
                body
                );
            EmailMess.IsBodyHtml = true;

            SmtpClient oSmtpClient = new SmtpClient("smtp.gmail.com");
            oSmtpClient.EnableSsl = true;
            //oSmtpClient.UseDefaultCredentials = false;
            oSmtpClient.Host = "smtp.gmail.com";
            oSmtpClient.Port = 587;
            oSmtpClient.Credentials = new System.Net.NetworkCredential(EmailORigen, pass);
            try
            {
                oSmtpClient.Send(EmailMess);
                oSmtpClient.Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }

            }


    }
}