using Bie.Business.Models.Base;

namespace Bie.Business.Models;
public class EmployeeServiceLink : EntityBase
{
    public string EmployeeId { get; set; } = string.Empty;
    public string ServiceId { get; set; } = string.Empty;
    public virtual ApplicationUser Employee { get; set; } = new();
    public virtual CompanyServiceOffered Service { get; set; } = new();
}