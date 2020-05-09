using System.Windows;
using System.Windows.Media.Animation;
using System;

namespace CryptoCalc
{
    /// <summary>
    /// Animation helpers for <see cref="Storyboard"/>
    /// </summary>
   public static class StoryboardHelpers
    {
        /// <summary>
        /// Adds a slide in from right animation to the storyboard
        /// </summary>
        /// <param name="storyboard">The storyboard to add to the animation</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="offset">The distance to the right to start from</param>
        /// <param name="decelerationRatio">The rate of deceleration</param>
        /// <param name="keepMargin">Whether to keep the element at the same width during animations</param>
        public static void AddSlideFromRight(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f, bool keepMargin = true)
        {
            //Create the margin animation from right
            var animation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = new Thickness(keepMargin ? offset : 0, 0, -offset, 0),
                To = new Thickness(0),
                DecelerationRatio = decelerationRatio
            };

            //Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

            // Add this to the storyboard
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Adds a slide in from left animation to the storyboard
        /// </summary>
        /// <param name="storyboard">The storyboard to add to the animation</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="offset">The distance to the left to start from</param>
        /// <param name="decelerationRatio">The rate of deceleration</param>
        /// <param name="keepMargin">Whether to keep the element at the same width during animations</param>
        public static void AddSlideFromLeft(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f, bool keepMargin = true)
        {
            //Create the margin animation from right
            var animation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = new Thickness(-offset, 0, keepMargin ? offset : 0, 0),
                To = new Thickness(0),
                DecelerationRatio = decelerationRatio
            };

            //Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

            // Add this to the storyboard
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Adds a slide in from top animation to the storyboard
        /// </summary>
        /// <param name="storyboard">The storyboard to add to the animation</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="offset">The distance to the left to start from</param>
        /// <param name="decelerationRatio">The rate of deceleration</param>
        /// <param name="keepMargin">Whether to keep the element at the same width during animations</param>
        public static void AddSlideFromTop(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f, bool keepMargin = true)
        {
            //Create the margin animation from right
            var animation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = new Thickness(0, -offset, 0, keepMargin ? offset : 0),
                To = new Thickness(0),
                DecelerationRatio = decelerationRatio
            };

            //Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

            // Add this to the storyboard
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Adds a slide out to left animation to the storyboard
        /// </summary>
        /// <param name="storyboard">The storyboard to add to the animation</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="offset">The distance to the right to end at from</param>
        /// <param name="decelerationRatio">The rate of deceleration</param>
        /// <param name="keepMargin">Whether to keep the element at the same width during animations</param>
        public static void AddSlideOutLeft(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f, bool keepMargin = true)
        {
            //Create the margin animation from right
            var animation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = new Thickness(0),
                To = new Thickness(-offset, 0, keepMargin ? offset : 0, 0),
                DecelerationRatio = decelerationRatio
            };

            //Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

            // Add this to the storyboard
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Adds a slide out to right animation to the storyboard
        /// </summary>
        /// <param name="storyboard">The storyboard to add to the animation</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="offset">The distance to the right to end at from</param>
        /// <param name="decelerationRatio">The rate of deceleration</param>
        /// <param name="keepMargin">Whether to keep the element at the same width during animations</param>
        public static void AddSlideOutRight(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f, bool keepMargin = true)
        {
            //Create the margin animation from right
            var animation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = new Thickness(0),
                To = new Thickness(keepMargin ? offset : 0, 0, -offset, 0),
                DecelerationRatio = decelerationRatio
            };

            //Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

            // Add this to the storyboard
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Adds a slide out to bottom animation to the storyboard
        /// </summary>
        /// <param name="storyboard">The storyboard to add to the animation</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="offset">The distance to the right to end at from</param>
        /// <param name="decelerationRatio">The rate of deceleration</param>
        /// <param name="keepMargin">Whether to keep the element at the same width during animations</param>
        public static void AddSlideOutBottom(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f, bool keepMargin = true)
        {
            //Create the margin animation from right
            var animation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = new Thickness(0),
                To = new Thickness(0, keepMargin ? offset : 0, 0, -offset),
                DecelerationRatio = decelerationRatio
            };

            //Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

            // Add this to the storyboard
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Adds a fade in animation to the storyboard
        /// </summary>
        /// <param name="storyboard">the storyboard to add the animation</param>
        /// <param name="seconds">the time it takes to do animation</param>
        public static void AddsFadeIn(this Storyboard storyboard, float seconds)
        {
            //Create the margin animate from right
            var animation = new DoubleAnimation
            {
                Duration = (TimeSpan.FromSeconds(seconds)),
                From = 0,
                To = 1,
            };

            //Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

            //Add this to the storyboard
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Adds a fade out animation to the storyboard
        /// </summary>
        /// <param name="storyboard">the storyboard to add the animation</param>
        /// <param name="seconds">the time it takes to do animation</param>
        public static void AddsFadeOut(this Storyboard storyboard, float seconds)
        {
            //Create the margin animate from right
            var animation = new DoubleAnimation
            {
                Duration = (TimeSpan.FromSeconds(seconds)),
                From = 1,
                To = 0,
            };

            //Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

            //Add this to the storyboard
            storyboard.Children.Add(animation);
        }
    }
}
