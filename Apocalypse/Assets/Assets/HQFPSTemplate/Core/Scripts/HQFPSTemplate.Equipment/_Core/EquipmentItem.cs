using System;
using UnityEngine;
using HQFPSTemplate.Items;
using UnityEngine.Events;

namespace HQFPSTemplate.Equipment
{
	[Serializable]
	public class DelayedCameraForce : ICloneable
	{
		public float Delay = 0f;
		public SpringForce Force = new SpringForce();

		public object Clone() => MemberwiseClone();
	}

	[Serializable]
	public class DelayedSound : ICloneable
	{
		public float Delay;
		public SoundPlayer Sound;

		public object Clone() => MemberwiseClone();
	}

	[Serializable]
	public class QueuedCameraForce
	{
		public DelayedCameraForce DelayedForce { get; private set; }
		public float PlayTime { get; private set; }

		public QueuedCameraForce(DelayedCameraForce force, float playTime)
		{
			DelayedForce = force;
			PlayTime = playTime;
		}
	}

	public class QueuedSound
	{
		public DelayedSound DelayedSound { get; private set; }
		public float PlayTime { get; private set; }

		public QueuedSound(DelayedSound clip, float playTime)
		{
			DelayedSound = clip;
			PlayTime = playTime;
		}
	}

	/// <summary>
	/// An object which can be used by a character.
	/// Base class from which weapon and tool scripts derive.
	/// </summary>
	public abstract partial class EquipmentItem : PlayerComponent
	{
		#region Internal
		[Serializable]
		public class GeneralInfo
		{
			// General
			[DatabaseItem]
			public string CorrespondingItem;

			[Space(4f)]
			public EquipmentItemInfo EquipmentInfo = null;

			// Animation
			[Space(4f)]
			[BHeader("Animation", false, order = 2)]

			public EquipmentAnimationInfo EquipmentAnimationInfo = null;

			[EnableIf("EquipmentAnimationInfo", true, 6f)]
			public Animator Animator = null;

			// Physics
			[Space(4f)]
			[BHeader("Physics", false, order = 2)]

			public EquipmentPhysicsInfo EquipmentPhysicsInfo = null;

			[EnableIf("EquipmentPhysicsInfo", true, 6f)]
			public Transform PhysicsPivot = null;

			// Model (Skin Handler)
			[Space(4f)]
			[BHeader("Model", false, order = 2)]

			public EquipmentModelHandler EquipmentModel = null;
		}

		[Serializable]
		public class GeneralEvents
		{
			[Serializable]
			public class SimpleBoolEvent : UnityEvent<bool>
			{ }

			[BHeader("Equipped / Unequipped", true)]
			public SimpleBoolEvent OnEquipped = new SimpleBoolEvent();

			[BHeader("Reload Start / Reload Stop", true)]
			public SimpleBoolEvent OnReload = new SimpleBoolEvent();

			[BHeader("Aim Start / Aim Stop", true)]
			public SimpleBoolEvent OnAim = new SimpleBoolEvent();

			[Space]

			public UnityEvent OnUse;
			public UnityEvent OnChangeUseMode;
		}

		#endregion

		#region Anim Hashing
		//Hashed animator strings (Improves performance)
		private readonly int animHash_IdleIndex = Animator.StringToHash("Idle Index");
		private readonly int animHash_EquipSpeed = Animator.StringToHash("Equip Speed");
		private readonly int animHash_Equip = Animator.StringToHash("Equip");
		private readonly int animHash_Unequip = Animator.StringToHash("Unequip");
		private readonly int animHash_UnequipSpeed = Animator.StringToHash("Unequip Speed");
		#endregion

		//Properties
		public EquipmentHandler EHandler { get; private set; }
		public EquipmentModelHandler EModel => m_GeneralInfo.EquipmentModel;
		public EquipmentItemInfo EInfo => m_GeneralInfo.EquipmentInfo;
		public EquipmentAnimationInfo EAnimation => m_GeneralInfo.EquipmentAnimationInfo;
		public EquipmentPhysicsInfo EPhysics => m_GeneralInfo.EquipmentPhysicsInfo;
		public Transform PhysicsPivot { get { return m_GeneralInfo.PhysicsPivot; } }
		public Animator Animator => m_GeneralInfo.Animator;

		public string CorrespondingItemName => m_GeneralInfo.CorrespondingItem;

		[SerializeField, Group]
		public GeneralInfo m_GeneralInfo = null;

		[SerializeField, Group]
		public GeneralEvents m_GeneralEvents = null;

		// Using
		protected float m_UseThreshold = 0.1f;
		protected float m_NextTimeCanUse;

		// Aiming
		protected float m_NextTimeCanAim;


		// Setup Methods
        public virtual void Initialize(EquipmentHandler eHandler)
        {
			EHandler = eHandler;

			EAnimation.AssignEquipmentAnimation(Animator);
		}

        public virtual void Equip(Item item)
		{
			EAnimation.AssignArmAnimations(EHandler.FPArmsHandler.Animator);
			EHandler.Animator_SetTrigger(animHash_Equip);
			EHandler.Animator_SetFloat(animHash_UnequipSpeed, m_GeneralInfo.EquipmentInfo.Unequipping.AnimationSpeed);
			EHandler.Animator_SetFloat(animHash_EquipSpeed, m_GeneralInfo.EquipmentInfo.Equipping.AnimationSpeed);

			EHandler.PlayDelayedSounds(m_GeneralInfo.EquipmentInfo.Equipping.Audio);

			Player.Camera.Physics.PlayDelayedCameraForces(m_GeneralInfo.EquipmentInfo.Equipping.CameraForces);
			Player.Camera.Physics.AimHeadbobMod = m_GeneralInfo.EquipmentInfo.Aiming.AimCamHeadbobMod;

			m_GeneralInfo.EquipmentModel.UpdateSkinIDProperty(item);
			m_GeneralInfo.EquipmentModel.UpdateMaterialsFov();

			m_GeneralEvents.OnEquipped.Invoke(true);
		}

		public virtual void Unequip()
		{
			if(m_GeneralInfo.EquipmentInfo.Unequipping.Audio != null)
				EHandler.PlayPersistentAudio(m_GeneralInfo.EquipmentInfo.Unequipping.Audio[0].Sound, 1f, ItemSelection.Method.RandomExcludeLast);

			Player.Camera.Physics.PlayDelayedCameraForces(m_GeneralInfo.EquipmentInfo.Unequipping.CameraForces);

			EHandler.Animator_SetTrigger(animHash_Unequip);

			m_GeneralEvents.OnEquipped.Invoke(false);
		}

        // Using Methods
        public virtual bool TryUseOnce(Ray[] itemUseRays, int useType = 0) { return false; }
		public virtual bool TryUseContinuously(Ray[] itemUseRays, int useType = 0) { return false; }
		public virtual void OnUseStart() {; }
		public virtual void OnUseEnd() {; }
		public virtual bool TryChangeUseMode() { return false; }

		// Get Using Info
		public virtual float GetUseRaySpreadMod() { return 1f; }
		public virtual float GetTimeBetweenUses() { return m_UseThreshold; }
		public virtual bool CanBeUsed() { return true; } // E.g. Gun: has enough bullets in the magazine
		public virtual int GetUseRaysAmount() { return 1; }

		// Reloading Methods
		public virtual bool TryStartReload() { return false; }
		public virtual bool IsDoneReloading() { return false; }
		public virtual void OnReloadStop() {; }

		// Aiming Methods
		public virtual bool CanAim() => m_NextTimeCanAim < Time.time;
		public virtual void OnAimStart()
		{
			EHandler.Animator_SetInteger(animHash_IdleIndex, 0);
			m_NextTimeCanAim = Time.time + m_GeneralInfo.EquipmentInfo.Aiming.AimThreshold;

			m_GeneralEvents.OnAim.Invoke(true);
		}
		public virtual void OnAimStop() 
		{
			EHandler.Animator_SetInteger(animHash_IdleIndex, 1);

			m_GeneralEvents.OnAim.Invoke(false);
		}
    }
}