using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

public class UserController : Controller
{

  // store uid for checking login state
  private int? uid { get { return HttpContext.Session.GetInt32("UUID"); } }

  private bool loggedIn { get { return uid != null; } }

  // MyContext class for db querying
  private MyContext _db;

  public UserController(MyContext context)
  {
    _db = context;
  }

  // index displays login and registration form
  [HttpGet("")]
  public IActionResult Index()
  {
    if (loggedIn)
    {
      return RedirectToAction("Dashboard", "Meetup");
    }
    return View("Index");
  }

  [HttpPost("/users/register")]
  public IActionResult Register(User newUser)
  {
    // checks for existing email
    if (_db.Users.Any(u => u.Email == newUser.Email))
    {
      ModelState.AddModelError("Email", "Email already in use");
      return Index();
    }

    // checks for model validations
    if (ModelState.IsValid == false)
    {
      return Index();
    }

    // no errors -> save to database and encrypt user password

    PasswordHasher<User> hasher = new PasswordHasher<User>();
    newUser.Password = hasher.HashPassword(newUser, newUser.Password);

    _db.Users.Add(newUser);
    _db.SaveChanges();

    // set session UUID/name to newly registered user
    HttpContext.Session.SetInt32("UUID", newUser.UserId);
    HttpContext.Session.SetString("Name", newUser.Name);


    return RedirectToAction("Dashboard", "Meetup");
  }

  // for loggin in using user credentials
  [HttpPost("Login")]
  public IActionResult Login(LoginUser loginUser)
  {
    if (ModelState.IsValid == false)
    {
      return Index();
    }

    // query for email in database
    // returns null if user is not found
    User? dbUser = _db.Users.FirstOrDefault(u => u.Email == loginUser.LoginEmail);

    if (dbUser == null)
    {
      ModelState.AddModelError("LoginEmail", "Email/Password is invalid");
      return Index();
    }

    // for verifying user password against encrypted/hashed password
    PasswordHasher<LoginUser> loginHasher = new PasswordHasher<LoginUser>();
    PasswordVerificationResult pwCompareResult = loginHasher.VerifyHashedPassword(loginUser, dbUser.Password, loginUser.LoginPassword);

    if (pwCompareResult == 0)
    {
      ModelState.AddModelError("LoginPassword", "Email/Password is invalid");
      return Index();
    }

    // no errors occured, set session data
    HttpContext.Session.SetInt32("UUID", dbUser.UserId);
    HttpContext.Session.SetString("Name", dbUser.Name);
    return RedirectToAction("Dashboard", "Meetup");
  }

  // for logging out -> clear session
  [HttpPost("/logout")]
  public IActionResult Logout()
  {
    HttpContext.Session.Clear();
    return RedirectToAction("Index");
  }
}