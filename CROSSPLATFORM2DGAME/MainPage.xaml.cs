using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using Microsoft.UI.Xaml.Input;
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

        AbsoluteLayout gameOverLayout;
        Label gameOverLabel1;
        Label gameOverLabel2;
        Label gameOverLabel3;
        int highestScore = 0;
        public static int score = 0; //static because it gets added to from within other objects when they die.

        AbsoluteLayout statsLayout;

        AbsoluteLayout fuelLayout;
        BoxView fuelBar;

        AbsoluteLayout boostLayout;
        BoxView boostBar;

        AbsoluteLayout timerLayout;
        Label timerLabel;
        System.Timers.Timer timeTimer;

        Label placeHolder;
        Label OBBPlaceHolder;
        Label OBBPlaceHolder2;


        //GAME OBJECTS
        player myP;
        enemy enemyTest;
        enemy enemyTest2;
        List<enemy> enemies;
        fuelObject fuelTest;
        List<gameObject> toRemove;

        //VARIABLES
        public static double gameLayoutWidth;
        public static double gameLayoutHeight;
        public float playerX = 0;
        public float playerY = 0;



        public MainPage() {
            InitializeComponent();
            keyHandler = new KeyHandler();
            enemies = new List<enemy>();
            toRemove = new List<gameObject>();
        }

        //SET UP GAME TIMER
        public void setUpTimer() {
            gameTimer = new System.Timers.Timer(16);
            gameTimer.Elapsed += GameTimer_Elapsed;
            gameTimer.Start();
        }

        public void setUpGameOverLayout() {
            gameOverLayout = new AbsoluteLayout {
                BackgroundColor = Color.FromArgb("#80000000"),
                WidthRequest = gameLayoutWidth,
                HeightRequest = gameLayoutHeight,
                Opacity = 0.8
            };

            gameOverLabel1 = new Label {
                WidthRequest = gameLayoutWidth / 3,
                HeightRequest = gameLayoutHeight / 6,
                Text = "GAME OVER",
                FontFamily= "Consolas"
            };
            gameOverLabel2 = new Label {
                WidthRequest = gameLayoutWidth / 4,
                HeightRequest = gameLayoutHeight / 7,
                Text = "HIGH SCORE: 0",
                FontFamily = "Consolas"
            };
            gameOverLabel3 = new Label {
                WidthRequest = gameLayoutWidth / 4,
                HeightRequest = gameLayoutHeight / 7,
                Text = "SCORE: ",
                FontFamily = "Consolas"
            };

            AbsoluteLayout.SetLayoutBounds(gameOverLayout, new Rect(0, 0, gameOverLayout.WidthRequest, gameOverLayout.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(gameOverLayout, AbsoluteLayoutFlags.None);

            AbsoluteLayout.SetLayoutBounds(gameOverLabel1, new Rect(0.5, 0.3, gameOverLabel1.WidthRequest, gameOverLabel1.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(gameOverLabel1, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(gameOverLabel2, new Rect(0.5, 0.45, gameOverLabel2.WidthRequest, gameOverLabel2.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(gameOverLabel2, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(gameOverLabel3, new Rect(0.5, 0.6, gameOverLabel3.WidthRequest, gameOverLabel3.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(gameOverLabel3, AbsoluteLayoutFlags.PositionProportional);
            gameOverLayout.Children.Add(gameOverLabel1);
            gameOverLayout.Children.Add(gameOverLabel2);
            gameOverLayout.Children.Add(gameOverLabel3);

            gameOverLayout.IsVisible = false;
        }

        bool gameOver = false;
        //gameOver logic here!
        public void endGame() {
            gameTimer.Stop();
            timeTimer.Stop();

            

            gameOver = true;
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
        bool isBoosting = false;
        int backFrame = 0;

        bool invincibleSkin = false;
        int invincibleFrame = 0;
        int needToChange = 0;

        public void GameTimer_Elapsed(object sender, ElapsedEventArgs e) {
            double rad = rp * Math.PI / 180.0; // convert rotation to radians

            for(int i = enemies.Count - 1; i >= 0; i--) {
                if (enemies[i].removeThis == true) {
                    OBBHandler.movingOBBs.Remove(enemies[i].objectOBB);
                    toRemove.Add(enemies[i]);
                }
            }

            if(myP.lives <= 0 || (speed <= 0 && myP.fuel <= 0)) {
                //game over in endGame();
                endGame();
              
            }

            

            // --- INPUT → ACCELERATION ------------------
            if (myP.fuel > 0) { //only allow drive if got fuel
                if (keyHandler.Up) {
                    
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
          
            //SUBTRACT FUEL WHILE MOVING
            if (Math.Abs(speed) > 0 && myP.fuel > 0) {
                myP.useFuel(boostMultiplier);
            }

            // --- APPLY FRICTION WHEN NO KEYS PRESSED ----
            if (!keyHandler.Up && !keyHandler.Down || myP.fuel <= 0) {
                if (speed > 0)
                    speed -= friction;
                if (speed < 0)
                    speed += friction;

                // avoid endless tiny motion
                if (Math.Abs(speed) < 0.05)
                    speed = 0;
            }

            // --- SPEED LIMIT ----------------------------
            //only allow boost if above threshold of boost amount so cant keep tapping space to go fast
            if (keyHandler.Space && keyHandler.Up && myP.fuel > 0) {
                // Start boost if above threshold
                if (!isBoosting && myP.boostAmount >= 25)
                    isBoosting = true;

                // Stop boost if below minimum
                if (myP.boostAmount <= 0)
                    isBoosting = false;

                if (isBoosting && myP.boostAmount > 0) {
                    boostMultiplier = 2.0; // GIVE JUICE
                    acceleration = accelRate * boostMultiplier;
                    speed += acceleration;
                    speed = Math.Clamp(speed, -boostMaxSpeed, boostMaxSpeed);

                    myP.useBoost();
                } else {
                    boostMultiplier = 1.0;
                    acceleration = accelRate * boostMultiplier;
                    speed += acceleration;
                    speed = Math.Clamp(speed, -maxSpeed, maxSpeed);
                }
            } else {
                // Space not pressed -> stop boosting
                isBoosting = false;
                boostMultiplier = 1.0;
               
                //speed += acceleration;
                speed = Math.Clamp(speed, -maxSpeed, maxSpeed);
            }
            //acceleration = accelRate * boostMultiplier;
            speed += acceleration;
            //speed = Math.Clamp(speed, -maxSpeed, maxSpeed);


            // --- MOVE CAR --------------------------------
            double dx = speed * Math.Sin(rad);
            double dy = speed * Math.Cos(rad);

           

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
                    toRemove.Add(OBBHandler.staticOBBs[i].thisObject); //add to remove list to remove in UI Thread.
                    OBBHandler.staticOBBs.RemoveAt(i);//can remove here as its a logical list
                   

                }
            }

    
            //MOVING COLLISION DETECTION
            //check for collision if not invincible i.e if invincibleFrame > 0, invincible, dont bother check collision.
            if (invincibleFrame <= 0) {
                for (int i = OBBHandler.movingOBBs.Count - 1; i >= 0; i--) {
                    if (myP.objectOBB.Intersects(OBBHandler.movingOBBs[i])) {
                        if (!collision) {
                            //collisionJustHappened = true; // only set to true on first collision detection
                            invincibleFrame = 180;
                            invincibleSkin = true;
                            needToChange = 1;
                            myP.lives--;
                        }
                        collision = true;
                        backFrame = 60; // amount of frames to allow back off after collision i.e one second at 60fps.
                        OBBHandler.movingOBBs[i].thisEnemy.collision = true; // inform the enemy it has collided
                        break;
                    }
                }
            }
            //we do this so that it only calculates collision math once.
            if (collision) {
                if (Math.Abs(speed) > 0 && invincibleFrame == 180) { //collisionJustHappened ensures we dont continously invert speed every frame
                    speed = -speed; //invert speed on collision - sort of a bounce back effect
                    dx = speed * Math.Sin(rad); // reapply dx dy with new speed
                    dy = speed * Math.Cos(rad);
                    //bounce back is as good as it gets. I dont want to overcomplicate it. It hurts to.
                }

                if (backFrame > 0) { // a bit of a lopsided collision response but basically if backframe is over 0, a collision has been called.
                    backFrame--; // decrement backframe until 0. backframe / 60 = x amount of seconds of collision response given
                                 //  60 fps  

                    xp += dx;
                    yp += dy;
                }

                
            } else if (!collision) {
                xp += dx;
                yp += dy;
            }

            if (invincibleFrame <= 180) {
                invincibleFrame--;
            }

            if(invincibleFrame <= 0) {
                invincibleFrame = 0;
                invincibleSkin = false;
            }

            

            //collisionJustHappened = false; // reset for next frame

            // OBB UPDATES
            myP.objectOBB.Update(new Vector2(playerX - (float)xp, playerY - (float)yp), rp * Math.PI / 180.0);
            //enemy is rotated -rp to keep it aligned with the map layout rotation
            //enemyTest.objectOBB.Update(new Vector2(enemyTest.enemyOBBCenterX, enemyTest.enemyOBBCenterY), -rp * Math.PI / 180.0);
            for (int i = 0; i < enemies.Count; i++) {
                enemies[i].update(playerX - (float)xp, playerY - (float)yp);
            }


                //UI UPDATES
                //SINCE GameTimer_Elapsed IS A BACKGROUND THREAD WE MUST INVOKE ON MAIN THREAD TO UPDATE UI ELEMENTS
                MainThread.BeginInvokeOnMainThread(() => {
                    
                    //compute gameOver stuff here graphically.
                    if (gameOver) {
                        statsLayout.Opacity = 0.8;
                        timerLayout.Opacity = 0.8;

                        gameOverLabel3.Text = "Score: " + score;

                        gameOverLayout.IsVisible = true;
                    }

                //TEST OUTPUTS
                    placeHolder.Text = $"Speed: {speed:F2} Accel: {acceleration:F2} Boost: {boostMultiplier:F1} Fuel {myP.fuel}";
                    OBBPlaceHolder.Text = $"OBB Center: ({myP.objectOBB.Center.X:F2}, {myP.objectOBB.Center.Y:F2})\n" +
                        $"Width: {myP.objectOBB.Width:F2} Height: {myP.objectOBB.Height:F2}\n" +
                        $"Rotation: {rp:F2}°";


                    OBBPlaceHolder2.Text = $"OBB Center: ({enemyTest.objectOBB.Center.X:F2}, {enemyTest.objectOBB.Center.Y:F2})\n" +
                        $"Width: {enemyTest.objectOBB.Width:F2} Height: {enemyTest.objectOBB.Height:F2}\n" +
                        $"Rotation: {rp:F2}°\nCollision: {collision}";

                

                //ACTUAL LAYOUT UPDATES
                    updateFuelMeter(myP.fuel);
                    updateTimerLabel(); // also updates boost VARIABLE but not LABEL.
                    updateBoostMeter(myP.boostAmount);

                    //it would be nice to have this in its own class but  the weird
                    //mix between player and MainPage is forced upon me by the layout system, curses.
                    mapLayout.TranslationX = xp;
                    mapLayout.TranslationY = yp;
                    rotateLayout.Rotation = rp;

                    if (invincibleSkin) { //we use needToChange to ensure that it only changes once per state.
                                          //not every frame per strate.
                        if (needToChange == 1) {
                            myP.changeImage("car41.png");
                            needToChange = 0;
                        }
                    } else {
                        if (needToChange == 0) {
                            myP.changeImage("car31.png");
                            needToChange = 1;
                        }
                    }

                    //update enemies on UI
                    for (int i = 0; i < enemies.Count; i++) {
                        enemies[i].updateUi((float)xp, (float)yp, rp, (float)gameLayoutWidth, (float)gameLayoutHeight, playerX, playerY);
                    }

                    //rather clear what this does.
                    if (toRemove.Count > 0) {
                        for (int i = 0; i < toRemove.Count; i++) {
                            mapLayout.Children.Remove(toRemove[i].gameObjectLayout);
                            if (toRemove[i] is enemy) {
                                enemies.Remove((enemy)toRemove[i]);
                            }
                        }
                    }


                });
        }

        protected override void OnDisappearing() {
            base.OnDisappearing();
            gameTimer.Stop();
            timeTimer.Stop();
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
            if (this.Window != null) {
                KeyHook.Attach(this.Window, keyHandler);
            }
            #endif

            // Initialize layouts and images

            //This is the overaraching layout that contains all other layouts
            if (onceOnAppearing) {
                SetupGameLayout();
                onceOnAppearing = false;
                //we only place layouts to map layout here
                mapLayout.SizeChanged += (s, e) => {

                    playerX = (float)(mapLayout.Width / 2);
                    playerY = (float)(mapLayout.Height / 2);

                };

                gameLayout.SizeChanged += (s, e) => {
                    //ONLY ASSIGN GAME LAYOUT WIDTH/HEIGHT IN HERE AS IT IS INVALID IN THE SETUP LAYOUT
                    if (myP == null) {

                        gameLayoutWidth = gameLayout.Width;
                        gameLayoutHeight = gameLayout.Height;

                        statsLayout = new AbsoluteLayout {
                            BackgroundColor = Colors.Transparent,
                            WidthRequest = gameLayoutWidth,
                            HeightRequest = gameLayoutHeight
                        };


                        //this positions the stats layout to cover the entire game layout
                        AbsoluteLayout.SetLayoutBounds(statsLayout, new Rect(0, 0, statsLayout.WidthRequest, statsLayout.HeightRequest));
                        AbsoluteLayout.SetLayoutFlags(statsLayout, AbsoluteLayoutFlags.None);

                        setUpFuelLayout();
                        setUpTimerLayout();
                        setUpBoostLayout();

                        //TO BE DELETED LATER - JUST FOR TESTING OBB VALUES
                        AbsoluteLayout.SetLayoutBounds(OBBPlaceHolder, new Rect(0, 25, statsLayout.WidthRequest, statsLayout.HeightRequest));
                        AbsoluteLayout.SetLayoutFlags(OBBPlaceHolder, AbsoluteLayoutFlags.None);

                        AbsoluteLayout.SetLayoutBounds(OBBPlaceHolder2, new Rect(0, 75, statsLayout.WidthRequest, statsLayout.HeightRequest));
                        AbsoluteLayout.SetLayoutFlags(OBBPlaceHolder2, AbsoluteLayoutFlags.None);

                        statsLayout.Children.Add(fuelLayout);
                        statsLayout.Children.Add(boostLayout);

                        statsLayout.Children.Add(OBBPlaceHolder);
                        statsLayout.Children.Add(OBBPlaceHolder2);
                        statsLayout.Children.Add(placeHolder);

                        setUpGameOverLayout();

                        gameLayout.Children.Add(gameOverLayout);
                        gameLayout.Children.Add(statsLayout);



                        //CREATE GAME OBJECTS HERE AS NOW THE MAPLAYOUT IS PROPERLY SET UP AND WE HAVE VALID WIDTH/HEIGHT VALUES
                        myP = new player();
                        enemyTest = new enemy(100,100);
                        enemyTest2 = new enemy(300, 300);

                        fuelTest = new fuelObject(300,300);

                        enemies.Add(enemyTest);
                        enemies.Add(enemyTest2);

                        //ADD GAME OBJECTS TO LAYOUTS
                        gameLayout.Children.Add(myP.gameObjectLayout);// this is unique. the rest should be added to mapLayout

                        mapLayout.Children.Add(enemyTest2.gameObjectLayout);
                        mapLayout.Children.Add(enemyTest.gameObjectLayout);
                        mapLayout.Children.Add(fuelTest.gameObjectLayout);

                        //We set this up here the center of mapLayout is now valid!
                        myP.setUpOBB(new Vector2(playerX, playerY), (float)myP.imageWidth - 8, (float)myP.imageHeight - 7, 0);
                        //myP.setUpOBB(new Vector2(playerX, playerY),32, 70, 0);
                        setUpTimer();
                        startTimeTimer();


                        //DisplayAlert("Debug", $"gameLayout measured size: {gameLayout.Width} x {gameLayout.Height}", "OK");
                    }
                };

            }

        }

        //this is an ugly method that sets up all the layouts for the game
        //rotten looking to say the least.
        public void SetupGameLayout() {

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

            gameLayout.Children.Add(statsLayout);

            //this sets the content of the page to the game layout i.e the overarching layout
            gameLayout.IsClippedToBounds = true;
            Content = gameLayout;

            gameLayoutWidth = gameLayout.WidthRequest;
            gameLayoutHeight = gameLayout.HeightRequest;
        }

        public void setUpTimerLayout() {
            timerLayout = new AbsoluteLayout {
                BackgroundColor = Colors.Transparent,
                WidthRequest = 100,
                HeightRequest = 50
            };
            timerLabel = new Label {
                FontFamily = "Consolas",
                Text = "00:00",
                FontSize = 24,
                TextColor = Colors.White,
                BackgroundColor = Colors.Transparent,
                WidthRequest = timerLayout.WidthRequest,
                HeightRequest = timerLayout.HeightRequest,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
            AbsoluteLayout.SetLayoutBounds(timerLayout, new Rect(0.5, 0.01, timerLayout.WidthRequest, timerLayout.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(timerLayout, AbsoluteLayoutFlags.PositionProportional);
            timerLayout.Children.Add(timerLabel);
            statsLayout.Children.Add(timerLayout);
        }

        int timeElapsed = 0;

        //LOGIC
        public void updateTimer(object o, EventArgs e) {
            timeElapsed++;
            
            if(boostMultiplier == 1.0) {
                if (myP.boostAmount < 100) {
                    myP.boostAmount += 2.5;
                    if (myP.boostAmount > 100) {
                        myP.boostAmount = 100;
                    }
                }
            }
            //timerLabel.Text = $"{minutes:D2}:{seconds:D2}";
        }

        //UI
        public void updateTimerLabel() {
            if(timeElapsed % 60 < 10)
                timerLabel.Text = "" + (timeElapsed / 60) + ":0" + (timeElapsed % 60);
            else
                timerLabel.Text = ""+(timeElapsed / 60) + ":" + (timeElapsed%60);
        }

        public void startTimeTimer() {
            timeTimer = new System.Timers.Timer(1000);
            timeTimer.Elapsed += updateTimer;
            timeTimer.Start();
        }

        double maxBarHeight = 200;

        public void setUpBoostLayout() {
            boostLayout = new AbsoluteLayout {
                BackgroundColor = Colors.Transparent,
                WidthRequest = 40,
                HeightRequest = maxBarHeight
            };

            boostBar = new BoxView {
                Color = Colors.Blue,
                WidthRequest = 40,
                HeightRequest = maxBarHeight
            };

            AbsoluteLayout.SetLayoutBounds(boostBar, new Rect(0, 0, boostBar.WidthRequest, boostBar.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(boostBar, AbsoluteLayoutFlags.None);

            //AbsoluteLayout.SetLayoutBounds(fuelLayout, new Rect(.95, .95, fuelLayout.WidthRequest, fuelLayout.HeightRequest));
            AbsoluteLayout.SetLayoutBounds(boostLayout, new Rect(gameLayoutWidth * .85, gameLayoutHeight * 0.48, boostLayout.WidthRequest, boostLayout.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(boostLayout, AbsoluteLayoutFlags.None);

            boostLayout.Children.Add(boostBar);

            //fuelBar.Rotation += 180;
            //fuelLayout.Rotation += 180;
        }

        public void updateBoostMeter(double boost) {
            double maxBoost = 100;
            double boostHeight = (boost / maxBoost) * maxBarHeight;

            //fuelBar.HeightRequest = fuelHeight;
            boostBar.HeightRequest = boostHeight;
            // Keep the bottom fixed
            AbsoluteLayout.SetLayoutBounds(boostBar, new Rect(
                0,
                maxBarHeight - boostHeight,
                boostBar.WidthRequest,
                boostHeight
            ));
        }

        public void setUpFuelLayout() {
            fuelLayout = new AbsoluteLayout {
                BackgroundColor = Colors.Transparent,
                WidthRequest = 40,
                HeightRequest = maxBarHeight
            };

            fuelBar = new BoxView {
                Color = Colors.Yellow,
                WidthRequest = 40,
                HeightRequest = maxBarHeight
            };

            AbsoluteLayout.SetLayoutBounds(fuelBar, new Rect(0, 0, fuelBar.WidthRequest, fuelBar.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(fuelBar, AbsoluteLayoutFlags.None);

            //AbsoluteLayout.SetLayoutBounds(fuelLayout, new Rect(.95, .95, fuelLayout.WidthRequest, fuelLayout.HeightRequest));
            AbsoluteLayout.SetLayoutBounds(fuelLayout, new Rect(gameLayoutWidth * .90, gameLayoutHeight * 0.48, fuelLayout.WidthRequest, fuelLayout.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(fuelLayout, AbsoluteLayoutFlags.None);
        
            fuelLayout.Children.Add(fuelBar);

            //fuelBar.Rotation += 180;
            //fuelLayout.Rotation += 180;
        }

        public void updateFuelMeter(double fuel) {
            double maxFuel = 100;
            double fuelHeight = (fuel / maxFuel) * maxBarHeight;

            //fuelBar.HeightRequest = fuelHeight;
            fuelBar.HeightRequest = fuelHeight;
            // Keep the bottom fixed
            AbsoluteLayout.SetLayoutBounds(fuelBar, new Rect(
                0,
                maxBarHeight - fuelHeight,
                fuelBar.WidthRequest,
                fuelHeight
            )); // Maui deciding to use proportional layout flags even if i specify not to is seriously annoying
        }       // hate this, highly unintuitive system at all.
    }
}
