using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SocialTG
{
    public class Ultils
    {
        public static void UploadImage(string fileName, string Base64)
        {
            File.WriteAllBytes(fileName, Convert.FromBase64String(Base64));
        }

        public static void UploadVideo(string fileName, string Base64)
        {
            File.WriteAllBytes(fileName, Convert.FromBase64String(Base64));
        }

        public static string GetHash(string s)
        {
            UTF8Encoding ue = new UTF8Encoding();
            byte[] v = ue.GetBytes(s);
            byte[] hashValue = new MD5CryptoServiceProvider().ComputeHash(v);
            return Convert.ToBase64String(hashValue);
        }

        public static string sendit(string ReciverMail, string body)
        {
            MailMessage msg = new MailMessage();

            msg.From = new MailAddress("shamisocialsvc@gmail.com");
            msg.To.Add(ReciverMail);
            msg.Subject = "Shasi: new Password !";
            msg.Body = "Your Password is: " + body;
            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = true;
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential("shamisocialsvc@gmail.com", "shami@123");
            client.Timeout = 20000;
            try
            {
                client.Send(msg);
                return "Mail has been successfully sent!";
            }
            catch (Exception ex)
            {
                return "Fail Has error" + ex.Message;
            }
            finally
            {
                msg.Dispose();
            }
        }


        public static string SendMessToUser(String TokenId, String message)
        {
            //string BrowserAPIKey = "AIzaSyCC57kWWDc-2AbvBq2FO5sw0z2GDl6JA-I";
            //send id: 178873681587
            string postData = "{ \"registration_ids\": [ \"" + TokenId + "\" ], \"data\": " + message + "}";
            return SendGCMNotification(postData);
        }

        public static string SendGCMNotification(string postData, string postDataContentType = "application/json")
        {

            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("https://android.googleapis.com/gcm/send");
            Request.Method = "POST";
            Request.KeepAlive = false;
            Request.ContentType = postDataContentType;
            Request.Headers.Add(string.Format("Authorization: key={0}", "AIzaSyCC57kWWDc-2AbvBq2FO5sw0z2GDl6JA-I"));
            Request.Headers.Add(string.Format("Sender: id={0}", "178873681587"));
            Request.ContentLength = byteArray.Length;

            Stream dataStream = Request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            //
            //  SEND MESSAGE
            try
            {
                WebResponse Response = Request.GetResponse();
                HttpStatusCode ResponseCode = ((HttpWebResponse)Response).StatusCode;
                if (ResponseCode.Equals(HttpStatusCode.Unauthorized) || ResponseCode.Equals(HttpStatusCode.Forbidden))
                {
                    var text = "Unauthorized - need new token";

                }
                else if (!ResponseCode.Equals(HttpStatusCode.OK))
                {
                    var text = "Response from web service isn't OK";
                }

                StreamReader Reader = new StreamReader(Response.GetResponseStream());
                string responseLine = Reader.ReadToEnd();
                Reader.Close();

                return responseLine;
            }
            catch (Exception e)
            {
                return "error";
            }
            
        }


        public static bool ValidateServerCertificate(
                                                  object sender,
                                                  X509Certificate certificate,
                                                  X509Chain chain,
                                                  SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}