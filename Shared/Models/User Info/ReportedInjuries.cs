using System;
using System.ComponentModel.DataAnnotations;
namespace ProServ.Shared.Models.UserInfo
{
    public class ReportedInjuries
    {
        [Key]
        public string UserId { get; set; }
        public DateTime InjuryDate { get; set; }
        public bool DuringWorkout { get; set; }
        public int Severity { get; set; }
        public string Comments { get; set; }

        public ReportedInjuries(string userId)
        {
            this.UserId = userId;
        }
        public ReportedInjuries()
        {

        }
    }
}

