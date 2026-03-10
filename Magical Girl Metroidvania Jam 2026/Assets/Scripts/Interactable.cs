using UnityEngine;

public abstract class Interactable : MonoBehaviour {

  [Header("Interactable Variables")]
  [SerializeField] private bool triggerOnContact;

  [Header("Interactable References")]
  [SerializeField] private GameObject prompt;

  [SerializeField] private bool disabled;

  private void OnTriggerEnter2D(Collider2D collision) {
    if (!disabled) {
      if (collision != null && collision.gameObject.CompareTag("Player")) {
        if (triggerOnContact)
          Interact(collision.gameObject.GetComponent<Player>());
        else {
          prompt.SetActive(true);
          collision.gameObject.GetComponent<Player>().InteractableInRange(this);
        }
      }
    }
  }

  private void OnTriggerExit2D(Collider2D collision) {
    if (!disabled) {
      if (collision != null && !triggerOnContact && collision.gameObject.CompareTag("Player")) {
        prompt.SetActive(false);
        collision.gameObject.GetComponent<Player>().InteractableInRange(null);
      }
    }
  }

  public abstract void Interact(Player p);

}
