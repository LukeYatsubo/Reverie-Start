using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAI : MonoBehaviour
{
    public bool seeTarget;
    public float stunDuration;
    public CharacterController enemyController;
    public Animator myAnimator;
    public FieldOfView AIFieldOfView;
    public Transform targetTransform;
    public float attackTimer;
    public AIStates currentState;
    void Start()
    {
        enemyController = gameObject.GetComponent<CharacterController>();
        myAnimator = gameObject.GetComponent<Animator>();
        AIFieldOfView = gameObject.GetComponent<FieldOfView>();
        Initialize();
    }
    //When enemies take damage, make it so that they send a function to this to blend in the "hurt" animation. That way, they can stick with their prior animation, but blend in a bit of something to give feedback.

    void Update()
    {
        if (stunDuration > 0)
        {
            stunDuration -= 1 * Time.deltaTime;
            return;
        }
        if(currentState == AIStates.moving)
        {
            Move();
        }
        Attack();
    }
    public void LateUpdate()
    {
        //as of now, this code only targets one thing: the player. If infighting ever gets programmed, we'll need to change this.
        if(AIFieldOfView.visibleTargets.Count > 0 && targetTransform == null)
        {
            targetTransform = AIFieldOfView.visibleTargets[0];
            myAnimator.ResetTrigger("Walk");
            myAnimator.SetTrigger("Walk");
           
            if(currentState != AIStates.attacking && currentState != AIStates.dying)
            {
                currentState = AIStates.moving;
            }
        }
    }
    public virtual void Initialize()
    {

    }
    public virtual void Search()
    {
       
    }
    public abstract void Move();
    public abstract void Attack();

    public virtual void StunAnimation(float stunLength)
    {
        stunDuration = stunLength;
        myAnimator.Play("Hurt");
        
        if(currentState == AIStates.attacking)
        {
            myAnimator.ResetTrigger("Attack");
            myAnimator.SetTrigger("Attack");
           
        }
        if(currentState == AIStates.moving)
        {
            myAnimator.ResetTrigger("Walk");
            myAnimator.SetTrigger("Walk");
        
        }
    }
    public void ChangeState(AIStates targetState)
    {
        currentState = targetState;
        StateChangeAction();
        //follow this pattern, take this version out if it ends up doing nothing, but base more substantial code on this framework. 
    }
    public virtual void StateChangeAction()
    {

    }
   //make an override that recieves animation. 
}
public enum AIStates
{
    idle,
    moving,
    attacking,
    dying
}
public enum ComplexAIStates
{
    search,
    goBack,
}