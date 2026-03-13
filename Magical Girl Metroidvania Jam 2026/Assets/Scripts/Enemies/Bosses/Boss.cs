using System.Collections;
using UnityEngine;

public class Boss : Destructible {

  private int lastAttack = 0;

  private SpriteRenderer[] sr;
  private Animator anim;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }

  public void FinishIntro() {

  }

  public void RandomizeAttack() {
    int index = lastAttack;

    while (index == lastAttack)
      index = Random.Range(1, 3);

    anim.SetTrigger("Attack" + index);
    lastAttack = index;
  }

  protected override void DamageEffect() {
    
  }

  protected override void Death() {

  }

  IEnumerator DamageFlash() {

    foreach (SpriteRenderer s in sr)
      s.color = Color.red;

    yield return new WaitForSeconds(0.5f);

    foreach (SpriteRenderer s in sr)
      s.color = Color.white;

  }
}
