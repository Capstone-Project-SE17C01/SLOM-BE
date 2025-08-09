using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.Business.DTOs.AnswerDTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class AnswerRepository : BaseRepository<Answer>, IAnswerRepository {
        public AnswerRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }

        public async Task<List<AnswerResponse>> GetListAnswerByQuestion(Guid questionId, int page) {
            var answerQuery = _dbContext.Answers;
            var answerAmount = answerQuery.Count();

            var listAnswer = await _dbContext.Answers
                .OrderByDescending(x => x.CreatedAt)
                .Where(x => x.QuestionId == questionId)
                .Skip((page - 1) * 10)
                .Take(10)
                .AsNoTracking()
                .Select(x => new AnswerResponse {
                    AnswerId = x.Id,
                    Author = new Author {
                        ProfileImage = x.Creator.AvatarUrl ?? "",
                        Username = x.Creator.Username ?? ""
                    },
                    CreatedAt = x.CreatedAt,
                    Content = x.Content,
                    Images = DeserializeStringToList(x.Images),
                    IsFull = answerAmount <= page * 10
                })
                .ToListAsync();
            return listAnswer;
        }

        public async Task<AnswerResponse> CreateAnswer(PostAnswerRequest request) {
            Profile? creator = await _dbContext.Profiles.FirstOrDefaultAsync(x => x.Id == request.CreatorId) ?? null;
            Question? question = await _dbContext.Questions.FirstOrDefaultAsync(x => x.Id == request.QuestionId);

            if (creator is null) {
                throw new Exception("User not found");
            }
            else if (question is null) {
                throw new Exception("Question not found");
            }

            var newAnswer = new Answer {
                Id = Guid.NewGuid(),
                CreatorId = request.CreatorId,
                Content = request.Content,
                Images = SerializeListToString(request.Images ?? new List<string>()),
                Creator = creator,
                QuestionId = request.QuestionId,
                Question = question
            };

            var answerCreated = await Create(newAnswer);

            return new AnswerResponse {
                Author = new Author {
                    ProfileImage = creator.AvatarUrl ?? "",
                    Username = creator.Username ?? "",
                },
                Content = newAnswer.Content,
                CreatedAt = newAnswer.CreatedAt,
                Images = DeserializeStringToList(newAnswer.Images),
                AnswerId = newAnswer.Id,
                QuestionId = newAnswer.QuestionId
            };
        }

        public async Task<AnswerResponse> UpdateAnswer(UpdateAnswerRequest request) {
            var answer = await _dbContext.Answers
                .Include(x => x.Creator)
                .FirstOrDefaultAsync(x => x.Id == request.AnswerId);

            if (answer == null) {
                throw new Exception("Answer not found");
            }

            answer.Content = request.Content;
            answer.Images = SerializeListToString(request.Images ?? new List<string>());

            await Update(answer);

            return new AnswerResponse {
                AnswerId = answer.Id,
                Author = new Author {
                    ProfileImage = answer.Creator.AvatarUrl ?? "",
                    Username = answer.Creator.Username ?? ""
                },
                Content = answer.Content,
                CreatedAt = answer.CreatedAt,
                Images = DeserializeStringToList(answer.Images),
                QuestionId = answer.QuestionId
            };
        }

        public async Task<bool> DeleteAnswer(Guid answerId) {
            var answer = await _dbContext.Answers.FirstOrDefaultAsync(x => x.Id == answerId);

            if (answer == null) {
                return false;
            }

            await Delete(answer);
            return true;
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
    }
}
