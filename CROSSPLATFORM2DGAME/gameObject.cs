using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CROSSPLATFORM2DGAME
{
    public class gameObject
    {
        //we treat the Image as the visual representation of the game object
        //and the Layout as the container / positioning element
        //try keep size tile to around 48 * 48. it doesnt have to fit perfectly. not that type of game.
        public AbsoluteLayout gameObjectLayout { get; set; }
        public Image gameObjectImage { get; set; }
   
        public double globalX { get; set; }
        public double globalY { get; set; }
        public double layoutWidth { get; set; }
        public double layoutHeight { get; set; }
        //global coordinates of the game object on the map based on its layout position and width and height
        public OBB objectOBB { get; set; }
        public double imageWidth { get; set; }
        public double imageHeight { get; set; }

        //FORMULA for making a game object
        
        //createLayout(width, height);
        //createImage(imageSource, width, height);
        //setImagePosition(x, y, width, height); //x and y relative to layout
        //setLayoutPosition(x, y, width, height); //x and y relative to game world
        //setUpOBB(center, width, height, rotation);
        //ADD TO OBB HANDLER DEPENDING ON IF ITS A MOVING OBJECT OR STATIC OBJECT

        public void setLayoutPosition(double x, double y, double width, double height) {
            AbsoluteLayout.SetLayoutBounds(gameObjectLayout, new Rect(x,y,width,height));
            //the x and y of the layout is relative to the game world so we pass them into its OBB, 
            //objectOBB = new OBB(new Vector2((float)x + (float)(width/2), (float)y+(float)height/2), width, height, 0); //not valid as playerlayout is based on gameLayout, not mapLayout. must be mapLayout
            //AbsoluteLayout.SetLayoutFlags(gameObjectLayout, AbsoluteLayoutFlags.None);
        }

        //use for moving objects like enemies - eventually to be diseccted into different sections on map. 
        //supposedly.
        public void setUpOBB(Vector2 center, float width, float height, double rotation, enemy e) {
            objectOBB = new OBB(center, width, height, rotation);
            objectOBB.thisEnemy = e;
        }

        public void setUpOBB(Vector2 center, float width, float height, double rotation) {
            objectOBB = new OBB(center, width, height, rotation);
        }

        //use for static objects like fuel cans, coins, etc.
        public void setUpOBB(Vector2 center, float width, float height, double rotation, string objectType) {
            objectOBB = new OBB(center, width, height, rotation);
            objectOBB.objectType = objectType;
            objectOBB.thisObject = this;
        }

        public void setImagePosition(double x, double y, double width, double height) {
            AbsoluteLayout.SetLayoutBounds(gameObjectImage, new Rect(x, y, width, height));
            AbsoluteLayout.SetLayoutFlags(gameObjectImage, AbsoluteLayoutFlags.None);

            gameObjectLayout.Children.Add(gameObjectImage);
        }

        public void createImage(string imageSource, double width, double height) {
            gameObjectImage = new Image {
                Source = imageSource,
                WidthRequest = width,
                HeightRequest = height
            };

            imageWidth = width;
            imageHeight = height;
        }

        public void createLayout(double width, double height) {
            gameObjectLayout = new AbsoluteLayout();
            gameObjectLayout.WidthRequest = width;
            gameObjectLayout.HeightRequest = height;
            this.layoutWidth = gameObjectLayout.WidthRequest;
            this.layoutHeight = gameObjectLayout.HeightRequest;
        }

        public double NormalizeAngle(double a) {
            a %= 360;
            if (a > 180) a -= 360;
            if (a < -180) a += 360;
            return a;
        }
    }
}


