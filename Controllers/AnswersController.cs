using AcaHelpAPI.Data;
using AcaHelpAPI.DTOs;
using AcaHelpAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AcaHelpAPI.Controllers
{
    [Route("api/questions/{questionId:int}/answers")]
    [ApiController]
    public class AnswersController : ControllerBase
    {
        private readonly MiDbContext _context;

        public AnswersController(MiDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostAnswer(int questionId, CreateAnswerDTO dto)
        {
            var stringUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (stringUserId == null || !int.TryParse(stringUserId, out var userId))
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse("Usuario no autenticado", "UNAUTHORIZED"));
            }

            var question = await _context.Questions.FindAsync(questionId);
            if (question == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Pregunta no encontrada", "QUESTION_NOT_FOUND"));
            }

            var answer = new Answer
            {
                QuestionId = questionId,
                UserId = userId,
                Body = dto.Body,
                IsAccepted = false,
                VoteCount = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(PostAnswer),
                new { questionId = questionId, id = answer.Id },
                ApiResponse<Answer>.SuccessResponse(answer, "ANSWER_CREATED", "Respuesta creada exitosamente"));
        }

        [Authorize]
        [HttpPut("{answerId:int}")]
        public async Task<IActionResult> PutAnswer(int questionId, int answerId, UpdateAnswerDTO dto)
        {
            var stringUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (stringUserId == null || !int.TryParse(stringUserId, out var userId))
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse("Usuario no autenticado", "UNAUTHORIZED"));
            }

            var answer = await _context.Answers
                .FirstOrDefaultAsync(a => a.Id == answerId && a.QuestionId == questionId);

            if (answer == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Respuesta no encontrada", "ANSWER_NOT_FOUND"));
            }

            if (answer.UserId != userId)
            {
                return Forbid();
            }

            answer.Body = dto.Body;
            answer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<Answer>.SuccessResponse(answer, "ANSWER_UPDATED", "Respuesta actualizada exitosamente"));
        }
    }
}
