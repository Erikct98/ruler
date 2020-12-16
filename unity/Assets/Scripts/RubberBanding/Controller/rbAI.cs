
// @Jurrien van Winden
public class rbAI : rbPlayer 
{
    public string method;

    // For easy reference
    public bool isAI = true;
    public rbAI(string method, int id, int score) : base(id, score)
    {

    }

    // Chooses a next point by applying a method to remove from the given playing field
    public void ChoosePoint()
    {
        // Apply method from settings here
        // @Jurrien van Winden
    }

    // Greedy method to remove a point from a convex hull, maximizing the area lost (score gained)
    public void greedyMethod()
    {
        // @Jurrien van Winden
    }

    // Randomly pick a point to remove
    public void randomMethod()
    {
        // @Jurrien van Winden
    }
}