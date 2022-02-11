using UnityEditor;
using UnityEngine;

namespace Apocalypse.Weapon
{
    public class WeaponController : MonoBehaviour
    {
        bool IsAttack;
        bool CanControl;
        int FireModeID;
        int FireModeCount;
        int ammo;
        float NextFireTime;

        [SerializeField] WeaponData data;
        [SerializeField] Animator GunAnimator;
        [SerializeField] Animator HandAnimator;

        public void Start()
        {
            Initialize();
        }
        void Initialize()
        {
            IsAttack = false;
            CanControl = true;
            FireModeID = (int)FireMode.safety;
            FireModeCounter();
            ammo = 0;
            NextFireTime = 0;
        }

        #region Input
        void FireModeCounter()
        {
            FireModeCount += data.CanAuto ? 1 : 0;
            FireModeCount += data.Canburst ? 1 : 0;
            FireModeCount += data.Cansafety ? 1 : 0;
            FireModeCount += data.CanSingle ? 1 : 0;
        }

        public void Update()
        {
            NextFireTime -= Time.deltaTime;
        }
        public void TryReload()
        {
            // 탄 소지 여부
            if (data.MaxAmmo == ammo) return;
            if (!CanControl) return;

            Reload();
        }
        void Reload()
        {
            CanControl = false;
            SetAnimTrigger("Reload");
        }
        
        public void TryChangeFireMode()
        {
            if (!CanControl) return;
            if (FireModeCount < 2) return;

            ChangeFireMode();
        }
        void ChangeFireMode()
        {
            bool change = true;
            while (change)
            {
                switch (FireModeID)
                {
                    case (int)FireMode.safety:
                        if (data.CanSingle)
                        {
                            FireModeID = (int)FireMode.Single;
                            change = false;
                        }
                        else
                        {
                            FireModeID++;
                        }
                        break;

                    case (int)FireMode.Single:
                        if (data.Canburst)
                        {
                            FireModeID = (int)FireMode.burst;
                            change = false;
                        }
                        else
                        {
                            FireModeID++;
                        }
                        break;

                    case (int)FireMode.burst:
                        if (data.CanAuto)
                        {
                            FireModeID = (int)FireMode.Auto;
                            change = false;
                        }
                        else
                        {
                            FireModeID++;
                        }
                        break;

                    case (int)FireMode.Auto:
                        if (data.Cansafety)
                        {
                            FireModeID = (int)FireMode.safety;
                            change = false;
                        }
                        else
                        {
                            FireModeID = 0;
                        }
                        break;
                }
            }
        }

        public void TryStartAttack()
        {
            if (!CanControl) return;
            if (ammo < 1) return;


        }

        public void TryEndAttack()
        {

        }

        public void TryAttack()
        {
            if (NextFireTime > 0) return;
            if (!IsAttack) return;
            if (ammo < 1) return;
        }
        #endregion

        #region AnimKeyFrameEvent

        public void EnterMagazine()
        {
            ammo = data.MaxAmmo;
        }

        public void EndReload()
        {
            CanControl = true;
        }

        #endregion


        void SetAnimBool(string name, bool value)
        {
            HandAnimator.SetBool(name, value);
            GunAnimator?.SetBool(name, value);
        }
        void SetAnimFlot(string name, float value)
        {

            HandAnimator.SetFloat(name, value);
            GunAnimator?.SetFloat(name, value);
        }
        void SetAnimTrigger(string name)
        {

            HandAnimator.SetTrigger(name);
            //GunAnimator?.SetTrigger(name);
        }
        void SetAnimInt(string name, int value)
        {
            HandAnimator.SetInteger(name, value);
            GunAnimator?.SetInteger(name, value);
        }
    }
}