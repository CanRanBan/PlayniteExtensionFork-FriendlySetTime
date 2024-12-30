using System;
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
        }

        private void CompletionStatusComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CompletionStatusCheckBox.IsChecked = true;
        }

        private void LastActivityRadioButtonToday_OnClick(object sender, RoutedEventArgs e)
        {
            LastActivityCheckBox.IsChecked = true;

            _enhancedGameData.LastActivity = DateTime.Today.Date;
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
