using System;
using System.ComponentModel.DataAnnotations;

namespace ProServ.Shared.Models.UserInfo;

public class ProfileOnboarding
{
    [Key]
    public string UserId { get; set; }
    public bool Completed { get; set; }


    public ProfileOnboarding(string userId)
    {
        this.UserId = userId;
    }
    public ProfileOnboarding()
    {

    }
}