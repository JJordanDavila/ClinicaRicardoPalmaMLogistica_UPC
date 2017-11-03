using DattatecPanel.Models.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace DattatecPanel.Models.Util
{
    public class MailSMTP
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool NetworkCredential { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public bool UseSSL { get; set; }
        public int TimeOut { get; set; }

        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        NetworkCredential credenciales;

        public MailSMTP(string uid, string pwd, string domain)
        {
            credenciales = new NetworkCredential(uid, pwd, domain);
        }

        public MailSMTP()
        {
            credenciales = new NetworkCredential("alertaUPCCRP@gmail.com", "00623060", string.Empty);
            this.Host = "smtp.gmail.com";
            this.Port = 587;
            this.NetworkCredential = true;
            this.UseDefaultCredentials = false;
            this.UseSSL = true;
        }

        public bool EnviarCorreo(string aliasCorreo, string destinatario, string asunto, string cuerpo, bool isHtml, List<AdjuntoCorreo> lstAdjuntos)
        {
            SmtpClient client = new SmtpClient();
            MailMessage message = new MailMessage();
            try
            {
                MailAddress mail = new MailAddress(credenciales.UserName, aliasCorreo);
                client.Host = this.Host;
                if (this.Port > 0) client.Port = this.Port;
                if (NetworkCredential)
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = credenciales;
                }
                else
                {
                    client.UseDefaultCredentials = true;
                    if (UseDefaultCredentials) client.Credentials = CredentialCache.DefaultNetworkCredentials;
                }
                message.From = mail;
                message.To.Add(destinatario);
                message.Subject = asunto;
                message.Body = cuerpo;
                message.IsBodyHtml = isHtml;
                if (UseSSL)
                {
                    client.EnableSsl = true;
                    ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                }
                if (lstAdjuntos != null && lstAdjuntos.Count > 0)
                {
                    foreach (AdjuntoCorreo item in lstAdjuntos)
                    {
                        Attachment adjunto = new Attachment(new MemoryStream(item.Adjunto), item.Nombre);
                        message.Attachments.Add(adjunto);
                    }
                }
                if (TimeOut > 0) client.Timeout = this.TimeOut;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(message);
                foreach (Attachment item in message.Attachments)
                {
                    item.Dispose();
                }
                message.Attachments.Dispose();
                message.Dispose();
                client.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                this.ErrorCode = -1;
                this.ErrorMessage = ex.Message;
                return false;
            }
        }
    }
}