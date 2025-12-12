using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CROSSPLATFORM2DGAME {
    internal class lootObject : gameObject {

        public lootObject(double x, double y) {

            createLayout(48, 48);
            createImage("loot1.png", 48, 48);
            setImagePosition(0, 0, gameObjectImage.Width, gameObjectImage.Height); //x and y relative to layout
            setLayoutPosition(x, y, gameObjectLayout.Width, gameObjectLayout.Height); //x and y relative to game world
            setUpOBB(new Vector2((float)x + (float)layoutWidth/2, (float)y + (float)layoutHeight / 2), (float)layoutWidth - 23, (float)layoutHeight - 10, 0, "loot"); //pass in object type as fuel so collision knows what to do.
            OBBHandler.staticOBBs.Add(objectOBB); //IT DONT MOVE SO STATIC OBB
        }

    }
}
