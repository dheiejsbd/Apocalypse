using UnityEngine;
using System.Collections;

namespace HQFPSTemplate.Equipment
{
    /// <summary>
    /// A component which can be attached to a weapon (E.g. Gun / LauncherWeapon)
    /// It handles all of the recoil for the attached weapon from camera to weapon model recoil.
    /// </summary>
    [RequireComponent(typeof(ProjectileWeapon))]
    public partial class ProjectileWeaponRecoil : PlayerComponent, IEquipmentComponent
    {
        [SerializeField]
        private ProjectileWeaponRecoilInfo m_RecoilInfo = null;

        private ProjectileWeapon m_Weapon;

        private bool m_AdditiveRecoilActive;
        private bool m_RecoilControlActive;

        private float m_RecoilStartTime;

        private Vector2 m_RecoilToAdd;
        private Vector2 m_RecoilToAddStart;
        private float m_RecoilFrameRemove;
        private Coroutine m_RecoilControlCoroutine;


        public void Initialize (EquipmentItem equipmentItem) => m_Weapon = equipmentItem as ProjectileWeapon;

        public void OnSelected()
        {
            m_Weapon.EHandler.EPhysicsHandler.AdjustRecoilSprings(m_RecoilInfo.ViewModelRecoil.SpringData);
            Player.Camera.Physics.AdjustRecoilSprings(m_RecoilInfo.CameraRecoil.SpringData);

            m_Weapon.EHandler.UsingItem.AddStartListener(StartRecoil);
            m_Weapon.EHandler.UsingItem.AddStopListener(StopRecoil);
            m_Weapon.FireHitPoints.AddListener(AddImpulseRecoil);
            m_Weapon.DryFire.AddListener(AddDryFireForce);
            Player.ChangeUseMode.AddListener(ChangeFireModeForce);
        }

        private void OnDisable()
        {
            m_Weapon.EHandler.UsingItem.RemoveStartListener(StartRecoil);
            m_Weapon.EHandler.UsingItem.RemoveStopListener(StopRecoil);
            m_Weapon.FireHitPoints.RemoveListener(AddImpulseRecoil);
            m_Weapon.DryFire.RemoveListener(AddDryFireForce);
            Player.ChangeUseMode.RemoveListener(ChangeFireModeForce);
        }

        //Non-Controllable/Impulse recoil
        private void AddImpulseRecoil (Vector3[] impactPoints) 
        {
            //Apply a random recoil force to the visual model
            float recoilMultiplier = m_RecoilInfo.ViewModelRecoil.RecoilOverTime.Evaluate(m_Weapon.EHandler.ContinuouslyUsedTimes / (float)m_Weapon.MagazineSize);

            for (int i = 0; i < impactPoints.Length; i++)
            {
                ApplyModelRecoilForce(Player.Aim.Active ? m_RecoilInfo.ViewModelRecoil.AimShootForce : m_RecoilInfo.ViewModelRecoil.ShootForce, recoilMultiplier);

                //Apply a recoil force to the camera
                ApplyCamRecoilForce(m_RecoilInfo.CameraRecoil.ShootForce, Player.Aim.Active ? m_RecoilInfo.CameraRecoil.AimMultiplier : 1f);
            }

            //Apply a shake force to the camera
            if (m_RecoilInfo.CameraRecoil.ShootShake.PositionAmplitude != Vector3.zero || m_RecoilInfo.CameraRecoil.ShootShake.RotationAmplitude != Vector3.zero)
                Player.Camera.Physics.DoShake(m_RecoilInfo.CameraRecoil.ShootShake, 1f);
        }

        private void AddDryFireForce() 
        {
            ApplyModelRecoilForce(m_RecoilInfo.ViewModelRecoil.DryFireForce);
        }

        //Controllable/Additive recoil
        private void StartRecoil() 
        {
            m_RecoilStartTime = Time.time;
            m_AdditiveRecoilActive = true;
            m_RecoilControlActive = false;

            m_RecoilToAdd = Vector2.zero;

            if (m_RecoilControlCoroutine != null)
                StopCoroutine(m_RecoilControlCoroutine);
        }
        
        private void Update() 
        {
            if (m_RecoilInfo.CameraRecoil.RecoilPattern.Length == 0)
                return;

            if (m_AdditiveRecoilActive)
            {
                //Additive Recoil
                int recoilControlIndex = Mathf.Clamp(m_Weapon.EHandler.ContinuouslyUsedTimes - 1, 0, m_RecoilInfo.CameraRecoil.RecoilPattern.Length - 1);

                Vector2 recoilThisFrame = new Vector2(
                    m_RecoilInfo.CameraRecoil.RecoilPattern[recoilControlIndex].x * m_RecoilInfo.CameraRecoil.RecoilPatternMultiplier * Time.deltaTime,
                    m_RecoilInfo.CameraRecoil.RecoilPattern[recoilControlIndex].y * m_RecoilInfo.CameraRecoil.RecoilPatternMultiplier * Time.deltaTime);

                if (Player.Aim.Active)
                    recoilThisFrame *= m_RecoilInfo.CameraRecoil.AimMultiplier;

                Player.Camera.MoveCamera(recoilThisFrame.x, recoilThisFrame.y);

                Vector2 lastMovement = -Player.Camera.LastMovement;

                m_RecoilToAdd -= recoilThisFrame;

                if (m_RecoilToAdd.x != 0f && Mathf.Sign(lastMovement.x) != Mathf.Sign(m_RecoilToAdd.x))
                    m_RecoilToAdd.x = Mathf.Max(m_RecoilToAdd.x + lastMovement.x, 0f);

                if (m_RecoilToAdd.y != 0f && Mathf.Sign(lastMovement.y) != Mathf.Sign(m_RecoilToAdd.y))
                    m_RecoilToAdd.y = Mathf.Max(m_RecoilToAdd.y + lastMovement.y, 0f);
            }
            else if (m_RecoilInfo.CameraRecoil.HasRecoilControl && m_RecoilControlActive)
            {
                //Recoil Control
                Vector2 lastMovement = Player.Camera.LastMovement;

                if (m_RecoilToAdd.x != 0f && Mathf.Sign(lastMovement.x) != Mathf.Sign(m_RecoilToAdd.x))
                    m_RecoilToAdd.x = Mathf.Max(m_RecoilToAdd.x + lastMovement.x, 0f);

                if (m_RecoilToAdd.y != 0f && Mathf.Sign(lastMovement.y) != Mathf.Sign(m_RecoilToAdd.y))
                    m_RecoilToAdd.y = Mathf.Max(m_RecoilToAdd.y + lastMovement.y, 0f);            

                Vector2 prevRecoil = m_RecoilToAdd;

                float recoilControlMod = Mathf.Clamp01(1 - (-m_RecoilToAdd.x / -m_RecoilToAddStart.x));        
                float recoilControlCurveMod = m_RecoilInfo.CameraRecoil.RecoilControlCurve.Evaluate(float.IsNaN(recoilControlMod) ? 0f : recoilControlMod);       

                RemoveRecoil(ref m_RecoilToAdd, Time.deltaTime * m_RecoilFrameRemove * recoilControlCurveMod);

                Vector2 recoilThisFrame = m_RecoilToAdd - prevRecoil;

                Player.Camera.LookAngles -= recoilThisFrame;

                //Disable the recoil control when the camera rotation gets reseted
                if (m_RecoilToAdd.sqrMagnitude < 0.15f)
                    m_RecoilControlActive = false;
            }
        }

        private void StopRecoil() 
        {
            m_AdditiveRecoilActive = false;

            //Start the recoil control after a certain amount of time

            if (m_RecoilControlCoroutine != null)
                StopCoroutine(m_RecoilControlCoroutine);

            m_RecoilControlCoroutine = StartCoroutine(C_StartRecoilControl());
        }

        private void RemoveRecoil(ref Vector2 recoil, float amount)
        {
            float signX = Mathf.Sign(recoil.x);
            float signY = Mathf.Sign(recoil.y);

            recoil.x -= recoil.x * amount;
            recoil.y -= recoil.y * amount;

            if (Mathf.Sign(recoil.x) != signX)
                recoil.x = 0f;

            if (Mathf.Sign(recoil.y) != signY)
                recoil.y = 0f;
        }

        private void ChangeFireModeForce() 
        {
            ApplyModelRecoilForce(m_RecoilInfo.ViewModelRecoil.ChangeFireModeForce);
        }

        private void ApplyCamRecoilForce(RecoilForce force, float forceMultiplier = 1f)
        {
            Player.Camera.Physics.AddPositionForce(force.PositionForce * forceMultiplier, force.Distribution);
            Player.Camera.Physics.AddRotationForce(force.RotationForce * forceMultiplier, force.Distribution);
        }

        private void ApplyModelRecoilForce(RecoilForce force, float forceMultiplier = 1f)
        {
            m_Weapon.EHandler.EPhysicsHandler.ApplyPositionRecoil(force.PositionForce * forceMultiplier);
            m_Weapon.EHandler.EPhysicsHandler.ApplyRotationRecoil(force.RotationForce * forceMultiplier);
        }

        private IEnumerator C_StartRecoilControl()
        {
            yield return new WaitForSeconds(m_RecoilInfo.CameraRecoil.RecoilControlDelay);

            if (m_Weapon.EHandler.UsingItem.Active)
                yield break;

            m_RecoilFrameRemove = Mathf.Clamp(m_RecoilToAdd.x * Mathf.Max(1f, (1 - (1 / (m_RecoilInfo.CameraRecoil.RecoilControlSpeedMod * (Time.time - m_RecoilStartTime))))), m_RecoilInfo.CameraRecoil.RecoilControlSpeedRange.x, m_RecoilInfo.CameraRecoil.RecoilControlSpeedRange.y);
            m_RecoilToAddStart = m_RecoilToAdd;
            m_RecoilControlActive = true;
        }
    }
}
