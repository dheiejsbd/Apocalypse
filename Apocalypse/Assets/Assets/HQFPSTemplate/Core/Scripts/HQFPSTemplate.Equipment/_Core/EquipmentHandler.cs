using System.Collections.Generic;
using HQFPSTemplate.Items;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace HQFPSTemplate.Equipment
{
    /// <summary>
    /// Controls all of the 'equipment items' underneath it
    /// </summary>
    public class EquipmentHandler : PlayerComponent
    {
        #region Internal
        #pragma warning disable 0649
        [Serializable]
        public struct UseRaySpread
        {
            [Range(0.01f, 10f)]
            public float JumpSpreadMod,
                         RunSpreadMod,
                         CrouchSpreadMod,
                         ProneSpreadMod,
                         WalkSpreadMod,
                         AimSpreadMod;
        }

        [Serializable]
        public class ItemPropertiesDictionary
        {
            public string AmmoProperty => m_AmmoProperty;
            public string AmmoTypeProperty => m_AmmoTypeProperty;
            public string FireModeProperty => m_FireModeProperty;

            [DatabaseProperty]
            public string m_AmmoProperty = "Ammo";

            [DatabaseProperty]
            public string m_AmmoTypeProperty = "Ammo Type";

            [DatabaseProperty]
            public string m_FireModeProperty = "Fire Mode";
        }
        #pragma warning restore 0649
        #endregion

        //Events
        public int ContinuouslyUsedTimes { get => m_ContinuouslyUsedTimes; }
        public Message OnChangeItem = new Message();
        public Activity UsingItem = new Activity();

        //Properties
        public Transform ItemUseTransform => m_ItemUseTransform;

        public Item Item 
        {
            get { return m_AttachedItem != null ? m_AttachedItem : Player.EquippedItem.Get(); }
            protected set { m_AttachedItem = value; }
        }

        public ItemPropertiesDictionary ItemProperties => m_ItemProperties;
        public FPArmsHandler FPArmsHandler => m_FPArmsHandler;
        public EquipmentPhysicsHandler EPhysicsHandler => m_EquipmentPhysicsHandler;
        public EquipmentItem EquipmentItem { get { return m_AttachedEquipmentItem != null ? m_AttachedEquipmentItem : m_Unarmed; } }

        [SerializeField] protected EquipmentPhysicsHandler m_EquipmentPhysicsHandler;
        [SerializeField] protected FPArmsHandler m_FPArmsHandler;

        [Space]

        [SerializeField] protected Transform m_ItemUseTransform = null;

        [SerializeField]
        [Group("Inverse of Accuracy - ", true)]
        protected UseRaySpread m_UseRaySpread = new UseRaySpread();

        [SerializeField]
        [Group]
        protected ItemPropertiesDictionary m_ItemProperties = null;

        protected EquipmentItem m_AttachedEquipmentItem;
        protected Item m_AttachedItem;
        protected Unarmed m_Unarmed;

        protected AudioSource m_AudioSource;
        protected AudioSource m_PersistentAudioSource;

        protected int m_ContinuouslyUsedTimes = 0;
        protected float m_NextTimeCanUseItem = -1f;

        protected List<QueuedSound> m_QueuedSounds = new List<QueuedSound>();
        protected Dictionary<int, EquipmentItem> m_EquipmentItems = new Dictionary<int, EquipmentItem>();


        public EquipmentItem GetEquipmentItem(int itemId) 
        {
            if (itemId == 0)
                return m_Unarmed;

            if (m_EquipmentItems.TryGetValue(itemId, out EquipmentItem equipmentItem))
                return equipmentItem;
            else
                return null;
        }

        public virtual void Reset()
        {
            UnequipItem();

            m_AttachedEquipmentItem = null;
            m_AttachedItem = null;

            m_ContinuouslyUsedTimes = 0;
            m_NextTimeCanUseItem = -1f;

            ClearDelayedCamForces();
            ClearDelayedSounds();
        }

        public bool ContainsEquipmentItem(int itemId) => m_EquipmentItems.ContainsKey(itemId);

        public virtual void EquipItem(Item item)
        {
            ClearDelayedSounds();
            ClearDelayedCamForces();

            m_AttachedItem = item;

            // Disable previous equipment item
            if(m_AttachedEquipmentItem != null)
                m_AttachedEquipmentItem.gameObject.SetActive(false);

            int itemId = item != null ? item.Id : 0;

            // Enable next equipment item
            m_AttachedEquipmentItem = GetEquipmentItem(itemId);
            m_AttachedEquipmentItem.gameObject.SetActive(true);

            // Notify the item components (e.g. animation, physics etc.) present on the Equipment Item object
            var itemComponents = m_AttachedEquipmentItem.GetComponents<IEquipmentComponent>();

            if (itemComponents.Length > 0)
            {
                foreach (var component in itemComponents)
                    component.OnSelected();
            }

            SetCharacterMovementSpeed(Player.Aim.Active ? m_AttachedEquipmentItem.EInfo.Aiming.AimMovementSpeedMod : 1f);
            m_NextTimeCanUseItem = Time.time + m_AttachedEquipmentItem.EInfo.Equipping.Duration;

            OnChangeItem.Send();

            m_AttachedEquipmentItem.Equip(item);
        }

        public virtual void UnequipItem()
        {
            if (m_AttachedEquipmentItem == null)
                return;

            m_AttachedItem = null;
            m_NextTimeCanUseItem = Time.time + m_AttachedEquipmentItem.EInfo.Unequipping.Duration;

            EquipmentItem.Unequip();
        }

        public virtual bool TryUse(bool continuously, int useType = 0)
        {
            bool usedSuccessfully = false;

            if (m_NextTimeCanUseItem < Time.time)
            {
                // Use Rays (E.g Weapons with more projectiles per shot will need more rays - Shotguns)
                var itemUseRays = GenerateItemUseRays(Player, m_ItemUseTransform, m_AttachedEquipmentItem.GetUseRaysAmount(), m_AttachedEquipmentItem.GetUseRaySpreadMod());

                if (continuously)
                    usedSuccessfully = m_AttachedEquipmentItem.TryUseContinuously(itemUseRays, useType);
                else
                    usedSuccessfully = m_AttachedEquipmentItem.TryUseOnce(itemUseRays, useType);

                if (usedSuccessfully)
                {
                    if (!UsingItem.Active)
                    {
                        UsingItem.ForceStart();
                        EquipmentItem.OnUseStart();
                    }

                    //Increment the 'm_ContinuouslyUsedTimes' variable, which shows how many times the weapon has been used consecutively
                    if (UsingItem.Active)
                        m_ContinuouslyUsedTimes++;
                    else
                        m_ContinuouslyUsedTimes = 1;
                }
            }

            return usedSuccessfully;
        }

        public Ray[] GenerateItemUseRays(Humanoid humanoid, Transform anchor, int raysAmount, float equipmentSpreadMod)
        {
            var itemUseRays = new Ray[raysAmount];

            float spreadMod = 1f;

            if (humanoid != null)
            {
                if (humanoid.Jump.Active)
                    spreadMod *= m_UseRaySpread.JumpSpreadMod;
                else if (humanoid.Run.Active)
                    spreadMod *= m_UseRaySpread.RunSpreadMod;
                else if (humanoid.Crouch.Active)
                    spreadMod *= m_UseRaySpread.CrouchSpreadMod;
                else if (humanoid.Prone.Active)
                    spreadMod *= m_UseRaySpread.ProneSpreadMod;
                else if (humanoid.Walk.Active)
                    spreadMod *= m_UseRaySpread.WalkSpreadMod;

                if (humanoid.Aim.Active)
                    spreadMod *= m_UseRaySpread.AimSpreadMod;
            }

            float raySpread = equipmentSpreadMod * spreadMod;

            for (int i = 0; i < itemUseRays.Length; i++)
            {
                Vector3 raySpreadVector = anchor.TransformVector(new Vector3(Random.Range(-raySpread, raySpread), Random.Range(-raySpread, raySpread), 0f));
                Vector3 rayDirection = Quaternion.Euler(raySpreadVector) * anchor.forward;

                itemUseRays[i] = new Ray(anchor.position, rayDirection);
            }

            return itemUseRays;
        }

        public virtual bool TryStartAim()
        {
            if (m_NextTimeCanUseItem > Time.time ||
                (!m_AttachedEquipmentItem.EInfo.Aiming.AimWhileAirborne && !Player.IsGrounded.Get()) || // Can this item be aimed while airborne?
                !m_AttachedEquipmentItem.EInfo.Aiming.Enabled || !m_AttachedEquipmentItem.CanAim()) // Can this item be aimed?
                return false;

            SetCharacterMovementSpeed(m_AttachedEquipmentItem.EInfo.Aiming.AimMovementSpeedMod);
            m_AttachedEquipmentItem.OnAimStart();

            return true;
        }

        public virtual void OnAimStop()
        {
            SetCharacterMovementSpeed(1f);

            if (m_AttachedEquipmentItem != null)
                m_AttachedEquipmentItem.OnAimStop();
        }

        public virtual bool TryStartReload() => m_AttachedEquipmentItem.TryStartReload();

        public virtual bool IsDoneReloading() => m_AttachedEquipmentItem.IsDoneReloading();

        public virtual void OnReloadStop()
        {
            ClearDelayedCamForces();

            m_AttachedEquipmentItem.OnReloadStop();
        }

        public virtual void OnGroundedChange(bool grounded) 
        {
            if ((m_AttachedEquipmentItem != null) && (!grounded && !m_AttachedEquipmentItem.EInfo.Aiming.AimWhileAirborne))
                Player.Aim.ForceStop();
        }

        protected virtual void SetCharacterMovementSpeed(float multiplier)
        {
            Player.MovementSpeedFactor.Set(m_AttachedEquipmentItem.EInfo.General.MovementSpeedMod * multiplier);
        }

        protected virtual void Awake()
        {
            m_Unarmed = GetComponentInChildren<Unarmed>(true);

            m_EquipmentPhysicsHandler = GetComponent<EquipmentPhysicsHandler>();

            EquipmentItem[] equipmentItems = GetComponentsInChildren<EquipmentItem>(true);
            ItemInfo itemInfo;

            foreach (var eItem in equipmentItems)
            {
                itemInfo = ItemDatabase.GetItemByName(eItem.CorrespondingItemName);

                if (eItem != m_Unarmed)
                {
                    int id = itemInfo.Id;

                    //Initialize the equipment items
                    if (!m_EquipmentItems.ContainsKey(id))
                        m_EquipmentItems.Add(id, eItem);
                    else
                        Debug.LogWarning($"There are multiple equipment items that correspond to the same item under '{gameObject.name}'");
                }
                
                eItem.Initialize(this);

                // Notify the item components (e.g. animation, physics etc.) present on the Equipment Item object
                var itemComponents = eItem.gameObject.GetComponents<IEquipmentComponent>();

                if (itemComponents.Length > 0)
                {
                    foreach (var component in itemComponents)
                        component.Initialize(eItem);
                }

                eItem.gameObject.SetActive(false);
            }

            // Equipment Items AudioSource (For Overall first person items audio)
            m_AudioSource = AudioUtils.CreateAudioSource("Audio Source", transform, Vector3.zero, false, 1f, 1f);
            m_AudioSource.bypassEffects = m_AudioSource.bypassListenerEffects = m_AudioSource.bypassReverbZones = false;
            m_AudioSource.maxDistance = 500f;

            // Persistent AudioSource (e.g. used for the fire tail sounds)
            m_PersistentAudioSource = AudioUtils.CreateAudioSource("Persistent Audio Source", transform, Vector3.zero, true, 1f, 2.5f);
            m_PersistentAudioSource.bypassEffects = m_PersistentAudioSource.bypassListenerEffects = m_PersistentAudioSource.bypassReverbZones = false;
            m_PersistentAudioSource.maxDistance = 500f;
        }

        protected virtual void Update()
        {
            for (int i = 0; i < m_QueuedSounds.Count; i++)
            {
                if (Time.time >= m_QueuedSounds[i].PlayTime)
                {
                    m_QueuedSounds[i].DelayedSound.Sound.Play(ItemSelection.Method.RandomExcludeLast, m_AudioSource);
                    m_QueuedSounds.RemoveAt(i);
                }
            }

            if (m_AttachedEquipmentItem != null)
            {
                //Stop the UsingItem activity after a few miliseconds from being used (e.g. this will not stop the activity if an item being used continuously)
                if (Player.UseItem.LastExecutionTime + Mathf.Clamp(m_AttachedEquipmentItem.GetTimeBetweenUses() * 2f, 0f, 0.3f) < Time.time &&
                    UsingItem.Active)
                {
                    UsingItem.ForceStop();
                    m_AttachedEquipmentItem.OnUseEnd();
                    m_ContinuouslyUsedTimes = 0;
                }
            }
        }

        protected virtual void ClearDelayedCamForces() => Player.Camera.Physics.ClearQueuedCamForces();

        #region Audio

        public void PlayPersistentAudio(SoundPlayer soundPlayer, float volume, ItemSelection.Method selectionMethod = ItemSelection.Method.RandomExcludeLast)
        {
            soundPlayer.Play(selectionMethod, m_PersistentAudioSource, volume);
        }

        public void PlayPersistentAudio(AudioClip clip, float volume)
        {
            m_PersistentAudioSource.PlayOneShot(clip, volume);
        }

        public void ClearDelayedSounds() { m_QueuedSounds.Clear(); }

        public void PlayDelayedSound(DelayedSound delayedSound)
        {
            m_QueuedSounds.Add(new QueuedSound(delayedSound, Time.time + delayedSound.Delay));
        }

        public void PlayDelayedSounds(DelayedSound[] clipsData)
        {
            for (int i = 0; i < clipsData.Length; i++)
                PlayDelayedSound(clipsData[i]);
        }

        public void PlaySound(SoundPlayer soundPlayer, float volume, ItemSelection.Method selectionMethod = ItemSelection.Method.RandomExcludeLast)
        {
            soundPlayer.Play(selectionMethod, m_AudioSource, volume);
        }

        #endregion

        #region Animation
        public virtual void Animator_SetTrigger(string _string)
        {
            if (m_AttachedEquipmentItem.Animator) m_AttachedEquipmentItem.Animator.SetTrigger(_string);
            if (m_FPArmsHandler.Animator) m_FPArmsHandler.Animator.SetTrigger(_string);
        }

        public virtual void Animator_SetTrigger(int _hashCode)
        {
            if (m_AttachedEquipmentItem.Animator) m_AttachedEquipmentItem.Animator.SetTrigger(_hashCode);
            if (m_FPArmsHandler.Animator) m_FPArmsHandler.Animator.SetTrigger(_hashCode);
        }

        public virtual void Animator_SetBool(string _string, bool _bool)
        {
            if (m_AttachedEquipmentItem.Animator) m_AttachedEquipmentItem.Animator.SetBool(_string, _bool);
            if (m_FPArmsHandler.Animator) m_FPArmsHandler.Animator.SetBool(_string, _bool);
        }

        public virtual void Animator_SetBool(int _hashCode, bool _bool)
        {
            if (m_AttachedEquipmentItem.Animator) m_AttachedEquipmentItem.Animator.SetBool(_hashCode, _bool);
            if (m_FPArmsHandler.Animator) m_FPArmsHandler.Animator.SetBool(_hashCode, _bool);
        }

        public virtual void Animator_SetInteger(string _string, int _int)
        {
            if (m_AttachedEquipmentItem.Animator) m_AttachedEquipmentItem.Animator.SetInteger(_string, _int);
            if (m_FPArmsHandler.Animator) m_FPArmsHandler.Animator.SetInteger(_string, _int);
        }

        public virtual void Animator_SetInteger(int _hashCode, int _int)
        {
            if (m_AttachedEquipmentItem.Animator) m_AttachedEquipmentItem.Animator.SetInteger(_hashCode, _int);
            if (m_FPArmsHandler.Animator) m_FPArmsHandler.Animator.SetInteger(_hashCode, _int);
        }

        public virtual void Animator_SetFloat(string _string, float _float)
        {
            if (m_AttachedEquipmentItem.Animator) m_AttachedEquipmentItem.Animator.SetFloat(_string, _float);
            if (m_FPArmsHandler.Animator) m_FPArmsHandler.Animator.SetFloat(_string, _float);
        }

        public virtual void Animator_SetFloat(int _hashCode, float _float)
        {
            if (m_AttachedEquipmentItem.Animator) m_AttachedEquipmentItem.Animator.SetFloat(_hashCode, _float);
            if (m_FPArmsHandler.Animator) m_FPArmsHandler.Animator.SetFloat(_hashCode, _float);
        }
        #endregion
    }
}
