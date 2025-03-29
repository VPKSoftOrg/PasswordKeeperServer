namespace PasswordKeeper.Interfaces;

/// <summary>
/// An interface for the shared members of a DAO/TDO using generated key challenge.
/// </summary>
public interface IKeyHashMembers
{
    /// <summary>
    /// The hash of the challenge key.
    /// </summary>
    string ChallengeKeyHash { get; set; }

    /// <summary>
    /// The salt of the challenge key.
    /// </summary>
    string ChallengeKeySalt { get; set; }
}