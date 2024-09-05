using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using POSIMSWebApi.Auth;
using POSIMSWebApi.Dtos;
using POSIMSWebApi.Dtos.Product;
using POSIMSWebApi.Dtos.ProductPrices;
using POSIMSWebApi.Interfaces;

namespace POSIMSWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductPricesController : ControllerBase
    {
        private readonly IProductPricesServices _productPricesService;
        private readonly UserManager<ApplicationIdentityUser> _userManager;

        public ProductPricesController(IProductPricesServices productPricesService, UserManager<ApplicationIdentityUser> userManager)
        {
            _productPricesService = productPricesService;
            _userManager = userManager;
        }

        private async Task<Guid> GetCurrentUser()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return Guid.Empty;
            }
            return Guid.Parse(await _userManager.GetUserIdAsync(user));
        }
        [Authorize(Roles = "User")]
        [HttpGet("GetAllProductPrices")]
        public async Task<ActionResult<ApiResponse<IList<ProductPricesDto>>>> GetAllProductPrices(
            [FromQuery] FilterText input
        )
        {
            try
            {
                var result = await _productPricesService.GetAllProductPrices(input);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new ApiResponse<IList<ProductPricesDto>>()
                    {
                        Data = new List<ProductPricesDto>(),
                        ErrorMessage = ex.Message,
                        IsSuccess = false
                    }
                );
            }
        }
        [Authorize(Roles = "User")]
        [HttpPost("CreateOrEditProductPrice")]
        public async Task<ActionResult<ApiResponse<string>>> CreateOrEditProductPrice(
            [FromQuery] CreateOrEditProductPricesDto input
        )
        {
            try
            {
                var userId = await GetCurrentUser();
                var result = await _productPricesService.CreateOrEdit(input, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new ApiResponse<string>()
                    {
                        Data = "",
                        ErrorMessage = ex.Message,
                        IsSuccess = false
                    }
                );
            }
        }
    }
}
