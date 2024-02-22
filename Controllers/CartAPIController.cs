﻿using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private ResponseDto _response;
        private IMapper _mapper;
        private AppDbContext _db;

        public CartAPIController(ResponseDto response, IMapper mapper, AppDbContext db)
        {
            _response = response;
            _mapper = mapper;
            _db = db;
        }


        [HttpPost("CartUpsert")]
        public async Task<IActionResult> CartUpsert(CartDto cartDto)
        {
            try
            {
                // quando l'utente inserisce il primo articolo devo creare il CartHeader e il CartDetails
                var cartHeaderFromDb = _db.CartHeaders.FirstOrDefault(x => x.UserId == cartDto.CartHeader.UserId);
                if (cartHeaderFromDb == null)
                {
                    // create header and details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _db.CartHeaders.Add(cartHeader);
                    await _db.SaveChangesAsync();

                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _db.SaveChangesAsync();
                }
                else
                {
                    // if header is not null
                    // check if details has same product
                    var cartDetailsFromDB = _db.CartDetails.
                              FirstOrDefault(p => p.ProductId == cartDto.CartDetails.First().ProductId
                              && p.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                    if (cartDetailsFromDB == null)
                    {
                        // create cartDetails
                    }
                    else
                    {
                        // update count in cart details
                    }
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
            }

        }
    }
}
