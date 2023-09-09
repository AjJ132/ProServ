using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProServ.Shared.Models.Workouts;

public class WorkoutBlock
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BlockId { get; set; }

    public int BlockOrder { get; set; }

    public int WorkoutId { get; set; }

    [MaxLength(50)]
    [Required]
    public string BlockName { get; set; }

    public string BlockType { get; set; }


    public virtual Workout? Workout { get; set; }
    public virtual ICollection<Parameter> Parameters { get; set; } = new List<Parameter>();
}

