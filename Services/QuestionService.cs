using AcaHelpAPI.Data;
using AcaHelpAPI.DTOs;
using AcaHelpAPI.Models;

namespace AcaHelpAPI.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly MiDbContext _context;

        public QuestionService(MiDbContext context)
        {
            _context = context;
        }

        public async Task<Question> CreateQuestion(int userId, CreateQuestionDTO data)
        {
            try
            {
                var question = new Question
                {
                    Body = data.body,
                    IsSolved = false,
                    Title = data.title,
                    TagId = data.tagId,
                    UserId = userId
                };

                _context.Questions.Add(question);
                await _context.SaveChangesAsync();
            }
            catch
            {

            }
            throw new NotImplementedException();
        }

        Task<Question> IQuestionService.GetQuestionByIdAsync(int questionId)
        {
            throw new NotImplementedException();
        }
    }
}
