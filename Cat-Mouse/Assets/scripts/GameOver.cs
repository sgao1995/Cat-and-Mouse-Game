﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Collections;

public class GameOver : MonoBehaviour {
    float timer;
    public Text text;
	// Use this for initialization
	void Start () {
        text= GameObject.Find("Text").GetComponent<Text>();
        transform.GetComponent<PhotonView>().RPC("updateWinText", PhotonTargets.AllBuffered);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > 15.0)
        {
            transform.GetComponent<PhotonView>().RPC("LoadLobby", PhotonTargets.AllBuffered);
        }
    }
    [PunRPC]
    void updateWinText()
    {
        string winner = GameObject.Find("WinObj").GetComponent<WinScript>().getWinner();
        text.text = winner + " wins!";
    }
    [PunRPC]
    void LoadLobby()
    {
        /*   if (!PhotonNetwork.isMasterClient)
           {
               Debug.LogError("PhotonNetwork : only master client can load level");
           }*/
        PhotonNetwork.isMessageQueueRunning = true;
        PhotonNetwork.automaticallySyncScene = true;
        Debug.Log("PhotonNetwork : Loading Level : ");
        SceneManager.LoadScene("lobby", LoadSceneMode.Single);
    }
}