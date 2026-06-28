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

            var responseData = await _answerService.CreateAnswerAsync(questionId, int.Parse(stringUserId), dto);

            return CreatedAtAction(
                nameof(PostAnswer),
                new { questionId = questionId, id = responseData.Id },
                ApiResponse<AnswerResponseDTO>.SuccessResponse(responseData, "ANSWER_CREATED", "Respuesta creada exitosamente"));
        }

        [Authorize]
        [HttpPut("{answerId:int}")]
        public async Task<IActionResult> PutAnswer(int questionId, int answerId, UpdateAnswerDTO dto)
        {
            var stringUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var userId = int.Parse(stringUserId);

            if (stringUserId == null)
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse("Usuario no autenticado", "UNAUTHORIZED"));
            }

            var questionExists = await _answerService.GetAnswerByIdAsync(questionId, answerId);
            if (questionExists == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Pregunta no encontrada", "QUESTION_NOT_FOUND"));
            }

            if(userId != questionExists.UserId)
            {
                return Forbid();
            }


            var responseData = await _answerService.UpdateAnswerAsync(questionId, answerId, userId, dto);
            if (responseData == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Respuesta no encontrada", "ANSWER_NOT_FOUND"));
            }

            return Ok(ApiResponse<AnswerResponseDTO>.SuccessResponse(responseData, "ANSWER_UPDATED", "Respuesta actualizada exitosamente"));
           
        }

        [Authorize]
        [HttpDelete("{answerId:int}")]
        public async Task<IActionResult> DeleteAnswer(int questionId, int answerId)
        {
            var stringUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (stringUserId == null || !int.TryParse(stringUserId, out var userId))
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse("Usuario no autenticado", "UNAUTHORIZED"));
            }

            try
            {
                var deleted = await _answerService.DeleteAnswerAsync(questionId, answerId, userId);
                if (!deleted)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse("Respuesta no encontrada", "ANSWER_NOT_FOUND"));
                }

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}
