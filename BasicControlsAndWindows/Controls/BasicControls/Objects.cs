using System;
using System.Reflection;

namespace BasicControlsAndWindows.Controls.BasicControls
{
    internal static class Objects
    {
        public static string GetStringFromObject(object ThisObj, string PropertyName)
        {
            if (ThisObj == null)
                return "";
            return GetStringValue(ThisObj, PropertyName);
        }

        private static string GetStringValue(object ThisItem, string PropertyName)
        {
            PropertyInfo ThisProp = ThisItem.GetType().GetRuntimeProperty(PropertyName);
            return ThisProp.GetValue(ThisItem).ToString();
        }

        //decided to risk not doing borderbutton.  don't remember even using that one.
        //if i realize i need it, can add.  by then, will have more details.


    }
}
