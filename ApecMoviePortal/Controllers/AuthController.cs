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

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            var response = await _authService.LoginAsync(loginDto);
            if (response.StatusCode == 200)
            {
                // Save tokens to cookies
                Response.Cookies.Append("AccessToken", response.Data.AccessToken, new CookieOptions { HttpOnly = true, Secure = true });
                Response.Cookies.Append("RefreshToken", response.Data.RefreshToken, new CookieOptions { HttpOnly = true, Secure = true });
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Message = "Đăng nhập thất bại, Hãy thử lại sau!";
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
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AccessToken");
            Response.Cookies.Delete("RefreshToken");
            return RedirectToAction("Login", "Auth");
        }
    }
}
