namespace CameraControl
{
    using UnityEngine;

    public class CameraShaker
    {
        private Vector2 _targetShakeOffset;
        private Vector2 _shakeOffset;
        
        private ShakeProperties _currentProperties;
	    
        private float _shakeRadius;
        private int _partionCounter;
        
        public CameraShaker()
        {
            _shakeRadius = 0;
            _partionCounter = 0;
        }

        public void Shake(ShakeProperties properties) {
            _currentProperties = properties;
            _targetShakeOffset = VectorUtil.RotateVector(new Vector2(properties.Strength, 0), properties.Angle);
            _partionCounter = 0;
            _shakeRadius = properties.Strength * ShakeFunction(_currentProperties.Dampening, (float)_partionCounter / _currentProperties.partitions);
        }
        
        public Vector2 ComputeShake()
        {
            if (!_shakeOffset.Equals(_targetShakeOffset)) {
                float distance = _currentProperties.Speed * Time.deltaTime;
                if (Vector2.Distance(_shakeOffset, _targetShakeOffset) <= distance)
                {
                    _shakeOffset = _targetShakeOffset;

                    _partionCounter++;
                    if (_partionCounter <= _currentProperties.partitions) {
                        _shakeRadius = _currentProperties.Strength * ShakeFunction(_currentProperties.Dampening, ((float)_partionCounter / (float)_currentProperties.partitions));

                        _targetShakeOffset *= -1f * (_shakeRadius / _targetShakeOffset.magnitude);
                        _targetShakeOffset = VectorUtil.RotateVector(_targetShakeOffset, Random.Range(-1f, 1f) * 180 * _currentProperties.Noise);
                    }
                    else {
                        _shakeRadius = 0;
                    }
                }
                else {
                    Vector2 v = _targetShakeOffset - _shakeOffset;
                    _shakeOffset += distance * v.normalized;
                }
            }

            return _shakeOffset;
        }
        
        private float ShakeFunction(float dampening, float x) {
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
}
