using System;
using System.ComponentModel.DataAnnotations;

namespace ProServ.Shared.Models.UserInfo
{
    public class UserTrackRecords
    {
        [Key]
        public string UserId { get; set; }

        public string? _400Time { get; set; }
        public string? _800Time { get; set; }
        public string? _1KTime { get; set; }
        public string? _1500time { get; set; }
        public string? _1600Time { get; set; }
        public string? _3KTime { get; set; }
        public string? _3200Time { get; set; }
        public string? _5KTime { get; set; }
        public string? _8KTime { get; set; }
        public string? _10KTime { get; set; }


        public UserTrackRecords(string userId)
        {
            this.UserId = userId;
        }
        public UserTrackRecords()
        {

        }
    }
}