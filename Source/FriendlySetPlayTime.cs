using System;
using System.Collections.Generic;
using System.Windows;
using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;

namespace FriendlySetPlayTime
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FriendlySetPlayTime : GenericPlugin
    {
        private static readonly ILogger s_logger = LogManager.GetLogger();

        public override Guid Id { get; } = Guid.Parse("84AAD786-7050-4558-8BF2-6A17C748FA26");

        public FriendlySetPlayTime(IPlayniteAPI api) : base(api)
        {
            Properties = new GenericPluginProperties
            {
                HasSettings = false
            };
        }

        public override IEnumerable<GameMenuItem> GetGameMenuItems(GetGameMenuItemsArgs args)
        {
            if (args.Games.Count == 1)
            {
                yield return new GameMenuItem
                {
                    Description = "Set Play Time",
                    // Note: Template with same name for args and actionArgs causes a compiler error. Propagation of values properly works regardless of different names.
                    Action = actionArgs =>
                    {
                        OpenSetPlayTimeWindowForGame(actionArgs.Games[0]);
                    }
                };
            }
        }

        private void OpenSetPlayTimeWindowForGame(Game game)
        {
            try
            {
                SetTimeWindow view = new SetTimeWindow(this, game);
                Window window = PlayniteApi.Dialogs.CreateWindow(new WindowCreationOptions
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
