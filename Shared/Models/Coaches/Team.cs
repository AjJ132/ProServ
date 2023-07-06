using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProServ.Shared.Models.Coaches;

public class Team
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TeamID { get; set; }
    public string TeamName { get; set; }
    public string Location { get; set; }
    public string? CoachesCode { get; set; }
    public string? UsersCode { get; set; }
    public bool Terminated { get; set; }
    public string OwnerID { get; set; }

    public virtual TeamInfo TeamInfo { get; set; }
    public virtual TeamPackage TeamPackage { get; set; }
}