﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Photon.PunBehaviour {
    public Maze mazePrefab;
    private Maze mazeInstance;
    List<MonsterSpawn> monsterSpawnList = new List<MonsterSpawn>();
    SpawnC[] sc;
    SpawnM[] sm;
    List<int> allPuzzleTypes = new List<int>();
    List<int> activePuzzleTypes = new List<int>();
    public BGM music;
	public MonsterSpawn mSpawn;

    private	void Start () {
		music = GameObject.Find("audBGM").GetComponent<BGM>();
		music.fadeOut = true;
        //s = GameObject.FindObjectsOfType<Spawn>();
        sc = GameObject.FindObjectsOfType<SpawnC>();
        sm = GameObject.FindObjectsOfType<SpawnM>();
        PhotonNetwork.isMessageQueueRunning = true;
        PhotonNetwork.automaticallySyncScene = true;
		// create the monster spawn locations
		for (int i = 0; i < 5; i++){
			for (int j = 0; j < 5; j++){
				Vector3 spawnPos = new Vector3(40f - i*20f, 0, 40f - j*20f);
				Quaternion spawnRot = new Quaternion(0f, 0f, 0f, 0f);
				MonsterSpawn newSpawn = Instantiate(mSpawn) as MonsterSpawn;
				mSpawn.transform.position = spawnPos;
				mSpawn.transform.rotation = spawnRot;
				monsterSpawnList.Add(newSpawn);
			}
		}
		
        for (int p = 0; p < 6; p++)
        {
            allPuzzleTypes.Add(p);
        }
        for (int p = 0; p < 6; p++)
        {
            int getPuzzle = Random.Range(0, allPuzzleTypes.Count);
            activePuzzleTypes.Add(allPuzzleTypes[getPuzzle]);
            allPuzzleTypes.RemoveAt(getPuzzle);
            Debug.Log(activePuzzleTypes);
        }
        SpawnMaze();
        if(GameObject.Find("TeamSelectionOBJ").GetComponent<teamselectiondata>().playertype == 0)
        {
            SpawnCat();
        }else
        {
            SpawnMouse();
        }
		// spawn basic monsters and elite monsters
        for (int i = 0; i< monsterSpawnList.Count; i++)
        {
            SpawnMonsters(i);
        }
		// spawn boss monster
		SpawnBoss();
		
		SpawnKeysAndChests();
        GameObject.Find("Timer").GetComponent<Timer>().enabled = true;
    }
    void OnGUI()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            GUILayout.BeginArea(new Rect(10, 10, 100, 100));
            if (GUILayout.Button("EXITGAME"))
            {
                LeaveRoom();
            }
            GUILayout.EndArea();
        }
    }
    void OnLeftRoom()
    {
        SceneManager.LoadScene("lobby", LoadSceneMode.Single);
    }
    void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.isMessageQueueRunning = false;
    }
    void SpawnMaze()
    {
		mazeInstance = Instantiate(mazePrefab) as Maze;
		var mazeScript = mazeInstance.GetComponent<Maze>();
		if (mazeScript != null)
		{
			mazeScript.StartMazeCreation();
		}
        if (PhotonNetwork.isMasterClient)
        {
            List<int> tempTypes = new List<int>();
            tempTypes.Add(5);
            tempTypes.Add(3);
            tempTypes.Add(2);
            mazeInstance.GeneratePuzzles(tempTypes);
            Debug.Log(tempTypes[0] + " " + tempTypes[1] + " " + tempTypes[2]);
            mazeInstance.GenerateChestLocations();
            //mazeInstance.GeneratePuzzles(activePuzzleTypes);
        }
    }
    void SpawnCat()
    {
        SpawnC mys = sc[0];
        GameObject myCat = (GameObject)PhotonNetwork.Instantiate("Cat", mys.transform.position, mys.transform.rotation, 0);
        myCat.GetComponent<CatMovement>().enabled = true;
        myCat.transform.FindChild("CatCam").gameObject.SetActive(true);
        //myCat.GetComponent<NetworkPlayer>().enabled = false;
        //enables minimap:
        myCat.GetComponent<Minimap>().enabled = true;
    }

    void SpawnMouse()
    {
        SpawnM mys = sm[Random.Range(0, 2)];
        GameObject myMouse = (GameObject)PhotonNetwork.Instantiate("Mouse", mys.transform.position, mys.transform.rotation, 0);
        myMouse.GetComponent<MouseMovement>().enabled = true;
        myMouse.transform.FindChild("MouseCam").gameObject.SetActive(true);
        //myMouse.GetComponent<NetworkPlayer>().enabled = false;
        //enables minimap:
        myMouse.GetComponent<Minimap>().enabled = true;
    }

    void SpawnMonsters(int spawn)
    {
        if (PhotonNetwork.isMasterClient)
        {
			MonsterSpawn monsterSpawn = monsterSpawnList[spawn];
			int formation = Random.Range(0, 3);
			// 1 normal monster
			if (formation == 0){
				GameObject monsterGO = (GameObject)PhotonNetwork.Instantiate("Monster", monsterSpawn.transform.position, monsterSpawn.transform.rotation, 0);
				monsterGO.GetComponent<MonsterAI>().enabled = true;
				MonsterAI monster = monsterGO.GetComponent<MonsterAI>();
				monster.setMonsterType("Monster");
			}
			// 2 normal monsters
			else if (formation == 1){
				for (int i = 0; i < 2; i++){
					GameObject monsterGO = (GameObject)PhotonNetwork.Instantiate("Monster", monsterSpawn.transform.position, monsterSpawn.transform.rotation, 0);
					monsterGO.GetComponent<MonsterAI>().enabled = true;
					MonsterAI monster = monsterGO.GetComponent<MonsterAI>();
					monster.setMonsterType("Monster");
				}
			}
			// 1 normal monster and 1 elite monster
			else if (formation == 2){
				GameObject monsterGO = (GameObject)PhotonNetwork.Instantiate("Monster", monsterSpawn.transform.position, monsterSpawn.transform.rotation, 0);
				monsterGO.GetComponent<MonsterAI>().enabled = true;
				MonsterAI monster = monsterGO.GetComponent<MonsterAI>();
				monster.setMonsterType("Monster");
			
				GameObject monsterGO2 = (GameObject)PhotonNetwork.Instantiate("MonsterElite", monsterSpawn.transform.position, monsterSpawn.transform.rotation, 0);
				monsterGO2.GetComponent<MonsterAI>().enabled = true;
				MonsterAI monster2 = monsterGO2.GetComponent<MonsterAI>();
				monster2.setMonsterType("MonsterElite");
			}
        }
    }
	
	void SpawnBoss(){
		// pick a random spawn to spawn at
		int location = Random.Range(0, monsterSpawnList.Count);
		MonsterSpawn bossSpawn = monsterSpawnList[location];
		GameObject monsterGO = (GameObject)PhotonNetwork.Instantiate("Boss", bossSpawn.transform.position, bossSpawn.transform.rotation, 0);
		monsterGO.GetComponent<MonsterAI>().enabled = true;
		MonsterAI monster = monsterGO.GetComponent<MonsterAI>();
		monster.setMonsterType("Boss");
	}
	
	// spawn the keys and chests in the puzzle rooms
	void SpawnKeysAndChests()
	{
		// spawn each key and chest
        if (PhotonNetwork.isMasterClient)
        {
            List<float> keyLocations = mazeInstance.getKeySpawns();
            List<float> chestLocations = mazeInstance.getChestSpawns();
            for (int i = 0; i < 6; i += 2)
            {
                Vector3 keyPos = new Vector3(keyLocations[i], 1, keyLocations[i + 1]);
                Quaternion keyRot = new Quaternion(0f, 0f, 0f, 0f);
                GameObject key = (GameObject)PhotonNetwork.Instantiate("Key", keyPos, keyRot, 0);
                Vector3 chestPos = new Vector3(chestLocations[i], 0.35f, chestLocations[i + 1]);
                Quaternion chestRot = new Quaternion(0f, 0f, 0f, 0f);
                GameObject chest = (GameObject)PhotonNetwork.Instantiate("Chest", chestPos, chestRot, 0);
				Chest newChest = chest.GetComponent<Chest>();
				newChest.whichPieceInside = (i/2)+1;
            }
        }
	}
  
}