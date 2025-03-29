using System;
using System.Collections.Generic;

namespace Project.Infrastructure;

public partial class UserCourseProgress
{
    public Guid UserId { get; set; }

    public Guid LessonId { get; set; }

    public DateTime? CompletedAt { get; set; }

    public virtual Lesson Lesson { get; set; } = null!;

    public virtual Profile User { get; set; } = null!;
}
