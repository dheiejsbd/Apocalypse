using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Massage
{
    public class Massage
    {
        Action action;
        public float LastCallTime => m_CallTime;
        private float m_CallTime;

        public void AddListner(Action _action)
        {
            action += action;
        }
        public void RemoveListner(Action _action)
        {
            action -= action;
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
    public class Massage<T>
    {
        Action<T> action;
        public float LastCallTime => m_CallTime;
        private float m_CallTime;

        public void AddListner(Action<T> _action)
        {
            action += action;
        }
        public void RemoveListner(Action<T> _action)
        {
            action -= action;
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
    public class Massage<T, V>
    {
        Action<T, V> action;
        public float LastCallTime => m_CallTime;
        private float m_CallTime;

        public void AddListner(Action<T,V> _action)
        {
            action += action;
        }
        public void RemoveListner(Action<T, V> _action)
        {
            action -= action;
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
    public class Massage<T, V, G>
    {
        Action<T, V, G> action;
        public float LastCallTime => m_CallTime;
        private float m_CallTime;

        public void AddListner(Action<T, V, G> _action)
        {
            action += action;
        }
        public void RemoveListner(Action<T, V, G> _action)
        {
            action -= action;
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
