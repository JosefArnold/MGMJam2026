using UnityEngine;

public abstract class Destructible : MonoBehaviour {

  [Header("Destructible Variables")]
  [SerializeField] protected int maxHealth;

  protected int health;

  public void TakeDamage(int damage) {
    health -= damage;

    Mathf.Clamp(health, 0, maxHealth);

    if (health <= 0)
      Death();
    else
      DamageEffect();
  }

  protected abstract void DamageEffect();

  protected abstract void Death();

}
