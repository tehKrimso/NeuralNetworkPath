using System;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public float speed; //Speed Multiplier
    public float rotation; //Rotation multiplier
    public LayerMask raycastMask; //Mask for the sensors

    public NeuralNetwork network;
    
    private float[] input = new float[5]; //input to the neural network

    public int position; //Checkpoint number on the course
    public bool collided; //To tell if the car has crashed

    private List<GameObject> _checkpointList = new List<GameObject>();
    private void DrawLidar()
    {
        for (int i = 0; i < 5; i++) //draws five debug rays as inputs
        {
            Vector3 newVector =
                Quaternion.AngleAxis(i * 45 - 90, new Vector3(0, 1, 0)) *
                -transform.forward; //calculating angle of raycast
            RaycastHit hit;
            Ray Ray = new Ray(transform.position, newVector);


            if (Physics.Raycast(Ray, out hit, 10, raycastMask))
            {
                input[i] = (10 - hit.distance) / 10; //return distance, 1 being close
            }
            else
            {
                input[i] = 0; //if nothing is detected, will return 0 to network
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
            position++;
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        collided = true;
        // if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Checkpoint")) //check if the car passes a gate
        // {
        //     GameObject[] checkPoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        //     for (int i = 0; i < checkPoints.Length; i++)
        //     {
        //         if (collision.collider.gameObject == checkPoints[i] &&
        //             i == (position + 1 + checkPoints.Length) % checkPoints.Length)
        //         {
        //             position++; //if the gate is one ahead of it, it increments the position, which is used for the fitness/performance of the network
        //             break;
        //         }
        //     }
        // }
        // else if (collision.collider.gameObject.layer != LayerMask.NameToLayer("Agent"))
        // {
        //     collided = true; //stop operation if car has collided
        // }
    }

    public int GetScore()
    {
        return position;
    }

    public float[] GetSensorData() //Прописать метод
    {
        DrawLidar();
        return input;
    }

    public void SetMoveValues(float linearMove, float angularMove)//прописать метод
    {
        if (collided)
            return;
        
        
        transform.Rotate(0, angularMove * rotation, 0, Space.World); //controls the cars movement
        transform.position += -transform.forward * linearMove * speed; //controls the cars turning
    }
}