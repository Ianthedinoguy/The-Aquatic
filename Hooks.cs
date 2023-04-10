using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using UnityEngine;
using RWCustom;
using LizardCosmetics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Gunk;

sealed class Hooks
{
	internal static void Apply()
	{
        IL.OverseerAbstractAI.HowInterestingIsCreature += il =>
        {
            var c = new ILCursor(il);
            ILLabel? label = null;
            if (c.TryGotoNext(
                x => x.MatchLdarg(1),
                x => x.MatchLdfld<AbstractCreature>("creatureTemplate"),
                x => x.MatchLdfld<CreatureTemplate>("type"),
                x => x.MatchLdsfld<CreatureTemplate.Type>("BlackLizard"),
                x => x.MatchCall(out _),
                x => x.MatchBrtrue(out label))
            && label != null)
            {
                c.Emit(OpCodes.Ldarg_1);
                c.EmitDelegate((AbstractCreature testCrit) => testCrit.creatureTemplate.type == CreatureTemplateType.GunkLizard);
                c.Emit(OpCodes.Brtrue, label);
            }
            else
                GunkPlugin.logger.LogError("Couldn't ILHook OverseerAbstractAI.HowInterestingIsCreature!");
        };
        On.LizardLimb.ctor += delegate (On.LizardLimb.orig_ctor orig, global::LizardLimb self, global::GraphicsModule owner, global::BodyChunk connectionChunk, int num, float rad, float sfFric, float aFric, float huntSpeed, float quickness, global::LizardLimb otherLimbInPair)
        {
            orig(self, owner, connectionChunk, num, rad, sfFric, aFric, huntSpeed, quickness, otherLimbInPair);
            global::LizardGraphics i = owner as global::LizardGraphics;
            bool flag;
            if (i != null)
            {
                global::Lizard lizard = i.lizard;
                flag = (((lizard != null) ? lizard.Template.type : null) == CreatureTemplateType.GunkLizard);
            }
            else
            {
                flag = false;
            }
            bool flag2 = flag;
            if (flag2)
            {
            }
        };
        On.LizardVoice.GetMyVoiceTrigger += delegate (On.LizardVoice.orig_GetMyVoiceTrigger orig, global::LizardVoice self)
        {
            global::SoundID res = orig(self);
            global::Lizard i = self.lizard;
            bool flag = i != null && i.Template.type == CreatureTemplateType.GunkLizard;
            if (flag)
            {
                string[] array = new string[]
                {
                        "A",
                        "B",
                        "C",
                        "D",
                        "E"
                };
                List<global::SoundID> list = new List<global::SoundID>();
                for (int j = 0; j < array.Length; j++)
                {
                    global::SoundID soundID = global::SoundID.None;
                    string text2 = "Lizard_Voice_Black_" + array[j];
                    bool flag2 = ExtEnum<global::SoundID>.values.entries.Contains(text2);
                    if (flag2)
                    {
                        soundID = new global::SoundID(text2, false);
                    }
                    bool flag3 = soundID != global::SoundID.None && soundID.Index != -1 && i.abstractCreature.world.game.soundLoader.workingTriggers[soundID.Index];
                    if (flag3)
                    {
                        list.Add(soundID);
                    }
                }
                bool flag4 = list.Count == 0;
                if (flag4)
                {
                    res = global::SoundID.None;
                }
                else
                {
                    res = list[UnityEngine.Random.Range(0, list.Count)];
                }
            }
            return res;
        };
        On.LizardBreeds.BreedTemplate_Type_CreatureTemplate_CreatureTemplate_CreatureTemplate_CreatureTemplate += delegate (On.LizardBreeds.orig_BreedTemplate_Type_CreatureTemplate_CreatureTemplate_CreatureTemplate_CreatureTemplate orig, global::CreatureTemplate.Type type, global::CreatureTemplate lizardAncestor, global::CreatureTemplate pinkTemplate, global::CreatureTemplate blueTemplate, global::CreatureTemplate greenTemplate)
        {
            bool flag = type == CreatureTemplateType.GunkLizard;
            bool flag2 = flag;
            global::CreatureTemplate result;
            if (flag2)
            {
                global::CreatureTemplate temp = orig(global::CreatureTemplate.Type.Salamander, lizardAncestor, pinkTemplate, blueTemplate, greenTemplate);
                global::LizardBreedParams breedParams = temp.breedParameters as global::LizardBreedParams;
                temp.name = "GunkLizard";
                breedParams.baseSpeed = 2.5f;
                breedParams.terrainSpeeds[1] = new global::LizardBreedParams.SpeedMultiplier(1f, 1f, 1f, 1f);
                breedParams.terrainSpeeds[2] = new global::LizardBreedParams.SpeedMultiplier(1f, 1f, 1f, 1f);
                breedParams.terrainSpeeds[3] = new global::LizardBreedParams.SpeedMultiplier(1.1f, 1f, 1f, 1f);
                breedParams.terrainSpeeds[4] = new global::LizardBreedParams.SpeedMultiplier(0.1f, 1f, 1f, 1f);
                breedParams.terrainSpeeds[5] = new global::LizardBreedParams.SpeedMultiplier(0.1f, 1f, 1f, 1f);
                breedParams.standardColor = new Color(0.1f, 0.1f, 0.1f);
                breedParams.biteDelay = 300;
                temp.type = type;
                breedParams.tongue = true;
                breedParams.tongueChance = 0.9f;
                breedParams.tongueAttackRange = 30f;
                breedParams.tongueSegments = 90;
                breedParams.tongueWarmUp = 1;
                breedParams.biteInFront = 20f;
                breedParams.biteRadBonus = 20f;
                breedParams.biteHomingSpeed = 4.5f;
                breedParams.biteChance = 0.9f;
                breedParams.attemptBiteRadius = 120f;
                breedParams.getFreeBiteChance = 1f;
                breedParams.biteDamage = 2f;
                breedParams.biteDamageChance = 0.4f;
                breedParams.toughness = 14f;
                breedParams.stunToughness = 5f;
                breedParams.regainFootingCounter = 1;
                breedParams.bodyMass = 4f;
                breedParams.bodySizeFac = 1.25f;
                breedParams.floorLeverage = 8f;
                breedParams.maxMusclePower = 12f;
                breedParams.wiggleSpeed = 0.4f;
                breedParams.wiggleDelay = 20;
                breedParams.bodyStiffnes = 0.4f;
                breedParams.swimSpeed = 1.4f;
                breedParams.idleCounterSubtractWhenCloseToIdlePos = 10;
                breedParams.danger = 0.6f;
                breedParams.aggressionCurveExponent = 0.7f;
                breedParams.headShieldAngle = 160f;
                breedParams.canExitLounge = false;
                breedParams.canExitLoungeWarmUp = false;
                breedParams.findLoungeDirection = 0.5f;
                breedParams.loungeDistance = 250f;
                breedParams.preLoungeCrouch = 35;
                breedParams.preLoungeCrouchMovement = -0.4f;
                breedParams.loungeSpeed = 2.7f;
                breedParams.loungeMaximumFrames = 30;
                breedParams.loungePropulsionFrames = 30;
                breedParams.loungeJumpyness = 0.8f;
                breedParams.loungeDelay = 120;
                breedParams.riskOfDoubleLoungeDelay = 0.5f;
                breedParams.postLoungeStun = 60;
                breedParams.loungeTendensy = 0.3f;
                temp.visualRadius = 2300f;
                temp.waterVision = 0.7f;
                temp.throughSurfaceVision = 0.95f;
                breedParams.perfectVisionAngle = Mathf.Lerp(1f, -1f, 0.44444445f);
                breedParams.periferalVisionAngle = Mathf.Lerp(1f, -1f, 0.7777778f);
                breedParams.biteDominance = 1f;
                breedParams.limbSize = 1.5f;
                breedParams.stepLength = 1f;
                breedParams.liftFeet = 0.3f;
                breedParams.feetDown = 0.5f;
                breedParams.noGripSpeed = 0.25f;
                breedParams.limbSpeed = 6f;
                breedParams.limbQuickness = 0.6f;
                breedParams.limbGripDelay = 1;
                breedParams.smoothenLegMovement = true;
                breedParams.legPairDisplacement = 0.75f;
                breedParams.walkBob = 5f;
                breedParams.tailSegments = 18;
                breedParams.tailStiffness = 300f;
                breedParams.tailStiffnessDecline = 0.75f;
                breedParams.tailLengthFactor = 0.8f;
                breedParams.tailColorationStart = 0.9f;
                breedParams.tailColorationExponent = 8f;
                breedParams.headSize = 1.1f;
                breedParams.neckStiffness = 0.37f;
                breedParams.jawOpenAngle = 75f;
                breedParams.jawOpenLowerJawFac = 0.7666667f;
                breedParams.jawOpenMoveJawsApart = 25f;
                breedParams.headGraphics = new int[7];
                breedParams.framesBetweenLookFocusChange = 20;
                breedParams.tamingDifficulty = 6f;
                temp.movementBasedVision = 0.3f;
                temp.waterPathingResistance = 3f;
                temp.dangerousToPlayer = breedParams.danger;
                temp.doPreBakedPathing = false;
                temp.requireAImap = true;
                temp.baseDamageResistance = 3f;
                temp.meatPoints = 9;
                temp.canSwim = true;
                temp.preBakedPathingAncestor = global::StaticWorld.GetCreatureTemplate(global::CreatureTemplate.Type.Salamander);
                temp.throwAction = "Call";
                result = temp;
            }
            else
            {
                result = orig(type, lizardAncestor, pinkTemplate, blueTemplate, greenTemplate);
            }
            return result;
        };
        On.LizardGraphics.ctor += delegate (On.LizardGraphics.orig_ctor orig, global::LizardGraphics self, global::PhysicalObject ow)
        {
            orig(self, ow);
            bool flag = self.lizard.Template.type == CreatureTemplateType.GunkLizard;
            if (flag)
            {
                UnityEngine.Random.State state = UnityEngine.Random.state;
                UnityEngine.Random.InitState(self.lizard.abstractCreature.ID.RandomSeed);
                int num = self.startOfExtraSprites + self.extraSprites;
            }
        };
        On.LizardTongue.ctor += delegate (On.LizardTongue.orig_ctor orig, global::LizardTongue self, global::Lizard lizard)
        {
            orig(self, lizard);
            self.range = 560f;
            self.lashOutSpeed = 32f;
            self.reelInSpeed = 0.000625f;
            self.terrainDrag = (self.chunkDrag = 0.01f);
            self.dragElasticity = 0.1f;
            self.emptyElasticity = 0.8f;
            self.involuntaryReleaseChance = 0.0025f;
            self.voluntaryReleaseChance = 0.0125f;
            Hooks.s_elasticRange.SetValue(self, 0.55f);
            Hooks.s_totR.SetValue(self, self.range * 1.1f);
        };
    On.Lizard.ctor += (orig, self, abstractCreature, world) =>
        {
            orig(self, abstractCreature, world);
            if (self.Template.type == CreatureTemplateType.GunkLizard)
            {
                var state = Random.state;
                Random.InitState(abstractCreature.ID.RandomSeed);
                self.effectColor = Custom.HSL2RGB(Custom.WrappedRandomVariation(0.05f, .05f, .05f), .3f, Custom.ClampedRandomVariation(.1f, .1f, .1f));
                Random.state = state;
            }
        };
        On.LizardGraphics.ctor += (orig, self, ow) =>
        {
            orig(self, ow);
            if (self.lizard.Template.type == CreatureTemplateType.GunkLizard)
            {
                var state = Random.state;
                Random.InitState(self.lizard.abstractCreature.ID.RandomSeed);
                var num = self.startOfExtraSprites + self.extraSprites;
            }
        };
    }
    [AllowNull]
    private static FieldInfo s_totR = typeof(global::LizardTongue).GetField("totR", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    // Token: 0x04000008 RID: 8
    [AllowNull]
    private static FieldInfo s_elasticRange = typeof(global::LizardTongue).GetField("elasticRange", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
}