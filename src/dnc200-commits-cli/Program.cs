using System;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;

namespace dnc200_commits_cli
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string commitType;
            string repositoryName;
            string userName;
            string uri = "https://api.github.com/";

            Console.WriteLine("Welcome to Commits-CLI");
            Console.WriteLine("Type 'repo' if you are searching a repository, or 'user' if you are searching a user:");
            commitType = Console.ReadLine();
            switch (commitType)
            {
                case "repo":
                    Console.Write("The repository's name is: ");
                    repositoryName = Console.ReadLine();
                    int y = 0;
                    bool x = int.TryParse(repositoryName, out y);
                    if (x)
                    {
                        Console.WriteLine("please enter valid name");
                    }
                    Console.Write("The repository's owner is: ");
                    userName = Console.ReadLine();
                    uri += $"repos/{userName}/{repositoryName}/stats/commit_activity";
                    Console.WriteLine(GetCommits(userName, GetFromURI(uri)));
                    break;
                case "user":
                    Console.Write("The user's name is: ");
                    userName = Console.ReadLine();
                    uri += $"users/{userName}/events/public";
                    Console.WriteLine(GetCommits(userName, GetFromURI(uri)));
                    break;
                default:
                    Console.WriteLine("Not a valid option, please try again.");
                    break;
            }
        }


        public static int GetCommits(string userName)
        {
            int x = 0;
            bool isInt = int.TryParse(userName, out x);
            if (isInt)
            {
                return x;
            }
            if (!isInt)
            {
                return 0;
            }
            return 0;
        }

        public static string GetCommits(string user, string data)
        {
            int x = 0;
            bool isData = int.TryParse(data, out x);
            List<dynamic> response = JsonConvert.DeserializeObject<List<dynamic>>(data);
            IEnumerable<dynamic> pushes = response.Where(e => e.type == "PushEvent");
            int commitCount = pushes.Aggregate(0, (total, push) => total + push.payload.commits.Count);
            return $"There have been {commitCount} public commits by {user} in the past 90 days";
        }

        public static string GetCommits(string user, string repo, string data)
        {
            List<dynamic> response = JsonConvert.DeserializeObject<List<dynamic>>(data);
            int commitCount = response.Take(11).Aggregate(0, (sum, week) => sum + (int)week.total);
            return $"There have been {commitCount} commits in {user}'s {repo} in the past 91 days";
        }

        public static string GetFromURI(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)NewMethod(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Headers.Add("User-Agent", "request");

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private static WebRequest NewMethod(string uri)
        {
            return WebRequest.Create(uri);
        }
    }
}
