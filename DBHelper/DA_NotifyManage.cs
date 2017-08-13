using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBHelper
{
    public class DA_NotifyManage
    {
        public static void putNotify(int senderId, int receiverId, string type, int postId)
        {
            var db = new SocialTGDBDataContext();
            Notify no = new Notify();
            no.sender = senderId;
            no.receiver = receiverId;
            no.type = type;
            no.createTime = DateTime.UtcNow;
            no.postId = postId;
            db.Notifies.InsertOnSubmit(no);
            db.SubmitChanges();
        }

        public static Object getListNoi(int memId)
        {
            var db = new SocialTGDBDataContext();
            var q = db.Notifies.Where(p => p.receiver == memId).OrderByDescending(p=>p.createTime).Select(p => new { senderName = DA_MemberManager.getDisplayName(p.sender ?? 0), senderAvatar = DA_MemberManager.getMember(p.sender ?? 0).Avatar, mess = p.type, p.createTime, p.postId}).ToList();
            return q;
        }
    }
}
