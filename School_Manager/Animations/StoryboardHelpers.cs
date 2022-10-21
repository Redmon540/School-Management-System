using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace School_Manager
{
    /// <summary>
    /// Animation Helper for<see cref="StoryBoard"/>
    /// </summary>
    
    public static class StoryboardHelpers
    {
        /// <summary>
        /// Adds a slide to right animation to the storyboard
        /// </summary>
        /// <param name="Storyboard"></param>
        /// <param name="Seconds"></param>
        /// <param name="Offset"></param>
        /// <param name="DecelerationRatio"></param>
        public static void AddSlideFromRight(this Storyboard Storyboard, float Seconds, double Offset, float DecelerationRatio = 0.9f)
        {
            // Create the margin animate from right
            var animation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(Seconds)),
                From = new Thickness(Offset, 0, -Offset, 0),
                To = new Thickness(0),
                DecelerationRatio = DecelerationRatio
            };

            //Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

            //Add this to the storyboard
            Storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Adds a slide to left animation to the storyboard
        /// </summary>
        /// <param name="Storyboard"></param>
        /// <param name="Seconds"></param>
        /// <param name="Offset"></param>
        /// <param name="DecelerationRatio"></param>
        public static void AddSlideToLeft(this Storyboard Storyboard, float Seconds, double Offset, float DecelerationRatio = 0.9f)
        {
            // Create the margin animate from right
            var animation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(Seconds)),
                From = new Thickness(0),
                To = new Thickness(-Offset, 0, Offset, 0),
                DecelerationRatio = DecelerationRatio
            };

            //Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

            //Add this to the storyboard
            Storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Adds an animation to increase width to the storyboard
        /// </summary>
        /// <param name="Storyboard"></param>
        /// <param name="Seconds"></param>
        /// <param name="Offset"></param>
        /// <param name="DecelerationRatio"></param>
        public static void AddIncreaseWidth(this Storyboard Storyboard, float Seconds, double Offset, float DecelerationRatio = 0.9f)
        {
            // Create the margin animate from right
            var animation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(Seconds)),
                From = 0,
                To = Offset,
                DecelerationRatio = DecelerationRatio
            };

            //Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath("Width"));

            //Add this to the storyboard
            Storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Adds an animation to decrease width to the storyboard
        /// </summary>
        /// <param name="Storyboard"></param>
        /// <param name="Seconds"></param>
        /// <param name="Offset"></param>
        /// <param name="DecelerationRatio"></param>
        public static void AddDecreaseWidth(this Storyboard Storyboard, float Seconds, double offset, float DecelerationRatio = 0.9f)
        {
            // Create the margin animate from right
            var animation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(Seconds)),
                To = offset,
                DecelerationRatio = DecelerationRatio
            };

            //Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath("Width"));

            //Add this to the storyboard
            Storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Adds a fade in animation to the storyboard
        /// </summary>
        /// <param name="Storyboard"></param>
        /// <param name="Seconds"></param>
        public static void AddFadeIn(this Storyboard Storyboard, float Seconds)
        {
            // Create the margin animate from right
            var animation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(Seconds)),
                From = 0,
                To = 1,
            };

            //Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

            //Add this to the storyboard
            Storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Adds a fade out animation to the storyboard
        /// </summary>
        /// <param name="Storyboard"></param>
        /// <param name="Seconds"></param>
        public static void AddFadeOut(this Storyboard Storyboard, float Seconds)
        {
            // Create the margin animate from right
            var animation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(Seconds)),
                From = 1,
                To = 0,
            };

            //Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

            //Add this to the storyboard
            Storyboard.Children.Add(animation);
        }

    }
}
