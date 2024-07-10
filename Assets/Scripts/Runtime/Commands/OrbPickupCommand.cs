using Entity;
using Managers;
using Mirror;
using UI;
using UnityEngine;

namespace Commands
{
    public class OrbPickupCommand : ICommand
    {
        private readonly Orb _orb;
        private readonly Vector3 _position;
        private readonly long _points;
        private readonly AudioClip _pickupSound;

        public OrbPickupCommand(Orb orb, Vector3 position, long points, AudioClip pickupSound)
        {
            _orb = orb;
            _position = position;
            _points = points;
            _pickupSound = pickupSound;
        }

        public void Execute()
        {
            // Update the score.
            WaveManager.Instance.Score += _points;

            // Spawn orb score feedback.
            var orbScoreText = PrefabManager.Create<OrbScoreText>(PrefabType.OrbScoreText);
            if (NetworkServer.active) NetworkServer.Spawn(orbScoreText.gameObject);
            orbScoreText.transform.position = _position;
            orbScoreText.SetText(_points.ToString());
            orbScoreText.OnSpawn();

            // Play the pickup sound.
            AudioManager.Instance.PlayOneShot(_pickupSound, _position);

            // Destroy the orb.
            if (NetworkServer.active)
                NetworkServer.Destroy(_orb.gameObject);
            else
                Object.Destroy(_orb.gameObject);
        }
    }
}
