using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WhyNotRun.BO;
using WhyNotRun.Models.CommentViewModel;
using WhyNotRun.Models.TechieViewModel;

namespace WhyNotRun.Models.PublicationViewModel
{
    public class ViewPublicationViewModel
    {
        [JsonProperty(PropertyName = "id")]
        public ObjectId Id { get; set; }
        
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "datePublication")]
        public DateTime DateCreation { get; set; }

        [JsonProperty(PropertyName = "user")]
        public UserInfosViewModel User { get; set; }

        [JsonProperty(PropertyName = "reactions")]
        public ReactionsViewModel Reactions { get; set; }

        [JsonProperty(PropertyName = "technologies")]
        public List<TechiesViewModel> Techies { get; set; }

        [JsonProperty(PropertyName = "comments")]
        public List<CreatedCommentViewModel> Comments { get; set; }
        
        
        public ViewPublicationViewModel(Publication publication)
        {
            Id = publication.Id;
            Title = publication.Title;
            Description = publication.Description;
            
            #region Pega tecnologias

            if (publication.Techies.Count > 0)
            {
                Techies = new List<TechiesViewModel>();
                List<Task<Techie>> tasks = new List<Task<Techie>>();

                var techieBo = new TechieBO();
                foreach (var tecId in publication.Techies)
                    tasks.Add(Task.Run(() => techieBo.SearchTechie(tecId)));

                Task.WaitAll(tasks.ToArray());
                Parallel.ForEach(tasks, task => Techies.Add(new TechiesViewModel(task.Result)));
            }

            #endregion

            #region Dados usuario

            User = new UserInfosViewModel
            {
                Id = publication.UserId
            };

            var userBo = new UserBO();

            Task.Run(async () =>
            {
                var user = await userBo.SearchUserPerId(publication.UserId);
                if (user != null)
                {
                    User.Name = user.Name;
                    User.Picture = user.Picture;
                    User.Profession = user.Profession;
                }
            }).Wait();

            #endregion

            #region Reactions
            
            Reactions = new ReactionsViewModel
            {
                AgreeQuantity = publication.Likes.Count,
                DisagreeQuantity = publication.Dislikes.Count,
                Like = null                
            };

            PublicationBO publicationBo = new PublicationBO();

            Task.Run(async () =>
            {
                Reactions.Comments = (await publicationBo.SearchPublicationById(publication.Id)).Comments.Count;

            }).Wait();

            if (!string.IsNullOrEmpty(UtilBO.ValorAuthorizationHeader(System.Web.HttpContext.Current)))
            {
                var hashToken = UtilBO.ValorAuthorizationHeader(System.Web.HttpContext.Current);
                if (!string.IsNullOrEmpty(hashToken))
                {
                    Token token = new Token();
                    var decriptedToken = token.DecodeToken(hashToken);
                    var decriptedTokenValue = decriptedToken.Remove((decriptedToken.Count() - 2), 2).Remove(0, 7);


                    if (publication.Likes.Contains(decriptedTokenValue.ToObjectId()))
                    {
                        Reactions.Like = true;
                    }
                    else if (publication.Dislikes.Contains(decriptedTokenValue.ToObjectId()))
                    {
                        Reactions.Like = false;
                    }
                }
            }
            


            #endregion

            Comments = CreatedCommentViewModel.ToList(publication.Comments);

            DateCreation = publication.DateCreation;

        }

        public ViewPublicationViewModel()
        {

        }
        public static List<ViewPublicationViewModel> ToList(List<Publication> publications)
        {
            List<ViewPublicationViewModel> publicationsList = new List<ViewPublicationViewModel>();

            foreach (var publication in publications)
            {
                publicationsList.Add(new ViewPublicationViewModel(publication));
            }

            return publicationsList;

        }

    }

    public class UserInfosViewModel
    {
        [JsonProperty(PropertyName = "id")]
        public ObjectId Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "picture")]
        public string Picture { get; set; }

        [JsonProperty(PropertyName = "profession")]
        public string Profession { get; set; }
    }

    public class ReactionsViewModel
    {
        [JsonProperty(PropertyName = "agreeQuantity")]
        public int AgreeQuantity { get; set; }

        [JsonProperty(PropertyName = "disagreeQuantity")]
        public int DisagreeQuantity { get; set; }

        [JsonProperty(PropertyName = "comments")]
        public int Comments { get; set; }
        
        [JsonProperty(PropertyName = "like")]
        public bool? Like { get; set; }
    }

    



}