namespace RubberBanding {
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
		private rbPlayer player;
		private rbPlayer opponent;

		// Current convex hull
		internal rbConvexHull rb_convexHull;

		// List which contains all points in the current level
		internal List<rbPoint> rb_points;

		internal List<rbSegment> rb_segments;

		// ID of the player whose current turn it is
		internal int turn;

        private List<rbLevel> rb_levels;
		[SerializeField]

		void Start () {
			// Init point set from level


			// Init players
			player = new rbPlayer(0, 0);
			opponent = new rbPlayer(1, 0);
			// Set it to be first player turn
			turn = player.id;

			// Start game
			InitGame();

		}

		void InitGame() 
		{
			// clear old level

			// pick a level
			// int levelIndex = Random.Range(0, rb_levels.Count);

            // // initialize point set from level
            // foreach (var point in rb_levels[levelIndex].Points)
            // {
            //     var obj = Instantiate(rb_pointPrefab, point, Quaternion.identity) as GameObject;
            //     obj.transform.parent = this.transform;
            //     instantiatedObjects.Add(obj);
            // }

            // create point set
            this.rb_points = FindObjectsOfType<rbPoint>().ToList();
			this.rb_convexHull = new rbConvexHull();
			
            // compute convex hull
            this.rb_convexHull.convexHull = this.rb_convexHull.BuildConvexHull(rb_points);
			Debug.Log(this.rb_convexHull.convexHull);
			Debug.Log(this.rb_points);

		}
		
		// Update is called once per frame
		void Update () {
			
			if (this.rb_chosenPoint != null) {
				Debug.Log(this.rb_chosenPoint);
				Debug.Log("Callleeeed");
				// Delete this point
				// RemovePoint(rb_chosenPoint);
				this.rb_convexHull.UpdateConvexHull(this.rb_chosenPoint);
				this.rb_chosenPoint = null;
				// Add scores and such, update hull

				// Check if this move ends the game
				if(CheckGameOver()) {
					Debug.Log("End game");
					// Do endscreen stuff, show winner, scores
				} else {
					// Reset state, switch player turn state
					NextTurn();
				}
			}
		}

		void NextTurn()
		{
			Debug.Log("Next turn");
			// @Jurrien van Winden
			if(turn == player.id) {
   				turn = opponent.id;
			} else {
				this.turn = player.id;
			}
			this.rb_chosenPoint = null;
		}

		// Removes a point from the level
		bool RemovePoint(rbPoint point)
		{
			// @Jurrien van Winden
			point.Removed = true;

			// Update convex hull
			return this.rb_convexHull.RemovePoint(point);
		}
		// Checks whether all points have been deleted except 2, AKA no pegs can be removed anymore and the game should end
		// Or we could check if we have a convex hull of size <= 2 ?
		bool CheckGameOver()
		{
			return this.rb_convexHull.convexHull.Count() <= 2;
		}

		public void AddSegment(rbPoint rb_point_1, rbPoint rb_point_2)
        {
            var segment = new rbSegment(new Util.Geometry.LineSegment(rb_point_1.Pos, rb_point_2.Pos));

            rb_segments.Add(segment);

            // instantiate new road mesh
            var roadmesh = Instantiate(rb_roadMeshPrefab, Vector3.forward, Quaternion.identity) as GameObject;
            roadmesh.transform.parent = this.transform;
            instantiatedObjects.Add(roadmesh);

            // roadmesh.GetComponent<rbSegment>().segment = segment;

            //var roadmeshScript = roadmesh.GetComponent<ReshapingMesh>();
            // roadmeshScript.CreateNewMesh(rb_point_1.transform.position, rb_point_2.transform.position);
            
        }

        public void RemoveSegment(rbSegment rb_segment)
        {
        }
	}	
}

