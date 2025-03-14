﻿using JetBrains.Annotations;

namespace Frelance.Contracts.Dtos;

[UsedImplicitly]
public record TaskDto(
    int Id,
    string Title,
    string Description,
    string ProjectTaskStatus,
    string Priority,
    List<TimeLogDto> TimeLogs,
    DateTime CreatedAt,
    DateTime? UpdatedAt);