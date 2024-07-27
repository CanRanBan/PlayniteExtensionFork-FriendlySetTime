using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Playnite.SDK;
using Playnite.SDK.Models;

namespace FriendlySetPlayTime
{
    public partial class FriendlySetPlayTimeWindow : UserControl
    {
        private readonly ILogger _logger;
        private readonly IPlayniteAPI _playniteApi;

        private readonly Game _selectedGame;

        public string Hours { get; set; }
        public string Minutes { get; set; }
        public string Seconds { get; set; }

        public List<string> statuses { get; set; } = new List<string>();

        public FriendlySetPlayTimeWindow(ILogger logger, IPlayniteAPI playniteAPI, Game selectedGame)
        {
            InitializeComponent();

            DataContext = this;

            _logger = logger;
            _playniteApi = playniteAPI;

            _selectedGame = selectedGame;

            ulong curseconds = selectedGame.Playtime;
            Seconds = (curseconds % 60).ToString();
            ulong bigMinutes = curseconds / 60;
            Minutes = (bigMinutes % 60).ToString();
            Hours = (bigMinutes / 60).ToString();

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

        private void StatusChanged(object sender, SelectionChangedEventArgs e)
        {
            // Detect if completion status wasn't set.
            string currentCompletionStatus = _selectedGame.CompletionStatus?.Name;
            string completionStatusNone = "";
            if (currentCompletionStatus != null)
            {
                if (currentCompletionStatus != newStatus.SelectedItem.ToString())
                {
                    updateStatus.IsChecked = true;
                }
            }
            else
            {
                if (completionStatusNone != newStatus.SelectedItem.ToString())
                {
                    updateStatus.IsChecked = true;
                }
            }
        }

        private void DidDateChange()
        {
            if (!(_selectedGame.LastActivity.HasValue && newDate.SelectedDate.HasValue &&
                newDate.SelectedDate.Value.Date.Equals(_selectedGame.LastActivity.Value.Date)))
            {
                setDate.IsChecked = true;
            }
        }

        private void SetToday_Checked(object sender, RoutedEventArgs e)
        {
            newDate.SelectedDate = DateTime.Today;
            DidDateChange();
        }

        private void NewDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DidDateChange();
            if (!(newDate.SelectedDate.HasValue && newDate.SelectedDate.Value.Date.Equals(DateTime.Today.Date)))
            {
                setToday.IsChecked = false;
            }
        }
    }
}
