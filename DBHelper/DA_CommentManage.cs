using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBHelper
{
    public class DA_CommentManage
    {
        public static void comment(int memId, int PostId, string content)
        {
            var db = new SocialTGDBDataContext();
            var cmt = new Comment();
            cmt.MemberId = memId;
            cmt.PostId = PostId;
            cmt.Content = content;
            cmt.CreateTime = DateTime.UtcNow;
            cmt.isActivate = true;
            db.Comments.InsertOnSubmit(cmt);
            var post = db.Posts.FirstOrDefault(m => m.id == PostId);
            post.TotalCmt++;
            db.SubmitChanges();
        }
        public static Object getListCmt(int postId)
        {
            var db = new SocialTGDBDataContext();
            var q = db.Comments.Where(p => p.PostId == postId && p.isActivate == true)
                .Select(p => new {p.Member.Email, p.Member.NickName, p.Member.Avatar, p.CreateTime, p.Content }).ToList();
            return q;
        }
    }
}
