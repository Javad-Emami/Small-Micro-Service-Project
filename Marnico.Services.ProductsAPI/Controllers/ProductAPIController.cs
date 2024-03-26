using Marnico.Services.ProductsAPI.DbContexts;
using Marnico.Services.ProductsAPI.Model.Dto;
using Marnico.Services.ProductsAPI.Model.Dtos;
using Marnico.Services.ProductsAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marnico.Services.ProductsAPI.Controllers
{
    [Route("api/products")]
    public class ProductAPIController : ControllerBase
    {
        protected ResponseDto _response;
        private IProductRepository _productRepository;

        public ProductAPIController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
            this._response = new ResponseDto();
        }
       
        [Authorize]
        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                IEnumerable<ProductDto> productDtos = await _productRepository.GetAllProducts();
                _response.Result = productDtos;
            } 
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message.ToString() };
            }
            return _response;
        }

        [HttpGet]
        [Authorize]
        [Route("{id}")]
        public async Task<object> GetProductById(int id)
        {
            try
            {
                var product =  await _productRepository.GetProductById(id);
                _response.Result=product;
            }
            catch (Exception ex) 
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message.ToString() };
            }
            return _response;
        }

        [HttpPost]
        [Authorize]
        public async Task<object> CreateProduct([FromBody] ProductDto productDto)
        {
            try
            {
                var model = await _productRepository.CreateUpdateProduct(productDto);
                _response.Result = model;
            }
            catch(Exception ex)
            {
                _response.IsSuccess=false;
                _response.ErrorMessages = new List<string> { ex.Message.ToString() };    
            }
            return _response;
        }

        [HttpPut]
        [Authorize]
        public async Task<object> UpdateProduct([FromBody] ProductDto productDto)
        {
            try
            {
            var model = await _productRepository.CreateUpdateProduct(productDto);
            _response.Result = model;
            }
            catch (Exception ex)
            {
                _response.IsSuccess =  false;
                _response.ErrorMessages = new List<string> { ex.Message.ToString() };
            }
            return _response;
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("{id}")]
        public async Task<object> DeleteProduct(int id)
        {
            try
            {
                bool isDeleted = await _productRepository.DeleteProduct(id);
                _response.Result = isDeleted;
            }
            catch (Exception ex)
            {
                _response.Result=false;
                _response.ErrorMessages = new List<string> { ex.Message.ToString() };
            }
            return _response;
        }
    }
}
