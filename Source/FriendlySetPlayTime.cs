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

        private static IPlayniteAPI s_playniteApi;

        public override Guid Id { get; } = Guid.Parse("84AAD786-7050-4558-8BF2-6A17C748FA26");

        public FriendlySetPlayTime(IPlayniteAPI api) : base(api)
        {
            // Use injected API instance.
            s_playniteApi = api;

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

        private static void OpenSetPlayTimeWindowForGame(Game selectedGame)
        {
            try
            {
                Window dialogWindow = s_playniteApi.Dialogs.CreateWindow(new WindowCreationOptions
                {
                    ShowMinimizeButton = false
                });

                dialogWindow.Title = "Friendly Set Play Time";
                dialogWindow.SizeToContent = SizeToContent.WidthAndHeight;
                dialogWindow.Content = new FriendlySetPlayTimeWindow(s_logger, s_playniteApi, selectedGame);

                dialogWindow.Owner = s_playniteApi.Dialogs.GetCurrentAppWindow();
                dialogWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                dialogWindow.ShowDialog();
            }
            catch (Exception e)
            {
                const string errorMessage = "Failed to open friendly set play time window for selected game.";
                const string errorCaption = "Friendly Set Play Time - Window Error";
                s_logger.Error(e, errorMessage);
                s_playniteApi.Dialogs.ShowErrorMessage(errorMessage + @"\n\nException: " + e.Message, errorCaption);
            }
        }
    }
}
