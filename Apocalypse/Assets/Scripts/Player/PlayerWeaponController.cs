using System;
using System.Collections.Generic;
using UnityEngine;
using Apocalypse.Weapon;

namespace Apocalypse.Player
{
    public class PlayerWeaponController : PlayerComponent 
    {
        WeaponController ActiveWeapon;
        WeaponController[] Weapons;
        private void Start()
        {
            Weapons = GetComponentsInChildren<WeaponController>();
            ActiveWeapon = Weapons[0];
            AddListener();
        }

        void AddListener()
        {
            Player.Reload.AddListener(Reload);
            Player.ChangeWeapon.AddListener(ChangeWeapon);
            Player.FireMode.AddListener(ChangeFireMode);
            Player.StartAttack.AddListener(StartAttack);
            Player.EndAttack.AddListener(EndAttack);
            Player.Attack.AddListener(Attack);

            Player.EnterMagazine.AddListener(EnterMagazine);
            Player.EndReload.AddListener(EndReload);
        }

        #region InputListener
        void ChangeWeapon(int ID)
        {

        }
        void Reload()
        {
            ActiveWeapon?.TryReload();
        }
        void ChangeFireMode()
        {
            ActiveWeapon?.TryChangeFireMode();
        }
        void StartAttack()
        {
            ActiveWeapon?.TryStartAttack();
        }
        void EndAttack()
        {
            ActiveWeapon?.TryEndAttack();
        }
        void Attack()
        {
            ActiveWeapon?.TryAttack();
        }
        #endregion

        #region KeyFrameListener
        void EnterMagazine()
        {
            ActiveWeapon.EnterMagazine();
        }
        void EndReload()
        {
            ActiveWeapon.EndReload();
        }
        #endregion
    }
}
