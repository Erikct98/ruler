

public class rbAI : rbPlayer 
{
    public int id;
    public int score;
    public string method;

    // For easy reference
    public bool isAI = true;
    public rbAI(string id, string score, string method)
    {
        id = id;
        score = score;
    }

    // Chooses a next point by applying a method to remove from the given playing field
    public ChoosePoint()
    {
        // Apply method from settings here
        // @Jurrien van Winden
    }

    // Greedy method to remove a point from a convex hull, maximizing the area lost (score gained)
    public greedyMethod()
    {
        // @Jurrien van Winden
    }

    // Randomly pick a point to remove
    public randomMethod()
    {
        // @Jurrien van Winden
    }
}