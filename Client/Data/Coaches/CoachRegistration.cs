using System;
using System.ComponentModel.DataAnnotations;

namespace ProServ.Client.Data.Coaches
{
    public class CoachRegistration
    {
        [Key]
        public string UserID { get; set; }
        public bool CompletedCoachingOnBoarding { get; set; }
        public string TeamName { get; set; }
        public string TeamLocationCity { get; set; }
        public string TeamLocationState { get; set; }

        public string CoachesCode { get; set; }
        public string UsersCode { get; set; }
        public string TeamCode { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public bool IsSchoolOrganization { get; set; }
        public string AffliatedSchool { get; set; }
        public string PackageID { get; set; }
        public string Email { get; set; }

        //All address variables
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }

        public CoachRegistration()
        {
            this.CompletedCoachingOnBoarding = false;
            this.TeamName = "";
            this.TeamLocationCity = "";
            this.TeamLocationState = "";
            this.CoachesCode = "";
            this.UsersCode = "";
            this.TeamCode = "";
            this.IsSchoolOrganization = false;
            this.AffliatedSchool = "";
            this.PackageID = "";
            this.Email = "";
            this.Address = "";
            this.Address2 = "";
            this.City = "";
            this.State = "";
            this.Zipcode = "";
        }

    }
}

