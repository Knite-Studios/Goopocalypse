using System.Collections;
using Cinemachine;
using UnityEngine;

namespace Effects
{
    public static class CameraShake
    {
        private static CinemachineBasicMultiChannelPerlin _virtualCameraNoise;
        private static float _intensity;
        private static float _timer;

        private static void Initialize(CinemachineVirtualCamera virtualCamera)
        {
            if (_virtualCameraNoise) return;
            _virtualCameraNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        /// <summary>
        /// Triggers a camera shake effect at the specified intensity and duration.
        /// </summary>
        /// <param name="virtualCamera">The virtual camera to apply shake to.</param>
        /// <param name="duration">The duration of the shake.</param>
        /// <param name="intensity">The intensity of the shake.</param>
        /// <param name="gain">The amount of gain. The larger the value the more it shakes rapidly.</param>
        public static void TriggerShake(CinemachineVirtualCamera virtualCamera,
            float duration = 1.0f, float intensity = 5.0f, float gain = 1.0f)
        {
            Initialize(virtualCamera);
            _intensity = intensity;
            _virtualCameraNoise.m_AmplitudeGain = intensity;
            _virtualCameraNoise.m_FrequencyGain = gain;
            _timer = duration;
            virtualCamera.StartCoroutine(ShakeCoroutine());
        }

        private static IEnumerator ShakeCoroutine()
        {
            var elapsed = 0.0f;

            while (elapsed < _timer)
            {
                elapsed += Time.deltaTime;
                var remainder = _timer - elapsed;
                _virtualCameraNoise.m_AmplitudeGain = Mathf.Lerp(0, _intensity, remainder / _timer);
                yield return null;
            }

            StopShake();
        }

        /// <summary>
        /// Resets the camera shake effect.
        /// </summary>
        public static void StopShake()
        {
            if (!_virtualCameraNoise) return;

            _timer = 0;
            _virtualCameraNoise.m_AmplitudeGain = 0;
        }
    }
}
