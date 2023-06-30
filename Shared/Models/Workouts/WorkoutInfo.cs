using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProServ.Shared.Models.Workouts
{
    public class WorkoutInfo
    {
        [Key]
        public int WorkoutID { get; set; }

        [MaxLength(45)]
        public string WorkoutName { get; set; }

        [MaxLength(150)]
        public string Description { get; set; }

        [MaxLength(30)]
        public string Difficulty { get; set; }

        public int TeamID { get; set; }

        [MaxLength(450)]
        public string CoachId { get; set; }

        public bool SharePublic { get; set; }

        public Workout Workout { get; set; }
    }
}
