using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Message
{
    public class Message
    {
        Action action;
        public float LastCallTime => m_CallTime;
        private float m_CallTime;

        public void AddListener(Action _action)
        {
            action += _action;
        }
        public void RemoveListener(Action _action)
        {
            action -= _action;
        }
        public void Clear()
        {
            action = null;
        }

        public void Send()
        {
            m_CallTime = Time.time;
            action?.Invoke();
        }
    }
    public class Message<T>
    {
        Action<T> action;
        public float LastCallTime => m_CallTime;
        private float m_CallTime;

        public void AddListener(Action<T> _action)
        {
            action += _action;
        }
        public void RemoveListener(Action<T> _action)
        {
            action -= _action;
        }
        public void Clear()
        {
            action = null;
        }

        public void Send(T T)
        {
            m_CallTime = Time.time;
            action?.Invoke(T);
        }
    }
    public class Message<T, V>
    {
        Action<T, V> action;
        public float LastCallTime => m_CallTime;
        private float m_CallTime;

        public void AddListener(Action<T,V> _action)
        {
            action += _action;
        }
        public void RemoveListener(Action<T, V> _action)
        {
            action -= _action;
        }
        public void Clear()
        {
            action = null;
        }

        public void Send(T T, V V)
        {
            m_CallTime = Time.time;
            action?.Invoke(T,V);
        }
    }
    public class Message<T, V, G>
    {
        Action<T, V, G> action;
        public float LastCallTime => m_CallTime;
        private float m_CallTime;

        public void AddListener(Action<T, V, G> _action)
        {
            action += _action;
        }
        public void RemoveListener(Action<T, V, G> _action)
        {
            action -= _action;
        }
        public void Clear()
        {
            action = null;
        }

        public void Send(T T, V V, G G)
        {
            m_CallTime = Time.time;
            action?.Invoke(T,V, G);
        }
    }
}
