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
        
        public player() {
            //create the image for the player
            createLayout(50, 50);
            createImage("player.png", 50, 50);
            
            double playerCenterX = layoutWidth / 2 - imageWidth / 2;
            double playerCenterY = layoutHeight / 2 - imageHeight / 2;

            //AbsoluteLayout.SetLayoutBounds(gameObjectImage, new Rect(playerCenterX, playerCenterY, gameObjectImage.WidthRequest, gameObjectImage.HeightRequest));
           /* setLayoutPosition(MainPage.gameLayoutWidth / 2 - layoutWidth / 2,
                        MainPage.gameLayoutHeight / 2 - layoutHeight / 2,
                        layoutWidth,
                        layoutHeight);*/
            setImagePosition(playerCenterX, playerCenterY, imageWidth, imageHeight);
            
            setLayoutPosition(MainPage.gameLayoutWidth / 2 - layoutWidth / 2,
                        MainPage.gameLayoutHeight / 2 - layoutHeight / 2,
                        layoutWidth,
                        layoutHeight);

            
        }

    }
}
