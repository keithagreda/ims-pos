using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using POSIMSWebApi.Auth;
using POSIMSWebApi.Dtos;
using POSIMSWebApi.Dtos.AuthDtos;
using POSIMSWebApi.Dtos.Product;
using POSIMSWebApi.Interfaces;

namespace POSIMSWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductServices _productServices;
        private readonly UserManager<ApplicationIdentityUser> _userManager;

        public ProductController(IProductServices productServices, UserManager<ApplicationIdentityUser> userManager)
        {
            _productServices = productServices;
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
        [HttpGet("GetProduct")]
        public async Task<ActionResult<ApiResponse<IList<ProductDto>>>> GetAllProducts(
            [FromQuery] FilterText input
        )
        {
            try
            {
                var result = await _productServices.GetAll(input);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new ApiResponse<IList<ProductDto>>()
                    {
                        Data = new List<ProductDto>(),
                        ErrorMessage = ex.Message,
                        IsSuccess = false
                    }
                );
            }
        }
        [Authorize(Roles = "User")]
        [HttpPost("CreateOrEditProduct")]
        public async Task<ActionResult<ApiResponse<ProductDto>>> CreateOrEditProduct(
            [FromQuery] CreateOrEditProductDto input
        )
        {
            try
            {
                var userId = await GetCurrentUser();
                var result = await _productServices.CreateOrEdit(input, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new ApiResponse<ProductDto>()
                    {
                        Data = new ProductDto(),
                        ErrorMessage = ex.Message,
                        IsSuccess = false
                    }
                );
            }
        }
    }
}
