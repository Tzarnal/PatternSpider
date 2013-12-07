using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using IrcDotNet;
using PatternSpider.Utility;

namespace PatternSpider.Irc
{
    public class IrcBot : IDisposable
    {
        private const int ClientQuitTimeout = 1000;        
        private IrcClient _ircClient;

        private bool _isRunning; // True if the read loop is currently active, false if ready to terminate.        
        private bool _isDisposed;

        private string _server;
        private List<string> _channelsToJoin;
        private IrcRegistrationInfo _registrationInfo;
        private DateTime _lastPing;
        private readonly TimeSpan _pingInterval = new TimeSpan(0, 2, 0);

        public string QuitMessage { get; set; }
        public string Server { get { return _server; }}
        public List<String> Channels 
        { 
            get
            {
                return _ircClient.Channels.Select(channel => channel.ToString()).ToList();
            }
        }

        public string Nickname { get { return _ircClient.LocalUser.NickName; } }

        public IrcBot()
        {
            _isRunning = false;
            _channelsToJoin = new List<string>();
            _lastPing = DateTime.Now;
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
                        _ircClient.Quit(ClientQuitTimeout, QuitMessage);
                        _ircClient.Dispose();
                    }
                }
            }
            _isDisposed = true;
        }

        public void Run()
        {
            var newThread = new Thread(ThreadRun);
            newThread.Start();
        }

        private void ThreadRun()
        {
            _isRunning = true;
            while (_isRunning)
            {                
                if (_ircClient == null) continue;
                
                //Check if we are conneted, if not and we have connection information, give connecting a shot.
                if (!_ircClient.IsConnected && Server != null && _registrationInfo != null)
                {
                    Connect(Server, _registrationInfo);
                }

                //If we are connected but not registered wait 30 seconds for the server, if we still don't register, disconnect.
                if (_ircClient.IsConnected && !_ircClient.IsRegistered)
                {
                    Thread.Sleep(30000);
                    if (!_ircClient.IsRegistered)
                    {
                        _ircClient.Disconnect();
                    }
                }

                //Throw out a ping every timeinterval to check connection.
                if (_ircClient.IsRegistered)
                {
                    if (DateTime.Now - _lastPing > _pingInterval)
                    {
                        _ircClient.Ping();
                        _lastPing = DateTime.Now;
                    }
                }                

                if (_ircClient.IsConnected && _ircClient.IsRegistered && !_ircClient.Channels.Any())
                {
                    Join(_channelsToJoin);
                }

                Thread.Sleep(5000);
            }

            Dispose();
        }

        public void Stop()
        {
            _isRunning = false;
        }

        public void Connect(string server, IrcRegistrationInfo registrationInfo)
        {
            // Create new IRC client and connect to given server.
            var client = new IrcClient {FloodPreventer = new IrcStandardFloodPreventer(4, 2000)};

            client.Connected += IrcClientConnected;
            client.Disconnected += IrcClientDisconnected;
            client.Registered += IrcClientRegistered;

            // Wait until connection has succeeded or timed out.
            using (var connectedEvent = new ManualResetEventSlim(false))
            {
                client.Connected += (sender2, e2) => { if (connectedEvent != null) connectedEvent.Set(); };
                client.Connect(server, false, registrationInfo);
                if (!connectedEvent.Wait(10000))
                {
                    client.Dispose();
                    ConsoleUtilities.WriteError("Connection to '{0}' timed out.", server);
                    return;
                }
            }

            if (_ircClient != null )
            {                
                if (_ircClient.IsConnected)
                {
                    client.Quit(ClientQuitTimeout, QuitMessage);    
                }
                
                _ircClient.Dispose();
            }
            
            _ircClient = client;
            _server = server;
            _registrationInfo = registrationInfo;

            Console.Out.WriteLine("Now connected to '{0}'.", server);
        }

        public void Disconnect()
        {
            // Disconnect IRC client that is connected to given server.
            var server = _ircClient.ServerName;
            
            var client = _ircClient;
            client.Quit(ClientQuitTimeout, QuitMessage);            
            client.Dispose();

            // Remove client from connection.
            _ircClient = new IrcClient();
            _server = null;

            Console.Out.WriteLine("Disconnected from '{0}'.", server);
        }

        public void SendMessage(IIrcMessageTarget target, string message) //Sends a message to either a channel or user depending on target
        {
            try
            {
                _ircClient.LocalUser.SendMessage(target, message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}, \n Message {1}", e.Message,message);
            }
            
        }

        public void SendQuery(string user, string message) //sends a query to a user without having a IrcMessageTarget for them
        {
            try
            {
                var target = _ircClient.Users.FirstOrDefault(ircUser => ircUser.NickName == user);
                if (target != null)
                {
                    SendMessage(target, message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}, \n Message {1}", e.Message,message);
            }
        }

        public void SendChannelMessage(string channel, string message) //sends a message to a channel without having a IrcMessageTarget for it
        {
            var target = _ircClient.Channels.FirstOrDefault(ircChannel => ircChannel.Name == channel);
            if (target != null)
            {
                SendMessage(target, message);
            }
        }

        public void Join(string channel)
        {
            Join(new List<string>{channel});
        }

        public void Join(List<string> channels)
        {
            foreach (var channel in channels.Where(channel => !_channelsToJoin.Contains(channel)))
            {
                _channelsToJoin.Add(channel);
            }

            var joinedchannels = Channels;

            if (_ircClient.IsRegistered)
            {
                foreach (var channel in _channelsToJoin.Where(channel => !joinedchannels.Contains(channel)))
                {
                    _ircClient.Channels.Join(channel);
                }
            }
        }

        public void Part(string channel)
        {
            Part(new List<string>{channel});
        }

        public void Part(List<string> channels)
        {
            foreach (var channel in channels)
            {
                _ircClient.Channels.Leave(channel);
                _channelsToJoin.Remove(channel);
            }
        }

        #region Exposed Event

        public delegate void ChannelMessage(object source, IrcBot ircBot, IrcMessageEventArgs e);
        public event ChannelMessage OnChannelMessage;

        public delegate void UserMessage(object source, IrcBot ircBot, IrcMessageEventArgs e);
        public event UserMessage OnUserMessage;


        #endregion 

        #region IRC Client Event Handlers

        private void IrcClientConnected(object sender, EventArgs e)
        {
        }

        private void IrcClientDisconnected(object sender, EventArgs e)
        {
            var client = (IrcClient)sender;

        }

        private void IrcClientRegistered(object sender, EventArgs e)
        {
            var client = (IrcClient)sender;

            client.LocalUser.NoticeReceived += IrcClientLocalUserNoticeReceived;
            client.LocalUser.MessageReceived += IrcClientLocalUserMessageReceived;
            client.LocalUser.JoinedChannel += IrcClientLocalUserJoinedChannel;
            client.LocalUser.LeftChannel += IrcClientLocalUserLeftChannel;

            Join(_channelsToJoin);
        }

        private void IrcClientLocalUserNoticeReceived(object sender, IrcMessageEventArgs e)
        {
            var localUser = (IrcLocalUser)sender;
        }

        private void IrcClientLocalUserMessageReceived(object sender, IrcMessageEventArgs e)
        {
            var localUser = (IrcLocalUser)sender;

            if (e.Source is IrcUser)
            {
                if (OnUserMessage != null)
                {
                    ThreadStart threadStart = () => OnUserMessage(sender, this, e);
                    new Thread(threadStart).Start();
                }                
            }
        }

        private void IrcClientLocalUserJoinedChannel(object sender, IrcChannelEventArgs e)
        {
            var localUser = (IrcLocalUser)sender;

            e.Channel.UserJoined += IrcClientChannelUserJoined;
            e.Channel.UserLeft += IrcClientChannelUserLeft;
            e.Channel.MessageReceived += IrcClientChannelMessageReceived;
            e.Channel.NoticeReceived += IrcClientChannelNoticeReceived; 
        }

        private void IrcClientLocalUserLeftChannel(object sender, IrcChannelEventArgs e)
        {
            var localUser = (IrcLocalUser)sender;

            e.Channel.UserJoined -= IrcClientChannelUserJoined;
            e.Channel.UserLeft -= IrcClientChannelUserLeft;
            e.Channel.MessageReceived -= IrcClientChannelMessageReceived;
            e.Channel.NoticeReceived -= IrcClientChannelNoticeReceived;
        }

        private void IrcClientChannelUserLeft(object sender, IrcChannelUserEventArgs e)
        {
            var channel = (IrcChannel)sender;
        }

        private void IrcClientChannelUserJoined(object sender, IrcChannelUserEventArgs e)
        {
            var channel = (IrcChannel)sender;
        }

        private void IrcClientChannelNoticeReceived(object sender, IrcMessageEventArgs e)
        {
            var channel = (IrcChannel)sender;
        }

        private void IrcClientChannelMessageReceived(object sender, IrcMessageEventArgs e)
        {
            var channel = (IrcChannel)sender;
            
            if (e.Source is IrcUser)
            {                
                if (OnChannelMessage != null)
                {   
                    ThreadStart threadStart = () => OnChannelMessage(sender, this, e);
                    new Thread(threadStart).Start();                            
                }                
            }
        }

        #endregion
    }    
}
