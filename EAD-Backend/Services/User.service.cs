/*
 * File: User service
 * Author: Perera V. H. P.
 * Description: This file contains service functions for user management.
 * Created: 07/10/2024
*/


using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EAD_Backend.Data;
using EAD_Backend.DTOs;
using EAD_Backend.Models;
using EAD_Backend.OtherModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace EAD_Backend.Services
{

    public class UserService
    {

        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<User> _userModel;
        private readonly IMongoCollection<Role> _roleModel;
        private readonly EmailService _emailService;




        public UserService(MongoDBService mongoDbService, IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
            _userModel = mongoDbService.Database?.GetCollection<User>("users");
            _roleModel = mongoDbService.Database?.GetCollection<Role>("roles");

        }

        //! =======================================================  Define Business | DB Operations for User ===================================>
        //! Get all users
        public async Task<IEnumerable<User>> GetAll()
        {
            return await _userModel.Find(user => true).ToListAsync();
        }

        //! create a user
        public async Task<User> Create(User user)
        {
            String password = user.Password;
            user.Active = true;
            user.Status = "Approved";
            user.Password = _passwordHasher.HashPassword(user, user.Password);
            user.IsFirstLogin = true;
            await _userModel.InsertOneAsync(user);
            user.Password = password;
            return user;
        }


        //! Login
        public async Task<object> Login(LoginDto user)
        {
            var userInDb = await _userModel.Find(u => u.Email == user.Email).FirstOrDefaultAsync();
            if (userInDb == null)
            {
                throw new ArgumentException("User not found");

            }


            var response = _passwordHasher.VerifyHashedPassword(userInDb, userInDb.Password, user.Password);

            if (response == PasswordVerificationResult.Failed)
            {

                throw new UnauthorizedAccessException("Invalid password");
            }


            var role = await _roleModel.Find(r => r.Id == userInDb.Role).FirstOrDefaultAsync();

            if (role == null)
            {
                throw new ArgumentException("Role not found");
            }



            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", userInDb.Id.ToString()),
                new Claim("Email", userInDb.Email.ToString()),
                new Claim(ClaimTypes.Role, role.Name)

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signin = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(10),
                signingCredentials: signin
            );

            string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
            return new
            {
                token = tokenValue,
                role = role.Name,
                name = userInDb.Name,
                profile = userInDb.ProfileImage,
                email = userInDb.Email
            }; ;

        }


        //! Self Register
        public async Task<User> SelfRegister(SelfRegisterDto user)
        {

            // check if user exists
            var userInDb = await _userModel.Find(u => u.Email == user.Email).FirstOrDefaultAsync();

            if (userInDb != null)
            {
                throw new ArgumentException("User email already exists");
            }

            var role = await _roleModel.Find(r => r.Name == "Customer").FirstOrDefaultAsync();
            if (role.Id == null)
            {
                throw new ArgumentException("Role not found");
            }




            User newUser = new User
            {
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                Role = role.Id,
                Active = false,
                Status = "Pending",
                IsFirstLogin = false
            };

            newUser.Password = _passwordHasher.HashPassword(newUser, user.Password);


            await _userModel.InsertOneAsync(newUser);
            return newUser;
        }


        //! Activate account
        public async Task<User> ActivateAccount(string id)
        {
            var user = await _userModel.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }


            user.Active = !user.Active;


            await _userModel.ReplaceOneAsync(u => u.Id == id, user);


            return user;
        }


        //! Deactivate account
        public async Task<User> DeactivateAccount(string id)
        {
            var user = await _userModel.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            user.Active = false;
            var response = await _userModel.ReplaceOneAsync(u => u.Id == id, user);
            return user;
        }

        //! Update user
        public async Task<User> Update(string id, UpdateUserDto userDto)
        {
            // Find the existing user in the database
            var userInDb = await _userModel.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (userInDb == null)
            {
                throw new ArgumentException("User not found");
            }

            // Update the user properties with values from the DTO
            userInDb.Name = userDto.Name;

            // Update the existing user in the database
            var response = await _userModel.ReplaceOneAsync(u => u.Id == id, userInDb);

            // Check if the update was successful
            if (!response.IsAcknowledged || response.ModifiedCount == 0)
            {
                throw new Exception("Failed to update the user");
            }

            // Return the updated user
            return userInDb;
        }

        //! Get all vendors
        public async Task<IEnumerable<User>> GetAllVendors()
        {
            return await _userModel.Find(user => true).ToListAsync();
        }

        //! Get user by id
        public async Task<User> GetById(string id)
        {
            return await _userModel.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        //! Get user by token
        public async Task<User> GetUserByToken(string id)
        {
            return await _userModel.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        //! Approve Customer
        public async Task<object> ApproveCustomer(string id)
        {
            var user = await _userModel.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }


            user.Status = "Approved";
            var response = await _userModel.ReplaceOneAsync(u => u.Id == id, user);
            return response;

        }

        //! Get all customers
        public async Task<IEnumerable<UserWithRoleDto>> GetAllCustomers()
        {

            var users = await _userModel
                .Find(u => u.Role == "66e97a5d59553323609b4f1e")
                .SortBy(u => u.Active)
                .ToListAsync();


            var role = await _roleModel.Find(r => r.Id == "66e97a5d59553323609b4f1e").FirstOrDefaultAsync();
            if (role == null)
            {
                throw new ArgumentException("Role not found");
            }


            var usersWithRole = users.Select(user => new UserWithRoleDto
            {
                Id = user.Id.ToString(),
                Name = user.Name,
                Email = user.Email,
                Active = user.Active ?? false,
                Role = role.Name
            }).ToList();

            return usersWithRole;
        }

        // public async Task<object> SendOtp(SendOtpDto dto)
        // {
        //     var user = await _userModel.Find(u => u.Email == dto.Email).FirstOrDefaultAsync();
        //     if (user == null)
        //     {
        //         throw new ArgumentException("User not found");
        //     }

        //     var otp = new Random().Next(1000, 9999).ToString();
        //     user.Otp = otp;
        //     await _userModel.ReplaceOneAsync(u => u.Email == dto.Email, user);

        //     // bool isSent = await _emailService.SendEmailAsync(user.Email, "Reset OTP", otp);

        //     if (!isSent)
        //     {
        //         throw new Exception("Failed to send OTP");
        //     }

        //     return new
        //     {
        //         otp = otp
        //     };
        // }

        //! reset password
        public async Task<object> ResetPassword(ResetPasswordDto dto)
        {
            var user = await _userModel.Find(u => u.Email == dto.Email).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }



            user.Password = _passwordHasher.HashPassword(user, dto.Password);
            await _userModel.ReplaceOneAsync(u => u.Email == dto.Email, user);

            return new
            {
                message = "Password reset"
            };
        }




    }

}