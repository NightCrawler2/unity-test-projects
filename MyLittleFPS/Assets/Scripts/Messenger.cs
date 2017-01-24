﻿// Messenger.cs v1.0 by Magnus Wolffelt, magnus.wolffelt@gmail.com
// Version 1.4 by Julie Iaccarino, biscuitWizard @ github.com
//
// Inspired by and based on Rod Hyde's Messenger:
// http://www.unifycommunity.com/wiki/index.php?title=CSharpMessenger
//
// This is a C# messenger (notification center). It uses delegates
// and generics to provide type-checked messaging between event producers and
// event consumers, without the need for producers or consumers to be aware of
// each other. The major improvement from Hyde's implementation is that
// there is more extensive error detection, preventing silent bugs.
//
// Usage example:
// Messenger<float>.AddListener("myEvent", MyEventHandler);
// ...
// Messenger<float>.Broadcast("myEvent", 1.0f);
//
// Callback example:
// Messenger<float>.AddListener<string>("myEvent", MyEventHandler);
// private string MyEventHandler(float f1) { return "Test " + f1; }
// ...
// Messenger<float>.Broadcast<string>("myEvent", 1.0f, MyEventCallback);
// private void MyEventCallback(string s1) { Debug.Log(s1"); }

using System;
using System.Collections.Generic;
using System.Linq;

public enum MessengerMode {
    DontRequireListener,
    RequireListener
}

internal static class MessengerInternal {
    public static readonly Dictionary<string, Delegate> EventTable = new Dictionary<string, Delegate>();
    public static readonly MessengerMode DefaultMode = MessengerMode.RequireListener;

    public static void AddListener(string eventType, Delegate callback) {
        OnListenerAdding(eventType, callback);
        EventTable[eventType] = Delegate.Combine(EventTable[eventType], callback);
    }

    public static void RemoveListener(string eventType, Delegate handler) {
        OnListenerRemoving(eventType, handler);
        EventTable[eventType] = Delegate.Remove(EventTable[eventType], handler);
        OnListenerRemoved(eventType);
    }

    public static T[] GetInvocationList<T>(string eventType) {
        Delegate d;
        if (!EventTable.TryGetValue(eventType, out d)) return null;
        if(d != null) {
            return d.GetInvocationList().Cast<T>().ToArray();
        } else {
            throw CreateBroadcastSignatureException(eventType);
        }
    }

    public static void OnListenerAdding(string eventType, Delegate listenerBeingAdded) {
        if (!EventTable.ContainsKey(eventType)) {
            EventTable.Add(eventType, null);
        }

        var d = EventTable[eventType];
        if (d != null && d.GetType() != listenerBeingAdded.GetType()) {
            throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
        }
    }

    public static void OnListenerRemoving(string eventType, Delegate listenerBeingRemoved) {
        if (EventTable.ContainsKey(eventType)) {
            var d = EventTable[eventType];

            if (d == null) {
                throw new ListenerException(string.Format("Attempting to remove listener with for event type {0} but current listener is null.", eventType));
            } else if (d.GetType() != listenerBeingRemoved.GetType()) {
                throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
            }
        } else {
            throw new ListenerException(string.Format("Attempting to remove listener for type {0} but Messenger doesn't know about this event type.", eventType));
        }
    }

    public static void OnListenerRemoved(string eventType) {
        if (EventTable[eventType] == null) {
            EventTable.Remove(eventType);
        }
    }

    public static void OnBroadcasting(string eventType, MessengerMode mode) {
        if (mode == MessengerMode.RequireListener && !EventTable.ContainsKey(eventType)) {
            throw new BroadcastException(string.Format("Broadcasting message {0} but no listener found.", eventType));
        }
    }

    public static BroadcastException CreateBroadcastSignatureException(string eventType) {
        return new BroadcastException(string.Format("Broadcasting message {0} but listeners have a different signature than the broadcaster.", eventType));
    }

    public class BroadcastException : Exception {
        public BroadcastException(string msg)
            : base(msg) {
        }
    }

    public class ListenerException : Exception {
        public ListenerException(string msg)
            : base(msg) {
        }
    }
}

// No parameters
public static class Messenger {
    public static void AddListener(string eventType, Action handler) {
        MessengerInternal.AddListener(eventType, handler);
    }

    public static void AddListener<TReturn>(string eventType, Func<TReturn> handler) {
        MessengerInternal.AddListener(eventType, handler);
    }

    public static void RemoveListener(string eventType, Action handler) {
        MessengerInternal.RemoveListener(eventType, handler);
    }

    public static void RemoveListener<TReturn>(string eventType, Func<TReturn> handler) {
        MessengerInternal.RemoveListener(eventType, handler);
    }

    public static void Broadcast(string eventType) {
        Broadcast(eventType, MessengerInternal.DefaultMode);
    }

    public static void Broadcast<TReturn>(string eventType, Action<TReturn> returnCall) {
        Broadcast(eventType, returnCall, MessengerInternal.DefaultMode);
    }

    public static void Broadcast(string eventType, MessengerMode mode) {
        MessengerInternal.OnBroadcasting(eventType, mode);
        var invocationList = MessengerInternal.GetInvocationList<Action>(eventType);

        foreach(var callback in invocationList)
            callback.Invoke();
    }

    public static void Broadcast<TReturn>(string eventType, Action<TReturn> returnCall, MessengerMode mode) {
        MessengerInternal.OnBroadcasting(eventType, mode);
        var invocationList = MessengerInternal.GetInvocationList<Func<TReturn>>(eventType);

        foreach(var result in invocationList.Select(del => del.Invoke())) {
            returnCall.Invoke(result);
        }
    }
}

// One parameter
public static class Messenger<T> {
    public static void AddListener(string eventType, Action<T> handler) {
        MessengerInternal.AddListener(eventType, handler);
    }

    public static void AddListener<TReturn>(string eventType, Func<T, TReturn> handler) {
        MessengerInternal.AddListener(eventType, handler);
    }

    public static void RemoveListener(string eventType, Action<T> handler) {
        MessengerInternal.RemoveListener(eventType, handler);
    }

    public static void RemoveListener<TReturn>(string eventType, Func<T, TReturn> handler) {
        MessengerInternal.RemoveListener(eventType, handler);
    }

    public static void Broadcast(string eventType, T arg1) {
        Broadcast(eventType, arg1, MessengerInternal.DefaultMode);
    }

    public static void Broadcast<TReturn>(string eventType, T arg1, Action<TReturn> returnCall) {
        Broadcast(eventType, arg1, returnCall, MessengerInternal.DefaultMode);
    }

    public static void Broadcast(string eventType, T arg1, MessengerMode mode) {
        MessengerInternal.OnBroadcasting(eventType, mode);
        var invocationList = MessengerInternal.GetInvocationList<Action<T>>(eventType);

        foreach(var callback in invocationList)
            callback.Invoke(arg1);
    }

    public static void Broadcast<TReturn>(string eventType, T arg1, Action<TReturn> returnCall, MessengerMode mode) {
        MessengerInternal.OnBroadcasting(eventType, mode);
        var invocationList = MessengerInternal.GetInvocationList<Func<T, TReturn>>(eventType);

        foreach(var result in invocationList.Select(del => del.Invoke(arg1))) {
            returnCall.Invoke(result);
        }
    }
}


// Two parameters
public static class Messenger<T, TU> {
    public static void AddListener(string eventType, Action<T, TU> handler) {
        MessengerInternal.AddListener(eventType, handler);
    }

    public static void AddListener<TReturn>(string eventType, Func<T, TU, TReturn> handler) {
        MessengerInternal.AddListener(eventType, handler);
    }

    public static void RemoveListener(string eventType, Action<T, TU> handler) {
        MessengerInternal.RemoveListener(eventType, handler);
    }

    public static void RemoveListener<TReturn>(string eventType, Func<T, TU, TReturn> handler) {
        MessengerInternal.RemoveListener(eventType, handler);
    }

    public static void Broadcast(string eventType, T arg1, TU arg2) {
        Broadcast(eventType, arg1, arg2, MessengerInternal.DefaultMode);
    }

    public static void Broadcast<TReturn>(string eventType, T arg1, TU arg2, Action<TReturn> returnCall) {
        Broadcast(eventType, arg1, arg2, returnCall, MessengerInternal.DefaultMode);
    }

    public static void Broadcast(string eventType, T arg1, TU arg2, MessengerMode mode) {
        MessengerInternal.OnBroadcasting(eventType, mode);
        var invocationList = MessengerInternal.GetInvocationList<Action<T, TU>>(eventType);

        foreach(var callback in invocationList)
            callback.Invoke(arg1, arg2);
    }

    public static void Broadcast<TReturn>(string eventType, T arg1, TU arg2, Action<TReturn> returnCall, MessengerMode mode) {
        MessengerInternal.OnBroadcasting(eventType, mode);
        var invocationList = MessengerInternal.GetInvocationList<Func<T, TU, TReturn>>(eventType);

        foreach(var result in invocationList.Select(del => del.Invoke(arg1, arg2))) {
            returnCall.Invoke(result);
        }
    }
}


// Three parameters
public static class Messenger<T, TU, TV> {
    public static void AddListener(string eventType, Action<T, TU, TV> handler) {
        MessengerInternal.AddListener(eventType, handler);
    }

    public static void AddListener<TReturn>(string eventType, Func<T, TU, TV, TReturn> handler) {
        MessengerInternal.AddListener(eventType, handler);
    }

    public static void RemoveListener(string eventType, Action<T, TU, TV> handler) {
        MessengerInternal.RemoveListener(eventType, handler);
    }

    public static void RemoveListener<TReturn>(string eventType, Func<T, TU, TV, TReturn> handler) {
        MessengerInternal.RemoveListener(eventType, handler);
    }

    public static void Broadcast(string eventType, T arg1, TU arg2, TV arg3) {
        Broadcast(eventType, arg1, arg2, arg3, MessengerInternal.DefaultMode);
    }

    public static void Broadcast<TReturn>(string eventType, T arg1, TU arg2, TV arg3, Action<TReturn> returnCall) {
        Broadcast(eventType, arg1, arg2, arg3, returnCall, MessengerInternal.DefaultMode);
    }

    public static void Broadcast(string eventType, T arg1, TU arg2, TV arg3, MessengerMode mode) {
        MessengerInternal.OnBroadcasting(eventType, mode);
        var invocationList = MessengerInternal.GetInvocationList<Action<T, TU, TV>>(eventType);

        foreach(var callback in invocationList)
            callback.Invoke(arg1, arg2, arg3);
    }

    public static void Broadcast<TReturn>(string eventType, T arg1, TU arg2, TV arg3, Action<TReturn> returnCall, MessengerMode mode) {
        MessengerInternal.OnBroadcasting(eventType, mode);
        var invocationList = MessengerInternal.GetInvocationList<Func<T, TU, TV, TReturn>>(eventType);

        foreach(var result in invocationList.Select(del => del.Invoke(arg1, arg2, arg3))) {
            returnCall.Invoke(result);
        }
    }
}