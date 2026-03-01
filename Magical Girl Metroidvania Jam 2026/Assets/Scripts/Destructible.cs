using UnityEngine;

public abstract class Destructible : MonoBehaviour {

  [Header("Destructible Variables")]
  [SerializeField] protected int health;

  public void TakeDamage(int damage) {
    health -= damage;
    Debug.Log("Owie :(");

    if (health <= 0)
      Death();
  }

  protected abstract void Death();

}
