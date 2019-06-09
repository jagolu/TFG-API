﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    public class UserBet
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }

        public Guid FootballBetId { get; set; }
        public FootballBet FootballBet { get; set; }

        public Guid userId { get; set; }
        public User User { get; set; }

        [Required]
        public int bet { get; set; }

        public Team Team { get; set; }

        public int? homeGoals { get; set; }
        public int? awayGoals { get; set; }

        [Required]
        public DateTime dateDone { get; set; } = DateTime.Now;

        [Required]
        public bool closed { get; set; } = false;

        [Required]
        public int penalization { get; set; } = 0;
    }
}