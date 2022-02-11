using UnityEngine;

namespace HQFPSTemplate.Equipment
{
    [CreateAssetMenu(fileName = "Gun Info", menuName = "HQ FPS Template/Equipment/Gun")]
    public class GunInfo : ProjectileWeaponInfo
    {
        [Group("7: ")] public GunSettings.Shooting Projectile;
    }
}
