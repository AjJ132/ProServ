using System;
using System.ComponentModel.DataAnnotations;
namespace ProServ.Shared.Models.UserInfo
{
    public class UserSubscription
    {
        [Key]
        public string UserId { get; set; }
        public string? SubscriptionType { get; set; }
        public bool SubscriptionStatus { get; set; }
        public DateTime SubscriptionStartDate { get; set; }
        public DateTime SubscriptionEndDate { get; set; }
        public bool AutoRenew { get; set; }
        public bool SubscriptionOnboarding { get; set; }
        public DateTime? SubscriptionCancelDate { get; set; }
        public string DiscountsApplied { get; set; }

        public UserSubscription(string userId)
        {
            this.UserId = userId;
        }
        public UserSubscription()
        {

        }
    }
}