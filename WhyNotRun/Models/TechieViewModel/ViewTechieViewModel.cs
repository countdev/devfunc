using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WhyNotRun.BO;

namespace WhyNotRun.Models.TechieViewModel
{
    public class ViewTechieViewModel
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "points")]
        public int Points { get; set; }

        [JsonProperty(PropertyName = "posts")]
        public int Posts { get; set; }

        public ViewTechieViewModel(Techie techie)
        {
            Name = techie.Name;

            var publicationBo = new PublicationBO();
            Task.Run(async () =>
            {
                var publications = await publicationBo.ListPublicationsPerTechieId(techie.Id);

                Points = publications.Sum(a => a.Likes.Count) - publications.Sum(a => a.Dislikes.Count);
                Posts = publications.Count;

            }).Wait();

        }
        public ViewTechieViewModel()
        {

        }

        public static List<ViewTechieViewModel> ToList(List<Techie> techies)
        {
            List<ViewTechieViewModel> viewTechies = new List<ViewTechieViewModel>();

            foreach (var techie in techies)
            {
                viewTechies.Add(new ViewTechieViewModel(techie));
            }

            var b = viewTechies.OrderByDescending(a => a.Points).Take(20).ToList();
            return b;//alterar isso
        }

    }
}