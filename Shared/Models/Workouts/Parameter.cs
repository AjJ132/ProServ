using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProServ.Shared.Models.Workouts;

public class Parameter
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ParameterId { get; set; }

    public int BlockId { get; set; }

    [MaxLength(100)]
    public string? SValue1 { get; set; }

    [MaxLength(100)]
    public string? SValue2 { get; set; }

    public TimeSpan? TTime1 { get; set; }

    public TimeSpan? TTime2 { get; set; }

    public double? sDistance1 { get; set; }

    public double? sDistance2 { get; set; }

    //public int ParameterType { get; set; }


    // Navigation properties
    public virtual WorkoutBlock? WorkoutBlock { get; set; }

}