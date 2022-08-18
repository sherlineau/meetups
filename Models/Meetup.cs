#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Meetup
{
  [Key]
  public int MeetupId { get; set; }

  // model specific properties below
  [Required(ErrorMessage = "is required")]
  public string Title { get; set; }

  [Required(ErrorMessage = "is required")]
  [FutureDateOnly]
  [DataType(DataType.DateTime)]
  public DateTime? Date { get; set; }

  [Required(ErrorMessage = "is required")]
  [Display(Name = "Duration")]
  public string DurationLength { get; set; }

  [Required(ErrorMessage = "is required")]
  public string DurationType { get; set; }

  [Required(ErrorMessage = "is required")]
  public string Description { get; set; }


  // for one to many
  public int UserId { get; set; }
  public User? Creator { get; set; }
  // many to many relationship attribute
  public List<UserMeetupParticipant> Participants { get; set; } = new List<UserMeetupParticipant>();

  //created at and updated at columns
  public DateTime CreatedAt { get; set; } = DateTime.Now;
  public DateTime UpdatedAt { get; set; } = DateTime.Now;

  public string getDuration()
  {
    return $"{DurationLength} {DurationType}";
  }
}