namespace PasswordKeeper.Interfaces.Enumerations;

/// <summary>
/// An enum for the reasons why a login attempt failed.
/// </summary>
public enum LoginRejectReason
{
    /// <summary>
    /// The login attempt was not rejected.
    /// </summary>
    None = 0,
    
    /// <summary>
    /// The login attempt was unauthorized.
    /// </summary>
    Unauthorized = 1,
    
    /// <summary>
    /// The password does not meet the complexity requirements for the system. It must be at least 8 characters long.
    /// </summary>
    PasswordMustBeAtLeast8Characters = 2,
    
    /// <summary>
    /// The password does not meet the complexity requirements for the system. It must contain at least one lowercase letter.
    /// </summary>
    PasswordMustContainAtLeastOneLowercaseLetter = 3,
    
    /// <summary>
    /// The password does not meet the complexity requirements for the system. It must contain at least one uppercase letter.
    /// </summary>
    PasswordMustContainAtLeastOneUppercaseLetter = 4,
    
    /// <summary>
    /// The password does not meet the complexity requirements for the system. It must contain at least one digit.
    /// </summary>
    PasswordMustContainAtLeastOneDigit = 5,
    
    /// <summary>
    /// The password does not meet the complexity requirements for the system. It must contain at least one special character.
    /// </summary>
    PasswordMustContainAtLeastOneSpecialCharacter = 6,
    
    /// <summary>
    /// The username must be at least 4 characters long.
    /// </summary>
    UsernameMustBeAtLeast4Characters = 7,
    
    /// <summary>
    /// Failed to create the admin user.
    /// </summary>
    FailedToCreateAdminUser = 8,
    
    /// <summary>
    /// The username or password is incorrect.
    /// </summary>
    InvalidUsernameOrPassword = 9,
    
    /// <summary>
    /// Data related to the user was not found.
    /// </summary>
    NotFound,
}