using System;
using System.ComponentModel.DataAnnotations;

namespace ProServ.Shared.Models.UserInfo;

public class UserCoachingStyle
{
    [Key]
    public string UserId { get; set; }
    public string? CoachingStyle { get; set; }
    public string? Notes { get; set; }
    public string? WeightsIntensity { get; set; }
    public bool InjuryHistory { get; set; }
    public string? InjuryHistoryNotes { get; set; }
    public bool InterestedInDiet { get; set; }
    public string DietIntensity { get; set; }


    public UserCoachingStyle(string userId)
    {
        this.UserId = userId;
    }
    public UserCoachingStyle()
    {

    }
}