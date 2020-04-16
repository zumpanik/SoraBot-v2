﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoraBot.Data.Models.SoraDb
{
    public class Guild
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong Id { get; set; }

        [Required] public string Prefix { get; set; }

        public Guild(ulong id, string prefix = "$")
        {
            this.Id = id;
            this.Prefix = prefix;
        }
    }
}