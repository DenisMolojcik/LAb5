using MedicalInstitution.Data;
using MedicalInstitution.Models;
using MedicalInstitution.Services;
using MedicalInstitution.ViewsModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalInstitution.Controllers
{
    public class UsersController : Controller
    {
            UserManager<AppUser> _userManager;
            private readonly ApplicationDbContext _context;

            public UsersController(UserManager<AppUser> userManager, ApplicationDbContext context)
            {
                _context = context;
                _userManager = userManager;
            }
            [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 292)]
            [Authorize(Roles = "admin")]
            public IActionResult Index() => View(_context.GetService<ICached<AppUser>>().GetList());

            public IActionResult Create() => View();

            [HttpPost]
            public async Task<IActionResult> Create(CreateUserViewModel model)
            {
                if (ModelState.IsValid)
                {
                    AppUser user = new AppUser { Email = model.Email, UserName = model.Email};
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
                AppUser user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                EditUserViewModel model = new EditUserViewModel { Id = user.Id, Email = user.Email };
                return View(model);
            }

            [HttpPost]
            public async Task<IActionResult> Edit(EditUserViewModel model)
            {
                if (ModelState.IsValid)
                {
                    AppUser user = await _userManager.FindByIdAsync(model.Id);
                    if (user != null)
                    {
                        user.Email = model.Email;
                        user.UserName = model.Email;
                        var _passwordValidator =
                        HttpContext.RequestServices.GetService(typeof(IPasswordValidator<AppUser>)) as IPasswordValidator<AppUser>;
                        var _passwordHasher =
                            HttpContext.RequestServices.GetService(typeof(IPasswordHasher<AppUser>)) as IPasswordHasher<AppUser>;
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
                }
                return View(model);
            }

            [HttpPost]
            public async Task<ActionResult> Delete(string id)
            {
                AppUser user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    IdentityResult result = await _userManager.DeleteAsync(user);
                }
                return RedirectToAction("Index");
            }
        }
    }
