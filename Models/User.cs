#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
  // primary key for database
  [Key]
  public int UserId { get; set; }

  // model specific properties below
  [Required(ErrorMessage = "is required")]
  [MinLength(2, ErrorMessage = " must be at least 2 characters")]
  public string Name { get; set; }

  [Required(ErrorMessage = "is required")]
  [EmailAddress]
  public string Email { get; set; }

  [Required(ErrorMessage = "is required")]
  [MinLength(8, ErrorMessage = " must be at least 8 characters")]
  [RegularExpression(@"^(?=.*[\W])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$",ErrorMessage ="must contain 1 letter, 1 number, 1 special character")]
  [DataType(DataType.Password)]
  public string Password { get; set; }

  [NotMapped]
  [Display(Name = "Confirm Password")]
  [Compare("Password", ErrorMessage = "Passwords must match")]
  [DataType(DataType.Password)]
  public string Confirm { get; set; }

  //relationshp properties
  // one to many -> many events created by one user
  //    user has a list of events they created
  // public List<Event> CreatedEvents { get; set; } = new List<Event>();
  // many to many -> for rsvping to other events
  //    user can rsvp to other events created by other users
  // public List<UserRSVPEvent> RSVPedEvents { get; set; } = new List<UserRSVPEvent>();

  //created at and updated at columns
  public DateTime CreatedAt { get; set; } = DateTime.Now;
  public DateTime UpdatedAt { get; set; } = DateTime.Now;

}