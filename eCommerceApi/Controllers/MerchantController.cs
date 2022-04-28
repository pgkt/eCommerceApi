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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MerchantController : ControllerBase
    {

        private IApplicationDbContext _dbContext { get; }
        private IMapper _mapper;
        private ILogger<MerchantController> _logger;

        public MerchantController(IApplicationDbContext dbContext, IMapper mapper, ILogger<MerchantController> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("GetAllMerchants")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllMerchants()
        {
            try
            {
                var merchantList = await GetMerchantDetails();

                if (merchantList == null || merchantList.Count == 0)
                {
                    return Ok(new { message="Merchant list is empty" });
                }

                var merchantListInfo = _mapper.Map<List<MerchantViewModel>>(merchantList);
                var roles = await GetRoles();

                foreach (var m in merchantListInfo)
                {
                    m.RoleName = roles.Where(r => r.RoleID == m.RoleId).FirstOrDefault().RoleName;
                }

                return Ok(merchantListInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Error occured while retrieving Merchants");
            }
        }

        [HttpGet]
        [Route("{merchantId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RetriveMerchantData(int merchantId)
        {
            try
            {
                var merchant = await _dbContext.Merchants.Where(m => m.MerchantId == merchantId).FirstOrDefaultAsync();

                if (merchant == null)
                {
                    return Ok(new { message = "Merchant not found." });
                }

                var merchantInfo = _mapper.Map<MerchantViewModel>(merchant);
                var roles = await GetRoles();

                merchantInfo.RoleName = roles.Where(r => r.RoleID == merchantInfo.RoleId).FirstOrDefault().RoleName;

                return Ok(merchantInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Error occured while retrieving Merchant info");
            }
        }

        [HttpPost]
        [Route("AddMerchant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddMerchant([FromBody] MerchantDto merchant)
        {
            try
            {
                var merchantList = await GetMerchantDetails();
                if(merchantList.Any(m => m.LoginID == merchant.LoginID))
                {
                    return Ok(new { message = "Login Id already exists. Please try with some other id." });
                }

                var merchantObj = _mapper.Map<Merchant>(merchant);

                await _dbContext.Merchants.AddAsync(merchantObj);
                await _dbContext.SaveChangesAsync(default);

                return Ok(new { message = "Merchant Created Successfully"});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Error occured while adding Merchant");
            }
        }

        [HttpPut]
        [Route("UpdateMerchant/{merchantId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status304NotModified)]
        public async Task<IActionResult> UpdateMerchantData(int merchantId, [FromBody] MerchantDto merchant)
        {
            try
            {
                var merchantList = await GetMerchantDetails();
                if(!merchantList.Any(m => m.MerchantId == merchantId))
                {
                    return Ok(new { message = "Cannot update as the merchant profile is deleted" });
                }
                else if (merchantList.Any(m => m.MerchantId != merchantId && m.LoginID == merchant.LoginID))
                {
                    return Ok(new { message = "Login Id already exists. Please try with some other id." });
                }

                var merchantData = merchantList.Where(m => m.MerchantId == merchantId).FirstOrDefault();
                _mapper.Map(merchant, merchantData);
                await _dbContext.SaveChangesAsync(default);

                return Ok(new { message = "Merchant Updated Successfully"});

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Error occured while updating Merchant info");
            }
        }

        [HttpDelete]
        [Route("DeleteMerchant/{merchantId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveMerchant(int merchantId)
        {
            try
            {
                var merchant = _dbContext.Merchants.Where(m => m.MerchantId == merchantId).FirstOrDefault();
                if(merchant == null)
                {
                    return Ok(new { message = "Merchant not found." });
                }

                merchant.STATUS = "Deleted";
                await _dbContext.SaveChangesAsync(default);

                return Ok(new { message = "Merchant Deleted Successfully"});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Error occured while deleting Merchant");
            }
        }

        private async Task<List<Merchant>> GetMerchantDetails()
        {
            return await _dbContext.Merchants.Where(m => m.STATUS != "Deleted").ToListAsync();
        }

        private async Task<List<Role>> GetRoles()
        {
            return await _dbContext.Roles.ToListAsync();
        }
    }
}
