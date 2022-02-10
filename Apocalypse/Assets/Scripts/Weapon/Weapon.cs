using System;
using System.Collections.Generic;
using UnityEngine;

namespace Apocalypse.Weapon
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObject/Weapon", order = int.MaxValue)]
    public class Weapon : ScriptableObject
    {
        #region Anim
        #region HandAnim
        public Animation HandEquip;
        public Animation HandFire;
        public Animation HandAimFire;
        public Animation HandHold;
        public Animation HandIdle;
        public Animation HandReload;
        public Animation HandReloadEmpty;
        public Animation HandUnequip;
        #endregion

        #region WeaponAnim
        public Animation WeaponEquip;
        public Animation WeaponAimFire;
        public Animation WeaponFire;
        public Animation WeaponHold;
        public Animation WeaponIdle;
        public Animation WeaponReload;
        public Animation WeaponReloadEmpty;
        public Animation WeaponUnequip;
        #endregion
        #endregion

        #region Recoil
        public float RecoilX;
        public float RecoilY;
        
        public float AimRecoilX;
        public float AimRecoilY;
        #endregion

        #region ShotGrouping
        public float ShotGroupingX;
        public float ShotGroupingY;
        #endregion

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

        #region MoveSpeed
        public float WalkSpeed;
        public float RunSpeed;
        public float CrouchSpeed;
        #endregion

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