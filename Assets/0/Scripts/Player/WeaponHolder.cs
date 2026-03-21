using System.Collections.Generic;
using Bellepron.Weapon;
using UnityEngine;

namespace Bellepron
{
    public class WeaponHolder : MonoBehaviour
    {
        public Transform rightHand;
        public Transform leftHand;
        public Transform leftHandCastTransform;

        public List<Transform> GetHoldPoints(WeaponHoldType holdType, WeaponStance stance)
        {
            List<Transform> points = new List<Transform>();

            switch (stance)
            {
                case WeaponStance.Single:
                    points.Add(GetSingleHoldPoint(holdType));
                    break;

                case WeaponStance.DualWield:
                    points.Add(rightHand);
                    points.Add(leftHand);
                    break;
            }

            return points;
        }

        private Transform GetSingleHoldPoint(WeaponHoldType holdType)
        {
            switch (holdType)
            {
                case WeaponHoldType.RightHand:
                    return rightHand;

                case WeaponHoldType.LeftHand:
                    return leftHand;

                case WeaponHoldType.TwoHanded:
                    return rightHand;

                default:
                    return rightHand;
            }
        }
    }
}