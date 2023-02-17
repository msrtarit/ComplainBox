using System.Linq;
using System.Threading.Tasks;
using ComplainBox.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ComplainBox.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password.Equals(model.ConfirmPassword))
                {
                    var user = new IdentityUser
                    {
                        UserName = model.Username,
                        Email = model.Email
                    };
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        return View("RegistrationConfirmed");
                    }
                    ModelState.AddModelError(result.Errors.FirstOrDefault()?.Code, result.Errors.FirstOrDefault()?.Description);
                }

                ModelState.AddModelError("", "Password and Confirmed Password are not same!");
            }

            return View(model);
        }


        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username) ??
                           await _userManager.FindByEmailAsync(model.Username);

                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid Username/Email!");
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, model.IsMarkedLoggedIn);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid Username/Password!");
            }

            return View(model);
        }


        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

    }
}