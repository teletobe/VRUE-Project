using UnityEngine;
using Photon.Pun;

public class ObjectChecker : MonoBehaviour, IPunObservable
{
    public bool tasksAchieved;
    public GameObject objectToBuild1;
    public bool object1Found;
    public GameObject objectToBuild2;
    public bool object2Found;
    public GameObject objectToBuild3;
    public bool object3Found;

    public GameObject leftController;
    public GameObject rightController;

    public GameObject winScreen;
    public GameObject walls;
    public GameObject teleportArea;

    private GameObject[] createdObjects;

    // Start is called before the first frame update
    void Start()
    {
        tasksAchieved = false;
    }

    // for debugging
    /*
    void Update()
    {
        CheckAllTasksAchieved();
    }
    */

    private void OnTriggerStay(Collider other)
    {
        // only check if not found already
        // otherwise it will be overwritten
        if (!object1Found)
        {
            object1Found = ColliderChecker(other, objectToBuild1);
        }
        if (!object2Found)
        {
            object2Found = ColliderChecker(other, objectToBuild2);
        }
        if (!object3Found)
        {
            object3Found = ColliderChecker(other, objectToBuild3);
        }
        CheckAllTasksAchieved();
    }

    private bool ColliderChecker(Collider other, GameObject objectToBuild)
    {
        bool value = false;
        CreatedObject objectToBuildScript = objectToBuild.GetComponent<CreatedObject>();
        CreatedObject otherScript = other.gameObject.GetComponent<CreatedObject>();

        // check if colour is correct
        if (objectToBuildScript.GetColour() == otherScript.GetColour())
        {
            // check if shape is correct
            if (objectToBuildScript.GetShape() == otherScript.GetShape())
            {
                // check if size is correct
                // round to two decimals
                if (Mathf.Round(objectToBuild.gameObject.transform.localScale.x * 100f) / 100f == Mathf.Round(other.gameObject.transform.localScale.x * 100f) / 100f )
                {
                    // send impulse
                    leftController.GetComponent<HapticFeedbackOnHover>().StartHapticPulse();
                    rightController.GetComponent<HapticFeedbackOnHover>().StartHapticPulse();

                    value = true;
                }
            }
        }
        return value;
    }

    
    void CheckAllTasksAchieved()
    {
        // Check if all three tasks are achieved
        if (object1Found == true && object2Found == true && object3Found == true)
        {
            tasksAchieved = true;
            Debug.Log("All tasks achieved!");

            // set objects active or inactive
            //walls.SetActive(false);
            winScreen.SetActive(true);
            teleportArea.SetActive(true);
        }
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsReading)
        {
            tasksAchieved = (bool)stream.ReceiveNext();
        }
        else if (stream.IsWriting)
        {
            stream.SendNext(tasksAchieved);
        }
    }
}
