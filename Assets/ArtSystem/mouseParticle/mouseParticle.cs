using UnityEngine;

public class mouseParticle : MonoBehaviour
{
    public Camera camera;
    public Transform trans;
    // Use this for initialization
    void Start()
    {
        camera.depth = 99999;
    }
    float time = 0;
    // Update is called once per frame
    void Update()
    {
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = 10;
        trans.position = camera.ScreenToWorldPoint(screenPoint);

        if (Input.GetMouseButton(0))
        {
            if (Input.GetMouseButtonDown(0))
            {
                time = 0;
            }
            time += Time.deltaTime;
            if (time > 0.49)
            {
                time = 0.49f;
            }
        }
    }
}
