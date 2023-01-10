using System;
using GMap.NET.Projections;

namespace GMap.NET.MapProviders
{
    public class MapboxElevationMapProvider : GMapProvider
    {
        public static readonly MapboxElevationMapProvider Instance;
        public string Key { get ; set; } = "pk.eyJ1IjoibWRmLXVrIiwiYSI6ImNsY200MnFpNDBtMG0zbnFxejZtbDRyNjQifQ.AxcmQj_Vg0vLTAVgDO7swQ";

        MapboxElevationMapProvider()
        {
            //Key = "pk.eyJ1IjoibWRmLXVrIiwiYSI6ImNsY200MnFpNDBtMG0zbnFxejZtbDRyNjQifQ.AxcmQj_Vg0vLTAVgDO7swQ";
            //RefererUrl = MapboxElevationServerUrl;
        }

        static MapboxElevationMapProvider()
        {
            Instance = new MapboxElevationMapProvider();
        }

        #region GMapProvider Members

        public override Guid Id
        {
            get;
        } = new Guid("f3c5d043-d288-4889-868e-f04b02f320af");

        public override string Name
        {
            get;
        } = "MapboxElevationMapProvider";

        public string ApiKey = string.Empty;

        GMapProvider[] _overlays;

        public override GMapProvider[] Overlays
        {
            get
            {
                if (_overlays == null)
                {
                    _overlays = new GMapProvider[] {this};
                }

                return _overlays;
            }
        }

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            string url = MakeTileImageUrl(pos, zoom, string.Empty);

            return GetTileImageUsingHttp(url);
        }

        public override PureProjection Projection => MercatorProjection.Instance;

        //public string MapboxElevationServerUrl = string.Empty;

        //public string CustomServerLetters = string.Empty;

        #endregion

        string MakeTileImageUrl(GPoint pos, int zoom, string language)
        {
            return string.Format(UrlFormat, zoom, pos.X, pos.Y, Key);
        }

        static readonly string UrlFormat = "https://api.mapbox.com/v4/mapbox.terrain-rgb/{0}/{1}/{2}.pngraw?access_token={3}";

    }
}
