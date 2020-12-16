namespace RubberBanding {
	using System.Collections.Generic;
	using UnityEngine;

	public class rbController : MonoBehaviour {

		public LineRenderer rb_line;
		[SerializeField]
		private GameObject rb_roadMeshPrefab;
		[SerializeField]
		private List<rbPlayer> rb_players;

		// Current convex hull
		internal rbConvexHull rb_convexHull;

		// List which contains all points in the current level
		internal List<rbPoint> rb_points;

		// The last clicked rbPoint by the player / AI 
		public rbPoint rb_chosenPoint = null;
		[SerializeField]

		// Use this for initialization
		void Start () {
			// get unity objects
            // rb_points = FindObjectsOfType<rbPoint>().ToList();

			// Init players
			rb_players.Add(new rbPlayer(0, 0));
			rb_players.Add(new rbPlayer(1, 0));

			// Make initial convex hull
			rb_convexHull = new rbConvexHull();
			rb_convexHull.BuildConvexHull(rb_points);

			// Start game
			

		}
		
		// Update is called once per frame
		void Update () {
			if (rb_chosenPoint != null) {
				// Delete this point
				RemovePoint(rb_chosenPoint);
				
				// Add scores and such, update hull
				
				// Check if this move ends the game
				CheckGameOver();
				// Reset state, switch player turn state
				NextTurn();
				
			}
		}

		void NextTurn()
		{
			// @Jurrien van Winden
			rb_chosenPoint = null;
		}

		// Removes a point from the level
		bool RemovePoint(rbPoint point)
		{
			// @Jurrien van Winden
			point.deleted = true;

			// Update convex hull
			return rb_convexHull.RemovePoint(point);
		}
		// Checks whether all points have been deleted except 2, AKA no pegs can be removed anymore and the game should end
		// Or we could check if we have a convex hull of size <= 2 ?
		void CheckGameOver()
		{
			// implement
		}
	}
		
}

