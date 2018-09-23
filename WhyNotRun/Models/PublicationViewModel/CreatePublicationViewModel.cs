using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WhyNotRun.BO;

namespace WhyNotRun.Models.PublicationViewModel
{
    public class CreatePublicationViewModel
    {
        [Required(ErrorMessage = "O título é obrigatório")]
        public string Title { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória")]
        public string Text { get; set; }

        public List<string> Techies { get; set; }

        [Required(ErrorMessage = "O usuário é obrigatório")]
        public string UserId { get; set; }

        public Publication ToPublication()
        {
            List<ObjectId> techiesId = new List<ObjectId>();

            TechieBO techieBo = new TechieBO();
            if (Techies.Count > 0)
            {
                Task.Run(async () =>
                {
                    foreach (var name in Techies)
                    {
                        var techie = await techieBo.SearchTechiePerName(name);
                        if (techie != null)
                        {
                            techiesId.Add(techie.Id);
                        }
                        else
                        {
                            var newTechie = await techieBo.CreateTechie(new Techie { Name = name });
                            techiesId.Add(newTechie.Id);
                        }
                    }
                }).Wait();
            }



            return new Publication
            {
                Title = Title,
                Description = Text,
                Techies = techiesId,
                UserId = UserId.ToObjectId()
            };
        }

    }
}