/*
 * File: UUpdate Product DTO
 * Author: Perera V. H. P.
 * Description: This file contains DTO class for update product.
 * Created: 07/10/2024
*/

namespace EAD_Backend.DTOs
{
    public class UpdateProductDto
    {

        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public double Price { get; set; }
        public string[]? Images { get; set; }
        public bool Active { get; set; }
        public int StockCount { get; set; }

    }
}