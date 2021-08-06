using System;
using System.Threading;

namespace BB84Core
{
    public class Game
    {
        string GameData;
        bool GameStarted;
        Player HostedPlayer;
        PlayerState[] States;

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
                OnGameReset(new EventArgs());
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
        public void Start()
        {
            //just post the initial gamedata
            GameData = Scene.BuildUpdate();

            if(States[0] == PlayerState.Hosted)
            {
                //play turn
            }
            else
            {
                //post a blank update
                ServerHandler.PostUpdate(GameData);
            }
        }

        public void Ready()
        {
            //post ready to server for player hosted
            ServerHandler.Ready(HostedPlayer);

            //begin update loop
            GetUpdate();
        }

        //post updated game to server
        public void PostUpdate()
        {
            //post gamedata to server
            ServerHandler.PostUpdate(GameData);
            //continue update loop
            GetUpdate();
        }

        //reset the game
        public void Reset()
        {
            ServerHandler.Reset();
        }

        private Game()
        {
            ServerHandler = new ServerHandler("include url here");
            GameStarted = false;
            InitialStates = new bool[4];
            for (int i = 0; i < InitialStates.Length; i++)
                InitialStates[i] = false;
            States = new PlayerState[3];

            GameData = "";
        }

        public void SetScene(byte KeyLength)
        {
            Scene = new Scene(KeyLength);
        }

        public static Game Instance = new Game();
    }
}
