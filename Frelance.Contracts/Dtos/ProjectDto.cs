﻿namespace Frelance.API.Frelance.Contracts.Dtos;

public record ProjectDto(int Id,string Title,string Description,DateTime CreatedAt,DateTime Deadline,List<string> Technologies);