using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PatternSpider.Utility;

namespace PatternSpider.Irc
{
    public class IrcBot : IDisposable
    {  
        private bool _isRunning; // True if the read loop is currently active, false if ready to terminate.        
        private bool _isDisposed;

        private string _server;
        private List<string> _channelsToJoin;

      
        public string QuitMessage { get; set; }
        public string Server { get { return _server; }}
        
        public List<String> Channels 
        { 
            get { throw new NotImplementedException(); }
        }

        public string Nickname { get { throw new NotImplementedException(); } }

        public IrcBot()
        {
            _isRunning = false;
            _channelsToJoin = new List<string>();
        
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
            

            // Wait until connection has succeeded or timed out.
            
           
        }

        public void Disconnect()
        {
            // Disconnect IRC client that is connected to given server.
           
            // Remove client from connection.
        }

        public void SendMessage(string target, string message) //Sends a message to either a channel or user 
        {
            try
            {
                
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
           
        }

        public void Part(string channel)
        {
           
        }

        public void Part(List<string> channels)
        {
            
        }

        #region Exposed Event

        public delegate void ChannelMessage();
        public event ChannelMessage OnChannelMessage;

        public delegate void UserMessage();
        public event UserMessage OnUserMessage;


        #endregion         
    }    
}
