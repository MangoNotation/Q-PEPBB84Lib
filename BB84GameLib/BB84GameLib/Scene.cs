using System;
namespace BB84GameLib
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

    public class Scene
    {
        public byte Keylength { get; private set; }
        public byte TurnNumber { get; private set; }

        public GameMode GameMode { get; private set; }

        public bool[] AliceBases { get; private set; }
        public bool[] BobBases { get; private set; }
        public bool[] EveBases { get; private set; }

        public bool[] AliceQubits { get; private set; }
        public bool[] BobQubits { get; private set; }
        public bool[] EveQubits { get; private set; }


        //bases selection update structure
        //Turn Number, Player, Alice/Bob/Eve Bases
        //Alice/Bob/Eve Qubits

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
        }

        public void ParseUpdate(string update)
        {
            //alice bob eve is always the order
            string[] splitUpdate = update.Split('.');
            TurnNumber = byte.Parse(splitUpdate[0]);

            GameMode = (GameMode)byte.Parse(splitUpdate[1]);

        }

        public string BuildUpdate()
        {
            //put together all information in the correct order and return
            //an update parseable by the ParseUpdate functiom

            //return
            return "not implemented";
        }
    }
}
