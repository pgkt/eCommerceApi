using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using eCommerceApi.Domain.Entities;
using eCommerceApi.Domain.Interfaces;
using eCommerceApi.DTOs;
using eCommerceApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eCommerceApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : BaseController
    {
        private IApplicationDbContext _dbContext { get; }
        private IMapper _mapper;
        private ILogger<ProductController> _logger;

        public ProductController(IApplicationDbContext dbContext, IMapper mapper, ILogger<ProductController> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetAllProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await GetProductDetails();

                if (products == null || products.Count == 0)
                {
                    return Ok(new { message = "Products list is empty" });
                }

                var productList = _mapper.Map<List<ProductViewModel>>(products);
                var categories = await GetProductCategoryDetails();

                foreach(var p in productList)
                {
                    p.ProductCategoryName = categories.Where(pc => pc.ProductCategoryID == p.ProductCategoryId).FirstOrDefault().CategoryName;
                }

                return Ok(productList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Error occured while retrieving Products");
            }
        }

        [HttpGet]
        [Route("{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RetriveProductDetails(int productId)
        {
            try
            {
                var product = await _dbContext.Products.Where(p => p.ProductId == productId).FirstOrDefaultAsync();

                if (product == null)
                {
                    return Ok(new { message = "Product not found." });
                }

                var productData = _mapper.Map<ProductViewModel>(product);
                var categories = await GetProductCategoryDetails();

                productData.ProductCategoryName = categories.Where(pc => pc.ProductCategoryID == productData.ProductCategoryId).FirstOrDefault().CategoryName;

                return Ok(productData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Error occured while retrieving Product info");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("AddProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddProduct([FromBody] ProductDto product)
        {
            try
            {
                var productList = await GetProductDetails();
                if (productList.Any(p => p.ProductName == product.ProductName))
                {
                    return Ok(new { message = "Product name already exists." });
                }

                var productObj = _mapper.Map<Product>(product);
                productObj.CreatedOn = DateTime.UtcNow;
                productObj.CreatedBy = GetUser();

                await _dbContext.Products.AddAsync(productObj);
                await _dbContext.SaveChangesAsync(default);

                return Ok(new { message = "Product Created Successfully"});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Error occured while adding Product");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("UpdateProduct/{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateProductInfo(int productId, [FromBody] ProductDto productDto)
        {
            try
            {
                var productList = await GetProductDetails();
                if (!productList.Any(p => p.ProductId == productId))
                {
                    return Ok(new { message = "Cannot update as the product is deleted" });
                }
                else if (productList.Any(p => p.ProductId != productId && p.ProductName == productDto.ProductName))
                {
                    return Ok(new { message = "Product name already exists." });
                }

                var productObj = productList.Where(p => p.ProductId == productId).FirstOrDefault();
                _mapper.Map(productDto, productObj);
                await _dbContext.SaveChangesAsync(default);

                return Ok(new { message = "Product Updated Successfully"});

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Error occured while updating product info");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("DeleteProduct/{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveProduct(int productId)
        {
            try
            {
                var product = _dbContext.Products.Where(p => p.ProductId == productId).FirstOrDefault();
                if(product == null)
                {
                    return Ok(new { message = "Product not found." });
                }

                product.isDeleted = true;
                await _dbContext.SaveChangesAsync(default);

                return Ok(new { message = "Product Deleted Successfully"});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Error occured while deleting Product");
            }
        }

        private async Task<List<Product>> GetProductDetails()
        {
            return await _dbContext.Products.Where(p => p.isDeleted == false).ToListAsync();
        }

        private async Task<List<ProductCategory>> GetProductCategoryDetails()
        {
            return await _dbContext.productCategories.Where(p => p.isDeleted == false).ToListAsync();
        }
    }
}
