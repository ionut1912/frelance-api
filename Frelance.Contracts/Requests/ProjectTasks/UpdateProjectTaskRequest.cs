﻿

using Frelance.Contracts.Enums;

namespace Frelance.Contracts.Requests.ProjectTasks;

public record UpdateProjectTaskRequest(string ProjectTitle, string Title, string Description,string Status, string Priority);