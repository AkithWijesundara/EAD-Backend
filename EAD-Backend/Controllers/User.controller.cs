/*
 * File: User Controller
 * Author: Perera V. H. P.
 * Description: This file contains the endpoints for User management.
 * Created: 07/10/2024
*/

using EAD_Backend.DTOs;
using EAD_Backend.Models;
using EAD_Backend.OtherModels;
using EAD_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EAD_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly EmailService _emailService;

        public UserController(UserService userService, EmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }

        //! ======================================================== Define API Endpoints ============================================================>

        //! Get all users
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _userService.GetAll();
            return Ok(new ApiResponse<object>("Successful", response));
        }

        //! Register a user
        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public async Task<IActionResult> Create(User user)
        {
            var response = await _userService.Create(user);
            var emailDto = new EmailDTO
            {
                Email = response.Email,
                Subject = "SuperMart Account",
                Message = $@"
            <h1>Your Account is created</h1>
            <p>Congratulations!</p> 
            <p>Your account has been activated. You can now log in using your credentials:</p>
            <p><strong>Email:</strong> {response.Email}</p>
            <p><strong>Password:</strong> {response.Password}</p>
            "
            };

            await _emailService.SendEmailAsync(emailDto);
            return Ok(new ApiResponse<User>("Create successful", response));
        }

        //! Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto user)
        {
            var response = await _userService.Login(user);
            return Ok(new ApiResponse<object>("Login successful", response));
        }

        //! Self Register
        [HttpPost("self-register")]
        public async Task<IActionResult> SelfRegister(SelfRegisterDto user)
        {
            var response = await _userService.SelfRegister(user);
            var emailDto = new EmailDTO
            {
                Email = user.Email,
                Subject = "SuperMart Account",
                Message = @"
                        <h1>Registration</h1>
                        <p>You have successfully registred</p> 
                        <p>Once the CSR activated account you can purchase products</p>
                        "
            };

            await _emailService.SendEmailAsync(emailDto);
            return Ok(new ApiResponse<User>("Create successful", response));
        }

        //! Activate account
        [Authorize(Roles = "CSR , Admin")]
        [HttpGet("activate/{id}")]
        public async Task<IActionResult> ActivateAccount(string id)
        {
            var response = await _userService.ActivateAccount(id);
            if (response.Active == false)
            {
                return Ok(new ApiResponse<object>("Account deactivated", response));
            }
            else
            {
                var emailDto = new EmailDTO
                {
                    Email = response.Email,
                    Subject = "SuperMart Account",
                    Message = @"
                        <h1>Account Activated</h1>
                        <p>Congratulations</p> 
                        <p>Your account has been activated. Now you can purchase products</p>
                        "
                };

                await _emailService.SendEmailAsync(emailDto);
                return Ok(new ApiResponse<object>("Account activated", response));
            }

        }

        //! Deactivate account
        [Authorize]
        [HttpGet("deactivate/{id}")]
        public async Task<IActionResult> DeactivateAccount(string id)
        {
            var response = await _userService.DeactivateAccount(id);
            var emailDto = new EmailDTO
            {
                Email = response.Email,
                Subject = "SuperMart Account",
                Message = $@"
            <h1>Your Account is Deactivated</h1>
            <p>Your account has been deactivated. You has no longer access to login</p>
            "
            };

            await _emailService.SendEmailAsync(emailDto);
            return Ok(new ApiResponse<object>("Account deactivated", response));
        }

        //! Update user
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UpdateUserDto user)
        {
            var response = await _userService.Update(id, user);
            return Ok(new ApiResponse<object>("Update successful", response));
        }

        //! Get all vendors
        [Authorize(Roles = "Admin")]
        [HttpGet("vendors")]
        public async Task<IActionResult> GetAllVendors()
        {
            var response = await _userService.GetAllVendors();
            return Ok(new ApiResponse<object>("Successful", response));
        }

        //! Get user by ID
        // [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _userService.GetById(id);
            return Ok(new ApiResponse<object>("Successful", response));

        }

        //! get user  by token
        [Authorize]
        [HttpGet("token")]
        public async Task<IActionResult> GetUserByToken()
        {
            var userId = User.FindFirst("UserId")?.Value;
            var response = await _userService.GetUserByToken(userId);
            return Ok(new ApiResponse<object>("Successful", response));
        }

        //! Approve Customers
        [Authorize]
        [HttpGet("approve/{id}")]
        public async Task<IActionResult> ApproveCustomer(string id)
        {
            var response = await _userService.ApproveCustomer(id);
            return Ok(new ApiResponse<object>("Customer approved", response));
        }

        [Authorize]
        [HttpGet("customers")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var response = await _userService.GetAllCustomers();
            return Ok(new ApiResponse<object>("Successful", response));
        }

        // //! Send OTP
        // [HttpPost("send-otp")]
        // public async Task<IActionResult> SendOtp(SendOtpDto user)
        // {
        //     var response = await _userService.SendOtp(user);
        //     return Ok(new ApiResponse<object>("OTP sent", response));
        // }

        //! Reset Password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto user)
        {
            var response = await _userService.ResetPassword(user);
            return Ok(new ApiResponse<object>("Password reset", response));
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> sendEmail(EmailDTO dto)
        {
            var emailDto = new EmailDTO
            {
                Email = dto.Email,
                Subject = dto.Subject,
                Message = @"
                        <h1>OTP</h1>
                        <p>Your OTP is <strong>123456</strong></p>
                        "
            };

            var response = await _emailService.SendEmailAsync(emailDto);
            return Ok(new ApiResponse<object>("Email sent", response));
        }
    }
}