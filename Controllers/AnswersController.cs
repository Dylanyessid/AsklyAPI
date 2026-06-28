using AcaHelpAPI.Data;
using AcaHelpAPI.DTOs;
using AcaHelpAPI.Models;
using AcaHelpAPI.Services;
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
        private readonly IAnswerService _answerService;

        public AnswersController(MiDbContext context, IAnswerService answerService)
        {
            _context = context;
            _answerService = answerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAnswers(int questionId, [FromQuery] int limit = 20, [FromQuery] string? cursor = null)
        {
            var questionExists = await _context.Questions.AnyAsync(question => question.Id == questionId);
            if (!questionExists)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Pregunta no encontrada", "QUESTION_NOT_FOUND"));
            }

            try
            {
                var page = await _answerService.GetAnswersByQuestionAsync(questionId, limit, cursor);
                return Ok(ApiResponse<AnswerListPageDTO>.SuccessResponse(page, "ANSWERS_GIVEN", "Respuestas obtenidas exitosamente"));
            }
            catch (ArgumentException)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Cursor inválido", "INVALID_CURSOR"));
            }
            catch (FormatException)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Cursor inválido", "INVALID_CURSOR"));
            }
        }

        [HttpGet("accepted")]
        public async Task<IActionResult> GetAcceptedAnswer(int questionId)
        {
            var questionExists = await _context.Questions.AnyAsync(question => question.Id == questionId);
            if (!questionExists)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Pregunta no encontrada", "QUESTION_NOT_FOUND"));
            }

            var acceptedAnswer = await _answerService.GetAcceptedAnswerByQuestionAsync(questionId);
            if (acceptedAnswer == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("No hay respuesta aceptada", "ACCEPTED_ANSWER_NOT_FOUND"));
            }

            return Ok(ApiResponse<AnswerListItemDTO>.SuccessResponse(acceptedAnswer, "ACCEPTED_ANSWER_GIVEN", "Respuesta aceptada obtenida exitosamente"));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostAnswer(int questionId, CreateAnswerDTO dto)
        {
            var stringUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var question = await _context.Questions.FindAsync(questionId);
            if (question == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Pregunta no encontrada", "QUESTION_NOT_FOUND"));
            }

            var answer = new Answer
            {
                QuestionId = questionId,
                UserId = int.Parse(stringUserId),
                Body = dto.Body,
                IsAccepted = false,
                VoteCount = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();

            var responseData = new AnswerResponseDTO
            {
                Id = answer.Id,
                QuestionId = answer.QuestionId,
                UserId = answer.UserId,
                Body = answer.Body,
                IsAccepted = answer.IsAccepted,
                VoteCount = answer.VoteCount,
                CreatedAt = answer.CreatedAt,
                UpdatedAt = answer.UpdatedAt
            };

            return CreatedAtAction(
                nameof(PostAnswer),
                new { questionId = questionId, id = answer.Id },
                ApiResponse<AnswerResponseDTO>.SuccessResponse(responseData, "ANSWER_CREATED", "Respuesta creada exitosamente"));
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

            var responseData = new AnswerResponseDTO
            {
                Id = answer.Id,
                QuestionId = answer.QuestionId,
                UserId = answer.UserId,
                Body = answer.Body,
                IsAccepted = answer.IsAccepted,
                VoteCount = answer.VoteCount,
                CreatedAt = answer.CreatedAt,
                UpdatedAt = answer.UpdatedAt
            };

            return Ok(ApiResponse<AnswerResponseDTO>.SuccessResponse(responseData, "ANSWER_UPDATED", "Respuesta actualizada exitosamente"));
        }
    }
}
