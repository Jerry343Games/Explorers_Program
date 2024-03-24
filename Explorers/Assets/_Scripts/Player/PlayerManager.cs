using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum OptionalFeature
{
    Logistics,//��ػ����˹��� ����
    Charging,//��ػ����˹��� ����
    Salvo,//�����ͻ����˹��� ����
    DestroyTorpedoes,//�����ͻ����˹��� ��������
    Dash,//�����ͻ����˹��� ��ײ
    SonicWave,//�����ͻ����˹��� ������
    TranquilizerGun,//�����ͻ����˹��� ����ǹ
    FloatingFort,//�����ͻ����˹��� ������̨
}
public class PlayerManager : SingletonPersistent<PlayerManager>
{
    public List<GameObject> players = new List<GameObject>();

    //������ɫ���� ֵ��ѡ��ļ���
    public Dictionary<int, OptionalFeature> playerFeaturesDic = new Dictionary<int, OptionalFeature>();

}
