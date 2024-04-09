
using UnityEngine;

namespace Abiogenesis3d
{
    public class CameraOrbit : MonoBehaviour
    {
        public Transform target;

        public float speed = 5;
        public float distance = 10;
        public float minAngle = 20;
        public float maxAngle = 89;

        Vector3 eulerAngles;

        void Start()
        {
            eulerAngles = transform.rotation.eulerAngles;
        }

        private void Update()
        {
            if (Input.GetMouseButton(1))
            {
                eulerAngles.y += Input.GetAxis("Mouse X") * speed;
                eulerAngles.x -= Input.GetAxis("Mouse Y") * speed;
            }
            eulerAngles.x = Mathf.Clamp(eulerAngles.x, minAngle, maxAngle);

            transform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, 0);
            transform.position = target.position - transform.forward * distance;

            transform.LookAt(target);
        }
    }
}
