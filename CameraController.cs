namespace CameraControl
{
	using UnityEngine;

	public class CameraController : MonoBehaviour
	{
		private Camera MainCamera { get; set; }

		public CameraShaker CameraShaker { get; set; }
		public CameraZoomer CameraZoomer { get; set; }
		
		public Vector2 cameraOffset;
		public float cameraSpeed;

		private GameObject _targetObject;

		private Vector2 _currentCenter;
		private Vector2 _targetVector;

		void Awake()
		{
			MainCamera = GetComponent<Camera>();
			
			CameraShaker = new CameraShaker();
			CameraZoomer = new CameraZoomer(MainCamera);

			_currentCenter = transform.position;
		}

		void FixedUpdate()
		{
			if (_targetObject != null)
			{
				_targetVector = _targetObject.transform.position;
				
				if (!_currentCenter.Equals(_targetVector))
				{
					float distance = cameraSpeed * Time.deltaTime;
					if (Vector2.Distance(_currentCenter, _targetVector) <= distance)
					{
						_currentCenter = _targetVector;
					}
					else
					{
						Vector2 v = _targetVector - _currentCenter;
						_currentCenter += distance * v.normalized;
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
}
