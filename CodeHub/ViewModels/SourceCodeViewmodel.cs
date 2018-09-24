using CodeHub.Helpers;
using CodeHub.Models;
using CodeHub.Services;
using CodeHub.Views;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CodeHub.ViewModels
{
	public class SourceCodeViewmodel : AppViewmodel
	{
		public Repository _repository;
		public Repository Repository
		{
			get => _repository;
			set => Set(() => Repository, ref _repository, value);
		}

		public ObservableCollection<RepositoryContentWithCommitInfo> _content;
		public ObservableCollection<RepositoryContentWithCommitInfo> Content
		{
			get => _content;
			set => Set(() => Content, ref _content, value);
		}

		public string _selectedBranch;
		public string SelectedBranch
		{
			get => _selectedBranch;
			set => Set(() => SelectedBranch, ref _selectedBranch, value);
		}

		public ObservableCollection<string> _branches;
		public ObservableCollection<string> Branches
		{
			get => _branches;
			set => Set(() => Branches, ref _branches, value);
		}
		public async Task Load(Repository repo)
		{

			if (GlobalHelper.IsInternet())
			{
				IsLoading = true;
				if (repo != Repository)
				{
					Repository = repo;

					if (string.IsNullOrWhiteSpace(Repository.DefaultBranch))
					{
						SelectedBranch = await RepositoryUtility.GetDefaultBranch(Repository.Id);
					}
					else
					{
						SelectedBranch = Repository.DefaultBranch;
					}

					Branches = await RepositoryUtility.GetAllBranches(Repository);
					Content = await RepositoryUtility.GetRepositoryContent(Repository, SelectedBranch);
				}
				IsLoading = false;
			}

		}

		public void RepoContentDrillNavigate(object sender, ItemClickEventArgs e)
		{
			RepositoryContent item = e.ClickedItem as RepositoryContentWithCommitInfo;
			if (item.Type == ContentType.File)
			{
				SimpleIoc
					.Default
					.GetInstance<IAsyncNavigationService>()
					.NavigateWithoutAnimations(typeof(FileContentView), Repository.FullName, (Repository, item.Path, SelectedBranch));
			}
			else if (item.Type == ContentType.Dir)
			{
				SimpleIoc
					.Default
					.GetInstance<IAsyncNavigationService>()
					.NavigateWithoutAnimations(typeof(ContentView), Repository.FullName, (Repository, item.Path, SelectedBranch));
			}
		}
		public async void BranchChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count != 0)
			{
				if (!GlobalHelper.IsInternet())
				{
					//Sending NoInternet message to all viewModels
					Messenger.Default.Send(new GlobalHelper.NoInternet().SendMessage());
				}
				else
				{
					IsLoading = true;
					SelectedBranch = e.AddedItems.First().ToString();
					Content = await RepositoryUtility.GetRepositoryContent(Repository, SelectedBranch);
					IsLoading = false;
				}
			}
		}

		private RelayCommand _repoDetailNavigateCommand;
		public RelayCommand RepoDetailNavigateCommand 
			=> _repoDetailNavigateCommand
			?? (_repoDetailNavigateCommand = new RelayCommand(() =>
													 {
														 SimpleIoc
															.Default
															.GetInstance<IAsyncNavigationService>()
															.NavigateAsync(typeof(RepoDetailView), Repository);
													 }));
	}
}


