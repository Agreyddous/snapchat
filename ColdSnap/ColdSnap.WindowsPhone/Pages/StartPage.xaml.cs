﻿using System;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using ColdSnap.Common;
using ColdSnap.Dialogs;
using ColdSnap.ViewModels;

namespace ColdSnap.Pages
{
	public sealed partial class StartPage
	{
		private readonly LogInDialog _logInDialog;

		public StartPage()
		{
			InitializeComponent();
			DataContext = ViewModel = new StartPageViewModel();

			// Make the status bar overlay our app.
			ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);

			// Lock rotation to portrait mode.
			DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

			// Set up login page.
			_logInDialog = new LogInDialog();
			_logInDialog.PrimaryButtonClick += LogInDialog_PrimaryButtonClick;

			// Show login page by default.
			Loaded += async delegate { await _logInDialog.ShowAsync(); };

			NavigationHelper = new NavigationHelper(this);
			NavigationHelper.LoadState += NavigationHelper_LoadState;
			NavigationHelper.SaveState += NavigationHelper_SaveState;
		}

		/// <summary>
		/// Gets the view model for this page.
		/// </summary>
		public StartPageViewModel ViewModel { get; private set; }

		/// <summary>
		/// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
		/// </summary>
		public NavigationHelper NavigationHelper { get; private set; }
		
		#region NavigationHelper registration

		/// <summary>
		/// The methods provided in this section are simply used to allow
		/// NavigationHelper to respond to the page's navigation methods.
		/// <para>
		/// Page specific logic should be placed in event handlers for the  
		/// <see cref="NavigationHelper.LoadState"/>
		/// and <see cref="NavigationHelper.SaveState"/>.
		/// The navigation parameter is available in the LoadState method 
		/// in addition to page state preserved during an earlier session.
		/// </para>
		/// </summary>
		/// <param name="e">Provides data for navigation methods and event
		/// handlers that cannot cancel the navigation request.</param>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			NavigationHelper.OnNavigatedTo(e);
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			NavigationHelper.OnNavigatedFrom(e);
		}

		#endregion

		/// <summary>
		/// Populates the page with content passed during navigation.  Any saved state is also
		/// provided when recreating a page from a prior session.
		/// </summary>
		/// <param name="sender">
		/// The source of the event; typically <see cref="NavigationHelper"/>
		/// </param>
		/// <param name="e">Event data that provides both the navigation parameter passed to
		/// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
		/// a dictionary of state preserved by this page during an earlier
		/// session.  The state will be null the first time a page is visited.</param>
		private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
		{
			// Clear backstack (we shouldn't be able to go back to any other page after logging out).
			(Window.Current.Content as Frame).BackStack.Clear();
		}

		/// <summary>
		/// Preserves state associated with this page in case the application is suspended or the
		/// page is discarded from the navigation cache.  Values must conform to the serialization
		/// requirements of <see cref="SuspensionManager.SessionState"/>.
		/// </summary>
		/// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
		/// <param name="e">Event data that provides an empty dictionary to be populated with
		/// serializable state.</param>
		private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
		{
		}

		private async void LogInButton_Tapped(object sender, TappedRoutedEventArgs e)
		{
			await _logInDialog.ShowAsync();
		}

		private async void LogInDialog_PrimaryButtonClick(ContentDialog dialog, ContentDialogButtonClickEventArgs args)
		{
			var deferral = args.GetDeferral();
			dialog.IsEnabled = dialog.IsPrimaryButtonEnabled = dialog.IsSecondaryButtonEnabled = false;

			await ViewModel.LogInAsync(_logInDialog.Username, _logInDialog.Password);
			_logInDialog.Password = String.Empty; // clear the password field in case of a failed login

			dialog.IsEnabled = dialog.IsPrimaryButtonEnabled = dialog.IsSecondaryButtonEnabled = true;
			deferral.Complete();
		}
	}
}
