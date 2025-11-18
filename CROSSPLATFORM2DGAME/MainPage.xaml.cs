using Microsoft.Maui.Layouts;
using System.Diagnostics;

namespace CROSSPLATFORM2DGAME {
    public partial class MainPage : ContentPage {
        
        //LAYOUT / VIEWS
        AbsoluteLayout gameLayout;
        AbsoluteLayout mapLayout;

        AbsoluteLayout statsLayout;

        //GAME OBJECTS
        player myP;

        //VARIABLES
        public static double gameLayoutWidth;
        public static double gameLayoutHeight;


#if WINDOWS
        Microsoft.UI.Xaml.Window nativeWindow; // Windows-specific window for key input
#endif

        public MainPage() {
            InitializeComponent();
        }

        bool onceOnAppearing = true;
        protected override async void OnAppearing() {

            //to describe onappearing
            // This method is called when the page appears
            // It creates the mapLayout and then adds the playerLayout to it
            // It also initializes the player and enemy images but only adds the player image to the layout for now
            // this is where we will set up the game elements and start the game loop for now
            // but a menu will be added later before the game loop starts
            base.OnAppearing();

            // Initialize layouts and images

            //This is the overaraching layout that contains all other layouts
            if(onceOnAppearing)
            {
                SetupGameLayout();
                onceOnAppearing = false;
                //we only place layouts to map layout here
                mapLayout.SizeChanged += (s, e) => {

                    //myP = new player();
                    // Center player layout in map layout
                    //gameLayout.Children.Add(myP.gameObjectLayout);
                    
                };

                gameLayout.SizeChanged += (s, e) =>
                {
                    // Only create player once
                    if (myP == null) {
                        myP = new player();

                        double centerX = gameLayout.Width / 2 - myP.layoutWidth / 2;
                        double centerY = gameLayout.Height / 2 - myP.layoutHeight / 2;

                        myP.setLayoutPosition(centerX, centerY, myP.layoutWidth, myP.layoutHeight);

                        gameLayout.Children.Add(myP.gameObjectLayout);

                        DisplayAlert("Debug", $"gameLayout measured size: {gameLayout.Width} x {gameLayout.Height}", "OK");
                    }
                };
            }
            
            //because of the width and height requests are only valid once the size is known
            

         
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
        public void SetupGameLayout()
        {

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
            AbsoluteLayout.SetLayoutBounds(statsLayout, new Rect(0, 0, statsLayout.WidthRequest, statsLayout.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(statsLayout, AbsoluteLayoutFlags.None);

            statsLayout.Children.Add(placeHolder);
            gameLayout.Children.Add(statsLayout);

            //this sets the content of the page to the game layout i.e the overarching layout
            Content = gameLayout;

            gameLayoutWidth = gameLayout.WidthRequest;
            gameLayoutHeight = gameLayout.HeightRequest;

            //return Task.CompletedTask;
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
