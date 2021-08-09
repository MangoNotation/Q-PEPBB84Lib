using System;
namespace BB84Core
{
    public enum GameMode : byte
    {
        Blank,
        CharacterSelection,
        BasisSelection,
        KeepDisregard
    }

    public enum Player : byte
    {
        Alice,
        Bob,
        Eve
    }

    public enum PlayerState : byte
    {
        Hosted, //played actively on this computer
        Auto, //played auto by this computer
        Remote, //hosted by another player
        Unassigned
    }

    public class Scene
    {
        //declarations
        public byte Keylength { get; private set; }
        public byte TurnNumber { get; private set; }

        public GameMode GameMode { get; private set; }
        public Player ActivePlayer { get; private set; }

        public bool[] AliceBases { get; private set; }
        public bool[] BobBases { get; private set; }
        public bool[] EveBases { get; private set; }

        public bool[] AliceQubits { get; private set; }
        public bool[] BobQubits { get; private set; }
        public bool[] EveQubits { get; private set; }

        public bool[] Keep { get; private set; }
        //end declarations


        public Scene(byte keylength)
        {
            Keylength = keylength;

            AliceBases = new bool[keylength];
            BobBases = new bool[keylength];
            EveBases = new bool[keylength];

            AliceQubits = new bool[keylength];
            //set qubits
            for (int i = 0; i < keylength; i++)
            {
                AliceQubits[i] = new Random().Next(0, 2) == 1;
            }
            BobQubits = new bool[keylength];
            EveBases = new bool[keylength];

            Keep = new bool[keylength];
        }


        //bases selection update structure
        //Gamemode - 0
        //Turn Number - 1/AliceKeep
        //Player - 2/BobKeep
        //Alice/Bob/Eve Bases - 3, 4, 5
        //Alice/Bob/Eve Qubits - 6, 7, 8
        public void ParseUpdate(string update)
        {

            //alice bob eve is always the order
            string[] splitUpdate = update.Split('.');
            
            if((GameMode)byte.Parse(splitUpdate[0]) != GameMode && GameMode == GameMode.KeepDisregard)
            {
                SwitchMode();
            }

            GameMode = (GameMode)byte.Parse(splitUpdate[0]);

            if(GameMode == GameMode.BasisSelection)
            {
                TurnNumber = byte.Parse(splitUpdate[1]);
                ActivePlayer = (Player)byte.Parse(splitUpdate[2]);

                char[] alice = splitUpdate[3].ToCharArray();
                char[] bob = splitUpdate[4].ToCharArray();
                char[] eve = splitUpdate[5].ToCharArray();

                char[] aliceq = splitUpdate[6].ToCharArray();
                char[] bobq = splitUpdate[7].ToCharArray();
                char[] eveq = splitUpdate[8].ToCharArray();

                for (int i = 0; i < TurnNumber; i++)
                {
                    AliceBases[i] = alice[i] == '1' ? true : false;
                    BobBases[i] = bob[i] == '1' ? true : false;
                    EveBases[i] = eve[i] == '1' ? true : false;

                    AliceQubits[i] = aliceq[i] == '1' ? true : false;
                    BobQubits[i] = bobq[i] == '1' ? true : false;
                    EveQubits[i] = eveq[i] == '1' ? true : false;
                }
            }
            //kd
            else if(GameMode == GameMode.KeepDisregard)
            {
                char[] alice = splitUpdate[1].ToCharArray();
                char[] bob = splitUpdate[2].ToCharArray();


                for(int i = 0; i < alice.Length; i++)
                {
                    if(bob[i] == '0')
                    {
                        Keep[i] = false;
                    }
                    else
                    {
                        Keep[i] = true;
                    }
                }
            }
            else
            {
                //do nothing, just wait
            }

        }

        public byte PushKeep(bool keep)
        {
            Keep[TurnNumber] = keep;
            return ++TurnNumber;
        }

        //switch mode
        public void SwitchMode()
        {
            TurnNumber = 0;
        }

        //next player/turn
        public void NextPlayer(bool Basis)
        {
            if((TurnNumber == Keylength - 1) && ActivePlayer == Player.Bob)
            {
                GameMode = GameMode.KeepDisregard;
                SwitchMode();
            }
            else
            {
                switch (ActivePlayer)
                {
                    case Player.Alice:
                        AliceBases[TurnNumber] = Basis;
                        ActivePlayer = Player.Eve;
                        break;
                    case Player.Bob:
                        BobBases[TurnNumber] = Basis;
                        if (Basis == EveBases[TurnNumber])
                        {
                            BobQubits[TurnNumber] = EveBases[TurnNumber];
                        }
                        else
                        {
                            BobQubits[TurnNumber] = new Random().Next(0, 2) == 1;
                        }
                        ActivePlayer = Player.Alice;
                        TurnNumber++;
                        break;
                    case Player.Eve:
                        EveBases[TurnNumber] = Basis;
                        if (Basis == AliceBases[TurnNumber])
                        {
                            EveQubits[TurnNumber] = AliceBases[TurnNumber];
                        }
                        else
                        {
                            EveQubits[TurnNumber] = new Random().Next(0, 2) == 1;
                        }
                        ActivePlayer = Player.Bob;
                        break;
                }
            }
        }

        public string BuildUpdate()
        {
            string update = "";
            if(GameMode == GameMode.BasisSelection)
            {
                update += ((byte)GameMode).ToString() + "." + TurnNumber.ToString() +
                    "." + (byte)ActivePlayer + "." + BuildString(AliceBases) + "." +
                    "." + BuildString(BobBases) + "." + BuildString(EveQubits) + "." +
                    "." + BuildString(AliceQubits) + "." + BuildString(BobQubits) +
                    "." + BuildString(EveQubits);

            }
            else if(GameMode == GameMode.KeepDisregard)
            {
                update += ((byte)GameMode).ToString() + "." + BuildString(Keep);
            }
            //put together all information in the correct order and return
            //an update parseable by the ParseUpdate functiom

            //return
            return update;
        }

        string BuildString(bool[] input)
        {
            string ret = "";
            foreach(bool b in input)
            {
                ret += b ? "1" : "0";
            }
            return ret;
        }
    }
}
