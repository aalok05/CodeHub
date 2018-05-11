using CodeHub.Helpers;
using CodeHub.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeHub.ViewModels
{
    public class EditProfileViewmodel : AppViewmodel
    {
        #region properties

        public UserUpdate _userUpdate;
        public UserUpdate UserUpdate
        {
            get
            {
                return _userUpdate;
            }
            set
            {
                Set(() => UserUpdate, ref _userUpdate, value);
            }
        }
       
        #endregion

        public void Load(object user)
        {
            if (user is User developer)
            {
                PopulateUserProperties(developer);
            }
        }

        private RelayCommand _updateProfileCommand;
        public RelayCommand UpdateProfileCommand
        {
            get
            {
                return _updateProfileCommand
                    ?? (_updateProfileCommand = new RelayCommand(
                                          async () =>
                                          {
                                              isLoading = true;
                                              var updatedUser = await UserService.UpdateUserProfile(UserUpdate);
                                              isLoading = false;

                                              if(updatedUser != null)
                                              {
                                                  // Updated successfully
                                                  PopulateUserProperties(updatedUser);

                                                  Messenger.Default.Send(new GlobalHelper.LocalNotificationMessageType
                                                  {
                                                      Message = "Profile updated",
                                                      Glyph = "\uE081"
                                                  });
                                              }
                                              else
                                              {
                                                  // Something went wrong

                                                  Messenger.Default.Send(new GlobalHelper.LocalNotificationMessageType
                                                  {
                                                      Message = "Something went wrong",
                                                      Glyph = "\uE783"
                                                  });
                                              }

                                          }));
            }
        }

        private void PopulateUserProperties(User developer)
        {
            UserUpdate = new UserUpdate
            {
                Bio = developer.Bio,
                Blog = developer.Blog,
                Company = developer.Company,
                Email = developer.Email,
                Hireable = (developer.Hireable == null || developer.Hireable == false) ? false : true,
                Location = developer.Location,
                Name = developer.Name
            };
        }
    }
}
