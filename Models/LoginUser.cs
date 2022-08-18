#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[NotMapped]
public class LoginUser
{
  [Required(ErrorMessage = ".")]
  [EmailAddress]
  [Display(Name = "Email")]
  public string LoginEmail { get; set; }

  [Required(ErrorMessage = "Email/Password is invalid")]
  [MinLength(8,ErrorMessage = "Email/Password is invalid")]
  [DataType(DataType.Password)]
  [Display(Name = "Password")]
  public string LoginPassword { get; set; }
}