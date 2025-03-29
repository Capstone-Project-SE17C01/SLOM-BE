using Bogus;
using Microsoft.EntityFrameworkCore;

namespace Project.Infrastructure.Data
{
    public class ApplicationDbContextConfigurations
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder
            .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
            .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
            .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
            .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn", "phone" })
            .HasPostgresEnum("auth", "one_time_token_type", new[] { "confirmation_token", "reauthentication_token", "recovery_token", "email_change_token_new", "email_change_token_current", "phone_change_token" })
            .HasPostgresEnum("pgsodium", "key_status", new[] { "default", "valid", "invalid", "expired" })
            .HasPostgresEnum("pgsodium", "key_type", new[] { "aead-ietf", "aead-det", "hmacsha512", "hmacsha256", "auth", "shorthash", "generichash", "kdf", "secretbox", "secretstream", "stream_xchacha20" })
            .HasPostgresEnum("realtime", "action", new[] { "INSERT", "UPDATE", "DELETE", "TRUNCATE", "ERROR" })
            .HasPostgresEnum("realtime", "equality_op", new[] { "eq", "neq", "lt", "lte", "gt", "gte", "in" })
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "pgjwt")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("graphql", "pg_graphql")
            .HasPostgresExtension("pgsodium", "pgsodium")
            .HasPostgresExtension("vault", "supabase_vault");

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("audit_logs_pkey");

                entity.ToTable("audit_logs");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.ActionType)
                    .HasMaxLength(50)
                    .HasColumnName("action_type");
                entity.Property(e => e.NewValue)
                    .HasColumnType("jsonb")
                    .HasColumnName("new_value");
                entity.Property(e => e.OldValue)
                    .HasColumnType("jsonb")
                    .HasColumnName("old_value");
                entity.Property(e => e.TargetTable)
                    .HasMaxLength(50)
                    .HasColumnName("target_table");
                entity.Property(e => e.Timestamp)
                    .HasDefaultValueSql("now()")
                    .HasColumnName("timestamp");
                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("audit_logs_user_id_fkey");
            });

            modelBuilder.Entity<AuditLogEntry>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("audit_log_entries_pkey");

                entity.ToTable("audit_log_entries", "auth", tb => tb.HasComment("Auth: Audit trail for user actions."));

                entity.HasIndex(e => e.InstanceId, "audit_logs_instance_id_idx");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.InstanceId).HasColumnName("instance_id");
                entity.Property(e => e.IpAddress)
                    .HasMaxLength(64)
                    .HasDefaultValueSql("''::character varying")
                    .HasColumnName("ip_address");
                entity.Property(e => e.Payload)
                    .HasColumnType("json")
                    .HasColumnName("payload");
            });

            modelBuilder.Entity<Bucket>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("buckets_pkey");

                entity.ToTable("buckets", "storage");

                entity.HasIndex(e => e.Name, "bname").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.AllowedMimeTypes).HasColumnName("allowed_mime_types");
                entity.Property(e => e.AvifAutodetection)
                    .HasDefaultValue(false)
                    .HasColumnName("avif_autodetection");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("now()")
                    .HasColumnName("created_at");
                entity.Property(e => e.FileSizeLimit).HasColumnName("file_size_limit");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Owner)
                    .HasComment("Field is deprecated, use owner_id instead")
                    .HasColumnName("owner");
                entity.Property(e => e.OwnerId).HasColumnName("owner_id");
                entity.Property(e => e.Public)
                    .HasDefaultValue(false)
                    .HasColumnName("public");
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("now()")
                    .HasColumnName("updated_at");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("courses_pkey");

                entity.ToTable("courses");

                entity.HasIndex(e => e.LanguageId, "idx_course_language");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.CategoryId).HasColumnName("category_id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("now()")
                    .HasColumnName("created_at");
                entity.Property(e => e.CreatedBy).HasColumnName("created_by");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.DifficultyLevel)
                    .HasMaxLength(20)
                    .HasColumnName("difficulty_level");
                entity.Property(e => e.LanguageId).HasColumnName("language_id");
                entity.Property(e => e.ThumbnailUrl).HasColumnName("thumbnail_url");
                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .HasColumnName("title");
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("now()")
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.Category).WithMany(p => p.Courses)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("courses_category_id_fkey");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Courses)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("courses_created_by_fkey");

                entity.HasOne(d => d.Language).WithMany(p => p.Courses)
                    .HasForeignKey(d => d.LanguageId)
                    .HasConstraintName("courses_language_id_fkey");
            });

            modelBuilder.Entity<CourseCategory>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("course_categories_pkey");

                entity.ToTable("course_categories");

                entity.HasIndex(e => e.Name, "course_categories_name_key").IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
                entity.Property(e => e.ParentCategoryId).HasColumnName("parent_category_id");

                entity.HasOne(d => d.ParentCategory).WithMany(p => p.InverseParentCategory)
                    .HasForeignKey(d => d.ParentCategoryId)
                    .HasConstraintName("course_categories_parent_category_id_fkey");
            });

            modelBuilder.Entity<CourseReview>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("course_reviews_pkey");

                entity.ToTable("course_reviews");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.Comment).HasColumnName("comment");
                entity.Property(e => e.CourseId).HasColumnName("course_id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("now()")
                    .HasColumnName("created_at");
                entity.Property(e => e.Rating).HasColumnName("rating");
                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Course).WithMany(p => p.CourseReviews)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("course_reviews_course_id_fkey");

                entity.HasOne(d => d.User).WithMany(p => p.CourseReviews)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("course_reviews_user_id_fkey");
            });

            modelBuilder.Entity<DecryptedSecret>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("decrypted_secrets", "vault");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.DecryptedSecret1)
                    .UseCollation("C")
                    .HasColumnName("decrypted_secret");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.KeyId).HasColumnName("key_id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Nonce).HasColumnName("nonce");
                entity.Property(e => e.Secret).HasColumnName("secret");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<FlowState>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("flow_state_pkey");

                entity.ToTable("flow_state", "auth", tb => tb.HasComment("stores metadata for pkce logins"));

                entity.HasIndex(e => e.CreatedAt, "flow_state_created_at_idx").IsDescending();

                entity.HasIndex(e => e.AuthCode, "idx_auth_code");

                entity.HasIndex(e => new { e.UserId, e.AuthenticationMethod }, "idx_user_id_auth_method");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.AuthCode).HasColumnName("auth_code");
                entity.Property(e => e.AuthCodeIssuedAt).HasColumnName("auth_code_issued_at");
                entity.Property(e => e.AuthenticationMethod).HasColumnName("authentication_method");
                entity.Property(e => e.CodeChallenge).HasColumnName("code_challenge");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.ProviderAccessToken).HasColumnName("provider_access_token");
                entity.Property(e => e.ProviderRefreshToken).HasColumnName("provider_refresh_token");
                entity.Property(e => e.ProviderType).HasColumnName("provider_type");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
                entity.Property(e => e.UserId).HasColumnName("user_id");
            });

            modelBuilder.Entity<Identity>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("identities_pkey");

                entity.ToTable("identities", "auth", tb => tb.HasComment("Auth: Stores identities associated to a user."));

                entity.HasIndex(e => e.Email, "identities_email_idx").HasOperators(new[] { "text_pattern_ops" });

                entity.HasIndex(e => new { e.ProviderId, e.Provider }, "identities_provider_id_provider_unique").IsUnique();

                entity.HasIndex(e => e.UserId, "identities_user_id_idx");

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("gen_random_uuid()")
                    .HasColumnName("id");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.Email)
                    .HasComputedColumnSql("lower((identity_data ->> 'email'::text))", true)
                    .HasComment("Auth: Email is a generated column that references the optional email property in the identity_data")
                    .HasColumnName("email");
                entity.Property(e => e.IdentityData)
                    .HasColumnType("jsonb")
                    .HasColumnName("identity_data");
                entity.Property(e => e.LastSignInAt).HasColumnName("last_sign_in_at");
                entity.Property(e => e.Provider).HasColumnName("provider");
                entity.Property(e => e.ProviderId).HasColumnName("provider_id");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User).WithMany(p => p.Identities)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("identities_user_id_fkey");
            });

            modelBuilder.Entity<Instance>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("instances_pkey");

                entity.ToTable("instances", "auth", tb => tb.HasComment("Auth: Manages users across multiple sites."));

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.RawBaseConfig).HasColumnName("raw_base_config");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
                entity.Property(e => e.Uuid).HasColumnName("uuid");
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("languages_pkey");

                entity.ToTable("languages");

                entity.HasIndex(e => e.Code, "languages_code_key").IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.Code)
                    .HasMaxLength(5)
                    .HasColumnName("code");
                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
                entity.Property(e => e.Region)
                    .HasMaxLength(50)
                    .HasColumnName("region");
            });

            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("lessons_pkey");

                entity.ToTable("lessons");

                entity.HasIndex(e => e.ModuleId, "idx_lessons_module");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.Content).HasColumnName("content");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("now()")
                    .HasColumnName("created_at");
                entity.Property(e => e.ModuleId).HasColumnName("module_id");
                entity.Property(e => e.OrderNumber).HasColumnName("order_number");
                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .HasColumnName("title");
                entity.Property(e => e.VideoUrl).HasColumnName("video_url");

                entity.HasOne(d => d.Module).WithMany(p => p.Lessons)
                    .HasForeignKey(d => d.ModuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("lessons_module_id_fkey");
            });

            modelBuilder.Entity<Meeting>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("meetings_pkey");

                entity.ToTable("meetings");

                entity.HasIndex(e => e.HostId, "idx_meetings_host");

                entity.HasIndex(e => e.GuestCode, "meetings_guest_code_key").IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.AllowGuests)
                    .HasDefaultValue(false)
                    .HasColumnName("allow_guests");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("now()")
                    .HasColumnName("created_at");
                entity.Property(e => e.EndTime).HasColumnName("end_time");
                entity.Property(e => e.GuestCode).HasColumnName("guest_code");
                entity.Property(e => e.HostId).HasColumnName("host_id");
                entity.Property(e => e.StartTime).HasColumnName("start_time");
                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasDefaultValueSql("'scheduled'::character varying")
                    .HasColumnName("status");
                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .HasColumnName("title");

                entity.HasOne(d => d.Host).WithMany(p => p.Meetings)
                    .HasForeignKey(d => d.HostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("meetings_host_id_fkey");
            });

            modelBuilder.Entity<MeetingParticipant>(entity =>
            {
                entity.HasKey(e => new { e.MeetingId, e.UserId }).HasName("meeting_participants_pkey");

                entity.ToTable("meeting_participants");

                entity.HasIndex(e => new { e.MeetingId, e.UserId }, "idx_meeting_participants");

                entity.Property(e => e.MeetingId).HasColumnName("meeting_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.DeviceInfo)
                    .HasColumnType("jsonb")
                    .HasColumnName("device_info");
                entity.Property(e => e.JoinTime)
                    .HasDefaultValueSql("now()")
                    .HasColumnName("join_time");
                entity.Property(e => e.LeaveTime).HasColumnName("leave_time");

                entity.HasOne(d => d.Meeting).WithMany(p => p.MeetingParticipants)
                    .HasForeignKey(d => d.MeetingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("meeting_participants_meeting_id_fkey");

                entity.HasOne(d => d.User).WithMany(p => p.MeetingParticipants)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("meeting_participants_user_id_fkey");
            });

            modelBuilder.Entity<MeetingRecording>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("meeting_recordings_pkey");

                entity.ToTable("meeting_recordings");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.Duration).HasColumnName("duration");
                entity.Property(e => e.MeetingId).HasColumnName("meeting_id");
                entity.Property(e => e.Processed)
                    .HasDefaultValue(false)
                    .HasColumnName("processed");
                entity.Property(e => e.StoragePath).HasColumnName("storage_path");
                entity.Property(e => e.Transcription).HasColumnName("transcription");

                entity.HasOne(d => d.Meeting).WithMany(p => p.MeetingRecordings)
                    .HasForeignKey(d => d.MeetingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("meeting_recordings_meeting_id_fkey");
            });

            modelBuilder.Entity<MfaAmrClaim>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("amr_id_pk");

                entity.ToTable("mfa_amr_claims", "auth", tb => tb.HasComment("auth: stores authenticator method reference claims for multi factor authentication"));

                entity.HasIndex(e => new { e.SessionId, e.AuthenticationMethod }, "mfa_amr_claims_session_id_authentication_method_pkey").IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.AuthenticationMethod).HasColumnName("authentication_method");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.SessionId).HasColumnName("session_id");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(d => d.Session).WithMany(p => p.MfaAmrClaims)
                    .HasForeignKey(d => d.SessionId)
                    .HasConstraintName("mfa_amr_claims_session_id_fkey");
            });

            modelBuilder.Entity<MfaChallenge>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("mfa_challenges_pkey");

                entity.ToTable("mfa_challenges", "auth", tb => tb.HasComment("auth: stores metadata about challenge requests made"));

                entity.HasIndex(e => e.CreatedAt, "mfa_challenge_created_at_idx").IsDescending();

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.FactorId).HasColumnName("factor_id");
                entity.Property(e => e.IpAddress).HasColumnName("ip_address");
                entity.Property(e => e.OtpCode).HasColumnName("otp_code");
                entity.Property(e => e.VerifiedAt).HasColumnName("verified_at");
                entity.Property(e => e.WebAuthnSessionData)
                    .HasColumnType("jsonb")
                    .HasColumnName("web_authn_session_data");

                entity.HasOne(d => d.Factor).WithMany(p => p.MfaChallenges)
                    .HasForeignKey(d => d.FactorId)
                    .HasConstraintName("mfa_challenges_auth_factor_id_fkey");
            });

            modelBuilder.Entity<MfaFactor>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("mfa_factors_pkey");

                entity.ToTable("mfa_factors", "auth", tb => tb.HasComment("auth: stores metadata about factors"));

                entity.HasIndex(e => new { e.UserId, e.CreatedAt }, "factor_id_created_at_idx");

                entity.HasIndex(e => e.LastChallengedAt, "mfa_factors_last_challenged_at_key").IsUnique();

                entity.HasIndex(e => new { e.FriendlyName, e.UserId }, "mfa_factors_user_friendly_name_unique")
                    .IsUnique()
                    .HasFilter("(TRIM(BOTH FROM friendly_name) <> ''::text)");

                entity.HasIndex(e => e.UserId, "mfa_factors_user_id_idx");

                entity.HasIndex(e => new { e.UserId, e.Phone }, "unique_phone_factor_per_user").IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.FriendlyName).HasColumnName("friendly_name");
                entity.Property(e => e.LastChallengedAt).HasColumnName("last_challenged_at");
                entity.Property(e => e.Phone).HasColumnName("phone");
                entity.Property(e => e.Secret).HasColumnName("secret");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.WebAuthnAaguid).HasColumnName("web_authn_aaguid");
                entity.Property(e => e.WebAuthnCredential)
                    .HasColumnType("jsonb")
                    .HasColumnName("web_authn_credential");

                entity.HasOne(d => d.User).WithMany(p => p.MfaFactors)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("mfa_factors_user_id_fkey");
            });

            modelBuilder.Entity<Migration>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("migrations_pkey");

                entity.ToTable("migrations", "storage");

                entity.HasIndex(e => e.Name, "migrations_name_key").IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.ExecutedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("executed_at");
                entity.Property(e => e.Hash)
                    .HasMaxLength(40)
                    .HasColumnName("hash");
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("modules_pkey");

                entity.ToTable("modules");

                entity.HasIndex(e => e.CourseId, "idx_modules_course");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.CourseId).HasColumnName("course_id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("now()")
                    .HasColumnName("created_at");
                entity.Property(e => e.OrderNumber).HasColumnName("order_number");
                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .HasColumnName("title");

                entity.HasOne(d => d.Course).WithMany(p => p.Modules)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("modules_course_id_fkey");
            });

            modelBuilder.Entity<Object>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("objects_pkey");

                entity.ToTable("objects", "storage");

                entity.HasIndex(e => new { e.BucketId, e.Name }, "bucketid_objname").IsUnique();

                entity.HasIndex(e => new { e.BucketId, e.Name }, "idx_objects_bucket_id_name").UseCollation(new[] { null, "C" });

                entity.HasIndex(e => e.Name, "name_prefix_search").HasOperators(new[] { "text_pattern_ops" });

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("gen_random_uuid()")
                    .HasColumnName("id");
                entity.Property(e => e.BucketId).HasColumnName("bucket_id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("now()")
                    .HasColumnName("created_at");
                entity.Property(e => e.LastAccessedAt)
                    .HasDefaultValueSql("now()")
                    .HasColumnName("last_accessed_at");
                entity.Property(e => e.Metadata)
                    .HasColumnType("jsonb")
                    .HasColumnName("metadata");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Owner)
                    .HasComment("Field is deprecated, use owner_id instead")
                    .HasColumnName("owner");
                entity.Property(e => e.OwnerId).HasColumnName("owner_id");
                entity.Property(e => e.PathTokens)
                    .HasComputedColumnSql("string_to_array(name, '/'::text)", true)
                    .HasColumnName("path_tokens");
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("now()")
                    .HasColumnName("updated_at");
                entity.Property(e => e.UserMetadata)
                    .HasColumnType("jsonb")
                    .HasColumnName("user_metadata");
                entity.Property(e => e.Version).HasColumnName("version");

                entity.HasOne(d => d.Bucket).WithMany(p => p.Objects)
                    .HasForeignKey(d => d.BucketId)
                    .HasConstraintName("objects_bucketId_fkey");
            });

            modelBuilder.Entity<OneTimeToken>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("one_time_tokens_pkey");

                entity.ToTable("one_time_tokens", "auth");

                entity.HasIndex(e => e.RelatesTo, "one_time_tokens_relates_to_hash_idx").HasMethod("hash");

                entity.HasIndex(e => e.TokenHash, "one_time_tokens_token_hash_hash_idx").HasMethod("hash");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("now()")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_at");
                entity.Property(e => e.RelatesTo).HasColumnName("relates_to");
                entity.Property(e => e.TokenHash).HasColumnName("token_hash");
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("now()")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updated_at");
                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User).WithMany(p => p.OneTimeTokens)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("one_time_tokens_user_id_fkey");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("payments_pkey");

                entity.ToTable("payments");

                entity.HasIndex(e => e.TransactionId, "payments_transaction_id_key").IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.Amount)
                    .HasPrecision(10, 2)
                    .HasColumnName("amount");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("now()")
                    .HasColumnName("created_at");
                entity.Property(e => e.Currency)
                    .HasMaxLength(3)
                    .HasDefaultValueSql("'USD'::character varying")
                    .HasColumnName("currency");
                entity.Property(e => e.PaymentMethod)
                    .HasMaxLength(50)
                    .HasColumnName("payment_method");
                entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User).WithMany(p => p.Payments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("payments_user_id_fkey");
            });

            modelBuilder.Entity<Profile>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("profiles_pkey");

                entity.ToTable("profiles");

                entity.HasIndex(e => e.Username, "profiles_username_key").IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("now()")
                    .HasColumnName("created_at");
                entity.Property(e => e.PreferredLanguage).HasColumnName("preferred_language");
                entity.Property(e => e.Role)
                    .HasMaxLength(20)
                    .HasDefaultValueSql("'registered'::character varying")
                    .HasColumnName("role");
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("now()")
                    .HasColumnName("updated_at");
                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .HasColumnName("username");

                entity.HasOne(d => d.IdNavigation).WithOne(p => p.Profile)
                    .HasForeignKey<Profile>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("profiles_id_fkey");

                entity.HasOne(d => d.PreferredLanguageNavigation).WithMany(p => p.Profiles)
                    .HasForeignKey(d => d.PreferredLanguage)
                    .HasConstraintName("profiles_preferred_language_fkey");
            });

            modelBuilder.Entity<Quiz>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("quizzes_pkey");

                entity.ToTable("quizzes");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.CorrectAnswer).HasColumnName("correct_answer");
                entity.Property(e => e.LessonId).HasColumnName("lesson_id");
                entity.Property(e => e.MaxScore)
                    .HasDefaultValue(10)
                    .HasColumnName("max_score");
                entity.Property(e => e.Options)
                    .HasColumnType("jsonb")
                    .HasColumnName("options");
                entity.Property(e => e.Question).HasColumnName("question");

                entity.HasOne(d => d.Lesson).WithMany(p => p.Quizzes)
                    .HasForeignKey(d => d.LessonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("quizzes_lesson_id_fkey");
            });

            modelBuilder.Entity<QuizAttempt>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("quiz_attempts_pkey");

                entity.ToTable("quiz_attempts");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.AttemptedAt)
                    .HasDefaultValueSql("now()")
                    .HasColumnName("attempted_at");
                entity.Property(e => e.QuizId).HasColumnName("quiz_id");
                entity.Property(e => e.Score).HasColumnName("score");
                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Quiz).WithMany(p => p.QuizAttempts)
                    .HasForeignKey(d => d.QuizId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("quiz_attempts_quiz_id_fkey");

                entity.HasOne(d => d.User).WithMany(p => p.QuizAttempts)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("quiz_attempts_user_id_fkey");
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("refresh_tokens_pkey");

                entity.ToTable("refresh_tokens", "auth", tb => tb.HasComment("Auth: Store of tokens used to refresh JWT tokens once they expire."));

                entity.HasIndex(e => e.InstanceId, "refresh_tokens_instance_id_idx");

                entity.HasIndex(e => new { e.InstanceId, e.UserId }, "refresh_tokens_instance_id_user_id_idx");

                entity.HasIndex(e => e.Parent, "refresh_tokens_parent_idx");

                entity.HasIndex(e => new { e.SessionId, e.Revoked }, "refresh_tokens_session_id_revoked_idx");

                entity.HasIndex(e => e.Token, "refresh_tokens_token_unique").IsUnique();

                entity.HasIndex(e => e.UpdatedAt, "refresh_tokens_updated_at_idx").IsDescending();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.InstanceId).HasColumnName("instance_id");
                entity.Property(e => e.Parent)
                    .HasMaxLength(255)
                    .HasColumnName("parent");
                entity.Property(e => e.Revoked).HasColumnName("revoked");
                entity.Property(e => e.SessionId).HasColumnName("session_id");
                entity.Property(e => e.Token)
                    .HasMaxLength(255)
                    .HasColumnName("token");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
                entity.Property(e => e.UserId)
                    .HasMaxLength(255)
                    .HasColumnName("user_id");

                entity.HasOne(d => d.Session).WithMany(p => p.RefreshTokens)
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("refresh_tokens_session_id_fkey");
            });

            modelBuilder.Entity<S3MultipartUpload>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("s3_multipart_uploads_pkey");

                entity.ToTable("s3_multipart_uploads", "storage");

                entity.HasIndex(e => new { e.BucketId, e.Key, e.CreatedAt }, "idx_multipart_uploads_list").UseCollation(new[] { null, "C", null });

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.BucketId).HasColumnName("bucket_id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("now()")
                    .HasColumnName("created_at");
                entity.Property(e => e.InProgressSize)
                    .HasDefaultValue(0L)
                    .HasColumnName("in_progress_size");
                entity.Property(e => e.Key)
                    .UseCollation("C")
                    .HasColumnName("key");
                entity.Property(e => e.OwnerId).HasColumnName("owner_id");
                entity.Property(e => e.UploadSignature).HasColumnName("upload_signature");
                entity.Property(e => e.UserMetadata)
                    .HasColumnType("jsonb")
                    .HasColumnName("user_metadata");
                entity.Property(e => e.Version).HasColumnName("version");

                entity.HasOne(d => d.Bucket).WithMany(p => p.S3MultipartUploads)
                    .HasForeignKey(d => d.BucketId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("s3_multipart_uploads_bucket_id_fkey");
            });

            modelBuilder.Entity<S3MultipartUploadsPart>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("s3_multipart_uploads_parts_pkey");

                entity.ToTable("s3_multipart_uploads_parts", "storage");

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("gen_random_uuid()")
                    .HasColumnName("id");
                entity.Property(e => e.BucketId).HasColumnName("bucket_id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("now()")
                    .HasColumnName("created_at");
                entity.Property(e => e.Etag).HasColumnName("etag");
                entity.Property(e => e.Key)
                    .UseCollation("C")
                    .HasColumnName("key");
                entity.Property(e => e.OwnerId).HasColumnName("owner_id");
                entity.Property(e => e.PartNumber).HasColumnName("part_number");
                entity.Property(e => e.Size)
                    .HasDefaultValue(0L)
                    .HasColumnName("size");
                entity.Property(e => e.UploadId).HasColumnName("upload_id");
                entity.Property(e => e.Version).HasColumnName("version");

                entity.HasOne(d => d.Bucket).WithMany(p => p.S3MultipartUploadsParts)
                    .HasForeignKey(d => d.BucketId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("s3_multipart_uploads_parts_bucket_id_fkey");

                entity.HasOne(d => d.Upload).WithMany(p => p.S3MultipartUploadsParts)
                    .HasForeignKey(d => d.UploadId)
                    .HasConstraintName("s3_multipart_uploads_parts_upload_id_fkey");
            });

            modelBuilder.Entity<SamlProvider>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("saml_providers_pkey");

                entity.ToTable("saml_providers", "auth", tb => tb.HasComment("Auth: Manages SAML Identity Provider connections."));

                entity.HasIndex(e => e.EntityId, "saml_providers_entity_id_key").IsUnique();

                entity.HasIndex(e => e.SsoProviderId, "saml_providers_sso_provider_id_idx");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.AttributeMapping)
                    .HasColumnType("jsonb")
                    .HasColumnName("attribute_mapping");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.EntityId).HasColumnName("entity_id");
                entity.Property(e => e.MetadataUrl).HasColumnName("metadata_url");
                entity.Property(e => e.MetadataXml).HasColumnName("metadata_xml");
                entity.Property(e => e.NameIdFormat).HasColumnName("name_id_format");
                entity.Property(e => e.SsoProviderId).HasColumnName("sso_provider_id");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(d => d.SsoProvider).WithMany(p => p.SamlProviders)
                    .HasForeignKey(d => d.SsoProviderId)
                    .HasConstraintName("saml_providers_sso_provider_id_fkey");
            });

            modelBuilder.Entity<SamlRelayState>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("saml_relay_states_pkey");

                entity.ToTable("saml_relay_states", "auth", tb => tb.HasComment("Auth: Contains SAML Relay State information for each Service Provider initiated login."));

                entity.HasIndex(e => e.CreatedAt, "saml_relay_states_created_at_idx").IsDescending();

                entity.HasIndex(e => e.ForEmail, "saml_relay_states_for_email_idx");

                entity.HasIndex(e => e.SsoProviderId, "saml_relay_states_sso_provider_id_idx");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.FlowStateId).HasColumnName("flow_state_id");
                entity.Property(e => e.ForEmail).HasColumnName("for_email");
                entity.Property(e => e.RedirectTo).HasColumnName("redirect_to");
                entity.Property(e => e.RequestId).HasColumnName("request_id");
                entity.Property(e => e.SsoProviderId).HasColumnName("sso_provider_id");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(d => d.FlowState).WithMany(p => p.SamlRelayStates)
                    .HasForeignKey(d => d.FlowStateId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("saml_relay_states_flow_state_id_fkey");

                entity.HasOne(d => d.SsoProvider).WithMany(p => p.SamlRelayStates)
                    .HasForeignKey(d => d.SsoProviderId)
                    .HasConstraintName("saml_relay_states_sso_provider_id_fkey");
            });

            modelBuilder.Entity<SchemaMigration>(entity =>
            {
                entity.HasKey(e => e.Version).HasName("schema_migrations_pkey");

                entity.ToTable("schema_migrations", "realtime");

                entity.Property(e => e.Version)
                    .ValueGeneratedNever()
                    .HasColumnName("version");
                entity.Property(e => e.InsertedAt)
                    .HasColumnType("timestamp(0) without time zone")
                    .HasColumnName("inserted_at");
            });

            modelBuilder.Entity<SchemaMigration1>(entity =>
            {
                entity.HasKey(e => e.Version).HasName("schema_migrations_pkey");

                entity.ToTable("schema_migrations", "auth", tb => tb.HasComment("Auth: Manages updates to the auth system."));

                entity.Property(e => e.Version)
                    .HasMaxLength(255)
                    .HasColumnName("version");
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("sessions_pkey");

                entity.ToTable("sessions", "auth", tb => tb.HasComment("Auth: Stores session data associated to a user."));

                entity.HasIndex(e => e.NotAfter, "sessions_not_after_idx").IsDescending();

                entity.HasIndex(e => e.UserId, "sessions_user_id_idx");

                entity.HasIndex(e => new { e.UserId, e.CreatedAt }, "user_id_created_at_idx");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.FactorId).HasColumnName("factor_id");
                entity.Property(e => e.Ip).HasColumnName("ip");
                entity.Property(e => e.NotAfter)
                    .HasComment("Auth: Not after is a nullable column that contains a timestamp after which the session should be regarded as expired.")
                    .HasColumnName("not_after");
                entity.Property(e => e.RefreshedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("refreshed_at");
                entity.Property(e => e.Tag).HasColumnName("tag");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
                entity.Property(e => e.UserAgent).HasColumnName("user_agent");
                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User).WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("sessions_user_id_fkey");
            });

            modelBuilder.Entity<SsoDomain>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("sso_domains_pkey");

                entity.ToTable("sso_domains", "auth", tb => tb.HasComment("Auth: Manages SSO email address domain mapping to an SSO Identity Provider."));

                entity.HasIndex(e => e.SsoProviderId, "sso_domains_sso_provider_id_idx");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.Domain).HasColumnName("domain");
                entity.Property(e => e.SsoProviderId).HasColumnName("sso_provider_id");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(d => d.SsoProvider).WithMany(p => p.SsoDomains)
                    .HasForeignKey(d => d.SsoProviderId)
                    .HasConstraintName("sso_domains_sso_provider_id_fkey");
            });

            modelBuilder.Entity<SsoProvider>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("sso_providers_pkey");

                entity.ToTable("sso_providers", "auth", tb => tb.HasComment("Auth: Manages SSO identity provider information; see saml_providers for SAML."));

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.ResourceId)
                    .HasComment("Auth: Uniquely identifies a SSO provider according to a user-chosen resource ID (case insensitive), useful in infrastructure as code.")
                    .HasColumnName("resource_id");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_subscription");

                entity.ToTable("subscription", "realtime");

                entity.Property(e => e.Id)
                    .UseIdentityAlwaysColumn()
                    .HasColumnName("id");
                entity.Property(e => e.Claims)
                    .HasColumnType("jsonb")
                    .HasColumnName("claims");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("timezone('utc'::text, now())")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_at");
                entity.Property(e => e.SubscriptionId).HasColumnName("subscription_id");
            });

            modelBuilder.Entity<SubscriptionPlan>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("subscription_plans_pkey");

                entity.ToTable("subscription_plans");

                entity.HasIndex(e => e.Name, "subscription_plans_name_key").IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.DurationDays).HasColumnName("duration_days");
                entity.Property(e => e.Features)
                    .HasColumnType("jsonb")
                    .HasColumnName("features");
                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
                entity.Property(e => e.Price)
                    .HasPrecision(10, 2)
                    .HasColumnName("price");
            });

            modelBuilder.Entity<Translation>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("translations_pkey");

                entity.ToTable("translations");

                entity.HasIndex(e => e.MeetingId, "idx_translations_meeting");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.MeetingId).HasColumnName("meeting_id");
                entity.Property(e => e.OriginalText).HasColumnName("original_text");
                entity.Property(e => e.SourceLanguage).HasColumnName("source_language");
                entity.Property(e => e.TargetLanguage).HasColumnName("target_language");
                entity.Property(e => e.Timestamp)
                    .HasDefaultValueSql("now()")
                    .HasColumnName("timestamp");
                entity.Property(e => e.TranslatedText).HasColumnName("translated_text");
                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Meeting).WithMany(p => p.Translations)
                    .HasForeignKey(d => d.MeetingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("translations_meeting_id_fkey");

                entity.HasOne(d => d.SourceLanguageNavigation).WithMany(p => p.TranslationSourceLanguageNavigations)
                    .HasForeignKey(d => d.SourceLanguage)
                    .HasConstraintName("translations_source_language_fkey");

                entity.HasOne(d => d.TargetLanguageNavigation).WithMany(p => p.TranslationTargetLanguageNavigations)
                    .HasForeignKey(d => d.TargetLanguage)
                    .HasConstraintName("translations_target_language_fkey");

                entity.HasOne(d => d.User).WithMany(p => p.Translations)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("translations_user_id_fkey");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("users_pkey");

                entity.ToTable("users", "auth", tb => tb.HasComment("Auth: Stores user login data within a secure schema."));

                entity.HasIndex(e => e.ConfirmationToken, "confirmation_token_idx")
                    .IsUnique()
                    .HasFilter("((confirmation_token)::text !~ '^[0-9 ]*$'::text)");

                entity.HasIndex(e => e.EmailChangeTokenCurrent, "email_change_token_current_idx")
                    .IsUnique()
                    .HasFilter("((email_change_token_current)::text !~ '^[0-9 ]*$'::text)");

                entity.HasIndex(e => e.EmailChangeTokenNew, "email_change_token_new_idx")
                    .IsUnique()
                    .HasFilter("((email_change_token_new)::text !~ '^[0-9 ]*$'::text)");

                entity.HasIndex(e => e.ReauthenticationToken, "reauthentication_token_idx")
                    .IsUnique()
                    .HasFilter("((reauthentication_token)::text !~ '^[0-9 ]*$'::text)");

                entity.HasIndex(e => e.RecoveryToken, "recovery_token_idx")
                    .IsUnique()
                    .HasFilter("((recovery_token)::text !~ '^[0-9 ]*$'::text)");

                entity.HasIndex(e => e.Email, "users_email_partial_key")
                    .IsUnique()
                    .HasFilter("(is_sso_user = false)");

                entity.HasIndex(e => e.InstanceId, "users_instance_id_idx");

                entity.HasIndex(e => e.IsAnonymous, "users_is_anonymous_idx");

                entity.HasIndex(e => e.Phone, "users_phone_key").IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.Aud)
                    .HasMaxLength(255)
                    .HasColumnName("aud");
                entity.Property(e => e.BannedUntil).HasColumnName("banned_until");
                entity.Property(e => e.ConfirmationSentAt).HasColumnName("confirmation_sent_at");
                entity.Property(e => e.ConfirmationToken)
                    .HasMaxLength(255)
                    .HasColumnName("confirmation_token");
                entity.Property(e => e.ConfirmedAt)
                    .HasComputedColumnSql("LEAST(email_confirmed_at, phone_confirmed_at)", true)
                    .HasColumnName("confirmed_at");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");
                entity.Property(e => e.EmailChange)
                    .HasMaxLength(255)
                    .HasColumnName("email_change");
                entity.Property(e => e.EmailChangeConfirmStatus)
                    .HasDefaultValue((short)0)
                    .HasColumnName("email_change_confirm_status");
                entity.Property(e => e.EmailChangeSentAt).HasColumnName("email_change_sent_at");
                entity.Property(e => e.EmailChangeTokenCurrent)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("''::character varying")
                    .HasColumnName("email_change_token_current");
                entity.Property(e => e.EmailChangeTokenNew)
                    .HasMaxLength(255)
                    .HasColumnName("email_change_token_new");
                entity.Property(e => e.EmailConfirmedAt).HasColumnName("email_confirmed_at");
                entity.Property(e => e.EncryptedPassword)
                    .HasMaxLength(255)
                    .HasColumnName("encrypted_password");
                entity.Property(e => e.InstanceId).HasColumnName("instance_id");
                entity.Property(e => e.InvitedAt).HasColumnName("invited_at");
                entity.Property(e => e.IsAnonymous)
                    .HasDefaultValue(false)
                    .HasColumnName("is_anonymous");
                entity.Property(e => e.IsSsoUser)
                    .HasDefaultValue(false)
                    .HasComment("Auth: Set this column to true when the account comes from SSO. These accounts can have duplicate emails.")
                    .HasColumnName("is_sso_user");
                entity.Property(e => e.IsSuperAdmin).HasColumnName("is_super_admin");
                entity.Property(e => e.LastSignInAt).HasColumnName("last_sign_in_at");
                entity.Property(e => e.Phone)
                    .HasDefaultValueSql("NULL::character varying")
                    .HasColumnName("phone");
                entity.Property(e => e.PhoneChange)
                    .HasDefaultValueSql("''::character varying")
                    .HasColumnName("phone_change");
                entity.Property(e => e.PhoneChangeSentAt).HasColumnName("phone_change_sent_at");
                entity.Property(e => e.PhoneChangeToken)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("''::character varying")
                    .HasColumnName("phone_change_token");
                entity.Property(e => e.PhoneConfirmedAt).HasColumnName("phone_confirmed_at");
                entity.Property(e => e.RawAppMetaData)
                    .HasColumnType("jsonb")
                    .HasColumnName("raw_app_meta_data");
                entity.Property(e => e.RawUserMetaData)
                    .HasColumnType("jsonb")
                    .HasColumnName("raw_user_meta_data");
                entity.Property(e => e.ReauthenticationSentAt).HasColumnName("reauthentication_sent_at");
                entity.Property(e => e.ReauthenticationToken)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("''::character varying")
                    .HasColumnName("reauthentication_token");
                entity.Property(e => e.RecoverySentAt).HasColumnName("recovery_sent_at");
                entity.Property(e => e.RecoveryToken)
                    .HasMaxLength(255)
                    .HasColumnName("recovery_token");
                entity.Property(e => e.Role)
                    .HasMaxLength(255)
                    .HasColumnName("role");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<UserActivity>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("user_activities_pkey");

                entity.ToTable("user_activities");

                entity.HasIndex(e => new { e.UserId, e.ActivityType }, "idx_user_activities");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.ActivityType)
                    .HasMaxLength(50)
                    .HasColumnName("activity_type");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("now()")
                    .HasColumnName("created_at");
                entity.Property(e => e.Metadata)
                    .HasColumnType("jsonb")
                    .HasColumnName("metadata");
                entity.Property(e => e.TargetId).HasColumnName("target_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User).WithMany(p => p.UserActivities)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("user_activities_user_id_fkey");
            });

            modelBuilder.Entity<UserCourseProgress>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LessonId }).HasName("user_course_progress_pkey");

                entity.ToTable("user_course_progress");

                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.LessonId).HasColumnName("lesson_id");
                entity.Property(e => e.CompletedAt)
                    .HasDefaultValueSql("now()")
                    .HasColumnName("completed_at");

                entity.HasOne(d => d.Lesson).WithMany(p => p.UserCourseProgresses)
                    .HasForeignKey(d => d.LessonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_course_progress_lesson_id_fkey");

                entity.HasOne(d => d.User).WithMany(p => p.UserCourseProgresses)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_course_progress_user_id_fkey");
            });

            modelBuilder.Entity<UserSubscription>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("user_subscriptions_pkey");

                entity.ToTable("user_subscriptions");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.EndDate).HasColumnName("end_date");
                entity.Property(e => e.PlanId).HasColumnName("plan_id");
                entity.Property(e => e.StartDate).HasColumnName("start_date");
                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasColumnName("status");
                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Plan).WithMany(p => p.UserSubscriptions)
                    .HasForeignKey(d => d.PlanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_subscriptions_plan_id_fkey");

                entity.HasOne(d => d.User).WithMany(p => p.UserSubscriptions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_subscriptions_user_id_fkey");
            });
            modelBuilder.HasSequence<int>("seq_schema_version", "graphql").IsCyclic();
        }

        public static void SeedData(ModelBuilder modelBuilder)
        {

        }

    }
}
