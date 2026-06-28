using AcaHelpAPI.Data;
using AcaHelpAPI.DTOs;
using AcaHelpAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AcaHelpAPI.Controllers
{
    [Route("api/answers/{answerId:int}/votes")]
    [ApiController]
    public class AnswerVotesController : ControllerBase
    {
        private readonly MiDbContext _context;

        public AnswerVotesController(MiDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostAnswerVote(int answerId, CreateAnswerVoteDTO dto)
        {
            var stringUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (stringUserId == null || !int.TryParse(stringUserId, out var userId))
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse("Usuario no autenticado", "UNAUTHORIZED"));
            }

            var answer = await _context.Answers.FindAsync(answerId);
            if (answer == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Respuesta no encontrada", "ANSWER_NOT_FOUND"));
            }

            var existingVote = await _context.AnswerVotes
                .FirstOrDefaultAsync(v => v.AnswerId == answerId && v.UserId == userId);

            if (existingVote != null)
            {
                return Conflict(ApiResponse<object>.ErrorResponse("El usuario ya votó esta respuesta", "ANSWER_VOTE_ALREADY_EXISTS"));
            }

            var vote = new AnswerVote
            {
                AnswerId = answerId,
                UserId = userId,
                VoteType = dto.VoteType,
                CreatedAt = DateTime.UtcNow
            };

            _context.AnswerVotes.Add(vote);
            answer.VoteCount += dto.VoteType;

            await _context.SaveChangesAsync();

            var responseData = new AnswerVoteResponseDTO
            {
                Id = vote.Id,
                AnswerId = vote.AnswerId,
                UserId = vote.UserId,
                VoteType = vote.VoteType,
                CreatedAt = vote.CreatedAt,
                UpdatedAt = vote.UpdatedAt
            };

            return Ok(ApiResponse<AnswerVoteResponseDTO>.SuccessResponse(responseData, "ANSWER_VOTE_CREATED", "Voto registrado exitosamente"));
        }
    }
}
