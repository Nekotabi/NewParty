using UnityEngine;

public class Camera : MonoBehaviour
{
    private Transform PlayerTr;
    private Vector3 CameraRevision = new Vector3(0.0f, 2.5f, -5.0f);

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerTr = player.GetComponent<Transform>();
    }

    private void Update()
    {
        this.transform.position = PlayerTr.position + CameraRevision;
    }
}
