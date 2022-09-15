using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

public class JoyStickMovementOVR : MonoBehaviour
{
    public Rigidbody player;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var joyStickAxis = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick, OVRInput.Controller.RTouch);
         float fixedY = player.position.y;

        player.position +=(transform.right*joyStickAxis.x + transform.forward*joyStickAxis.y)*Time.deltaTime*speed;
        player.position = new Vector3(player.position.x, fixedY, player.position.z);
    }
}
