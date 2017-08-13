using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBHelper
{
    public class DA_FollowManage
    {
        public static void followMember(int memId, int followId)
        {
            var db = new SocialTGDBDataContext();
            var q = db.Follows.Where(p => p.followerId == memId && p.followingId == followId).ToList();
            if (q.Count() > 0)
                db.Follows.DeleteOnSubmit(q[0]);
            else
            {
                Follow f = new Follow();
                f.followerId = memId;
                f.followingId = followId;
                f.isAgree = true;
                db.Follows.InsertOnSubmit(f);
            }
            db.SubmitChanges();
        }
        public static void agreeFollow(int memId, int followId)
        {
            var db = new SocialTGDBDataContext();
            Follow f = db.Follows.FirstOrDefault(p => p.followerId == memId && p.followingId == followId);
            f.isAgree = true;
            db.SubmitChanges();
        }
        public static int getCountFollower(int id)
        {
            var db = new SocialTGDBDataContext();
            return db.Follows.Where(p => p.followingId == id).Count();
        }
        public static int getCountFollowing(int id)
        {
            var db = new SocialTGDBDataContext();
            return db.Follows.Where(p => p.followerId == id).Count();
        }
        public static Object getListFollower(int id)
        {
            var db = new SocialTGDBDataContext();
            var q = db.Follows.Where(p => p.followingId == id && p.isAgree == true).Select(p => new { p.followerId, p.Member.Name, p.Member.Email, p.Member.Avatar, p.Member.NickName });
            return q.ToList();
        }
        public static Object getListFollowing(int id)
        {
            var db = new SocialTGDBDataContext();
            var q = db.Follows.Where(p => p.followerId == id && p.isAgree == true).Select(p => new { p.followingId, p.Member1.Name, p.Member1.Email, p.Member1.Avatar, p.Member1.NickName });
            return q.ToList();
        }
        public static bool checkFollowed(int memid, int following)
        {
            if (following == -1)
                return false;
            var db = new SocialTGDBDataContext();
            var q = db.Follows.Where(p => p.followerId == memid && p.followingId == following).Count();
            return q > 0;
        }
    }
}
