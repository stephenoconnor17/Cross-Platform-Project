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
        fuelObject fuelTest;
        List<gameObject> toRemove;

        //VARIABLES
        public static double gameLayoutWidth;
        public static double gameLayoutHeight;
        public float playerX =0;
        public float playerY =0;

        

        public MainPage() {
            InitializeComponent();
            keyHandler = new KeyHandler();
            toRemove = new List<gameObject>();
        }

        //SET UP GAME TIMER
        public void setUpTimer() {
            gameTimer =  new System.Timers.Timer(16);
            gameTimer.Elapsed += GameTimer_Elapsed;
            gameTimer.Start();
        }

        //GAME LOOP VARIABLES
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
        double boostMultiplier = 1.0; // speed boost multiplier (SPEED IS * BOOST SO BOOST MUST BE 1.0 AT MINIMUM)
        double boostMaxSpeed = 7.0;
        int backFrame = 0;

        public void GameTimer_Elapsed(object sender, ElapsedEventArgs e) {
            double rad = rp * Math.PI / 180.0; // convert rotation to radians

            // --- INPUT → ACCELERATION ------------------
            if (myP.fuel > 0) { //only allow drive if got fuel
                if (keyHandler.Up) {
                    if (keyHandler.Space) {
                        boostMultiplier = 2.0; // GIVE JUICE
                    } else if (!keyHandler.Space) {
                        boostMultiplier = 1.0;
                    }
                    acceleration = accelRate * boostMultiplier;

                    //Left and right steering only checked for while moving forward or backward
                    //because cars dont go literally sideways
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
            } else {
                //OUT OF FUEL - NO ACCELERATION
                acceleration = 0;
            }
            // --- APPLY ACCELERATION ---------------------
            speed += acceleration;

            //SUBTRACT FUEL WHILE MOVING
            if(Math.Abs(speed) > 0 && myP.fuel > 0) {
                myP.useFuel(boostMultiplier);
            }

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

            //UPDATES ENEMIES
            /*
             
                blah blah where
            the enemy decides how to kill the player

             */

            bool collision = false;
            //STATIC COLLISION DETECTION
            for (int i = OBBHandler.staticOBBs.Count - 1; i >= 0; i--) {
                if (myP.objectOBB.Intersects(OBBHandler.staticOBBs[i])) {

                    //FUEL PICKUP DETECTION
                    if (OBBHandler.staticOBBs[i].objectType == "fuel") {
                        myP.addFuel();
                    }
                    //LOOT PICKUP DETECTION



                    //GENERAL COLLISION RESPONSE
                    OBBHandler.staticOBBs.RemoveAt(i);//can remove here as its a logical list
                    toRemove.Add(fuelTest);//add to remove list to remove in UI Thread.

                }
            }

            //MOVING COLLISION DETECTION
            for (int i = OBBHandler.movingOBBs.Count - 1; i >= 0; i--) {
                if (myP.objectOBB.Intersects(OBBHandler.movingOBBs[i])) {
                    collision = true;
                    backFrame = 60; // amount of frames to allow back off after collision
                    break;
                }
            }
            if (collision) {
                if (speed > 0) {
                    speed = -speed; //invert speed on collision - sort of a bounce back effect
                    dx = speed * Math.Sin(rad); // reapply dx dy with new speed
                    dy = speed * Math.Cos(rad);
                    //bounce back is as good as it gets. I dont want to overcomplicate it. It hurts to.
                }

                if (backFrame > 0) { // a bit of a lopsided collision response but basically if backframe is over 0, a collision has been called.
                    backFrame--; // decrement backframe until 0. backframe is > 0 = 1 second of collision response given
                                //  60 fps  

                    xp += dx;
                    yp += dy;
                }
            } else if (!collision) {
                xp += dx;
                yp += dy;
            }

            // OBB UPDATES
            myP.objectOBB.Update(new Vector2(playerX - (float)xp, playerY - (float)yp), rp * Math.PI / 180.0);
            //enemy is rotated -rp to keep it aligned with the map layout rotation
            enemyTest.objectOBB.Update(new Vector2(enemyTest.enemyOBBCenterX, enemyTest.enemyOBBCenterY), -rp * Math.PI / 180.0);

            //UI UPDATES
            //SINCE GameTimer_Elapsed IS A BACKGROUND THREAD WE MUST INVOKE ON MAIN THREAD TO UPDATE UI ELEMENTS
            MainThread.BeginInvokeOnMainThread(() =>
            {
                //TEST OUTPUTS
                placeHolder.Text = $"Speed: {speed:F2} Accel: {acceleration:F2} Boost: {boostMultiplier:F1} Fuel {myP.fuel}";
                OBBPlaceHolder.Text = $"OBB Center: ({myP.objectOBB.Center.X:F2}, {myP.objectOBB.Center.Y:F2})\n" +
                    $"Width: {myP.objectOBB.Width:F2} Height: {myP.objectOBB.Height:F2}\n" +
                    $"Rotation: {rp:F2}°";

                
                OBBPlaceHolder2.Text = $"OBB Center: ({enemyTest.objectOBB.Center.X:F2}, {enemyTest.objectOBB.Center.Y:F2})\n" +
                    $"Width: {enemyTest.objectOBB.Width:F2} Height: {enemyTest.objectOBB.Height:F2}\n" +
                    $"Rotation: {rp:F2}°\nCollision: {collision}";

                //ACTUAL LAYOUT UPDATES
                mapLayout.TranslationX = xp;
                mapLayout.TranslationY = yp;
                rotateLayout.Rotation = rp;

                //rather clear what this does.
                if (toRemove.Count > 0) {
                    for(int i = 0; i < toRemove.Count; i++) {
                        mapLayout.Children.Remove(toRemove[i].gameObjectLayout);
                    }
                }
            });
        }

        protected override void OnDisappearing() {
            base.OnDisappearing();
            gameTimer.Stop();
        }

        //INITIAL SETUP BEFORE STARTING GAME LOOP
        bool onceOnAppearing = true; // because OnAppearing can be called many times, we only want to call it once!
        protected override async void OnAppearing() {

            //OnAppearing is being treated as the initial setup function for the game
            //Because the values we use to set up the layouts are invalid in the constructor
            //to double ensure we are getting values
            //we use sizeChanged events on the layouts to double triple ensure we have valid width/height values before using them.

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

                    playerX = (float)(mapLayout.Width / 2);
                    playerY = (float)(mapLayout.Height / 2);

                };

                gameLayout.SizeChanged += (s, e) =>
                {
                    //ONLY ASSIGN GAME LAYOUT WIDTH/HEIGHT IN HERE AS IT IS INVALID IN THE SETUP LAYOUT
                    if (myP == null) {

                        gameLayoutWidth = gameLayout.Width;
                        gameLayoutHeight = gameLayout.Height;

                        //CREATE GAME OBJECTS HERE AS NOW THE MAPLAYOUT IS PROPERLY SET UP AND WE HAVE VALID WIDTH/HEIGHT VALUES
                        myP = new player();
                        enemyTest = new enemy();
                        fuelTest = new fuelObject();

                        //ADD GAME OBJECTS TO LAYOUTS
                        gameLayout.Children.Add(myP.gameObjectLayout);// this is unique. the rest should be added to mapLayout

                        mapLayout.Children.Add(enemyTest.gameObjectLayout);
                        mapLayout.Children.Add(fuelTest.gameObjectLayout);

                        //We set this up here the center of mapLayout is now valid!
                        myP.setUpOBB(new Vector2(playerX, playerY), (float)myP.imageWidth - 8, (float)myP.imageHeight - 7, 0);
                        //myP.setUpOBB(new Vector2(playerX, playerY),32, 70, 0);
                        setUpTimer();


                        //DisplayAlert("Debug", $"gameLayout measured size: {gameLayout.Width} x {gameLayout.Height}", "OK");
                    }
                };
                
            }
            
        }

        //this is an ugly method that sets up all the layouts for the game
        //rotten looking to say the least.
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
        }
    }
}
