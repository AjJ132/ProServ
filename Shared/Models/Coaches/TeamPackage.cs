using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProServ.Shared.Models.Coaches;


public class TeamPackage
{
    [Key]
    public int TeamID { get; set; }
    public int PackageID { get; set; }
    public DateTime PackageStart { get; set; }
    public DateTime PackageEnd { get; set; }

    [JsonIgnore]
    public virtual Team Team { get; set; }
}