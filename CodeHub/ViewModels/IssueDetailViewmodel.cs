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

        public async Task Load(Tuple<Repository, Issue> tuple)
        {
            Issue = tuple.Item2;
            Repository = tuple.Item1;

            if (!GlobalHelper.IsInternet())
            {
                //Sending NoInternet message to all viewModels
                Messenger.Default.Send(new GlobalHelper.LocalNotificationMessageType { Message="No Internet", Glyph= "\uE704" });
            }
            else
            {
                isLoading = true;
                Comments = await IssueUtility.GetAllCommentsForIssue(Repository.Owner.Login, Repository.Name, Issue.Number);
                isLoading = false;

            }
        }

        public void CommentTapped(object sender, ItemClickEventArgs e)
        {
            SimpleIoc.Default.GetInstance<Services.IAsyncNavigationService>().NavigateAsync(typeof(CommentsView), "Comments", e.ClickedItem as IssueComment);
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
    }
}
