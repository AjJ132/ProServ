
using System;
using System.ComponentModel.DataAnnotations;


namespace ProServ.Shared.Models.UserInfo
{
    public class UserInformation
    {

        [Key]
        public string UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Height { get; set; }
        public int Weight { get; set; }
        public bool IsInHighschool { get; set; }
        public string? School { get; set; }
        public DateTime Birthday { get; set; }
        public string? Gender { get; set; }
        public string? UserType { get; set; }
        public string? ReportsTo { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastAccessed { get; set; }
        public bool ActiveUser { get; set; }
        public int TeamID { get; set; }

        public UserInformation(string userId)
        {
            this.UserId = userId;
            this.UserType = "User"; //User,Coach,Admin //Set manually for now and can update in database
        }
        public UserInformation()
        {

        }


    }
}
