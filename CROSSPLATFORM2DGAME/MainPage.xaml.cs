using Microsoft.Maui.Layouts;
using System.Diagnostics;

namespace CROSSPLATFORM2DGAME {
    public partial class MainPage : ContentPage {
        Image playerImg;
        Image enemyImg;

        AbsoluteLayout gameLayout;
        AbsoluteLayout mapLayout;

        AbsoluteLayout statsLayout;

        AbsoluteLayout playerLayout;
        AbsoluteLayout enemyLayout;

#if WINDOWS
        Microsoft.UI.Xaml.Window nativeWindow; // Windows-specific window for key input
#endif

        public MainPage() {
            InitializeComponent();
        }

        private void initializeImgObj() {
            playerImg = new Image {
                Source = "player.png",
                WidthRequest = 50,
                HeightRequest = 50
            };
            enemyImg = new Image {
                Source = "player.png",
                WidthRequest = 50,
                HeightRequest = 50
            };
        }

        protected override void OnAppearing() {

            //to describe onappearing
            // This method is called when the page appears
            // It creates the mapLayout and then adds the playerLayout to it
            // It also initializes the player and enemy images but only adds the player image to the layout for now
            // this is where we will set up the game elements and start the game loop for now
            // but a menu will be added later before the game loop starts
            base.OnAppearing();

            // Initialize layouts and images

            //This is the overaraching layout that contains all other layouts
            gameLayout = new AbsoluteLayout {
                WidthRequest = this.Width,
                HeightRequest = this.Height,
                BackgroundColor = Colors.Transparent

            };

            //this will hold the map background and enemies
            mapLayout = new AbsoluteLayout {
                BackgroundColor = Colors.LightGreen,
                WidthRequest = 800,
                HeightRequest = 400
            };

            //this will hold the stats like health fuel time etc
            statsLayout = new AbsoluteLayout {
                BackgroundColor = Colors.Transparent,
                WidthRequest = gameLayout.WidthRequest,
                HeightRequest = gameLayout.HeightRequest
            };

            //place holder label for stats layout
            Label placeHolder = new Label {
                Text = "Stats Placeholder",
                BackgroundColor = Colors.Transparent

            };

            //this centers the map layout in the game layout
            AbsoluteLayout.SetLayoutBounds(mapLayout, new Rect(0.5, 0.5, mapLayout.WidthRequest, mapLayout.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(mapLayout, AbsoluteLayoutFlags.PositionProportional);

            gameLayout.Children.Add(mapLayout);

            //this positions the stats layout to cover the entire game layout
            AbsoluteLayout.SetLayoutBounds(statsLayout, new Rect(0,0, statsLayout.WidthRequest, statsLayout.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(statsLayout, AbsoluteLayoutFlags.None);

            statsLayout.Children.Add(placeHolder);
            gameLayout.Children.Add(statsLayout);

            //this.Padding = new Thickness(0,40,0,0);

            //this sets the content of the page to the game layout i.e the overarching layout
            Content = gameLayout;

            mapLayout.SizeChanged += (s, e) => {

                initializeImgObj();

                // Create player layout and add player image
                playerLayout = new AbsoluteLayout();
                playerLayout.WidthRequest = 50;
                playerLayout.HeightRequest = 50;
                //enemyLayout = new AbsoluteLayout();

                // Create enemy layout and add enemy image
                enemyLayout = new AbsoluteLayout();
                enemyLayout.WidthRequest = 50;
                enemyLayout.HeightRequest = 50;

                //This gets the values to center the images in their layouts
                double playerCenterX = playerLayout.WidthRequest / 2 - playerImg.WidthRequest / 2;
                double playerCenterY = playerLayout.HeightRequest / 2 - playerImg.HeightRequest / 2;

                double enemyCenterX = enemyLayout.WidthRequest / 2 - enemyImg.WidthRequest / 2;
                double enemyCenterY = enemyLayout.HeightRequest / 2 - enemyImg.HeightRequest / 2;

                //this positions the player and enemy images in the center of their respective layouts
                AbsoluteLayout.SetLayoutBounds(playerImg, new Rect(playerCenterX, playerCenterY, playerImg.WidthRequest, playerImg.HeightRequest));
                AbsoluteLayout.SetLayoutFlags(playerImg, AbsoluteLayoutFlags.None);
                playerLayout.Children.Add(playerImg);

                AbsoluteLayout.SetLayoutBounds(enemyImg, new Rect(enemyCenterX, enemyCenterY, enemyImg.WidthRequest, enemyImg.HeightRequest));
                AbsoluteLayout.SetLayoutFlags(enemyImg, AbsoluteLayoutFlags.None);
                enemyLayout.Children.Add(enemyImg);

                // Center player layout in map layout
                AbsoluteLayout.SetLayoutBounds(playerLayout, new Rect(
                    gameLayout.Width / 2 - playerLayout.WidthRequest / 2,
                    gameLayout.Height / 2 - playerLayout.HeightRequest / 2,
                    playerLayout.WidthRequest,
                    playerLayout.HeightRequest));

                // Position enemy layout at a fixed point in map layout
                AbsoluteLayout.SetLayoutBounds(enemyLayout, new Rect(
                    200,
                    200,
                    enemyLayout.WidthRequest,
                    enemyLayout.HeightRequest));

                // Add layouts to map layout
                mapLayout.Children.Add(enemyLayout);
                gameLayout.Children.Add(playerLayout);
            };



            // Start game loop
            //Device.StartTimer(TimeSpan.FromMilliseconds(16), GameLoop);

            //below is the platform specific code for key listeners
            //this is only for windows for now
            //and a placeholder. 
            //later on a centralised input manager will be created to handle inputs from all platforms
#if WINDOWS
            // Attach Windows key listener non-intrusively
            Device.BeginInvokeOnMainThread(() =>
            {
                nativeWindow = Application.Current.Windows[0].Handler.PlatformView as Microsoft.UI.Xaml.Window;
                if (nativeWindow != null)
                {
                    var root = nativeWindow.Content as Microsoft.UI.Xaml.FrameworkElement;
                    if (root != null)
                    {
                        root.KeyDown += Root_KeyDown;
                        root.KeyUp += Root_KeyUp;
                        Debug.WriteLine("✅ Windows key listener attached");
                    }
                }
            });
#endif
        }

#if WINDOWS
        // Handle key down events
        private void Root_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            Debug.WriteLine($"Key Down: {e.Key}");

            switch (e.Key)
            {
                case Windows.System.VirtualKey.W:
                    Debug.WriteLine("Move Up");
                    mapLayout.TranslationY += 10;
                    //playerLayout.TranslationY -= 10;
                    break;
                case Windows.System.VirtualKey.A:
                    //playerLayout.Rotation -= 5;
                    mapLayout.Rotation += 5;
                    Debug.WriteLine("Move Left");
                    break;
                case Windows.System.VirtualKey.S:
                    Debug.WriteLine("Move Down");
                    mapLayout.TranslationY -= 10;
                    //playerLayout.TranslationY += 10;
                    break;
                case Windows.System.VirtualKey.D:

                    //playerLayout.Rotation += 5;
                    mapLayout.Rotation -= 5;
                    Debug.WriteLine("Move Right");
                    break;
            }
        }

        // Handle key up events
        private void Root_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            Debug.WriteLine($"Key Up: {e.Key}");
        }
#endif
    }
}
