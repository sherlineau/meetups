using System.ComponentModel.DataAnnotations;

public class FutureDateOnlyAttribute : ValidationAttribute 
{
  public override string FormatErrorMessage(string name)
  {
    return " should not be in the past";
  }

  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    var dateValue = value as DateTime? ?? new DateTime();

    if (dateValue.Date < DateTime.Now.Date)
    {
      return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
    }
    
    return ValidationResult.Success;
  }
}