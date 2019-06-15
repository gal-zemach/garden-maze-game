using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TileMap/MultiSidedInfectedSprite")]
public class MultiSidedTile : ScriptableObject
{
//    public struct NamedAnimator
//    {
//        public RuntimeAnimatorController controller;
//        public string name;
//
//        public NamedAnimator(RuntimeAnimatorController _controller, string _name)
//        {
//            controller = _controller;
//            name = _name;
//        }
//    }
    
    public Sprite zeroSided, oneSidedN, oneSidedS, oneSidedE, oneSidedW, twoSidedNE, twoSidedNW, twoSidedSE, twoSidedSW, 
                  twoSidedNS, twoSidedEW, threeSidedN, threeSidedS, threeSidedE, threeSidedW, fourSided;

    [Space(20)]
    public String a = "animations";
    public RuntimeAnimatorController zeroSidedAnim, oneSidedNAnim, oneSidedSAnim, oneSidedEAnim, oneSidedWAnim, 
                                     twoSidedNEAnim, twoSidedNWAnim, twoSidedSEAnim, twoSidedSWAnim, 
                                     twoSidedNSAnim, twoSidedEWAnim, threeSidedNAnim, threeSidedSAnim, threeSidedEAnim, 
                                     threeSidedWAnim, fourSidedAnim;

    [Space(20)]
    public string ANIM_NAME = "anim";
    public string SPRITE_NAME = "sprite";    
    
    public RuntimeAnimatorController UpdateAnimator(TileMap map, Vector2 index)
    {
        var nTile = map.GetTile(index + new Vector2(1, 0)) as MoveableWall;
        var sTile = map.GetTile(index + new Vector2(-1, 0)) as MoveableWall;
        var wTile = map.GetTile(index + new Vector2(0, 1)) as MoveableWall;
        var eTile = map.GetTile(index + new Vector2(0, -1)) as MoveableWall;

        var hasNSide = hasSide(nTile);
        var hasSSide = hasSide(sTile);
        var hasWSide = hasSide(wTile);
        var hasESide = hasSide(eTile);
        
        return ChooseAnim(hasNSide, hasSSide, hasESide, hasWSide);
    }

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
    
    private RuntimeAnimatorController ChooseAnim(bool nSide, bool sSide, bool eSide, bool wSide)
    {
        if (nSide)
        {
            if (sSide)
            {
                if (eSide)
                {
                    if (wSide)
                    {
                        return fourSidedAnim;
                    }
                    return threeSidedWAnim;
                }
                if (wSide)
                {
                    return threeSidedEAnim;
                }

                return twoSidedNSAnim;
            }

            if (eSide)
            {
                if (wSide)
                {
                    return threeSidedSAnim;
                }

                return twoSidedNEAnim;
            }

            if (wSide)
            {
                return twoSidedNWAnim;
            }

            return oneSidedNAnim;
        }

        if (sSide)
        {
            if (eSide)
            {
                if (wSide)
                {
                    return threeSidedNAnim;
                }

                return twoSidedSEAnim;
            }

            if (wSide)
            {
                return twoSidedSWAnim;
            }

            return oneSidedSAnim;
        }

        if (eSide)
        {
            if (wSide)
            {
                return twoSidedEWAnim;
            }

            return oneSidedEAnim;
        }

        if (wSide)
        {
            return oneSidedWAnim;
        }

        return zeroSidedAnim;
    }
}
