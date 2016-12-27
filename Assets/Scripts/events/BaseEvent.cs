using System;

public class BaseEvent : IEvent
{
    private object _data;
    public object Data
    {
        get
        {
            throw new NotImplementedException();
        }

        set
        {
            throw new NotImplementedException();
        }
    }

    private string _type;
    public string Type
    {
        get
        {
            throw new NotImplementedException();
        }        
    }

    public BaseEvent(string type, object data=null)
    {
        _type = type;
        _data = data;
    }
}
