using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using archive.Data.Entities;

namespace archive.Models.User
{
    public class ProfileViewModel
    {
        [Display(Name = "Awatar")]
        public byte[] AvatarImage { get; set; }
        
        [Display(Name = "Nazwa użytkownika")]
        public string UserName { get; set; }
        
        [Display(Name = "Strona Domowa")]
        public string HomePage { get; set; }
        
        [Display(Name = "Adres email")]
        public string Email { get; set; }
        
        [Display(Name = "Numer telefonu")]
        public string Phone { get; set; }
        
        [Display(Name = "Ostatnio aktywny")]
        public DateTime? LastActive { get; set; }

        [Display(Name = "Achievementy")]

        public ICollection<Achievement> UserAchievements { get; set; }
    }
}