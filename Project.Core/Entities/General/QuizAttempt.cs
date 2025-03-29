using System;
using System.Collections.Generic;

namespace Project.Infrastructure;

public partial class QuizAttempt
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid QuizId { get; set; }

    public int? Score { get; set; }

    public DateTime? AttemptedAt { get; set; }

    public virtual Quiz Quiz { get; set; } = null!;

    public virtual Profile User { get; set; } = null!;
}
