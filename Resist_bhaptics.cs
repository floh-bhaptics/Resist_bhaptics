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
    }
}
