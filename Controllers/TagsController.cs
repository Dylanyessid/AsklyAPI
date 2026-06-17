using AcaHelpAPI.Data;
using AcaHelpAPI.DTOs;
using AcaHelpAPI.Helpers;
using AcaHelpAPI.Models;
using AcaHelpAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AcaHelpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly MiDbContext _context;
        private readonly ICacheService _cacheService;

        public TagsController(MiDbContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        // GET: api/Tags
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTags()
        {
            string cacheKey = "all_tags";

            var cachedTags = await _cacheService.GetItemInCache<List<GetTagResponseDTO>>(cacheKey);
            
            if (cachedTags != null)
            {
                var cachedResponseData = new GetMultipleTagsResponseDTO
                {
                    tags = cachedTags
                };
                return Ok(ApiResponse<GetMultipleTagsResponseDTO>.SuccessResponse(cachedResponseData, ApiSuccessResponseCodes.TAGS_GIVEN.ToString(), ""));
            }

            var tags = await _context.Tags.ToListAsync();
            var responseTags = tags.Select(tag => new GetTagResponseDTO { id = tag.Id, name = tag.Name }).ToList();
 
            
            await _cacheService.SetItemInCache(cacheKey, responseTags, TimeSpan.FromHours(1));
            var responseData = new GetMultipleTagsResponseDTO
            {
                tags = responseTags,
                
            };
            return Ok(ApiResponse<GetMultipleTagsResponseDTO>.SuccessResponse(responseData, ApiSuccessResponseCodes.TAGS_GIVEN.ToString(), ""));
        }

        // GET: api/Tags/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tag>> GetTag(int id)
        {
            var tag = await _context.Tags.FindAsync(id);

            if (tag == null)
            {
                return NotFound(ApiResponse<GetTagResponseDTO>.ErrorResponse(ApiErrorResponseCodes.TAG_NOT_FOUND.ToString(), "Tag no encontrada"));
            }

            var responseData = new GetTagResponseDTO { id = tag.Id, name = tag.Name };

            return Ok(ApiResponse<GetTagResponseDTO>.SuccessResponse(responseData, ApiSuccessResponseCodes.TAGS_GIVEN.ToString(), ""));
        }

      
    }
}
