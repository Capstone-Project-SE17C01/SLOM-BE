using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.General;
using Project.Core.Exceptions;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class MeetingRepository : BaseRepository<Meeting>, IMeetingRepository {

        public MeetingRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }

        public async Task<Meeting> CreateMeetingAsync(Meeting meeting) {
            await _dbContext.Meetings.AddAsync(meeting);
            await _dbContext.SaveChangesAsync();
            return meeting;
        }

        public async Task<Meeting?> GetMeetingByIdAsync(Guid id) {
            var meeting = await _dbContext.Meetings
                .Include(m => m.Host)
                .Include(m => m.Participants)
                .ThenInclude(p => p.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            return meeting;
        }
        public async Task<IEnumerable<Meeting>> GetActiveMeetingsAsync() {
            var now = DateTime.UtcNow;
            return await _dbContext.Meetings
                .Include(m => m.Host)
                .Include(m => m.Participants)
                .Where(m => m.Status == "Active" && (m.EndTime == null || m.EndTime > now))
                .OrderByDescending(m => m.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<IEnumerable<Meeting>> GetActiveMeetingsAsync(Guid userId) {
            var now = DateTime.UtcNow;
            return await _dbContext.Meetings
                .Include(m => m.Host)
                .Include(m => m.Participants)
                .Where(m => m.Status == "Active" &&
                           (m.EndTime == null || m.EndTime > now) &&
                           (m.HostId == userId || m.Participants.Any(p => p.UserId == userId)))
                .OrderByDescending(m => m.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<IEnumerable<Meeting>> GetScheduledMeetingsByMonthAsync(int year, int month) {
            var startDate = DateTime.SpecifyKind(new DateTime(year, month, 1), DateTimeKind.Utc);
            var endDate = DateTime.SpecifyKind(startDate.AddMonths(1).AddDays(-1), DateTimeKind.Utc);

            return await _dbContext.Meetings
                .Include(m => m.Host)
                .Where(m => m.StartTime >= startDate && m.StartTime <= endDate)
                .OrderBy(m => m.StartTime)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<IEnumerable<Meeting>> GetScheduledMeetingsByMonthAsync(int year, int month, Guid userId) {
            var startDate = DateTime.SpecifyKind(new DateTime(year, month, 1), DateTimeKind.Utc);
            var endDate = DateTime.SpecifyKind(startDate.AddMonths(1).AddDays(-1), DateTimeKind.Utc);

            return await _dbContext.Meetings
                .Include(m => m.Host)
                .Include(m => m.Participants)
                .Where(m => m.StartTime >= startDate && m.StartTime <= endDate &&
                           (m.HostId == userId || m.Participants.Any(p => p.UserId == userId)))
                .OrderBy(m => m.StartTime)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<IEnumerable<Meeting>> GetScheduledMeetingsByDateAsync(DateTime date) {
            var startDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
            var endDate = DateTime.SpecifyKind(startDate.AddDays(1).AddSeconds(-1), DateTimeKind.Utc);

            return await _dbContext.Meetings
                .Include(m => m.Host)
                .Where(m => m.StartTime >= startDate && m.StartTime <= endDate)
                .OrderBy(m => m.StartTime)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<IEnumerable<Meeting>> GetScheduledMeetingsByDateAsync(DateTime date, Guid userId) {
            var startDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
            var endDate = DateTime.SpecifyKind(startDate.AddDays(1).AddSeconds(-1), DateTimeKind.Utc);

            return await _dbContext.Meetings
                .Include(m => m.Host)
                .Include(m => m.Participants)
                .Where(m => m.StartTime >= startDate && m.StartTime <= endDate &&
                      (m.HostId == userId || m.Participants.Any(p => p.UserId == userId)))
                .OrderBy(m => m.StartTime)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Meeting> UpdateMeetingAsync(Meeting meeting) {
            _dbContext.Meetings.Update(meeting);
            await _dbContext.SaveChangesAsync();
            return meeting;
        }

        public async Task<bool> DeleteMeetingAsync(Guid id) {
            var meeting = await _dbContext.Meetings.FindAsync(id);
            if (meeting == null)
                return false;

            meeting.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> AddParticipantAsync(Guid meetingId, Guid userId, string deviceInfo) {
            var participant = new MeetingParticipant {
                MeetingId = meetingId,
                UserId = userId,
                JoinTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                DeviceInfo = deviceInfo
            };

            await _dbContext.MeetingParticipants.AddAsync(participant);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveParticipantAsync(Guid meetingId, Guid userId) {
            var participant = await _dbContext.MeetingParticipants
                .FirstOrDefaultAsync(p => p.MeetingId == meetingId && p.UserId == userId && p.LeaveTime == null);

            if (participant == null)
                return false;

            participant.LeaveTime = DateTime.UtcNow;
            _dbContext.MeetingParticipants.Update(participant);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<MeetingRecording> AddRecordingAsync(MeetingRecording recording) {
            await _dbContext.MeetingRecordings.AddAsync(recording);
            await _dbContext.SaveChangesAsync();
            return recording;
        }

        public async Task<IEnumerable<MeetingRecording>> GetRecordingsByUserIdAsync(Guid userId) {
            return await _dbContext.MeetingRecordings
                .Include(r => r.Meeting)
                .Where(r => r.Meeting.HostId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<MeetingInvitation> AddMeetingInvitationAsync(MeetingInvitation invitation) {
            await _dbContext.MeetingInvitations.AddAsync(invitation);
            await _dbContext.SaveChangesAsync();
            return invitation;
        }

        public async Task<int> CountActiveMeetingsAsync() {
            return await _dbContext.Meetings.CountAsync(m => m.Status == "Active");
        }

        public async Task<int> CountScheduleMeetingAsync() {
            return await _dbContext.Meetings.CountAsync(m => m.Status == "Scheduled");
        }

        public async Task<int> CountRecordMeetingAsync() {
            return await _dbContext.MeetingRecordings.CountAsync();
        }

        public async Task<bool> DeleteMeetingRecordAsync(Guid id) {
            var recording = _dbContext.MeetingRecordings.Find(id) ?? throw new NotFoundException();
            _dbContext.MeetingRecordings.Remove(recording);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
