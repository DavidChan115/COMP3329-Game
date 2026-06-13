using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Camera cam;
    public Transform followTarget;

    // Starting position for the parallax game object
    Vector2 startingPosition;

    // Starting Z-axis value of the parallax game object
    // In 2D game, Z-axis is the distance from the camera to the background
    // Ex. Player will have a very negative Z-axis value (Z-axis value tends to negative, the object tends to smaller)
    float startingZ;

    // Start is called before the first frame update
    void Start()
    {   
        // Normally, transform.position gives a Vector3 value, 
        // but here since startingPosition is Vector2, we only extract X and Y-axis value to startingPosition
        startingPosition = transform.position;

        // startingZ obtains the Z-axis value of transform.position
        startingZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {     

        Vector2 camMoveSinceStart = (Vector2)cam.transform.position - startingPosition;

        // We only care about the distance from the background that this parallax effect is going (Z-axis values)
        float zDistanceFromTarget = transform.position.z - followTarget.transform.position.z;

        // If the parallax game object is in front of the Player, it's gonna have a negative value = using cam.nearClipPlane
        // If the parallax game object is behind of the Player, it's gonna use cam.farClipPlane
        float clippingPlane = cam.transform.position.z + (zDistanceFromTarget > 0 ? cam.farClipPlane : cam.nearClipPlane);

        // The absolute value of the distance between the parallax game object and the follow target object
        float parallaxFactor = Mathf.Abs(zDistanceFromTarget) / clippingPlane;

        // When the game updates, we want to move the position of our Parallax object
        // based on how far the camera has moved from the starting position
        Vector2 newPosition = startingPosition + camMoveSinceStart * parallaxFactor;

        // Z-axis does not change since this is a 2D game
        transform.position = new Vector3(newPosition.x, newPosition.y, startingZ);
    }
}
