using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CROSSPLATFORM2DGAME {
    internal class fuelObject : gameObject{

        public fuelObject() {

            createLayout(28, 40);
            createImage("jerrycan.png", 28, 40);
            setImagePosition(0, 0, gameObjectImage.Width, gameObjectImage.Height); //x and y relative to layout
            setLayoutPosition(300, 300, gameObjectLayout.Width, gameObjectLayout.Height); //x and y relative to game world
            setUpOBB(new Vector2(300,300), (float)gameObjectLayout.Width, (float)gameObjectLayout.Height, 0, "fuel"); //pass in object type as fuel so collision knows what to do.
            OBBHandler.staticOBBs.Add(objectOBB); //IT DONT MOVE SO STATIC OBB
        }
       
    }
}
