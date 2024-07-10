using DG.Tweening;
using Mirror;
using TMPro;
using UnityEngine;

namespace UI
{
    public class OrbScoreText : NetworkBehaviour
    {
        [SerializeField] private float upForce = 1.5f;
        [SerializeField] private float duration = 1.5f;
        [SerializeField] private float speed = 0.5f;

        private TMP_Text _text;

        private void Awake()
            => _text = GetComponent<TMP_Text>();

        public void SetText(string text)
            => _text.text = $"+{text}";

        public void OnSpawn()
        {
            transform.DOMoveY(transform.position.y + upForce, duration)
                .SetSpeedBased()
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    if (NetworkServer.active)
                        NetworkServer.Destroy(gameObject);
                    else
                        Destroy(gameObject);
                });
        }
    }
}
