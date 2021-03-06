﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            if (string.IsNullOrWhiteSpace(prefix)) 
                throw new ArgumentNullException(nameof(prefix));
            this.Prefix = prefix;
        }

        public virtual Starboard Starboard { get; set; }
        public virtual ICollection<StarboardMessage> StarboardMessages { get; set; }
        public virtual ICollection<GuildUser> GuildUsers { get; set; }
        public virtual ICollection<Sar> Sars { get; set; }
    }
}