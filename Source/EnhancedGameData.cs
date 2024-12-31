using System;
using System.Collections.Generic;
using Playnite.SDK;
using Playnite.SDK.Models;

namespace FriendlySetPlayTime
{
    internal class EnhancedGameData
    {
        private readonly ILogger _logger;
        private readonly IPlayniteAPI _playniteAPI;
        private readonly Game _selectedGame;

        // Play Time
        public ulong Days { get; set; }
        public ulong Hours { get; set; }
        public ulong Minutes { get; set; }
        public ulong Seconds { get; set; }

        // Completion Status
        public string CompletionStatus { get; set; }
        public List<string> CompletionStatusList { get; set; }
        internal Dictionary<string, Guid> CompletionStatusDictionary { get; set; }

        // Last Activity
        internal DateTime LastActivity { get; set; }
        internal string DateLongAndTimeLong { get; set; }
        internal string DateLongAndTimeShort { get; set; }
        internal string DateShortAndTimeLong { get; set; }
        internal string DateShortAndTimeShort { get; set; }
        internal string DateLong { get; set; }
        internal string DateShort { get; set; }
        internal string TimeLong { get; set; }
        internal string TimeShort { get; set; }

        public EnhancedGameData(ILogger logger, IPlayniteAPI playniteAPI, Game selectedGame)
        {
            _logger = logger;
            _playniteAPI = playniteAPI;
            _selectedGame = selectedGame;

            EnhancePlayTime(selectedGame);
            EnhanceCompletionStatus(selectedGame);
            EnhanceLastActivity(selectedGame);
        }

        private void EnhancePlayTime(Game selectedGame)
        {
            // Play Time value is in seconds potentially including minutes, hours, days.
            ulong secondsAndMore = selectedGame.Playtime;

            if (secondsAndMore < 60)
            {
                Seconds = secondsAndMore;
            }
            else
            {
                // Remainder of modulo using 60 = seconds. Possible values = 0-59.
                Seconds = secondsAndMore % 60;

                // Quotient of division using 60 = minutes potentially including hours and days.
                ulong minutesAndMore = secondsAndMore / 60;

                if (minutesAndMore < 60)
                {
                    Minutes = minutesAndMore;
                }
                else
                {
                    // Remainder of modulo using 60 = minutes. Possible values = 0-59. 
                    Minutes = minutesAndMore % 60;

                    // Quotient of division using 60 = hours potentially including days.
                    ulong hoursAndMore = minutesAndMore / 60;

                    if (hoursAndMore < 24)
                    {
                        Hours = hoursAndMore;
                    }
                    else
                    {
                        // Remainder of modulo using 24 = hours. Possible values = 0-23.
                        Hours = hoursAndMore % 24;

                        // Quotient of division using 24 = days.
                        Days = hoursAndMore / 24;
                    }
                }
            }
        }

        public ulong SimplifyPlayTime()
        {
            return SimplifyPlayTime(Days, Hours, Minutes, Seconds);
        }

        private static ulong SimplifyPlayTime(ulong days, ulong hours, ulong minutes, ulong seconds)
        {
            ulong playTime = 0;

            if (days > 0)
            {
                playTime += days * 24 * 60 * 60;
            }

            if (hours > 0)
            {
                playTime += hours * 60 * 60;
            }

            if (minutes > 0)
            {
                playTime += minutes * 60;
            }

            if (seconds > 0)
            {
                playTime += seconds;
            }

            return playTime;
        }

        private void EnhanceCompletionStatus(Game selectedGame)
        {
            // Create list of all available completion status.
            var emptyCompletionStatus = Playnite.SDK.Models.CompletionStatus.Empty;
            CompletionStatusList.Add(emptyCompletionStatus.Name);
            CompletionStatusDictionary.Add(emptyCompletionStatus.Name, emptyCompletionStatus.Id);
            foreach (CompletionStatus completionStatus in _playniteAPI.Database.CompletionStatuses)
            {
                CompletionStatusList.Add(completionStatus.Name);
                CompletionStatusDictionary.Add(completionStatus.Name, completionStatus.Id);
            }

            // Use completion status none if unset.
            CompletionStatus = selectedGame.CompletionStatus?.Name ?? emptyCompletionStatus.Name;
        }

        public Guid SimplifyCompletionStatus()
        {
            return SimplifyCompletionStatus(CompletionStatus);
        }

        private Guid SimplifyCompletionStatus(string completionStatus)
        {
            Guid completionStatusId = Guid.Empty;

            if (!string.IsNullOrEmpty(completionStatus))
            {
                if (CompletionStatusDictionary.ContainsKey(completionStatus))
                {
                    CompletionStatusDictionary.TryGetValue(completionStatus, out completionStatusId);
                }
            }

            return completionStatusId;
        }

        private void EnhanceLastActivity(Game selectedGame)
        {
            if (!selectedGame.LastActivity.HasValue)
            {
                return;
            }

            LastActivity = selectedGame.LastActivity.Value;

            DateLongAndTimeLong = LastActivity.ToString("F");
            DateLongAndTimeShort = LastActivity.ToString("f");
            DateShortAndTimeLong = LastActivity.ToString("G");
            DateShortAndTimeShort = LastActivity.ToString("g");
            DateLong = LastActivity.ToString("D");
            DateShort = LastActivity.ToString("d");
            TimeLong = LastActivity.ToString("T");
            TimeShort = LastActivity.ToString("t");
        }
    }
}
