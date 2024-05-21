using Cinemachine;
using UnityEngine;

namespace Effects
{
    public class CameraShake : MonoBehaviour
    {
        private CinemachineVirtualCamera _virtualCamera;
        private CinemachineBasicMultiChannelPerlin _virtualCameraNoise;
        private float _duration;
        private float _timer;

        private void Awake()
        {
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
            _virtualCameraNoise = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        private void Update()
        {
            if (_timer > 0)
            {
                _timer -= Time.deltaTime;
                if (_timer <= 0) StopShake();
            }
        }

        /// <summary>
        /// Triggers a camera shake effect at the specified intensity and duration.
        /// </summary>
        /// <param name="duration">The duration of the shake.</param>
        /// <param name="intensity">The intensity of the shake.</param>
        /// <param name="gain">The amount of gain. The larger the value the more it shakes rapidly.</param>
        public void TriggerShake(float duration, float intensity, float gain = 1.0f)
        {
            _virtualCameraNoise.m_AmplitudeGain = intensity;
            _virtualCameraNoise.m_FrequencyGain = gain;
            _duration = duration;
            _timer = duration;
        }

        /// <summary>
        /// Resets the camera shake effect.
        /// </summary>
        private void StopShake()
        {
            _timer = 0;
            _virtualCameraNoise.m_AmplitudeGain = 0;
        }
    }
}
