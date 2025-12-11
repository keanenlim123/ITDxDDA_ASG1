using System.Collections.Generic;

public class Player
{
    public string username;
    public string email;
    public string password;
    public List<HabitatEntry> habitats;
}

public class HabitatEntry
{
    public string habitatName;
    public HabitatStats stats;

    public HabitatEntry(string name, HabitatStats stats)
    {
        habitatName = name;
        this.stats = stats;
    }
}
public class HabitatStats
{
    public float timeSpent;
    public int pointsEarned;

    public HabitatStats(float time, int points)
    {
        timeSpent = time;
        pointsEarned = points;
    }
}
