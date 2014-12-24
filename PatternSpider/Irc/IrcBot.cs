using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PatternSpider.Utility;
using Meebey.SmartIrc4net;

namespace PatternSpider.Irc
{
    public class IrcBot : IDisposable
    {  
        private bool _isRunning; // True if the read loop is currently active, false if ready to terminate.        
        private bool _isDisposed;

        private IrcClient _ircClient;
        private IrcUser _ircUser;
        private string _server;
        private List<string> _channelsToJoin;

      
        public string QuitMessage { get; set; }
        public string Server { get { return _server; }}
        
        public List<String> Channels 
        { 
            get { throw new NotImplementedException(); }
        }

        public string Nickname { get { throw new NotImplementedException(); } }

        public IrcBot(IrcUser ircUser)
        {
            _isRunning = false;
            _channelsToJoin = new List<string>();
            _ircUser = ircUser;
        }

        ~IrcBot()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    if (_ircClient != null)
                    {
                        Disconnect();
                    }
                }
            }
            _isDisposed = true;
        }

        public void Run()
        {
            var newThread = new Thread(ThreadRun);
            newThread.Start();
            
            _ircClient.Listen();
        }

        private void ThreadRun()
        {
            _isRunning = true;
            while (_isRunning)
            {                                
                Console.Write(".");
                Thread.Sleep(5000);
            }

            Dispose();
        }

        public void Stop()
        {
            _isRunning = false;
        }

        public void Connect(string server)
        {
            // Create new IRC client and connect to given server.
            var ircClient = new IrcClient {AutoReconnect = true};

            ircClient.OnChannelMessage += IrcClientOnOnChannelMessage;
            ircClient.OnQueryMessage += IrcClientOnOnQueryMessage;

            try
            {
                ircClient.Connect(server, 6669);
            }
            catch
            {
                Console.WriteLine("Error: Could not connect to {0}.\n", server);
                return;
            }

            try
            {
                //Log in
                if (!string.IsNullOrEmpty(_ircUser.NickServPassword))
                {
                    ircClient.Login(_ircUser.Nickname, _ircUser.Realname,4,_ircUser.Username,_ircUser.NickServPassword);
                }
                else
                {
                    ircClient.Login(_ircUser.Nickname, _ircUser.Realname);    
                }
            }
            catch
            {
                Console.WriteLine("Error: Could not Login to {0} with {1}.\n", server, _ircUser.Nickname);
                return;
            }
            
                                    
            //Finish up
            
            _server = server;
            _ircClient = ircClient;


        }



        public void Disconnect()
        {
            // Disconnect IRC client that is connected to given server.
           _ircClient.Disconnect();

            // Remove client from connection.
            _ircClient = null;
        }

        public void SendMessage(string target, string message) //Sends a message to either a channel or user 
        {
            try
            {
                _ircClient.SendMessage(SendType.Message, target, message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}, \n Message {1}", e.Message,message);
            }
            
        }

        public void SendQuery(string user, string message) //sends a query to a user
        {
            try
            {
                _ircClient.SendMessage(SendType.Message, user, message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}, \n Message {1}", e.Message,message);
            }
        }


        public void Join(string channel)
        {
            Join(new List<string>{channel});
        }

        public void Join(List<string> channels)
        {            
            _ircClient.RfcJoin(channels.ToArray());
        }

        public void Part(string channel)
        {
           Part(new List<string>{channel});
        }

        public void Part(List<string> channels)
        {
            _ircClient.RfcPart(channels.ToArray());
        }


        #region Event Handlers

        private void IrcClientOnOnChannelMessage(object sender, IrcEventArgs ircEventArgs)
        {
            var m = new IrcMessage
                {
                    Channel = ircEventArgs.Data.Channel,
                    Sender = ircEventArgs.Data.Nick,
                    Server = _server,
                    Text = ircEventArgs.Data.Message,
                    TextArray = ircEventArgs.Data.MessageArray
                };
            
            if (OnChannelMessage != null)
            {
                ThreadStart threadStart = () => OnChannelMessage(sender, this, m);
                new Thread(threadStart).Start();
            } 
        }

        private void IrcClientOnOnQueryMessage(object sender, IrcEventArgs ircEventArgs)
        {
            var m = new IrcMessage
            {
                Sender = ircEventArgs.Data.Nick,
                Server = _server,
                Text = ircEventArgs.Data.Message,
                TextArray = ircEventArgs.Data.MessageArray
            };

            if (OnChannelMessage != null)
            {
                ThreadStart threadStart = () => OnUserMessage(sender, this, m);
                new Thread(threadStart).Start();
            } 
        }

        #endregion

        #region Exposed Event

        public delegate void ChannelMessage(object source, IrcBot ircBot, IrcMessage e);
        public event ChannelMessage OnChannelMessage;

        public delegate void UserMessage(object source, IrcBot ircBot, IrcMessage e);
        public event UserMessage OnUserMessage;

        #endregion         
    }    
}
