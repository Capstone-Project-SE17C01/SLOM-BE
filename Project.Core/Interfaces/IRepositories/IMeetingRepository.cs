using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories {
    public interface IMeetingRepository : IBaseRepository<Meeting> {
        Task<Meeting> CreateMeetingAsync(Meeting meeting);
        Task<Meeting?> GetMeetingByIdAsync(Guid id);
        Task<IEnumerable<Meeting>> GetActiveMeetingsAsync();
        Task<IEnumerable<Meeting>> GetActiveMeetingsAsync(Guid userId);
        Task<IEnumerable<Meeting>> GetScheduledMeetingsByMonthAsync(int year, int month);
        Task<IEnumerable<Meeting>> GetScheduledMeetingsByMonthAsync(int year, int month, Guid userId);
        Task<IEnumerable<Meeting>> GetScheduledMeetingsByDateAsync(DateTime date);
        Task<IEnumerable<Meeting>> GetScheduledMeetingsByDateAsync(DateTime date, Guid userId);
        Task<Meeting> UpdateMeetingAsync(Meeting meeting);
        Task<bool> DeleteMeetingAsync(Guid id);
        Task<bool> AddParticipantAsync(Guid meetingId, Guid userId, string deviceInfo);
        Task<bool> RemoveParticipantAsync(Guid meetingId, Guid userId);
        Task<MeetingRecording> AddRecordingAsync(MeetingRecording recording);
        Task<IEnumerable<MeetingRecording>> GetRecordingsByUserIdAsync(Guid userId);
        Task<MeetingInvitation> AddMeetingInvitationAsync(MeetingInvitation invitation);
        Task<int> CountActiveMeetingsAsync();
        Task<int> CountScheduleMeetingAsync();
        Task<int> CountRecordMeetingAsync();
    }
}
