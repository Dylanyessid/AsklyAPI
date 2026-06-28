using AcaHelpAPI.DTOs;

namespace AcaHelpAPI.Services
{
    public interface IAnswerService
    {
        Task<AnswerListPageDTO> GetAnswersByQuestionAsync(int questionId, int pageSize, string? cursor);
    }
}
