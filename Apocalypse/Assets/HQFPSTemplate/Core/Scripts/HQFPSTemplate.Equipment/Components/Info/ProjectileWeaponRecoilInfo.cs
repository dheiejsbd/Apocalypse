using UnityEngine;
using System;

namespace HQFPSTemplate.Equipment
{
    [CreateAssetMenu(fileName = "Weapon Recoil Info", menuName = "HQ FPS Template/Equipment Component/Weapon Recoil")]
    public class ProjectileWeaponRecoilInfo : ScriptableObject
    {
        #region Internal
        [Serializable]
        public class WeaponRecoilModule : CloneableObject<WeaponRecoilModule>
        {
            public AnimationCurve RecoilOverTime = null;

            [Space]

            [Group] public SpringSettings SpringData = new SpringSettings();

            [Space]

            [Group] public RecoilForce ShootForce = new RecoilForce();
            [Group] public RecoilForce AimShootForce = new RecoilForce();
            [Group] public RecoilForce DryFireForce = new RecoilForce();
            [Group] public RecoilForce ChangeFireModeForce = new RecoilForce();
        }

        [Serializable]
        public class CameraRecoilModule : CloneableObject<CameraRecoilModule>
        {
            [SerializeField]
            [Range(0f, 2f)]
            public float AimMultiplier = 0.75f;

            [BHeader("Controllable recoil", order = 100)]

            public float RecoilPatternMultiplier = 1f;

            [Tooltip("Controllable - vertical(x) & horizontal(y) recoil, add as many values as the mag size of the attached weapon")]
            public Vector2[] RecoilPattern;

            [Space(3f)]

            public bool HasRecoilControl = true;

            [EnableIf("HasRecoilControl", true)]
            public AnimationCurve RecoilControlCurve = null;

            [EnableIf("HasRecoilControl", true)]
            public float RecoilControlDelay = 1f;

            [EnableIf("HasRecoilControl", true)]
            public Vector2 RecoilControlSpeedRange = new Vector2(2f, 7f);

            [EnableIf("HasRecoilControl", true)]
            public float RecoilControlSpeedMod = 2f;

            [Space(3f)]

            [BHeader("Non-Controllable recoil", order = 100)]

            [Group] public SpringSettings SpringData = new SpringSettings();

            [Tooltip("Non-Controlable - Use/Shoot cam force")]
            [Group] public RecoilForce ShootForce = new RecoilForce();

            [Tooltip("Non-Controlable - Use/Shoot cam shake")]
            [Group] public CameraShakeSettings ShootShake = null;
        }
        #endregion

        [Group] public WeaponRecoilModule ViewModelRecoil = null;
        [Group] public CameraRecoilModule CameraRecoil = null;
    }
}
