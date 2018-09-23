using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WhyNotRun.BO;

namespace WhyNotRun.Models.CommentViewModel
{
    public class CreatedCommentViewModel
    {
        [JsonProperty("id")]
        public ObjectId Id { get; set; }

        [JsonProperty("text")]
        public string Description { get; set; }

        [JsonProperty("dateComment")]
        public DateTime DateComment { get; set; }

        [JsonProperty("user")]
        public UserCommentInfosViewModel User { get; set; }


        public CreatedCommentViewModel(Comment comment)
        {
            User = new UserCommentInfosViewModel
            {
                Id = comment.UserId
            };
            var userBo = new UserBO();

            Task.Run(async () =>
            {
                var user = await userBo.SearchUserPerId(comment.UserId);
                if (user != null)
                {
                    User.Name = user.Name;
                    User.Picture = user.Picture;
                }
            }).Wait();

            Id = comment.Id;
            Description = comment.Description;
            DateComment = comment.DateCreation.ToLocalTime();
            
        }

        public static List<CreatedCommentViewModel> ToList(List<Comment> comments)
        {
            List<CreatedCommentViewModel> createdComments = new List<CreatedCommentViewModel>();
            foreach (var comment in comments)
            {
                createdComments.Add(new CreatedCommentViewModel(comment));
            }

            return createdComments;
        }

    }

    public class UserCommentInfosViewModel
    {
        [JsonProperty(PropertyName = "id")]
        public ObjectId Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "picture")]
        public string Picture { get; set; }
    }
}