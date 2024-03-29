﻿using AutoMapper;
using Mango.Services.ShoppingCartAPI.Models.Dtos;
using Mango.Services.ShoppingCartAPI.Models;

namespace Mango.Services.ShoppingCartAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(
                config =>
                {
                    config.CreateMap<ProductDto, Product>().ReverseMap();
                    config.CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
                    config.CreateMap<CartDetails, CartDetailsDto>().ReverseMap();
                    config.CreateMap<Cart, CartDto>().ReverseMap();
                }
                );
            return mappingConfig;
        }
    }
}
