using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using eCommerceApi.Domain.Entities;
using eCommerceApi.Domain.Interfaces;
using eCommerceApi.DTOs;
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
    public class ProductCategoryController : BaseController
    {
        private IApplicationDbContext _dbContext { get; }
        private IMapper _mapper;
        private ILogger<ProductCategoryController> _logger;

        public ProductCategoryController(IApplicationDbContext dbContext, IMapper mapper, ILogger<ProductCategoryController> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetAllCategories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await GetProductCategoryDetails();

                if (categories == null || categories.Count == 0)
                {
                    return Ok(new { message = "Category list is empty." });
                }

                return Ok(_mapper.Map<List<ProductCategoryDto>>(categories));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Error occured while retrieving Product Categories");
            }
        }

        [HttpGet]
        [Route("{categoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RetriveProductCategory(int categoryId)
        {
            try
            {
                var category = await _dbContext.productCategories.Where(pc => pc.ProductCategoryID == categoryId).FirstOrDefaultAsync();

                if (category == null)
                {
                    return Ok(new { message = "Category not found." });
                }

                return Ok(_mapper.Map<ProductCategoryDto>(category));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Error occured while retrieving Product Category info");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("AddProductCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddProductCategory([FromBody] ProductCategoryDto categoryDto)
        {
            try
            {
                var categories = await GetProductCategoryDetails();
                if (categories.Any(pc => pc.CategoryName == categoryDto.CategoryName))
                {
                    return Ok(new { message = "Product Category already exists." });
                }

                var categoryObj = _mapper.Map<ProductCategory>(categoryDto);
                categoryObj.CreatedOn = DateTime.UtcNow;
                categoryObj.CreatedBy = GetUser();

                await _dbContext.productCategories.AddAsync(categoryObj);
                await _dbContext.SaveChangesAsync(default);

                return Ok(new { message = "Product Category Created Successfully"});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Error occured while adding Product");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("UpdateProductCategory/{categoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateProductCategory(int categoryId, [FromBody] ProductCategoryDto categoryDto)
        {
            try
            {
                var categories = await GetProductCategoryDetails();
                if (!categories.Any(pc => pc.ProductCategoryID == categoryId))
                {
                    return Ok(new { message = "Cannot update as the product category is deleted." });
                }
                else if (categories.Any(pc => pc.ProductCategoryID != categoryId && pc.CategoryName == categoryDto.CategoryName))
                {
                    return Ok(new { message = "Product Category already exists." });
                }
                var categoryObj = categories.Where(m => m.ProductCategoryID == categoryId).FirstOrDefault();
                _mapper.Map(categoryDto, categoryObj);
                await _dbContext.SaveChangesAsync(default);

                return Ok(new { message = "Product Category Updated Successfully" });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Error occured while updating product category");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("DeleteProductCategory/{categoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveProductCategory(int categoryId)
        {
            try
            {
                var category = _dbContext.productCategories.Where(pc => pc.ProductCategoryID == categoryId).FirstOrDefault();
                if (category == null)
                {
                    return Ok(new { message = "Category not found." });
                }

                category.isDeleted = true;
                await _dbContext.SaveChangesAsync(default);

                return Ok(new { message = "Product Category Deleted Successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Error occured while deleting Product Category");
            }
        }

        private async Task<List<ProductCategory>> GetProductCategoryDetails()
        {
            return await _dbContext.productCategories.Where(pc => pc.isDeleted == false).ToListAsync();
        }
    }
}
