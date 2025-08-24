using UnityEngine.Serialization;

namespace CameraControl
{
	using UnityEngine;

	public class CameraController : MonoBehaviour
	{
		private Camera MainCamera { get; set; }

		public CameraShaker CameraShaker { get; set; }
		public CameraZoomer CameraZoomer { get; set; }
		
		public Vector2 cameraOffset;

		public CameraTrackingType cameraTrackingType;

		[HideInInspector]
		public float cameraMovementSpeed;
		[HideInInspector]
		public float cameraMovementTime;

		private GameObject _targetObject;

		private Vector2 _currentCenter;
		private Vector2 _targetVector;
		private Vector2 _previousTargetVector;

		private float _timeSinceOnTarget;

		void Awake()
		{
			MainCamera = GetComponent<Camera>();
			
			CameraShaker = new CameraShaker();
			CameraZoomer = new CameraZoomer(MainCamera);

			_currentCenter = transform.position;

			_timeSinceOnTarget = 0;
		}

		void FixedUpdate()
		{
			if (_targetObject != null)
			{
				if (!_targetObject.transform.position.Equals(_targetVector))
				{
					_timeSinceOnTarget = 0;
					_previousTargetVector = _targetVector;
					_targetVector = _targetObject.transform.position;
				}

				if (!_currentCenter.Equals(_targetVector))
				{
					_timeSinceOnTarget += Time.deltaTime;
					
					switch (cameraTrackingType)
					{
						case CameraTrackingType.Solid:
							_currentCenter = _targetVector;
							break;
						
						case CameraTrackingType.Speed:
							float distance = cameraMovementSpeed * Time.deltaTime;
							if (Vector2.Distance(_currentCenter, _targetVector) <= distance)
							{
								_currentCenter = _targetVector;
							}
							else
							{
								Vector2 v = _targetVector - _currentCenter;
								_currentCenter += distance * v.normalized;
							}

							break;
						
						case CameraTrackingType.Time:
							_currentCenter = Vector2.Lerp(_previousTargetVector, _targetVector,
								_timeSinceOnTarget / cameraMovementTime);
							
							break;
					}
				}
			}

			Vector2 cameraPos = _currentCenter + cameraOffset + CameraShaker.ComputeShake();
			transform.position = new Vector3(cameraPos.x, cameraPos.y, transform.position.z);
		}

		public GameObject GetTarget()
		{
			return _targetObject;
		}

		public void SetTarget(GameObject obj, bool setPos = false)
		{
			if (setPos)
			{
				SetCenter(_targetObject.transform.position);
			}

			_targetObject = obj;
		}

		public void SetCenter(Vector2 v)
		{
			_currentCenter = v;
		}
	}

	public enum CameraTrackingType
	{
		Solid,
		Speed,
		Time
	}
}
