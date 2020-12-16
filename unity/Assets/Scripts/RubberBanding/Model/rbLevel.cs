namespace RubberBanding
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Data container for rubberbanding level containing initial point set and possibly initial convex hull.
    /// </summary>
    [CreateAssetMenu(fileName = "rubberbandingLevelNew", menuName = "Levels/Rubberbanding Level")]
    public class rbLevel : ScriptableObject
    {
        [Header("Point set")]
        public List<Vector2> Points = new List<Vector2>();

        [Header("Initial convex hull")]
        public List<Vector2> ConvexHull = new List<Vector2>();
    }
}