using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.General;

namespace Project.Infrastructure.Data {
    public static class ApplicationDbContextConfigurations {
        public static void Configure(ModelBuilder modelBuilder) {

            modelBuilder.Entity<Language>(entity => {
                entity.HasKey(e => e.Id).HasName("languages_pkey");
                entity.ToTable("languages");
                entity.HasIndex(e => e.Code, "languages_code_key").IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever().HasColumnName("id");
                entity.Property(e => e.Code).HasMaxLength(5).HasColumnName("code");
                entity.Property(e => e.Name).HasMaxLength(50).HasColumnName("name");
                entity.Property(e => e.Region).HasMaxLength(50).HasColumnName("region");
            });

            modelBuilder.Entity<Role>(entity => {
                entity.HasKey(e => e.Id).HasName("roles_pkey");
                entity.ToTable("roles");
                entity.HasIndex(e => e.Name, "roles_name_key").IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever().HasColumnName("id");
                entity.Property(e => e.Name).HasMaxLength(50).HasColumnName("name");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.IsActive).HasDefaultValue(true).HasColumnName("is_active");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
            });

            modelBuilder.Entity<Profile>(entity => {
                entity.HasKey(e => e.Id).HasName("profiles_pkey");
                entity.ToTable("profiles");
                entity.HasIndex(e => e.Username, "profiles_username_key");

                entity.Property(e => e.Id).ValueGeneratedNever().HasColumnName("id");
                entity.Property(e => e.Username).HasMaxLength(50).HasColumnName("username");
                entity.Property(e => e.Email).HasMaxLength(255).HasColumnName("email");
                entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url");
                entity.Property(e => e.PreferredLanguageId).HasColumnName("preferred_language");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()").HasColumnName("updated_at");
                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Profiles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("profiles_role_id_fkey");

                entity.HasOne(d => d.PreferredLanguage)
                    .WithMany(p => p.Profiles)
                    .HasForeignKey(d => d.PreferredLanguageId)
                    .HasConstraintName("profiles_preferred_language_fkey");
            });

            modelBuilder.Entity<SubscriptionPlan>(entity => {
                entity.HasKey(e => e.Id).HasName("subscription_plans_pkey");
                entity.ToTable("subscription_plans");
                entity.HasIndex(e => e.Name, "subscription_plans_name_key").IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever().HasColumnName("id");
                entity.Property(e => e.Name).HasMaxLength(50).HasColumnName("name");
                entity.Property(e => e.Price).HasPrecision(10, 2).HasColumnName("price");
                entity.Property(e => e.DurationDays).HasColumnName("duration_days");
                entity.Property(e => e.Features).HasColumnType("jsonb").HasColumnName("features");
                entity.Property(e => e.IsActive).HasDefaultValue(true).HasColumnName("is_active");
            });

            modelBuilder.Entity<UserSubscription>(entity => {
                entity.HasKey(e => e.Id).HasName("user_subscriptions_pkey");
                entity.ToTable("user_subscriptions");

                entity.Property(e => e.Id).ValueGeneratedNever().HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.PlanId).HasColumnName("plan_id");
                entity.Property(e => e.StartDate).HasColumnName("start_date");
                entity.Property(e => e.EndDate).HasColumnName("end_date");
                entity.Property(e => e.Status).HasMaxLength(20).HasColumnName("status");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserSubscriptions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("user_subscriptions_user_id_fkey");

                entity.HasOne(d => d.Plan)
                    .WithMany(p => p.UserSubscriptions)
                    .HasForeignKey(d => d.PlanId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("user_subscriptions_plan_id_fkey");
            });

            modelBuilder.Entity<Payment>(entity => {
                entity.HasKey(e => e.Id).HasName("payments_pkey");
                entity.ToTable("payments");
                entity.HasIndex(e => e.TransactionId, "payments_transaction_id_key").IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever().HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.SubscriptionId).HasColumnName("subscription_id");
                entity.Property(e => e.Amount).HasPrecision(10, 2).HasColumnName("amount");
                entity.Property(e => e.Currency).HasMaxLength(3).HasDefaultValueSql("'USD'::character varying").HasColumnName("currency");
                entity.Property(e => e.PaymentMethod).HasMaxLength(50).HasColumnName("payment_method");
                entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValueSql("'pending'::character varying").HasColumnName("status");
                entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("payments_user_id_fkey");

                entity.HasOne(d => d.Subscription)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.SubscriptionId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("payments_subscription_id_fkey");
            });

            modelBuilder.Entity<Meeting>(entity => {
                entity.HasKey(e => e.Id).HasName("meetings_pkey");
                entity.ToTable("meetings");
                entity.HasIndex(e => e.HostId, "idx_meetings_host");
                entity.HasIndex(e => e.GuestCode, "meetings_guest_code_key").IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever().HasColumnName("id");
                entity.Property(e => e.HostId).HasColumnName("host_id");
                entity.Property(e => e.Title).HasMaxLength(100).HasColumnName("title");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.StartTime).HasColumnName("start_time");
                entity.Property(e => e.EndTime).HasColumnName("end_time");
                entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValueSql("'scheduled'::character varying").HasColumnName("status");
                entity.Property(e => e.MaxParticipants).HasDefaultValue(50).HasColumnName("max_participants");
                entity.Property(e => e.IsPrivate).HasDefaultValue(false).HasColumnName("is_private");
                entity.Property(e => e.GuestCode).HasColumnName("guest_code");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");

                entity.HasOne(d => d.Host)
                    .WithMany(p => p.HostedMeetings)
                    .HasForeignKey(d => d.HostId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("meetings_host_id_fkey");
            });

            modelBuilder.Entity<MeetingParticipant>(entity => {
                entity.HasKey(e => new { e.MeetingId, e.UserId }).HasName("meeting_participants_pkey");
                entity.ToTable("meeting_participants");
                entity.HasIndex(e => new { e.MeetingId, e.UserId }, "idx_meeting_participants");

                entity.Property(e => e.MeetingId).HasColumnName("meeting_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.JoinTime).HasDefaultValueSql("now()").HasColumnName("join_time");
                entity.Property(e => e.LeaveTime).HasColumnName("leave_time");
                entity.Property(e => e.DeviceInfo).HasColumnType("jsonb").HasColumnName("device_info");

                entity.HasOne(d => d.Meeting)
                    .WithMany(p => p.Participants)
                    .HasForeignKey(d => d.MeetingId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("meeting_participants_meeting_id_fkey");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MeetingParticipations)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("meeting_participants_user_id_fkey");
            });

            modelBuilder.Entity<MeetingRecording>(entity => {
                entity.HasKey(e => e.Id).HasName("meeting_recordings_pkey");
                entity.ToTable("meeting_recordings");

                entity.Property(e => e.Id).ValueGeneratedNever().HasColumnName("id");
                entity.Property(e => e.MeetingId).HasColumnName("meeting_id");
                entity.Property(e => e.StoragePath).HasColumnName("storage_path");
                entity.Property(e => e.Duration).HasColumnName("duration");
                entity.Property(e => e.Processed).HasDefaultValue(false).HasColumnName("processed");
                entity.Property(e => e.Transcription).HasColumnName("transcription");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");

                entity.HasOne(d => d.Meeting)
                    .WithMany(p => p.Recordings)
                    .HasForeignKey(d => d.MeetingId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("meeting_recordings_meeting_id_fkey");
            });

            modelBuilder.Entity<Translation>(entity => {
                entity.HasKey(e => e.Id).HasName("translations_pkey");
                entity.ToTable("translations");
                entity.HasIndex(e => e.MeetingId, "idx_translations_meeting");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.MeetingId).HasColumnName("meeting_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.SourceLanguageId).HasColumnName("source_language");
                entity.Property(e => e.TargetLanguageId).HasColumnName("target_language");
                entity.Property(e => e.OriginalText).HasColumnName("original_text");
                entity.Property(e => e.TranslatedText).HasColumnName("translated_text");
                entity.Property(e => e.Confidence).HasColumnName("confidence");
                entity.Property(e => e.IsCorrected).HasDefaultValue(false).HasColumnName("is_corrected");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");

                entity.HasOne(d => d.Meeting)
                    .WithMany(p => p.Translations)
                    .HasForeignKey(d => d.MeetingId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("translations_meeting_id_fkey");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Translations)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("translations_user_id_fkey");

                entity.HasOne(d => d.SourceLanguage)
                    .WithMany(p => p.SourceTranslations)
                    .HasForeignKey(d => d.SourceLanguageId)
                    .HasConstraintName("translations_source_language_fkey");

                entity.HasOne(d => d.TargetLanguage)
                    .WithMany(p => p.TargetTranslations)
                    .HasForeignKey(d => d.TargetLanguageId)
                    .HasConstraintName("translations_target_language_fkey");
            });

            modelBuilder.Entity<CourseCategory>(entity => {
                entity.HasKey(e => e.Id).HasName("course_categories_pkey");
                entity.ToTable("course_categories");
                entity.HasIndex(e => e.Name, "course_categories_name_key").IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever().HasColumnName("id");
                entity.Property(e => e.Name).HasMaxLength(50).HasColumnName("name");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.ParentId).HasColumnName("parent_id");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.Subcategories)
                    .HasForeignKey(d => d.ParentId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("course_categories_parent_id_fkey");
            });

            modelBuilder.Entity<Course>(entity => {
                entity.HasKey(e => e.Id).HasName("courses_pkey");
                entity.ToTable("courses");
                entity.HasIndex(e => e.LanguageId, "idx_course_language");

                entity.Property(e => e.Id).ValueGeneratedNever().HasColumnName("id");
                entity.Property(e => e.Title).HasMaxLength(100).HasColumnName("title");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.DifficultyLevel).HasMaxLength(20).HasColumnName("difficulty_level");
                entity.Property(e => e.ThumbnailUrl).HasColumnName("thumbnail_url");
                entity.Property(e => e.LanguageId).HasColumnName("language_id");
                entity.Property(e => e.CategoryId).HasColumnName("category_id");
                entity.Property(e => e.CreatorId).HasColumnName("creator_id");
                entity.Property(e => e.IsPublished).HasDefaultValue(false).HasColumnName("is_published");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()").HasColumnName("updated_at");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.LanguageId)
                    .HasConstraintName("courses_language_id_fkey");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("courses_category_id_fkey");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.CreatedCourses)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("courses_creator_id_fkey");
            });

            modelBuilder.Entity<Module>(entity => {
                entity.HasKey(e => e.Id).HasName("modules_pkey");
                entity.ToTable("modules");
                entity.HasIndex(e => e.CourseId, "idx_modules_course");

                entity.Property(e => e.Id).ValueGeneratedNever().HasColumnName("id");
                entity.Property(e => e.CourseId).HasColumnName("course_id");
                entity.Property(e => e.Title).HasMaxLength(100).HasColumnName("title");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.OrderNumber).HasColumnName("order_number");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Modules)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("modules_course_id_fkey");
            });

            modelBuilder.Entity<Lesson>(entity => {
                entity.HasKey(e => e.Id).HasName("lessons_pkey");
                entity.ToTable("lessons");
                entity.HasIndex(e => e.ModuleId, "idx_lessons_module");

                entity.Property(e => e.Id).ValueGeneratedNever().HasColumnName("id");
                entity.Property(e => e.ModuleId).HasColumnName("module_id");
                entity.Property(e => e.Title).HasMaxLength(100).HasColumnName("title");
                entity.Property(e => e.Content).HasColumnName("content");
                entity.Property(e => e.VideoUrl).HasColumnName("video_url");
                entity.Property(e => e.DurationMinutes).HasColumnName("duration_minutes");
                entity.Property(e => e.OrderNumber).HasColumnName("order_number");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.Lessons)
                    .HasForeignKey(d => d.ModuleId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("lessons_module_id_fkey");
            });

            modelBuilder.Entity<Quiz>(entity => {
                entity.HasKey(e => e.Id).HasName("quizzes_pkey");
                entity.ToTable("quizzes");

                entity.Property(e => e.Id).ValueGeneratedNever().HasColumnName("id");
                entity.Property(e => e.LessonId).HasColumnName("lesson_id");
                entity.Property(e => e.Question).HasColumnName("question");
                entity.Property(e => e.Options).HasColumnType("jsonb").HasColumnName("options");
                entity.Property(e => e.CorrectAnswer).HasColumnName("correct_answer");
                entity.Property(e => e.Explanation).HasColumnName("explanation");
                entity.Property(e => e.MaxScore).HasDefaultValue(10).HasColumnName("max_score");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");

                entity.HasOne(d => d.Lesson)
                    .WithMany(p => p.Quizzes)
                    .HasForeignKey(d => d.LessonId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("quizzes_lesson_id_fkey");
            });

            modelBuilder.Entity<QuizAttempt>(entity => {
                entity.HasKey(e => e.Id).HasName("quiz_attempts_pkey");
                entity.ToTable("quiz_attempts");

                entity.Property(e => e.Id).ValueGeneratedNever().HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.QuizId).HasColumnName("quiz_id");
                entity.Property(e => e.SelectedAnswer).HasColumnName("selected_answer");
                entity.Property(e => e.Score).HasColumnName("score");
                entity.Property(e => e.CompletedAt).HasDefaultValueSql("now()").HasColumnName("completed_at");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.QuizAttempts)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("quiz_attempts_user_id_fkey");

                entity.HasOne(d => d.Quiz)
                    .WithMany(p => p.Attempts)
                    .HasForeignKey(d => d.QuizId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("quiz_attempts_quiz_id_fkey");
            });

            modelBuilder.Entity<UserCourseProgress>(entity => {
                entity.HasKey(e => new { e.UserId, e.LessonId }).HasName("user_course_progress_pkey");
                entity.ToTable("user_course_progress");

                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.LessonId).HasColumnName("lesson_id");
                entity.Property(e => e.CompletedAt).HasDefaultValueSql("now()").HasColumnName("completed_at");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.CourseProgresses)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("user_course_progress_user_id_fkey");

                entity.HasOne(d => d.Lesson)
                    .WithMany(p => p.UserProgresses)
                    .HasForeignKey(d => d.LessonId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("user_course_progress_lesson_id_fkey");
            });

            modelBuilder.Entity<CourseReview>(entity => {
                entity.HasKey(e => e.Id).HasName("course_reviews_pkey");
                entity.ToTable("course_reviews");

                entity.Property(e => e.Id).ValueGeneratedNever().HasColumnName("id");
                entity.Property(e => e.CourseId).HasColumnName("course_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Rating).HasColumnName("rating");
                entity.Property(e => e.Comment).HasColumnName("comment");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("course_reviews_course_id_fkey");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.CourseReviews)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("course_reviews_user_id_fkey");
            });

            modelBuilder.Entity<UserMessage>(entity => {
                entity.HasKey(e => e.MessageId).HasName("user_message_pkey");
                entity.ToTable("user_messages");

                entity.Property(e => e.MessageId).ValueGeneratedOnAdd().HasColumnName("id");
                entity.Property(e => e.SenderId).HasColumnName("sender_id");
                entity.Property(e => e.ReceiverId).HasColumnName("receiver_id");
                entity.Property(e => e.Message).HasColumnName("message");
                entity.Property(e => e.DateTime).HasDefaultValueSql("now()").HasColumnName("sent_date");

                entity.HasOne(e => e.Sender)
                    .WithMany(e => e.SentMessages)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("sent_message_profile_fkey");

                entity.HasOne(e => e.Receiver)
                    .WithMany(e => e.ReceivedMessages)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("received_message_profile_fkey");
            });
        }

        public static void SeedData(ModelBuilder modelBuilder) {

        }
    }
}
