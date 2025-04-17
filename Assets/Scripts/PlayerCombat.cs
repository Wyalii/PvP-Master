using UnityEngine;
using Photon.Pun;
using System.Collections;
public class PlayerCombat : MonoBehaviourPun
{
    private Animator animator;
    public float meleeAttackRange;
    public Transform meleeAttackPoint;
    public LayerMask enemyLayers;
    public float health;
    public bool isStunned = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (isStunned) return;

        if (Input.GetKeyDown(KeyCode.J))
        {
            Attack();
        }
    }

    void Attack()
    {
        animator.SetTrigger("isAttacking");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(meleeAttackPoint.position, meleeAttackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            PhotonView enemyPhotonView = enemy.GetComponent<PhotonView>();

            if (enemyPhotonView != null && !enemyPhotonView.IsMine)
            {
                enemyPhotonView.RPC("TakeDamage", RpcTarget.All, 1);
                Debug.Log("Hit the opponent!");

            }
        }
    }

    [PunRPC]
    public void TakeDamage(int amount)
    {
        // Reduce health
        health -= amount;

        // 👇 Stun the player
        StartCoroutine(StunPlayer(0.5f)); // 1 second stun
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(meleeAttackPoint.position, meleeAttackRange);
    }

    IEnumerator StunPlayer(float duration)
    {
        isStunned = true;

        yield return new WaitForSeconds(duration);

        isStunned = false;
    }

}
