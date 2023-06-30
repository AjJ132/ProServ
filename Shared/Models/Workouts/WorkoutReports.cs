// File: WorkoutReports.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProServ.Shared.Models.Workouts
{
    public class WorkoutReports
    {
        [Key]
        public int WorkoutId { get; set; }

        [MaxLength(450)]
        public string AssigneeID { get; set; }

        public short StarRating { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }

        public DateTimeOffset DateReported { get; set; }

        // Navigation property
        [ForeignKey("WorkoutId")]
        public Workout Workout { get; set; }
    }
}
