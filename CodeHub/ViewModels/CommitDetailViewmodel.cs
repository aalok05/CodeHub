using CodeHub.Services;
using CodeHub.Views;
using GalaSoft.MvvmLight.Command;
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
    public class CommitDetailViewmodel : AppViewmodel
    {
        public Repository _repository;
        public Repository Repository
        {
            get
            {
                return _repository;
            }
            set
            {
                Set(() => Repository, ref _repository, value);

            }
        }

        public GitHubCommit _Commit;
        public GitHubCommit Commit
        {
            get
            {
                return _Commit;
            }
            set
            {
                Set(() => Commit, ref _Commit, value);

            }
        }
        public ObservableCollection<GitHubCommitFile> _Files;
        public ObservableCollection<GitHubCommitFile> Files
        {
            get
            {
                return _Files;
            }
            set
            {
                Set(() => Files, ref _Files, value);

            }
        }

        public async Task Load(object param)
        {
            isLoading = true;
            if(param as Tuple<long,string> != null)
            {
                var tuple = param as Tuple<long, string>;
                Commit = await CommitService.GetCommit(tuple.Item1, tuple.Item2);
                Files = new ObservableCollection<GitHubCommitFile>(Commit.Files);
            }
            else
            {
                Commit = param as GitHubCommit;
                Files = new ObservableCollection<GitHubCommitFile>(Commit.Files);
            }
            isLoading = false;
        }
    }
}
