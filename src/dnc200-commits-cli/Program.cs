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
                    Console.Write("The repository's owner is: ");
                    userName = Console.ReadLine();
                    uri += $"repos/{userName}/{repositoryName}/stats/commit_activity";
                    Console.WriteLine(GetCommits(userName, repositoryName, GetFromURI(uri)));
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

        // Makes a reqest to the Github api and extracts the amount of commits made
        public static string GetCommits(string user, string data)
        {
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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Headers.Add("User-Agent", "request");

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
