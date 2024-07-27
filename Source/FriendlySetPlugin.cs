using System;
using System.Collections.Generic;
using System.Windows;
using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;

namespace FriendlySetPlayTime
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FriendlySetPlugin : GenericPlugin
    {
        private static readonly ILogger s_logger = LogManager.GetLogger();

        public override Guid Id { get; } = Guid.Parse("84AAD786-7050-4558-8BF2-6A17C748FA26");

        public FriendlySetPlugin(IPlayniteAPI api) : base(api)
        {
            Properties = new GenericPluginProperties
            {
                HasSettings = false
            };
        }

        public override IEnumerable<GameMenuItem> GetGameMenuItems(GetGameMenuItemsArgs args)
        {
            List<GameMenuItem> gameMenuItems = new List<GameMenuItem>();
            if (args.Games.Count == 1)
            {
                gameMenuItems.Add(new GameMenuItem
                {
                    Description = "Set Playtime",
                    Action = (GameMenuItem) =>
                    {
                        DoSetTime(args.Games[0]);
                    }
                });
            }
            return gameMenuItems;
        }

        private void DoSetTime(Game game)
        {
            try
            {
                SetTimeWindow view = new SetTimeWindow(this, game);
                var window = PlayniteApi.Dialogs.CreateWindow(new WindowCreationOptions
                {
                    ShowMinimizeButton = false
                });

                window.SizeToContent = SizeToContent.WidthAndHeight;
                window.Title = "Set Time";
                window.Content = view;

                window.Owner = PlayniteApi.Dialogs.GetCurrentAppWindow();
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                window.ShowDialog();
            }
            catch (Exception E)
            {
                s_logger.Error(E, "Error during creatin set time window");
                PlayniteApi.Dialogs.ShowErrorMessage(E.Message, "Error during set time");
            }
        }
    }
}
