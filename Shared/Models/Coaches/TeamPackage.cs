using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProServ.Shared.Models.Coaches;


public class TeamPackage
{
    [Key]
    public int PackageID { get; set; }
    public int TeamID { get; set; }
    public DateTime PackageStart { get; set; }
    public DateTime PackageEnd { get; set; }

    public virtual Team Team { get; set; }
}