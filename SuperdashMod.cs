using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modding;
using UnityEngine;
using ModCommon;
using HutongGames.PlayMaker;

namespace Superdash
{
    public class SuperdashMod : Mod
    {
        public SuperdashMod() : base(null)
        {

        }
        public override void Initialize()
        {
            ModHooks.HeroUpdateHook += ModHooks_HeroUpdateHook;
            On.HeroController.TakeDamage += HeroController_TakeDamage;
        }

        private void HeroController_TakeDamage(On.HeroController.orig_TakeDamage orig,
            HeroController self, GameObject go, GlobalEnums.CollisionSide damageSide, int damageAmount, int hazardType)
        {
            if (!self.cState.superDashing) orig(self, go, damageSide, damageAmount, hazardType);
        }

        private void ModHooks_HeroUpdateHook()
        {
            HeroController.instance.gameObject.GetOrAddComponent<Script>();
        }
    }
}
