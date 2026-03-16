using UnityEngine;

public class AbilityItem : Interactable {

  [SerializeField] private int abilityIndex; // Which ability this should toggle for the player
  [SerializeField] private GameObject tutorialText;
  private Player player;
  private bool buffer = false;
  private SFX sfx;
  private Animator anim;

  private void Start() {
    sfx = GetComponent<SFX>();
    anim = GetComponent<Animator>();
  }

  public override void Interact(Player p) {
    if (!buffer) {
      player = p;
      player.ToggleAbility(abilityIndex, true);
      p.ToggleControls(false);
      sfx.SetSFX(0, false, true);
      anim.SetTrigger("Interact");
      buffer = true;
    }
  }

  public void EndInteraction() {
    player.ToggleControls(true);

    if (abilityIndex == 0) {
      GameManager.ptr.p.MagicalGirlTransformation();
      LevelHUD.ptr.ToggleHealth(true);
    }

    tutorialText.SetActive(true);
    Destroy(gameObject);
  }
}
