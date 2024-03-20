using System;

/// <summary>
/// 事件中心
/// </summary>
public static class EventCenter
{
    //游戏开始事件
    public static event Action GameStartedEvent;

    //玩家注册事件
    public static event Action PlayerRegistered; 
    
    //主电池加入事件
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
