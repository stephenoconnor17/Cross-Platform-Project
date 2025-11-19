using Microsoft.Maui.Layouts;
using System.Diagnostics;
using System.Timers;

namespace CROSSPLATFORM2DGAME {
    public partial class MainPage : ContentPage {

        //KEYHANDLER
        KeyHandler keyHandler;

        //GAME TIMER
        System.Timers.Timer gameTimer;

        //LAYOUT / VIEWS
        AbsoluteLayout gameLayout;
        AbsoluteLayout mapLayout;

        AbsoluteLayout statsLayout;

        //GAME OBJECTS
        player myP;

        //VARIABLES
        public static double gameLayoutWidth;
        public static double gameLayoutHeight;

        public MainPage() {
            InitializeComponent();
            keyHandler = new KeyHandler();

        }

        //SET UP GAME TIMER
        public void setUpTimer() {
            gameTimer =  new System.Timers.Timer(16);
            gameTimer.Elapsed += GameTimer_Elapsed;
            gameTimer.Start();
        }

        //GAME LOOP
        double yp = 0;
        double rp = 0;
        private void GameTimer_Elapsed(object sender, ElapsedEventArgs e) {
            //UPDATE GAME STATE HERE
            if (keyHandler.Up) {
                yp += 2;
            }

            if (keyHandler.Down) {
                yp -= 2;
            }

            if (keyHandler.Left) {
                rp += .5;
            }

            if (keyHandler.Right) {
                rp -= .5;
            }

            //UPDATE UI ON MAIN THREAD
            MainThread.BeginInvokeOnMainThread(() =>
            {
                mapLayout.TranslationY = yp;
                mapLayout.Rotation = rp;
            });
        }

        protected override void OnDisappearing() {
            base.OnDisappearing();
            gameTimer.Stop();
        }

        //INITIAL SETUP BEFORE STARTING GAME LOOP
        bool onceOnAppearing = true;
        protected override async void OnAppearing() {

            //to describe onappearing
            // This method is called when the page appears
            // It creates the mapLayout and then adds the playerLayout to it
            // It also initializes the player and enemy images but only adds the player image to the layout for now
            // this is where we will set up the game elements and start the game loop for now
            // but a menu will be added later before the game loop starts
            base.OnAppearing();
#if WINDOWS
        if (this.Window != null)
        {
            KeyHook.Attach(this.Window, keyHandler);
        }
#endif

            // Initialize layouts and images

            //This is the overaraching layout that contains all other layouts
            if (onceOnAppearing)
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
                    //ONLY ASSIGN GAME LAYOUT WIDTH/HEIGHT IN HERE AS IT IS INVALID IN THE SETUP LAYOUT
                    if (myP == null) {

                        gameLayoutWidth = gameLayout.Width;
                        gameLayoutHeight = gameLayout.Height;

                        myP = new player();

                        /*
                        double centerX = gameLayout.Width / 2 - myP.layoutWidth / 2;
                        double centerY = gameLayout.Height / 2 - myP.layoutHeight / 2;

                        myP.setLayoutPosition(centerX, centerY, myP.layoutWidth, myP.layoutHeight);
                        */

                        gameLayout.Children.Add(myP.gameObjectLayout);

                        setUpTimer();

                        //DisplayAlert("Debug", $"gameLayout measured size: {gameLayout.Width} x {gameLayout.Height}", "OK");
                    }
                };
            }
            
            //because of the width and height requests are only valid once the size is known
            

         
            //below is the platform specific code for key listeners
            //this is only for windows for now
            //and a placeholder. 
            //later on a centralised input manager will be created to handle inputs from all platforms

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



    }
}
