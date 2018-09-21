using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {

        private MyContext _context;
        public HomeController(MyContext context){
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index(){
            return View();
        }
        [HttpPost("register")]
        public IActionResult Register(User info){
            User NewUser = new User{
                FirstName = info.FirstName,
                LastName = info.LastName,
                Email = info.Email,
                Password = info.Password,
                PassConf = info.PassConf,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            if(ModelState.IsValid){
                User CheckEmail = _context.Users.SingleOrDefault(user => user.Email == info.Email);
                if(info.PassConf == info.Password && CheckEmail == null){
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    NewUser.Password = Hasher.HashPassword(NewUser, NewUser.Password);
                    _context.Add(NewUser);
                    _context.SaveChanges();
                    HttpContext.Session.SetInt32("CurrUser", NewUser.UserId);
                    return RedirectToAction("Dashboard");
                }
                else{
                    if(info.PassConf != info.Password){
                        ViewBag.PassConf = "Pass don't match!";
                    }
                    if(CheckEmail != null){
                        ViewBag.Email = "This email is already registered!";
                    }
                    return View("Index");
                }
            }
                else{
                    return View("Index");
                }
        }
        [HttpPost("login")]
        public IActionResult Login(string Email2, string Password2){
            User ThisUser = _context.Users.SingleOrDefault(user => user.Email == Email2);
            if(ThisUser == null){
                ViewBag.Login = "Invalid login info";
                HttpContext.Session.SetInt32("CurrUser", ThisUser.UserId);
                return View("Index");
            }
            else{
                var Hasher = new PasswordHasher<User>();
                var result = Hasher.VerifyHashedPassword(ThisUser, ThisUser.Password, Password2);
                if(result != 0){
                    HttpContext.Session.SetInt32("CurrUser", ThisUser.UserId);
                    return RedirectToAction("Dashboard");
                }
                else{
                    ViewBag.Login = "Invalid login info";
                    return View("Index");
                }
            }
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard(){
            int? CurrId = HttpContext.Session.GetInt32("CurrUser");
            if(CurrId == null){
                return View("Index");
            }
            User ThisUser = _context.Users.SingleOrDefault(user => user.UserId == (int)CurrId);
            List<Wedding> AllWeddings = _context.Weddings
                .Include(crtr => crtr.Creator)
                .Include(gst => gst.Guests)
                .ToList();
            ViewBag.User = ThisUser;
            ViewBag.Weddings = AllWeddings;
            List<int> weddingIds = new List<int>();
            foreach(var wedding in AllWeddings){
                foreach(var guest in wedding.Guests){
                    if(guest.UserId == ThisUser.UserId){
                        weddingIds.Add(wedding.WeddingId);
                    }
                }
            }
            ViewBag.Attending = weddingIds;
            return View();
        }

        [HttpGet("add")]
        public IActionResult AddWedding(){
            return View();
        }

        [HttpPost("")]
        public IActionResult Add(Wedding info){
            int? CurrId = HttpContext.Session.GetInt32("CurrUser");
            User CurrUser = _context.Users.SingleOrDefault( user => user.UserId == (int)CurrId);

            Wedding NewWedding = new Wedding{
                WedderOne = info.WedderOne,
                WedderTwo = info.WedderTwo,
                Date = info.Date,
                Address = info.Address,
                UserId = (int)CurrId,
                Creator = CurrUser,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Add(NewWedding);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        [HttpPost("RSVP")]
        public IActionResult RSVP(int Id){
            int? CurrId = HttpContext.Session.GetInt32("CurrUser");
            User CurrUser = _context.Users.SingleOrDefault( user => user.UserId == (int)CurrId);
            List<Wedding> ThisWedding = _context.Weddings.Where(wed => wed.WeddingId == Id)
                .Include(gst => gst.Guests)
                .ToList();
            Wedding CurrWedding = _context.Weddings.SingleOrDefault(wed => wed.WeddingId == Id);
            Guest NewGuest = new Guest{
                UserId = (int)CurrId,
                User = CurrUser,
                WeddingId = Id,
                Wedding = CurrWedding,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            
            _context.Add(NewGuest);
            CurrWedding.Guests.Add(NewGuest);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");

        }
        [HttpPost("UnRSVP")]
        public IActionResult UnRSVP(int Id){
            int? CurrId = HttpContext.Session.GetInt32("CurrUser");
            User CurrUser = _context.Users.SingleOrDefault( user => user.UserId == (int)CurrId);
            List<Wedding> ThisWedding = _context.Weddings.Where(wed => wed.WeddingId == Id)
                .Include(gst => gst.Guests)
                .ToList();
            Guest FindGuest = _context.Guests.SingleOrDefault(gst => gst.UserId == (int)CurrId && gst.WeddingId == Id);
            _context.Guests.Remove(FindGuest);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpPost("Delete")]
        public IActionResult Delete(int Id){
            int? CurrId = HttpContext.Session.GetInt32("CurrUser");
            User CurrUser = _context.Users.SingleOrDefault( user => user.UserId == (int)CurrId);
            Wedding ThisWedding = _context.Weddings.SingleOrDefault(wed=>wed.WeddingId == Id);
            _context.Weddings.Remove(ThisWedding);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
