using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcaHelpAPI.Data;
using AcaHelpAPI.Models;
using AcaHelpAPI.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using AcaHelpAPI.Services;

namespace AcaHelpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly MiDbContext _context;
        private readonly IQuestionService _questionService;

        public QuestionsController(MiDbContext context, IQuestionService questionService)
        {
            _context = context;
            _questionService = questionService;
        }

        // GET: api/Questions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Question>>> GetQuestions()
        {
            return await _context.Questions.ToListAsync();
        }

        // GET: api/Questions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Question>> GetQuestion(int id)
        {
            var question = await _context.Questions.FindAsync(id);

            if (question == null)
            {
                return NotFound();
            }

            return question;
        }

        // PUT: api/Questions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuestion(int id, Question question)
        {
            if (id != question.Id)
            {
                return BadRequest();
            }

            _context.Entry(question).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Questions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Question>> PostQuestion(CreateQuestionDTO questionDto)
        {
            var stringUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;


            var result = await this._questionService.CreateQuestion(int.Parse(stringUserId), questionDto);
           

            var responseData = new CreateQuestionResponseDTO
            {
                id = result.Id,
                body = result.Body,
                tagId = result.TagId,
                title = result.Title,
                userId = result.UserId,
                createdAt = result.CreatedAt,
                UpdatedAt = result.UpdatedAt
            };
            var responsePayload = ApiResponse<CreateQuestionResponseDTO>.SuccessResponse(responseData, "QUESTION_CREATED", "Question created successfully");
            return CreatedAtAction("GetQuestion", new { id = result.Id }, responsePayload);
        }

        // DELETE: api/Questions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.Id == id);
        }
    }
}
