using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CROSSPLATFORM2DGAME {
    public class enemy : gameObject {
        int enemyRank;
        double fuel;
        public float enemyOBBCenterX;
        public float enemyOBBCenterY;
        public double rotation;

        public double directionRotation;
        public float directionCenterX;
        public float directionCenterY;

        public bool hasBeenKilled = false; //this because the add to score gets more than once.
        public bool removeThis = false;


        public enemy(double globalX, double globalY) {
            fuel = 100;

            this.rotation = 0;
            createLayout(40, 77);
            createImage("car51.png", 40, 77);

            double enemyCenterX = layoutWidth / 2 - imageWidth / 2;
            double enemyCenterY = layoutHeight / 2 - imageHeight / 2;

            //AbsoluteLayout.SetLayoutBounds(gameObjectImage, new Rect(playerCenterX, playerCenterY, gameObjectImage.WidthRequest, gameObjectImage.HeightRequest));
            /* setLayoutPosition(MainPage.gameLayoutWidth / 2 - layoutWidth / 2,
                         MainPage.gameLayoutHeight / 2 - layoutHeight / 2,
                         layoutWidth,
                         layoutHeight);*/
            //setImagePosition(enemyCenterX, enemyCenterY, imageWidth, imageHeight);
            setImagePosition(0, 0, imageWidth, imageHeight);

            //temp for testing.
            this.globalX = globalX;
            this.globalY = globalY;

            setLayoutPosition(globalX,
                        globalY,
                        layoutWidth,
                        layoutHeight);

            enemyOBBCenterX = (float)globalX + (float)layoutWidth / 2;
            enemyOBBCenterY = (float)globalY + (float)layoutHeight / 2;

            setUpOBB(new Vector2(enemyOBBCenterX, enemyOBBCenterY), (float)layoutWidth - 8, (float)layoutHeight - 7, rotation, this); //-8/-7 to take away some of the empty space around the OBB, only for enemies;
            //setUpOBB(new Vector2(enemyOBBCenterX, enemyOBBCenterY), 32,, 0);
            OBBHandler.movingOBBs.Add(this.objectOBB);//IT MOVE SO MOVING OBB LIST    
        }

        bool turnLeft = false;
        bool turnRight = false;
        double speed = 3.75;

        double dx;
        double dy;

        int backframe = 0;
        public bool collision = false;

        /*
        public void update(double playerCenterX, double playerCenterY) {
        */
        private double targetCenterX;
        private double targetCenterY;
        private int updateTargetCounter = 0;
        private int updateTargetDelay = 30; // Update every 30 frames (~0.5 sec)

        public void update(double playerCenterX, double playerCenterY) {
            // Only update target periodically
            updateTargetCounter++;
            if (updateTargetCounter >= updateTargetDelay) {
                targetCenterX = playerCenterX;
                targetCenterY = playerCenterY;
                updateTargetCounter = 0;
            }

            // --- angle to player ---
            double angleToPlayer = Math.Atan2(targetCenterY - enemyOBBCenterY, targetCenterX - enemyOBBCenterX) * 180.0 / Math.PI;
            angleToPlayer += 90.0;
            angleToPlayer = NormalizeAngle(angleToPlayer);

            // --- smallest signed difference ---
            double diff = angleToPlayer - NormalizeAngle(this.rotation);
            diff = NormalizeSigned(diff);

            // --- COLLISION WITH PLAYER ---
            if (collision) {
                if (backframe == 0) {
                    backframe = 60;
                }
                speed = -speed;
                collision = false;
            }

            // --- CHECK COLLISION WITH OTHER ENEMIES ---
            bool enemyCollision = false;
            double avoidanceAngle = 0;

            for (int i = 0; i < OBBHandler.movingOBBs.Count; i++) {
                // Don't check collision with self
                if (OBBHandler.movingOBBs[i] == this.objectOBB)
                    continue;

                // Check if we're colliding or about to collide
                if (this.objectOBB.Intersects(OBBHandler.movingOBBs[i])) {
                    enemyCollision = true;

                    // Calculate angle AWAY from the other enemy
                    enemy otherEnemy = OBBHandler.movingOBBs[i].thisEnemy;
                    double angleAway = Math.Atan2(
                        enemyOBBCenterY - otherEnemy.enemyOBBCenterY,
                        enemyOBBCenterX - otherEnemy.enemyOBBCenterX
                    ) * 180.0 / Math.PI;
                    angleAway += 90.0;
                    avoidanceAngle = NormalizeAngle(angleAway);
                    break; // Only avoid one enemy at a time
                }
            }
            for (int i = 0; i < OBBHandler.staticOBBs.Count; i++) {

                // Check if we're colliding or about to collide
                if (this.objectOBB.Intersects(OBBHandler.staticOBBs[i])) {
                    if (OBBHandler.staticOBBs[i].objectType == "wall") {
                        enemyCollision = true;

                        // Calculate angle AWAY from the other enemy
                        gameObject otherObject = OBBHandler.staticOBBs[i].thisObject;
                        double angleAway = Math.Atan2(this.objectOBB.Center.Y  - otherObject.objectOBB.Center.Y, this.objectOBB.Center.X - otherObject.objectOBB.Center.X) * 180.0 / Math.PI;
                        angleAway += 90.0;
                        avoidanceAngle = NormalizeAngle(angleAway);
                        break; // Only avoid one enemy at a time
                    }
                }
            }

            double rad = 0;
            // --- COLLISION ---
            if (this.fuel > 0) {


                if (backframe > 0) {
                    backframe--;
                    rad = this.rotation * Math.PI / 180.0;
                    dx = speed * Math.Sin(rad);
                    dy = -speed * Math.Cos(rad);

                    if (backframe == 0) {
                        speed = Math.Abs(speed);
                    }
                } else {
                    double turnSpeed = 1.5;
                    double targetAngle;

                    // If colliding with enemy, steer away instead of toward player
                    if (enemyCollision) {
                        targetAngle = avoidanceAngle;
                        turnSpeed = 2.5; // Turn faster to avoid
                    } else {
                        targetAngle = angleToPlayer;
                    }

                    double turnDiff = targetAngle - NormalizeAngle(this.rotation);
                    turnDiff = NormalizeSigned(turnDiff);
                    double turn = Math.Max(-turnSpeed, Math.Min(turnSpeed, turnDiff));
                    this.rotation += turn;

                    // Slow down when avoiding
                    double currentSpeed = enemyCollision ? speed * 0.7 : speed;

                    rad = this.rotation * Math.PI / 180.0;
                    dx = currentSpeed * Math.Sin(rad);
                    dy = -currentSpeed * Math.Cos(rad);
                }
                useFuel();
            } else {
                //dying
                speed -= 0.05;//quick stop.
                speed = Math.Clamp(speed, 0, 3.75);
                dx = speed * Math.Sin(rad);
                dy = -speed * Math.Cos(rad);
                
                //death calculation for enemy here
                if(speed <= 0 && this.fuel <= 0) {

                    if (!hasBeenKilled) {
                        MainPage.score += 100;
                        hasBeenKilled = true;
                    }
                    removeThis = true;
                }
            }

            enemyOBBCenterX += (float)dx;
            enemyOBBCenterY += (float)dy;
            

            objectOBB.Update(new Vector2(enemyOBBCenterX, enemyOBBCenterY), (float)(this.rotation * Math.PI / 180.0));
        }



        public void updateUi(float xp, float yp, double rpDegrees, float screenWidth, float screenHeight, float playerX, float playerY) {
            // Enemy is in mapLayout. Since setLayoutPosition uses SetLayoutBounds,
            // we need to update the bounds, NOT use TranslationX/Y
            // (Translation is an OFFSET from the bounds position)

            // Enemy's world position (top-left corner)
            double topLeftX = enemyOBBCenterX - (layoutWidth * 0.5);
            double topLeftY = enemyOBBCenterY - (layoutHeight * 0.5);

            // Update the layout bounds directly in mapLayout coordinates
            AbsoluteLayout.SetLayoutBounds(gameObjectLayout,
                new Microsoft.Maui.Graphics.Rect(topLeftX, topLeftY, layoutWidth, layoutHeight));

            // Counter-rotate the sprite to maintain world-space heading
            gameObjectLayout.Rotation = this.rotation;
        }

        double NormalizeSigned(double a) {
            a %= 360;
            if (a > 180) a -= 360;
            if (a < -180) a += 360;
            return a;
        }

        double NormalizeAngle(double a) {
            a %= 360;
            if (a < 0) a += 360;
            return a;
        }

        public void useFuel() {
            this.fuel -= 0.025;
            if(this.fuel <= 0) {
                this.fuel = 0;
            }
        }
    }

}
