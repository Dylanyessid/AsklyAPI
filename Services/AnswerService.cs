using AcaHelpAPI.Data;
using AcaHelpAPI.DTOs;
using AcaHelpAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace AcaHelpAPI.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly MiDbContext _context;

        public AnswerService(MiDbContext context)
        {
            _context = context;
        }

        public async Task<AnswerResponseDTO?> GetAnswerByIdAsync(int questionId, int answerId)
        {
            var answer = await _context.Answers
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == answerId && a.QuestionId == questionId && a.DeletedAt == null);
            if (answer == null)
            {
                return null;
            }
            return new AnswerResponseDTO
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
        }

        public async Task<AnswerListPageDTO> GetAnswersByQuestionAsync(int questionId, int pageSize, string? cursor)
        {
            var safePageSize = pageSize <= 0 ? 20 : Math.Min(pageSize, 100);

            var query = _context.Answers
                .AsNoTracking()
                .Where(answer => answer.QuestionId == questionId && answer.DeletedAt == null && !answer.IsAccepted);

            query = query
                .OrderBy(answer => answer.CreatedAt)
                .ThenBy(answer => answer.Id)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(cursor))
            {
                var cursorValues = DecodeCursor(cursor);
                query = query.Where(answer =>
                    answer.CreatedAt > cursorValues.CreatedAt ||
                    (answer.CreatedAt == cursorValues.CreatedAt && answer.Id > cursorValues.Id))
                    .OrderBy(answer => answer.CreatedAt)
                    .ThenBy(answer => answer.Id);
            }

            var results = await query
                .Take(safePageSize + 1)
                .ToListAsync();

            var hasMore = results.Count > safePageSize;
            var pageItems = results.Take(safePageSize)
                .Select(answer => new AnswerListItemDTO
                {
                    Id = answer.Id,
                    QuestionId = answer.QuestionId,
                    UserId = answer.UserId,
                    Body = answer.Body,
                    IsAccepted = answer.IsAccepted,
                    VoteCount = answer.VoteCount,
                    CreatedAt = answer.CreatedAt,
                    UpdatedAt = answer.UpdatedAt
                })
                .ToList();

            var nextCursor = hasMore
                ? EncodeCursor(results[safePageSize - 1].CreatedAt, results[safePageSize - 1].Id)
                : null;

            return new AnswerListPageDTO
            {
                Items = pageItems,
                HasMore = hasMore,
                NextCursor = nextCursor
            };
        }

        public async Task<AnswerListItemDTO?> GetAcceptedAnswerByQuestionAsync(int questionId)
        {
            return await _context.Answers
                .AsNoTracking()
                .Where(answer => answer.QuestionId == questionId && answer.IsAccepted && answer.DeletedAt == null)
                .OrderByDescending(answer => answer.UpdatedAt)
                .ThenByDescending(answer => answer.CreatedAt)
                .ThenByDescending(answer => answer.Id)
                .Select(answer => new AnswerListItemDTO
                {
                    Id = answer.Id,
                    QuestionId = answer.QuestionId,
                    UserId = answer.UserId,
                    Body = answer.Body,
                    IsAccepted = answer.IsAccepted,
                    VoteCount = answer.VoteCount,
                    CreatedAt = answer.CreatedAt,
                    UpdatedAt = answer.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<AnswerResponseDTO> CreateAnswerAsync(int questionId, int userId, CreateAnswerDTO dto)
        {
            var answer = new Answer
            {
                QuestionId = questionId,
                UserId = userId,
                Body = dto.Body,
                IsAccepted = false,
                VoteCount = 0
            };

            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();

            return MapToResponse(answer);
        }

        public async Task<AnswerResponseDTO?> UpdateAnswerAsync(int questionId, int answerId, int userId, UpdateAnswerDTO dto)
        {
            var answer = await _context.Answers
                .FirstOrDefaultAsync(a => a.Id == answerId && a.QuestionId == questionId);

            if (answer == null)
            {
                return null;
            }

            if (answer.UserId != userId)
            {
                throw new UnauthorizedAccessException("No tienes permiso para editar esta respuesta.");
            }

            answer.Body = dto.Body;
            await _context.SaveChangesAsync();

            return MapToResponse(answer);
        }

        public async Task<bool> DeleteAnswerAsync(int questionId, int answerId, int userId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var answer = await _context.Answers
                .FirstOrDefaultAsync(a => a.Id == answerId && a.QuestionId == questionId);

            if (answer == null)
            {
                return false;
            }

            if (answer.UserId != userId)
            {
                throw new UnauthorizedAccessException("No tienes permiso para eliminar esta respuesta.");
            }

            var answerVotes = await _context.AnswerVotes
                .Where(vote => vote.AnswerId == answerId)
                .ToListAsync();

            if (answerVotes.Count > 0)
            {
                _context.AnswerVotes.RemoveRange(answerVotes);
            }

            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }

        private static AnswerResponseDTO MapToResponse(Answer answer)
        {
            return new AnswerResponseDTO
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
        }

        private static string EncodeCursor(DateTime createdAt, int id)
        {
            var payload = $"{createdAt:O}|{id}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));
        }

        private static (DateTime CreatedAt, int Id) DecodeCursor(string cursor)
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
            var parts = decoded.Split('|', 2);
            if (parts.Length != 2 || !DateTime.TryParse(parts[0], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var createdAt) || !int.TryParse(parts[1], out var id))
            {
                throw new ArgumentException("Cursor inválido.");
            }

            return (createdAt, id);
        }
    }
}
