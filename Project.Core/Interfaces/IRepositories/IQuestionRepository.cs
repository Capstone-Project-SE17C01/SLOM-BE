using Project.Core.Entities.Business.DTOs.QuestionDTOs;
using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories {
    public interface IQuestionRepository : IBaseRepository<Question> {
        public Task<List<QuestionResponse>> GetQuestionReponsePaginated(int pageNumber, Guid userId, bool isCurrentUser, bool isAdmin);
        public Task<QuestionResponse> CreateQuestion(PostQuestionRequest request);
        public Task<QuestionResponse> UpdateQuestion(UpdateQuestionRequest request);
        public Task<bool> DeleteQuestion(Guid questionId);
        public Task<List<string>> GetTags();
        public Task<List<QuestionResponse>> GetQuestionsByTag(string[] tags, int pageNumber, Guid userId, bool isCurrentUser, bool isAdmin);
    }
}
