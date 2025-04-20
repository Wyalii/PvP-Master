using UnityEngine;
using Photon.Pun;
using System.Collections;
using UnityEngine.UI;
public class PlayerCombat : MonoBehaviourPun
{
    [SerializeField] private Image _healtBarSprite;
    private Animator animator;
    public float meleeAttackRange;
    public Transform meleeAttackPoint;
    public LayerMask enemyLayers;
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isStunned = false;
    private int attackCount = 0;
    private bool isMeleeCooldown = false;
    private float meleeCooldownDuration = 4f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (isStunned || isMeleeCooldown) return;


        if (Input.GetKeyDown(KeyCode.J))
        {
            if (attackCount < 4)
            {
                MeleeAttack();
                attackCount++;
            }

            if (attackCount >= 4)
            {
                StartCoroutine(MeleeAttackCooldown());
            }
        }
    }

    void MeleeAttack()
    {
        animator.SetTrigger("MeleeAttack");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(meleeAttackPoint.position, meleeAttackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            PhotonView enemyPhotonView = enemy.GetComponent<PhotonView>();

            if (enemyPhotonView != null && !enemyPhotonView.IsMine)
            {
                enemyPhotonView.RPC("TakeDamage", RpcTarget.All, 4);
                Debug.Log("Hit the opponent!");

            }
        }
    }

    IEnumerator MeleeAttackCooldown()
    {
        isMeleeCooldown = true;
        Debug.Log("Cooldown started!");
        yield return new WaitForSeconds(meleeCooldownDuration);
        attackCount = 0;
        isMeleeCooldown = false;
        Debug.Log("Cooldown ended!");
    }


    [PunRPC]
    public void TakeDamage(int amount)
    {

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (_healtBarSprite != null)
        {
            _healtBarSprite.fillAmount = currentHealth / maxHealth;
        }
        // Stun the player
        StartCoroutine(StunPlayer(0.5f));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(meleeAttackPoint.position, meleeAttackRange);
    }

    IEnumerator StunPlayer(float duration)
    {
        animator.SetBool("isStunned", true);
        isStunned = true;
        yield return new WaitForSeconds(duration);
        isStunned = false;
        animator.SetBool("isStunned", false);
    }

}
