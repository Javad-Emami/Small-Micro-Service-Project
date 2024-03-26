using AutoMapper;
using Marnico.Services.ProductsAPI.DbContexts;
using Marnico.Services.ProductsAPI.Model;
using Marnico.Services.ProductsAPI.Model.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Marnico.Services.ProductsAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDBContext _db;
        private readonly IMapper _mapper;

        public ProductRepository(ApplicationDBContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<ProductDto> CreateUpdateProduct(ProductDto productDto)
        {
            var product =  _mapper.Map<ProductDto, Products>(productDto);
            if(product.ProductId > 0)
            {
                _db.Update(product);
            }
            else
            {
                _db.Add(product);               
            }
            await _db.SaveChangesAsync();
            return  _mapper.Map<Products, ProductDto>(product);
        }

        public async Task<bool> DeleteProduct(int productId)
        {
            try
            {
                var product = await _db.Products.Where(a => a.ProductId == productId).FirstOrDefaultAsync();
                if (product != null)
                    _db.Products.Remove(product);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }            
        }

        public async Task<IEnumerable<ProductDto>> GetAllProducts()
        {
            List<Products> productList = await _db.Products.ToListAsync();
            return _mapper.Map<List<ProductDto>>(productList);
        }

        public async Task<ProductDto> GetProductById(int productId)
        {
            var product = await _db.Products.Where(a => a.ProductId == productId).FirstOrDefaultAsync();
            return _mapper.Map<ProductDto>(product);
        }
    }
}
