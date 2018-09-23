using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WhyNotRun.Models.PublicationViewModel
{
    public class ReactPublicationViewModel
    {
        [Required(ErrorMessage = "O usuário é obrigatório")]
        public string UserId { get; set; }
        

        [Required(ErrorMessage = "A reação é obrigatoria")]
        public bool? Like { get; set; }

    }
}