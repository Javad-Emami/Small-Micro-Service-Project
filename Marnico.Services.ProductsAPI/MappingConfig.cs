using AutoMapper;
using Marnico.Services.ProductsAPI.Model;
using Marnico.Services.ProductsAPI.Model.Dtos;

namespace Marnico.Services.ProductsAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductDto, Products>();
                config.CreateMap<Products, ProductDto>();
            });
            return mappingConfig;
        }
    }
}
