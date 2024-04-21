using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 只负责正常的寻路即可。其他状态比如冲刺喷射的方法，放在每个敌人专属的脚本中提供,如果是远程攻击怪，并且距离到达要求，就自动停止寻路即可。别的复杂逻辑就交给别的代码了。。
/// 只负责在寻路时计算出最期望的方向即可。别的都交给专属的脚本就行
/// </summary>
public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private List<SteeringBehaviour> steeringBehaviours;

    [SerializeField]
    private List<Detector> detectors;

    [SerializeField]
    private AIData aiData;

    [SerializeField]
    private float detectionDelay = 0.1f, aiUpdateDelay = 0.1f;//, attackDelay = 1f;

    [SerializeField]
    private float attackDistance = 0.5f;
    //整个脚本最后只返回一个结果的向量就行了
    
    public Vector2 FinalMovement { get => movementInput; }

    //Inputs sent from the Enemy AI to the Enemy controller
    //左边是玩家进入攻击范围执行的方法（如冲刺，喷射等）；右边是如果接触到玩家执行的方法
    //public UnityEvent OnPlayerInAttackArea, OnTouchAttack;
    //public UnityEvent<Vector2> OnMovementInput, OnPointerInput;

    [SerializeField]
    private Vector2 movementInput;
    //[SerializeField]
    //private Vector3 rotateInput;//暂时写在这，用于旋转
    [SerializeField]
    private ContextSolver movementDirectionSolver;
    

    bool following = false;

    public void Start()
    {
        //Detecting Player and Obstacles around
        InvokeRepeating("PerformDetection", 0, detectionDelay);
        
    }

    private void PerformDetection()
    {
        foreach (Detector detector in detectors)
        {
            detector.Detect(aiData);
        }
        
    
    }
    
    private void Update()
    {
        //Enemy AI movement based on Target availability
        if (aiData.currentTarget != null)
        {
            //Looking at the Target
            //OnPointerInput?.Invoke(aiData.currentTarget.position);
            //********暂时先放在这里
            //rotateInput = aiData.currentTarget.position - transform.position;
            
            if (following == false)
            {
                following = true;
                StartCoroutine(ChaseAndAttack());
            }
        }
        else if (aiData.GetTargetsCount() > 0)
        {
            //Target acquisition logic
            aiData.currentTarget = aiData.targets[0];
        }
        //Moving the Agent
        //OnMovementInput?.Invoke(movementInput*enemy.moveSpeed);
    }
    /// <summary>
    /// 整个追逐+攻击的判断，并且执行攻击的事件
    /// </summary>
    /// <returns></returns>
    
    private IEnumerator ChaseAndAttack()
    {
        if (aiData.currentTarget == null)
        {
            //Stopping Logic
            Debug.Log("Stopping");
            movementInput = Vector2.zero;
            //rotateInput = Vector3.zero;
            following = false;
            //StartCoroutine(ChaseAndAttack());
            yield break;
        }
        else
        {
            //Chase logic
            //只是给移动的字段赋值，具体的移动还是在update里
            movementInput = movementDirectionSolver.GetDirectionToMove(steeringBehaviours, aiData);
            yield return new WaitForSeconds(aiUpdateDelay);
            StartCoroutine(ChaseAndAttack());
            //float distance = Vector2.Distance(aiData.currentTarget.position, transform.position);

            /*if (distance < attackDistance)
            {
                //Attack logic
                //执行一下攻击的事件，然后等待attackdelay秒再次执行追击协程（等于说只有后摇）
                movementInput = Vector2.zero;
                OnPlayerInAttackArea?.Invoke();
                Debug.Log("Attack");
                yield return new WaitForSeconds(attackDelay);
                StartCoroutine(ChaseAndAttack());
            }
            else
            {
                //Chase logic
                //只是给移动的字段赋值，具体的移动还是在update里
                movementInput = movementDirectionSolver.GetDirectionToMove(steeringBehaviours, aiData);
                yield return new WaitForSeconds(aiUpdateDelay);
                StartCoroutine(ChaseAndAttack());
            }*/

        }

    }
    public Transform GetCurrentTarget()
    {
        return aiData.currentTarget;
    }
    
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, movementInput);
        }
    }
}
