using System;

/// <summary>
/// 事件中心
/// </summary>
public static class EventCenter
{
    //游戏开始事件
    public static event Action GameStartedEvent;

    public static void CallGameStartedEvent()
    {
        GameStartedEvent?.Invoke();
    }

}
