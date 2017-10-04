using CodeHub.Services;
using GalaSoft.MvvmLight.Ioc;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CodeHub.ViewModels
{
    public class CommitsViewmodel : AppViewmodel
    {

        public ObservableCollection<GitHubCommit> _Commits;
        public ObservableCollection<GitHubCommit> Commits
        {
            get
            {
                return _Commits;
            }
            set
            {
                Set(() => Commits, ref _Commits, value);
            }

        }

        public async Task Load(object param)
        {
            isLoading = true;
            Commits = new ObservableCollection<GitHubCommit>();
            if (param as Tuple<long, IReadOnlyList<Commit>> != null)
            {
                var tuple = param as Tuple<long, IReadOnlyList<Commit>>;

                foreach (var commit in tuple.Item2)
                {
                    var githubCommit = await CommitService.GetCommit(tuple.Item1, commit.Sha);
                    Commits.Add(githubCommit);
                }
            }
            isLoading = false;
        }
        public void CommitList_ItemClick(object sender, ItemClickEventArgs e)
        {
            SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(CommitDetailView), e.ClickedItem as GitHubCommit);
        }
    }
}
