using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers
{
    public class GameInvitationController : Controller
    {
        private IUserService _userService;
        private IStringLocalizer<GameInvitationController> _stringLocalizer;

        public GameInvitationController(IUserService userService, IStringLocalizer<GameInvitationController> stringLocalizer) 
        { 
            _userService = userService; 
            _stringLocalizer = stringLocalizer; 
        } 
        // GET
        public IActionResult Index()
        {
            return View();
        }
        
        
       
        [HttpGet] 
        public async Task<IActionResult> Index(string email) 
        { 
            var gameInvitationModel = new GameInvitationModel {
                InvitedBy = email }; 
            HttpContext.Session.SetString("email", email); 
            return View(gameInvitationModel); 
        }

        [HttpPost]
        public IActionResult Index(GameInvitationModel gameInvitationModel)
        {
            return Content(_stringLocalizer["GameInvitationConfirmation Message", gameInvitationModel.EmailTo]);
        }
    }
}