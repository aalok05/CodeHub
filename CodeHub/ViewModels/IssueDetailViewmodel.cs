using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.Services;
using CodeHub.Views;
using Octokit;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CodeHub.ViewModels
{
    public class IssueDetailViewmodel : AppViewmodel
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

        public Issue _issue;
        public Issue Issue
        {
            get
            {
                return _issue;
            }
            set
            {
                Set(() => Issue, ref _issue, value);

            }
        }

        public bool _CanEditIssue;
        public bool CanEditIssue
        {
            get
            {
                return _CanEditIssue; 
            }
            set
            {
                Set(() => CanEditIssue, ref _CanEditIssue, value);
            }
        }

        public bool _IsMyRepo;
        public bool IsMyRepo
        {
            get
            {
                return _IsMyRepo;
            }
            set
            {
                Set(() => IsMyRepo, ref _IsMyRepo, value);
            }
        }

        public ObservableCollection<IssueComment> _comments;
        public ObservableCollection<IssueComment> Comments
        {
            get
            {
                return _comments;
            }
            set
            {
                Set(() => Comments, ref _comments, value);

            }
        }

        public string _CommentText;
        public string CommentText
        {
            get
            {
                return _CommentText;
            }
            set
            {
                Set(() => CommentText, ref _CommentText, value);
            }
        }

        public string _NewIssueTitleText;
        public string NewIssueTitleText
        {
            get
            {
                return _NewIssueTitleText;
            }
            set
            {
                Set(() => NewIssueTitleText, ref _NewIssueTitleText, value);
            }
        }

        public string _NewIssueBodyText;
        public string NewIssueBodyText
        {
            get
            {
                return _NewIssueBodyText;
            }
            set
            {
                Set(() => NewIssueBodyText, ref _NewIssueBodyText, value);
            }
        }

        public bool _isEditingIssue;
        public bool IsEditingIssue
        {
            get
            {
                return _isEditingIssue;
            }
            set
            {
                Set(() => IsEditingIssue, ref _isEditingIssue, value);

            }
        }

        public ObservableCollection<Label> _AllLabels;
        /// <summary>
        /// All available labels in the repository
        /// </summary>
        public ObservableCollection<Label> AllLabels
        {
            get
            {
                return _AllLabels;
            }
            set
            {
                Set(() => AllLabels, ref _AllLabels, value);

            }
        }

        public async Task Load(object param)
        {
            if (param as Tuple<Repository, Issue> != null)
            {
                var tuple = param as Tuple<Repository, Issue>;
                Issue = tuple.Item2;
                Repository = tuple.Item1;
            }
            else
            {
                var tuple = (param as Tuple<string, string, Issue>);
                if(tuple != null)
                {
                    Issue = tuple.Item3;
                    Repository = await RepositoryUtility.GetRepository(tuple.Item1, tuple.Item2);
                }
            }

            if (!GlobalHelper.IsInternet())
            {
                //Sending NoInternet message to all viewModels
                Messenger.Default.Send(new GlobalHelper.LocalNotificationMessageType { Message="No Internet", Glyph= "\uE704" });
            }
            else
            {
                isLoading = true;
                Comments = await IssueUtility.GetAllCommentsForIssue(Repository.Id, Issue.Number);
                isLoading = false;

                if(Repository.Owner == null) 
                    Repository = await RepositoryUtility.GetRepository(Repository.Id);

                if (Repository.Owner.Login == GlobalHelper.UserLogin || Issue.User.Login == GlobalHelper.UserLogin)
                    CanEditIssue = true;
                if (Repository.Owner.Login == GlobalHelper.UserLogin)
                    IsMyRepo = true;
            }
        }

        public void CommentTapped(object sender, ItemClickEventArgs e)
        {
            SimpleIoc.Default.GetInstance<Services.IAsyncNavigationService>().NavigateAsync(typeof(CommentView), "Comment", e.ClickedItem as IssueComment);
        }
        private RelayCommand _userTapped;
        public RelayCommand UserTapped
        {
            get
            {
                return _userTapped
                    ?? (_userTapped = new RelayCommand(
                                          () =>
                                          {
                                              SimpleIoc.Default.GetInstance<Services.IAsyncNavigationService>().NavigateAsync(typeof(DeveloperProfileView), "Profile", Issue.User.Login);
                                          }));
            }
        }

        private RelayCommand _CommentCommand;
        public RelayCommand CommentCommand
        {
            get
            {
                return _CommentCommand
                    ?? (_CommentCommand = new RelayCommand(
                                          async () =>
                                          {
                                              if(!string.IsNullOrWhiteSpace(CommentText))
                                              {
                                                   isLoading = true;
                                                   IssueComment newComment = await IssueUtility.CommentOnIssue(Repository.Id, Issue.Number, CommentText);
                                                   isLoading = false;
                                                   if(newComment != null)
                                                   {
                                                      Comments.Add(newComment);
                                                      CommentText = string.Empty;
                                                   }

                                              }
                                          }));
            }
        }

        public async Task EditIssue()
        {
            IssueUpdate updatedIssue = new IssueUpdate();
            updatedIssue.Title = NewIssueTitleText;
            updatedIssue.Body = NewIssueBodyText;
            IsEditingIssue = true;
            Issue issue = await IssueUtility.EditIssue(Repository.Id, Issue.Number, updatedIssue);
            IsEditingIssue = false;
            if (issue != null)
            {
                Issue = issue;
            }
        }
    }
}
