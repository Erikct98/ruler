using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rbLevel : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	    /*
         * TODO: paint new elements?
         */
	}

    void GenerateGame(int seed)
    {
        /*
         * TODO: initialize a new game based on the given seed
         * This includes:
         * - Generating a random set of points in the XY plane
         * - Initialize the convex hull
         * - Build the range tree based on the points
         */
    }

    void UpdateConvexHull(rbPoint p)
    {
        /*
         * TODO: check if p in CH. 
         * If so, update CH and score.
         * remove p from RQT and from board.
         */
    }

    double ComputeScore(rbPoint one, rbPoint two, rbPoint three)
    {
        /*
         * TODO: compute the surface of the triangle described
         * by one, two and three
         */
        return 0.0;
    }
}
