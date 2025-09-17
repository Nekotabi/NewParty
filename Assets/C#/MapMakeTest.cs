using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class MapMakeTest : MonoBehaviour
{
    public int MapLength, MapWidth;
    public List<GameObject> Grounds = new();
    void Start()
    {
        for(int i = 0; i < MapLength - 1; i++)
        {
            for(int j = 0; j < MapWidth - 1; i++)
            {
                int a = Random.Range(0, 2);
                Debug.Log(Instantiate(Grounds[a], new Vector3(i * 10, 0, 0), Quaternion.identity).name);
            }
        }
    }
}
