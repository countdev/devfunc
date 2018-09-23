using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WhyNotRun.BO;
using WhyNotRun.Models.TechieViewModel;

namespace WhyNotRun.Models.PublicationViewModel
{
    public class SugestPublicationViewModel
    {
        [JsonProperty(PropertyName = "id")]
        public ObjectId Id { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "technologies")]
        public List<TechiesViewModel> Techies { get; set; }
        
        [JsonIgnore]
        [JsonProperty(PropertyName = "points")]
        public int Points { get; set; }

        public SugestPublicationViewModel(Publication publication)
        {
            Id = publication.Id;
            Title = publication.Title;
            
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

            Points = publication.Likes.Count - publication.Dislikes.Count;


        }

        public SugestPublicationViewModel()
        {

        }

        public static List<SugestPublicationViewModel> ToList(List<Publication> publications)
        {
            List<SugestPublicationViewModel> sugests = new List<SugestPublicationViewModel>();
            foreach (var publication in publications)
            {
                sugests.Add(new SugestPublicationViewModel(publication));
            }

            return sugests.OrderByDescending(a => a.Points).ToList();
        }
    }
}