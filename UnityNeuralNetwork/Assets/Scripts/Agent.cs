using System;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public float speed; //Speed Multiplier
    public float rotation; //Rotation multiplier
    public LayerMask raycastMask; //Mask for the sensors

    public NeuralNetwork network;
    
    private float[] input = new float[8]; //input to the neural network

    public int position; //Checkpoint number on the course
    public bool collided; //To tell if the car has crashed

    private List<GameObject> _checkpointList = new List<GameObject>();
    private void DrawLidar()
    {
        for (int i = 0; i < 8; i++) //draws debug rays as inputs
        {
            Vector3 newVector =
                Quaternion.AngleAxis(i * 45 - 90, new Vector3(0, 1, 0)) *
                -transform.forward; //calculating angle of raycast
            RaycastHit hit;
            Ray Ray = new Ray(transform.position, newVector);


            int raycastDistance = 10;
            
            if (Physics.Raycast(Ray, out hit, raycastDistance, raycastMask))
            {
                input[i] = (raycastDistance - hit.distance) / raycastDistance;
            }
            else
            {
                input[i] = 0;
            }
            
            //
            Debug.DrawRay(transform.position, newVector, Color.red);
            //
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint") && !_checkpointList.Contains(other.gameObject))
        {
            _checkpointList.Add(other.gameObject);
            
            //
            if(other.gameObject.name == "CheckPoint (43)" && _checkpointList.Count > 43)
                _checkpointList.Clear();
            //
            
            position++;
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        collided = true;
    }

    public int GetScore()
    {
        return position;
    }

    public float[] GetSensorData() 
    {
        DrawLidar();
        return input;
    }

    public void SetMoveValues(float linearMove, float angularMove)
    {
        if (collided)
            return;


        transform.Rotate(0, angularMove * rotation, 0, Space.World); //controls rotation
        transform.position += -transform.forward * linearMove * speed; //controls linear movement
    }
}