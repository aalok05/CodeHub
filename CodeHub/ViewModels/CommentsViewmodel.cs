using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.Views;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeHub.ViewModels
{
    public class CommentsViewmodel : AppViewmodel
    {
        public IssueComment _comment;
        public IssueComment Comment
        {
            get
            {
                return _comment;
            }
            set
            {
                Set(() => Comment, ref _comment, value);

            }
        }

        public void Load(IssueComment comment)
        {
            Comment = comment;

            if (!GlobalHelper.IsInternet())
            {
                Messenger.Default.Send(new GlobalHelper.NoInternetMessageType()); //Sending NoInternet message to all viewModels
            }
            else
            {
                Messenger.Default.Send(new GlobalHelper.HasInternetMessageType()); //Sending Internet available message to all viewModels
            }
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
                                              SimpleIoc.Default.GetInstance<Services.IAsyncNavigationService>().NavigateAsync(typeof(DeveloperProfileView), "Profile", Comment.User.Login);
                                          }));
            }
        }
    }
}
