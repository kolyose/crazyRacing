using UnityEngine;
using System;
using System.Collections.Generic;

public class EventSystem : MonoBehaviour
{
    private Dictionary<string, Queue<IEvent>> _eventsPool;

    void Awake()
    {
        _eventsPool = new Dictionary<string, Queue<IEvent>>();
    }

    public void AddListener(string eventType, Action<IEvent> handler)
    {
        Messenger<IEvent>.AddListener(eventType, handler);
    }

    public void RemoveListener(string eventType, Action<IEvent> handler)
    {
        Messenger<IEvent>.RemoveListener(eventType, handler);
    }

    public void Dispatch(string eventType, object data=null)
    {
        IEvent evt;

        if (_eventsPool[eventType] == null)
            _eventsPool[eventType] = new Queue<IEvent>();

        if (_eventsPool[eventType].Count > 0)
        {
            evt = _eventsPool[eventType].Dequeue();
            evt.Data = data;
        }
        else
        {
            evt = new BaseEvent(eventType, data); //TODO: use factory here! like "factory.getEventByType(eventType, data)"
        }
        
        Messenger<IEvent>.Broadcast<IEvent>(eventType, evt, callback);
    }

    private void callback(IEvent evt)
    {
        _eventsPool[evt.Type].Enqueue(evt);
    }
}
