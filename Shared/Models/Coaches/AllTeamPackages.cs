using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProServ.Shared.Models.Coaches;


public class AllTeamPackages
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PackageID { get; set; }
    public string PackageName { get; set; }
    public string PackageDescription { get; set; }
    public Decimal PackagePrice { get; set; }
    public int PackageMaxMembers { get; set; }
    public int PackageMaxAssistantCoaches { get; set; }
    public bool IsPublic { get; set; }

}