using System;
using System.Web.Services;
using DBHelper;
using Newtonsoft.Json;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using System.Collections.Generic;

namespace SocialTG
{
    /// <summary>
    /// Summary description for MainServices
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class MainServices : System.Web.Services.WebService
    {
        [WebMethod]
        public string Register(string email, string pass, string name,string nickname, string dob)
        {
            Respond sp = new Respond();
            try
            {
                if(DA_MemberManager.isExist(email))
                {
                    sp.success = false;
                    sp.messages = "Email da dc su dung";
                }
                else
                {
                    sp.messages = DA_MemberManager.insert(email, pass, name, nickname, dob, null) +"";
                    sp.success = true;
                }
            }
            catch (Exception e)
            {
                sp.success = false;
                sp.messages = e.Message;
            }
            return JsonConvert.SerializeObject(sp);
        }
        [WebMethod]
        public string Login(string email,string pass)
        {
            Respond sp = new Respond();
            Object memberInfo;
            try
            {
                if (DA_MemberManager.login(email, pass, out memberInfo))
                {
                    sp.success = true;
                    sp.messages = JsonConvert.SerializeObject(memberInfo);
                }
                else
                {
                    sp.success = false;
                    sp.messages = "Dang nhap ko thanh cong";
                }
            }
            catch (Exception e)
            {
                sp.success = false;
                sp.messages = e.Message;
            }
            return JsonConvert.SerializeObject(sp);
        }

        [WebMethod]
        public string getInfoUser(int memId)
        {
            Respond sp = new Respond();
            try
            {
                Object obj = DA_MemberManager.getinfoUser(memId);
                sp.success = true;
                sp.messages = JsonConvert.SerializeObject(obj);
            }
            catch (Exception e)
            {
                sp.success = false;
                sp.messages = e.Message;
            }
            return JsonConvert.SerializeObject(sp);
        }

        [WebMethod]
        public string getInfoOtherUser(int memId,int currentMemId)
        {
            Respond sp = new Respond();
            try
            {
                Object obj = DA_MemberManager.getInfoOtherUser(memId,currentMemId);
                sp.success = true;
                sp.messages = JsonConvert.SerializeObject(obj);
            }
            catch (Exception e)
            {
                sp.success = false;
                sp.messages = e.Message;
            }
            return JsonConvert.SerializeObject(sp);
        }

        [WebMethod]
        public string updateInfo(int memId, String name, String nickname, String dob)
        {
            Respond sp = new Respond();
            try
            {
                DA_MemberManager.update(memId,name,nickname,dob);
                sp.success = true;
                sp.messages = "Ok";
            }
            catch (Exception e)
            {
                sp.success = false;
                sp.messages = e.Message;
            }
            return JsonConvert.SerializeObject(sp);
        }

        [WebMethod]
        public string ChangePassword(string email, string oldPass, string newpass)
        {
            Respond sp = new Respond();
            try
            {
                DA_MemberManager.UpdatePass(email, oldPass, newpass);
                sp.success = true;
            }
            catch(Exception e)
            {
                sp.success = false;
                sp.messages = e.Message;
            }
            return JsonConvert.SerializeObject(sp);
        }

        [WebMethod]
        public string ForgotPass(string email)
        {
            Respond sp = new Respond();
            try
            {
                Random ran = new Random();
                int temp = ran.Next(100000, 999999);
                string newP = temp.ToString();
                DA_MemberManager.UpdatePass(email, newP);
                Ultils.sendit(email, newP);
                sp.success = true;
            }
            catch (Exception e)
            {
                sp.success = false;
                sp.messages = e.Message;
            }
            return JsonConvert.SerializeObject(sp);
        }

        [WebMethod]
        public string UpdateAvatar(string email, string base64)
        {
            Member m = DA_MemberManager.getMember(email);
            String pt = m.id + ".jpeg";
            Respond sp = new Respond();
            try
            {
                //Ultils.UploadImage(Server.MapPath(pt), base64);
                WebClient wc = new WebClient();
                var values = new NameValueCollection();
                values["name"] = pt;
                values["data"] = base64;
                var response = wc.UploadValues("http://shami.96.lt/uploadImages.php", values);
                string data = Encoding.Default.GetString(response);
                if (data.StartsWith("ok"))
                {
                    String imgUrl = data.Substring(2);
                    DA_MemberManager.updateAvatar(email, imgUrl);
                    sp.success = true;
                }
                else
                {
                    sp.success = false;
                    sp.messages = "upload failed!";
                }
            }
            catch (Exception e)
            {
                sp.success = false;
                sp.messages = e.Message;
            }
            return JsonConvert.SerializeObject(sp);
        }

        [WebMethod]
        public string UpdateTokenId(int memId, string tokenId)
        {
            Respond sp = new Respond();
            try
            {
                DA_MemberManager.updateTokenId(memId,tokenId);
                sp.success = true;
                sp.messages = "OK";
            }
            catch (Exception e)
            {
                sp.success = false;
                sp.messages = e.Message;
            }
            return JsonConvert.SerializeObject(sp);
        }

        [WebMethod]
        public string upNewPost(string title, string description, string Base64, string videoUrl, int memId, bool isVideo, bool isYoutube, bool isPrivate)
        {
            string imageName = DateTime.Now.TimeOfDay.TotalMilliseconds + ".jpeg";
            string url = null;
            Respond sp = new Respond();
            Object rs = null;
            try
            {
                if (!isVideo)
                {
                    WebClient wc = new WebClient();
                    var values = new NameValueCollection();
                    values["name"] = imageName;
                    values["data"] = Base64;
                    var response = wc.UploadValues("http://shami.96.lt/uploadImages.php", values);
                    string data = Encoding.Default.GetString(response);
                    if (data.StartsWith("ok"))
                    {
                        url = data.Substring(2);
                    }
                    else
                    {
                        sp.success = false;
                        sp.messages = "upload failed!";
                        return JsonConvert.SerializeObject(sp);
                    }
                }

                rs = DA_PostManager.insertPost(title, description, url, videoUrl, memId, isVideo, isYoutube, isPrivate);
                sp.success = true;
                sp.messages = JsonConvert.SerializeObject(rs);

                CLMess mess = new CLMess();
                mess.senderName = "newPost";
                mess.mess = "newPost";
                //mess.postId = postId;
                List<string> lst = DA_MemberManager.listToken();
                foreach (string token in lst)
                {
                    if (!String.IsNullOrEmpty(token))
                        Ultils.SendMessToUser(token, JsonConvert.SerializeObject(mess));
                }

            }
            catch (Exception e)
            {
                sp.success = false;
                sp.messages = e.Message;
            }
            return JsonConvert.SerializeObject(sp);
        }

        [WebMethod]
        public string getTop10Public(int id)
        {
            Object lst;
            Respond sp = new Respond();
            try
            {
                lst = DA_PostManager.get10PostPublic(id);
                sp.success = true;
                sp.messages = JsonConvert.SerializeObject(lst);
            }
            catch (Exception e)
            {
                sp.success = false;
                sp.messages = e.Message;
            }
            return JsonConvert.SerializeObject(sp);
        }

        [WebMethod]
        public string getNextTop10Public(int id,int postId)
        {
            Object lst;
            Respond sp = new Respond();
            try
            {
                lst = DA_PostManager.getNext10PostPublic(id,postId);
                sp.success = true;
                sp.messages = JsonConvert.SerializeObject(lst);
            }
            catch (Exception e)
            {
                sp.success = false;
                sp.messages = e.Message;
            }
            return JsonConvert.SerializeObject(sp);
        }

        [WebMethod]
        public string getTop10PostFollowed(int id)
        {
            Object lst;
            Respond sp = new Respond();
            try
            {
                lst = DA_PostManager.get10PostByFollowing(id);
                sp.success = true;
                sp.messages = JsonConvert.SerializeObject(lst);
            }
            catch (Exception e)
            {
                sp.success = false;
                sp.messages = e.Message;
            }
            return JsonConvert.SerializeObject(sp);
        }

        [WebMethod]
        public string getNextTop10PostFollowed(int id,int postId)
        {
            Object lst;
            Respond sp = new Respond();
            try
            {
                lst = DA_PostManager.getNext10PostByFollowing(id,postId);
                sp.success = true;
                sp.messages = JsonConvert.SerializeObject(lst);
            }
            catch (Exception e)
            {
                sp.success = false;
                sp.messages = e.Message;
            }
            return JsonConvert.SerializeObject(sp);
        }

        [WebMethod]
        public string getListFollower(int userid)
        {
            Object lst;
            Respond sp = new Respond();
            try
            {
                lst = DA_FollowManage.getListFollower(userid);
                sp.success = true;
                sp.messages = JsonConvert.SerializeObject(lst);
            }
            catch (Exception e)
            {
                sp.success = false;
                sp.messages = e.Message;
            }
            return JsonConvert.SerializeObject(sp);
        }

        [WebMethod]
        public string getListFollowing(int userid)
        {
            Object lst;
            Respond sp = new Respond();
            try
            {
                lst = DA_FollowManage.getListFollowing(userid);
                sp.success = true;
                sp.messages = JsonConvert.SerializeObject(lst);
            }
            catch (Exception e)
            {
                sp.success = false;
                sp.messages = e.Message;
            }
            return JsonConvert.SerializeObject(sp);
        }

        [WebMethod]
        public string Follow(int follower, int following)
        {
            Respond sp = new Respond();
            try
            {
                DA_FollowManage.followMember(follower, following);
                sp.success = true;
            }
            catch (Exception e)
            {
                sp.success = false;
                sp.messages = e.Message;
            }
            return JsonConvert.SerializeObject(sp);
        }

        [WebMethod]
        public string ConfirmFollow(int follower, int following)
        {
            Respond sp = new Respond();
            try
            {
                DA_FollowManage.agreeFollow(follower, following);
                sp.success = true;
            }
            catch (Exception e)
            {
                sp.success = false;
                sp.messages = e.Message;
            }
            return JsonConvert.SerializeObject(sp);
        }

        [WebMethod]
        public string likeOrUnlike(int memId,int postId)
        {
            Respond sp = new Respond();
            try
            {
                DA_MemberManager.likeOrUnlikePost(memId, postId);
                sp.success = true;
                if(DA_MemberManager.isLiked(memId,postId))
                {
                    CLMess mess = new CLMess();
                    mess.senderName = DA_MemberManager.getDisplayName(memId);
                    mess.mess = "like";
                    mess.postId = postId;
                    Post temp = DA_PostManager.getPost(postId);
                    DA_NotifyManage.putNotify(memId, temp.MemberId ?? 0, "like", postId);
                    String token = DA_MemberManager.getTokenByPost(postId);
                    if (!String.IsNullOrEmpty(token))
                        Ultils.SendMessToUser(token, JsonConvert.SerializeObject(mess));
                }
            }
            catch (Exception e)
            {
                sp.success = false;
                sp.messages = e.Message;
            }
            return JsonConvert.SerializeObject(sp);
        }

        [WebMethod]
        public string comment(int memId, int postId, string content)
        {
            Respond sp = new Respond();
            try
            {
                DA_CommentManage.comment(memId, postId, content);
                sp.success = true;
                CLMess mess = new CLMess();
                mess.senderName = DA_MemberManager.getDisplayName(memId);
                mess.mess = "cmt";
                mess.postId = postId;
                Post temp = DA_PostManager.getPost(postId);
                DA_NotifyManage.putNotify(memId, temp.MemberId ?? 0, "cmt", postId);
                String token = DA_MemberManager.getTokenByPost(postId);
                if(!String.IsNullOrEmpty(token))
                    Ultils.SendMessToUser(token, JsonConvert.SerializeObject(mess));
            }
            catch (Exception e)
            {
                sp.success = false;
                sp.messages = e.Message;
            }
            return JsonConvert.SerializeObject(sp);
        }

        [WebMethod]
        public string getListCmt(int postId)
        {
            Respond sp = new Respond();
            try
            {
                Object obj = DA_CommentManage.getListCmt(postId);
                sp.success = true;
                sp.messages = JsonConvert.SerializeObject(obj);
            }
            catch (Exception e)
            {
                sp.success = false;
                sp.messages = e.Message;
            }
            return JsonConvert.SerializeObject(sp);
        }

        [WebMethod]
        public string deletePost(int memId, int postId)
        {
            Respond sp = new Respond();
            try
            {
                DA_PostManager.DeletePost(memId,postId);
                sp.success = true;
                sp.messages = "ok";
            }
            catch (Exception e)
            {
                sp.success = false;
                sp.messages = e.Message;
            }
            return JsonConvert.SerializeObject(sp);
        }
        [WebMethod]
        public string registerForAPI(string email, string name, string nickName,  String dob)
        {
            Respond sp = new Respond();
            try
            {
                sp.messages = DA_MemberManager.registerForAPI(email, name, nickName, dob) + "";
                sp.success = true;
            }
            catch (Exception e)
            {
                sp.success = false;
                sp.messages = e.Message;
            }
            return JsonConvert.SerializeObject(sp);
        }
        [WebMethod]
        public string getListImageUploaded(int memId)
        {
            Respond sp = new Respond();
            try
            {
                Object obj = new Object();
                obj = DA_MemberManager.getListImageUploaded(memId);
                sp.success = true;
                sp.messages = JsonConvert.SerializeObject(obj);
            }
            catch (Exception e)
            {
                sp.success = false;
                sp.messages = e.Message;
            }
            return JsonConvert.SerializeObject(sp);
        }

        [WebMethod]
        public string getListNotify(int memId)
        {
            Respond sp = new Respond();
            try
            {
                Object obj = new Object();
                obj = DA_NotifyManage.getListNoi(memId);
                sp.success = true;
                sp.messages = JsonConvert.SerializeObject(obj);
            }
            catch (Exception e)
            {
                sp.success = false;
                sp.messages = e.Message;
            }
            return JsonConvert.SerializeObject(sp);
        }
    }
}
