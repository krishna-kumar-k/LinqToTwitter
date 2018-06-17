using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToTwitter;
using System.Collections.Specialized;


namespace MyTweetTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("working on it....");
            var tweetList = GetTwitterFeeds();
            Console.WriteLine("Tweets Count " + tweetList.Count);
            var file = new System.IO.StreamWriter("C:\\TestTweet\\TweetsList.txt", true); // Make sure to change the path according to your system  
            foreach (var item in tweetList)
            {
                file.WriteLine(item.Text);
            }
            file.Close();
            Console.WriteLine("Done! check your drive file has been created");
            Console.ReadLine();
        }
        public static List<Status> GetTwitterFeeds()
        {
            string screenname = "csharpcorner";

            var auth = new SingleUserAuthorizer
            {

                CredentialStore = new InMemoryCredentialStore()
                {

                    ConsumerKey = ConfigurationManager.AppSettings["consumerkey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["consumersecret"],
                    OAuthToken = ConfigurationManager.AppSettings["accessToken"],
                    OAuthTokenSecret = ConfigurationManager.AppSettings["accessTokenSecret"]

                }

            };
            var twitterCtx = new TwitterContext(auth);
            var ownTweets = new List<Status>();

            ulong maxId = 0;
            bool flag = true;
            var statusResponse = new List<Status>();
            statusResponse = (from tweet in twitterCtx.Status
                              where tweet.Type == StatusType.User
                              && tweet.ScreenName == screenname
                              && tweet.Count == 200
                              select tweet).ToList();

            if (statusResponse.Count > 0)
            {
                maxId = ulong.Parse(statusResponse.Last().StatusID.ToString()) - 1;
                ownTweets.AddRange(statusResponse);

            }
            do
            {
                int rateLimitStatus = twitterCtx.RateLimitRemaining;
                if (rateLimitStatus != 0)
                {

                    statusResponse = (from tweet in twitterCtx.Status
                                      where tweet.Type == StatusType.User
                                      && tweet.ScreenName == screenname
                                      && tweet.MaxID == maxId
                                      && tweet.Count == 200
                                      select tweet).ToList();

                    if (statusResponse.Count != 0)
                    {
                        maxId = ulong.Parse(statusResponse.Last().StatusID.ToString()) - 1;
                        ownTweets.AddRange(statusResponse);
                    }
                    else
                    {
                        flag = false;
                    }
                }
                else
                {
                    flag = false;
                }
            } while (flag);

            return ownTweets;
        }

    }
}
