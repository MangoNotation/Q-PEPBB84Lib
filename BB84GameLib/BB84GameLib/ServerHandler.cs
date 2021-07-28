using System;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Collections.Specialized;

namespace BB84GameLib
{
    public class ServerHandler
    {
        string URL;

        WebRequest WR;
        Stream ObjStream;
        StreamReader Reader;

        public ServerHandler(string URL)
        {
            this.URL = URL;
        }

        //pulls update from server
        public string GetUpdate()
        {
            throw new NotImplementedException();
        }

        //posts update from server and returns update
        public string PostUpdate(string update)
        {
            throw new NotImplementedException();
        }

        //requests player from server
        public string RequestPlayer(Player player)
        {
            throw new NotImplementedException();
        }
        //gets player states
        public string PlayerStates()
        {
            throw new NotImplementedException();
        }

        //get/post keylength
        public string GetKeyLength()
        {
            throw new NotImplementedException();
        }
        public string PostKeyLength()
        {
            throw new NotImplementedException();
        }

        //requests start state from server
        public string Initial()
        {
            throw new NotImplementedException();
        }

        //readys player if player is not host
        public string Ready()
        {
            //get initial update
            //throw new NotImplementedException();
        }

        //starts game
        public string Start()
        {
            throw new NotImplementedException();
        }
    }
}
