﻿using System.Diagnostics;
using Unity.Netcode;
using UnityNetMessages.Logging;

namespace UnityNetMessages.Events;

/// <summary>
/// Base class for all network events and messages.
/// </summary>
public abstract class MessageReceiver : IDisposable
{
    private bool _disposed;

    protected void RegisterEvent<TReturnType>()
    {
        uint? hash = GetHashInternal();
        if (hash is null)
            throw new NullReferenceException("Cannot register event as it is uninitialized.");

        NetworkMessaging.RegisterEvent<TReturnType>(hash.Value, this);
    }

    protected void UnregisterEvent()
    {
        uint? hash = GetHashInternal();
        if (hash is null) 
            return;
        
        NetworkMessaging.UnregisterEvent(hash.Value, this);
    }

    internal void Invoke(ulong senderId, FastBufferReader buffer)
    {
        OnReceiveMessage(senderId, buffer);
    }

    protected abstract uint? GetHash();
    protected abstract void OnReceiveMessage(ulong senderId, FastBufferReader buffer); // todo fix fast buffer not being readable multiple times
    
    protected uint? GetHashInternal()
    {
        return _disposed ? null : GetHash();
    }

    ~MessageReceiver()
    {
        Dispose();
    }
    
    /// <summary>
    /// Clears the wrapper, releasing the resources and making it unusable.
    /// Is automatic, it is not necessary to call it.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        DoDispose();
    }

    protected virtual void DoDispose()
    {
        UnregisterEvent();
    }
}