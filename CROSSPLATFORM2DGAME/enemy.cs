using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CROSSPLATFORM2DGAME {
    internal class enemy : gameObject{
        int enemyRank;
        public float enemyOBBCenterX;
        public float enemyOBBCenterY;
        public enemy() {
            createLayout(50, 50);
            createImage("player.png", 50, 50);

            double enemyCenterX = layoutWidth / 2 - imageWidth / 2;
            double enemyCenterY = layoutHeight / 2 - imageHeight / 2;

            //AbsoluteLayout.SetLayoutBounds(gameObjectImage, new Rect(playerCenterX, playerCenterY, gameObjectImage.WidthRequest, gameObjectImage.HeightRequest));
            /* setLayoutPosition(MainPage.gameLayoutWidth / 2 - layoutWidth / 2,
                         MainPage.gameLayoutHeight / 2 - layoutHeight / 2,
                         layoutWidth,
                         layoutHeight);*/
            setImagePosition(enemyCenterX, enemyCenterY, imageWidth, imageHeight);

            //temp for testing.
            globalX = 100;
            globalY= 100;
            setLayoutPosition(globalX,
                        globalY,
                        layoutWidth,
                        layoutHeight);

            enemyOBBCenterX = (float)globalX + (float)layoutWidth / 2;
            enemyOBBCenterY = (float)globalY + (float)layoutHeight / 2;

            setUpOBB(new Vector2(enemyOBBCenterX, enemyOBBCenterY),(float)layoutWidth,(float)layoutHeight,0);
            OBBHandler.movingOBBs.Add(this.objectOBB);
        }
    }
}
