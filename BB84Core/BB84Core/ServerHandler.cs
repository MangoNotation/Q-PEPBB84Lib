using System;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Text;

namespace BB84Core
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
            string updateURL = URL + "/update";
            WR = WebRequest.Create(updateURL);
            ObjStream = WR.GetResponse().GetResponseStream();
            Reader = new StreamReader(ObjStream);
            return Reader.ReadLine();
        }

        //posts update from server and returns update
        public string PostUpdate(string update)
        {
            using (var wb = new WebClient())
            {
                NameValueCollection data = new NameValueCollection();
                data["udpate"] = update;
                var response = wb.UploadValues(URL + "/statuschange", "POST", data);
                return Encoding.UTF8.GetString(response);
            }
        }

        //requests player from server
        public string RequestPlayer(Player player)
        {
            string PURL = "";
            switch (player)
            {
                case Player.Alice:
                    PURL = URL + "/playalice";
                    break;
                case Player.Bob:
                    PURL = URL + "/playbob";
                    break;
                case Player.Eve:
                    PURL = URL + "/playeve";
                    break;
            }
            WR = WebRequest.Create(PURL);
            ObjStream = WR.GetResponse().GetResponseStream();
            Reader = new StreamReader(ObjStream);

            return Reader.ReadLine();
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
        public string Ready(Player player)
        {
            using(var wb = new WebClient())
            {
                NameValueCollection data = new NameValueCollection();

                switch (player)
                {
                    case Player.Alice:
                        data["player"] = "a";
                        break;
                    case Player.Bob:
                        data["player"] = "b";
                        break;
                    case Player.Eve:
                        data["player"] = "c";
                        break;
                }

                var response = wb.UploadValues(URL + "/ready", "POST", data);
                return Encoding.UTF8.GetString(response);
            }
        }

        //starts game
        public string Start()
        {
            throw new NotImplementedException();
        }
    }
}
