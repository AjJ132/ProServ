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

        public int WorkoutTypeId { get; set; }

		[MaxLength(100)]
		public string WorkoutName { get; set; }

		[MaxLength(450)]
        public string CoachId { get; set; }

        [MaxLength(150)]
        public string Notes { get; set; }



        // Navigation properties
        public ICollection<WorkoutBlock> WorkoutBlocks { get; set; }

        public virtual WorkoutInfo WorkoutInfo { get; set; }
    }

}
