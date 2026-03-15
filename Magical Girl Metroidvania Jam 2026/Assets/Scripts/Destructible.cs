using UnityEngine;

public abstract class Destructible : MonoBehaviour {

  [Header("Destructible Variables")]
  [SerializeField] protected int maxHealth;

  protected bool iFrames;

  protected int health;

  public void TakeDamage(int damage) {
    if (!iFrames || damage < 0) {
      health -= damage;

      Mathf.Clamp(health, 0, maxHealth);

      if (health <= 0)
        Death();
      else if (damage > 0)
        DamageEffect();
    }
  }

  protected abstract void DamageEffect();

  protected abstract void Death();

}
