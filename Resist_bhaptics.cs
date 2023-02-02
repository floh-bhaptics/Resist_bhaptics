using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;
using Il2Cpp;

[assembly: MelonInfo(typeof(Resist_bhaptics.Resist_bhaptics), "Resist_bhaptics", "1.0.0", "Florian Fahrenberger")]
[assembly: MelonGame("The Binary Mill", "Resist")]

namespace Resist_bhaptics
{
    public class Resist_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr = null!;

        public override void OnInitializeMelon()
        {
            tactsuitVr = new TactsuitVR();
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }
        
        [HarmonyPatch(typeof(Gun), "Shoot", new Type[] { })]
        public class bhaptics_GunShoot
        {
            [HarmonyPostfix]
            public static void Postfix(Gun __instance)
            {
                bool isRightHand = (!__instance.isLeft);
                tactsuitVr.Recoil(isRightHand);
            }
        }

        [HarmonyPatch(typeof(GrappleGun), "Swing", new Type[] { })]
        public class bhaptics_Swing
        {
            [HarmonyPostfix]
            public static void Postfix(GrappleGun __instance)
            {
                bool isRightHand = (!__instance.isLeft);
                if (!__instance.Hooked)
                {
                    tactsuitVr.StopSwinging(isRightHand);
                    return;
                }
                tactsuitVr.StartSwinging(isRightHand);
            }
        }

        [HarmonyPatch(typeof(GrappleGun), "CancelHook", new Type[] { })]
        public class bhaptics_CancelHook
        {
            [HarmonyPostfix]
            public static void Postfix(GrappleGun __instance)
            {
                bool isRightHand = (!__instance.isLeft);
                tactsuitVr.StopSwinging(isRightHand);
            }
        }

        [HarmonyPatch(typeof(GrappleGun), "Hook", new Type[] { })]
        public class bhaptics_Hook
        {
            [HarmonyPostfix]
            public static void Postfix(GrappleGun __instance)
            {
                bool isRightHand = (!__instance.isLeft);
                tactsuitVr.Recoil(isRightHand, 0.5f);
            }
        }

        [HarmonyPatch(typeof(GrappleGun), "Release", new Type[] { })]
        public class bhaptics_HookRelease
        {
            [HarmonyPostfix]
            public static void Postfix(GrappleGun __instance)
            {
                bool isRightHand = (!__instance.isLeft);
                tactsuitVr.StopSwinging(isRightHand);
            }
        }

        private static KeyValuePair<float, float> getAngleAndShift(Vector3 hit)
        {
            // bhaptics pattern starts in the front, then rotates to the left. 0° is front, 90° is left, 270° is right.
            // y is "up", z is "forward" in local coordinates
            Vector3 patternOrigin = new Vector3(0f, 0f, 1f);
            Vector3 flattenedHit = new Vector3(hit.x, 0f, hit.z);

            // get angle. .Net < 4.0 does not have a "SignedAngle" function...
            float hitAngle = Vector3.Angle(flattenedHit, patternOrigin);
            // check if cross product points up or down, to make signed angle myself
            Vector3 crossProduct = Vector3.Cross(flattenedHit, patternOrigin);
            if (crossProduct.y < 0f) { hitAngle *= -1f; }
            // relative to player direction
            float myRotation = hitAngle;
            // switch directions (bhaptics angles are in mathematically negative direction)
            myRotation *= -1f;
            // convert signed angle into [0, 360] rotation
            if (myRotation < 0f) { myRotation = 360f + myRotation; }


            // up/down shift is in y-direction
            // in Battle Sister, the torso Transform has y=0 at the neck,
            // and the torso ends at roughly -0.5 (that's in meters)
            // so cap the shift to [-0.5, 0]...
            float hitShift = hit.y;
            //tactsuitVr.LOG("HitShift: " + hitShift.ToString());
            float upperBound = 0.5f;
            float lowerBound = -0.5f;
            if (hitShift > upperBound) { hitShift = 0.5f; }
            else if (hitShift < lowerBound) { hitShift = -0.5f; }
            // ...and then spread/shift it to [-0.5, 0.5], which is how bhaptics expects it
            else { hitShift = (hitShift - lowerBound) / (upperBound - lowerBound) - 0.5f; }
            hitShift = 1.0f;
            // No tuple returns available in .NET < 4.0, so this is the easiest quickfix
            return new KeyValuePair<float, float>(myRotation, hitShift);
        }

        [HarmonyPatch(typeof(PlayerHealth), "Update", new Type[] {  })]
        public class bhaptics_UpdateHealth
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerHealth __instance)
            {
                //if (__instance.Health <= 0.25f * __instance.MaxHealth) tactsuitVr.StartHeartBeat();
                //tactsuitVr.LOG("Health: " + __instance.Health.ToString());
                if (__instance.Health <= 0.25f) tactsuitVr.StartHeartBeat();
                else tactsuitVr.StopHeartBeat();
            }
        }

        [HarmonyPatch(typeof(PlayerHealth), "ApplyDamage", new Type[] { typeof(float) })]
        public class bhaptics_PlayerDamage
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerHealth __instance)
            {
                tactsuitVr.PlaybackHaptics("BulletHit");
            }
        }

        [HarmonyPatch(typeof(PlayerDeath), "HandleDeath", new Type[] { })]
        public class bhaptics_PlayerDeath
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.StopThreads();
            }
        }

        [HarmonyPatch(typeof(PlayerDeath), "HandleDeath", new Type[] { typeof(bool), typeof(bool) })]
        public class bhaptics_PlayerDeathParameters
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.StopThreads();
            }
        }

        /*
        [HarmonyPatch(typeof(PlayerCollision), "OnMovementHit")]
        public class bhaptics_PlayerCollision
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("Impact");
            }
        }
        
        [HarmonyPatch(typeof(PlayerCollision), "OnGroundHit")]
        public class bhaptics_PlayerGroundHit
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("Land");
            }
        }
        */
        [HarmonyPatch(typeof(PlayerAudio), "Jump", new Type[] { })]
        public class bhaptics_PlayerJump
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("Jump");
            }
        }

        [HarmonyPatch(typeof(PlayerAudio), "Land", new Type[] { })]
        public class bhaptics_PlayerLand
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("Land");
            }
        }

        [HarmonyPatch(typeof(PlayerAudio), "PlayJetpackIdle", new Type[] { typeof(bool) })]
        public class bhaptics_PlayerAirBoost
        {
            [HarmonyPostfix]
            public static void Postfix(bool on)
            {
                if (!on) return;
                tactsuitVr.PlaybackHaptics("Jetpack");
            }
        }
        
    }
}
