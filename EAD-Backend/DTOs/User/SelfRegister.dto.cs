/*
 * File: Self Register DTO
 * Author: Perera V. H. P.
 * Description: This file contains DTO class for customer self registration.
 * Created: 07/10/2024
*/


namespace EAD_Backend.DTOs
{
    public class SelfRegisterDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Name { get; set; }

    }
}