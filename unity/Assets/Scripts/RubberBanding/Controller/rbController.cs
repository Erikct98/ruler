﻿namespace RubberBanding {
	using System.Collections.Generic;
	using UnityEngine;
    using System.Linq;

	public class rbController : MonoBehaviour {

		// The last clicked rbPoint by the player / AI 
		public rbPoint rb_chosenPoint = null;
		[SerializeField]
		public LineRenderer rb_line;
		[SerializeField]
		private GameObject rb_roadMeshPrefab;
		[SerializeField]

		private GameObject rb_pointPrefab;
        [SerializeField]

		// List of instantiated objects for a level
		private List<GameObject> instantiatedObjects;
		private List<rbPlayer> rb_players;

		// Current convex hull
		internal rbConvexHull rb_convexHull;

		// List which contains all points in the current level
		internal List<rbPoint> rb_points;

		// ID of the player whose current turn it is
		internal int turn;

        private List<rbLevel> rb_levels;
		[SerializeField]

		void Start () {
			// Init point set from level


			// Init players
			rb_players.Add(new rbPlayer(0, 0));
			rb_players.Add(new rbPlayer(1, 0));

			// Make initial convex hull
			rb_convexHull = new rbConvexHull();
			rb_convexHull.BuildConvexHull(rb_points);

			// Start game
			InitGame();

		}

		void InitGame() 
		{
			// clear old level

			// pick a level
			int levelIndex = Random.Range(0, rb_levels.Count);

            // initialize point set from level
            foreach (var point in rb_levels[levelIndex].Points)
            {
                var obj = Instantiate(rb_pointPrefab, point, Quaternion.identity) as GameObject;
                obj.transform.parent = this.transform;
                instantiatedObjects.Add(obj);
            }

            // create point set
            rb_points = FindObjectsOfType<rbPoint>().ToList();

            // compute convex hull
            rb_convexHull.BuildConvexHull(rb_points);

		}

		// Start the game so that the players can play
		void startGame()
		{
			// @Jurrien van Winden
		}
		
		// Update is called once per frame
		void Update () {
			if (rb_chosenPoint != null) {
				// Delete this point
				RemovePoint(rb_chosenPoint);
				
				// Add scores and such, update hull

				// Check if this move ends the game
				if(CheckGameOver()) {
					// Do endscreen stuff, show winner, scores
				} else {
					// Reset state, switch player turn state
					NextTurn();
				}
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
			point.Removed = true;

			// Update convex hull
			return rb_convexHull.RemovePoint(point);
		}
		// Checks whether all points have been deleted except 2, AKA no pegs can be removed anymore and the game should end
		// Or we could check if we have a convex hull of size <= 2 ?
		bool CheckGameOver()
		{
			if (rb_convexHull.convexHull.Count <= 2) {
				// We should end the game here
				return true;
			}
			return false;
		}
	}	
}

