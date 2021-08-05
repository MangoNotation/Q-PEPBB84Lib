using System;
using System.Threading;

namespace BB84Core
{
    public class Game
    {
        string GameData;
        bool GameStarted;

        ServerHandler ServerHandler;
        Scene Scene;

        //set events
        public event EventHandler Update;
        public event EventHandler PlayerReadied;
        public event EventHandler GameReset;
        public event EventHandler GameStart;

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

        public void Ready()
        {
            //post ready to server
            ServerHandler.Ready();

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

        private Game()
        {
            ServerHandler = new ServerHandler("include url here");
            GameStarted = false;
            GameData = "";
        }

        public void SetScene(byte KeyLength)
        {
            Scene = new Scene(KeyLength);
        }

        public static Game Instance = new Game();
    }
}
