using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WhyNotRun.BO;

namespace WhyNotRun.Models.PublicationViewModel
{
    public class ListOfPublicationsViewModel
    {
        [JsonProperty("quantityOfPages")]
        public double QuantityOfPages { get; set; }

        [JsonProperty("publications")]
        public List<ViewPublicationViewModel> Publications { get; set; }

        public ListOfPublicationsViewModel(List<Publication> publications)
        {
            PublicationBO publicationBo = new PublicationBO();

            Task.Run(async () =>
            {
                QuantityOfPages = Math.Ceiling((Convert.ToDouble((await publicationBo.ListPublications()).Count) / 20));
            }).Wait();

            Publications = new List<ViewPublicationViewModel>();
            Publications = ViewPublicationViewModel.ToList(publications);


        }
    }
}