using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LinqToTwitter;
using System.Web;

namespace Plugin_UrlTitle
{    
    class TwitterHandler
    {
        private ApiKeys _apiKeys;
        private TwitterContext _twitterContext;

        public TwitterHandler()
        {
            if (File.Exists(ApiKeys.FullPath))
            {
                _apiKeys = ApiKeys.Load();
            }
            else
            {
                _apiKeys = new ApiKeys();
                _apiKeys.Save();
            }

            Authorize();
            
        }

        private async void Authorize()
        {
            var auth = new ApplicationOnlyAuthorizer()
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = _apiKeys.TwitterConsumerKey,
                    ConsumerSecret = _apiKeys.TwitterConsumerSecretKey
                },
            };

            await auth.AuthorizeAsync();

            _twitterContext = new TwitterContext(auth);
        }

        public string GetTweet(string tweetId)
        {
            try
            {
                var status =
                    (from tweet in _twitterContext.Status
                        where tweet.Type == StatusType.Lookup &&
                              tweet.TweetIDs == tweetId
                        select tweet).First();

                var checkmark = "";
                if (status.User.Verified)
                {
                    checkmark = "✓";
                }

                return $"@{status.User.ScreenNameResponse}{checkmark}: {status.Text}";
            }
            catch (Exception e)
            {
                return $"Could not display this tweet: {tweetId}.";
            }
                        
        }
    }
}
