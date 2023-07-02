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
    public string PackageSubtext { get; set; }
    public Decimal PackagePriceMonthly { get; set; }
    public Decimal PackagePriceYearly { get; set; }
    public int PackageMaxMembers { get; set; }
    public int PackageMaxAssistantCoaches { get; set; }

    [NotMapped]
    public List<string> PackageFeatures { get; set; }

    public bool IsPublic { get; set; }

}