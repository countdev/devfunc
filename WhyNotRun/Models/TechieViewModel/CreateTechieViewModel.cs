using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WhyNotRun.Models.TechieViewModel
{
    public class CreateTechieViewModel
    {
        [Required(ErrorMessage = "O nome da Tecnologia é obrigatória")]
        public string Name { get; set; }

        public Techie ToTechie()
        {
            return new Techie
            {
                Name = Name
            };
        }
    }
}