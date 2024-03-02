using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ͨ���͵�������
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour  where T : MonoBehaviour
{
    private static T instance;

    public static T Instance => instance;

    protected virtual void Awake()
    {
        instance = this as T;
    }

}
