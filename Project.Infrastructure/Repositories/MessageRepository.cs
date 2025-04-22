using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.Business.DTOs.MessageDTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class MessageRepository : BaseRepository<UserMessage>, IMessageRepository {


        public MessageRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }

        public async Task<MessageCreateResponse> CreateMessage(MessageCreateRequest request) {
            var receiver = await _dbContext.Profiles.FirstOrDefaultAsync(x => x.Email == request.ReceiverEmail);
            var sender = await _dbContext.Profiles.FirstOrDefaultAsync(x => x.Email == request.SenderEmail);

            UserMessage newMessage = new UserMessage() {
                DateTime = request.DateTime,
                Message = request.Content,
                Receiver = receiver ?? new Profile(),
                Sender = sender ?? new Profile(),
                ReceiverId = receiver is null ? Guid.NewGuid() : receiver.Id,
                SenderId = sender is null ? Guid.NewGuid() : sender.Id
            };

            _dbContext.UserMessages.Add(newMessage);
            await _dbContext.SaveChangesAsync();

            return new MessageCreateResponse() {
                Message = newMessage.Message,
                MessageId = newMessage.MessageId,
                Sender = newMessage.Sender is null ? "" : newMessage.Sender.Username is null ? "" : newMessage.Sender.Username
            };
        }

        public async Task<MessageResponse> GetMessage(MessageRequest request) {
            var otherUser = await _dbContext.Profiles.AsNoTracking().FirstOrDefaultAsync(x => x.Email == request.ReceiverEmail);
            var otherUserId = otherUser is null ? Guid.NewGuid() : otherUser.Id;

            var userMessages = _dbContext.UserMessages
                .Where(x => (x.ReceiverId == request.UserId && x.SenderId == otherUserId)
                || (x.ReceiverId == otherUserId && x.SenderId == request.UserId)).AsNoTracking();

            var data = await userMessages
                .OrderByDescending(x => x.MessageId)
                .Skip((request.PageNumber - 1) * 20)
                .Take(20)
                .AsNoTracking()
                .OrderBy(x => x.MessageId)
                .Select(x => new MessageContent() { Id = x.MessageId, Content = x.Message, IsSender = request.UserId == x.SenderId })
                .ToListAsync();

            var messageQuantity = userMessages.Count() - (request.PageNumber * 20);
            var result = new MessageResponse() {
                IsLoadFullPage = messageQuantity <= 0,
                Data = data
            };

            return result;
        }

        public async Task<List<MessageUserResponse>> GetMessageUser(Guid userId) {
            var usersMessage = await _dbContext.UserMessages
                .AsNoTracking()
                .Where(x => x.SenderId == userId || x.ReceiverId == userId)
                .GroupBy(x => x.SenderId == userId ? x.ReceiverId : x.SenderId)
                .Select(g => new {
                    OtherUserId = g.Key,
                    LatestMessageId = g.Max(m => m.MessageId)
                })
                .OrderByDescending(x => x.LatestMessageId)
                .Take(20).ToListAsync();

            List<MessageUserResponse> response = new List<MessageUserResponse>();

            foreach (var user in usersMessage) {
                var otherUser = await _dbContext.Profiles.FirstOrDefaultAsync(x => x.Id == user.OtherUserId);
                var lastMessage = await _dbContext.UserMessages.FirstOrDefaultAsync(x => x.MessageId == user.LatestMessageId);

                response.Add(new MessageUserResponse() {
                    Avatar = otherUser is null ? String.Empty : otherUser.AvatarUrl ?? String.Empty,
                    LastMessage = lastMessage is null ? String.Empty : lastMessage.Message ?? String.Empty,
                    IsSender = (otherUser is null ? Guid.NewGuid() : otherUser.Id) == (lastMessage is null ? Guid.NewGuid() : lastMessage.ReceiverId),
                    LastSent = GetTimeAgo(lastMessage is null ? DateTime.MinValue : lastMessage.DateTime),
                    UserName = (otherUser is null ? String.Empty : otherUser.Username ?? ""),
                    UserEmail = (otherUser is null ? String.Empty : otherUser.Email ?? "")
                });
            }
            return response;
        }

        private string GetTimeAgo(DateTime messageTime) {
            var timeSpan = DateTime.Now - messageTime;

            if (timeSpan.TotalSeconds < 60)
                return $"{(int)timeSpan.TotalSeconds} seconds";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} minutes";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hours";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} days";
            if (timeSpan.TotalDays < 30)
                return $"{(int)(timeSpan.TotalDays / 7)} weeks";
            if (timeSpan.TotalDays < 365)
                return $"{(int)(timeSpan.TotalDays / 30)} months";

            return $"{(int)(timeSpan.TotalDays / 365)} years";
        }
    }
}
