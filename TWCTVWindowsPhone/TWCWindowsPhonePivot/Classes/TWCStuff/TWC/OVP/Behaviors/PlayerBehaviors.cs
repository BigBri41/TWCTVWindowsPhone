namespace TWC.OVP.Behaviors
{
    using Microsoft.SilverlightMediaFramework.Core;
    using System;
    using System.Windows;

    public class PlayerBehaviors
    {
        public static readonly DependencyProperty PlayerCommandProperty = DependencyProperty.RegisterAttached("PlayerCommand", typeof(PlayerCommand), typeof(PlayerBehaviors), new PropertyMetadata(new PropertyChangedCallback(PlayerBehaviors.OnPlayerCommandChanged)));

        public static PlayerCommand GetPlayerCommand(SMFPlayer player)
        {
            return (PlayerCommand) player.GetValue(PlayerCommandProperty);
        }

        private static void OnPlayerCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SMFPlayer player = (SMFPlayer) d;
            switch (((PlayerCommand) e.NewValue))
            {
                case PlayerCommand.Play:
                    player.Play();
                    return;

                case PlayerCommand.Pause:
                    player.Pause();
                    return;

                case PlayerCommand.Stop:
                    player.Stop();
                    return;

                case PlayerCommand.Replay:
                    player.Replay();
                    break;
            }
        }

        public static void SetPlayerCommand(SMFPlayer player, PlayerCommand value)
        {
            player.SetValue(PlayerCommandProperty, value);
        }
    }
}

