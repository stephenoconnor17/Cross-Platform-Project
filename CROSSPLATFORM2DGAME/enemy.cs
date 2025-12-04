using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CROSSPLATFORM2DGAME {
    public class enemy : gameObject {
        int enemyRank;
        public float enemyOBBCenterX;
        public float enemyOBBCenterY;
        public double rotation;

        public double directionRotation;
        public float directionCenterX;
        public float directionCenterY;


        public enemy(double globalX, double globalY) {
            this.rotation = 0;
            createLayout(40, 77);
            createImage("car31.png", 40, 77);

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
        double speed = 3.5;

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
            angleToPlayer += 90.0;                 // adjust because sprite faces "up"
            angleToPlayer = NormalizeAngle(angleToPlayer);

            // --- smallest signed difference ---
            double diff = angleToPlayer - NormalizeAngle(this.rotation);
            diff = NormalizeSigned(diff);

            if (collision) {
                if(backframe == 0) {
                    backframe = 60;
                }
                
                speed = -speed;
                collision = false;
            }


            if (backframe > 0) {
                backframe--;
                // --- move backward along current heading (0° = up) ---
                double rad = this.rotation * Math.PI / 180.0;
                dx = speed * Math.Sin(rad);        // X delta
                dy = -speed * Math.Cos(rad);       // Y delta (negative because up = -Y)
           

                if(backframe == 0) {
                    speed = Math.Abs(speed);
                }
            } else {
                double turnSpeed = 1.5; // degrees per frame — tune this
                double turn = Math.Max(-turnSpeed, Math.Min(turnSpeed, diff));
                this.rotation += turn;

                // --- move forward along current heading (0° = up) ---
                double rad = this.rotation * Math.PI / 180.0;
                dx = speed * Math.Sin(rad);        // X delta
                dy = -speed * Math.Cos(rad);       // Y delta (negative because up = -Y)

                

                // --- update OBB: rotation MUST be radians ---

                

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
    }

}
