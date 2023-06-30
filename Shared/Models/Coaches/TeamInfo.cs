using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProServ.Shared.Models.Coaches;

public class TeamInfo
{
    [Key]
    public int TeamID { get; set; }
    public DateTime DateCreated { get; set; }
    public string OwnerID { get; set; }
    public bool IsSchoolOrganization { get; set; }
    public int TeamPackageID { get; set; }
    public int TimeChanged { get; set; }

    public virtual Team Team { get; set; }
}