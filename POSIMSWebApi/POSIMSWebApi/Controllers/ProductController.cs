using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using POSIMSWebApi.Dtos;
using POSIMSWebApi.Interfaces;

namespace POSIMSWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductServices _productServices;

        public ProductController(IProductServices productServices)
        {
            _productServices = productServices;
        }

        [HttpGet("GetProduct")]
        public async Task<ActionResult<ApiResponse<IList<ProductDto>>>> GetAllProducts(
            FilterText input
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
    }
}
