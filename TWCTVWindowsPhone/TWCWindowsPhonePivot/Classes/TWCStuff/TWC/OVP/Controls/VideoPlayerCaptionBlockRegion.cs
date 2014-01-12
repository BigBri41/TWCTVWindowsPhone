namespace TWC.OVP.Controls
{
    using Microsoft.SilverlightMediaFramework.Core.Accessibility.Captions;
    using Microsoft.SilverlightMediaFramework.Utilities.Extensions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using TWC.OVP.Framework.Models;

    public class VideoPlayerCaptionBlockRegion : UserControl
    {
        private IDictionary<CaptionElement, UIElement> _activeElements;
        private MediaMarkerManager<TimedTextElement> _captionManager;
        private bool _contentLoaded;
        private TimeSpan _mediaPosition;
        private const int BRUSH_CACHE_CAPACITY = 50;
        private static Dictionary<Color, CachedBrush> cachedBrushes = new Dictionary<Color, CachedBrush>();
        public static readonly DependencyProperty CaptionRegionProperty = DependencyProperty.Register("CaptionRegion", typeof(Microsoft.SilverlightMediaFramework.Core.Accessibility.Captions.CaptionRegion), typeof(VideoPlayerCaptionBlockRegion), new PropertyMetadata(new PropertyChangedCallback(VideoPlayerCaptionBlockRegion.OnCaptionRegionPropertyChanged)));
        internal Border CaptionsBorder;
        public static readonly DependencyProperty CaptionsOverrideSettingsProperty = DependencyProperty.Register("CaptionsOverrideSettings", typeof(TWC.OVP.Framework.Models.CaptionsOverrideSettings), typeof(VideoPlayerCaptionBlockRegion), new PropertyMetadata(new PropertyChangedCallback(VideoPlayerCaptionBlockRegion.OnCaptionsOverrideSettingsPropertyChanged)));
        internal StackPanel captionsPanel;
        internal Grid CaptionsRoot;
        private const long DefaultMaximumCaptionSeekSearchWindowMillis = 0xea60L;
        internal Grid LayoutRoot;
        private const int MaxOverflow = 0x438;
        public static readonly DependencyProperty VisibleCaptionTextProperty = DependencyProperty.Register("VisibleCaptionText", typeof(string), typeof(VideoPlayerCaptionBlockRegion), new PropertyMetadata(null));

        public VideoPlayerCaptionBlockRegion()
        {
            System.Action<MediaMarkerManager<TimedTextElement>, TimedTextElement> action = null;
            System.Action<MediaMarkerManager<TimedTextElement>, TimedTextElement, bool> action2 = null;
            RoutedEventHandler handler = null;
            SizeChangedEventHandler handler2 = null;
            this._activeElements = new Dictionary<CaptionElement, UIElement>();
            this._captionManager = new MediaMarkerManager<TimedTextElement>();
            this._captionManager.SeekingSearchWindow = new TimeSpan?(TimeSpan.FromMilliseconds(60000.0));
            if (action == null)
            {
                action = (s, c) => this.HideCaption(c);
            }
            this._captionManager.MarkerLeft += action;
            if (action2 == null)
            {
                action2 = (s, c, f) => this.ShowCaption(c);
            }
            this._captionManager.MarkerReached += action2;
            if (handler == null)
            {
                handler = (s, e) => this.RedrawActiveCaptions();
            }
            base.Loaded += handler;
            this.InitializeComponent();
            if (handler2 == null)
            {
                handler2 = (s, e) => this.UpdateSize();
            }
            base.SizeChanged += handler2;
        }

        private void ApplyCaptionOverrideSettings(CaptionElement caption)
        {
            if (this.CaptionsOverrideSettings != null)
            {
                this.ApplyTextOverrides(caption);
                this.ApplyWindowOverrides(caption);
                foreach (TimedTextElement element in caption.Children)
                {
                    CaptionElement element2 = element as CaptionElement;
                    if (element2 != null)
                    {
                        this.ApplyCaptionOverrideSettings(element2);
                    }
                }
            }
        }

        private Grid ApplyDepressedEdge(TimedTextStyle style, double panelWidth, double height, string text, TextBlock textblock)
        {
            return this.ApplyEdge(style, panelWidth, height, text, textblock, 1.0, 1.0, 1.0);
        }

        private Grid ApplyDropShadowEdge(TimedTextStyle style, double panelWidth, double height, string text, TextBlock textblock)
        {
            Grid grid = new Grid();
            TextBlock block = this.GetStyledTextblock(style, panelWidth, height, true);
            this.SetContent(block, text);
            grid.Children.Add(block);
            double num = block.FontSize * 0.06;
            TranslateTransform transform = new TranslateTransform {
                X = num,
                Y = num
            };
            block.RenderTransform = transform;
            grid.Children.Add(textblock);
            return grid;
        }

        private Grid ApplyEdge(TimedTextStyle style, double panelWidth, double height, string text, TextBlock textblock, double size, double offsetX, double offsetY)
        {
            Grid grid = new Grid();
            TextBlock block = this.GetStyledTextblock(style, panelWidth, height, true);
            this.SetContent(block, text);
            grid.Children.Add(block);
            TranslateTransform transform = new TranslateTransform {
                X = -size + offsetX,
                Y = -size + offsetY
            };
            block.RenderTransform = transform;
            block = this.GetStyledTextblock(style, panelWidth, height, true);
            this.SetContent(block, text);
            grid.Children.Add(block);
            TranslateTransform transform2 = new TranslateTransform {
                X = size + offsetX,
                Y = size + offsetY
            };
            block.RenderTransform = transform2;
            block = this.GetStyledTextblock(style, panelWidth, height, true);
            this.SetContent(block, text);
            grid.Children.Add(block);
            TranslateTransform transform3 = new TranslateTransform {
                X = size + offsetX,
                Y = -size + offsetY
            };
            block.RenderTransform = transform3;
            block = this.GetStyledTextblock(style, panelWidth, height, true);
            this.SetContent(block, text);
            grid.Children.Add(block);
            TranslateTransform transform4 = new TranslateTransform {
                X = -size + offsetX,
                Y = size + offsetY
            };
            block.RenderTransform = transform4;
            grid.Children.Add(textblock);
            return grid;
        }

        private Grid ApplyRaisedEdge(TimedTextStyle style, double panelWidth, double height, string text, TextBlock textblock)
        {
            return this.ApplyEdge(style, panelWidth, height, text, textblock, 1.0, -1.0, -1.0);
        }

        private void ApplyRegionStyles()
        {
            Size effectiveSize = this.GetEffectiveSize();
            double num = this.CaptionRegion.CurrentStyle.FontSize.ToPixelLength(effectiveSize.Height);
            this.SetZIndex(this.CaptionRegion.CurrentStyle.ZIndex);
            base.FontSize = (num > 0.0) ? num : base.FontSize;
            base.FontFamily = this.CaptionRegion.CurrentStyle.FontFamily;
            base.Foreground = GetCachedBrush(this.CaptionRegion.CurrentStyle.Color);
            this.CaptionsBorder.Background = GetCachedBrush(this.CaptionRegion.CurrentStyle.BackgroundColor);
            this.CaptionsBorder.Padding = this.CaptionRegion.CurrentStyle.Padding.ToThickness(new Size?(effectiveSize));
            this.LayoutRoot.Visibility = (this.CaptionRegion.CurrentStyle.Display == Visibility.Collapsed) ? Visibility.Collapsed : this.CaptionRegion.CurrentStyle.Visibility;
            switch (this.CaptionRegion.CurrentStyle.DisplayAlign)
            {
                case DisplayAlign.Before:
                    this.captionsPanel.VerticalAlignment = VerticalAlignment.Top;
                    break;

                case DisplayAlign.Center:
                    this.captionsPanel.VerticalAlignment = VerticalAlignment.Center;
                    break;

                case DisplayAlign.After:
                    this.captionsPanel.VerticalAlignment = VerticalAlignment.Bottom;
                    break;
            }
            if (this.CaptionRegion.CurrentStyle.Overflow != Overflow.Visible)
            {
                this.captionsPanel.Margin = new Thickness();
            }
            else
            {
                switch (this.CaptionRegion.CurrentStyle.DisplayAlign)
                {
                    case DisplayAlign.Before:
                        this.captionsPanel.Margin = new Thickness(0.0, 0.0, 0.0, -1080.0);
                        break;

                    case DisplayAlign.Center:
                        this.captionsPanel.Margin = new Thickness(0.0, -1080.0, 0.0, -1080.0);
                        break;

                    case DisplayAlign.After:
                        this.captionsPanel.Margin = new Thickness(0.0, -1080.0, 0.0, 0.0);
                        break;
                }
                switch (this.CaptionRegion.CurrentStyle.TextAlign)
                {
                    case TextAlignment.Center:
                        this.captionsPanel.Margin = new Thickness(-1080.0, this.captionsPanel.Margin.Top, -1080.0, this.captionsPanel.Margin.Bottom);
                        return;

                    case TextAlignment.Right:
                        this.captionsPanel.Margin = new Thickness(-1080.0, this.captionsPanel.Margin.Top, 0.0, this.captionsPanel.Margin.Bottom);
                        return;
                }
                this.captionsPanel.Margin = new Thickness(0.0, this.captionsPanel.Margin.Top, -1080.0, this.captionsPanel.Margin.Bottom);
            }
        }

        private void ApplyTextOverrides(CaptionElement caption)
        {
            if (caption.CaptionElementType == TimedTextElementType.Text)
            {
                if (this.CaptionsOverrideSettings.CharacterColor != CaptionsOverrideColors.Default)
                {
                    caption.CurrentStyle.Color = this.CaptionsOverrideSettings.CharacterColor.ToColor();
                }
                if (this.CaptionsOverrideSettings.CharacterOpacity != CaptionsOverrideOpacities.Default)
                {
                    caption.CurrentStyle.Opacity = this.CaptionsOverrideSettings.CharacterOpacity.ToDouble();
                }
                if (this.CaptionsOverrideSettings.CharacterBackgroundColor != CaptionsOverrideColors.Default)
                {
                    Color color = this.CaptionsOverrideSettings.CharacterBackgroundColor.ToColor();
                    color.A = caption.CurrentStyle.BackgroundColor.A;
                    caption.CurrentStyle.BackgroundColor = color;
                }
                if (this.CaptionsOverrideSettings.CharacterBackgroundOpacity != CaptionsOverrideOpacities.Default)
                {
                    Color backgroundColor = caption.CurrentStyle.BackgroundColor;
                    backgroundColor.A = (byte) (this.CaptionsOverrideSettings.CharacterBackgroundOpacity.ToDouble() * 255.0);
                    caption.CurrentStyle.BackgroundColor = backgroundColor;
                }
                if (this.CaptionsOverrideSettings.CharacterFont != CaptionsOverrideFonts.Default)
                {
                    caption.CurrentStyle.FontFamily = this.CaptionsOverrideSettings.CharacterFont.ToFontFamily();
                }
            }
        }

        private Grid ApplyUniformEdge(TimedTextStyle style, double panelWidth, double height, string text, TextBlock textblock)
        {
            return this.ApplyEdge(style, panelWidth, height, text, textblock, 1.1, 0.0, 0.0);
        }

        private void ApplyWindowOverrides(CaptionElement caption)
        {
            if (caption.CaptionElementType == TimedTextElementType.Container)
            {
                if (this.CaptionsOverrideSettings.WindowColor != CaptionsOverrideColors.Default)
                {
                    Color color = this.CaptionsOverrideSettings.WindowColor.ToColor();
                    color.A = caption.CurrentStyle.BackgroundColor.A;
                    caption.CurrentStyle.BackgroundColor = color;
                }
                if (this.CaptionsOverrideSettings.WindowOpacity != CaptionsOverrideOpacities.Default)
                {
                    Color backgroundColor = caption.CurrentStyle.BackgroundColor;
                    backgroundColor.A = (byte) (this.CaptionsOverrideSettings.WindowOpacity.ToDouble() * 255.0);
                    caption.CurrentStyle.BackgroundColor = backgroundColor;
                }
            }
        }

        private static Brush GetCachedBrush(Color src)
        {
            if (cachedBrushes.ContainsKey(src))
            {
                CachedBrush brush = cachedBrushes[src];
                brush.LastUse = DateTime.Now;
                return brush.Brush;
            }
            Brush brush2 = new SolidColorBrush(src);
            if (cachedBrushes.Count >= 50)
            {
                KeyValuePair<Color, CachedBrush> pair = (from b in cachedBrushes
                    orderby b.Value.LastUse
                    select b).First<KeyValuePair<Color, CachedBrush>>();
                cachedBrushes.Remove(pair.Key);
            }
            cachedBrushes.Add(src, new CachedBrush(brush2));
            return brush2;
        }

        private TextBlock GetStyledTextblock(TimedTextStyle style, double width, double height, bool fOutline)
        {
            TextBlock block = new TextBlock {
                FontStyle = style.FontStyle,
                FontWeight = style.FontWeight,
                VerticalAlignment = VerticalAlignment.Bottom,
                FontFamily = style.FontFamily
            };
            if (!double.IsNaN(height) && (height != 0.0))
            {
                if ((this.CaptionsOverrideSettings != null) && (this.CaptionsOverrideSettings.CharacterSize != CaptionsOverrideSizes.pct100))
                {
                    Length length = new Length {
                        Unit = style.FontSize.Unit,
                        Value = style.FontSize.Value * (((double) this.CaptionsOverrideSettings.CharacterSize.ToPercentage()) / 100.0)
                    };
                    block.FontSize = Math.Round(length.ToPixelLength(height));
                }
                else
                {
                    block.FontSize = Math.Round(style.FontSize.ToPixelLength(height));
                }
            }
            block.Foreground = GetCachedBrush(fOutline ? style.OutlineColor : style.Color);
            block.TextAlignment = style.TextAlign;
            return block;
        }

        private void GetTextRecursive(CaptionElement captionElement, ref string text)
        {
            if (captionElement.IsActiveAtPosition(this._mediaPosition) && (captionElement.CurrentStyle.Display == Visibility.Visible))
            {
                if (captionElement.CaptionElementType == TimedTextElementType.Container)
                {
                    foreach (CaptionElement element in captionElement.Children.Cast<CaptionElement>())
                    {
                        this.GetTextRecursive(element, ref text);
                    }
                }
                else if (captionElement.CaptionElementType == TimedTextElementType.Text)
                {
                    text = text + captionElement.Content + Environment.NewLine;
                }
            }
        }

        private void HideCaption(TimedTextElement timedTextElement)
        {
            CaptionElement key = timedTextElement as CaptionElement;
            if ((key != null) && this._activeElements.ContainsKey(key))
            {
                this.captionsPanel.Children.Remove(this._activeElements[key]);
                this._activeElements.Remove(key);
            }
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Application.LoadComponent(this, new Uri("/TWC.OVP;component/Controls/VideoPlayer/VideoPlayerCaptionBlockRegion.xaml", UriKind.Relative));
                this.LayoutRoot = (Grid) base.FindName("LayoutRoot");
                this.CaptionsBorder = (Border) base.FindName("CaptionsBorder");
                this.CaptionsRoot = (Grid) base.FindName("CaptionsRoot");
                this.captionsPanel = (StackPanel) base.FindName("captionsPanel");
            }
        }

        private StackPanel NewPanel(StackPanel parent, ref double offset, CaptionElement element, TextAlignment align)
        {
            StackPanel panel = new StackPanel {
                Orientation = Orientation.Horizontal
            };
            switch (align)
            {
                case TextAlignment.Center:
                    panel.HorizontalAlignment = HorizontalAlignment.Center;
                    break;

                case TextAlignment.Left:
                    panel.HorizontalAlignment = HorizontalAlignment.Left;
                    break;

                case TextAlignment.Right:
                    panel.HorizontalAlignment = HorizontalAlignment.Right;
                    break;

                case TextAlignment.Justify:
                    panel.HorizontalAlignment = HorizontalAlignment.Stretch;
                    break;
            }
            parent.Children.Add(panel);
            offset = 0.0;
            return panel;
        }

        private void OnCaptionRegionChanged()
        {
            this.ApplyRegionStyles();
        }

        private static void OnCaptionRegionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as VideoPlayerCaptionBlockRegion).IfNotNull<VideoPlayerCaptionBlockRegion>(i => i.OnCaptionRegionChanged());
        }

        private void OnCaptionsOverrideSettingsChanged()
        {
            this.RedrawActiveCaptions();
        }

        private static void OnCaptionsOverrideSettingsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as VideoPlayerCaptionBlockRegion).IfNotNull<VideoPlayerCaptionBlockRegion>(i => i.OnCaptionsOverrideSettingsChanged());
        }

        public void RedrawActiveCaptions()
        {
            List<CaptionElement> list = this._activeElements.Keys.ToList<CaptionElement>();
            list.ForEach(new Action<CaptionElement>(this.HideCaption));
            list.ForEach(new Action<CaptionElement>(this.ShowCaption));
        }

        private UIElement RenderElement(CaptionElement element)
        {
            StackPanel parent = null;
            try
            {
                parent = new StackPanel {
                    Background = GetCachedBrush(element.CurrentStyle.BackgroundColor),
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                TextAlignment textAlign = element.CurrentStyle.TextAlign;
                double offset = 0.0;
                StackPanel p = this.NewPanel(parent, ref offset, element, textAlign);
                this.RenderElementRecurse(parent, ref p, element, ref offset, textAlign);
            }
            catch (Exception)
            {
            }
            return parent;
        }

        private void RenderElementRecurse(StackPanel parent, ref StackPanel p, CaptionElement element, ref double offset, TextAlignment align)
        {
            if (element.IsActiveAtPosition(this._mediaPosition) && (element.CurrentStyle.Display == Visibility.Visible))
            {
                if (element.CaptionElementType == TimedTextElementType.Text)
                {
                    string text = (element.Content != null) ? element.Content.ToString() : string.Empty;
                    offset = this.WrapElement(parent, ref p, offset, text, element, align, false);
                }
                else if (element.CaptionElementType == TimedTextElementType.Container)
                {
                    foreach (CaptionElement element2 in element.Children)
                    {
                        this.RenderElementRecurse(parent, ref p, element2, ref offset, align);
                    }
                }
                else if (element.CaptionElementType == TimedTextElementType.LineBreak)
                {
                    p = this.NewPanel(parent, ref offset, element, align);
                }
            }
        }

        private void SetAllContent(FrameworkElement contentElement, string text)
        {
            if (contentElement is TextBlock)
            {
                this.SetContent((TextBlock) contentElement, text);
            }
            else if (contentElement is Panel)
            {
                foreach (TextBlock block in ((Panel) contentElement).Children.OfType<TextBlock>())
                {
                    this.SetContent(block, text);
                }
            }
        }

        private void SetContent(TextBlock textblock, string text)
        {
            textblock.Text = text;
            textblock.UpdateLayout();
        }

        private void SetZIndex(int zIndex)
        {
            this.GetVisualParent<ContentPresenter>().IfNotNull<ContentPresenter>(i => i.SetValue(Canvas.ZIndexProperty, zIndex));
        }

        private void ShowCaption(TimedTextElement timedTextElement)
        {
            Action<KeyValuePair<CaptionElement, UIElement>> action = null;
            CaptionElement caption = timedTextElement as CaptionElement;
            if (caption != null)
            {
                this.ApplyCaptionOverrideSettings(caption);
                caption.CalculateCurrentStyle(this._mediaPosition);
                UIElement element2 = this.RenderElement(caption);
                if (element2 != null)
                {
                    if (this._activeElements.ContainsKey(caption))
                    {
                        this.HideCaption(timedTextElement);
                    }
                    this._activeElements.Add(caption, element2);
                    this.captionsPanel.Children.Clear();
                    if (action == null)
                    {
                        action = i => this.captionsPanel.Children.Add(i.Value);
                    }
                    (from i in this._activeElements
                        orderby i.Key.Index
                        select i).ForEach<KeyValuePair<CaptionElement, UIElement>>(action);
                }
            }
        }

        public void UpdateCaptions(TimeSpan mediaPosition, bool isSeeking)
        {
            this._mediaPosition = mediaPosition;
            (from i in this.CaptionRegion.Children
                where i.End < mediaPosition
                select i).ToList<TimedTextElement>().ForEach((Action<TimedTextElement>) (i => this.CaptionRegion.Children.Remove(i)));
            this._captionManager.CheckMarkerPositions(mediaPosition, this.CaptionRegion.Children, isSeeking, false);
            (from i in this.CaptionRegion.Children.WhereActiveAtPosition(mediaPosition, null)
                where i.HasAnimations
                select i).ForEach<TimedTextElement>(i => i.CalculateCurrentStyle(mediaPosition)).ForEach<TimedTextElement>(new Action<TimedTextElement>(this.HideCaption)).ForEach<TimedTextElement>(new Action<TimedTextElement>(this.ShowCaption));
            this.UpdateVisibleCaptionTextProperty();
        }

        private void UpdateSize()
        {
            double effectiveWidth = this.GetEffectiveWidth();
            double effectiveHeight = this.GetEffectiveHeight();
            if ((!DesignerProperties.GetIsInDesignMode(this) && (effectiveWidth != 0.0)) && (effectiveHeight != 0.0))
            {
                Origin origin = this.CaptionRegion.CurrentStyle.Origin;
                Extent extent = this.CaptionRegion.CurrentStyle.Extent;
                double num3 = extent.Height.ToPixelLength(effectiveHeight);
                double num4 = extent.Width.ToPixelLength(effectiveWidth);
                this.CaptionsBorder.Width = (num4 < 0.0) ? effectiveWidth : num4;
                this.CaptionsBorder.Height = (num3 < 0.0) ? effectiveHeight : num3;
                this.CaptionsBorder.VerticalAlignment = (num3 < 0.0) ? VerticalAlignment.Bottom : VerticalAlignment.Top;
                Thickness thickness = new Thickness {
                    Left = origin.Left.ToPixelLength(effectiveWidth),
                    Top = origin.Top.ToPixelLength(effectiveHeight)
                };
                this.CaptionsBorder.Margin = thickness;
                this.ApplyRegionStyles();
                this.RedrawActiveCaptions();
            }
        }

        private void UpdateVisibleCaptionTextProperty()
        {
            string captionText = "";
            (from a in this._activeElements
                orderby a.Key.Index
                select a).ForEach<KeyValuePair<CaptionElement, UIElement>>(a => this.GetTextRecursive(a.Key, ref captionText));
            this.VisibleCaptionText = captionText;
        }

        private double WrapElement(StackPanel parent, ref StackPanel p, double offset, string text, CaptionElement element, TextAlignment align, bool directionApplied = false)
        {
            if ((text == null) || (text == ""))
            {
                return offset;
            }
            Size effectiveSize = this.GetEffectiveSize();
            TimedTextStyle currentStyle = element.CurrentStyle;
            Size size2 = currentStyle.Extent.ToPixelSize(new Size?(effectiveSize));
            double width = size2.Width;
            double height = size2.Height;
            if ((currentStyle.Direction == Direction.RightToLeft) && !directionApplied)
            {
                text = new string(text.ToCharArray().Reverse<char>().ToArray<char>());
            }
            double num3 = ((currentStyle.FontSize.Unit == LengthUnit.PixelProportional) || (currentStyle.FontSize.Unit == LengthUnit.Cell)) ? effectiveSize.Height : height;
            TextBlock textblock = this.GetStyledTextblock(currentStyle, width, num3, false);
            this.SetContent(textblock, text);
            Border border = new Border {
                Background = GetCachedBrush(currentStyle.BackgroundColor)
            };
            FrameworkElement contentElement = null;
            double num4 = currentStyle.OutlineWidth.ToPixelLength(effectiveSize.Height);
            if (this.CaptionsOverrideSettings == null)
            {
                return offset;
            }
            switch (this.CaptionsOverrideSettings.CharacterEdgeAttribute)
            {
                case CaptionsOverrideCharacterEdges.Default:
                    if (num4 <= 0.0)
                    {
                        contentElement = textblock;
                        break;
                    }
                    contentElement = this.ApplyUniformEdge(currentStyle, width, num3, text, textblock);
                    break;

                case CaptionsOverrideCharacterEdges.NoAttribute:
                    contentElement = textblock;
                    break;

                case CaptionsOverrideCharacterEdges.Raised:
                    contentElement = this.ApplyRaisedEdge(currentStyle, width, num3, text, textblock);
                    break;

                case CaptionsOverrideCharacterEdges.Depressed:
                    contentElement = this.ApplyDepressedEdge(currentStyle, width, num3, text, textblock);
                    break;

                case CaptionsOverrideCharacterEdges.Uniform:
                    contentElement = this.ApplyUniformEdge(currentStyle, width, num3, text, textblock);
                    break;

                case CaptionsOverrideCharacterEdges.DropShadow:
                    contentElement = this.ApplyDropShadowEdge(currentStyle, width, num3, text, textblock);
                    break;
            }
            contentElement.Opacity = (currentStyle.Visibility == Visibility.Visible) ? currentStyle.Opacity : 0.0;
            border.Child = contentElement;
            p.Children.Add(border);
            string str = text;
            string str2 = string.Empty;
            double effectiveWidth = textblock.GetEffectiveWidth();
            if (((offset + effectiveWidth) <= size2.Width) || (currentStyle.WrapOption != TextWrapping.Wrap))
            {
                offset += effectiveWidth;
                return offset;
            }
            if ((text.Length <= 0) || (text.IndexOf(' ') >= 0))
            {
                while ((offset + textblock.GetEffectiveWidth()) > size2.Width)
                {
                    int num7 = str.LastIndexOf(' ');
                    if (num7 < 0)
                    {
                        this.SetAllContent(contentElement, text);
                        return 0.0;
                    }
                    str2 = text.Substring(num7 + 1);
                    str = text.Substring(0, num7);
                    this.SetAllContent(contentElement, str);
                }
                p = this.NewPanel(parent, ref offset, element, align);
                return this.WrapElement(parent, ref p, offset, str2, element, align, true);
            }
            if ((offset != 0.0) && (effectiveWidth < size2.Width))
            {
                p.Children.Remove(border);
                p = this.NewPanel(parent, ref offset, element, align);
                return this.WrapElement(parent, ref p, 0.0, text, element, align, true);
            }
            int length = text.Length - 1;
            str = text.Substring(0, length);
            str2 = text.Substring(length);
            this.SetAllContent(contentElement, str);
            while ((offset + textblock.GetEffectiveWidth()) > size2.Width)
            {
                length--;
                str = text.Substring(0, length);
                str2 = text.Substring(length);
                this.SetAllContent(contentElement, str);
                p.UpdateLayout();
            }
            p = this.NewPanel(parent, ref offset, element, align);
            return this.WrapElement(parent, ref p, offset, str2, element, align, true);
        }

        public Microsoft.SilverlightMediaFramework.Core.Accessibility.Captions.CaptionRegion CaptionRegion
        {
            get
            {
                return (Microsoft.SilverlightMediaFramework.Core.Accessibility.Captions.CaptionRegion) base.GetValue(CaptionRegionProperty);
            }
            set
            {
                base.SetValue(CaptionRegionProperty, value);
            }
        }

        public TWC.OVP.Framework.Models.CaptionsOverrideSettings CaptionsOverrideSettings
        {
            get
            {
                return (TWC.OVP.Framework.Models.CaptionsOverrideSettings) base.GetValue(CaptionsOverrideSettingsProperty);
            }
            set
            {
                base.SetValue(CaptionsOverrideSettingsProperty, value);
            }
        }

        public string VisibleCaptionText
        {
            get
            {
                return (string) base.GetValue(VisibleCaptionTextProperty);
            }
            set
            {
                base.SetValue(VisibleCaptionTextProperty, value);
            }
        }

        private class CachedBrush
        {
            public CachedBrush(System.Windows.Media.Brush Brush)
            {
                this.Brush = Brush;
                this.LastUse = DateTime.Now;
            }

            public System.Windows.Media.Brush Brush { get; private set; }

            public DateTime LastUse { get; set; }
        }
    }
}

