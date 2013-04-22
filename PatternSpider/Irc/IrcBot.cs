using System;
using System.Threading;
using IrcDotNet;
using PatternSpider.Utility;

namespace PatternSpider.Irc
{
    class IrcBot : IDisposable
    {
        private const int ClientQuitTimeout = 1000;

        // Internal and exposable collection of all clients that communicate individually with servers.
        private IrcClient _ircClient;

        // True if the read loop is currently active, false if ready to terminate.
        private bool _isRunning;
        private bool _isDisposed;
        

        public IrcBot()
        {
            _isRunning = false;
        }

        ~IrcBot()
        {
            Dispose(false);
        }

        public virtual string QuitMessage
        {
            get { return null; }
        }

        public IrcRegistrationInfo RegistrationInfo
        {
            get
            {
                return new IrcUserRegistrationInfo
                           {
                    NickName = "DevSpider",
                    UserName = "DevSpider",
                    RealName = "DevSpider"
                };
            }
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
            // Read commands from stdin until bot terminates.
            _isRunning = true;
            while (_isRunning)
            {

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

            Console.Out.WriteLine("Disconnected from '{0}'.", server);
        }


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
            }
        }

        #endregion

    }    
}
