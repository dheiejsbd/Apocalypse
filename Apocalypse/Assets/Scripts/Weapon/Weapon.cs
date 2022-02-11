using System;
using System.Collections.Generic;
using UnityEngine;

namespace Apocalypse.Weapon
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObject/Weapon", order = int.MaxValue)]
    public class Weapon : ScriptableObject
    {
        #region Stat
        public string Name;
        public AmmoType ammoType;
        public int MaxAmmo;
        public float RPM;
        bool safety;
        bool Single;
        bool burst;
        bool Auto;
        #endregion
        #region Anim
        [Header("HandAnim")]
        #region HandAnim
        public AnimationClip HandEquip;
        public AnimationClip HandFire;
        public AnimationClip HandAimFire;
        public AnimationClip HandHold;
        public AnimationClip HandIdle;
        public AnimationClip HandReload;
        public AnimationClip HandReloadEmpty;
        public AnimationClip HandUnequip;
        #endregion
        [Header("WeaponAnim")]
        #region WeaponAnim
        public AnimationClip WeaponEquip;
        public AnimationClip WeaponAimFire;
        public AnimationClip WeaponFire;
        public AnimationClip WeaponHold;
        public AnimationClip WeaponIdle;
        public AnimationClip WeaponReload;
        public AnimationClip WeaponReloadEmpty;
        public AnimationClip WeaponUnequip;
        #endregion
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