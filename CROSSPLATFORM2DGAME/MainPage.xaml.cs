using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using System.Diagnostics;
using System.Numerics;
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
        AbsoluteLayout rotateLayout;

        AbsoluteLayout statsLayout;
        Label placeHolder;
        Label OBBPlaceHolder;
        Label OBBPlaceHolder2;


        //GAME OBJECTS
        player myP;
        enemy enemyTest;

        //VARIABLES
        public static double gameLayoutWidth;
        public static double gameLayoutHeight;
        public float playerX =0;
        public float playerY =0;

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
        
        private double xp = 0; // mapLayout X position
        private double yp = 0; // mapLayout Y position
        private double rp = 0; // rotation
        double speed = 0.0;
        double acceleration = 0.0;
        double maxSpeed = 4.0;
        double accelRate = 0.05;      // how fast the car accelerates
        double brakeRate = 0.0375;     // stronger deceleration
        double friction = 0.05;      // slows car when no input
        double turnSpeed = 1.0;      // base turn speed
        double boostMultiplier = 1.0; // speed boost multiplier
        double boostMaxSpeed = 7.0;
        int backFrame = 0;

        public void GameTimer_Elapsed(object sender, ElapsedEventArgs e) {
            double rad = rp * Math.PI / 180.0;

            // --- INPUT → ACCELERATION ------------------
            if (keyHandler.Up) {
                if (keyHandler.Space) {
                    boostMultiplier = 2.0;
                } else if (!keyHandler.Space) {
                    boostMultiplier = 1.0;
                }
                acceleration = accelRate * boostMultiplier;

                if (keyHandler.Left)
                    rp += turnSpeed * (speed / maxSpeed);  // proportional turning
                if (keyHandler.Right)
                    rp -= turnSpeed * (speed / maxSpeed);
            } else if (keyHandler.Down) {
                acceleration = -brakeRate;
                if (keyHandler.Left)
                    rp += turnSpeed * (speed / maxSpeed);  // proportional turning
                if (keyHandler.Right)
                    rp -= turnSpeed * (speed / maxSpeed);
            } else
                acceleration = 0;

            // --- APPLY ACCELERATION ---------------------
            speed += acceleration;

            // --- APPLY FRICTION WHEN NO KEYS PRESSED ----
            if (!keyHandler.Up && !keyHandler.Down) {
                if (speed > 0)
                    speed -= friction;
                if (speed < 0)
                    speed += friction;

                // avoid endless tiny motion
                if (Math.Abs(speed) < 0.05)
                    speed = 0;
            }

            // --- SPEED LIMIT ----------------------------
            if (keyHandler.Space && keyHandler.Up)
                speed = Math.Clamp(speed, -boostMaxSpeed, boostMaxSpeed);
            else
                speed = Math.Clamp(speed, -maxSpeed, maxSpeed);

            // --- MOVE CAR --------------------------------
            double dx = speed * Math.Sin(rad);
            double dy = speed * Math.Cos(rad);

            
            //OBB UPDATES
            myP.objectOBB.Update(new Vector2(playerX - (float)xp, playerY - (float)yp), rp * Math.PI / 180.0);
            enemyTest.objectOBB.Update(new Vector2(enemyTest.enemyOBBCenterX, enemyTest.enemyOBBCenterY), 0);

            bool collision = false;
            for (int i = OBBHandler.movingOBBs.Count - 1; i >= 0; i--) {
                if (myP.objectOBB.Intersects(OBBHandler.movingOBBs[i])) {
                    collision = true;
                    backFrame = 60;
                    break;
                }
            }

            if (collision) {
                if (backFrame > 0) {
                    acceleration -= brakeRate * 50;
                    speed += acceleration;
                    xp += dx;
                    yp += dy;

                    backFrame--;
                }

            } else if (!collision) {
                xp += dx;
                yp += dy;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                placeHolder.Text = $"Speed: {speed:F2} Accel: {acceleration:F2} Boost: {boostMultiplier:F1}";
                OBBPlaceHolder.Text = $"OBB Center: ({myP.objectOBB.Center.X:F2}, {myP.objectOBB.Center.Y:F2})\n" +
                    $"Width: {myP.objectOBB.Width:F2} Height: {myP.objectOBB.Height:F2}\n" +
                    $"Rotation: {rp:F2}°";
                mapLayout.TranslationX = xp;
                mapLayout.TranslationY = yp;
                rotateLayout.Rotation = rp;
                OBBPlaceHolder2.Text = $"OBB Center: ({enemyTest.objectOBB.Center.X:F2}, {enemyTest.objectOBB.Center.Y:F2})\n" +
                    $"Width: {enemyTest.objectOBB.Width:F2} Height: {enemyTest.objectOBB.Height:F2}\n" +
                    $"Rotation: {rp:F2}°\nCollision: {collision}";
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
                    playerX = (float)(mapLayout.Width / 2);
                    playerY = (float)(mapLayout.Height / 2);

                };

                gameLayout.SizeChanged += (s, e) =>
                {
                    //ONLY ASSIGN GAME LAYOUT WIDTH/HEIGHT IN HERE AS IT IS INVALID IN THE SETUP LAYOUT
                    if (myP == null) {

                        gameLayoutWidth = gameLayout.Width;
                        gameLayoutHeight = gameLayout.Height;

                        myP = new player();
                        enemyTest = new enemy();
                        /*
                        double centerX = gameLayout.Width / 2 - myP.layoutWidth / 2;
                        double centerY = gameLayout.Height / 2 - myP.layoutHeight / 2;

                        myP.setLayoutPosition(centerX, centerY, myP.layoutWidth, myP.layoutHeight);
                        */

                       
                        gameLayout.Children.Add(myP.gameObjectLayout);
                        mapLayout.Children.Add(enemyTest.gameObjectLayout);

                        myP.setUpOBB(new Vector2(playerX, playerY), (float)myP.layoutWidth, (float)myP.layoutHeight, 0);
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

            rotateLayout = new AbsoluteLayout {
                BackgroundColor = Colors.Transparent,
                WidthRequest = gameLayout.WidthRequest,
                HeightRequest = gameLayout.HeightRequest
            };

            //this will hold the stats like health fuel time etc
            statsLayout = new AbsoluteLayout {
                BackgroundColor = Colors.Transparent,
                WidthRequest = gameLayout.WidthRequest,
                HeightRequest = gameLayout.HeightRequest
            };

            OBBPlaceHolder = new Label {
                Text = "OBB Placeholder",
                BackgroundColor = Colors.Transparent
            };

            OBBPlaceHolder2 = new Label {
                BackgroundColor = Colors.Transparent
            };

            //place holder label for stats layout
            placeHolder = new Label {
                Text = "Stats Placeholder",
                BackgroundColor = Colors.Transparent

            };

            //this centers the map layout in the game layout
            AbsoluteLayout.SetLayoutBounds(mapLayout, new Rect(0.5, 0.5, mapLayout.WidthRequest, mapLayout.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(mapLayout, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(rotateLayout, new Rect(0.5, 0.5, gameLayout.WidthRequest, gameLayout.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(rotateLayout, AbsoluteLayoutFlags.PositionProportional);

            rotateLayout.Children.Add(mapLayout);
            gameLayout.Children.Add(rotateLayout);

            //this positions the stats layout to cover the entire game layout
            AbsoluteLayout.SetLayoutBounds(statsLayout, new Rect(0, 0, statsLayout.WidthRequest, statsLayout.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(statsLayout, AbsoluteLayoutFlags.None);

            AbsoluteLayout.SetLayoutBounds(OBBPlaceHolder, new Rect(0, 25, statsLayout.WidthRequest, statsLayout.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(OBBPlaceHolder, AbsoluteLayoutFlags.None);

            AbsoluteLayout.SetLayoutBounds(OBBPlaceHolder2, new Rect(0, 75, statsLayout.WidthRequest, statsLayout.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(OBBPlaceHolder2, AbsoluteLayoutFlags.None);

            statsLayout.Children.Add(OBBPlaceHolder);
            statsLayout.Children.Add(OBBPlaceHolder2);
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
