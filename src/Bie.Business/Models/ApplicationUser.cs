using Bie.Business.Enums;
using Microsoft.AspNetCore.Identity;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bie.Business.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    public DateOnly BirthDate { get; set; }
    public string? CPF { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    [DefaultValue(StatusEnum.Active)]
    public StatusEnum Status { get; set; }
    public bool AutoGeneratedImage { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImagePrompt { get; set; }
    [NotMapped]
    public string? ImageBase64 { get; set; }

    public IList<Scheduling>? Schedulings { get; set; }
    public IList<CompanyEmployee>? UserCompanies { get; set; }
    [NotMapped]
    public IList<Company>? Companies { get; set; }
    public List<EmployeeServiceLink> Services { get; set; } = new();
}