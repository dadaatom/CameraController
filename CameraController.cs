using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public static CameraController Instance;
	public Vector2 CameraOffset;
	public float CameraSpeed;

	public float MaxScreenSize;
	public float MinScreenSize;
	public float ZoomSpeed;

	private Camera _mainCamera;
	
	private GameObject _targetObject;

	private Vector2 _screenSize;

	//private Vector3 OriginVector;
	//private Vector3 TargetVector;

	private Vector2 _cameraCenter;
	private Vector2 _shakeOffset;
	private Vector2 _targetVector;
	private Vector2 _targetShakeOffset;

	private ShakeProperties _currentProperties;
	private float _shakeRadius;
	private int _partionCounter;

	void Awake()
	{
		DontDestroyOnLoad(this);

		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}

		_shakeRadius = 0;
		_partionCounter = 0;

		_mainCamera = GetComponent<Camera>();
		
		_screenSize = new Vector2(_mainCamera.aspect * _mainCamera.orthographicSize * 2, _mainCamera.orthographicSize * 2);
	}

	void Start()
	{
		_cameraCenter = new Vector3(Player.Instance.transform.position.x, Player.Instance.transform.position.y, transform.position.z);
	}

	void FixedUpdate()
	{
		_targetVector = _targetObject.transform.position;

		if (_targetObject != null) {
			if (!_cameraCenter.Equals(_targetVector)) {
				float distance = CameraSpeed * Time.deltaTime;
				if (Vector2.Distance(_cameraCenter, _targetVector) <= distance) {
					_cameraCenter = _targetVector;
				}
				else {
					Vector2 v = _targetVector - _cameraCenter;
					_cameraCenter += distance * v.normalized;
				}
			}
		}
		if (!_shakeOffset.Equals(_targetShakeOffset)) {
			float distance = _currentProperties.Speed * Time.deltaTime;
			if (Vector2.Distance(_shakeOffset, _targetShakeOffset) <= distance)
			{
				_shakeOffset = _targetShakeOffset;

				_partionCounter++;
				if (_partionCounter <= _currentProperties.partitions) {
					_shakeRadius = _currentProperties.Strength * ShakeFunction(_currentProperties.Dampening, ((float)_partionCounter / (float)_currentProperties.partitions));

					_targetShakeOffset *= -1f * (_shakeRadius / _targetShakeOffset.magnitude);
					_targetShakeOffset = VectorRotated(_targetShakeOffset, Random.Range(-1f, 1f) * 180 * _currentProperties.Noise);
				}
				else {
					_shakeRadius = 0;
				}
				
				/*
				 * Vector B = A;
					B *= (reductionLength/A.length());
					return B;
				 */
			}
			else {
				Vector2 v = _targetShakeOffset - _shakeOffset;
				_shakeOffset += distance * v.normalized;
			}
		}
		transform.position = new Vector3(_cameraCenter.x + _shakeOffset.x + CameraOffset.x, _cameraCenter.y + _shakeOffset.y + CameraOffset.y, transform.position.z);
	}

	public void Zoom(float multiplier) {
		float TargetSize = Camera.main.orthographicSize + multiplier * ZoomSpeed;
		if (TargetSize < MinScreenSize) {
			TargetSize = MinScreenSize;
		}
		else if (TargetSize > MaxScreenSize) {
			TargetSize = MaxScreenSize;
		}

		_mainCamera.orthographicSize = TargetSize;
	}

	public void Shake(ShakeProperties properties) {
		_currentProperties = properties;
		_targetShakeOffset = VectorRotated(new Vector2(properties.Strength, 0), properties.Angle);
		_partionCounter = 0;
		_shakeRadius = properties.Strength * ShakeFunction(_currentProperties.Dampening, _partionCounter / _currentProperties.partitions);

		//Debug.Log(TargetShakeOffset);

		//Debug.Log(shakeRadius);
	}

	public Vector2 GetScreenSize() {
		return _screenSize;
	}

	public GameObject GetTarget() {
		return _targetObject;
	}

	public void SetTarget(GameObject obj) {
		_targetObject = obj;
	}

	public void SetCenter(Vector2 v) {
		_cameraCenter = v;
	}

	private Vector2 VectorRotated(Vector2 v, float angle) {
		return new Vector2(v.x * Mathf.Cos(Mathf.Deg2Rad * angle) - v.y * Mathf.Sin(Mathf.Deg2Rad * angle), v.x * Mathf.Sin(Mathf.Deg2Rad * angle) + v.y * Mathf.Cos(Mathf.Deg2Rad * angle));
	}

	private float ShakeFunction(float dampening, float x) {
		//Debug.Log("p: " + partionCounter + " - " + partitions);
		float a = Mathf.Lerp(.25f, 2f, Mathf.Clamp01(dampening));
		float b = 1 - Mathf.Pow(x, dampening);
		return b * b * b;
		//return Mathf.Pow(1-Mathf.Pow(x, .25f + 1.75f *dampening),3f);
	}

	[System.Serializable]
	public class ShakeProperties {
		public float Strength, Speed, Angle;
		public int partitions;

		[Range(0, 1)]
		public float Noise;

		[Range(0,1)]
		public float Dampening;
	}
}
