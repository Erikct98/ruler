namespace RubberBanding {
	using System.Collections.Generic;
	using UnityEngine;
    using System.Linq;
	using Util.Geometry;
	using General.Model;

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

		internal List<LineSegment> rb_segments;

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
			Debug.Log(this.rb_points.Count());

			this.rb_segments = new List<LineSegment>();
			this.rb_convexHull = new rbConvexHull();
			
            // compute convex hull
            this.rb_convexHull.convexHull = this.rb_convexHull.BuildConvexHull(rb_points);
			// Draws the currently stored convex hull
			DrawConvexHull();
		}
		
		// Update is called once per frame
		void Update () {
			
			if (this.rb_chosenPoint != null) {
				// Delete this point, recompute hull, compute score
				int score = this.rb_convexHull.UpdateConvexHull(this.rb_chosenPoint);
				UpdateScore(score);

				// Reset chosen point
				this.rb_chosenPoint = null;

				// Redraw the updated hull
				DrawConvexHull();

				// Check if this move ends the game
				if(CheckGameOver()) {
					Debug.Log("End game");
					// SceneManager.LoadScene(m_victoryScene);
					// Do endscreen stuff, show winner, scores
				} else {
					// Switch player turn state
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
		}

		// Checks whether all points have been deleted except 2, AKA no pegs can be removed anymore and the game should end
		// Or we could check if we have a convex hull of size <= 2 ?
		bool CheckGameOver()
		{
			return this.rb_convexHull.convexHull.Count() <= 2;
		}

		// Adds the score value to the score of the player whose turn it is
		public void UpdateScore(int score) {
			if(turn == player.id) {
   				player.score += score;
			} else {
				opponent.score += score;
			}
		}

		public void DrawConvexHull() {
			ClearConvexHullDrawing();
			rbPoint prev = null;
			rb_convexHull.convexHull.ForEach((point) => {
				Debug.Log(point);
				if (prev != null) {
					AddSegment(prev, point);
				}
				prev = point;
			});
			// Add line segment between first and last element as well
			AddSegment(rb_convexHull.convexHull[0], rb_convexHull.convexHull[rb_convexHull.convexHull.Count - 1]);
		}

		public void AddSegment(rbPoint rb_point_1, rbPoint rb_point_2)
        {

			var segment = new Util.Geometry.LineSegment(rb_point_1.Pos, rb_point_2.Pos);

            rb_segments.Add(segment);

            // instantiate new road mesh
            var mesh = Instantiate(rb_roadMeshPrefab, Vector3.forward, Quaternion.identity) as GameObject;
            mesh.transform.parent = this.transform;
			mesh.tag = "Segment";
            instantiatedObjects.Add(mesh);

            mesh.GetComponent<rbSegment>().segment = segment;

            var meshScript = mesh.GetComponent<ReshapingMesh>();
            meshScript.CreateNewMesh(rb_point_1.transform.position, rb_point_2.transform.position);
            
        }

        public void RemoveSegment(rbSegment rb_segment)
        {
			rb_segments.Remove(rb_segment.segment);
			// Dunno if needed?
			Destroy(gameObject);
        }

		public void ClearConvexHullDrawing()
		{	
			rb_segments.Clear();
			// destroy line segments in level
            foreach (var obj in instantiatedObjects)
            {
				if (obj != null && obj.tag == "Segment") {
					// Must be immediate as the controller starts adding objects afterwards
                	DestroyImmediate(obj);
				}
            }
		}
	}	
}

