using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ModCommon;
using ModCommon.Util;
using Modding;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace Superdash
{
    class Script : MonoBehaviour
    {
        class SDCheck : FsmStateAction
        {
            public FsmBool eq = null;
            public override void OnEnter()
            {
                OnUpdate();
            }
            public override void OnUpdate()
            {
                Rigidbody2D rb = HeroController.instance.GetComponent<Rigidbody2D>();
                if (rb.velocity.x < 0.1 && rb.velocity.x > 0.1 && rb.velocity.y < 0.1 & rb.velocity.y > 0.1)
                {
                    if (eq != null) eq.Value = true;
                }
                else
                {
                    if (eq != null) eq.Value = false;
                }
            }
        }
        class SD : FsmStateAction
        {
            public override void OnEnter()
            {
                HeroController hero = HeroController.instance;
                hero.cState.transitioning = false;
                hero.transitionState = GlobalEnums.HeroTransitionState.WAITING_TO_TRANSITION;
                hero.cState.invulnerable = false;
                PlayerData.instance.disablePause = false;
                FindObjectOfType<Script>().FacingUpdate();
                Finish();
            }
        }
        
        public FsmFloat UpSpeed = new FsmFloat();
        bool sdl = false;
        float sdspeed = 30;
        void Start()
        {
            FsmBool eq = HeroController.instance.superDash.FsmVariables.FindFsmBool("Zero Last Frame");
            gameObject.GetFSMActionOnState<SetVelocity2d>("Dashing", "Superdash").y = UpSpeed;
            gameObject.GetFSMActionOnState<SetVelocity2d>("Cancelable", "Superdash").y = UpSpeed;
            gameObject.GetFSMActionOnState<FloatTestToBool>("Dashing", "Superdash").equalBool = null;
            gameObject.GetFSMActionOnState<FloatTestToBool>("Cancelable", "Superdash").equalBool = null;
            HeroController.instance.superDash.InsertAction("Dashing", new SDCheck() { eq = eq } , 6);
            HeroController.instance.superDash.InsertAction("Cancelable", new SDCheck() { eq = eq }, 6);
            HeroController.instance.superDash.AddAction("Enter Velocity", new SD());
            UpSpeed.Value = 0;
        }
        public enum Facing
        {
            Up,Down,Left,Right
        }
        public Facing facing = Facing.Left;
        public void FacingUpdate()
        {
            FsmFloat speed = HeroController.instance.superDash.FsmVariables.FindFsmFloat("Current SD Speed");
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (facing == Facing.Up)
            {
                if (HeroController.instance.cState.facingRight)
                {
                    rb.rotation = 90;
                }
                else
                {
                    rb.rotation = -90;
                }
                UpSpeed.Value = sdspeed;
                speed.Value = 0;
            }
            else if (facing == Facing.Down)
            {
                if (HeroController.instance.cState.facingRight)
                {
                    rb.rotation = -90;
                }
                else
                {
                    rb.rotation = 90;
                }
                UpSpeed.Value = -sdspeed;
                speed.Value = 0;
            }
            else if (facing == Facing.Left)
            {
                rb.rotation = 0;
                HeroController.instance.FaceLeft();
                UpSpeed.Value = 0;
                speed.Value = HeroController.instance.superDash.FsmVariables.FindFsmFloat("Superdash Speed neg").Value;
            }
            else if (facing == Facing.Right)
            {
                rb.rotation = 0;
                HeroController.instance.FaceRight();
                UpSpeed.Value = 0;
                speed.Value = HeroController.instance.superDash.FsmVariables.FindFsmFloat("Superdash Speed").Value;
            }
        }
        void Update()
        {
            HeroController.instance.superDash.FsmVariables.FindFsmFloat("Superdash Speed neg").Value = -sdspeed;
            HeroController.instance.superDash.FsmVariables.FindFsmFloat("Superdash Speed").Value = sdspeed;
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (HeroController.instance.cState.transitioning)
            {
                sdl = false;
                rb.rotation = 0;
                UpSpeed.Value = 0;
                return;
            }
            
            if (HeroController.instance.cState.superDashing)
            {
                HeroController.instance.SetDamageMode(GlobalEnums.DamageMode.HAZARD_ONLY);
                sdl = true;
                if (InputHandler.Instance.inputActions.up.IsPressed)
                {
                    facing = Facing.Up;
                    FacingUpdate();
                }
                if (InputHandler.Instance.inputActions.down.IsPressed)
                {
                    facing = Facing.Down;
                    FacingUpdate();
                }
                
                if (InputHandler.Instance.inputActions.left.IsPressed)
                {
                    facing = Facing.Left;
                    FacingUpdate();
                }
                if (InputHandler.Instance.inputActions.right.IsPressed)
                {
                    facing = Facing.Right;
                    FacingUpdate();
                }
            }
            else
            {
                if (!HeroController.instance.cState.transitioning)
                {
                    facing = HeroController.instance.cState.facingRight ? Facing.Right : Facing.Left;
                }
                if (sdl)
                {
                    HeroController.instance.SetDamageMode(GlobalEnums.DamageMode.FULL_DAMAGE);
                    sdl = false;
                    rb.rotation = 0;
                    UpSpeed.Value = 0;
                }
            }
        }
    }
}
