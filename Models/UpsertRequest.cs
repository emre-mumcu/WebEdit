using System;

namespace WebEdit.Models;

public record UpsertRequest(string Key, string? Value);
