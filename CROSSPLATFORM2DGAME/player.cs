using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CROSSPLATFORM2DGAME
{
    class player : gameObject {

        public int lives;
        public double fuel;
        public double boostAmount;

        public player() {
            lives = 5;
            fuel = 100;
            boostAmount = 100;
            //create the image for the player
            createLayout(40, 77);
            createImage("car31.png", 40, 77);

            //AbsoluteLayout.SetLayoutBounds(gameObjectImage, new Rect(playerCenterX, playerCenterY, gameObjectImage.WidthRequest, gameObjectImage.HeightRequest));
            //if rect could rotate my life wouldve been a lot nicer before trying to implement OBBs

            //setImagePosition(playerCenterX, playerCenterY, imageWidth, imageHeight);
            //playerCenterX and Y arent needed as the image takes up the full layout

            setImagePosition(0,0,imageWidth, imageHeight); 

            setLayoutPosition(MainPage.gameLayoutWidth / 2 - layoutWidth / 2,
                        MainPage.gameLayoutHeight / 2 - layoutHeight / 2,
                        layoutWidth,
                        layoutHeight);    
        }



        public void useBoost() {
            this.boostAmount -= 0.60;
            if (this.boostAmount < 0) {
                this.boostAmount = 0;
            }
        }

        public void useFuel(double boostMultiplier) {
            this.fuel -= 0.0275 * boostMultiplier;//use more fuel if boosting
        }

        public void addFuel() {
            this.fuel += 25;
            if (this.fuel > 100) {
                this.fuel = 100;
            }
        }

        public void changeImage(string imageSource) {
            this.gameObjectImage.Source = imageSource;
        }

    }
}
