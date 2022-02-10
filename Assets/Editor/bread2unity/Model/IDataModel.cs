using System.Collections;
using System.Collections.Generic;

namespace Bread2Unity
{
    public class IDataModel
    {
        public List<ISprite> sprites = new List<ISprite>();
        public List<IAnimation> animations = new List<IAnimation>();
        public int sheetW;
        public int sheetH;
    }
}