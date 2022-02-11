using System;
using UnityEngine;

namespace HQFPSTemplate
{
    public class Activity
    {
        public bool Active { get { return m_Active; } }

        public float LastStartTime { get; private set; }
        public float LastEndTime { get; private set; }

        private event TryerDelegate m_StartTryers;
        private event TryerDelegate m_StopTryers;
        private event Action m_StartCallbacks;
        private event Action m_StopCallbacks;

        private bool m_Active = false;


        /// <summary>
        /// Will be called when this activity starts.
        /// </summary>
        public void AddStartListener(Action listener)
        {
            m_StartCallbacks += listener;
        }

        /// <summary>
        /// Will be called when this activity stops.
        /// </summary>
        public void AddStopListener(Action listener)
        {
            m_StopCallbacks += listener;
        }

        public void SetStartTryer(TryerDelegate tryer)
        {
            m_StartTryers = tryer;
        }

        public void SetStopTryer(TryerDelegate tryer)
        {
            m_StopTryers = tryer;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ForceStart()
        {
            if (m_Active)
                return;

            m_Active = true;

            if (m_StartCallbacks != null)
            {
                m_StartCallbacks();
                LastStartTime = Time.time;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool TryStart(bool bypassState = false)
        {
            if (m_Active && !bypassState)
                return false;

            if (m_StartTryers != null)
            {
                bool activityStarted = CallStartApprovers();

                if (activityStarted)
                    m_Active = true;

                if (activityStarted && m_StartCallbacks != null)
                {
                    m_StartCallbacks();
                    LastStartTime = Time.time;
                }

                return activityStarted;
            }
            else
                Debug.LogWarning("[Activity] - You tried to start an activity which has no tryer (if no one checks if the activity can start, it won't start).");

            return false;
        }

        /// <summary>
        ///
        /// </summary>
        public bool TryStop()
        {
            if (!m_Active)
                return false;

            if (m_StopTryers != null)
            {
                if (CallStopApprovers())
                {
                    m_Active = false;

                    if (m_StopCallbacks != null)
                    {
                        m_StopCallbacks();
                        LastEndTime = Time.time;
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The activity will stop immediately.
        /// </summary>
        public void ForceStop()
        {
            if (!m_Active)
                return;

            m_Active = false;

            if (m_StopCallbacks != null)
            {
                m_StopCallbacks();
                LastEndTime = Time.time;
            }
        }

        public void RemoveStartListener(Action listener) 
        {
            m_StartCallbacks -= listener;
        }

        public void RemoveStopListener(Action listener)
        {
            m_StopCallbacks -= listener;
        }

        private bool CallStartApprovers()
        {
            var invocationList = m_StartTryers.GetInvocationList();

            for (int i = 0; i < invocationList.Length; i++)
            {
                if (!(bool)invocationList[i].DynamicInvoke())
                    return false;
            }

            return true;
        }

        private bool CallStopApprovers()
        {
            var invocationList = m_StopTryers.GetInvocationList();

            for (int i = 0; i < invocationList.Length; i++)
            {
                if (!(bool)invocationList[i].DynamicInvoke())
                    return false;
            }

            return true;
        }
    }

    public class Activity<T>
    {
        public delegate bool ActivityTryerDelegate(T parameter);

        public bool Active { get { return m_Active; } }
        public T Parameter { get { return m_Parameter; } }

        public float LastStartTime { get; private set; }
        public float LastEndTime { get; private set; }

        private event ActivityTryerDelegate m_StartTryers;
        private event ActivityTryerDelegate m_StopTryers;
        private event Action m_StartCallbacks;
        private event Action m_StopCallbacks;

        private T m_Parameter;

        private bool m_Active = false;


        public void SetStartTryer(ActivityTryerDelegate tryer)
        {
            m_StartTryers = tryer;
        }

        public void SetStopTryer(ActivityTryerDelegate tryer)
        {
            m_StopTryers = tryer;
        }

        /// <summary>
        /// Will be called when this activity starts.
        /// </summary>
        public void AddStartListener(Action listener)
        {
            m_StartCallbacks += listener;
        }

        /// <summary>
        /// Will be called when this activity stops.
        /// </summary>
        public void AddStopListener(Action listener)
        {
            m_StopCallbacks += listener;
        }

        /// <summary>
        ///
        /// </summary>
        public void ForceStart(T parameter)
        {
            if (m_Active)
                return;

            m_Active = true;
            m_Parameter = parameter;

            if (m_StartCallbacks != null)
            {
                m_StartCallbacks();
                LastStartTime = Time.time;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool TryStart(T parameter)
        {
            if (m_Active)
                return false;

            if (m_StartTryers != null)
            {
                bool activityStarted = CallStartApprovers(parameter);

                if (activityStarted)
                {
                    m_Active = true;
                    m_Parameter = parameter;
                }

                if (activityStarted && m_StartCallbacks != null)
                {
                    m_StartCallbacks();
                    LastStartTime = Time.time;
                }

                return activityStarted;
            }
            else
                Debug.LogWarning("[Activity] - You tried to start an activity which has no tryer (if no one checks if the activity can start, it won't start).");

            return false;
        }

        /// <summary>
        ///
        /// </summary>
        public bool TryStop()
        {
            if (!m_Active)
                return false;

            if (m_StopTryers != null)
            {
                if (CallStopApprovers(m_Parameter))
                {
                    m_Active = false;

                    if (m_StopCallbacks != null)
                    {
                        m_StopCallbacks();
                        LastEndTime = Time.time;
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The activity will stop immediately.
        /// </summary>
        public void ForceStop()
        {
            if (!m_Active)
                return;

            m_Active = false;

            if (m_StopCallbacks != null)
            {
                m_StopCallbacks();
                LastEndTime = Time.time;
            }
        }

        public void RemoveStartListener(Action listener)
        {
            m_StartCallbacks -= listener;
        }

        public void RemoveStopListener(Action listener)
        {
            m_StopCallbacks -= listener;
        }

        private bool CallStartApprovers(T parameter)
        {
            var invocationList = m_StartTryers.GetInvocationList();

            for (int i = 0; i < invocationList.Length; i++)
            {
                if (!(bool)invocationList[i].DynamicInvoke(parameter))
                    return false;
            }

            return true;
        }

        private bool CallStopApprovers(T parameter)
        {
            var invocationList = m_StopTryers.GetInvocationList();
            for (int i = 0; i < invocationList.Length; i++)
            {
                if (!(bool)invocationList[i].DynamicInvoke(parameter))
                    return false;
            }

            return true;
        }
    }
}