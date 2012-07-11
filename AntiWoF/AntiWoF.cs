﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hooks;
using Terraria;

namespace AntiWoF
{
    [APIVersion(1, 12)]
    public class AntiWoF : TerrariaPlugin
    {
        public override string Author
        {
            get { return "MarioE"; }
        }
        public override string Description
        {
            get { return "Disables the boxes that the wall of flesh spawns when defeated."; }
        }
        public override string Name
        {
            get { return "AntiWoF"; }
        }
        public override Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        public AntiWoF(Main game)
            : base(game)
        {
            Order = -10;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                NetHooks.GetData -= OnGetData;
            }
        }
        public override void Initialize()
        {
            NetHooks.GetData += OnGetData;
        }

        void OnGetData(GetDataEventArgs e)
        {
            if (!e.Handled)
            {
                if (e.MsgID == PacketTypes.NpcItemStrike)
                {
                    NPC npc = Main.npc[BitConverter.ToInt16(e.Msg.readBuffer, e.Index)];
                    Player plr = Main.player[e.Msg.readBuffer[e.Index + 2]];
                    if (npc.realLife >= 0)
                    {
                        npc = Main.npc[npc.realLife];
                    }
                    if (npc.type == 113 && npc.life - Main.CalculateDamage(plr.inventory[plr.selectedItem].damage, npc.defense) <= 0)
                    {
                        NetMessage.SendData(25, -1, -1, "Wall of Flesh has been defeated!", 255, 175, 75, 255);
                        npc.active = false;
                        e.Handled = true;
                    }
                }
                else if (e.MsgID == PacketTypes.NpcStrike)
                {
                    NPC npc = Main.npc[BitConverter.ToInt16(e.Msg.readBuffer, e.Index)];
                    if (npc.realLife >= 0)
                    {
                        npc = Main.npc[npc.realLife];
                    }
                    if (npc.type == 113)
                    {
                        double dmg = Main.CalculateDamage(BitConverter.ToInt16(e.Msg.readBuffer, e.Index + 2), npc.defense);
                        if (e.Msg.readBuffer[e.Index + 9] == 1)
                        {
                            dmg *= 2;
                        }
                        if (npc.life - dmg <= 0)
                        {
                            NetMessage.SendData(25, -1, -1, "Wall of Flesh has been defeated!", 255, 175, 75, 255);
                            npc.active = false;
                            e.Handled = true;
                        }
                    }
                }
            }
        }
    }
}