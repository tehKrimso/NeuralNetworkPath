using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleManager : MonoBehaviour
{
    [Range(1f, 20f)] 
    public float TimeScale = 1f;
    
    void Update()
    {
        Time.timeScale = TimeScale;
    }
}
