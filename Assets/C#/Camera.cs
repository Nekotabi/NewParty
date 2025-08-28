using UnityEngine;

public class Camera : MonoBehaviour
{
    private GameObject PlayerObj;
    private Transform PlayerPos;
    private Transform MyPos;
    private Vector3 Offset;
    void Start()
    {   //‚¢‚ë‚¢‚ë’è‹`
        PlayerObj = transform.parent.gameObject;
        PlayerPos = PlayerPos.GetComponent<Transform>();
        MyPos = this.GetComponent<Transform>();
        Offset = PlayerPos.position - MyPos.position;
    }

    void Update()
    {
    }
}
