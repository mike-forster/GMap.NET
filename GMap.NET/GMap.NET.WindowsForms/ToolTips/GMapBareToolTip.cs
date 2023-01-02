using System;
using System.Drawing;
using System.Runtime.Serialization;

// Added by MDF

namespace GMap.NET.WindowsForms.ToolTips
{

    /// <summary>
    /// GMap.NET marker
    /// </summary>
    [Serializable]
    public class GMapBareToolTip : GMapToolTip, ISerializable
    {
        public float Radius = 5f; // Actually offset of text from centre

        public GMapBareToolTip(GMapMarker marker)
           : base(marker)
        {
            TextPadding = new Size((int)Radius, (int)Radius);
        }
        
        public override void OnRender(Graphics g)
        {
            var st = g.MeasureString(Marker.ToolTipText, Font).ToSize();

            var rect = new Rectangle(Marker.ToolTipPosition.X, 
                Marker.ToolTipPosition.Y - st.Height - TextPadding.Height, 
                st.Width + TextPadding.Width * 2, 
                st.Height + TextPadding.Height);
            rect.Offset(Offset.X, Offset.Y);

            if (Format.Alignment == StringAlignment.Near)
            {
                rect.Offset(TextPadding.Width, 0);
            }
            g.DrawString(Marker.ToolTipText, Font, Foreground, rect, Format);
        }


        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Radius", Radius);

            base.GetObjectData(info, context);
        }

        protected GMapBareToolTip(SerializationInfo info, StreamingContext context)
           : base(info, context)
        {
            Radius = Extensions.GetStruct<float>(info, "Radius", 5f);
        }

        #endregion
    }
}
