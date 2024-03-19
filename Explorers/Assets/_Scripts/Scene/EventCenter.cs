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

    public static void CallGameStartedEvent()
    {
        GameStartedEvent?.Invoke();
    }

    public static void CallPlayerRegisteredEvent()
    {
        PlayerRegistered?.Invoke();
    }

}
