using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;

namespace Resist_bhaptics
{
    public class Resist_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr;

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
                tactsuitVr.StopThreads();
            }
        }

        [HarmonyPatch(typeof(GrappleGun), "Swing", new Type[] { })]
        public class bhaptics_Swing
        {
            [HarmonyPostfix]
            public static void Postfix(GrappleGun __instance)
            {
                bool isRightHand = (!__instance.isLeft);
                tactsuitVr.StopThreads();
            }
        }

        [HarmonyPatch(typeof(GrappleGun), "CancelHook", new Type[] { })]
        public class bhaptics_CancelHook
        {
            [HarmonyPostfix]
            public static void Postfix(GrappleGun __instance)
            {
                bool isRightHand = (!__instance.isLeft);
                tactsuitVr.StopThreads();
            }
        }

        [HarmonyPatch(typeof(GrappleGun), "Cast", new Type[] { })]
        public class bhaptics_CastHook
        {
            [HarmonyPostfix]
            public static void Postfix(GrappleGun __instance)
            {
                bool isRightHand = (!__instance.isLeft);
                tactsuitVr.StopThreads();
            }
        }

        [HarmonyPatch(typeof(GrappleGun), "Hook", new Type[] { })]
        public class bhaptics_Hook
        {
            [HarmonyPostfix]
            public static void Postfix(GrappleGun __instance)
            {
                bool isRightHand = (!__instance.isLeft);
                tactsuitVr.StopThreads();
            }
        }

        [HarmonyPatch(typeof(GrappleGun), "Release", new Type[] { })]
        public class bhaptics_HookRelease
        {
            [HarmonyPostfix]
            public static void Postfix(GrappleGun __instance)
            {
                bool isRightHand = (!__instance.isLeft);
                tactsuitVr.StopThreads();
            }
        }
        /*
        [HarmonyPatch(typeof(GrappleGun), "UpdateVelocity", new Type[] { })]
        public class bhaptics_UpdateVelocity
        {
            [HarmonyPostfix]
            public static void Postfix(GrappleGun __instance)
            {
                bool isRightHand = (!__instance.isLeft);
                tactsuitVr.StopThreads();
            }
        }
        */
    }
}
