using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TileMap/MultiSidedInfectedSprite")]
public class MultiSidedTile : ScriptableObject
{
    public Sprite zeroSided, oneSidedN, oneSidedS, oneSidedE, oneSidedW, twoSidedNE, twoSidedNW, twoSidedSE, twoSidedSW, 
                  twoSidedNS, twoSidedEW, threeSidedN, threeSidedS, threeSidedE, threeSidedW, fourSided;

    public Sprite UpdateSprite(TileMap map, Vector2 index)
    {
        var nTile = map.GetTile(index + new Vector2(1, 0)) as MoveableWall;
        var sTile = map.GetTile(index + new Vector2(-1, 0)) as MoveableWall;
        var wTile = map.GetTile(index + new Vector2(0, 1)) as MoveableWall;
        var eTile = map.GetTile(index + new Vector2(0, -1)) as MoveableWall;

        var hasNSide = hasSide(nTile);
        var hasSSide = hasSide(sTile);
        var hasWSide = hasSide(wTile);
        var hasESide = hasSide(eTile);

        return ChooseSprite(hasNSide, hasSSide, hasESide, hasWSide);
    }

    private bool hasSide(MoveableWall tile)
    {
        return (tile != null) && (tile.infected || tile.halfInfected);
    }

    private Sprite ChooseSprite(bool nSide, bool sSide, bool eSide, bool wSide)
    {
        if (nSide)
        {
            if (sSide)
            {
                if (eSide)
                {
                    if (wSide)
                    {
                        return fourSided;
                    }
                    return threeSidedW;
                }
                if (wSide)
                {
                    return threeSidedE;
                }

                return twoSidedNS;
            }

            if (eSide)
            {
                if (wSide)
                {
                    return threeSidedS;
                }

                return twoSidedNE;
            }

            if (wSide)
            {
                return twoSidedNW;
            }

            return oneSidedN;
        }

        if (sSide)
        {
            if (eSide)
            {
                if (wSide)
                {
                    return threeSidedN;
                }

                return twoSidedSE;
            }

            if (wSide)
            {
                return twoSidedSW;
            }

            return oneSidedS;
        }

        if (eSide)
        {
            if (wSide)
            {
                return twoSidedEW;
            }

            return oneSidedE;
        }

        if (wSide)
        {
            return oneSidedW;
        }

        return zeroSided;
    }
}
