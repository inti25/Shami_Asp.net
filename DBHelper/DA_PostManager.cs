using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DBHelper
{
    public class DA_PostManager
    {
        public static Object get10PostPublic(int memId)
        {
            var db = new SocialTGDBDataContext();
            var q = db.Posts.Where(p => p.isPrivate == false && p.isActive == true).OrderByDescending(p => p.id)
                .Select(p => new
                {
                    p.id,
                    p.Title,
                    p.Description,
                    p.CreateTime,
                    p.isVideo,
                    p.isYouTube,
                    p.imageUrl,
                    p.videoUrl,
                    p.TotalCmt,
                    p.TotalLike,
                    p.ViewCount,
                    p.MemberId,
                    Name = DA_MemberManager.getDisplayName(p.MemberId??1),
                    p.Member.Avatar,
                    isFollowed = DA_FollowManage.checkFollowed(memId, p.MemberId ?? -1),
                    isLiked = DA_MemberManager.isLiked(memId, p.id)
                }).Take(10);
            return q.ToList();
        }
        public static Object getNext10PostPublic(int memId,int postId)
        {
            var db = new SocialTGDBDataContext();
            var q = db.Posts.Where(p => p.isPrivate == false && p.isActive == true && p.id < postId).OrderByDescending(p => p.id)
                .Select(p => new
                {
                    p.id,
                    p.Title,
                    p.Description,
                    p.CreateTime,
                    p.isVideo,
                    p.isYouTube,
                    p.imageUrl,
                    p.videoUrl,
                    p.TotalCmt,
                    p.TotalLike,
                    p.ViewCount,
                    p.MemberId,
                    Name = DA_MemberManager.getDisplayName(p.MemberId ?? 1),
                    p.Member.Avatar,
                    isFollowed = DA_FollowManage.checkFollowed(memId, p.MemberId ?? -1),
                    isLiked = DA_MemberManager.isLiked(memId, p.id)
                }).Take(10);
            return q.ToList();
        }
        public static Object get10PostByFollowing(int memId)
        {
            var db = new SocialTGDBDataContext();
            var q = db.Posts.Where(p => p.isActive == true && DA_FollowManage.checkFollowed(memId, p.MemberId ?? -1)).OrderByDescending(p => p.id)
                .Select(p => new
                {
                    p.id,
                    p.Title,
                    p.Description,
                    p.CreateTime,
                    p.isVideo,
                    p.isYouTube,
                    p.imageUrl,
                    p.videoUrl,
                    p.TotalCmt,
                    p.TotalLike,
                    p.ViewCount,
                    p.MemberId,
                    Name = DA_MemberManager.getDisplayName(p.MemberId ?? 1),
                    p.Member.Avatar,
                    isFollowed = true,
                    isLiked = DA_MemberManager.isLiked(memId, p.id)
                }).Take(10);
            return q.ToList();
        }

        public static Object getNext10PostByFollowing(int memId, int postId)
        {
            var db = new SocialTGDBDataContext();
            var q = db.Posts.Where(p => p.isActive == true && DA_FollowManage.checkFollowed(memId, p.MemberId ?? -1) && p.id < postId).OrderByDescending(p => p.id)
                .Select(p => new
                {
                    p.id,
                    p.Title,
                    p.Description,
                    p.CreateTime,
                    p.isVideo,
                    p.isYouTube,
                    p.imageUrl,
                    p.videoUrl,
                    p.TotalCmt,
                    p.TotalLike,
                    p.ViewCount,
                    p.MemberId,
                    Name = DA_MemberManager.getDisplayName(p.MemberId ?? 1),
                    p.Member.Avatar,
                    isFollowed = true,
                    isLiked = DA_MemberManager.isLiked(memId, p.id)
                }).Take(10);
            return q.ToList();
        }

        public static Object insertPost(string title, string description, string imageUrl, string videoUrl, int memId, bool isVideo, bool isYoutube, bool isPrivate)
        {
            Post p = new Post();
            p.Title = title;
            p.Description = description;
            p.imageUrl = imageUrl;
            p.videoUrl = videoUrl;
            p.MemberId = memId;
            p.isVideo = isVideo;
            p.isYouTube = isYoutube;
            p.isActive = true;
            p.CreateTime = DateTime.UtcNow;
            p.isPrivate = isPrivate;
            p.ViewCount = 0;
            p.TotalLike = 0;
            p.TotalCmt = 0;

            var db = new SocialTGDBDataContext();
            db.Posts.InsertOnSubmit(p);
            db.SubmitChanges();

            JObject obj = new JObject();
            obj.Add("postId", p.id);
            obj.Add("videoUrl", p.videoUrl);
            return obj;
        }

        public static void UpdateVideoUrl(int id, string videourl, string imageurl)
        {
            var db = new SocialTGDBDataContext();
            var q = db.Posts.FirstOrDefault(p => p.id == id);
            q.videoUrl = videourl;
            q.imageUrl = imageurl;
            q.isActive = true;
            db.SubmitChanges();
        }

        public static void DeletePost(int memId,int postId)
        {
            var db = new SocialTGDBDataContext();
            var q = db.Posts.FirstOrDefault(p => p.id == postId);
            if (q.MemberId == memId || memId == 1)
            {
                db.Posts.DeleteOnSubmit(q);
                db.SubmitChanges();
            }
            else
                throw new System.InvalidOperationException("You don't have permission to delete this post!");
        }

        public static Post getPost(int id)
        {
            var db = new SocialTGDBDataContext();
            return db.Posts.FirstOrDefault(p => p.id == id);
        }

        public static int getPostCount(int id)
        {
            var db = new SocialTGDBDataContext();
            return db.Posts.Where(p => p.MemberId == id).Count();
        }

        public static int getCommentCount(int postId)
        {
            var db = new SocialTGDBDataContext();
            return db.Comments.Where(p => p.PostId == postId).Count();
        }
    }
}
