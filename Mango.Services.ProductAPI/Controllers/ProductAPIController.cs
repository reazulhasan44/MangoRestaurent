﻿using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dtos;
using Mango.Services.ProductAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Security.AccessControl;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        protected ResponseDto _response;
        private readonly IProductRepository _iProductRepository;
        public ProductAPIController(IProductRepository iProductRepository)
        {
            _iProductRepository = iProductRepository;
            _response = new ResponseDto();
        }

        [HttpGet]
       // [Authorize]
        public async Task<object> Get()
        {
            try
            {
                IEnumerable<ProductDto> productDtos = await _iProductRepository.GetProducts();
                _response.Result = productDtos;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() {
                    ex.ToString()
                };
            }
            return _response;
        }

        [HttpGet]
       // [Authorize]
        [Route("{id}")]
        public async Task<object> Get(int id)
        {
            try
            {
                ProductDto productDto = await _iProductRepository.GetProductById(id);
                _response.Result = productDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() {
                    ex.ToString()
                };
            }
            return _response;
        }

        [HttpDelete]
        [Authorize(Roles ="Admin")]
        [Route("{id}")]
        public async Task<object> Delete(int id)
        {
            try
            {
                bool isSuccess = await _iProductRepository.DeleteProduct(id);
                _response.Result = isSuccess;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() {
                    ex.ToString()
                };
            }
            return _response;
        }

        [HttpPost]
        [Authorize]
        public async Task<object> Post([FromBody] ProductDto productDto)
        {
            try
            {
                ProductDto data = await _iProductRepository.CreateUpdateProduct(productDto);
                _response.Result = data;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() {
                    ex.ToString()
                };
            }
            return _response;
        }
        [HttpPut]
        [Authorize]
        public async Task<object> Put([FromBody] ProductDto productDto)
        {
            try
            {
                ProductDto data = await _iProductRepository.CreateUpdateProduct(productDto);
                _response.Result = data;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() {
                    ex.ToString()
                };
            }
            return _response;
        }

    }
}
