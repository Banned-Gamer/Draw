using UnityEngine;

public class GetPosition : MonoBehaviour
{
    void OnMouseDown()
    {
        transform.position = Input.mousePosition;
        Debug.Log(Input.mousePosition);
    }
}