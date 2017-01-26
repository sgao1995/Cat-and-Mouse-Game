﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class lobby : Photon.MonoBehaviour
{
    const string VER = "0.0.1";
    Spawn[] s;
    public Maze mazePrefab;
    private Maze mazeInstance;
    List<int> allPuzzleTypes = new List<int>();
    List<int> activePuzzleTypes = new List<int>();
    public string roomName;
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(VER);
        roomName = "Room " + Random.Range(0, 20);
        /*SceneManager.sceneLoaded += (scene, loadscene) =>
         {
             if (SceneManager.GetActiveScene().name == "catmousegame3")
             {
                 s = GameObject.FindObjectsOfType<Spawn>();
             }
         };*/
    }
    void SpawnMaze()
    {
        mazeInstance = Instantiate(mazePrefab) as Maze;
        var mazeScript = mazeInstance.GetComponent<Maze>();
        if (mazeScript != null)
        {
            mazeScript.StartMazeCreation();
        }
        mazeInstance.GeneratePuzzles(activePuzzleTypes);
    }
    void SpawnCat()
    {
        
        Spawn mys = s[Random.Range(0, s.Length)];
        GameObject myCat = (GameObject)PhotonNetwork.Instantiate("Cat", mys.transform.position, mys.transform.rotation, 0);
        myCat.GetComponent<CatMovement>().enabled = true;
        myCat.transform.FindChild("CatCam").gameObject.SetActive(true);
        //myCat.GetComponent<NetworkPlayer>().enabled = false;
        //enables minimap:
        myCat.GetComponent<Minimap>().enabled = true;
    }
    void SpawnMonsters()
    {

        Spawn monsterSpawn = s[1];
        GameObject monster = (GameObject)PhotonNetwork.Instantiate("Monster", monsterSpawn.transform.position, monsterSpawn.transform.rotation, 0);
        monster.GetComponent<MonsterAI>().enabled = true;
    }
    void OnGUI()
    {
    }
    void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
        Invoke("Refresh", 0.1f);
        Refresh();
    }
    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("OnPhotonRandomJoinFailed");
        PhotonNetwork.CreateRoom(null);
    }
    void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        PhotonNetwork.LoadLevel("catmousegame3");
        //SpawnCat();
        //SpawnMonsters();
        //SpawnMaze();

    }

    private List<GameObject> roomPF = new List<GameObject>();
    public GameObject roomPreFab;
    //button events for lobby menu:
    public void ButtonEvents(string Event)
    {
        switch (Event)
        {
            case "CreateBtn":
                if (PhotonNetwork.JoinLobby())
                {
                    RoomOptions roomOpt = new RoomOptions();
                    roomOpt.MaxPlayers = 4;
                    PhotonNetwork.CreateRoom(roomName, roomOpt, TypedLobby.Default);
                }
                //PhotonNetwork.JoinRandomRoom();
                break;
            case "RefreshBtn":
                if (PhotonNetwork.JoinLobby())
                {
                    Refresh();
                }
                break;
            case "JoinBtn":
                PhotonNetwork.JoinRandomRoom();
                break;
        }
    }
    void Refresh()
    {
        if (roomPF.Count > 0)
        {
            for (int i = 0; i < roomPF.Count; i++)
            {
                Destroy(roomPF[i]);
            }
            roomPF.Clear();
        }
        for (int i = 0; i < PhotonNetwork.GetRoomList().Length; i++)
        {
            GameObject r = Instantiate(roomPreFab);
            r.transform.SetParent(roomPreFab.transform.parent);
            r.GetComponent<RectTransform>().localScale = roomPreFab.GetComponent<RectTransform>().localScale;
            r.GetComponent<RectTransform>().position = new Vector3(roomPreFab.GetComponent<RectTransform>().position.x, roomPreFab.GetComponent<RectTransform>().position.y - (i * 55), roomPreFab.GetComponent<RectTransform>().position.z);
            //r.transform.FindChild("RText").GetComponent<Text>().text = PhotonNetwork.GetRoomList()[i].name;
           
            r.SetActive(true);
            roomPF.Add(r);
        }
    }
    void join(string r)
    {
        PhotonNetwork.JoinRoom(r);
    }
}