using Project.Core.Entities.General;
using Project.Tests.TestData.SeedData;

namespace Project.Tests.Helpers {
    public static class TestDataFixture {

        public static List<Course> Courses => CourseSeed.GetCourses();
        public static Course SingleCourse => CourseSeed.GetSingleCourse();

        public static List<Lesson> Lessons => LessonSeed.GetLessons();
        public static Lesson SingleLesson => LessonSeed.GetSingleLesson();

        public static List<Payment> Payments => PaymentSeed.GetPayments();
        public static Payment SinglePayment => PaymentSeed.GetSinglePayment();

        public static List<Meeting> Meetings => MeetingSeed.GetMeetings();
        public static Meeting SingleMeeting => MeetingSeed.GetSingleMeeting();

        public static List<Profile> Profiles => ProfileSeed.GetProfiles();
        public static Profile SingleProfile => ProfileSeed.GetSingleProfile();
    }
}
