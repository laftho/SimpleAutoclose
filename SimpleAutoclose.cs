using System;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;

namespace laftho.SimpleAutoclose
{
    class Program : Sandbox.ModAPI.Ingame.MyGridProgram
    {
        #region script

        /* Simple Auto Close Door Script for Space Engineers
           Thomas LaFreniere aka laftho
           v1.0 - May 29, 2016

            This script is will simply auto close doors on your grid after
            300ms if it has the [autoclose] tag in the name.

            Setup the Programmable Block with an Timer Block loop on Trigger Now.

            Default arguments:
            Key: [autoclose]
            Delay: 300ms

            Run script with optional arguments:
            key,delay

            E.g.

            argument: [fancyDoor],200

            This will find all doors tagged [fancyDoor] and auto close after 200ms.

            If you wish to close any doors, omit the key in the arguments and simply
            provide a delay integer.

            E.g.

            argument: 150

            This will auto close all doors after 150ms.

            Note it takes 100ms for the door animation to go from closed to open. Unless
            you want impossible to open doors, set this value above 100ms.
         */

        AutoCloseDoors doors;

        public Program()
        {
            doors = new AutoCloseDoors(this);
        }

        public void Main(string argument)
        {
            if (!string.IsNullOrEmpty(argument)) {
                string key = null;
                int delay = -1;

                if (argument.Contains(","))
                {
                    string[] parts = argument.Split(',');

                    if (parts.Length >= 2)
                    {
                        if (!Int32.TryParse(parts[0], out delay))
                            key = parts[0];
                        else
                            Int32.TryParse(parts[1], out delay);
                    }
                } else
                {
                    if (!Int32.TryParse(argument, out delay))
                        key = argument;
                }

                if (key != doors.Key || doors.Delay != delay)
                    doors = new AutoCloseDoors(this, key, delay);
            }

            doors.Run();
        }
        
        class AutoCloseDoors
        {
            class Door
            {
                public IMyDoor MyDoor { get; private set; }
                public int Delay { get; set; }

                public string Id
                {
                    get
                    {
                        var pos = MyDoor.Position;
                        return pos.X + ":" + pos.Y + ":" + pos.Z;
                    }
                }

                private bool PreviousOpen = false;
                public bool IsOpen { get { return MyDoor.Open; } }
                private int Ticks { get; set; }

                public Door(IMyDoor door, int delay = 300)
                {
                    MyDoor = door;
                    Delay = delay;
                    Update();
                }

                private void Tick() { if (Ticks > 0) Ticks -= 1; }

                public void Close()
                {
                    MyDoor.ApplyAction("Open_Off");
                }

                public void Update()
                {
                    if (!PreviousOpen && IsOpen)
                        Ticks = Delay;

                    if (Ticks <= 0 && IsOpen)
                        Close();

                    PreviousOpen = IsOpen;
                    Tick();
                }
            }

            public string Key { get; private set; }
            public int Delay { get; private set; }
            Sandbox.ModAPI.Ingame.MyGridProgram Program;
            Dictionary<string, Door> doors;
            
            public AutoCloseDoors(Sandbox.ModAPI.Ingame.MyGridProgram program, string key = "[autoclose]", int delay = 300)
            {
                doors = new Dictionary<string, Door>();
                Key = key;
                Delay = (delay < 0) ? 300 : delay;
                Program = program;
            }

            private void Init()
            {
                var blocks = new List<IMyTerminalBlock>();
                Program.GridTerminalSystem.GetBlocksOfType<IMyDoor>(blocks, block => ((!string.IsNullOrEmpty(Key)) ? block.CustomName.Contains(Key): true) && block.CubeGrid == Program.Me.CubeGrid);
                
                var deleteKeys = new List<string>(doors.Keys);

                foreach (var block in blocks)
                {
                    var door = new Door(block as IMyDoor, Delay);

                    if (!doors.ContainsKey(door.Id))
                    {
                        doors.Add(door.Id, door);
                    }
                    else
                    {
                        deleteKeys.Remove(door.Id);
                        door = doors[door.Id];
                        door.Delay = Delay;
                    }
                }

                foreach (var key in deleteKeys)
                    doors.Remove(key);
            }

            public void Run()
            {
                Init();
                foreach (var door in doors.Values) door.Update();
            }
        }

        #endregion
    }
}
