using System;

/// <summary>
/// �¼�����
/// </summary>
public static class EventCenter
{
    //��Ϸ��ʼ�¼�
    public static event Action GameStartedEvent;

    public static void CallGameStartedEvent()
    {
        GameStartedEvent?.Invoke();
    }

}
