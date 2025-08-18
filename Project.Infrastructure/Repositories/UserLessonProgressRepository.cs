using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class UserLessonProgressRepository : BaseRepository<UserLessonProgress>, IUserLessonProgressRepository {
        private readonly IUserModuleProgressRepository _userModuleRepo;
        private readonly IUserCourseProgressRepository _userCourseRepo;
        public UserLessonProgressRepository(ApplicationDbContext dbContext) : base(dbContext) {
            _userModuleRepo = new UserModuleProgressRepository(dbContext);
            _userCourseRepo = new UserCourseProgressRepository(dbContext);
        }

        public async Task<int> CountCompletedAsync(Guid courseId, Guid userId) {
            List<UserModuleProgress> userModuleProgresses = await _dbContext.UserModuleProgress
                .Include(x => x.Module)
                .Where(x =>
                    x.Module != null &&
                    x.Module.CourseId == courseId &&
                    x.UserId == userId
                )
                .ToListAsync();
            List<UserLessonProgress> userLessonProgresses = await _dbContext.UserLessonProgress
                .Include(x => x.Lesson)
                .Where(x =>
                    x.Lesson != null &&
                    userModuleProgresses.Select(m => m.ModuleId).Contains(x.Lesson.ModuleId) &&
                    x.UserId == userId
                )
                .ToListAsync();

            return userLessonProgresses
                .Where(x => x.CompletedAt != null)
                .Count();
        }

        public async Task<int> CountLast7DaysCompletedLessonsAsync(Guid userId) {
            var last7Days = DateTime.UtcNow.AddDays(-7);

            return await _dbContext.UserLessonProgress
                .Where(x =>
                    x.CompletedAt != null &&
                    x.UserId == userId &&
                    x.CompletedAt >= last7Days
                )
                .CountAsync();
        }

        public async Task<UserLessonProgress?> GetActiveUserLessonProgressByUserIdAsync(Guid userId) {
            return await _dbContext.UserLessonProgress
                .Where(x => x.UserId == userId && x.IsActive)
                .Include(x => x.Lesson)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<Lesson?> GetActiveLessonByUserIdAsync(Guid userId) {
            var result = await _dbContext.Lessons
                .Include(l => l.Module)
                .Include(l => l.UserLessonProgress)
                .Where(l => l.UserLessonProgress.Any(ulp => ulp.UserId == userId && ulp.IsActive))
                .AsNoTracking()
                .FirstOrDefaultAsync();
            return result;
        }
        public async Task<List<Lesson>> GetLearnedLessons(Guid userId) {
            var result = await _dbContext.Lessons
                .Where(l => l.UserLessonProgress.Any(ulp => ulp.IsLearned && ulp.UserId == userId))
                .Select(l => new Lesson {
                    Id = l.Id,
                    Title = l.Title,
                    Content = l.Content,
                    VideoUrl = l.VideoUrl,
                    DurationMinutes = l.DurationMinutes,
                    OrderNumber = l.OrderNumber,
                    CreatedAt = l.CreatedAt,
                    ModuleId = l.ModuleId,
                    UserLessonProgress = l.UserLessonProgress
                        .Where(ulp => ulp.UserId == userId)
                        .ToList()
                })
                .AsNoTracking()
                .ToListAsync();

            return result;
        }

        public async Task<List<string>> GetTitleLearnedLessons(Guid userId) {
            var titles = await _dbContext.Lessons
                .Where(l => l.UserLessonProgress.Any(ulp => ulp.IsLearned && ulp.UserId == userId))
                .Select(l => l.Title)
                .AsNoTracking()
                .ToListAsync();

            return titles;
        }


        public async Task<bool> CreateNewLessonProgress(Guid userId, Guid lessonId) {
            var user = _dbContext.Profiles.Where(x => x.Id == userId).AsNoTracking()
                .FirstOrDefault();
            var lesson = _dbContext.Lessons.Where(x => x.Id == lessonId).AsNoTracking()
                .FirstOrDefault();
            //get moduleId from lesson
            var moduleId = await _dbContext.Lessons
                .Where(x => x.Id == lessonId)
                .Select(x => x.ModuleId)
                .FirstOrDefaultAsync();
            //get courseId from module
            var courseId = await _dbContext.Modules
                .Where(m => m.Id == moduleId)
                .Select(m => m.CourseId)
                .FirstOrDefaultAsync();

            if (user is null || lesson is null) {
                throw new Exception(user is null ? "User not found" : "Lesson not found");
            }

            //create new progress if not exists
            var userLessonProgress = await _dbContext.UserLessonProgress
                .Where(x => x.UserId == userId && x.LessonId == lessonId).AsNoTracking()
                .FirstOrDefaultAsync();
            var moduleProgress = await _dbContext.UserModuleProgress
                .Where(x => x.UserId == userId && x.ModuleId == moduleId).AsNoTracking()
                .FirstOrDefaultAsync();
            var courseProgress = await _dbContext.UserCourseProgress
                .Where(x => x.UserId == userId && x.CourseId == courseId).AsNoTracking()
                .FirstOrDefaultAsync();
            bool createStatus = true;

            //if lesson progess is null, create new progress
            if (userLessonProgress == null) {
                userLessonProgress = new UserLessonProgress() {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    LessonId = lessonId,
                    IsActive = true,
                    IsLearned = false,
                    CompletedAt = null
                };
                try {
                    await Create(userLessonProgress);
                    createStatus = true;
                }
                catch {
                    createStatus = false;
                }

                if (userLessonProgress != null) {
                    await ActivateCurrentLessonAsync(userLessonProgress);
                }

                #region create lesson progress for all lessons in module
                //create lesson progress for all lessons in module, check if exists
                var lessonsInModule = await _dbContext.Lessons
                    .Where(x => x.ModuleId == moduleId)
                    .Select(x => x.Id)
                    .ToListAsync();
                //check if user already has progress for these lessons
                var existingProgresses = await _dbContext.UserLessonProgress
                    .Where(x => x.UserId == userId && lessonsInModule.Contains(x.LessonId))
                    .Select(x => x.LessonId)
                    .ToListAsync();
                //create new progresses for lessons that do not exist
                var newProgresses = lessonsInModule
                    .Where(x => !existingProgresses.Contains(x))
                    .Select(x => new UserLessonProgress {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        LessonId = x,
                        IsActive = false,
                        IsLearned = false,
                        CompletedAt = null
                    }).ToList();
                //if new progresses exist, add to db
                if (newProgresses.Any()) {
                    try {
                        await _dbContext.UserLessonProgress.AddRangeAsync(newProgresses);
                        await SaveChangeAsync();
                        createStatus = true;
                    }
                    catch {
                        createStatus = false;
                    }
                }
                #endregion
            }
            else {
                await ActivateCurrentLessonAsync(userLessonProgress);
            }
            //if module progress is null, create new progress
            if (moduleProgress == null) {
                moduleProgress = new UserModuleProgress() {
                    Id = Guid.NewGuid(),
                    ModuleId = moduleId,
                    UserId = userId,
                    IsActive = true,
                    CompletedAt = null
                };

                try {
                    await _userModuleRepo.Create(moduleProgress);
                    createStatus = true;
                }
                catch {
                    createStatus = false;
                }
            }
            //if course progress is null, create new progress
            if (courseProgress == null) {
                courseProgress = new UserCourseProgress() {
                    Id = Guid.NewGuid(),
                    CourseId = courseId,
                    UserId = userId,
                    IsActive = true,
                    CompletedAt = null
                };
                try {
                    await _userCourseRepo.Create(courseProgress);
                    createStatus = true;
                }
                catch {
                    createStatus = false;
                }
            }
            return createStatus;
        }

        public async Task<bool> LearnedLessons(Guid userId, Guid lessonId) {
            var progress = await _dbContext.UserLessonProgress
                .Include(x => x.Lesson)
                .Where(x => x.UserId == userId && x.LessonId == lessonId)
                .FirstOrDefaultAsync();

            if (progress is null) {
                throw new Exception("You have not learned this lessoon");
            }

            if (progress.IsLearned) {
                return true;
            }

            progress.IsLearned = true;
            progress.CompletedAt = null;

            try {
                await Update(progress);
                return true;
            }
            catch (Exception) {
                return false;
            }
        }

        public async Task<bool> CompleteLessons(Guid userId, Guid lessonId) {
            var progress = await _dbContext.UserLessonProgress
                .Include(x => x.Lesson)
                .Where(x => x.UserId == userId && x.LessonId == lessonId && x.IsLearned)
                .FirstOrDefaultAsync();

            if (progress is null) {
                throw new Exception("You have not learned this lessoon");
            }
            if (progress.CompletedAt != null) {
                return true;
            }
            progress.CompletedAt = DateTime.UtcNow;

            try {
                await Update(progress);

                await HandleModuleAndCourseCompletionAsync(progress);

                await ActivateNextLessonAsync(progress);

                return true;
            }
            catch (Exception) {
                return false;
            }

        }

        private async Task HandleModuleAndCourseCompletionAsync(UserLessonProgress progress) {
            var moduleId = progress.Lesson!.ModuleId;
            var courseId = await _dbContext.Modules
                .Where(m => m.Id == moduleId)
                .Select(m => m.CourseId)
                .FirstAsync();

            if (await _userModuleRepo.CheckIfAllLessonsCompleted(moduleId, progress.UserId))
                await _userModuleRepo.MarkModuleCompleted(progress.UserId, moduleId);

            if (await _userCourseRepo.CheckIfAllModulesCompleted(courseId, progress.UserId))
                await _userCourseRepo.MarkCourseCompleted(progress.UserId, courseId);
        }

        private async Task ActivateCurrentLessonAsync(UserLessonProgress progress) {
            var userId = progress.UserId;
            var lessonId = progress.LessonId;

            var actives = await _dbContext.UserLessonProgress
                .Where(x => x.UserId == userId && x.IsActive)
                .ToListAsync();

            actives.ForEach(x => {
                x.IsActive = false;
            });
            _dbContext.UserLessonProgress.UpdateRange(actives);
            await SaveChangeAsync();

            progress.IsActive = true;
            await Update(progress);

            var moduleId = await _dbContext.Lessons
                .Where(x => x.Id == lessonId)
                .Select(x => x.ModuleId)
                .FirstOrDefaultAsync();

            var moduleProgress = await _dbContext.UserModuleProgress
                .Include(x => x.Module)
                .Where(x => x.UserId == userId && x.Module != null && x.Module.CourseId == moduleId)
                .ToListAsync();

            moduleProgress.ForEach(x => {
                x.IsActive = false;
            });

            _dbContext.UserModuleProgress.UpdateRange(moduleProgress);
            await SaveChangeAsync();
            var moduleProgressNew = await _dbContext.UserModuleProgress
               .Where(x => x.UserId == userId && x.ModuleId == moduleId)
               .FirstOrDefaultAsync();
            if (moduleProgressNew == null) {
                moduleProgressNew = new UserModuleProgress {
                    Id = Guid.NewGuid(),
                    ModuleId = moduleId,
                    UserId = userId,
                    IsActive = true,
                    CompletedAt = null
                };
                await _userModuleRepo.Create(moduleProgressNew);
            }
            else {
                moduleProgressNew.IsActive = true;
                moduleProgressNew.CompletedAt = null;
                await _userModuleRepo.Update(moduleProgressNew);
            }
        }

        private async Task ActivateNextLessonAsync(UserLessonProgress completedProgress) {
            var userId = completedProgress.UserId;

            //find activating lesson -> unactive
            var actives = await _dbContext.UserLessonProgress
                .Where(x => x.UserId == userId && x.IsActive)
                .ToListAsync();

            actives.ForEach(x => {
                x.IsActive = false;
            });
            _dbContext.UserLessonProgress.UpdateRange(actives);
            await SaveChangeAsync();

            //find next lesson -> active

            var next = await _dbContext.UserLessonProgress
                .Include(p => p.Lesson)
                .Where(p =>
                    p.UserId == userId &&
                    p.CompletedAt == null && p.Lesson!.ModuleId == completedProgress.Lesson!.ModuleId)
                .OrderBy(p => p.Lesson!.OrderNumber)
                .FirstOrDefaultAsync();

            if (next != null) {
                next.IsActive = true;
                next.CompletedAt = null;

                await Update(next);

            }
            else {
                await ActivateFirstLessonOfNextModuleAsync(completedProgress);
            }
            await SaveChangeAsync();
        }

        private async Task ActivateFirstLessonOfNextModuleAsync(UserLessonProgress completedProgress) {
            var userId = completedProgress.UserId;
            var courseId = await _dbContext.Modules
                .Where(m => m.Id == completedProgress.Lesson!.ModuleId)
                .Select(m => m.CourseId)
                .FirstOrDefaultAsync();

            var nextModule = await _dbContext.Modules
                .Where(m => m.CourseId == courseId && m.Id != completedProgress.Lesson!.ModuleId)
                .OrderBy(m => m.OrderNumber)
                .FirstOrDefaultAsync();


            if (nextModule == null) return;
            var moduleProgress = await _dbContext.UserModuleProgress
                .Include(x => x.Module)
                .Where(x => x.UserId == userId && x.Module != null && x.Module.CourseId == nextModule.CourseId)
                .ToListAsync();
            var moduleProgressNew = await _dbContext.UserModuleProgress
                .Where(x => x.UserId == userId && x.ModuleId == nextModule.Id)
                .FirstOrDefaultAsync();
            if (moduleProgressNew == null) {
                moduleProgressNew = new UserModuleProgress {
                    Id = Guid.NewGuid(),
                    ModuleId = nextModule.Id,
                    UserId = userId,
                    IsActive = true,
                    CompletedAt = null
                };
            }
            else {
                moduleProgressNew.IsActive = true;
                moduleProgressNew.CompletedAt = null;
                await _userModuleRepo.Update(moduleProgressNew);
            }

            if (moduleProgress != null) {
                moduleProgress.ForEach(x => {
                    x.IsActive = false;
                });
                _dbContext.UserModuleProgress.UpdateRange(moduleProgress);
                await SaveChangeAsync();
            }

            var nextLesson = await _dbContext.Lessons
            .Where(l => l.ModuleId == nextModule.Id)
            .OrderBy(l => l.OrderNumber)
            .FirstOrDefaultAsync();
            if (nextLesson == null) return;

            var nextProgress = await _dbContext.UserLessonProgress
                .Where(x => x.UserId == userId && x.LessonId == nextLesson.Id).FirstOrDefaultAsync();
            if (nextProgress == null) {
                await Create(new UserLessonProgress {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    LessonId = nextLesson.Id,
                    IsActive = true,
                    IsLearned = false
                });
            }
            else {
                nextProgress.IsActive = true;
                await Update(nextProgress);
            }
        }
    }
}
