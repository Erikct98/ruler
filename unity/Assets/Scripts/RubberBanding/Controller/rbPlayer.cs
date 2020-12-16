
// Class to accomodate player related variables and methods.
// @Jurrien van Winden
using UnityEngine;

public class rbPlayer : MonoBehaviour
{

    public int id;
    public int score;
    public rbPlayer(int id, int score)
    {
        this.id = id;
        this.score = score;
    }
}