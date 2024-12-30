using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Playnite.SDK;
using Playnite.SDK.Models;

namespace FriendlySetPlayTime
{
    public partial class FriendlySetPlayTimeWindow
    {
        private readonly ILogger _logger;
        private readonly IPlayniteAPI _playniteAPI;
        private readonly Game _selectedGame;

        private EnhancedGameData _enhancedGameData;

        public FriendlySetPlayTimeWindow(ILogger logger, IPlayniteAPI playniteAPI, Game selectedGame)
        {
            InitializeComponent();

            DataContext = this;

            _logger = logger;
            _playniteAPI = playniteAPI;
            _selectedGame = selectedGame;

            _enhancedGameData = LoadCurrentGameData(_logger, _playniteAPI, _selectedGame);
        }

        private EnhancedGameData LoadCurrentGameData(ILogger logger, IPlayniteAPI playniteAPI, Game selectedGame)
        {
            return _enhancedGameData = new EnhancedGameData(logger, playniteAPI, selectedGame);

            string completionStatusNone = "";
            statuses.Add(completionStatusNone);
            foreach (CompletionStatus completionStatus in _playniteApi.Database.CompletionStatuses)
            {
                statuses.Add(completionStatus.Name);
            }
            newDate.SelectedDate = _selectedGame.LastActivity;

            // Use completion status none if it's not set.
            string currentCompletionStatus = _selectedGame.CompletionStatus?.Name;
            if (currentCompletionStatus != null)
            {
                newStatus.SelectedIndex = statuses.IndexOf(currentCompletionStatus);
            }
            else
            {
                newStatus.SelectedIndex = statuses.IndexOf(completionStatusNone);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                uint mins = UInt32.Parse(minutes.Text.Trim());
                uint hrs = UInt32.Parse(hours.Text.Trim());
                ulong scnds = UInt64.Parse(seconds.Text.Trim());
                scnds += mins * 60;
                scnds += hrs * 3600;
                _selectedGame.Playtime = scnds;
                if ((bool)updateStatus.IsChecked)
                {
                    string status = newStatus.SelectedItem.ToString();
                    if (status != "")
                    {
                        _selectedGame.CompletionStatusId = _playniteApi.Database.CompletionStatuses.Where(x => x.Name == status).DefaultIfEmpty(_selectedGame.CompletionStatus).First().Id;
                    }
                    else
                    {
                        _selectedGame.CompletionStatusId = Guid.Empty;
                    }
                }
                if ((bool)setDate.IsChecked)
                {
                    _selectedGame.LastActivity = newDate.SelectedDate;
                }
                _playniteApi.Database.Games.Update(_selectedGame);
                ((Window)Parent).Close();
            }
            catch (Exception E)
            {
                _logger.Error(E, "Error when parsing time");
                _playniteApi.Dialogs.ShowErrorMessage(E.Message, "Error when parsing time");
            }
        }

        private void CompletionStatusComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CompletionStatusCheckBox.IsChecked = true;
        }

        private void LastActivityRadioButtonToday_OnClick(object sender, RoutedEventArgs e)
        {
            LastActivityCheckBox.IsChecked = true;

            _enhancedGameData.LastActivity = DateTime.Today;
        }

        private void LastActivityRadioButtonDatePicker_OnClick(object sender, RoutedEventArgs e)
        {
            LastActivityCheckBox.IsChecked = true;
        }

        private void LastActivityDatePicker_OnSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LastActivityDatePicker.SelectedDate.HasValue)
            {
                LastActivityCheckBox.IsChecked = true;

                _enhancedGameData.LastActivity = LastActivityDatePicker.SelectedDate.Value;
            }
        }
    }
}
