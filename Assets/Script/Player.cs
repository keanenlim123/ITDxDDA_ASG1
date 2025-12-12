using System.Collections.Generic;

/// <summary>
/// Represents a player in the game, including username, email, password,
/// and a list of habitats the player interacts with.
/// </summary>
public class Player
{
    /// <summary>
    /// The player's chosen username.
    /// </summary>
    public string username;

    /// <summary>
    /// The player's email address.
    /// </summary>
    public string email;

    /// <summary>
    /// The player's password.
    /// </summary>
    public string password;

    /// <summary>
    /// List of habitats owned or interacted with by the player.
    /// </summary>
    public List<HabitatEntry> habitats;
}

/// <summary>
/// Represents an individual habitat entry for a player,
/// including its name and statistics.
/// </summary>
public class HabitatEntry
{
    /// <summary>
    /// Name of the habitat.
    /// </summary>
    public string habitatName;

    /// <summary>
    /// Statistics related to this habitat.
    /// </summary>
    public HabitatStats stats;

    /// <summary>
    /// Constructor for a HabitatEntry.
    /// </summary>
    /// <param name="name">The name of the habitat.</param>
    /// <param name="stats">The statistics for the habitat.</param>
    public HabitatEntry(string name, HabitatStats stats)
    {
        habitatName = name;
        this.stats = stats;
    }
}

/// <summary>
/// Represents the statistics for a habitat,
/// including time spent and points earned.
/// </summary>
public class HabitatStats
{
    /// <summary>
    /// Time spent in this habitat.
    /// </summary>
    public float timeSpent;

    /// <summary>
    /// Points earned in this habitat.
    /// </summary>
    public int pointsEarned;

    /// <summary>
    /// Constructor for HabitatStats.
    /// </summary>
    /// <param name="time">Time spent in the habitat.</param>
    /// <param name="points">Points earned in the habitat.</param>
    public HabitatStats(float time, int points)
    {
        timeSpent = time;
        pointsEarned = points;
    }
}
