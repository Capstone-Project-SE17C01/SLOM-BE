using Project.Core.Entities.Business.DTOs.AnswerDTOs;
using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories {
    public interface IAnswerRepository : IBaseRepository<Answer> {
        public Task<List<AnswerResponse>> GetListAnswerByQuestion(Guid questionId, int page);
        public Task<AnswerResponse> CreateAnswer(PostAnswerRequest request);
        public Task<bool> DeleteAnswer(Guid answerId);
    }
}
