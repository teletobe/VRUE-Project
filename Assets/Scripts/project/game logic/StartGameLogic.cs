using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.XR.CoreUtils;


public class StartGameLogic : MonoBehaviour, IPunObservable
{
    public bool isPlaying = false;
    public bool isReset = false;
    public bool localisReset = false;

    private GameObject[] startButtons;
    private GameObject[] buttonWalls;

    // for visibility
    public GameObject gameTable;
    public GameObject player2Uis;
    public Game1UiRenderer game1UiRenderer;

    // for haptic feedback
    public GameObject leftController;
    public GameObject rightController;

    // for reset
    public XROrigin xrOrigin = null;
    public GameObject networkManager;
    private NetworkPlayerSpawner networkPlayerSpawnerScript;
    public GameObject checkerPlate;
    private ObjectChecker objectCheckerScript;

    private GameObject[] players;

    // Start is called before the first frame update
    void Start()
    {
        gameTable.SetActive(false);
        player2Uis.SetActive(false);

        networkPlayerSpawnerScript = networkManager.GetComponent<NetworkPlayerSpawner>();
        objectCheckerScript = checkerPlate.GetComponent<ObjectChecker>();   
    }

    void Update()
    {
        if (isReset && !localisReset)
        {
            ResetGame();
        }
        if (isReset)
        {
            checkBothIsReset();
        }

        // check if player touches button at the same time
        if (!isPlaying)
        {
            buttonWalls = GameObject.FindGameObjectsWithTag("ButtonWall");
            if (buttonWalls != null && buttonWalls.Length != 0)
            {
                int counter = 0;
                foreach (GameObject btnWall in buttonWalls)
                {
                    if (btnWall.GetComponent<ButtonController>().isTouched)
                    {
                        counter++;
                    }
                }

                if (counter == PhotonNetwork.PlayerList.Length)
                {
                    StartGame();
                    isPlaying = true;
                }
            }
        }
    }


    private void StartGame()
    {
        Debug.Log("Start Game");
        game1UiRenderer.gameHasStarted = true;

        // send impulse
        leftController.GetComponent<HapticFeedbackOnHover>().StartHapticPulse();
        rightController.GetComponent<HapticFeedbackOnHover>().StartHapticPulse();

        PlaceGameObjects();
    }

    private void PlaceGameObjects()
    {
        gameTable.SetActive(true);
        player2Uis.SetActive(true);

        startButtons = GameObject.FindGameObjectsWithTag("StartButton");
        foreach (GameObject btn in startButtons)
        {
            btn.SetActive(false);
        }
        foreach (GameObject btnWall in buttonWalls)
        {
            btnWall.GetComponent<ButtonController>().isTouched = false;
        }
    }

    private void ResetGame()
    {
        // reset visibilities
        gameTable.SetActive(false);
        player2Uis.SetActive(false);
        foreach (GameObject btn in startButtons)
        {
            btn.SetActive(true);
        }

        // reset player position
        xrOrigin.transform.position = networkPlayerSpawnerScript.playerPosition;

        // reset helper object position
        GameObject helperObject = networkPlayerSpawnerScript.GetHelperObject();
        if (helperObject != null)
        {
            helperObject.transform.position = networkPlayerSpawnerScript.helperPosition;
        }

        // reset tasks achieved
        objectCheckerScript.tasksAchieved = false;

        // reset game status
        isPlaying = false;
        localisReset = true;

    }

    public void checkBothIsReset()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        int counter = 0;
        foreach (GameObject player in players)
        {
            if (player.GetComponent<NetworkPlayerScript>().status == PlayerStatus.hasRestarted)
            {
                counter++;
            }
        }
        if (counter == PhotonNetwork.PlayerList.Length)
        {
            isReset = false;
        }
    }

    public void setResetGame(bool value)
    {
        isReset = value;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsReading)
        {
            isReset = (bool)stream.ReceiveNext();
        }
        else if (stream.IsWriting)
        {
            stream.SendNext(isReset);
        }
    }


    /*
    private void ResetGame()
    {

        // TODO RESET EVERYTHING TO BEGINNING... ?
        PlaceGameObjects();

        leftStartButton.SetActive(false);
        rightStartButton.SetActive(false);
        leftResetButton.SetActive(true);
        rightResetButton.SetActive(true);
        leftStartReady = false;
        rightStartReady = false;
        leftStartReady = false;
        rightStartReady = false;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        startReady[0] = leftStartReady;
        startReady[1] = rightStartReady;

        if (stream.IsReading)
        {
            startReady = (bool[])stream.ReceiveNext();

            leftStartReady = startReady[0];
            rightStartReady = startReady[1];
        }
        else if (stream.IsWriting)
        {

            stream.SendNext(startReady);
        }
    }*/

}
