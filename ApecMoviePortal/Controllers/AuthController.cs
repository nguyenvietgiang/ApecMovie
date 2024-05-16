using ApecMoviePortal.Models;
using ApecMoviePortal.Services.AuthServices;
using Microsoft.AspNetCore.Mvc;

namespace ApecMoviePortal.Controllers
{
    public class AuthController : Controller
    {
        public readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Login() 
        {
            return View();
        }

        public IActionResult Register()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.RegisterUserAsync(registerUserDto);
                if (result)
                {
                    ViewBag.Message = "Tài khoản đã được đăng ký thành công";
                    return View();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Đang ký thất bại, hãy thử lại.");
                }
            }
            return View(registerUserDto);
        }
    }
}
