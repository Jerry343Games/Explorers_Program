using System;

/// <summary>
/// �¼�����
/// </summary>
public static class EventCenter
{
    //��Ϸ��ʼ�¼�
    public static event Action GameStartedEvent;

    //���ע���¼�
    public static event Action PlayerRegistered; 
    
    //����ؼ����¼�
    public static event Action BattaryJoined;
    
    public static void CallGameStartedEvent()
    {
        GameStartedEvent?.Invoke();
    }

    public static void CallPlayerRegisteredEvent()
    {
        PlayerRegistered?.Invoke();
    }

    public static void OnBattaryJoin()
    {
        BattaryJoined?.Invoke();
    }

}
