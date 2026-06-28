using AcaHelpAPI.Data;
using AcaHelpAPI.DTOs;
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

        public async Task<AnswerListPageDTO> GetAnswersByQuestionAsync(int questionId, int pageSize, string? cursor)
        {
            var safePageSize = pageSize <= 0 ? 20 : Math.Min(pageSize, 100);

            var query = _context.Answers
                .AsNoTracking()
                .Where(answer => answer.QuestionId == questionId)
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
