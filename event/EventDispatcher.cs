﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void EventCallback(string evt, System.Object data);

public struct DispatchEvent
{
    public string evt;
    public object target;
    public EventCallback callback;
    public DispatchEvent(string e, object t, EventCallback c)
    {
        evt = e;
        target = t;
        callback =c;
    }
    public bool Equals(DispatchEvent e)
    {
        return e.evt.Equals(evt) && e.target == target && e.callback == callback;
    }
}

public class EventDispatcher : BaseObject
{
    Dictionary<string, Dictionary<object, DispatchEvent>> mStringEventMap;
    protected override void OnEnable()
    {
        base.OnEnable();
        mStringEventMap = new Dictionary<string, Dictionary<object, DispatchEvent>>();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        mStringEventMap.Clear();
    }

    public void dispatchEvent(string evt)
    {
        dispatch(evt, null);
    }

    public void dispatchEvent(string evt, object data)
    {
        if (mStringEventMap.ContainsKey(evt))
        {
            Dictionary<object, DispatchEvent> target2Event = mStringEventMap[evt];
            foreach(object key in target2Event.Keys)
            {
                DispatchEvent devt = target2Event[key];
                devt.callback(evt, data);
            }
        }
    }

    public void mapEvent(string evt, object target, EventCallback callback)
    {
        mapEvent(new DispatchEvent(evt, target, callback));
    }

    public void mapEvent(DispatchEvent devt)
    {
        Dictionary<object, DispatchEvent> target2Event = null;
        if (!mStringEventMap.ContainsKey(devt.evt))
        {
            mStringEventMap[devt.evt] = new Dictionary<object, DispatchEvent>();
        }
        target2Event = mStringEventMap[devt.evt];
        target2Event[devt.target] = devt;
    }

    public void unmapEvent(string evt, object target, EventCallback callback)
    {
        if (mStringEventMap.ContainsKey(evt))
        {
            Dictionary<object, DispatchEvent> target2Event = mStringEventMap[evt];
            if (target2Event.ContainsKey(target))
            {
                target2Event.Remove(target);
                if(target2Event.Count == 0)
                {
                    mStringEventMap.Remove(evt);
                }
            }
        }
    }

    public void unmapEvent(DispatchEvent devt)
    {
        unmapEvent(devt.evt, devt.target, devt.callback);
    }

    public void unmapAllEvents(object target)
    {
        if(mStringEventMap != null) {
            ArrayList needRemoveEvt = new ArrayList();
            foreach(string evt in mStringEventMap.Keys)
            {
                Dictionary<object, DispatchEvent> dict = mStringEventMap[evt];
                if (dict.ContainsKey(target))
                {
                    dict.Remove(target);
                    if(dict.Count == 0)
                    {
                        needRemoveEvt.Add(evt);
                    }
                }
            }
            //清理空集合
            foreach(string evt in needRemoveEvt)
            {
                mStringEventMap.Remove(evt);
            }
        }
    }
}