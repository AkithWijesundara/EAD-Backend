/*
 * File: User with role DTO
 * Author: Perera V. H. P.
 * Description: This file contains DTO class for retring user.
 * Created: 07/10/2024
*/

public class UserWithRoleDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public bool Active { get; set; }
    public string Role { get; set; }
}
