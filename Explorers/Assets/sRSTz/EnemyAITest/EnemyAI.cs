using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// ֻ����������Ѱ·���ɡ�����״̬����������ķ���������ÿ������ר���Ľű����ṩ,�����Զ�̹����֣����Ҿ��뵽��Ҫ�󣬾��Զ�ֹͣѰ·���ɡ���ĸ����߼��ͽ�����Ĵ����ˡ���
/// ֻ������Ѱ·ʱ������������ķ��򼴿ɡ���Ķ�����ר���Ľű�����
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
    //�����ű����ֻ����һ�����������������
    
    public Vector2 FinalMovement { get => movementInput; }

    //Inputs sent from the Enemy AI to the Enemy controller
    //�������ҽ��빥����Χִ�еķ��������̣�����ȣ����ұ�������Ӵ������ִ�еķ���
    //public UnityEvent OnPlayerInAttackArea, OnTouchAttack;
    //public UnityEvent<Vector2> OnMovementInput, OnPointerInput;

    [SerializeField]
    private Vector2 movementInput;
    //[SerializeField]
    //private Vector3 rotateInput;//��ʱд���⣬������ת
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
            //********��ʱ�ȷ�������
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
    /// ����׷��+�������жϣ�����ִ�й������¼�
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
            //ֻ�Ǹ��ƶ����ֶθ�ֵ��������ƶ�������update��
            movementInput = movementDirectionSolver.GetDirectionToMove(steeringBehaviours, aiData);
            yield return new WaitForSeconds(aiUpdateDelay);
            StartCoroutine(ChaseAndAttack());
            //float distance = Vector2.Distance(aiData.currentTarget.position, transform.position);

            /*if (distance < attackDistance)
            {
                //Attack logic
                //ִ��һ�¹������¼���Ȼ��ȴ�attackdelay���ٴ�ִ��׷��Э�̣�����˵ֻ�к�ҡ��
                movementInput = Vector2.zero;
                OnPlayerInAttackArea?.Invoke();
                Debug.Log("Attack");
                yield return new WaitForSeconds(attackDelay);
                StartCoroutine(ChaseAndAttack());
            }
            else
            {
                //Chase logic
                //ֻ�Ǹ��ƶ����ֶθ�ֵ��������ƶ�������update��
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
