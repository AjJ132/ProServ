// File: WorkoutHistory.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProServ.Shared.Models.Workouts
{
    public class WorkoutHistory
    {
        [Key]
        public int Index { get; set; }

        [MaxLength(450)]
        public string UserId { get; set; }

        [MaxLength(30)]
        public string WorkoutDate { get; set; }

        [MaxLength(150)]
        public string Notes { get; set; }

        public int WorkoutID { get; set; }

    }
}
