using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace RefOrganiser.Controls
{
    [TemplatePart(Name = ViewboxPartName, Type = typeof(Grid))]
    [TemplatePart(Name = IconPartName, Type = typeof(Path))]
    public sealed class ExpandButton : Control
    {
        // Template Parts.
        private const string ViewboxPartName = "PART_Viewbox";
        private const string IconPartName = "PART_Icon";

        private Path Icon { get; set; }
        private Compositor IconCompositor { get; set; }
        private ScalarKeyFrameAnimation RotateDown { get; set; }
        private ScalarKeyFrameAnimation RotateUp { get; set; }
        public ExpandButton()
        {
            this.DefaultStyleKey = typeof(ExpandButton);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            
            Icon = GetTemplateChild(IconPartName) as Path;
            IconCompositor = ElementCompositionPreview.GetElementVisual(Icon).Compositor;
            RotateDown = IconCompositor.CreateScalarKeyFrameAnimation();
            var easing = IconCompositor.CreateLinearEasingFunction();
            RotateDown.InsertKeyFrame(0.0f, 0.0f);
            RotateDown.InsertKeyFrame(1.0f,90.0f,easing);
            RotateDown.Duration = TimeSpan.FromMilliseconds(250);
            RotateDown.IterationBehavior = AnimationIterationBehavior.Count;
            RotateDown.IterationCount = 1;

            RotateUp = IconCompositor.CreateScalarKeyFrameAnimation();
            RotateUp.InsertKeyFrame(0.0f, 90.0f);
            RotateUp.InsertKeyFrame(1.0f, 0.0f, easing);
            RotateUp.Duration = TimeSpan.FromMilliseconds(250);
            RotateUp.IterationBehavior = AnimationIterationBehavior.Count;
            RotateUp.IterationCount = 1;
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            e.Handled = true;
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            e.Handled = true;
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);
            //Inverse state
            State = State==ExpandState.Expanded ? ExpandState.NotExpanded : ExpandState.Expanded;
            //Play animation based on state
            var iconVisual = ElementCompositionPreview.GetElementVisual(Icon);
            if (State == ExpandState.Expanded)
            {
                
                iconVisual.CenterPoint = new Vector3(
                    (float) Icon.ActualWidth / 2.0f,
                    (float) Icon.ActualHeight / 2.0f,
                    0.0f);
                iconVisual.StartAnimation(nameof(iconVisual.RotationAngleInDegrees), RotateDown);
            }
            else
            {
                iconVisual.CenterPoint = new Vector3(
                    (float)Icon.ActualWidth / 2.0f,
                    (float)Icon.ActualHeight / 2.0f,
                    0.0f);
                iconVisual.StartAnimation(nameof(iconVisual.RotationAngleInDegrees), RotateUp);
            }

            if (TappedEvent == null)
            {
                e.Handled = true;
            }
        }

        #region Properties
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
            "State", typeof(ExpandState), typeof(ExpandButton), new PropertyMetadata(default(ExpandState)));

        public ExpandState State
        {
            get { return (ExpandState) GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(
            "Duration", typeof(double), typeof(ExpandButton), new PropertyMetadata(default(double)));

        public double Duration
        {
            get { return (double) GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }
        #endregion
        public enum ExpandState
        {
            NotExpanded, Expanded
        }
    }
}
