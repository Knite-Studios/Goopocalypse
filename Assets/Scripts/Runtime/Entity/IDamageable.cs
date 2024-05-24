using Mirror;
using UnityEngine;

namespace Entity
{
    public interface IDamageable
    {
        /// <summary>
        /// Reference to the (likely) MonoBehavior's game object.
        /// </summary>
        public GameObject gameObject { get; }

        /// <summary>
        /// A readonly property to read the health of the current object.
        /// </summary>
        public int CurrentHealth { get; }

        /// <summary>
        /// Subtracts health from the object.
        /// </summary>
        /// <param name="damage">The damage to deal.</param>
        /// <param name="trueDamage">Whether the damage is absolute. (no reductions)</param>
        public void Damage(int damage, bool trueDamage = false);

        /// <summary>
        /// Adds health to the object.
        /// </summary>
        /// <param name="amount">The amount to heal.</param>
        public void Heal(int amount);

        /// <summary>
        /// Invoked when the object's health changes.
        /// </summary>
        /// <param name="amount">A positive integer if healing occured, otherwise a negative integer to indicate damage.</param>
        [ClientRpc]
        public void OnHealthChange(int amount);

        /// <summary>
        /// Invoked when the object dies.
        /// </summary>
        [ClientRpc]
        public void OnDeath();
    }

    /// <summary>
    /// Extension methods for the IDamageable interface.
    /// </summary>
    public static class DamageableExtensions
    {
        /// <summary>
        /// Server command for applying damage to objects.
        /// </summary>
        /// <param name="damageable">The damageable object.</param>
        /// <param name="damage">The damage to deal.</param>
        /// <param name="trueDamage">Whether the damage is absolute. (no reductions)</param>
        [Command]
        public static void CmdDamage(
            this IDamageable damageable,
            int damage,
            bool trueDamage = false)
            => damageable.Damage(damage, trueDamage);

        /// <summary>
        /// Server command for healing objects.
        /// </summary>
        /// <param name="damageable">The damageable object.</param>
        /// <param name="amount">The amount to heal.</param>
        [Command]
        public static void CmdHeal(this IDamageable damageable, int amount)
            => damageable.Heal(amount);
    }
}
