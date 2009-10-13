using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace OnlabNeuralis
{
    public interface IDragable
    {        
        bool PointInInsideArea(Point p);
        bool PointInOutsideArea(Point p);
        void SetPosition(Point p, bool addToRoute);
        void SetSecondParameterAgainstPosition(Point p);
        void SetSelectedState(int inside, int outside);//0 = nem, 1 = mouse over, 2 = clicked
    }
}
