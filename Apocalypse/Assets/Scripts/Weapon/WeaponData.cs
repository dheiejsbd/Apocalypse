using System;
using System.Collections.Generic;
using UnityEngine;

namespace Apocalypse.Weapon
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObject/Weapon", order = int.MaxValue)]
    public class WeaponData : ScriptableObject
    {
        #region Stat
        public string Name;
        public AmmoType ammoType;
        public int MaxAmmo;
        public float RPM;
        public bool Cansafety = true;
        public bool CanSingle = true;
        public bool Canburst = true;
        public bool CanAuto = true;
        #endregion
        #region Anim
        [Header("Anim")]
        public AnimatorOverrideController HandAnim;
        public AnimatorOverrideController GunAnim;
        #endregion
        [Header("Recoil")]
        #region Recoil
        public float RecoilX;
        public float RecoilY;
        
        public float AimRecoilX;
        public float AimRecoilY;
        #endregion

        [Header("ShotGruping")]
        #region ShotGrouping
        public float ShotGroupingX;
        public float ShotGroupingY;
        #endregion

        [Header("MoveSpeed")]
        #region MoveSpeed
        public float WalkSpeed;
        public float RunSpeed;
        public float CrouchSpeed;
        #endregion

        [Header("Sound")]
        #region Sound

        AudioClip[] Shoot;

        #endregion
    }
    public enum FireMode
    {
        safety,
        Single,
        burst,
        Auto,
    }
    public enum AmmoType
    {
        ammo5,
        ammo7,
        ammo9,
        ammo12,
    }
    public struct WeaponSound
    {
        float delay;
        AudioClip[] audioClip;
        float Volume;
        float pitch;

    }
}