using System.ComponentModel.DataAnnotations;
using Kunigi.Data;

namespace Kunigi.CustomAttributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class UniqueTeamNameAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var dbContext = (DataContext)validationContext.GetService(typeof(DataContext));
        var existingTeam = dbContext?.Teams.FirstOrDefault(t => t.Name == (string)value);

        return existingTeam != null ? new ValidationResult("Υπάρχει ήδη ομάδα με αυτό το όνομα.") : ValidationResult.Success;
    }
}