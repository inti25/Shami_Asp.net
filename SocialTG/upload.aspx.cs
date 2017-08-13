using DBHelper;
using Newtonsoft.Json;
using System;

namespace SocialTG
{
    public partial class upload : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Respond rep = new Respond();
            rep.success = false;
            string error = Request.QueryString["error"];
            if (!string.IsNullOrEmpty(error))
            {
                if (error.Equals("die"))
                {
                    rep.messages = "Upload Failed!";
                }
                else if (error.Equals("miss"))
                {
                    rep.messages = "Missing feild!";
                }
            }
            else if (string.IsNullOrEmpty(Request.QueryString["memId"])
                || string.IsNullOrEmpty(Request.QueryString["isPrivate"])
                || string.IsNullOrEmpty(Request.QueryString["videoUrl"]))
            {
                rep.messages = "Missing feild!";
            }
            else
            {
                string thumbnail = "http://shami.96.lt/thumbnails/default-thumbnail.png";
                if (!string.IsNullOrEmpty(Request.QueryString["thumbnails"]))
                {
                    thumbnail = Request.QueryString["thumbnails"];
                }
                int memId = int.Parse(Request.QueryString["memId"]);
                string des = Request.QueryString["description"];
                string videoUrl = Request.QueryString["videoUrl"];
                bool isPrivate = bool.Parse(Request.QueryString["isPrivate"]);
                DA_PostManager.insertPost("", des, thumbnail, videoUrl, memId, true, false, isPrivate);
                rep.success = true;
            }
            Response.ContentType = "text/plain";
            Response.Write(JsonConvert.SerializeObject(rep));
            Response.End();
        }
    }
}