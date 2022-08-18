using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class MeetupController : Controller
{
  private int? uid { get { return HttpContext.Session.GetInt32("UUID"); } }
  private bool loggedIn { get { return uid != null; } }

  // MyContext class for db querying
  private MyContext _db;

  public MeetupController(MyContext context)
  {
    _db = context;
  }

    // display dashboard to logged in users
  [HttpGet("/dashboard")]
  public IActionResult Dashboard()
  {
    // if not logged in, return index method to display login/registration
    if (!loggedIn)
    {
      return RedirectToAction("Index");
    }

    List<Meetup> AllMeetups = _db.Meetups.Include(m => m.Creator).Include(m => m.Participants).OrderBy(m => m.Date).ToList();

    List<Meetup> pastMeetups = new List<Meetup>();

    // this loop goes through all the meetups from the database and adds any that have dates set to before Datetime.Now to a the past meetups list
    foreach (Meetup meetup in AllMeetups)
    {
      if (meetup.Date < DateTime.Now)
      {
        Meetup? pastMeetup = _db.Meetups.FirstOrDefault(m => m.MeetupId == meetup.MeetupId);
        if(pastMeetup != null)
        {
          _db.Meetups.Remove(pastMeetup);
          _db.SaveChanges();
          pastMeetups.Add(meetup);
        }
      }
    }

    //  this loops through the pastmeetups list and then removes them from the all meetups list
    foreach(Meetup m in pastMeetups)
    {
      AllMeetups.Remove(m);
    }

    return View("Dashboard", AllMeetups);
  }

  //  for display create form
  [HttpGet("/meetups/new")]
  public IActionResult New()
  {
    if (!loggedIn)
    {
      return RedirectToAction("Index", "User");
    }
    return View("New");
  }

  // for "posting" create to database
  [HttpPost("/meetups/create")]
  public IActionResult Create(Meetup newMeetup)
  {
    // if user is not logged in -> redirect to login
    if (!loggedIn || uid == null)
    {
      return RedirectToAction("Index", "User");
    }

    // if model/form validations fail
    if (ModelState.IsValid == false)
    {
      return New();
    }

    // save form to database and set newMeetup user id to logged in user
    newMeetup.UserId = (int)uid;
    _db.Meetups.Add(newMeetup);
    _db.SaveChanges();

    return RedirectToAction("Details", new { meetupId = newMeetup.MeetupId});
  }

  [HttpGet("/meetups/{meetupId}")]
  public IActionResult Details(int meetupId)
  {
        if (!loggedIn)
    {
      return RedirectToAction("Index", "User");
    }

    Meetup? meetup = _db.Meetups.Include(m => m.Creator).Include(m => m.Participants).ThenInclude(p => p.Participant).FirstOrDefault( m => m.MeetupId == meetupId);

    if (meetup == null)
    {
      return RedirectToAction("Dashboard");
    }

    return View("Details",meetup);
  }

  // delete
  [HttpPost("/meetups/{meetupId}/delete")]
  public IActionResult Delete(int meetupId)
  {
    if (!loggedIn)
    {
      return RedirectToAction("Index", "User");
    }

    Meetup? meetupToBeDeleted = _db.Meetups.FirstOrDefault(e => e.MeetupId == meetupId);

    //only delete if creator id matches uid
    if (meetupToBeDeleted != null)
    {
      if (meetupToBeDeleted.UserId == uid)
      {
        _db.Meetups.Remove(meetupToBeDeleted);
        _db.SaveChanges();
      }
    }
    return RedirectToAction("Dashboard");
  }

  // for leaving/join meetup
  [HttpPost("/events/{meetupId}/join")]
  public IActionResult Join(int meetupId)
  {
    if (!loggedIn || uid == null)
    {
      return RedirectToAction("Index", "User");
    }

    UserMeetupParticipant? existingJoin = _db.UserMeetupParticipants.FirstOrDefault(join => join.MeetupId == meetupId && join.UserId == uid);

    if(existingJoin == null)
    {
      UserMeetupParticipant newJoin = new UserMeetupParticipant()
      {
        MeetupId = meetupId,
        UserId = (int)uid
      };
      _db.UserMeetupParticipants.Add(newJoin);
    }
    else 
    {
      _db.Remove(existingJoin);
    }

    _db.SaveChanges();
    return RedirectToAction("Dashboard");
  }
}