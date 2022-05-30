using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAI : GroundAI
{

    //Author: Luke Sapir
    /*
    Inherited members:
    public bool seeTarget;
    public float stunDuration;
    public CharacterController enemyController;
    public Animator myAnimator;
    public FieldOfView AIFieldOfView;
    public Transform targetTransform;
    public float attackTimer;
    public AIStates currentState; 
    */

    public override void Move()
    {
        if (currentState == AIStates.idle)
        {
            currentState = AIStates.moving;
        }
        if(targetTransform != null)
        {
            myNavMeshAgent.SetDestination(targetTransform.position);
        }
        
    }
    public override void Attack()
    {
        if (targetTransform == null)
        {
            return;
        }
        else if (Vector3.Distance(transform.position, targetTransform.position) < 5f && attackTimer <= 0f)
        {

            myAnimator.ResetTrigger("Attack");
            myAnimator.SetTrigger("Attack");
            attackTimer = 3f;
        }
        else if (Vector3.Distance(transform.position, targetTransform.position) < 5f)
        {
            attackTimer -= 1 * Time.deltaTime;
        }
    }
        public void AttackEvent()
        {
            if(Vector3.Distance(transform.position, targetTransform.position) < 5f)
        {
            targetTransform.gameObject.GetComponent<CreatureStats>().TakeDamage(gameObject.GetComponent<CreatureStats>().attack);
        }

        }
     /*IEnumerator DamageAtEnd()
        {
            AnimatorClipInfo[] currentClipInfo = this.myAnimator.GetCurrentAnimatorClipInfo(0);
            float clipLength = currentClipInfo[0].clip.length;
            yield return new WaitForSeconds (clipLength *.85f);
            myAnimator.ResetTrigger("Walk");
            myAnimator.SetTrigger("Walk");
            if (Vector3.Distance(transform.position, targetTransform.position) < 5f)
            {
                 
            }
           
        }
     */
    }

