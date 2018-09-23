using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhyNotRun.Models.UserViewModel
{
    public class VisualizationUserViewModel
    {
        public VisualizationUserViewModel(User user)
        {
            Id = user.Id;
            Name = user.Name;
            Email = user.Email;
            Profession = user.Profession;
            Picture = user.Picture;
        }

        [JsonProperty(PropertyName = "id")]
        public ObjectId Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "profession")]
        public string Profession { get; set; }
        [JsonProperty(PropertyName = "picture")]
        public string Picture { get; set; }
    }
}