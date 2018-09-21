using CodeHub.Helpers;
using CodeHub.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CodeHub.ViewModels
{
	public class EditProfileViewmodel : AppViewmodel
	{
		#region properties

		public UserUpdate _userUpdate;
		public UserUpdate UserUpdate
		{
			get => _userUpdate;
			set => Set(() => UserUpdate, ref _userUpdate, value);
		}

		public string _selectedPublicEmail;
		public string SelectedPublicEmail
		{
			get => _selectedPublicEmail;
			set => Set(() => SelectedPublicEmail, ref _selectedPublicEmail, value);
		}

		public ObservableCollection<string> _emailAddresses;
		public ObservableCollection<string> EmailAddresses
		{
			get => _emailAddresses;
			set => Set(() => EmailAddresses, ref _emailAddresses, value);
		}

		#endregion

		public async Task Load(object user)
		{
			if (user is User developer)
			{
				await PopulateUserProperties(developer);
			}
		}

		private RelayCommand _updateProfileCommand;
		public RelayCommand UpdateProfileCommand
			=> _updateProfileCommand
			?? (_updateProfileCommand = new RelayCommand(async () =>
												 {
													 IsLoading = true;
													 UserUpdate.Email = SelectedPublicEmail;
													 var updatedUser = await UserService.UpdateUserProfile(UserUpdate);
													 IsLoading = false;

													 if (updatedUser != null)
													 {
														 // Updated successfully
														 await PopulateUserProperties(updatedUser);

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


		private RelayCommand _resetFormCommand;
		public RelayCommand ResetFormCommand 
			=> _resetFormCommand
			?? (_resetFormCommand = new RelayCommand(async () =>
												{
													IsLoading = true;
													var user = await UserService.GetUserInfo(GlobalHelper.UserLogin);
													await PopulateUserProperties(user);
													IsLoading = false;

												}));

		private async Task PopulateUserProperties(User developer)
		{
			IsLoading = true;
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

			// Populate the Emails dropdown
			var emails = await UserService.GetVerifiedEmails();
			EmailAddresses = new ObservableCollection<string>();
			if (emails != null)
			{
				foreach (var email in emails)
				{
					if (email.Verified)
					{
						EmailAddresses.Add(email.Email);
					}
				}
			}

			// Set selected public email in emails dropdown
			SelectedPublicEmail = developer.Email;

			IsLoading = false;
		}
	}
}
