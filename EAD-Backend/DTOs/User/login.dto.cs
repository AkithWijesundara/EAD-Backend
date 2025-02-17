/*
 * File: Login DTO
 * Author: Perera V. H. P.
 * Description: This file contains DTO class for login.
 * Created: 07/10/2024
*/

namespace EAD_Backend.DTOs
{
    public class LoginDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}