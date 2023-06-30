using System;
using System.ComponentModel.DataAnnotations;
namespace ProServ.Shared.Models.UserInfo
{
    public class UserProfile
    {
        [Key]
        public string UserId { get; set; }
        public string? BestEvent { get; set; }
        public string? PrimaryGoal { get; set; }
        public string? WhyRun { get; set; }
        public string? RecordOrOlympics { get; set; }

        public UserProfile(string userId)
        {
            this.UserId = userId;
        }
        public UserProfile()
        {
            
        }

        
    }
}

