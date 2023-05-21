using Diplom.Models;
using Diplom.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Diplom.Controllers
{
    public class HomeController : Controller
    {
        public ContextDB db { get; set; }
        public ApplicationContext ApplicationContext { get; set; }
        UserManager<User> _userManager;

        public HomeController(ContextDB context, ApplicationContext applicationContext, UserManager<User> userManager)
        {
            _userManager = userManager;
            db = context;
            ApplicationContext = applicationContext;


        }

        public IActionResult Index()
        {

            var events = db.Events.ToList();


            Events[] eventsDate = db.Events.ToArray();

            DateTime date = DateTime.Now;

            foreach (var d in eventsDate)
            {

                var s = d.start_date;

                int result = DateTime.Compare(s, date);

                if (result < 0)
                {
                    d.status = "close";
                }
                db.SaveChanges();
            }

            return View(events);
        }

        public IActionResult Create()
        {

            SelectList teams = new SelectList(db.Category, "Id", "name");

            ViewBag.catigoryItems = teams; 

            return View();
        }


        public string fileName_one;
        public string fileName_two;
        public string fileName_three;

        [HttpPost]
        public async Task<IActionResult> Create(string Name, string short_description, string description, IFormFile img_one, IFormFile img_two, IFormFile img_three, int age_limit, DateTime start_date, string city, int registrations_required, string catigory)
        {

            if (img_one != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "events");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + img_one.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                img_one.CopyTo(new FileStream(filePath, FileMode.Create));
                fileName_one = uniqueFileName;

            }

            if (img_two != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "events");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + img_two.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                img_two.CopyTo(new FileStream(filePath, FileMode.Create));
                fileName_two = uniqueFileName;

            }
            if (img_three != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "events");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + img_two.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                img_three.CopyTo(new FileStream(filePath, FileMode.Create));
                fileName_three = uniqueFileName;
            }

            db.Events.Add(new Models.Events
            {
                Name = Name,
                short_description = short_description,
                description = description,
                img_one = fileName_one,
                img_two = fileName_two,
                img_three = fileName_three,
                age_limit = age_limit,
                start_date = start_date,
                city = city,
                created_by = this.HttpContext.User.Identity.Name,
                registrations_required = registrations_required,
                catigory = catigory,
            });


            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Details(int ID)
        {
            DateTime dateTime = DateTime.Now.Date;


            CountOfView[] countOfView = db.CountOfView.ToArray();

            if (countOfView.Length <= 0)
            {
                db.CountOfView.Add(new CountOfView
                {
                    EventId = ID,
                    date = dateTime,
                    count = 1
                });
                db.SaveChanges();
            }
            else
            {

				CountOfView count = db.CountOfView.FirstOrDefault(s => s.EventId == ID & s.date == dateTime);
                if (count == null)
                {
					db.CountOfView.Add(new CountOfView
					{
						EventId = ID,
						date = dateTime,
						count = 1
					});
                    db.SaveChanges();
				}
                else
                {
                    count.count += 1;
                    db.SaveChanges();
                }
            }
            
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;
            string userId_ = userId + ";";

            Registration[] registration = db.Registration.Where(s => s.event_id == ID & s.user_id == userId_).ToArray();
            Registration registration_one = await db.Registration.FirstOrDefaultAsync(s => s.event_id == ID & s.user_id == userId_);

            if (registration.Length == 0)
            {
                ViewBag.register = "false";
                ViewBag.appreciated = "false";
            }
            if (registration.Length > 0 && registration_one.appreciated && registration_one.status_of_registrate == "new")
            {
                ViewBag.register = "true";
                ViewBag.appreciated = "true";
            }
            if (registration.Length > 0 && !registration_one.appreciated && registration_one.status_of_registrate == "new")
            {
                ViewBag.register = "true";
                ViewBag.appreciated = "false";
            }

            if (registration.Length > 0 && registration_one.appreciated && registration_one.status_of_registrate == "canceled")
            {
                ViewBag.register = "false";
                ViewBag.appreciated = "true";
            }
            if (registration.Length > 0 && !registration_one.appreciated && registration_one.status_of_registrate == "canceled")
            {
                ViewBag.register = "false";
                ViewBag.appreciated = "false";
            }


            if (ID != null)
            {

                Comments[] comment = db.Comments.Where(s => s.event_id == ID).ToArray();

                var result = db.Comments.Select(x => x.user_id).ToList();

                ViewBag.CommentsNull = true;
                if (comment != null)
                {
                    foreach (var com in comment)
                    {
                        ViewBag.CommentsNull = false;
                        ViewBag.comment = comment;
                    }
                }



                Events events = await db.Events.FirstOrDefaultAsync(p => p.Id == ID);

                if (events != null)
                {
                    return View(events);
                }
            }
            return NotFound();
        }



        public async Task<IActionResult> Edit(int? ID)
        {
            Events events = await db.Events.FirstOrDefaultAsync(p => p.Id == ID);
            if (events == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.ID = ID;

            SelectList teams = new SelectList(db.Category, "Id", "name");

            ViewBag.catigoryItems = teams;


            Events model = new Events
            {
                Id = events.Id,
                Name = events.Name,
                short_description = events.short_description,
                description = events.description,
                img_one = events.img_one,
                img_two = events.img_two,
                img_three = events.img_three,
                age_limit = events.age_limit,
                start_date = events.start_date,
                city = events.city,
                registrations_required = events.registrations_required
            };

            return View(model);
        }



        public string fileName_one_more;
        public string fileName_two_more;
        public string fileName_three_more;

        [HttpPost]
        public async Task<IActionResult> Edit(
            int id, 
            string Name, 
            string short_description, 
            string description, 
            IFormFile img_one, 
            IFormFile img_two, 
            IFormFile img_three, 
            int age_limit, 
            DateTime start_date, 
            string city, 
            int registrations_required, 
            string catigory,

            string img_one_old,
            string img_two_old,
            string img_three_old
            )
        {

            Events events = await db.Events.FirstOrDefaultAsync(p => p.Id == id);


            if (img_one != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "events");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + img_one.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                img_one.CopyTo(new FileStream(filePath, FileMode.Create));
                fileName_one_more = uniqueFileName;

            }
            else
            {
                fileName_one_more = img_one_old;
            }

            if (img_two != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "events");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + img_two.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                img_two.CopyTo(new FileStream(filePath, FileMode.Create));
                fileName_two_more = uniqueFileName;

            }
            else
            {
                fileName_two_more = img_two_old;
            }

            if (img_three != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "events");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + img_two.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                img_three.CopyTo(new FileStream(filePath, FileMode.Create));
                fileName_three_more = uniqueFileName;
            }
            else
            {
                fileName_three_more = img_three_old;
            }



            

            events.Id = id;
            events.Name = Name;
            events.short_description = short_description;
            events.description = description;
            events.img_one = fileName_one_more;
            events.img_two = fileName_two_more;
            events.img_three = fileName_three_more;
            events.age_limit = age_limit;
            events.start_date = start_date;
            events.city = city;
            events.registrations_required = registrations_required;
            events.catigory = catigory;
            


            await db.SaveChangesAsync();
            return RedirectToAction("Index");

        }


        public async Task<IActionResult> Delete(int id)
        {
            Events events = await db.Events.FirstOrDefaultAsync(p => p.Id == id);

            ViewBag.Id = id;
            return View(events);
        }


        [HttpPost]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            if (id != null)
            {
                Events events = await db.Events.FirstOrDefaultAsync(p => p.Id == id);
                if (events != null)
                {
                    db.Events.Remove(events);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }


        public async Task<IActionResult> AdminEventsList()
        {
            return View(db.Events.ToList());
        }

        public async Task<IActionResult> AdminEditEvents(int id)
        {
            Events events = await db.Events.FirstOrDefaultAsync(p => p.Id == id);
            if (events == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.ID = id;

            SelectList teams = new SelectList(db.Category, "Id", "name");

            ViewBag.catigoryItems = teams;


            Events model = new Events
            {
                Id = events.Id,
                Name = events.Name,
                short_description = events.short_description,
                description = events.description,
                img_one = events.img_one,
                img_two = events.img_two,
                img_three = events.img_three,
                age_limit = events.age_limit,
                start_date = events.start_date,
                created_by = events.created_by,
                paid = events.paid,
                city = events.city,
                number_of_registration = events.number_of_registration,
                registrations_required = events.registrations_required,
                catigory = events.catigory
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AdminEditEvents(
            int id,
            string Name,
            string short_description,
            string description,
            IFormFile img_one,
            IFormFile img_two,
            IFormFile img_three,
            int age_limit,
            DateTime start_date,
            string created_by,
            bool paid,
            string city,
            int number_of_registration,
            int registrations_required,
            string status,
            string catigory,

            string img_one_old,
            string img_two_old,
            string img_three_old
            )
        {
            Events events = await db.Events.FirstOrDefaultAsync(p => p.Id == id);


            if (img_one != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "events");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + img_one.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                img_one.CopyTo(new FileStream(filePath, FileMode.Create));
                fileName_one_more = uniqueFileName;

            }
            else
            {
                fileName_one_more = img_one_old;
            }

            if (img_two != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "events");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + img_two.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                img_two.CopyTo(new FileStream(filePath, FileMode.Create));
                fileName_two_more = uniqueFileName;

            }
            else
            {
                fileName_two_more = img_two_old;
            }

            if (img_three != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "events");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + img_two.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                img_three.CopyTo(new FileStream(filePath, FileMode.Create));
                fileName_three_more = uniqueFileName;
            }
            else
            {
                fileName_three_more = img_three_old;
            }



            try
            {
                events.Id = id;
                events.Name = Name;
                events.short_description = short_description;
                events.description = description;
                events.img_one = fileName_one_more;
                events.img_two = fileName_two_more;
                events.img_three = fileName_three_more;
                events.age_limit = age_limit;
                events.start_date = start_date;
                events.city = city;
                events.registrations_required = registrations_required;
                events.created_by = created_by;
                events.paid = paid;
                events.number_of_registration = number_of_registration;
                events.status = status;
                events.catigory = catigory;
            }
            catch (Exception ex)
            {
                return NotFound();
            }


            await db.SaveChangesAsync();
            return RedirectToAction("AdminEventsList", "Home");
        }


        public async Task<IActionResult> AdminDeleteEvents(int id)
        {
            if (id != null)
            {
                Events events = await db.Events.FirstOrDefaultAsync(p => p.Id == id);
                if (events != null)
                {
                    db.Events.Remove(events);
                    await db.SaveChangesAsync();
                    return RedirectToAction("admineventslist");
                }
            }
            return NotFound();
        }


        public async Task<IActionResult> Registration(int Id)
        {

            Events events = await db.Events.FirstOrDefaultAsync(p => p.Id == Id);

            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;

            if (events != null && userId != null)
            {
                ViewBag.eventsId = Id;
                ViewBag.userId = userId;

            }
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Registration(int eventsId, string userId)
        {

            var user = await _userManager.GetUserAsync(User);
            var userIds = user?.Id;
            string userId_ = userIds + ";";


            Registration[] registration = db.Registration.Where(s => s.event_id == eventsId & s.user_id == userId_).ToArray();
            Registration registration_one = await db.Registration.FirstOrDefaultAsync(s => s.event_id == eventsId & s.user_id == userId_);

            if (registration.Length == 0)
            {
                db.Registration.Add(new Models.Registration
                {
                    event_id = eventsId,
                    user_id = userId,

                    status_of_registrate = "new"
                });
            }

            if (registration.Length > 0)
            {
                Registration registration_oneMore = await db.Registration.FirstOrDefaultAsync(s => s.event_id == eventsId & s.user_id == userId_);

                registration_oneMore.status_of_registrate = "new";
            }




            Events events = await db.Events.FirstOrDefaultAsync(p => p.Id == eventsId);

            events.number_of_registration += 1;

            if (events.number_of_registration == events.registrations_required)
            {
                events.status = "events";
            }
            else
            {
                events.status = "news";
            }

            db.SaveChanges();




            return RedirectToAction("Details", "Home", new { id = eventsId });

        }

        public async Task<IActionResult> appreciated(int Id)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;
            string userId_ = userId + ";";


            Registration[] registration = db.Registration.Where(s => s.event_id == Id & s.user_id == userId_).ToArray();

            if (registration.Length > 0)
            {
                Registration registration_one = await db.Registration.FirstOrDefaultAsync(s => s.event_id == Id & s.user_id == userId_);

                if (!registration_one.appreciated)
                {
                    registration_one.appreciated = true;

                    Events events = await db.Events.FirstOrDefaultAsync(p => p.Id == Id);

                    events.rating += 1;



                    await db.SaveChangesAsync();
                    return RedirectToAction("Details", "Home", new { id = Id });

                }

            }

            return RedirectToAction("Details", "Home", new { id = Id });

        }


        public async Task<IActionResult> Unappreciated(int Id)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;
            string userId_ = userId + ";";


            Registration[] registration = db.Registration.Where(s => s.event_id == Id & s.user_id == userId_).ToArray();

            if (registration.Length > 0)
            {
                Registration registration_one = await db.Registration.FirstOrDefaultAsync(s => s.event_id == Id & s.user_id == userId_);

                if (registration_one.appreciated)
                {
                    registration_one.appreciated = false;

                    Events events = await db.Events.FirstOrDefaultAsync(p => p.Id == Id);

                    events.rating -= 1;

                    await db.SaveChangesAsync();
                    return RedirectToAction("Details", "Home", new { id = Id });

                }

            }

            return RedirectToAction("Details", "Home", new { id = Id });

        }


        public async Task<IActionResult> UnRegistration(int Id)
        {

            var user = await _userManager.GetUserAsync(User);
            var userIds = user?.Id;
            string userId_ = userIds + ";";

            Registration registration = await db.Registration.FirstOrDefaultAsync(s => s.event_id == Id & s.user_id == userId_);

            registration.status_of_registrate = "canceled";


            Events events = await db.Events.FirstOrDefaultAsync(p => p.Id == Id);

            events.number_of_registration -= 1;


            if (events.number_of_registration == events.registrations_required)
            {
                events.status = "events";
            }
            else
            {
                events.status = "news";
            }

            await db.SaveChangesAsync();
            return RedirectToAction("Details", "Home", new { id = Id });
        }

        [HttpPost]
        public async Task<IActionResult> Comment(int Id, string comment)
        {
            if (comment != null)
            {

                var user = await _userManager.GetUserAsync(User);
                var userIds = user?.Id;
                string userId_ = userIds + ";";


                db.Comments.Add(new Models.Comments
                {
                    event_id = Id,
                    user_id = userId_,
                    comment = comment
                });

                db.SaveChanges();
            }
            else
            {
                return RedirectToAction("Details", "Home", new { id = Id });
            }

            return RedirectToAction("Details", "Home", new { id = Id });
        }



        public IActionResult RegistartionData(string Id)
        {
            string userId_ = Id + ";";

            //Registration[] reg = db.Registration.Where(s => s.user_id == userId_).ToArray<Registration>();
            //int[] ints = db.Registration.Where(s => s.user_id == userId_).Select(u => u.Id).ToArray();



            //List<Diplom.Models.Events> result = db.Registration.Join(db.Events, reg => reg.event_id, ev => ev.Id, (reg, ev) => new Diplom.Models.Events() { Name = reg.} ).ToList();
            //List<AA> result = db.Events.Join(db.Registration, ev => ev.Id, reg => reg.event_id, (ev, reg) => new AA { Name = ev.Name, status = ev.status }).ToList();


            List<RegistrationClass> result = db.Events.Join(db.Registration.Where(us => us.user_id == userId_ & us.status_of_registrate != "canceled"),
                ev => ev.Id, reg => reg.event_id, (ev, reg) => new RegistrationClass
                {
                    Id = ev.Id,
                    Name = ev.Name,
                    short_description = ev.short_description,
                    created_by = ev.created_by,
                    start_date = ev.start_date,
                    img_one = ev.img_one
                }).ToList();


            return View(result);
        }



        public async Task<IActionResult> AdminPanel()
        {
            return View();
        }



        public async Task<IActionResult> CommentsList(int? hide, int? unhide, int? all)
        {

            if (hide == 1)
            {
                Comments[] hidecomments = db.Comments.Where(c => c.edit == true).ToArray();
                return View(hidecomments);
            }
            else if (unhide == 1)
            {
                Comments[] unhidecomments = db.Comments.Where(c => c.edit == false).ToArray();
                return View(unhidecomments);
            }
            else if (all == 1)
            {
                Comments[] comments = db.Comments.ToArray();
                return View(comments);
            }
            else
            {
                Comments[] comments = db.Comments.ToArray();
                return View(comments);
            }



        }


        public async Task<IActionResult> AdminHideComment(int Id)
        {

            Comments comments = await db.Comments.FirstOrDefaultAsync(c => c.Id == Id);

            if (comments != null)
            {
                comments.edit = true;
            }
            db.SaveChanges();
            return RedirectToAction("CommentsList", "Home");
        }

        public async Task<IActionResult> AdminUnHideComment(int Id)
        {

            Comments comments = await db.Comments.FirstOrDefaultAsync(c => c.Id == Id);

            if (comments != null)
            {
                comments.edit = false;
            }
            db.SaveChanges();
            return RedirectToAction("CommentsList", "Home");
        }


        public async Task<IActionResult> AdminCatigoryList()
        {
            Catigory[] catigory = db.Category.ToArray();

            return View(catigory);
        }



        public async Task<IActionResult> AdminCreateCatigory()
        {
            return View();
        }




        [HttpPost]
        public async Task<IActionResult> AdminCreateCatigory(string name)
        {

            db.Category.Add(new Catigory
            {
                name = name
            });

            db.SaveChanges();

            return RedirectToAction("AdminCatigoryList", "Home");
        }

        public async Task<IActionResult> AdminEditCatigory(int Id)
        {

            Catigory catigory = await db.Category.FirstOrDefaultAsync(c => c.Id == Id);

            Catigory cat = new Catigory()
            {
                Id = catigory.Id,
                name = catigory.name
            };
            return View(cat);
        }

        [HttpPost]
        public async Task<IActionResult> AdminEditCatigory(int Id, string name)
        {
            Catigory catigory = await db.Category.FirstOrDefaultAsync(c => c.Id == Id);

            catigory.Id = Id;
            catigory.name = name;
            db.SaveChanges();

            return RedirectToAction("AdminCatigoryList", "Home");
        }

        public async Task<IActionResult> AdminDeleteCatigory(int Id)
        {

            Catigory catigory = await db.Category.FirstOrDefaultAsync(c => c.Id == Id);

            Catigory cat = new Catigory()
            {
                Id = catigory.Id,
                name = catigory.name
            };
            return View(cat);
        }


        [HttpPost]
        public async Task<IActionResult> AdminDeleteCatigoryConfirm(int Id)
        {
            Catigory catigory = await db.Category.FirstOrDefaultAsync(c => c.Id == Id);

            db.Category.Remove(catigory);

            db.SaveChanges();

            return RedirectToAction("AdminCatigoryList", "Home");
        }



        public async Task<IActionResult> Statistics ()
        {

			var getUser = User.Identity.Name;

            Events[] events = db.Events.Where(s => s.created_by == getUser).ToArray();
			return View(events);
        }


		public async Task<IActionResult> StatisticsDetail(int Id)
        {
            CountOfView[] countOfViews = db.CountOfView.Where(c => c.EventId  == Id).OrderByDescending(b => b.Id).Take(100).ToArray();

            int views = 0;

            foreach(var view in countOfViews)
            {
                views += view.count;
            }

            ViewBag.Views = views;


            return View(countOfViews);
        }

		public async Task<IActionResult> AdminStatistics(int? all, int? today, DateTime? date)
        {

            CountOfView[] countOfViewss = db.CountOfView.ToArray();

            int total = 0;
            foreach(var count in countOfViewss)
            {
                total += count.count;
            };

            ViewBag.total = total;

            //var result = db.CountOfView.GroupBy(p => p.EventId).Select(g => new { EventId = g.Key, Count = g.Sum(p => p.count) }).ToList();
            //ViewBag.result = result;

            List<StatisticModel> result = db.CountOfView.GroupBy(p => p.EventId).Select(g => new StatisticModel {

                EventsId = g.Key, 
                count = g.Sum(p => p.count) 
            }).ToList();

            ViewBag.result = result;



            if (all != null)
            {
				CountOfView[] countOfViews = db.CountOfView.ToArray();
				return View(countOfViews);
			}

            else if (today != null) 
            {
                CountOfView[] countOfViews = db.CountOfView.Where(s => s.date == DateTime.Now.Date).ToArray();
                return View(countOfViews);
			}
            else if (date != null)
            {
                CountOfView[] countOfViews = db.CountOfView.Where(s => s.date == date).ToArray();
                return View(countOfViews);
            }

            else
            {
				CountOfView[] countOfViews = db.CountOfView.ToArray();
				return View(countOfViews);
			}
        }

        public async Task<IActionResult> AddStatistics()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> ConfirmAddStatistics(int EventId, DateTime date, int count)
        {

            db.CountOfView.Add(new CountOfView
            {
                EventId = EventId,
                date = date,
                count = count
            });
            db.SaveChanges();

            return RedirectToAction("AdminPanel", "Home");
        }


	}

	


}
