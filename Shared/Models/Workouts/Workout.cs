using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProServ.Shared.Models.Workouts
{
    public class Workout
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WorkoutId { get; set; }

        //public int WorkoutTypeId { get; set; }

        [MaxLength(100)]
        public string WorkoutName { get; set; }

        [MaxLength(450)]
        public string CoachId { get; set; }

        [MaxLength(150)]
        public string Notes { get; set; }

        [NotMapped]
        public DateTime DateToComplete { get; set; }
        [NotMapped]
        [Required]
        public string CoachName { get; set; }



        // Navigation properties
        public virtual ICollection<WorkoutBlock> WorkoutBlocks { get; set; }

        //public virtual WorkoutInfo WorkoutInfo { get; set; }

        //**Have to init this list or else it throws a required error when transporting over API not sure why 09/05/2023
        public virtual ICollection<AssignedWorkout> AssignedWorkouts { get; set; } = new List<AssignedWorkout>();
    }

}
