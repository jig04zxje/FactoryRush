using UnityEngine;
using Unity.Cinemachine; 

namespace FactoryRush.Scripts.Core
{
    public class CinemachineRTSCamera : MonoBehaviour
    {
        [SerializeField] 
        private CinemachineCamera vCam;

        public float panSpeed = 20f;
        public float panBorderThickness = 15f;

        public float zoomSpeed = 5f;
        public float minZoom = 3f;
        public float maxZoom = 10f;
        void Update()
        {
            //Panning logic
            Vector3 pos = transform.position;
            //Panning up
            if (Input.mousePosition.y >= Screen.height - panBorderThickness || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                pos.y += panSpeed * Time.deltaTime;
            //Panning down
            if (Input.mousePosition.y <= panBorderThickness || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                pos.y -= panSpeed * Time.deltaTime;
            //Panning right
            if (Input.mousePosition.x >= Screen.width - panBorderThickness || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                pos.x += panSpeed * Time.deltaTime;
            //Panning left
            if (Input.mousePosition.x <= panBorderThickness || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                pos.x -= panSpeed * Time.deltaTime;

            transform.position = pos;

            //Zoom logic
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (vCam != null && scroll != 0f)
            {
                // Get current Orthographic Size 
                float currentSize = vCam.Lens.OrthographicSize;

                currentSize -= scroll * zoomSpeed;

                vCam.Lens.OrthographicSize = Mathf.Clamp(currentSize, minZoom, maxZoom);
            }
        }
    }
}