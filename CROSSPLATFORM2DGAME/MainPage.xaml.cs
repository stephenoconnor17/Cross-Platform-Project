using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using Microsoft.UI.Xaml.Input;
using System.Data;
using System.Diagnostics;
using System.Numerics;
using System.Timers;
using Plugin.Maui.Audio;


namespace CROSSPLATFORM2DGAME {
    public partial class MainPage : ContentPage {

        //AUDIO
        // private IAudioPlayer backgroundMusicPlayer;
        AudioHandler myAudioHandler;

        //KEYHANDLER
        KeyHandler keyHandler;

        //GAME TIMER
        System.Timers.Timer gameTimer;

        //MAP GENERATOR
        mapGenerator mg;

        //LAYOUT / VIEWS
        AbsoluteLayout startLayout;
        Label startTitle;
        Button startButton;
        Button settingsButton;

        AbsoluteLayout gameLayout;
        AbsoluteLayout mapLayout;
        public static double mapLayoutWidth = 0;
        public static double mapLayoutHeight = 0;

        AbsoluteLayout rotateLayout;

        AbsoluteLayout gameOverLayout;
        Label gameOverLabel1;
        Label gameOverLabel2;
        Label gameOverLabel3;
        Button gameOverButton;
        Button gameOverButton2;
        int highestScore = 0;
        public static int score = 0; //static because it gets added to from within other objects when they die.

        AbsoluteLayout statsLayout;
        Label scoreLabel;

        AbsoluteLayout fuelLayout;
        BoxView fuelBar;
        Label fuelLabel;

        AbsoluteLayout healthLayout;
        Image h1, h2, h3, h4, h5;

        AbsoluteLayout boostLayout;
        BoxView boostBar;
        Label boostLabel;

        AbsoluteLayout timerLayout;
        Label timerLabel;
        System.Timers.Timer timeTimer;

        Label placeHolder;
        Label OBBPlaceHolder;
        Label OBBPlaceHolder2;
        
        //GAME OBJECTS
        player myP;
        //List<enemy> enemies;

       // wallObject wallTest;
        lootObject lootTest;
        fuelObject fuelTest;
        List<gameObject> toRemove;

        //VARIABLES
        public static double gameLayoutWidth;
        public static double gameLayoutHeight;
        public float playerX = 0;
        public float playerY = 0;
        public MainPage() {
            InitializeComponent();
            this.BackgroundColor = Colors.DeepSkyBlue;          
            keyHandler = new KeyHandler();
            toRemove = new List<gameObject>();

            myAudioHandler = new AudioHandler();
        }

        

        //SET UP GAME TIMER
        public void setUpTimer() {
            gameTimer = new System.Timers.Timer(16);
            gameTimer.Elapsed += GameTimer_Elapsed;
            gameTimer.Start();
        }



        public void restartGame() {
            // Stop timers FIRST - this is critical to prevent race conditions
            if (gameTimer != null && gameTimer.Enabled) {
                gameTimer.Stop();
            }
            if (timeTimer != null && timeTimer.Enabled) {
                timeTimer.Stop();
            }

            // Wait a moment to ensure the game loop has fully stopped
            System.Threading.Thread.Sleep(50); // Give the timer threads time to finish

            // Reset game state variables
            gameOver = false;
            xp = 0;
            yp = 0;
            rp = 0;
            speed = 0.0;
            acceleration = 0.0;
            boostMultiplier = 1.0;
            isBoosting = false;
            backFrame = 0;
            outOfBoundsFrame = 0;
            invincibleSkin = false;
            invincibleFrame = 0;
            needToChange = 0;
            timeElapsed = 0;
            previousMinute = 0;

            // Update high score if needed
            if (score > highestScore) {
                highestScore = score;
                gameOverLabel2.Text = "HIGH SCORE: " + highestScore;
            }

            // Reset score
            score = 0;

            // Clear existing game objects from mapLayout
            if (mg != null) {
                // Clear OBB lists BEFORE clearing enemies to prevent null reference
                OBBHandler.staticOBBs.Clear();
                OBBHandler.movingOBBs.Clear();

                // Now clear enemies
                mg.enemies.Clear();

                // Clear map layout children
                mapLayout.Children.Clear();
            }

            // Clear toRemove list
            toRemove.Clear();

            // Reset player
            if (myP != null) {
                myP.fuel = 100;
                myP.boostAmount = 100;
                myP.lives = 5;
                myP.changeImage("car31.png");

                // Reset player OBB position
                myP.objectOBB.Update(new Vector2(playerX, playerY), 0);
            }

            // Recreate map generator with fresh state
            mg = new mapGenerator(mapLayout, mapLayoutWidth, mapLayoutHeight);

            // Reset UI layouts
            mapLayout.TranslationX = 0;
            mapLayout.TranslationY = 0;
            rotateLayout.Rotation = 0;

            // Hide game over layout
            gameOverLayout.IsVisible = false;
            statsLayout.Opacity = 1.0;
            statsLayout.InputTransparent = false; // Restore normal input
            timerLayout.Opacity = 1.0;

            // Update all UI elements to initial state
            updateHealthLayout();
            updateFuelMeter(myP.fuel);
            updateBoostMeter(myP.boostAmount);
            updateScoreLabel();
            timerLabel.Text = "00:00";

            // Restart timers LAST - after everything is reset
            setUpTimer();
            startTimeTimer();
        }

        public void setUpGameOverLayout() {
            gameOverLayout = new AbsoluteLayout {
                BackgroundColor = Color.FromArgb("#80000000"),
                WidthRequest = gameLayoutWidth,
                HeightRequest = gameLayoutHeight,
                Opacity = 0.8,
                InputTransparent = false
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

            gameOverButton = new Button {
                WidthRequest = gameLayoutWidth / 8,
                HeightRequest = gameLayoutHeight / 16,
                BackgroundColor = Colors.Transparent,
                TextColor = Colors.White,
                Text = "RESTART",
                FontFamily = "Consolas",
                InputTransparent = false
            };

            gameOverButton2 = new Button {
                WidthRequest = gameLayoutWidth / 8,
                HeightRequest = gameLayoutHeight / 16,
                BackgroundColor = Colors.Transparent,
                TextColor = Colors.White,
                Text = "EXIT",
                FontFamily = "Consolas",
                InputTransparent = false
            };


            AbsoluteLayout.SetLayoutBounds(gameOverButton, new Rect(0.4, 0.75, gameOverButton.WidthRequest, gameOverButton.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(gameOverButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(gameOverButton2, new Rect(0.6, 0.75, gameOverButton2.WidthRequest, gameOverButton2.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(gameOverButton2, AbsoluteLayoutFlags.PositionProportional);

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
            gameOverLayout.Children.Add(gameOverButton);
            gameOverLayout.Children.Add(gameOverButton2);

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
        int outOfBoundsFrame = 0;

        bool invincibleSkin = false;
        int invincibleFrame = 0;
        int needToChange = 0;

        //THE GAME LOOP
        public void GameTimer_Elapsed(object sender, ElapsedEventArgs e) {
            double rad = rp * Math.PI / 180.0; // convert rotation to radians

            for(int i = mg.enemies.Count - 1; i >= 0; i--) {
                if (mg.enemies[i].removeThis == true) {
                    OBBHandler.movingOBBs.Remove(mg.enemies[i].objectOBB);
                    toRemove.Add(mg.enemies[i]);
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
                        myAudioHandler.playSoundEffect("fuel");
                        toRemove.Add(OBBHandler.staticOBBs[i].thisObject);
                        OBBHandler.staticOBBs.RemoveAt(i);
                        mg.RemoveObject(OBBHandler.staticOBBs[i].thisObject);
                    }
                    //LOOT PICKUP DETECTION
                    else if (OBBHandler.staticOBBs[i].objectType == "loot") {
                        score += 200;
                        myAudioHandler.playSoundEffect("loot");
                        toRemove.Add(OBBHandler.staticOBBs[i].thisObject);
                        OBBHandler.staticOBBs.RemoveAt(i);
                        mg.RemoveObject(OBBHandler.staticOBBs[i].thisObject);
                    }
                    //WALL COLLISION
                    else if (OBBHandler.staticOBBs[i].objectType == "wall") {
                        collision = true;
                        backFrame = 60;
                        myAudioHandler.playSoundEffect("collision");

                    } else if (OBBHandler.staticOBBs[i].objectType == "health") {
                        if (myP.lives < 5) {
                            myP.lives += 2;
                            myAudioHandler.playSoundEffect("health");
                            if (myP.lives > 5) {
                                myP.lives = 5;
                            }
                            mg.RemoveObject(OBBHandler.staticOBBs[i].thisObject);
                            toRemove.Add(OBBHandler.staticOBBs[i].thisObject);
                            OBBHandler.staticOBBs.RemoveAt(i);
                        }
                    }

                   
                }
            }


            //MOVING COLLISION DETECTION
            //check for collision if not invincible i.e if invincibleFrame > 0, invincible, dont bother check collision.
            if (invincibleFrame <= 0) {
                for (int i = OBBHandler.movingOBBs.Count - 1; i >= 0; i--) {
                    if (myP.objectOBB.Intersects(OBBHandler.movingOBBs[i])) {
                        if (!collision) {
                            //collisionJustHappened = true; // only set to true on first collision detection
                            myAudioHandler.playSoundEffect("collision");
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

            //PLAYER OUT OF BOUNDS CHECK
            //We use Corners instead of OBB.intersects here because well 
            //its out of bounds general, its x and y are in a certain range.
            bool outOfBounds = false;
            foreach (var corner in myP.objectOBB.Corners) {
                if ((corner.X < 0 || corner.X > mapLayoutWidth || corner.Y < 0 || corner.Y > mapLayoutHeight) && outOfBoundsFrame <= 0) {
                    outOfBounds = true;
                    myAudioHandler.playSoundEffect("collision");
                    outOfBoundsFrame = 15;
                    break;
                }
            }

            //add collision if out of bounds
            if (outOfBounds && outOfBoundsFrame == 15) {
                
                collision = true;
                backFrame = 60;
            }

            if(outOfBoundsFrame > 0) {
                outOfBoundsFrame--;
            }

            //we do this so that it only calculates collision math once.
            if (collision) {
                if (Math.Abs(speed) > 0 && backFrame == 60) { //collisionJustHappened ensures we dont continously invert speed every frame
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
            // OBB UPDATE
            myP.objectOBB.Update(new Vector2(playerX - (float)xp, playerY - (float)yp), rp * Math.PI / 180.0);
           
            for (int i = 0; i < mg.enemies.Count; i++) {
                mg.enemies[i].update(playerX - (float)xp, playerY - (float)yp);
            }

            //dont know why everything is tabbed forward.
            //will tab back when cleaning code.
                //UI UPDATES
                //SINCE GameTimer_Elapsed IS A BACKGROUND THREAD WE MUST INVOKE ON MAIN THREAD TO UPDATE UI ELEMENTS
                MainThread.BeginInvokeOnMainThread(() => {
                    
                    //compute gameOver stuff here graphically.
                    if (gameOver) {
                        statsLayout.Opacity = 0.8;
                        timerLayout.Opacity = 0.8;

                        gameOverLabel3.Text = "Score: " + score;

                        statsLayout.InputTransparent = true;
                        gameOverLayout.IsVisible = true;
                    }

                //TEST OUTPUTS
                /*
                    placeHolder.Text = $"Speed: {speed:F2} Accel: {acceleration:F2} Boost: {boostMultiplier:F1} Fuel {myP.fuel}";
                    OBBPlaceHolder.Text = $"OBB Center: ({myP.objectOBB.Center.X:F2}, {myP.objectOBB.Center.Y:F2})\n" +
                        $"Width: {myP.objectOBB.Width:F2} Height: {myP.objectOBB.Height:F2}\n" +
                        $"Rotation: {rp:F2}°";

                    
                    OBBPlaceHolder2.Text = $"OBB Center: ({enemyTest.objectOBB.Center.X:F2}, {enemyTest.objectOBB.Center.Y:F2})\n" +
                        $"Width: {enemyTest.objectOBB.Width:F2} Height: {enemyTest.objectOBB.Height:F2}\n" +
                        $"Rotation: {rp:F2}°\nCollision: {collision}";
                    
                    if (wallTest != null && wallTest.objectOBB != null) {
                        OBBPlaceHolder2.Text = $"Wall OBB: ({wallTest.objectOBB.Center.X:F2}, {wallTest.objectOBB.Center.Y:F2})\n" +
                                               $"Player OBB: ({myP.objectOBB.Center.X:F2}, {myP.objectOBB.Center.Y:F2})\n" +
                                               $"Distance: {Vector2.Distance(wallTest.objectOBB.Center, myP.objectOBB.Center):F2}\n" +
                                               $"Intersects: {myP.objectOBB.Intersects(wallTest.objectOBB)}\n" +
                                               $"ObjectType: {wallTest.objectOBB.objectType}";
                    }
                */

                    //ACTUAL LAYOUT UPDATES
                    updateHealthLayout();
                    updateFuelMeter(myP.fuel);
                    updateTimerLabel(); // also updates boost VARIABLE but not LABEL.
                    updateBoostMeter(myP.boostAmount);
                    updateScoreLabel();

                    //it would be nice to have this in its own class but  the weird
                    //mix between player and MainPage is forced upon me by the layout system, curses.
                    mapLayout.TranslationX = xp;
                    mapLayout.TranslationY = yp;
                    rotateLayout.Rotation = rp;

                    if (invincibleSkin) { //we use needToChange to ensure that it only changes once per state.
                                          //not every frame per state.
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
                    for (int i = 0; i < mg.enemies.Count; i++) {
                        mg.enemies[i].updateUi((float)xp, (float)yp, rp, (float)gameLayoutWidth, (float)gameLayoutHeight, playerX, playerY);
                    }

                    //rather clear what this does.
                    if (toRemove.Count > 0) {
                        for (int i = 0; i < toRemove.Count; i++) {
                            mapLayout.Children.Remove(toRemove[i].gameObjectLayout);
                            if (toRemove[i] is enemy) {
                                mg.enemies.Remove((enemy)toRemove[i]);
                            }
                        }
                    }
                });
        }

        protected override void OnDisappearing() {
            base.OnDisappearing();
            if (gameTimer != null) {
                gameTimer.Stop();
            }

            if (timeTimer != null) {
                timeTimer.Stop();
            }
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
                        setUpHealthLayout();
                        setUpScoreLabel();

                        myAudioHandler.playBackgroundMusic();

                        //TO BE DELETED LATER - JUST FOR TESTING OBB VALUES
                        /*
                        AbsoluteLayout.SetLayoutBounds(OBBPlaceHolder, new Rect(0, 25, statsLayout.WidthRequest, statsLayout.HeightRequest));
                        AbsoluteLayout.SetLayoutFlags(OBBPlaceHolder, AbsoluteLayoutFlags.None);

                        AbsoluteLayout.SetLayoutBounds(OBBPlaceHolder2, new Rect(0, 75, statsLayout.WidthRequest, statsLayout.HeightRequest));
                        AbsoluteLayout.SetLayoutFlags(OBBPlaceHolder2, AbsoluteLayoutFlags.None);

                        */

                        statsLayout.Children.Add(scoreLabel);
                        statsLayout.Children.Add(fuelLayout);
                        statsLayout.Children.Add(boostLayout);
                        statsLayout.Children.Add(healthLayout);

                        /*
                        statsLayout.Children.Add(OBBPlaceHolder);
                        statsLayout.Children.Add(OBBPlaceHolder2);
                        statsLayout.Children.Add(placeHolder);
                        */

                        setUpStartLayout();
                        setUpGameOverLayout();

                        gameOverButton.Clicked += (sender, args) => {
                            restartGame();
                        };

                        gameOverButton2.Clicked += (sender, args) => {
                            if (gameTimer != null) gameTimer.Stop();
                            if (timeTimer != null) timeTimer.Stop();

                            // Return to start screen
                            gameOverLayout.IsVisible = false;
                            mapLayout.IsVisible = false;
                            statsLayout.IsVisible = false;
                            rotateLayout.IsVisible = false;
                            myP.gameObjectLayout.IsVisible = false;
                            startLayout.IsVisible = true;
                        };

                        gameLayout.Children.Add(startLayout);
                        gameLayout.Children.Add(gameOverLayout);
                        gameLayout.Children.Add(statsLayout);

                        //CREATE GAME OBJECTS HERE AS NOW THE MAPLAYOUT IS PROPERLY SET UP AND WE HAVE VALID WIDTH/HEIGHT VALUES
                        myP = new player();
                        //ADD GAME OBJECTS TO LAYOUTS
                        gameLayout.Children.Add(myP.gameObjectLayout);// this is unique. the rest should be added to mapLayout
                        //We set this up here the center of mapLayout is now valid!
                        statsLayout.InputTransparent = true; //so clicks go through to gameOverLayout when visible
                        myP.setUpOBB(new Vector2(playerX, playerY), (float)myP.imageWidth - 8, (float)myP.imageHeight - 7, 0);
                        //myP.setUpOBB(new Vector2(playerX, playerY),32, 70, 0);
                        mapLayout.IsVisible = false;

                        statsLayout.IsVisible = false;
                        rotateLayout.IsVisible = false;
                        myP.gameObjectLayout.IsVisible = false;
                        
                        //DisplayAlert("Debug", $"gameLayout measured size: {gameLayout.Width} x {gameLayout.Height}", "OK");
                    }
                };

            } 

        }

        public void setUpStartLayout() {
            startLayout = new AbsoluteLayout {
                WidthRequest = gameLayoutWidth,
                HeightRequest = gameLayoutHeight,
                BackgroundColor = Colors.Gray
            };

            startButton = new Button {
                BackgroundColor = Colors.DarkGray,
                Text = "Start",
                FontFamily = "Consolas",
            };

            double shortestSide = Math.Min(gameLayoutWidth, gameLayoutHeight);

            double fontSize = shortestSide * 0.20; // 8% of screen

            startTitle = new Label {
                Text = "driv.r", //TITLE FOR THE Game ! 
                FontFamily = "Consolas",
                FontSize = fontSize
            };

            AbsoluteLayout.SetLayoutBounds(startTitle, new Rect(.5,.2,300,300));
            AbsoluteLayout.SetLayoutFlags(startTitle, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(startLayout, new Rect(0,0, gameLayoutWidth, gameLayoutHeight));
            AbsoluteLayout.SetLayoutFlags(startLayout, AbsoluteLayoutFlags.None);

            AbsoluteLayout.SetLayoutBounds(startButton, new Rect(.5, .5, 100, 50));
            AbsoluteLayout.SetLayoutFlags(startButton, AbsoluteLayoutFlags.PositionProportional);

            startButton.Clicked += startButton_Clicked;

            startLayout.Children.Add(startTitle);
            startLayout.Children.Add(startButton);
        }
        public void startButton_Clicked(object o, EventArgs e) {
            startLayout.IsVisible = false;
            rotateLayout.IsVisible = true;
            mapLayout.IsVisible = true;
            statsLayout.IsVisible = true;
            statsLayout.InputTransparent = false; // ADD THIS - allow stats interaction during normal gameplay
            myP.gameObjectLayout.IsVisible = true;

            mg = new mapGenerator(mapLayout, mapLayoutWidth, mapLayoutHeight);

            setUpTimer();
            startTimeTimer();
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
                WidthRequest = 3200,
                HeightRequest = 1600,
               

            };

            mapLayoutWidth = mapLayout.WidthRequest;
            mapLayoutHeight = mapLayout.HeightRequest;

            rotateLayout = new AbsoluteLayout {
                BackgroundColor = Colors.Transparent,
                WidthRequest = gameLayout.WidthRequest,
                HeightRequest = gameLayout.HeightRequest
            };

            //this will hold the stats like health fuel time etc
           
            /*
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
            */

            //this centers the map layout in the game layout
            AbsoluteLayout.SetLayoutBounds(mapLayout, new Rect(0.5, 0.5, mapLayout.WidthRequest, mapLayout.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(mapLayout, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(rotateLayout, new Rect(0.5, 0.5, gameLayout.WidthRequest, gameLayout.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(rotateLayout, AbsoluteLayoutFlags.PositionProportional);

            rotateLayout.Children.Add(mapLayout);
            gameLayout.Children.Add(rotateLayout);

            gameLayout.Children.Add(statsLayout);

            //this makes it so the content doesnt clip outside the game layout bounds
            gameLayout.IsClippedToBounds = true;
            Content = gameLayout;

            gameLayoutWidth = gameLayout.WidthRequest;
            gameLayoutHeight = gameLayout.HeightRequest;
        }

        public void updateScoreLabel() {
            scoreLabel.Text = "SCORE: " + score;
        }

        public void setUpScoreLabel() {
            scoreLabel = new Label {
                FontFamily = "Consolas",
                Text = "SCORE: 0",
                FontSize = 18,
                TextColor = Colors.White,
                BackgroundColor = Colors.Transparent,
                WidthRequest = 150,
                HeightRequest = 30,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };

            AbsoluteLayout.SetLayoutBounds(scoreLabel, new Rect(0.001, 0.005, scoreLabel.WidthRequest, scoreLabel.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(scoreLabel, AbsoluteLayoutFlags.PositionProportional);

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

        //TIME TIMER LOGIC
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
        }

        //TIME TIMER UI
        int previousMinute = 0;
        public void updateTimerLabel() {
            int currentMinute = timeElapsed / 60;

            if (timeElapsed % 60 < 10)
                timerLabel.Text = "" + (timeElapsed / 60) + ":0" + (timeElapsed % 60);
            else
                timerLabel.Text = ""+(timeElapsed / 60) + ":" + (timeElapsed%60);

            //HAS TO BE DONE HERE BECAUSE MAIN THREAD INVOKE IS WHATS NEEDED.
            if (timeElapsed % 60 == 0 && timeElapsed >= 60 && previousMinute < currentMinute) {
                previousMinute = currentMinute;
                currentMinute++;
                mg.spawnEnemies(timeElapsed / 60 + 3);
                mg.spawnItems();
            }
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

            boostLabel = new Label {
                Text = "BOOST",
                FontFamily = "Consolas",
                FontSize = 14,
                TextColor = Colors.White,
                BackgroundColor = Colors.Transparent,
                WidthRequest = boostLayout.WidthRequest,
                HeightRequest = 20,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };

            AbsoluteLayout.SetLayoutBounds(boostBar, new Rect(0, 0, boostBar.WidthRequest, boostBar.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(boostBar, AbsoluteLayoutFlags.None);

            AbsoluteLayout.SetLayoutBounds(boostLabel, new Rect(0, boostBar.HeightRequest, boostLayout.WidthRequest, 20));
            AbsoluteLayout.SetLayoutFlags(boostLabel, AbsoluteLayoutFlags.None);

            //AbsoluteLayout.SetLayoutBounds(fuelLayout, new Rect(.95, .95, fuelLayout.WidthRequest, fuelLayout.HeightRequest));
            AbsoluteLayout.SetLayoutBounds(boostLayout, new Rect(gameLayoutWidth * .85, gameLayoutHeight * 0.48, boostLayout.WidthRequest, boostLayout.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(boostLayout, AbsoluteLayoutFlags.None);

            boostLayout.Children.Add(boostLabel);
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

        bool healthInvulnerable = false;
        public void updateHealthLayout() {
            //we need this switch because
            //when we regain health we need to make hearts visible again     

            switch (myP.lives) {
                case 5:
                    h5.IsVisible = true;
                    h4.IsVisible = true;
                    h3.IsVisible = true;
                    h2.IsVisible = true;
                    h1.IsVisible = true;
                    break;
                case 4:
                    h5.IsVisible = false;
                    h4.IsVisible = true;
                    h3.IsVisible = true;
                    h2.IsVisible = true;
                    h1.IsVisible = true;
                    break;
                case 3:
                    h4.IsVisible = false;
                    h3.IsVisible = true;
                    h2.IsVisible = true;
                    h1.IsVisible = true;
                    break;
                case 2:
                    h3.IsVisible = false;
                    h2.IsVisible = true;
                    h1.IsVisible = true;
                    break;
                case 1:
                    h2.IsVisible = false;
                    h1.IsVisible = true;
                    break;
                case 0:
                    h1.IsVisible = false;
                    break;
            }

            if (invincibleFrame > 0) {
                if (!healthInvulnerable) {
                    h1.Source = "heart2.png";
                    h2.Source = "heart2.png";
                    h3.Source = "heart2.png";
                    h4.Source = "heart2.png";
                    h5.Source = "heart2.png";
                    healthInvulnerable = true;
                }
            } else if (healthInvulnerable) {
                h1.Source = "heart1.png";
                h2.Source = "heart1.png";
                h3.Source = "heart1.png";
                h4.Source = "heart1.png";
                h5.Source = "heart1.png";
                healthInvulnerable = false;
            }
        }

        public void setUpHealthLayout() {
            healthLayout = new AbsoluteLayout {
                BackgroundColor = Colors.Transparent,
                WidthRequest = gameLayoutWidth / 6,
                HeightRequest = gameLayoutHeight / 12
            };

            h1 = new Image {
                Source = "heart1.png",
            };

            h2 = new Image {
                Source = "heart1.png",
            };

            h3 = new Image {
                Source = "heart1.png",
            };

            h4 = new Image {
                Source = "heart1.png",
            };

            h5 = new Image {
                Source = "heart1.png",
            };

            AbsoluteLayout.SetLayoutBounds(healthLayout, new Rect(0.025, .90, healthLayout.WidthRequest, healthLayout.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(healthLayout, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(h1, new Rect(0 ,0, 48, 48));
            AbsoluteLayout.SetLayoutBounds(h2, new Rect(healthLayout.WidthRequest / 5 * 1, 0, 48, 48));
            AbsoluteLayout.SetLayoutBounds(h3, new Rect(healthLayout.WidthRequest / 5 * 2, 0, 48, 48));
            AbsoluteLayout.SetLayoutBounds(h4, new Rect(healthLayout.WidthRequest / 5 * 3, 0, 48, 48));
            AbsoluteLayout.SetLayoutBounds(h5, new Rect(healthLayout.WidthRequest / 5 * 4, 0, 48, 48));

            healthLayout.Children.Add(h1);
            healthLayout.Children.Add(h2);
            healthLayout.Children.Add(h3);
            healthLayout.Children.Add(h4);
            healthLayout.Children.Add(h5);
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

            fuelLabel = new Label {
                Text = "FUEL",
                FontFamily = "Consolas",
                FontSize = 14,
                TextColor = Colors.White,
                BackgroundColor = Colors.Transparent,
                WidthRequest = fuelLayout.WidthRequest,
                HeightRequest = 20,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };

            AbsoluteLayout.SetLayoutBounds(fuelBar, new Rect(0, 0, fuelBar.WidthRequest, fuelBar.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(fuelBar, AbsoluteLayoutFlags.None);

            AbsoluteLayout.SetLayoutBounds(fuelLabel, new Rect(0, fuelBar.HeightRequest, fuelLayout.WidthRequest, 20));
            AbsoluteLayout.SetLayoutFlags(fuelLabel, AbsoluteLayoutFlags.None);
            //AbsoluteLayout.SetLayoutBounds(fuelLayout, new Rect(.95, .95, fuelLayout.WidthRequest, fuelLayout.HeightRequest));
            AbsoluteLayout.SetLayoutBounds(fuelLayout, new Rect(gameLayoutWidth * .90, gameLayoutHeight * 0.48, fuelLayout.WidthRequest, fuelLayout.HeightRequest));
            AbsoluteLayout.SetLayoutFlags(fuelLayout, AbsoluteLayoutFlags.None);
        
            fuelLayout.Children.Add(fuelBar);
            fuelLayout.Children.Add(fuelLabel);
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
    }           // maybe it is i who is highly unintuitive.
}
