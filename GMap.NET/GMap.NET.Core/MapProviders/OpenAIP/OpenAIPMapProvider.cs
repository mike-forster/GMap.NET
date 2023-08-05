using System;
using GMap.NET.Projections;

namespace GMap.NET.MapProviders
{
    /// <summary>
    ///     OpenAIPMap provider - http://www.opencyclemap.org
    /// </summary>
    public class OpenAIPMapProvider : GMapProvider
    {
        public static readonly OpenAIPMapProvider Instance;

        public OpenAIPMapProvider()
        {
            RefererUrl = "https://www.openAIP.net/";
        }

        static OpenAIPMapProvider()
        {
            Instance = new OpenAIPMapProvider();
        }

        public readonly string ServerLetters = "abc";
        private readonly string OPEN_API_KEY = "d78dd8fa8f7d62434b529a3c97b56140";

        #region GMapProvider Members

        public override Guid Id
        {
            get;
        } = new Guid("ecb983d7-ff07-4567-a27b-5fd168ce331c");

        public override string Name
        {
            get;
        } = "OpenAIPMap";

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

        public override PureProjection Projection
        {
            get
            {
                return MercatorProjection.Instance;
            }
        }

        #endregion

        string MakeTileImageUrl(GPoint pos, int zoom, string language)
        {
            char letter = ServerLetters[GetServerNum(pos, 3)];
            return string.Format(UrlFormat, letter, zoom, pos.X, pos.Y, OPEN_API_KEY);
        }

        static readonly string UrlFormat = "https://{0}.api.tiles.openaip.net/api/data/openaip/{1}/{2}/{3}.png?apiKey={4}";
    }
}
