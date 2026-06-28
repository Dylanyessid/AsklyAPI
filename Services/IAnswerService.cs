using AcaHelpAPI.DTOs;

namespace AcaHelpAPI.Services
{
    public interface IAnswerService
    {
        Task<AnswerResponseDTO> GetAnswerByIdAsync(int questionId, int answerId);
        Task<AnswerListPageDTO> GetAnswersByQuestionAsync(int questionId, int pageSize, string? cursor);
        Task<AnswerListItemDTO?> GetAcceptedAnswerByQuestionAsync(int questionId);
        Task<AnswerResponseDTO> CreateAnswerAsync(int questionId, int userId, CreateAnswerDTO dto);
        Task<AnswerResponseDTO?> UpdateAnswerAsync(int questionId, int answerId, int userId, UpdateAnswerDTO dto);
        Task<bool> DeleteAnswerAsync(int questionId, int answerId, int userId);
    }
}
