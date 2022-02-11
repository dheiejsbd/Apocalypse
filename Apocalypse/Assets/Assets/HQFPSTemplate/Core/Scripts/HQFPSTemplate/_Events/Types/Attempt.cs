using System;
using UnityEngine;

namespace HQFPSTemplate
{
	public delegate bool TryerDelegate();

	/// <summary>
	/// 
	/// </summary>
	public class Attempt
	{
		public float LastExecutionTime => m_LastExecutionTime;

		private TryerDelegate m_Tryer;
		private Action m_Listeners;

		private float m_LastExecutionTime;


		/// <summary>
		/// Registers a method that will try to execute this action.
		/// NOTE: Only 1 tryer is allowed!
		/// </summary>
		public void SetTryer(TryerDelegate tryer)
		{
			m_Tryer = tryer;
		}

		public void AddListener(Action listener)
		{
			m_Listeners += listener;
		}

		public void RemoveListener(Action listener)
		{
			m_Listeners -= listener;
		}

		public bool Try()
		{
			bool wasSuccessful = (m_Tryer == null || m_Tryer());
			if(wasSuccessful)
			{
				if (m_Listeners != null)
				{
					m_Listeners();

					m_LastExecutionTime = Time.time;
				}
				return true;
			}

			return false;
		}
	}

	public class Attempt<T>
	{
		public float LastExecutionTime => m_LastExecutionTime;

		public delegate bool GenericTryerDelegate(T arg);

		private GenericTryerDelegate m_Tryer;
		private Action<T> m_Listeners;
		
		private float m_LastExecutionTime;


		/// <summary>
		/// Registers a method that will try to execute this action.
		/// NOTE: Only 1 tryer is allowed!
		/// </summary>
		public void SetTryer(GenericTryerDelegate tryer)
		{
			m_Tryer = tryer;
		}

		public void AddListener(Action<T> listener)
		{
			m_Listeners += listener;
		}

		public void RemoveListener(Action<T> listener)
		{
			m_Listeners -= listener;
		}

		public bool Try(T arg)
		{
			bool succeeded = m_Tryer != null && m_Tryer(arg);
			if(succeeded)
			{
				if (m_Listeners != null)
				{
					m_Listeners(arg);

					m_LastExecutionTime = Time.time;
				}
				return true;
			}

			return false;
		}
	}

	public class Attempt<T, V>
	{
		public float LastExecutionTime => m_LastExecutionTime;
		public delegate bool GenericTryerDelegate(T arg1, V arg2);

		private GenericTryerDelegate m_Tryer;
		private Action<T, V> m_Listeners;

		private float m_LastExecutionTime;


		/// <summary>
		/// Registers a method that will try to execute this action.
		/// NOTE: Only 1 tryer is allowed!
		/// </summary>
		public void SetTryer(GenericTryerDelegate tryer)
		{
			m_Tryer = tryer;
		}

		public void AddListener(Action<T, V> listener)
		{
			m_Listeners += listener;
		}

		public void RemoveListener(Action<T, V> listener)
		{
			m_Listeners -= listener;
		}

		public bool Try(T arg1, V arg2)
		{
			bool succeeded = m_Tryer != null && m_Tryer(arg1, arg2);
			if(succeeded)
			{
				if (m_Listeners != null)
				{
					m_Listeners(arg1, arg2);

					m_LastExecutionTime = Time.time;
				}
				return true;
			}

			return false;
		}
	}
}