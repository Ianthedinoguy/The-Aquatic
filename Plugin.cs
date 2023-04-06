using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using SlugBase.Features;
using System;
using IL;
using System.Collections.Generic;
using On;
using SlugBase;
using System.Linq;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using System.Runtime.CompilerServices;
using RWCustom;

namespace The_Aquatic
{
    [BepInPlugin(MOD_ID, "The Aquatic", "0.1.0")]
    internal class Plugin : BaseUnityPlugin
    {
        private const string MOD_ID = "toothandclaw.theaquatic";
        private static readonly PlayerFeature<bool> AquaticHead = PlayerBool("theaquatic/aquatichead");
        public static readonly PlayerFeature<float> SuperJump = PlayerFloat("theaquatic/super_jump");
        public PlayerFeature<bool> SuperSlide = PlayerBool("theaquatic/super_slide");
        public PlayerFeature<float> SuperCrawl = PlayerFloat("theaquatic/super_crawl");
        private static readonly PlayerFeature<bool> CustomTail = PlayerBool("theaquatic/fancytail");
        private static readonly PlayerFeature<bool> IsTheAquaticChar = FeatureTypes.PlayerBool("theaquatic/is_the_aquatic");


        public static SlugcatStats.Name TheAquatic { get; private set; }

        // Add hooks
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += Extras.WrapInit(new Action<RainWorld>(this.LoadResources));
            On.Player.Jump += Player_Jump;
            On.Player.UpdateBodyMode += Player_UpdateBodyMode;
            On.PlayerGraphics.DrawSprites += this.PlayerGraphics_DrawSprites;
            On.PlayerGraphics.ctor += this.PlayerGraphics_ctor;
            On.SaveState.ctor += this.SaveState_Ctor;
            IL.Player.UpdateAnimation += delegate (ILContext ilcontext)
            {
                ILCursor ilcursor = new ILCursor(ilcontext);
                ILCursor ilcursor2 = ilcursor;
                MoveType moveType = MoveType.Before;
                Func<Instruction, bool>[] array = new Func<Instruction, bool>[1];
                array[0] = ((Instruction i) => i.MatchLdcR4(18.1f));
                bool flag = !ilcursor2.TryGotoNext(moveType, array);
                if (!flag)
                {
                    ilcursor.MoveAfterLabels();
                    ilcursor.RemoveRange(2);
                    ilcursor.Emit(OpCodes.Ldarg_0);
                    ilcursor.EmitDelegate<Func<global::Player, float>>(delegate (global::Player player)
                    {
                        bool flag2 = player != null;
                        float result;
                        if (flag2)
                        {
                            result = 14f;
                        }
                        else
                        {
                            result = 18.1f;
                        }
                        return result;
                    });
                    ilcursor.Emit(OpCodes.Stloc, 24);
                    ilcursor.Emit(OpCodes.Ldarg_0);
                    ilcursor.EmitDelegate<Func<global::Player, float>>(delegate (global::Player player)
                    {
                        bool flag3;
                        bool flag2 = this.SuperSlide.TryGet(player, out flag3) && flag3;
                        float result;
                        if (flag2)
                        {
                            result = 22f;
                        }
                        else
                        {
                            result = 28f;
                        }
                        return result;
                    });
                    ilcursor.Emit(OpCodes.Stloc, 25);
                }
            };

        }


        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld)
        {
            global::Futile.atlasManager.LoadAtlas("atlases/aquaticsprites");
        }


        // Implement SuperJump
        private void Player_Jump(On.Player.orig_Jump orig, Player self)
        {
            orig(self);

            if (SuperJump.TryGet(self, out float power))
            {
                self.jumpBoost *= 1f + power;
            }
        }


        //Implement Player throw thing
        private static void Player_ThrowObject(ILContext il)
        {
            ILCursor ilcursor = new(il);
            ILCursor ilcursor2 = ilcursor;
            MoveType moveType = MoveType.Before;
            Func<Instruction, bool>[] array = new Func<Instruction, bool>[1];
            bool flag = !ilcursor2.TryGotoNext(moveType, array);
            if (!flag)
            {
                ILCursor ilcursor3 = ilcursor;
                MoveType moveType2 = MoveType.Before;
                Func<Instruction, bool>[] array2 = new Func<Instruction, bool>[1];
                array2[0] = ((Instruction i) => i.MatchLdloc(1));
                bool flag2 = !ilcursor3.TryGotoNext(moveType2, array2);
                if (!flag2)
                {
                    ilcursor.MoveAfterLabels();
                    ilcursor.Emit(OpCodes.Ldarg_0);
                    ilcursor.EmitDelegate<Func<global::Player, bool>>(delegate (global::Player player)
                    {
                        bool flag3;
                        return Plugin.IsTheAquaticChar.TryGet(player, out flag3) && flag3 && player.bodyChunks[1].ContactPoint.y != -1 && !(player.bodyMode == global::Player.BodyModeIndex.Crawl) && !player.standing;
                    });
                    ilcursor.Emit(OpCodes.Or);
                }
            }
        }


        // Implement Custom Tail
        private void PlayerGraphics_ctor(On.PlayerGraphics.orig_ctor orig, global::PlayerGraphics self, global::PhysicalObject ow)
        {
            orig(self, ow);
            bool flag = Plugin.CustomTail.TryGet(self.player, out bool flag2) && flag2;
            if (flag)
            {
                self.tail = new global::TailSegment[6];
                self.tail[0] = new global::TailSegment(self, 10f, 4f, null, 0.1f, 1f, 1f, true);
                self.tail[1] = new global::TailSegment(self, 8f, 7f, self.tail[0], 0.85f, 1f, 0.5f, true);
                self.tail[2] = new global::TailSegment(self, 6f, 7f, self.tail[1], 0.85f, 1f, 0.5f, true);
                self.tail[3] = new global::TailSegment(self, 4f, 7f, self.tail[2], 0.85f, 1f, 0.5f, true);
                self.tail[4] = new global::TailSegment(self, 2f, 7f, self.tail[3], 0.85f, 1f, 0.5f, true);
                self.tail[5] = new global::TailSegment(self, 1f, 7f, self.tail[4], 0.85f, 1f, 0.5f, true);
                List<global::BodyPart> list = self.bodyParts.ToList<global::BodyPart>();
                list.RemoveAll((global::BodyPart x) => x is global::TailSegment);
                list.AddRange(self.tail);
                self.bodyParts = list.ToArray();
            }
        }

        // Implement Playerclass
        static Plugin()
        {
            Plugin.TheAquatic = (SlugcatStats.Name)new SlugcatStats.Name("Aquatic", false);
        }


        // Implement SuperCrawl
        public void Player_UpdateBodyMode(On.Player.orig_UpdateBodyMode orig, global::Player self)
        {
            orig(self);
            bool flag = !self.standing && SuperCrawl.TryGet(self, out _) && (self.bodyMode == Player.BodyModeIndex.Default || self.bodyMode == global::Player.BodyModeIndex.Crawl);
            if (flag)
            {
                self.dynamicRunSpeed[0] += 3;
                self.dynamicRunSpeed[1] += 6;
            }
        }

        //Implement SaveState
        private void SaveState_Ctor(On.SaveState.orig_ctor orig, global::SaveState self, global::SlugcatStats.Name saveStateNumber, global::PlayerProgression progression)
        {
            orig(self, saveStateNumber, progression);
            bool flag = self.saveStateNumber == Plugin.TheAquatic;
            if (flag)
            {
                self.deathPersistentSaveData.theMark = true;
            }
        }

        // Implement SpearStrong
        public void Player_ThrownSpear(On.Player.orig_ThrownSpear orig, global::Player self, global::Spear spear)
        {
            orig(self, spear);
            bool flag = self.SlugCatClass == Plugin.TheAquatic;
            if (flag)
            {
                spear.spearDamageBonus = 99999999999999999999999.0f;
                global::BodyChunk firstChunk = spear.firstChunk;
                global::BodyChunk bodyChunk = firstChunk;
                bodyChunk.vel.x = bodyChunk.vel.x * 999999999999999999999.0f;
            }
        }

        // Implement AquaticHead
        private void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, global::PlayerGraphics self, global::RoomCamera.SpriteLeaser sLeaser, global::RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            global::FSprite fsprite = sLeaser.sprites[3];
            bool flag2;
            bool flag = Plugin.AquaticHead.TryGet(self.player, out flag2) && flag2;
            if (flag)
            {
                bool flag3 = !fsprite.element.name.Contains("Aquatic");
                if (flag3)
                {
                    fsprite.SetElementByName("Aquatic" + fsprite.element.name);
                }
            }
        }
    }
}