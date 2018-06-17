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
            var tweetList = GetTweets(200);
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
        public static List<Status> GetTweets(int max)
        {
            const int MaxTweetsToReturn = 200;
            // get the auth
            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = ConfigurationManager.AppSettings["consumerkey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["consumersecret"],
                    OAuthToken = ConfigurationManager.AppSettings["accessToken"],
                    OAuthTokenSecret = ConfigurationManager.AppSettings["accessTokenSecret"]
                }
            };
            // get the context, query the last status
            var context = new TwitterContext(auth);
            var tweets =
                from tw in context.Status
                where
                    tw.Type == StatusType.User &&
                    tw.Count == MaxTweetsToReturn
                select tw;
            // handle exceptions, twitter service might be down
            
                return tweets.Take(max).ToList();            
        }

    }
}
