using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhyNotRun.Models.TechieViewModel
{
    public class TechiesViewModel
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        public TechiesViewModel(Techie techie)
        {
            Name = techie.Name;
        }
        
        public static List<TechiesViewModel> ToList(List<Techie> techies)
        {
            List<TechiesViewModel> techiesView = new List<TechiesViewModel>();
            foreach (var techie in techies)
            {
                techiesView.Add(new TechiesViewModel(techie));
            }

            return techiesView;
        }
    }
}