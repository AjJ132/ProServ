using System;
using System.ComponentModel.DataAnnotations;

namespace ProServ.Shared.Models.UserInfo;

public class UserGoals
{
    [Key]
    public int GoalIndex { get; set; }
    public string UserId { get; set; }
    public string? Goal { get; set; }
    public DateTime GoalStartDate { get; set; }
    public DateTime GoalEndDate { get; set; }
    public bool CompletedGoal { get; set; }
    public DateTime DateCompleted { get; set; }

    public UserGoals(string userId)
    {
        this.UserId = userId;
    }
    public UserGoals()
    {

    }
}