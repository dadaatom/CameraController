namespace CameraControl
{
    using UnityEngine;

    public class CameraZoomer
    {
        public Camera MainCamera { get; set; }

        public float MaxScreenSize;
        public float MinScreenSize;
        public float ZoomSpeed;

        public CameraZoomer(Camera mainCamera)
        {
            MainCamera = mainCamera;
        }

        public void Zoom(float multiplier)
        {
            float TargetSize = MainCamera.orthographicSize + multiplier * ZoomSpeed;

            if (TargetSize < MinScreenSize)
            {
                TargetSize = MinScreenSize;
            }
            else if (TargetSize > MaxScreenSize)
            {
                TargetSize = MaxScreenSize;
            }

            MainCamera.orthographicSize = TargetSize;
        }
    }
}
