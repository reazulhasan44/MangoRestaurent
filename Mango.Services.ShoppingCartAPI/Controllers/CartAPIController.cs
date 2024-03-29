﻿using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Messages;
using Mango.Services.ShoppingCartAPI.Models.Dtos;
using Mango.Services.ShoppingCartAPI.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        protected ResponseDto _response;
        private readonly ICartRepository _iCartRepository;
        private readonly ICouponRepository _iCouponRepository;
        private readonly IMessageBus _iMessageBus;
        private readonly string checkoutMessageTopic;
        private readonly string checkoutMessageQueue;
        public CartAPIController(ICartRepository iCartRepository, IMessageBus iMessageBus, ICouponRepository iCouponRepository, IConfiguration configuration)
        {
            _configuration = configuration;
            _iCartRepository = iCartRepository;
            _response = new ResponseDto();
            _iMessageBus = iMessageBus;
            _iCouponRepository = iCouponRepository;
            checkoutMessageTopic = _configuration.GetValue<string>("CheckoutMessageTopic");
            checkoutMessageQueue = _configuration.GetValue<string>("CheckoutMessageQueue");
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<object> GetCart(string userId)
        {
            try
            {
                CartDto cartDto = await _iCartRepository.GetCartByUserID(userId);
                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPost("AddCart")]
        public async Task<object> AddCart(CartDto cartDto)
        {
            try
            {
                CartDto cartDt = await _iCartRepository.CreateUpdateCart(cartDto);
                _response.Result = cartDt;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPost("UpdateCart")]
        public async Task<object> UpdateCart(CartDto cartDto)
        {
            try
            {
                CartDto cartDt = await _iCartRepository.CreateUpdateCart(cartDto);
                _response.Result = cartDt;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPost("RemoveCart")]
        public async Task<object> RemoveCart([FromBody] int cartId)
        {
            try
            {
                bool isSuccess = await _iCartRepository.RemoveFromCart(cartId);
                _response.Result = isSuccess;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody] string userId)
        {
            try
            {
                bool isSuccess = await _iCartRepository.RemoveCoupon(userId);
                _response.Result = isSuccess;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                bool isSuccess = await _iCartRepository.ApplyCoupon(cartDto.CartHeader.UserId, cartDto.CartHeader.CouponCode);
                _response.Result = isSuccess;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPost("Checkout")]
        public async Task<object> Checkout(CheckoutHeaderDto checkoutHeaderDto)
        {
            try
            {
                CartDto cartDto = await _iCartRepository.GetCartByUserID(checkoutHeaderDto.UserId);
                if (cartDto == null)
                {
                    return BadRequest();
                }
                if (!string.IsNullOrEmpty(checkoutHeaderDto.CouponCode))
                {
                    CouponDto coupon = await _iCouponRepository.GetCoupon(checkoutHeaderDto.CouponCode);
                    if (checkoutHeaderDto.DiscountTotal != coupon.DiscountAmount)
                    {
                        _response.IsSuccess = false;
                        _response.ErrorMessages = new List<string>() { "Coupon Price has changed, please confirm" };
                        _response.DisplayMessage = "Coupon Price has changed, please confirm";
                        return _response;
                    }
                }
                checkoutHeaderDto.CartDetails = cartDto.CartDetails;
                // logic to add messages to process order
                // topic & subcription
                //await _iMessageBus.PusblishMessage(checkoutHeaderDto, checkoutMessageTopic);
                // queue
                await _iMessageBus.PusblishMessage(checkoutHeaderDto, checkoutMessageQueue);
                await _iCartRepository.ClearCart(checkoutHeaderDto.UserId);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

    }
}
