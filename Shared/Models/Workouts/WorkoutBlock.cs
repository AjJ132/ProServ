using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProServ.Shared.Models.Workouts;

public class WorkoutBlock
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BlockId { get; set; }

    public int WorkoutId { get; set; }

    [MaxLength(50)]
    public string BlockName { get; set; }

    public int BlockType { get; set; }

    public Workout Workout { get; set; }

    public virtual ICollection<Parameter> Parameters { get; set; }
}

