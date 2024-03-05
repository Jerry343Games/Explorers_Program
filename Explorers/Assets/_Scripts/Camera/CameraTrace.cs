using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrace : MonoBehaviour
{
    public float smoothSpeed = 0.125f; // ����ƶ���ƽ���ٶ�
    public float maxZoom=20;
    public float minZoom;
    public float zoomLimiter = 50f;

    private float greatestDistance;
    
    void LateUpdate()
    {
        TraceCenter();
        CameraZoom();
    }
    
    
    /// <summary>
    /// ׷��������Һ�����������
    /// </summary>
    private void TraceCenter()
    {
        // �������б��Ϊ"Player"����Ϸ����
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        // �������б��Ϊ"Battery"����Ϸ����
        GameObject[] batteries = GameObject.FindGameObjectsWithTag("Battery");

        // �ϲ���������
        GameObject[] allObjects = new GameObject[players.Length + batteries.Length];
        
        players.CopyTo(allObjects, 0);
        batteries.CopyTo(allObjects, players.Length);

        if (allObjects.Length == 0) return; // ���û���ҵ����󣬲����в���
        greatestDistance = GetGreatestDistance(allObjects);
        Vector3 centerPoint = GetCenterPoint(allObjects); // �������ж�������ĵ�

        Vector3 desiredPosition = centerPoint; // �����Ŀ��λ��
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed); // ƽ���ƶ���Ŀ��λ��
        transform.position = smoothedPosition; // �������λ��
    }
    
    /// <summary>
    /// ���������Զ
    /// </summary>
    private void CameraZoom()
    {
        float zoom = Mathf.Lerp(minZoom, maxZoom, greatestDistance / zoomLimiter); // �������֮���������������������
        float camX = Camera.main.transform.position.x;
        float camY = Camera.main.transform.position.y;
        float camZ = Camera.main.transform.position.z;
        Camera.main.transform.position = new Vector3(camX,camY,-zoom);
    }
    
    /// <summary>
    /// �������ж������ĵ�
    /// </summary>
    /// <param name="objects"></param>
    /// <returns></returns>
    private Vector3 GetCenterPoint(GameObject[] objects)
    {
        if (objects.Length == 1)
        {
            return objects[0].transform.position; // ���ֻ��һ���������ĵ㼴Ϊ�����λ��
        }

        var bounds = new Bounds(objects[0].transform.position, Vector3.zero);
        for (int i = 1; i < objects.Length; i++)
        {
            bounds.Encapsulate(objects[i].transform.position); // ��չ�߽��԰���ÿ�������λ��
        }
        return bounds.center; // ���ر߽�����ĵ㣬�����ж�������ĵ�
    }
    
    /// <summary>
    /// ��ȡ���֮��������
    /// </summary>
    /// <param name="players"></param>
    /// <returns></returns>
    float GetGreatestDistance(GameObject[] players)
    {
        var bounds = new Bounds(players[0].transform.position, Vector3.zero);
        for (int i = 1; i < players.Length; i++)
        {
            bounds.Encapsulate(players[i].transform.position);
        }
        return bounds.size.x; // ����X���ϵľ�����Ϊ�����룬Ҳ����ѡ��ʹ�ñ߽�����ά��
    }
}
