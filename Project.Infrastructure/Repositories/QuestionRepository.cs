using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.Business.DTOs.QuestionDTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class QuestionRepository : BaseRepository<Question>, IQuestionRepository {
        public QuestionRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }

        public async Task<List<QuestionResponse>> GetQuestionReponsePaginated(int pageNumber, Guid userId, bool isCurrentUser, bool isAdmin) {
            var questions = _dbContext.Questions;
            var questionAmount = questions.Count();
            IQueryable<Question> questionQuery;

            if (isAdmin) {
                questionQuery = questions
                    .Where(x => (isCurrentUser ? x.Answers.FirstOrDefault(x => x.CreatorId == userId) == null : true));
            } else {
                questionQuery = questions
                    .Where(x => (isCurrentUser ? x.CreatorId == userId : x.Privacy != "Only admin can view and answer" || x.CreatorId == userId));
            }

            var question = await questionQuery
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * 10)
                .Take(10)
                .AsNoTracking()
                .Select(x => new QuestionResponse {
                    QuestionId = x.Id,
                    Author = new Author {
                        ProfileImage = x.Creator.AvatarUrl ?? "",
                        Username = x.Creator.Username ?? ""
                    },
                    AnswerAmount = x.Answers.Count,
                    Content = x.Content,
                    CreatedAt = x.CreatedAt,
                    Images = DeserializeStringToList(x.Images),
                    IsFull = questionAmount <= pageNumber * 10,
                    Privacy = x.Privacy
                })
                .ToListAsync();

            return question;
        }

        public async Task<QuestionResponse> CreateQuestion(PostQuestionRequest request) {
            Profile? creator = await _dbContext.Profiles.FirstOrDefaultAsync(x => x.Id == request.CreatorId) ?? null;

            if (creator is null) {
                throw new Exception("User not found");
            }

            var newQuestion = new Question {
                Id = Guid.NewGuid(),
                CreatorId = request.CreatorId,
                Content = request.Content,
                Images = SerializeListToString(request.Images ?? new List<string>()),
                Privacy = request.Privacy,
                Creator = creator
            };

            var questionCreated = Create(newQuestion);

            return new QuestionResponse {
                Author = new Author {
                    ProfileImage = creator.AvatarUrl ?? "",
                    Username = creator.Username ?? "",
                },
                AnswerAmount = 0,
                Content = newQuestion.Content,
                CreatedAt = newQuestion.CreatedAt,
                Images = DeserializeStringToList(newQuestion.Images),
                QuestionId = newQuestion.Id,
                Privacy = newQuestion.Privacy
            };
        }

        private string SerializeListToString(List<string> list) {
            if (list == null || list.Count == 0)
                return string.Empty;

            return string.Join(",", list);
        }

        private static List<string> DeserializeStringToList(string str) {
            if (string.IsNullOrEmpty(str))
                return new List<string>();

            return str.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public async Task<QuestionResponse> UpdateQuestion(UpdateQuestionRequest request) {
            var question = await _dbContext.Questions.FirstOrDefaultAsync(x => x.Id == request.QuestionId);
            if (question == null) {
                return new QuestionResponse();
            }

            question.Content = request.Content;
            question.Images = SerializeListToString(request.Images ?? new List<string>());
            question.Privacy = request.Privacy;

            _dbContext.Questions.Update(question);
            await _dbContext.SaveChangesAsync();

            return new QuestionResponse {
                QuestionId = question.Id,
                Author = new Author {
                    ProfileImage = question.Creator.AvatarUrl ?? "",
                    Username = question.Creator.Username ?? ""
                },
                AnswerAmount = question.Answers.Count,
                Content = question.Content,
                CreatedAt = question.CreatedAt,
                Images = DeserializeStringToList(question.Images),
                Privacy = question.Privacy
            };
        }

        public async Task<bool> DeleteQuestion(Guid questionId) {
            var question = await _dbContext.Questions
                .Include(x => x.Answers)
                .FirstOrDefaultAsync(x => x.Id == questionId);

            if (question == null) {
                return false;
            }

            // Remove all related answers first
            if (question.Answers.Any()) {
                _dbContext.Answers.RemoveRange(question.Answers);
            }

            // Remove the question
            _dbContext.Questions.Remove(question);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
