using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    public Vector3 offset = new Vector3(0, 6, -7);

    void Update()
    {
        if (player != null)
        {
            transform.position = player.transform.position + offset;
        }
    }
}