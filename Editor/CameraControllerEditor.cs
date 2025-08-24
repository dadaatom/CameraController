using UnityEditor;
using UnityEngine;

namespace CameraControl
{
    [CustomEditor(typeof(CameraController))]
    public class CameraControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Get a reference to the target script
            CameraController cameraController = (CameraController)target;

            // Draw the default Inspector for the enum
            DrawDefaultInspector(); 

            // Conditionally draw other fields based on the enum value
            switch (cameraController.cameraTrackingType)
            {
                case CameraTrackingType.Solid:
                    break;
                case CameraTrackingType.Speed:
                    cameraController.cameraMovementSpeed = EditorGUILayout.FloatField("Camera Movement Speed", cameraController.cameraMovementSpeed);
                    break;
                case CameraTrackingType.Time:
                    cameraController.cameraMovementTime = EditorGUILayout.FloatField("Camera Movement Time", cameraController.cameraMovementTime);
                    break;
            }

            // Apply changes if any were made in the Inspector
            if (GUI.changed)
            {
                EditorUtility.SetDirty(cameraController);
            }
        }
    }
}