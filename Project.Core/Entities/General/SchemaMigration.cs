﻿namespace Project.Infrastructure;

public partial class SchemaMigration {
    public long Version { get; set; }

    public DateTime? InsertedAt { get; set; }
}
