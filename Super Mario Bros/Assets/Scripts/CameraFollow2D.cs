using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Mario
{
    public class CameraFollow2D : MonoBehaviour
    {

        public Transform target;
        public static CameraFollow2D cameraFollow;
        // Use this for initialization
        void Start()
        {
            cameraFollow = this;
        }

        // Update is called once per frame
        void Update()
        {
            if (target.position.x >= transform.position.x)
            {
                transform.position = new Vector3(target.position.x, transform.position.y, transform.position.z);
            }
        }
        public void ResetPosition()
        {
            transform.position = new Vector3(target.position.x, transform.position.y, transform.position.z);
        }
    }
}