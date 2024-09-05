namespace Kunigi.ViewModels;

public interface IValidate
{
    bool IsValid();
    
    IDictionary<string, string[]> GetValidationErrors();
}