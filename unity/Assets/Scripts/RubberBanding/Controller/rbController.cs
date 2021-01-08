namespace RubberBanding {
	using System.Collections.Generic;
	using UnityEngine;
    using System.Linq;
	using Util.Geometry;
	using General.Model;
	using UnityEngine.UI;
	using UnityEngine.SceneManagement;

	public class rbController : MonoBehaviour {

		// The last clicked rbPoint by the player / AI 
		public rbPoint rb_chosenPoint = null;
		[SerializeField]
		public LineRenderer rb_line;
		[SerializeField]
		private GameObject rb_SegmentMeshPrefab;
		[SerializeField]
		private GameObject rb_PreviewSegmentMeshPrefab;
		[SerializeField]

		private string rb_victoryScene;
		[SerializeField]
		private GameObject rb_pointPrefab;
        [SerializeField]
		public GameObject rb_player_1_score;
		[SerializeField]

		public GameObject rb_player_2_score;
		[SerializeField]

		private List<rbLevel> rb_levels;
		[SerializeField]

		// List of instantiated objects for a level
		private List<GameObject> instantiatedObjects;
		public rbPlayer player;
		public rbPlayer opponent;

		// Current convex hull
		internal rbConvexHull rb_convexHull;

		// List which contains all points in the current level
		internal List<rbPoint> rb_points;

		internal List<LineSegment> rb_segments;

		// ID of the player whose current turn it is
		internal int turn;

        

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
			// Debug.Log(rb_levels.Count);
			int levelIndex = Random.Range(0, rb_levels.Count);
			Debug.Log(levelIndex);

            // initialize point set from level
            foreach (var point in rb_levels[levelIndex].Points)
            {
                var obj = Instantiate(rb_pointPrefab, point, Quaternion.identity) as GameObject;
                obj.transform.parent = this.transform;
                instantiatedObjects.Add(obj);
            }
            // create point set
            this.rb_points = FindObjectsOfType<rbPoint>().ToList();

			this.rb_segments = new List<LineSegment>();
			this.rb_convexHull = new rbConvexHull();

			rb_player_1_score.GetComponent<Text>().text = "0";
			rb_player_2_score.GetComponent<Text>().text = "0";
			
            // compute convex hull
            this.rb_convexHull.convexHull = this.rb_convexHull.BuildConvexHull(rb_points);
			// Draws the currently stored convex hull
			DrawConvexHull(this.rb_convexHull.convexHull);
		}
		
		// Update is called once per frame
		void Update () {
			
			if (this.rb_chosenPoint != null) {
				clearPreview();
                // Find which points are considered for CH
                List<rbPoint> consideredPoints = rb_convexHull.GetReplacements(rb_chosenPoint);

				// Delete this point, recompute hull, compute score
				int score;
				List<rbPoint> area;
				if (this.rb_convexHull.convexHull.IndexOf(this.rb_chosenPoint) != -1) {

				}
				this.rb_convexHull.UpdateConvexHull(this.rb_chosenPoint, true, out score, out area);
				if (area != null) {

				}
				Debug.Log(score);
				Debug.Log(area.Count());
				UpdateScore(score);

				// Reset chosen point
				this.rb_chosenPoint = null;

				// Redraw the updated hull
				DrawConvexHull(this.rb_convexHull.convexHull);

				// Check if this move ends the game
				if (CheckGameOver()) {
					Debug.Log("End game");
					if (player.score >= opponent.score) {
						SceneManager.LoadScene("rbVictory_1");
					} else {
						SceneManager.LoadScene("rbVictory_2");
					}
					return;
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
				rb_player_1_score.GetComponent<Text>().text = player.score.ToString("F0");
			} else {
				opponent.score += score;
				rb_player_2_score.GetComponent<Text>().text = opponent.score.ToString("F0");
			}
		}

		public void DrawConvexHull(List<rbPoint> consideredPoints) {
			ClearConvexHullDrawing();
			rbPoint prev = null;
			LineSegment newSegment;
			rb_convexHull.convexHull.ForEach((point) => {
				//Debug.Log(point);
				if (prev != null) {
					AddSegment(prev, point, out newSegment);
					AddSegmentMesh(newSegment, "Segment");
				}
				prev = point;
			});
			// Add line segment between first and last element as well
			AddSegment(rb_convexHull.convexHull[0], rb_convexHull.convexHull[rb_convexHull.convexHull.Count - 1], out newSegment);
			AddSegmentMesh(newSegment, "Segment");

            // Add line above all considered points
            // consideredPoints.ForEach((point) => {
            //     AddSegment(point.Pos, point.Pos + new Vector2(0, 25), out newSegment);
			// 	AddSegmentMesh(newSegment, "Segment");
            // });
		}

		public void PreviewConvexHull(rbPoint previewPoint) {
			int score;
			List<rbPoint> area;
			LineSegment newSegment;
			var index = this.rb_convexHull.convexHull.IndexOf(previewPoint);

			if (index != -1) {
				this.rb_convexHull.UpdateConvexHull(previewPoint, false, out score, out area);
				rbPoint prev = null;
				if (area != null) {
					area.ForEach((point) => {
						if (prev != null) {
							AddSegment(prev, point, out newSegment);
							AddSegmentMesh(newSegment, "PreviewSegment", rb_PreviewSegmentMeshPrefab);
						}
						prev = point;
					});
					// Debug.Log(area.Count);
					// Add line segment between first and last element as well
					AddSegment(area[0], area[area.Count - 1], out newSegment);
					AddSegmentMesh(newSegment, "PreviewSegment", rb_PreviewSegmentMeshPrefab);
				}
			}
			
		}

		

        public void AddSegment(rbPoint rb_point_1, rbPoint rb_point_2, out LineSegment segment)
        {
            AddSegment(rb_point_1.Pos, rb_point_2.Pos, out segment);
        }

        public void AddSegment(Vector2 one, Vector2 two, out LineSegment segment)
        {
			segment = new Util.Geometry.LineSegment(one, two);
            rb_segments.Add(segment);
        }

		private GameObject AddSegmentMesh(LineSegment rb_segment, string tag) {
			return AddSegmentMesh(rb_segment, tag, rb_SegmentMeshPrefab);
		}
		private GameObject AddSegmentMesh(LineSegment rb_segment, string tag, GameObject prefab) {
			// instantiate new road mesh
            var mesh = Instantiate(prefab, Vector3.forward, Quaternion.identity) as GameObject;
            mesh.transform.parent = this.transform;
			mesh.tag = tag;
            instantiatedObjects.Add(mesh);
			
            mesh.GetComponent<rbSegment>().segment = rb_segment;

            var meshScript = mesh.GetComponent<ReshapingMesh>();
            meshScript.CreateNewMesh(new Vector3(rb_segment.Point1.x, rb_segment.Point1.y, -1), new Vector3(rb_segment.Point2.x, rb_segment.Point2.y, -1));

			return mesh;
		}

        public void RemoveSegment(LineSegment rb_segment, List<LineSegment> list)
        {
			list.Remove(rb_segment);
        }

		public void ClearConvexHullDrawing()
		{	
			rb_segments.Clear();
			// Loop backwards so that we can destroy elements immediately
			for (var i = instantiatedObjects.Count - 1; i > -1; i--)
			{
				GameObject obj = instantiatedObjects[i];
				if (obj != null && (obj.tag == "Segment" || obj.tag == "PreviewSegment")) {
					// Must be immediate as the controller starts adding objects afterwards
					DestroyImmediate(obj);
				}
				if (obj == null) {
					instantiatedObjects.RemoveAt(i);
				}
			}
		}
		public void clearPreview(){
			// destroy preview line segments in level
			// Loop backwards so that we can destroy elements immediately
			for (var i = instantiatedObjects.Count - 1; i > -1; i--)
			{
				GameObject obj = instantiatedObjects[i];
				if (obj != null && obj.tag == "PreviewSegment") {
					// Must be immediate as the controller starts adding objects afterwards
					DestroyImmediate(obj);
				}
				if (obj == null) {
					instantiatedObjects.RemoveAt(i);
				}
			}
		}
	}	
}

