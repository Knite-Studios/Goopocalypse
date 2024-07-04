using UnityEngine;

namespace Entity.Enemies
{
    public class Idler : MonoBehaviour
    {
        private IdlerEnemy _idlerEnemy;
        private bool _isLateStart;

        private void Awake()
        {
            _idlerEnemy = gameObject.GetOrAddComponent<IdlerEnemy>();
            _idlerEnemy.enabled = false;
        }

        private void LateUpdate()
        {
            if (!_isLateStart)
            {
                _idlerEnemy.enabled = true;
                _isLateStart = true;
            }
        }
    }
}
