#if ANDROID
using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CROSSPLATFORM2DGAME {
    internal class androidInput {
        Image upArrow;
        Image downArrow;
        Image leftArrow;
        Image rightArrow;

        public bool Up { get; private set; }
        public bool Down { get; private set; }
        public bool Left { get; private set; }
        public bool Right { get; private set; }
        public bool Boost { get; private set; }

        Image boostButton;

        public androidInput() {
            upArrow = new Image { 
                Source = "arrow1.png",
                Aspect = Aspect.Fill
            };
            downArrow = new Image {
                Source = "arrow1.png",
                Rotation = 180,
                Aspect = Aspect.Fill
            };
            leftArrow = new Image {
                Source = "arrow1.png",
                Rotation = -90,
                Aspect = Aspect.Fill
            };
            rightArrow = new Image {
                Source = "arrow1.png",
                Rotation = 90,
                Aspect = Aspect.Fill
            };

            boostButton = new Image { 
                Source = "boost1.png",
                Aspect = Aspect.Fill
            };

            SetupGestures();
        }

        private void SetupGestures() {
            // Up arrow
            var upPointer = new PointerGestureRecognizer();
            upPointer.PointerPressed += (s, e) => Up = true;
            upPointer.PointerReleased += (s, e) => Up = false;
            upArrow.GestureRecognizers.Add(upPointer);

            // Down arrow
            var downPointer = new PointerGestureRecognizer();
            downPointer.PointerPressed += (s, e) => Down = true;
            downPointer.PointerReleased += (s, e) => Down = false;
            downArrow.GestureRecognizers.Add(downPointer);

            // Left arrow
            var leftPointer = new PointerGestureRecognizer();
            leftPointer.PointerPressed += (s, e) => Left = true;
            leftPointer.PointerReleased += (s, e) => Left = false;
            leftArrow.GestureRecognizers.Add(leftPointer);

            // Right arrow
            var rightPointer = new PointerGestureRecognizer();
            rightPointer.PointerPressed += (s, e) => Right = true;
            rightPointer.PointerReleased += (s, e) => Right = false;
            rightArrow.GestureRecognizers.Add(rightPointer);

            // Boost button
            var boostPointer = new PointerGestureRecognizer();
            boostPointer.PointerPressed += (s, e) => Boost = true;
            boostPointer.PointerReleased += (s, e) => Boost = false;
            boostButton.GestureRecognizers.Add(boostPointer);
        }

        public AbsoluteLayout controlsLayout;

        public void AddToLayout(AbsoluteLayout layout, double screenWidth, double screenHeight) {
            double buttonSize = Math.Min(screenWidth, screenHeight) * 0.2;
            double spacing = buttonSize * 0.2; // Space between buttons

            // Create a container layout for all controls
            controlsLayout = new AbsoluteLayout {
                BackgroundColor = Color.FromArgb("#40000000"), // Semi-transparent to see it
                WidthRequest = buttonSize * 3 + spacing * 2,
                HeightRequest = buttonSize * 3 + spacing * 2,
                InputTransparent = false // Make sure it can receive input
            };

            // Position buttons relative to controlsLayout
            // Up arrow - top center
            AbsoluteLayout.SetLayoutBounds(upArrow, 
                new Rect(buttonSize + spacing, 0, buttonSize, buttonSize));
            AbsoluteLayout.SetLayoutFlags(upArrow, AbsoluteLayoutFlags.None);

            // Down arrow - bottom center
            AbsoluteLayout.SetLayoutBounds(downArrow, 
                new Rect(buttonSize + spacing, buttonSize * 2 + spacing * 2, buttonSize, buttonSize));
            AbsoluteLayout.SetLayoutFlags(downArrow, AbsoluteLayoutFlags.None);

            // Left arrow - left middle
            AbsoluteLayout.SetLayoutBounds(leftArrow, 
                new Rect(0, buttonSize + spacing, buttonSize, buttonSize));
            AbsoluteLayout.SetLayoutFlags(leftArrow, AbsoluteLayoutFlags.None);

            // Right arrow - right middle
            AbsoluteLayout.SetLayoutBounds(rightArrow, 
                new Rect(buttonSize * 2 + spacing * 2, buttonSize + spacing, buttonSize, buttonSize));
            AbsoluteLayout.SetLayoutFlags(rightArrow, AbsoluteLayoutFlags.None);

            // Boost button - center
            AbsoluteLayout.SetLayoutBounds(boostButton, 
                new Rect(buttonSize + spacing, buttonSize + spacing, buttonSize, buttonSize));
            AbsoluteLayout.SetLayoutFlags(boostButton, AbsoluteLayoutFlags.None);

            controlsLayout.Children.Add(upArrow);
            controlsLayout.Children.Add(downArrow);
            controlsLayout.Children.Add(leftArrow);
            controlsLayout.Children.Add(rightArrow);
            controlsLayout.Children.Add(boostButton);

            // Position the entire controls layout at bottom-center of screen
            double layoutX = (screenWidth - controlsLayout.WidthRequest) / 5;
            double layoutY = screenHeight - controlsLayout.HeightRequest - 40;

            AbsoluteLayout.SetLayoutBounds(controlsLayout, 
                new Rect(layoutX, layoutY, controlsLayout.WidthRequest, controlsLayout.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(controlsLayout, AbsoluteLayoutFlags.None);

            layout.Children.Add(controlsLayout);
        }
    }
}
#endif