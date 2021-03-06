﻿using UnityEngine;
using System;

// a cell in the maze
public class MazeCell : MonoBehaviour {
	// variables defining the cell
	public IntVector2 coordinates;
	private MazeCellEdge[] edges = new MazeCellEdge[MazeDirections.numDirections];
	private int initializedEdges;
	public MazeRoom room;
	public Material lavaMat;
	float offset = 0;
	
	// create the cell and add the materials 
	public void Initialize (MazeRoom room) {
		room.Add(this);
		transform.GetChild(0).GetComponent<Renderer>().material = room.roomSettings.floorMaterial;
	}
	
	// update material
	public void updateMaterial (MazeRoom room) {
		transform.GetChild(0).GetComponent<Renderer>().material = room.roomSettings.floorMaterial;
	}
	
	// change material
	public void changeMaterial (Material mat) {	
		transform.GetChild(0).GetComponent<Renderer>().material = mat;
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
	
	// During creation of the maze, return a direction that has not been created yet
	public MazeDirection UninitializedDirection (int seedNumber) {
			// set number of "skips" for open edges
			float generatedNoise = Mathf.PerlinNoise(coordinates.x/(float)seedNumber, coordinates.z/(float)seedNumber);
			int numSkipEdge = (int)(generatedNoise * 1000) % 4;
			int potentialSkips = numSkipEdge - initializedEdges;
			if (potentialSkips < 0){
				numSkipEdge = 0;
			}
			else{
				numSkipEdge = potentialSkips;
			}
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
	
	void Update(){
		// if lava then move it
		if (transform.GetChild(0).GetComponent<Renderer>().sharedMaterial == lavaMat){
			offset += 0.005f;
			transform.GetChild(0).GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", new Vector2(offset, 0));
		}
	}
}