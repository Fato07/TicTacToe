using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers
{
    public class UserRegistrationController : Controller
    {
        private readonly IUserService _userService; 
        public UserRegistrationController(IUserService userService) 
        { 
            _userService = userService; 
        } 

        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Index(UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                await _userService.RegisterUser(userModel);
                return RedirectToAction(nameof(EmailConfirmation), new { userModel.Email }); 
            }
            else
            {
                return View(userModel);
            }
        }
        
        [HttpGet] 
        [HttpGet] 
        public async Task<IActionResult> EmailConfirmation(string email) 
        { 
            var user = await _userService.GetUserByEmail(email); 
            if (user?.IsEmailConfirmed == true) 
                return RedirectToAction("Index", "GameInvitation", new { email = email }); 
 
            ViewBag.Email = email;
            return View(); 
        }
    }
}