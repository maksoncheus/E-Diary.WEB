using System.Net.Mail;
using System.Net;
using TransportationService.WEB.Configuration;

namespace TransportationService.WEB.Services
{
    public class MailService
    {
        private readonly ILogger<MailService> _logger;
        private readonly SMTPSettings _smtpSettings;

        public MailService(ILogger<MailService> logger, SMTPSettings smtpSettings)
        {
            _logger = logger;
            _smtpSettings = smtpSettings;
        }
        //public void SendConfirmationLink(string link, string emailTo)
        //{
        //    try
        //    {
        //        using (MailMessage mm = new MailMessage(
        //            new MailAddress(_smtpSettings.Email, "Электронный дневник Школа-Школа-Школа-ла-ла"), new MailAddress(emailTo)))
        //        {
        //            mm.Subject = "Подтверждение почты TransportationService";
        //            mm.Body = "Благодарим вас за регистрацию в сервисе TransportationService!"
        //                + Environment.NewLine +
        //                $"Для подтверждения аккаунта пройдите по ссылке: {link}";
        //            mm.IsBodyHtml = false;
        //            SendMessage(mm);
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        _logger.Log(LogLevel.Error, ex.Message);
        //    }
        //}
        public void SendResetLink(string link, string emailTo)
        {
            try
            {
                using (MailMessage mm = new MailMessage(
                    new MailAddress(_smtpSettings.Email, "Электронный дневник Школа-Школа-Школа-ла-ла"), new MailAddress(emailTo)))
                {
                    mm.Subject = "Сброс пароля в электронном дневнике";
                    mm.Body = "Для вашего аккаунта был запрошен сброс пароля"
                        + Environment.NewLine +
                        $"Для сброса пройдите по ссылке: {link}"
                        + Environment.NewLine +
                        "Если вы не запрашивали сброс пароля, проигнорируйте данное сообщение";
                    mm.IsBodyHtml = false;
                    SendMessage(mm);
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.Message);
            }
        }

        private void SendMessage(MailMessage mm)
        {
            using (SmtpClient smtp = new SmtpClient())
            {
                smtp.Host = _smtpSettings.Host;
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential(_smtpSettings.Email, _smtpSettings.Password);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = NetworkCred;
                smtp.Port = _smtpSettings.Port;
                smtp.Send(mm);
            }
        }
    }
}
