﻿using UnityEngine;

// a cell in the maze
public class MazeCell : MonoBehaviour {
	// variables defining the cell
	public IntVector2 coordinates;
	private MazeCellEdge[] edges = new MazeCellEdge[MazeDirections.numDirections];
	private int initializedEdges;
	public MazeRoom room;
	
	// create the cell and add the materials 
	public void Initialize (MazeRoom room) {
		room.Add(this);
		transform.GetChild(0).GetComponent<Renderer>().material = room.roomSettings.floorMaterial;
	}

	// check to see if the cell creation is complete
	public bool IsFullyInitialized {
		get {
			return initializedEdges == MazeDirections.numDirections;
		}
	}
	
	// create an edge for the cell
	public void SetEdge (MazeDirection direction, MazeCellEdge edge) {
		edges[(int)direction] = edge;
		initializedEdges += 1;
	}
	
	// return the cell edge for the given direction
	public MazeCellEdge GetEdge (MazeDirection direction) {
		return edges[(int)direction];
	}
	
	// During creation of the maze, return a random direction that has not been created yet
	public MazeDirection RandomUninitializedDirection {
		get {
			// random number of "skips" for open edges
			int numSkipEdge = Random.Range(0, 4 - initializedEdges);
			// skip until 0 skips left, then create edge
			for (int count = 0; count < 4; count++) {
				if (edges[count] == null) {
					if (numSkipEdge == 0) {
						return (MazeDirection)count;
					}
					numSkipEdge -= 1;
				}
			}
			throw new System.InvalidOperationException("Fully initialized");
		}
	}
}