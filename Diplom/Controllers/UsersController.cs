using Diplom.Models;
using Diplom.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Diplom.Controllers
{
    public class UsersController : Controller
    {
        UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index() => View(_userManager.Users.ToList());

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.UserName, birthDay = model.birthDay };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
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

        public async Task<IActionResult> Edit(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.Id = id;
            EditUserViewModel model = new EditUserViewModel { Id = user.Id, Email = user.Email, UserName = user.UserName, image = user.image, linkToVk = user.linkToVk, birthDay = user.birthDay };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(String? Id, string Email, string UserName, List<IFormFile>? image, string? linkToVk, DateTime birthDay)
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
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }




        public async Task<IActionResult> ChangePassword(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ChangePasswordViewModel model = new ChangePasswordViewModel { Id = user.Id, Email = user.Email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    var _passwordValidator =
                        HttpContext.RequestServices.GetService(typeof(IPasswordValidator<User>)) as IPasswordValidator<User>;
                    var _passwordHasher =
                        HttpContext.RequestServices.GetService(typeof(IPasswordHasher<User>)) as IPasswordHasher<User>;

                    IdentityResult result =
                        await _passwordValidator.ValidateAsync(_userManager, user, model.NewPassword);
                    if (result.Succeeded)
                    {
                        user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
                        await _userManager.UpdateAsync(user);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден");
                }
            }
            return View(model);
        }


    }
}
