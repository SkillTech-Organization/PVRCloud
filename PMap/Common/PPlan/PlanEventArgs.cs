using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using PMapCore.DB;
using PMapCore.BO;

namespace PMapCore.Common.PPlan
{
    public enum ePlanEventMode
    {
        ChgZoom,                        //Zoom változott
        ChgShowPlannedFlag,             //Változott a tervezett lerakók megjelenítés checkbox
        ChgShowUnPlannedFlag,           //Változott a tervezetlen lerakók megjelenítés checkbox
        ChgZoomToSelectedTour,          //Változott a nagyítás a kiválasztott túrára checkbox
        ChgZoomToSelectedUnPlanned,     //Változott a nagyítás a kiválasztott tervezetlen lerakóra checkbox
        ChgShowAllOrdersInGrid,         //Változott a nagyítás az összes megrendelés a listában checkbox
        ChgTooltipMode,                 //Változott markerek tooltip megjelenítési módja
        ChgTourSelected,                //Változott egy túra láthatósága
        ChgTourColor,                   //Változott egy túra színe
        RemoveTour,                     //Túra törlése történt
        AddTour,                        //Új túra létrehozása történt
        HideAllTours,                   //Összes túra eltüntetése
        ShowAllTours,                   //Összes túra megjelenítése
        ReInit,                         //Mindent újrainicializálni
        Refresh,                        //Minden frissítése
        RefreshTour,                    //Egy túra frissítése
        RefreshOrders,                  //Megrendelések frissítése
        ChgFocusedTour,                 //Fókuszált túra változott
        ChgFocusedTourPoint,            //Fókuszált túrapont változott
        ChgFocusedOrder,                //Fókuszált megrendelés változott
        ViewerMode,                     //Browse üzemmód
        EditorMode,                     //Szerkesztés üzemmód
        PrevTour,                       //Előző túra kiválasztása (pplPPlanEditor használja)
        NextTour,                       //Következő túra kiválasztása (pplPPlanEditor használja)
        FirstTour,                      //Gridben a legelső túra kiválasztása 
        CheckMode                       //Ellenörző üzemmód

    }

    public class PlanEventArgs : EventArgs
    {
        public ePlanEventMode EventMode { get; set; }
        public bool IsVisible { get; set; }
        public Color Color { get; set; }
        public boPlanTour Tour { get; set; }
        public boPlanTourPoint TourPoint { get; set; }
        public boPlanOrder PlanOrder { get; set; }
        public bool NeedRefresh { get; set; }
        public PlanEventArgs(ePlanEventMode p_eventMode)
        {
            EventMode = p_eventMode;
            NeedRefresh = true;
        }

        public PlanEventArgs(ePlanEventMode p_eventMode, boPlanTour p_Tour, bool p_Visible)
        {
            EventMode = p_eventMode;
            Tour = p_Tour;
            IsVisible = p_Visible;
            NeedRefresh = true;
        }

        public PlanEventArgs(ePlanEventMode p_eventMode, boPlanTour p_Tour, Color p_Color)
        {
            EventMode = p_eventMode;
            Tour = p_Tour;
            Color = p_Color;

            NeedRefresh = true;
        }

        public PlanEventArgs(ePlanEventMode p_eventMode, boPlanTour p_Tour)
        {
            EventMode = p_eventMode;
            Tour = p_Tour;

            NeedRefresh = true;
        }

        public PlanEventArgs(ePlanEventMode p_eventMode, boPlanTourPoint p_TourPoint)
        {
            EventMode = p_eventMode;
            TourPoint = p_TourPoint;
            NeedRefresh = true;
        }

        public PlanEventArgs(ePlanEventMode p_eventMode, boPlanOrder p_PlanOrder)
        {
            EventMode = p_eventMode;
            PlanOrder = p_PlanOrder;
            NeedRefresh = true;
        }

    }
}
