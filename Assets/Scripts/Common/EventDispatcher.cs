using System.Collections.Generic;
using System;

public enum PlayEventType
{
    // OWNER_TYPE[_SUB]
    EMPTY_EVENT = 0,
    CONTROL_NAVIGATE = 1,

    PLAYER_PLACE = 101,
    PLAYER_PLACE_ROAD = 102,
    PLAYER_PLACE_GRASS = 103,
    PLAYER_PLACE_FACTORY = 104,
    PLAYER_PLACE_DEFEND = 105,
    PLAYER_PLACE_STORAGE = 106,


    PLAYER_KILL = 110,
    PLAYER_KILL_LAZER = 111,
    PLAYER_KILL_GIANT = 112,

    PLAYER_MOVE = 120,

    PLAYER_DEFEAT = 121,

    HEALTH_BEATTACKED = 131,

    PIECE_DAMAGE = 151,
    PIECE_DAMAGE_LAZER = 152,

    PIECE_DAMAGE_GIANT = 153,
    PIECE_DAMAGE_TOWER = 154,
    // PIECE_DAMAGE = 155,

    GAME_ENTER_SWITCH = 200,
    GAME_ENTER_DAY = 201,
    GAME_ENTER_NIGHT = 202

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
