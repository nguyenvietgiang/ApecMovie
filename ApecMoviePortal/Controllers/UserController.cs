using ApecMoviePortal.Services.AuthServices;
using Microsoft.AspNetCore.Mvc;

namespace ApecMoviePortal.Controllers
{
    public class UserController : Controller
    { 

        private readonly IAuthService _authService;

        public UserController(IAuthService authService)
        {
            _authService = authService;
        }

    public async Task<IActionResult> Info()
        {
            var token = Request.Cookies["AccessToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            var userInfo = await _authService.GetUserInfoAsync(token);
            return View(userInfo);
        }
    }
} 
