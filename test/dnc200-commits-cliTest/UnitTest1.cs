using System;
using Xunit;
using dnc200_commits_cli;

namespace dnc200_commits_cliTest
{
    public class UnitTest1
    {
        [Fact]
        public void GetCommits_User()
        {
            string userName = "";
            int expectedCommits = 0;
            int actualCommits = Program.GetCommits(userName);
            Assert.Equal(expectedCommits, actualCommits);
        }

        [Fact]
        public void GetCommits_Repo()
        {
            string repoName = "";
            string userName = "";
            int expectedCommits = 0;
            int actualCommits = Program.GetCommits(userName, repoName);
            Assert.Equal(expectedCommits, actualCommits);
        }
    }
}
