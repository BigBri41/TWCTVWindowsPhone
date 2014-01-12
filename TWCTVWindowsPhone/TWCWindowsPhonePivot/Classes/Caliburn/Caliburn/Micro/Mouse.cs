namespace Caliburn.Micro
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;

    public static class Mouse
    {
        public static void Initialize(UIElement element)
        {
            element.MouseMove += (s, e) => (Position = e.GetPosition(null));
        }

        public static Point Position
        {
            [CompilerGenerated]
            get
            {
                return <Position>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                <Position>k__BackingField = value;
            }
        }
    }
}

