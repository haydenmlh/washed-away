using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public Transform player;


    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + new Vector3(0, 1, -2);
    }
}
