using System.Collections.Generic;
using System;

public enum PlayEventType
{
    // OWNER_TYPE[_SUB]
    EMPTY_EVENT,
    CONTROL_NAVIGATE,

    PLAYER_PLACE_ROAD,
    PLAYER_PLACE_GRASS,
    PLAYER_PLACE_FACTORY,


    PLAYER_DEFEAT

}


public class EventDispatcher
{

    public EventDispatcher()
    {
        events = new Dictionary<PlayEventType, List<Action>>();
    }
    private Dictionary<PlayEventType, List<Action>> events;


    public void AddEventListener(PlayEventType type, Action callback)
    {
        if (!events.ContainsKey(type))
            events[type] = new List<Action>();
        events[type].Add(callback);
    }
    public void RemoveEventListener(PlayEventType type, Action callback)
    {
        if (!events.ContainsKey(type))
            return;
        events[type].Remove(callback);
    }

    public void SendEvent(PlayEventType type)
    {
        if (!events.ContainsKey(type))
            return;
        foreach (var _event in events[type])
        {
            _event();
        }
    }


}