using System.Collections;
using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    public AIPath ai;
    public int playerIndex = 0;
    [SerializeField] public Sprite icon;
    [SerializeField] public int cost = 10;

    //might change at runtime
    public Unit target;
    public bool isDead = false;
    public bool isEngaging = false;
    public bool isInCombat = false;
    public int type = 0;

    public int baseHP;
    public int baseDamage;
    public int baseArmor;
    //public int baseResistance;
    public float baseAttackSpeed;

    //[SerializeField] int speed;
    [SerializeField] GameObject redPopupTextPrefab;
    [SerializeField] GameObject greenPopupTextPrefab;

    //don't change at runtime
    public int healthPoints;
    public int damage;
    public int armor;
    //public int resistance;
    public float attackSpeed;

    [SerializeField] int range;
    [SerializeField] public int human_cav_siege;

    Animator unitAnimator;

    private void Awake()
    {
        UnitsManager.Agent.Add(this);
        ai = GetComponent<AIPath>();
        ai.endReachedDistance = range;

        unitAnimator = GetComponent<Animator>();
    }
    private void Start()
    {

        InitUnit();
    }

    void InitUnit ()
    {
        healthPoints = baseHP + baseHP * GameManager.Instance.HealthModifier;
        damage = baseDamage + baseDamage * GameManager.Instance.DamageModifier;
        armor = baseArmor + baseArmor * GameManager.Instance.ArmorModifier;
        //resistance = baseResistance + baseResistance * GameManager.Instance.ResistanceModifier;
        attackSpeed = baseAttackSpeed + baseAttackSpeed * GameManager.Instance.AttackSpeedModifier;
    }

    IEnumerator CheapUpdate ()
    {
        if (this.transform.position.y < -5)
            KillUnit(this);

        yield return new WaitForSeconds(1);
        StartCoroutine(CheapUpdate());
    }

    Vector3 lastPos;
    private void Update()
    {
        if (healthPoints <= 0 && !isDead)
            KillUnit(this);

        if (target == null || target.isDead)
            isInCombat = false;

        if (ai.reachedEndOfPath && isEngaging && !isInCombat && !isAttackCoroutineOn)
        {
            this.isEngaging = false;
            this.isInCombat = true;

            StartCoroutine(Attack(target));
        }

        unitAnimator.SetBool("isFighting", isInCombat);
        unitAnimator.SetBool("isMoving", transform.position != lastPos);

        lastPos = transform.position;
    }

    bool isAttackCoroutineOn = false;
    public IEnumerator Attack (Unit target)
    {
        isAttackCoroutineOn = true;
        yield return new WaitForSeconds(attackSpeed);

        if (target)
            target.TakeDamage(this.damage);

        if (target.healthPoints > 0)
            StartCoroutine(Attack(target));
        else
        {
            this.isInCombat = false;
            isAttackCoroutineOn = false;
        }
    }

    void KillUnit (Unit unit)
    {
        isDead = true;
        StopAllCoroutines();
        unit.unitAnimator.SetTrigger("isDying");

        LeanTween.delayedCall(2, ()=> Destroy(unit.gameObject));
        UnitsManager.Agent.Remove(unit);
    }

    public void DestroyUnit ()
    {
        StopAllCoroutines();
        UnitsManager.Agent.Remove(this);
        Destroy(this.gameObject);
    }

    public void TakeDamage (int damage)
    {
        int actualDamage = Mathf.Abs(damage - armor);

        healthPoints -= actualDamage;
        if (target != null)
            DisplayDamageTaken(actualDamage, target.transform.position);
    }

    void DisplayDamageTaken (int damage, Vector3 target_)
    {
        Vector3 offsetPos = target_ + Vector3.up * 2;

        Transform damageText;
        if (target.type == 0)
            damageText = Instantiate(redPopupTextPrefab, offsetPos, Quaternion.identity).transform;
        else
            damageText = Instantiate(greenPopupTextPrefab, offsetPos, Quaternion.identity).transform;

        damageText.SetParent(target.transform, true);
        damageText.GetComponent<DamageTextHandler>().damageText.text = damage.ToString();
    }

    public void Heal (int heal)
    {
        healthPoints += Mathf.Clamp(heal, 0, baseHP - baseHP);
    }

    public void RemoveBuffsAndDebuffs ()
    {
        healthPoints = baseHP;
        damage = baseDamage;
        armor = baseArmor;
        //resistance = baseRes;
    }

    private void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(this.transform.position, target.transform.position);
        }
    }
}
