using Diplom.Models;
using Diplom.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace Diplom.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

		public ContextDB db { get; set; }
		public ApplicationContext ApplicationContext { get; set; }

		public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ContextDB context, ApplicationContext applicationContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
			db = context;
			ApplicationContext = applicationContext;
		}
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.UserName, birthDay = model.birthDay };
                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // установка куки
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }



        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> UserData()
        {

            var userData = _userManager.Users.ToList().Where(s => s.UserName == this.HttpContext.User.Identity.Name);

            var getUser = User.Identity.Name;


			Events[] events = db.Events.Where(s => s.created_by == getUser).ToArray();

            ViewBag.usersEvents = events;

			return View(userData);
            
        }



        public async Task<IActionResult> EditUserData(String? Id)
        {
            if (Id != null)
            {
                User user = await _userManager.FindByIdAsync(Id);
                if (user == null)
                {
                    return NotFound();
                }
                ViewBag.Id = Id;
                EditUserViewModel model = new EditUserViewModel { Id = user.Id, Email = user.Email, UserName = user.UserName, image = user.image, linkToVk = user.linkToVk, birthDay = user.birthDay };
                return View(model);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditUserData(String? Id, string Email, string UserName, List<IFormFile>? image, string? linkToVk, DateTime birthDay)
        {
            User user = await _userManager.FindByIdAsync(Id);
            foreach (var formfile in image)
            {
                if (formfile.Length > 0)
                {
                    string fp = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", formfile.FileName);
                    using (var stream = System.IO.File.Create(fp))
                    {
                        formfile.CopyTo(stream);
                        
                        user.Email = Email;
                        user.UserName = UserName;
                        user.image = formfile.FileName;
                        user.linkToVk = linkToVk;
                        user.birthDay = birthDay;
                    }
                };
            }
            await _userManager.UpdateAsync(user);
            return RedirectToAction("UserData");
        }
    }
}
