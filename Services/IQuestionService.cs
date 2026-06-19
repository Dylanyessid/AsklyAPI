using AcaHelpAPI.DTOs;
using AcaHelpAPI.Models;

namespace AcaHelpAPI.Services
{
    public interface IQuestionService
    {
        Task<Question> GetQuestionByIdAsync(int questionId);
        Task<Question> CreateQuestion(int userId, CreateQuestionDTO data);
    }
}
