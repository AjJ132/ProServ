using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProServ.Shared.Models.Workouts;

public class AssignedWorkout
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Index { get; set; }

    public int WorkoutId { get; set; }

    
    public DateTime WorkoutDate { get; set; }

    [MaxLength(150)]
    public string Notes { get; set; }

    [MaxLength(450)]
    public string AssigneeId { get; set; }

    public bool ReportBack { get; set; }

[MaxLength(100)]
    public string WorkoutName {get;set;}

    // Navigation property
    [ForeignKey("WorkoutId")]
    public Workout Workout { get; set; }
}