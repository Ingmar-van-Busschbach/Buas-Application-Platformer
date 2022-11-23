using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TileBorders
{
    public bool top, bottom;
    public bool left, right;
    public bool topLeft, topRight;
    public bool bottomLeft, bottomRight;

    public bool BordersMatch(TileBorders tileBorder)
    {
        bool topBottom = this.top == tileBorder.top && this.bottom == tileBorder.bottom;
        bool leftRight = this.left == tileBorder.left && this.right == tileBorder.right;
        bool topDiagonals = this.topLeft == tileBorder.topLeft && this.topRight == tileBorder.topRight;
        bool bottomDiagionals = this.bottomLeft == tileBorder.bottomLeft && this.bottomRight == tileBorder.bottomRight;
        return topBottom && leftRight && topDiagonals && bottomDiagionals;
    }
}