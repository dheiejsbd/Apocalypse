using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace FrameWork.Math
{
    public class FOV
    {

        public static Vector3 GetTargetVector(Vector3 origin, Vector3 direction, LayerMask TargetlayerMask, float viewRange, float viewAngle, float Offset)
        {
            origin += Vector3.up * Offset;
            Collider[] Targets = Physics.OverlapSphere(origin, viewRange, TargetlayerMask);

            Vector3 Target = Vector3.zero;
            float Dist = float.MaxValue;

            for (int i = 0; i < Targets.Length; i++)
            {
                Vector3 Targetdir = (Targets[i].transform.position - origin).normalized;

                if (Vector3.Angle(direction, Targetdir) < viewAngle * 0.5f)
                {
                    if (Vector3.Distance(Targets[i].transform.position, origin) < Dist)
                    {
                        Dist = Vector3.Distance(Targets[i].transform.position, origin);
                        Target = Targets[i].transform.position;
                    }
                }
            }
            return Target;
        }
        public static Vector3 GetTargetVector(Vector3 origin, Vector3 direction, LayerMask TargetlayerMask, float viewRange, float viewAngle, float Offset, LayerMask RaycastMask)
        {
            origin += Vector3.up* Offset;
            Collider[] Targets = Physics.OverlapSphere(origin, viewRange, TargetlayerMask);

            Vector3 Target = Vector3.zero;
            float Dist = float.MaxValue;

            for (int i = 0; i < Targets.Length; i++)
            {
                Vector3 Targetdir = (Targets[i].transform.position - origin).normalized;

                if (Vector3.Angle(direction, Targetdir) < viewAngle * 0.5f)
                {
                    if (Vector3.Distance(Targets[i].transform.position, origin) < Dist)
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(origin, Targetdir, out hit, Dist, RaycastMask))
                        {
                            if ((hit.transform.gameObject.layer & RaycastMask) == hit.transform.gameObject.layer)
                                Dist = Vector3.Distance(Targets[i].transform.position, origin);
                            Target = Targets[i].transform.position;
                        }
                    }
                }
            }
            return Target;
        }
        public static Transform GetTarget(Vector3 origin, Vector3 direction, LayerMask TargetlayerMask, float viewRange, float viewAngle, float Offset, LayerMask RaycastMask)
        {
            origin += Vector3.up * Offset;
            Collider[] Targets = Physics.OverlapSphere(origin, viewRange, TargetlayerMask);

            Transform Target = null;
            float Dist = float.MaxValue;

            for (int i = 0; i < Targets.Length; i++)
            {
                Vector3 Targetdir = (Targets[i].transform.position - origin).normalized;

                if (Vector3.Angle(direction, Targetdir) < viewAngle * 0.5f)
                {
                    if (Vector3.Distance(Targets[i].transform.position, origin) < Dist)
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(origin, Targetdir, out hit, Dist, RaycastMask))
                        {
                            if ((hit.transform.gameObject.layer & RaycastMask) == hit.transform.gameObject.layer)
                                Dist = Vector3.Distance(Targets[i].transform.position, origin);
                            Target = Targets[i].transform;
                        }
                    }
                }
            }
            return Target;
        }

    }
}
