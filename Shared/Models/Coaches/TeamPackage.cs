using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProServ.Shared.Models.Coaches;


public class TeamPackage
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int EntryId { get; set;}
    public int PackageID { get; set; }
    public int TeamID { get; set; }
    public DateTime PackageStart { get; set; }
    public DateTime PackageEnd { get; set; }

    public virtual Team Team { get; set; }
}