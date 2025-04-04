﻿using System.Text.Json.Serialization;
using PasswordKeeper.Interfaces;

namespace PasswordKeeper.DTO;

/// <summary>
/// A DTO for the <c>User</c> database table data.
/// </summary>
public class UserDto : IUser
{
    /// <inheritdoc cref="IHasId.Id" />
    public long Id { get; set; }
    
    /// <inheritdoc cref="IUser.Username" />
    public string Username { get; set; } = string.Empty;

    /// <inheritdoc cref="IUser.UserFullName" />
    public string UserFullName { get; set; } = string.Empty;

    /// <inheritdoc cref="IUser.PasswordHash" />
    [JsonIgnore]
    public string PasswordHash { get; set; } = string.Empty;

    /// <inheritdoc cref="IUser.PasswordSalt" />
    [JsonIgnore]
    public string PasswordSalt { get; set; } = string.Empty;
    
    /// <inheritdoc cref="IUser.IsAdmin" />
    public bool IsAdmin { get; set; }    
}