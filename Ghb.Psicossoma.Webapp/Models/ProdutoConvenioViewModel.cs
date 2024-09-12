﻿using System.ComponentModel.DataAnnotations;

namespace Ghb.Psicossoma.Webapp.Models
{
    public class ProdutoConvenioViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nome")]
        [StringLength(20)]
        public string Descricao { get; set; } = string.Empty;
    }
}
