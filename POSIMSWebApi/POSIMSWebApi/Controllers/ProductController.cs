using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using POSIMSWebApi.Dtos;
using POSIMSWebApi.Dtos.Product;
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
        public async Task<ActionResult<ApiResponse<IList<ProductDto>>>> GetAllProducts([FromQuery]FilterText input)
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
        [HttpPost("CreateOrEditProduct")]
        public async Task<ActionResult<ApiResponse<ProductDto>>> CreateOrEditProduct([FromBody] CreateOrEditProductDto input)
        {
            try
            {
                var result = await _productServices.CreateOrEdit(input);
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
