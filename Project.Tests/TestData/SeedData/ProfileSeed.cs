using Project.Core.Entities.General;

namespace Project.Tests.TestData.SeedData
{
    public static class ProfileSeed
    {
        public static List<Profile> GetProfiles()
        {
            return new List<Profile>
            {
                new Profile
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Username = "user1",
                    Email = "user1@example.com",
                    RoleId = null,
                    AvatarUrl = null,
                    PreferredLanguageId = null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Profile
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Username = "user2",
                    Email = "user2@example.com",
                    RoleId = null,
                    AvatarUrl = null,
                    PreferredLanguageId = null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };
        }

        public static Profile GetSingleProfile()
        {
            return new Profile
            {
                Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                Username = "singleuser",
                Email = "singleuser@example.com",
                RoleId = null,
                AvatarUrl = null,
                PreferredLanguageId = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}
