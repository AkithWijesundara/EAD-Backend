/*
 * File:Master Data Controller
 * Author: Perera V. H. P.
 * Description: This file contains Endpoints for Master data and dropdowns data.
 * Created: 07/10/2024
*/


using EAD_Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace EAD_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterDataController : ControllerBase
    {
        private readonly MasterDataService _masterDataService;

        public MasterDataController(MasterDataService masterDataService)
        {
            _masterDataService = masterDataService;
        }

        //! ======================================================== Define API Endpoints ============================================================>
        //! GET: api/MasterData/GetRoles
        [HttpGet("GetRoles")]
        public IActionResult GetRoles()
        {
            return Ok(_masterDataService.GetRoles());
        }

        //! Get Product Categories
        [HttpGet("GetCategories")]
        public IActionResult GetCategories()
        {
            return Ok(_masterDataService.GetCategories());
        }

        //! Get product sub categories
        [HttpGet("GetSubCategories/{categoryId}")]
        public async Task<IActionResult> GetSubCategoriesByCategoryId(string categoryId)
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                return BadRequest("CategoryId cannot be null or empty");
            }

            var subCategories = await _masterDataService.GetSubCategoriesByCategoryId(categoryId);

            if (subCategories == null || subCategories.Count == 0)
            {
                return NotFound($"No subcategories found for category id {categoryId}");
            }

            return Ok(subCategories);
        }
    }
}