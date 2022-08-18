#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;

public class MyContext : DbContext
{
  public MyContext(DbContextOptions options) : base(options) {}
  public DbSet<User> Users { get; set; }
  public DbSet<Meetup> Meetups { get; set; }
  public DbSet<UserMeetupParticipant> UserMeetupParticipants { get; set; }
}
