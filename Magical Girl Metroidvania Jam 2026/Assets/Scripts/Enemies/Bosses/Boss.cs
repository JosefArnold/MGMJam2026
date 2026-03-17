using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class Boss : Destructible {

  private int lastAttack = 0;

  [SerializeField] private PlayableDirector director;
  [SerializeField] private PlayableAsset deathCutscene;
  [SerializeField] private SpriteRenderer[] sr;
  [SerializeField] private GameObject projectile;
  [SerializeField] private Transform projectileSpawnPoint;
  [SerializeField] private Sprite[] projectileSprites;
  private SFX sfx;
  private Animator anim;
  private bool stunned;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {
    sfx = GetComponent<SFX>();
    anim = GetComponent<Animator>();

    health = maxHealth;
  }

  // Update is called once per frame
  void Update() {

  }



  public void FinishIntro() {
    GameManager.ptr.p.ToggleControls(true);
  }

  public void RandomizeAttack() {
    if (!stunned) {
      if (lastAttack != 0)
        anim.ResetTrigger("Attack" + lastAttack);

      int index = lastAttack;

      while (index == lastAttack)
        index = Random.Range(1, 4);

      anim.SetTrigger("Attack" + index);
      lastAttack = index;
    }
  }

  public void ShootProjectile() {
    if (!stunned) {
      Player p = GameManager.ptr.p;
      Vector2 projAngle = p.transform.position - projectileSpawnPoint.position;
      projAngle.Normalize();

      GameObject proj = Instantiate(projectile, projectileSpawnPoint.transform.position, Quaternion.identity);

      proj.GetComponent<Attack>().AssignSprite(projectileSprites[Random.Range(0, 3)]);
      proj.GetComponent<Attack>().SetProjectileDirection(projAngle);
    }
  }

  public void Stun() {
    anim.SetBool("Stunned", true);

    StartCoroutine(StunWearOff());

    InvokeRepeating("StunDamage", 0.0f, 1.0f);
  }

  private void StunDamage() {
    TakeDamage(2);
  }

  IEnumerator StunWearOff() {
    yield return new WaitForSeconds(3.0f);

    anim.SetBool("Stunned", false);
    CancelInvoke();
    stunned = false;
  }

  protected override void DamageEffect() {
    StartCoroutine(DamageFlash());
  }

  protected override void Death() {
    stunned = false;
    sfx.SetSFX(0, false, true);
    anim.SetTrigger("Death");
    director.playableAsset = deathCutscene;
    director.Play();
  }

  public void DestroyBoss() {
    Destroy(gameObject);
  }

  IEnumerator DamageFlash() {

    foreach (SpriteRenderer s in sr)
      s.color = new Color(1.0f, 0.41f, 0.41f);

    yield return new WaitForSeconds(0.5f);

    foreach (SpriteRenderer s in sr)
      s.color = Color.white;

  }
}
