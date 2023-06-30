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
    public string sValue1 { get; set; }

    [MaxLength(100)]
    public string sValue2 { get; set; }

    public TimeSpan tTime1 { get; set; }

    public TimeSpan tTime2 { get; set; }

    public TimeSpan sDistance1 { get; set; }

    public TimeSpan sDistance2 { get; set; }

    public int ParameterType { get; set; }

    public int BlockType { get; set; }

    // Navigation property
    [ForeignKey("BlockId")]
    public WorkoutBlock WorkoutBlock { get; set; }
}