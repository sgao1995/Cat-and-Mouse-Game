﻿using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour {
	public Transform hinge;
	public bool chestOpen = false;
	public bool chestOpening = false;
	public float duration = 30f;
	float newRot = 0;
	private int whichPieceInside;
	public bool puzzlePieceSpawned = false;	
	public PuzzlePiece[] puzzlePiecePrefabs;
	
	public void setWhichPieceInside(int wp){
		whichPieceInside = wp;
	}
	
	// open the chest
	public int Interact(){
		if (chestOpen == false && chestOpening == false){
			chestOpening = true;
			Debug.Log("open chest");
			return 1;
		}
		return 0;
	}
	
	// animate chest opening
	void Update(){
		if (chestOpening){
			newRot += 70f/duration;
			if (newRot >= 70f + 70f/duration){
				chestOpening = false;
				chestOpen = true;
			}
			Quaternion newQuat = Quaternion.Euler(newRot, 0f, 0f);
			hinge.localRotation = newQuat;
		}
		if(chestOpen && !puzzlePieceSpawned){
			Vector3 spawnPos = new Vector3(this.transform.position.x, -1f, this.transform.position.z);
			Quaternion spawnRot = new Quaternion(0f, 0f, 0f, 0f);
			string whichPiece = "PuzzlePiece" + whichPieceInside;
			GameObject newGO = (GameObject)PhotonNetwork.InstantiateSceneObject(whichPiece, spawnPos, spawnRot, 0);
			puzzlePieceSpawned = true;
			PuzzlePiece newPiece = newGO.GetComponent<PuzzlePiece>();
			newPiece.pieceID = whichPieceInside;
		}
	}
}
