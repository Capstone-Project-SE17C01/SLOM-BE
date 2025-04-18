using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
            var receiver = _dbContext.Profiles.Where(x => x.Username == request.ReceiverUserName).FirstOrDefault();
            var sender = _dbContext.Profiles.Where(x => x.Username == request.SenderUserName).FirstOrDefault();

            UserMessage newMessage = new UserMessage() {
                DateTime = request.DateTime,
                Message = request.Content,
                Receiver = receiver,
                Sender = sender,
                ReceiverId = receiver.Id,
                SenderId = sender.Id
            };

            _dbContext.UserMessages.Add(newMessage);
            await _dbContext.SaveChangesAsync();

            return new MessageCreateResponse() {
                Message = newMessage.Message,
                MessageId = newMessage.MessageId,
                Sender = newMessage.Sender is null ? "" : newMessage.Sender.Username
            };
        }

        public async Task<MessageResponse> GetMessage(MessageRequest request) {
            var otherUser = await _dbContext.Profiles.Where(x => x.Username == request.ReceiverName).FirstOrDefaultAsync();
            var otherUserId = otherUser.Id;

            var userMessages = _dbContext.UserMessages
                .Where(x => (x.ReceiverId == request.UserId && x.SenderId == otherUserId)
                    || (x.ReceiverId == otherUserId && x.SenderId == request.UserId));

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
                var otherUser = _dbContext.Profiles.FirstOrDefault(x => x.Id == user.OtherUserId);
                var lastMessage = _dbContext.UserMessages.FirstOrDefault(x => x.MessageId == user.LatestMessageId);

                response.Add(new MessageUserResponse() {
                    Avatar = otherUser.AvatarUrl,
                    LastMessage = lastMessage.Message,
                    IsSender = otherUser.Id == lastMessage.ReceiverId,
                    LastSent = GetTimeAgo(lastMessage.DateTime),
                    UserName = otherUser.Username,
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
