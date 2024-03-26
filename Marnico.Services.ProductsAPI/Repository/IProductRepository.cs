using Marnico.Services.ProductsAPI.Model.Dtos;

namespace Marnico.Services.ProductsAPI.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductDto>> GetAllProducts();

        Task<ProductDto> GetProductById(int productId);

        Task<ProductDto> CreateUpdateProduct(ProductDto productDto);

        Task<bool> DeleteProduct(int productId);

    }
}
