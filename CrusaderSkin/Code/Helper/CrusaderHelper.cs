using CrusaderSkin.Code.ModConfig;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using System;

namespace CrusaderSkin.Code.Helper;

public static class CrusaderHelper
{
    public static bool IsIronclad(Creature? creature)
    {
        return creature != null && creature.Player != null && creature.Player.Character is Ironclad;
    }
    public static bool IsIronclad(Player? player)
    {
        return player != null && player.Character is Ironclad;
    }
    public static bool IsCardOfIronclad(CardModel? cardModel)
    {
        return cardModel != null && cardModel.Pool is IroncladCardPool;
    }
    public static bool IsLowHealth(Creature? creature, decimal lowHealthPercent = 25m, decimal delta = 0m)
    {
        return creature != null && (creature.CurrentHp - delta) * 100m / creature.MaxHp <= lowHealthPercent;
    }
    public static void ResetAdvancedConditions(AnimationTree? animTree, Creature creature)
    {
        AnimationTree? animationTree = animTree;
        if (animationTree == null && creature != null)
        {
            NCreature? creatureNode = creature.GetCreatureNode();
            if (creatureNode != null)
            {
                animationTree = creatureNode.Visuals.GetNodeOrNull<AnimationTree>("AnimationTree");
            }
        }
        if (animationTree != null)
        {
            /*if (FlagellantConfig.ShouldUseDeathDoorIdle)
            {
                animationTree.Set("parameters/conditions/HitToIdle", !CrusaderHelper.IsLowHealth(creature));
                animationTree.Set("parameters/conditions/HitToDeathIdle", CrusaderHelper.IsLowHealth(creature));
            }
            else*/
            {
                animationTree.Set("parameters/conditions/HitToIdle", true);
                animationTree.Set("parameters/conditions/HitToDeathIdle", false);
            }
        }
    }
    public static bool IsInAnyIdle(AnimationTree? animTree, Creature creature)
    {
        AnimationTree? animationTree = animTree;
        if (animationTree == null && creature != null)
        {
            NCreature? creatureNode = creature.GetCreatureNode();
            if (creatureNode != null)
            {
                animationTree = creatureNode.Visuals.GetNodeOrNull<AnimationTree>("AnimationTree");
            }
        }
        if (animationTree != null)
        {
            var state_machine = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
            if (state_machine != null)
            {
                return state_machine.GetCurrentNode() == "Idle"; //||
                    //state_machine.GetCurrentNode() == "Revive";
                //|| state_machine.GetFadingFromNode() == "Idle" || xxxxxx;
            }
        }
        return false;
    }
    public static void SetBannerAndScriptureVisibility(NCreature nCreature, string animName)
    {
        if (nCreature == null) return;

        Node3D? Banner = nCreature.Visuals.GetNodeOrNull<Node3D>("Visuals/SubViewportContainer/SubViewport/mdl_crusader/mdl_crusader (merge)/mdl_crusader/SHJntGrp/Skeleton3D/msh_crusader_wpn_banner");
        Node3D? Scripture = nCreature.Visuals.GetNodeOrNull<Node3D>("Visuals/SubViewportContainer/SubViewport/mdl_crusader/mdl_crusader (merge)/mdl_crusader/SHJntGrp/Skeleton3D/msh_crusader_wpn_scripture");
        if (Banner != null)
        {
            Banner.Visible = ShouldShowBanner(animName);
        }
        if (Scripture != null)
        {
            Scripture.Visible = ShouldShowScripture(animName);
        }
    }
    public static void SetBladeFlameVisibility(NCreature nCreature, string animName)
    {
        if (nCreature == null) return;

        GpuParticles3D? BladeFlameLeft = nCreature.Visuals.GetNodeOrNull<GpuParticles3D>("Visuals/SubViewportContainer/SubViewport/mdl_crusader/mdl_crusader (merge)/mdl_crusader/SHJntGrp/Skeleton3D/BladeAttachment/BladeFlameLeft");
        GpuParticles3D? BladeFlameRight = nCreature.Visuals.GetNodeOrNull<GpuParticles3D>("Visuals/SubViewportContainer/SubViewport/mdl_crusader/mdl_crusader (merge)/mdl_crusader/SHJntGrp/Skeleton3D/BladeAttachment/BladeFlameRight");
        MeshInstance3D? BladeGlowBody = nCreature.Visuals.GetNodeOrNull<MeshInstance3D>("Visuals/SubViewportContainer/SubViewport/mdl_crusader/mdl_crusader (merge)/mdl_crusader/SHJntGrp/Skeleton3D/BladeAttachment/BladeGlowBody");
        if (BladeFlameLeft != null && BladeFlameRight != null)
        {
            BladeFlameLeft.Emitting = ShouldShowBladeFlame(animName);
            BladeFlameRight.Emitting = ShouldShowBladeFlame(animName);
            BladeFlameLeft.Position = new Vector3(BladeFlameLeft.Position.X, GetBladeFlameParticlePositionY(animName), BladeFlameLeft.Position.Z);
            BladeFlameRight.Position = new Vector3(BladeFlameRight.Position.X, GetBladeFlameParticlePositionY(animName), BladeFlameRight.Position.Z);
        }
        if (BladeGlowBody != null)
        {
            BladeGlowBody.Visible = ShouldShowBladeFlame(animName);
        }
    }
    public static bool ShouldShowScripture(string animName)
    {
        if (!animName.Contains("CardPlay/")) return false;

        if (animName.Contains("Accus")
            || animName.Contains("Heal") )
        {
            return true;
        }
        return false;
    }
    public static bool ShouldShowBanner(string animName)
    {
        if (!animName.Contains("CardPlay/")) return false;

        if (animName.Contains("Insp")
            || animName.Contains("Rally")
            || animName.Contains("Tenacity"))
        {
            return true;
        }
        return false;
    }
    public static bool ShouldShowBladeFlame(string animName)
    {
        if (animName == "CardSelect/Holy"
            || animName == "CardPlay/Holy"
            || animName == "CardPlay/Radiance")
        {
            return true;
        }
        return false;
    }
    public static void TryPlayCombatEffect(string animName, NCreature nCreature)
    {
        if (!CrusaderSettings.PlayCardVfx) return;

        bool shouldReturn = false;
        string effectPath = "";
        bool shouldBackContainer = false;
        bool useCenterPos = true;
        switch (animName)
        {
            case "CardPlay/Smite":
                effectPath = "res://crusader_assets/Effects/SmiteEffect.tscn";
                useCenterPos = false;
                break;
            case "CardPlay/Reap":
                effectPath = "res://crusader_assets/Effects/ReapEffect.tscn";
                useCenterPos = false;
                break;
            case "CardPlay/Bulwark":
                effectPath = "res://crusader_assets/Effects/SelfGlow.tscn";
                shouldBackContainer = true;
                break;
            case "CardPlay/Tenacity":
                effectPath = "res://crusader_assets/Effects/SelfGlow.tscn";
                shouldBackContainer = true;
                break;
            case "CardPlay/Insp":
                effectPath = "res://crusader_assets/Effects/BirdGlow.tscn";
                shouldBackContainer = true;
                useCenterPos = false;
                break;
            case "CardPlay/Rally":
                effectPath = "res://crusader_assets/Effects/DustFlow.tscn";
                useCenterPos = false;
                break;
            default:
                shouldReturn = true;
                break;
        }
        if (shouldReturn) return;
        Node2D effectNode = PreloadManager.Cache.GetScene(effectPath).Instantiate<Node2D>();
        if (effectNode == null) return;

        if (effectNode != null && effectNode.GetParent() == null && NCombatRoom.Instance != null && nCreature != null)
        {
            var parentNode = shouldBackContainer ? NCombatRoom.Instance.BackCombatVfxContainer : NCombatRoom.Instance.CombatVfxContainer;
            parentNode.AddChild(effectNode);

            Marker2D centerPos = nCreature.Visuals.GetNodeOrNull<Marker2D>("%CenterPos");
            effectNode.GlobalPosition = centerPos != null && useCenterPos ? centerPos.GlobalPosition + GetEffectNodeOffSet(animName, nCreature) : nCreature.GlobalPosition;
            effectNode.Scale *= nCreature.Visuals.Scale;
        }

        var AnimPlayer = effectNode.GetNodeOrNull<AnimationPlayer>("%AnimationPlayer");
        if (AnimPlayer != null)
        {
            AnimPlayer.Stop();
            AnimPlayer.Play("Show");
            AnimPlayer.AnimationFinished += (animName) =>
            {
                //每次都是新生成的特效结点，不用判断是否重复连接信号
                if (effectNode != null && !effectNode.IsQueuedForDeletion())
                {
                    effectNode.QueueFreeSafely();
                }
            };
        }
    }
    private static Vector2 GetEffectNodeOffSet(string animName, NCreature nCreature)
    {
        if (nCreature == null) return Vector2.Zero;
        Control bounds = nCreature.Visuals.GetNodeOrNull<Control>("%Bounds");
        if (bounds == null) return Vector2.Zero;

        float offSetX = 0.0f;
        float offSetY = 0.0f;

        //想偏移多少改这里
        float offSetScaleX = 0.0f;  //限定了float类型，防止直接填俩int进去相除还不知道咋回事呢
        float offSetScaleY = 0.0f;  //限定了float类型，防止直接填俩int进去相除还不知道咋回事呢
        switch (animName)
        {
            case "CardPlay/Bulwark":
                offSetScaleX = 1.0f;
                offSetScaleY = -2.8f;
                break;
            case "CardPlay/Tenacity":
                offSetScaleX = 0.0f;
                offSetScaleY = -2.8f;
                break;
            default:
                break;
        }
        offSetX = offSetScaleX != 0.0f ? bounds.Size.X * (offSetScaleX / 10.0f) * Math.Min(1.0f, nCreature.Visuals.Scale.X) : 0.0f;
        offSetY = offSetScaleY != 0.0f ? bounds.Size.Y * (offSetScaleY / 10.0f) * Math.Min(1.0f, nCreature.Visuals.Scale.Y) : 0.0f;
        return new Vector2(offSetX, offSetY);
    }
    private static float GetBladeFlameParticlePositionY(string animName)
    {
        float offSetY = 0.0f;
        switch (animName)
        {
            case "CardPlay/Radiance":
                offSetY = -0.1f;
                break;
            default:
                offSetY = 0.1f;
                break;
        }
        return offSetY;
    }
    public static bool CanTravelTo(string nodeName)
    {
        switch (nodeName)
        {
            case "Smite":
            case "Stun":
            case "Accus":
            case "Heal":
            case "Insp":
            case "Rally":
            case "Tenacity":
            case "Reap":
            case "Bulwark":
            case "Radiance":
            case "Holy":
            case "Mercy":
                return true;
            default:
                return false;
        }
    }
}
