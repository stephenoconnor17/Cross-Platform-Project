using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CROSSPLATFORM2DGAME
{
    class gameObject
    {
        //we treat the Image as the visual representation of the game object
        //and the Layout as the container / positioning element


        //formula for making a game object
        //
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

        //method to set where it will be on the map.
        public void setLayoutPosition(double x, double y, double width, double height) {
            AbsoluteLayout.SetLayoutBounds(gameObjectLayout, new Rect(x,y,width,height));
            //the x and y of the layout is relative to the game world so we pass them into its globalCoordinateObject
            //objectOBB = new OBB(new Vector2((float)x + (float)(width/2), (float)y+(float)height/2), width, height, 0); //not valid as playerlayout is based on gameLayout, not mapLayout. must be mapLayout
            //AbsoluteLayout.SetLayoutFlags(gameObjectLayout, AbsoluteLayoutFlags.None);
        }
        public void setUpOBB(Vector2 center, float width, float height, double rotation) {
            objectOBB = new OBB(center, width, height, rotation);
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

    }
}
