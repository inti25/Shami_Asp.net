using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace DBHelper
{
    public class DA_MemberManager
    {
        public static bool login(string email, string pass, out Object mem)
        {
            var db = new SocialTGDBDataContext();
            mem = null;
            if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(pass))
                return false;
            var q = db.Members.Where(m => m.Email == email && m.Password == pass).ToList();
            if (q.Count() < 1)
                return false;
            q[0].LastLogin = DateTime.UtcNow;
            db.SubmitChanges();
            var obj = q.Select(m => new { m.id, m.Name, m.Password, m.Email, m.DateOfBirth, m.Avatar, m.isActivate, m.isPrivate, m.LastLogin, m.RegisterTime }).ToList();
            mem = obj[0];
            return true;
        }

        public static Object getinfoUser(int memId)
        {
            var db = new SocialTGDBDataContext();
            var q = db.Members.Where(m => m.id == memId).ToList();
            if(q == null)
                throw new System.InvalidOperationException("Email is invalid !");
            var obj = q.Select(m => new { m.id, m.Name, m.NickName, m.Email, m.DateOfBirth, m.Avatar, m.isActivate, m.isPrivate, m.LastLogin, m.RegisterTime, FollowerC = DA_FollowManage.getCountFollower(m.id), FollowingC = DA_FollowManage.getCountFollowing(m.id), totalPost = DA_PostManager.getPostCount(m.id) }).ToList();
            return obj[0];
        }

        public static Object getInfoOtherUser(int memId,int currentMemId)
        {
            var db = new SocialTGDBDataContext();
            var q = db.Members.Where(m => m.id == memId).ToList();
            if (q == null)
                throw new System.InvalidOperationException("Email is invalid !");
            var obj = q.Select(m => new { m.id, m.Name, m.NickName, m.Email, m.DateOfBirth, m.Avatar, m.isActivate, m.isPrivate, m.LastLogin, m.RegisterTime, FollowerC = DA_FollowManage.getCountFollower(m.id), FollowingC = DA_FollowManage.getCountFollowing(m.id), totalPost = DA_PostManager.getPostCount(m.id), isFollowed = DA_FollowManage.checkFollowed(currentMemId,memId) }).ToList();
            return obj[0];
        }

        public static List<string> listToken()
        {
            var db = new SocialTGDBDataContext();
            return db.Members.Select(p => p.Hash).ToList();
        }

        public static int insert(String email, String pass, String name, String nickname, String dob, String Avatar)
        {
            var db = new SocialTGDBDataContext();
            var q = db.Members.Where(p => p.Email == email).Count();
            if(q > 0)
                throw new System.InvalidOperationException("Email Address Already in Use!");
            Member m = new Member();
            m.Email = email;
            m.Password = pass;
            m.Name = name;
            m.NickName = nickname;
            if (dob != null)
                m.DateOfBirth = DateTime.ParseExact(dob, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            if (String.IsNullOrEmpty(Avatar))
                m.Avatar = "http://www.petzoned.com/assets/default_avatar-468e8f42276a2fd234b034f28536570b.png";
            else
                m.Avatar = Avatar;
            m.isActivate = true;
            m.RegisterTime = DateTime.UtcNow;
            db.Members.InsertOnSubmit(m);
            db.SubmitChanges();
            return m.id;
        }
        public static void update(int memId,String name, String nickname, String dob)
        {
            var db = new SocialTGDBDataContext();
            var q = db.Members.FirstOrDefault(p => p.id == memId);
            if (!String.IsNullOrEmpty(name))
                q.Name = name;
            if (!String.IsNullOrEmpty(nickname))
                q.NickName = nickname;
            if (!String.IsNullOrEmpty(dob))
                q.DateOfBirth = DateTime.ParseExact(dob, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            db.SubmitChanges();
        }
        public static int registerForAPI(string email, string name, string nickname,  String dob)
        {
            var db = new SocialTGDBDataContext();
            var q = db.Members.Where(p => p.Email == email).ToList();
            if (q.Count > 0)
                return q[0].id;
            return insert(email, "   ", name, nickname, dob, null);
        }
        public static Object getListImageUploaded(int memId)
        {
            var db = new SocialTGDBDataContext();
            var q = db.Posts.Where(p => p.MemberId == memId).Select(p => new { p.id, p.imageUrl }).OrderByDescending(p=>p.id).ToList();
            return q;
        }
        public static bool isExist(string email)
        {
            var db = new SocialTGDBDataContext();
            var q = db.Members.Where(p => p.Email == email).ToList();
            if (q.Count > 0)
                return true;
            return false;
        }
        public static void updateAvatar(string email, string urlImage)
        {
            var db = new SocialTGDBDataContext();
            var q = db.Members.FirstOrDefault(p => p.Email == email);
            q.Avatar = urlImage;
            db.SubmitChanges();
        }
        public static void updateTokenId(int id, string tokenId)
        {
            var db = new SocialTGDBDataContext();
            var q = db.Members.FirstOrDefault(p => p.id == id);
            q.Hash = tokenId;
            db.SubmitChanges();
        }
        public static string getTokenId(int id)
        {
            var db = new SocialTGDBDataContext();
            var q = db.Members.FirstOrDefault(p => p.id == id);
            return q.Hash;
        }
        public static string getDisplayName(int id)
        {
            var db = new SocialTGDBDataContext();
            var q = db.Members.FirstOrDefault(p => p.id == id);
            if (!String.IsNullOrEmpty(q.NickName))
                return q.NickName;
            if (!String.IsNullOrEmpty(q.Name))
                return q.Name;
            return q.Email;
        }

        public static string getTokenByPost(int postId)
        {
            var db = new SocialTGDBDataContext();
            var post = db.Posts.FirstOrDefault(p => p.id == postId);
            return post.Member.Hash;
        }

        public static Member getMember(string email)
        {
            var db = new SocialTGDBDataContext();
            return db.Members.FirstOrDefault(p => p.Email == email);
        }

        public static Member getMember(int id)
        {
            var db = new SocialTGDBDataContext();
            return db.Members.FirstOrDefault(p => p.id == id);
        }

        public static void UpdatePass(string email,string oldpass,string newpass)
        {
            var db = new SocialTGDBDataContext();
            var q = db.Members.FirstOrDefault(p => p.Email == email);
            if (q == null)
                throw new System.InvalidOperationException("Email is invalid !");
            if(q.Password != oldpass)
                throw new System.InvalidOperationException("Old Password is incorrect !");
            q.Password = newpass;
            db.SubmitChanges();
        }

        public static void UpdatePass(string email, string newpass)
        {
            var db = new SocialTGDBDataContext();
            var q = db.Members.FirstOrDefault(p => p.Email == email);
            if (q == null)
                throw new System.InvalidOperationException("Email is invalid !");
            q.Password = newpass;
            db.SubmitChanges();
        }

        public static void likeOrUnlikePost(int memId, int postId)
        {
            var db = new SocialTGDBDataContext();
            var q = db.Likes.Where(p => p.MemberId == memId && p.PostId == postId);
            if(q.Count() > 0)
            {
                db.Likes.DeleteOnSubmit(q.ToList()[0]);
                var post = db.Posts.FirstOrDefault(m => m.id == postId);
                post.TotalLike--;
            }
            else
            {
                Like l = new Like();
                l.MemberId = memId;
                l.PostId = postId;
                l.CreateTime = DateTime.UtcNow;
                db.Likes.InsertOnSubmit(l);
                var post = db.Posts.FirstOrDefault(m => m.id == postId);
                post.TotalLike++;
            }
            db.SubmitChanges();
        }

        public static bool isLiked(int memId, int postId)
        {
            var db = new SocialTGDBDataContext();
            var q = db.Likes.Where(p => p.MemberId == memId && p.PostId == postId).Count();
            return q > 0;
        }
    }
}
