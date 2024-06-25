using System;

namespace GMap.NET.MapProviders
{
    /// <summary>
    ///     OpenTopoMap provider - http://www.opentopomap.org
    /// </summary>
    public class OpenTopoMapProvider : OpenStreetMapProviderBase
    {
        public static readonly OpenTopoMapProvider Instance;

        OpenTopoMapProvider()
        {
            RefererUrl = "http://www.opentopomap.org/";
        }

        static OpenTopoMapProvider()
        {
            Instance = new OpenTopoMapProvider();
        }

        #region GMapProvider Members

        public override Guid Id
        {
            get;
        } = new Guid("b74a95b7-086d-433c-abfb-42f13021416e");

        public override string Name
        {
            get;
        } = "OpenTopoMap";

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

        #endregion

        string MakeTileImageUrl(GPoint pos, int zoom, string language)
        {
            char letter = ServerLetters[GetServerNum(pos, 3)];
            return string.Format(UrlFormat, letter, zoom, pos.X, pos.Y);
        }

        static readonly string UrlFormat = "http://{0}.tile.opentopomap.org/#map={1}/{2}/{3}.png";
    }
}
