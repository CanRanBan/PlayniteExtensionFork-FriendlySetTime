using Playnite.SDK.Models;

namespace FriendlySetPlayTime
{
    internal class EnhancedGameData
    {
        internal ulong Days { get; set; }
        internal ulong Hours { get; set; }
        internal ulong Minutes { get; set; }
        internal ulong Seconds { get; set; }

        public EnhancedGameData(Game selectedGame)
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
    }
}
