using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;
using System;


public class BodyBasedSteering : MonoBehaviour
{
    public InputActionReference steeringReference = null;
    public Camera mainCamera = null;
    public XROrigin xrOrigin = null;
    public float speed = 0;
    public float bounceForce = 1f;


    public GameObject currentHelper;
    private bool isStandingOnHelper;
    Vector3 movementRestriction = new Vector3(1.0f, 0.0f, 1.0f);

    private bool isCollidingObstacle = false;
    private Collider collidingObject;

    private bool isOnPoison = true;

    void Update()
    {
        isStandingOnHelper = checkCollider("HelperObject");
        if (isStandingOnHelper && steeringReference.action.IsPressed())
        {
            if (!isOnPoison)
            {
                BounceBack();
            } else
            {
                Steering();
            }
        }      

    }

    private void Steering()
    {
        // if is not colliding against anything,  move in x and z direction
        // if collided, restriction during setCollidingObstacle()
        if (!isCollidingObstacle)
        {
            movementRestriction = new Vector3(1.0f, 0.0f, 1.0f);
        } 

        Vector3 deltaSteering = (Vector3.Scale(mainCamera.transform.forward, movementRestriction));

        currentHelper.transform.position += deltaSteering * speed * Time.deltaTime;
        xrOrigin.transform.position += deltaSteering * speed * Time.deltaTime;
    }

    private void BounceBack()
    {
        Vector3 closestPoint = collidingObject.ClosestPointOnBounds(currentHelper.transform.position);

        Vector3 bounceDirection = -closestPoint;  // Bounce in the opposite direction
        Vector3 bounceForceVector = bounceDirection * bounceForce;

        Vector3 deltaSteering = (Vector3.Scale(bounceForceVector, movementRestriction));
        currentHelper.transform.Translate(deltaSteering * Time.deltaTime, Space.World);
    }

    // check if player is standing on helper objects
    // if yes change color
    public bool checkCollider(string tag)
    {
        var offset = new Vector3(0, 2, 0);
        var localPoint0 = mainCamera.transform.position - offset;
        var localPoint1 = mainCamera.transform.position + offset;

        var colliders = Physics.OverlapCapsule(localPoint0, localPoint1, 0.1f);

        if (colliders.Length > 0)
        {
            foreach (Collider col in colliders)
            {
                if (col.CompareTag(tag))
                {
                    currentHelper = col.gameObject;

                    // set color 
                    Color customColor = new Color(0.6f, 1, 0.6f, 0.7f);
                    col.gameObject.GetComponent<Renderer>().material.SetColor("_Color", customColor);

                    return true;
                } else
                {

                    // reset color 
                    GameObject[] helpers = GameObject.FindGameObjectsWithTag(tag);
                    if (helpers.Length != 0)
                    {
                        foreach (GameObject helper in helpers)
                        {
                            Color customColor = new Color(0.6f, 0.9f, 1, 0.7f);
                            helper.GetComponent<Renderer>().material.SetColor("_Color", customColor);
                        }
                    }
                }
            }
        }
         return false;
    }

    public void setIsCollidingObstacle(bool value)
    {
        isCollidingObstacle = value;
    }

    public void setCollidingObject(Collider collider)
    {
        collidingObject = collider;
    }

    public void setHelperObject(GameObject helperObjet)
    {
        currentHelper = helperObjet;

    }
    public void setIsOnPoison(bool value)
    {
        isOnPoison = value;
    }

}

// not used yet, but maybe can be used for later
/*    
    public void setIsLookingAtObstacle(Collider collider)
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        isLookingAtObstacle = collider.bounds.IntersectRay(ray);
    }


private void checkClosestPoint()
{
    // if is colliding against obstacle, do not move in direction of obstacle; set once during collision start
    // TODO: currently stick to the obstacle - can be used as a feature
    Vector3 closestPoint = collidingObstacle.ClosestPointOnBounds(currentHelper.transform.position);

    // if closestPoint is closer to x direction 
    // do not move in x direction
    if (Math.Abs(closestPoint.x - currentHelper.transform.position.x) <= 0.2)
    {
        movementRestriction = new Vector3(1.0f, 0.0f, 0.0f);
    }

    // if closestPoint is closer to z direction 
    // do not move in z direction
    else if (Math.Abs(closestPoint.z - currentHelper.transform.position.z) <= 0.2)
    {
        movementRestriction = new Vector3(0.0f, 0.0f, 1.0f);
    }
}*/