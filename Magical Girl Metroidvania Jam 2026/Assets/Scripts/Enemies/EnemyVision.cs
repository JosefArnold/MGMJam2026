using UnityEngine;

public class EnemyVision : MonoBehaviour {

  [Header("References")]
  [SerializeField] private Enemy enemy;

  [Header("Variables")]
  [SerializeField] private bool visionTrigger;
  [SerializeField] private bool attackRangeTrigger;
  [SerializeField] private int attackIndex; // If this is an attack range trigger, then which enemy attack does it correspond to?

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }

  private void OnTriggerEnter2D(Collider2D collision) {
    if (collision != null) {
      if (collision.gameObject.CompareTag("Player")) {
        enemy.SpotPlayer(true, attackRangeTrigger, collision.gameObject.GetComponent<Player>()); // So, if the player's in the enemy's attack range,
                                                                                                 // they've been spotted by default. But being spotted
                                                                                                 // doesn't mean the player's within attack range
      }
    }
  }

  private void OnTriggerExit2D(Collider2D collision) {
    if (collision != null) {
      if (collision.gameObject.CompareTag("Player")) {
        enemy.SpotPlayer(visionTrigger ? false : true, false, collision.gameObject.GetComponent<Player>());
      }
    }
  }
}
