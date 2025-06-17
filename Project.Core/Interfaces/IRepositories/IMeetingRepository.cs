using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories {
    public interface IMeetingRepository {
        Task<Meeting> CreateMeetingAsync(Meeting meeting);
        Task<Meeting> GetMeetingByIdAsync(Guid id);
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
        Task<IEnumerable<MeetingRecording>> GetRecordingsForMeetingAsync(Guid meetingId);
        Task<IEnumerable<MeetingRecording>> GetRecordingsByUserIdAsync(Guid userId);
        Task<IEnumerable<MeetingRecording>> GetAllRecordingsAsync();
        Task<IEnumerable<Meeting>> GetUserMeetingsAsync(Guid userId);

        // Meeting Invitation methods
        Task<MeetingInvitation> CreateInvitationAsync(MeetingInvitation invitation);
        Task<IEnumerable<MeetingInvitation>> GetInvitationsByMeetingIdAsync(Guid meetingId);
        Task<IEnumerable<MeetingInvitation>> GetInvitationsByUserIdAsync(Guid userId);
        Task<IEnumerable<MeetingInvitation>> GetInvitationsByEmailAsync(string email);
        Task<MeetingInvitation?> GetInvitationByCodeAsync(string invitationCode);
        Task<MeetingInvitation> UpdateInvitationAsync(MeetingInvitation invitation);
        Task<bool> DeleteInvitationAsync(Guid invitationId);
        Task<IEnumerable<Meeting>> GetMeetingsByInvitationAsync(Guid userId);
    }
}
