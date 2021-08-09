using System;
using System.Threading;

namespace BB84Core
{
    public class Game
    {
        string GameData;
        bool GameStarted;
        public Player HostedPlayer { get; private set; }
        public PlayerState[] States { get; private set; }

        ServerHandler ServerHandler;
        public Scene Scene { get; private set; }

        //set events
        public event EventHandler Update;
        public event EventHandler PlayerReadied;
        public event EventHandler GameReset;
        public event EventHandler GameStart;

        public bool[] InitialStates;

        protected virtual void OnUpdate(EventArgs e)
        {
            //parse the update
            Scene.ParseUpdate(GameData);

            //invoke the event, let the visualizer handle what happens with 
            //the information
            EventHandler handler = Update;
            handler?.Invoke(this, e);
        }
        protected virtual void OnPlayerReadied(EventArgs e)
        {
            EventHandler handler = PlayerReadied;
            handler.Invoke(this, e);
        }
        protected virtual void OnGameReset(EventArgs e)
        {
            GameStarted = false;
            GameData = "";
            EventHandler handler = GameReset;
            handler?.Invoke(this, e);

            Instance = new Game();
        }
        protected virtual void OnGameStart(EventArgs e)
        {
            EventHandler handler = GameStart;
            handler?.Invoke(this, e);
        }
        //end event declarations

        //wait for a new update from the server
        public void GetUpdate()
        { 
            while(GameData == ServerHandler.GetUpdate())
            {
                Thread.Sleep(500);
            }
            GameData = ServerHandler.GetUpdate();
            if (GameData.Contains("n"))
            {
                //signifies game has been reset
                Reset();
            }
            //if the game hasn't registered start - start it
            if (!GameStarted)
            {
                OnGameStart(new EventArgs());
                GameStarted = true;
            }

            OnUpdate(new EventArgs());
        }

        //setup
        public void InitialSetup()
        {
            string initialStates = ServerHandler.Initial();

            if (initialStates.Contains('s'))
            {
                InitialStates[0] = true;
            }
            if (initialStates.Contains('a'))
            {
                States[0] = PlayerState.Remote;
                InitialStates[1] = true;
            }
            if (initialStates.Contains('b'))
            {
                States[1] = PlayerState.Remote;
                InitialStates[2] = true;
            }
            if (initialStates.Contains('e'))
            {
                States[2] = PlayerState.Remote;
                InitialStates[3] = true;
            }
        }

        //start
        public bool Start()
        {
            //just post the initial gamedata
            GameData = Scene.BuildUpdate();

            if (InitialStates[0])
            {
                Refresh();

                for (int i = 0; i < States.Length; i++)
                {
                    States[i] = States[i] == PlayerState.Unassigned ? PlayerState.Auto : States[i];
                }
            }


            if(States[0] == PlayerState.Hosted)
            {
                return true;
            }
            else
            {
                //post a blank update
                ServerHandler.PostUpdate(GameData);
                return false;
            }
        }

        //Play Character
        public bool PlayCharacter(Player player)
        {
            bool ret = ServerHandler.RequestPlayer(player).Contains('n');
            States[(byte)player] = ret ? PlayerState.Hosted : PlayerState.Remote;
            if (ret)
            {
                HostedPlayer = player;
            }
            return ret;
        }

        //
        public void Ready()
        {
            //post ready to server for player hosted
            ServerHandler.Ready(HostedPlayer);
            Refresh();

            //begin update loop
            GetUpdate();
        }

        //post updated game to server
        public void PostUpdate()
        {
            //post gamedata to server
            ServerHandler.PostUpdate(GameData);
            //continue update loop

            if (ServerHandler.GetUpdate().Contains('n'))
            {
                Reset();
            }
            else
            {
                GetUpdate();
            }
        }

        //reset the game
        public void Reset()
        {
            ServerHandler.Reset();
            OnGameReset(new EventArgs());
        }

        //input the next step
        public void NextStep(bool basis)
        {
            Scene.NextPlayer(basis);
        }

        //refresh
        public void Refresh()
        {
            string states = ServerHandler.PlayerStates();

            if (states.Contains('a') && HostedPlayer != Player.Alice)
            {
                States[0] = PlayerState.Remote;
            }
            if (states.Contains('b') && HostedPlayer != Player.Bob)
            {
                States[1] = PlayerState.Remote;
            }
            if (states.Contains('e') && HostedPlayer != Player.Eve)
            {
                States[2] = PlayerState.Remote;
            }
        }

        //post keylength
        public void PostKeyLength(byte length)
        {
            ServerHandler.PostKeyLength(length);
        }

        //get Keylength
        public byte GetKeyLength()
        {
            while (ServerHandler.GetKeyLength() == "")
            {
                Thread.Sleep(250);
            }

            return byte.Parse(ServerHandler.GetKeyLength());
        }

        private Game()
        {
            ServerHandler = new ServerHandler("http://23.254.165.102");
            GameStarted = false;
            InitialStates = new bool[4];
            for (int i = 0; i < InitialStates.Length; i++)
                InitialStates[i] = false;
            States = new PlayerState[3];
            for (int i = 0; i < States.Length; i++)
                States[i] = PlayerState.Unassigned;

            GameData = "";
        }

        public void SetScene(byte KeyLength)
        {
            Scene = new Scene(KeyLength);
        }

        public static Game Instance = new Game();
    }
}
