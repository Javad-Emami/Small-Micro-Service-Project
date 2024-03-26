using Marnico.WebApp.Model;
using Marnico.WebApp.Models;
using Marnico.WebApp.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Marnico.WebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDto> list = new() ;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.GetAllProductAsync<ResponseDto>(accessToken);
            if(response != null && response.IsSuccess)
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));

            return View(list);
        }

        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductCreate(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var modelResponse = await _productService.CreateProductAsync<ResponseDto>(model, accessToken);
               if(modelResponse != null && modelResponse.IsSuccess)
                return RedirectToAction("ProductIndex");
            }
            return View(model);
        }

        public async Task<IActionResult> ProductUpdate(int productId)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.GetProductByIdAsync<ResponseDto>(productId, accessToken);
            if (response != null && response.IsSuccess)
            {
                var model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductUpdate(ProductDto productDto)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var productModel = await _productService.UpdateProductAsync<ResponseDto>(productDto, accessToken);
            if (productModel != null && productModel.IsSuccess)
                return RedirectToAction("ProductIndex");
            return View(productModel);
        }

        public async Task<IActionResult> ProductDelete(int productId)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.GetProductByIdAsync<ResponseDto>(productId, accessToken);
            if (response != null && response.IsSuccess)
            {
                var model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductDelete(ProductDto productDto)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.DeleteProductAsync<ResponseDto>(productDto.ProductId, accessToken);
            if (response.IsSuccess)
                return RedirectToAction("ProductIndex");
            return View(productDto);
        }
    }
}
