using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public ProductPricesController(IProductPricesServices productPricesService)
        {
            _productPricesService = productPricesService;
        }

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

        [HttpPost("CreateOrEditProductPrice")]
        public async Task<ActionResult<ApiResponse<string>>> CreateOrEditProductPrice(
            [FromQuery] CreateOrEditProductPricesDto input
        )
        {
            try
            {
                var result = await _productPricesService.CreateOrEdit(input);
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
