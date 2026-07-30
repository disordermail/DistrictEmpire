using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace DistrictEmpire.Presentation
{
    public readonly struct MapRegion
    {
        public readonly string Id;
        public readonly string Name;
        public readonly double Latitude;
        public readonly double Longitude;

        public MapRegion(string id, string name, double latitude, double longitude)
        {
            Id = id;
            Name = name;
            Latitude = latitude;
            Longitude = longitude;
        }
    }

    /// <summary>Loads only the visible OpenStreetMap tiles for the selected market region.</summary>
    public sealed class OpenStreetMapTileLayer : MonoBehaviour
    {
        private const int Zoom = 9;
        private const string UserAgent = "DistrictEmpirePrototype/0.1 (local Unity MVP)";
        private static readonly Dictionary<string, Texture2D> CachedTiles = new();
        private int renderVersion;

        public void Show(VisualElement map, MapRegion region)
        {
            renderVersion++;
            var version = renderVersion;
            var tileLayer = new VisualElement { pickingMode = PickingMode.Ignore };
            tileLayer.AddToClassList("osm-tile-layer");
            map.Insert(0, tileLayer);

            var centerX = LongitudeToTile(region.Longitude, Zoom);
            var centerY = LatitudeToTile(region.Latitude, Zoom);
            for (var row = -1; row <= 1; row++)
            for (var column = -1; column <= 1; column++)
            {
                var tile = new Image { scaleMode = ScaleMode.ScaleAndCrop, pickingMode = PickingMode.Ignore };
                tile.AddToClassList("osm-tile");
                tile.style.left = new Length((column + 1) * 100f / 3f, LengthUnit.Percent);
                tile.style.top = new Length((row + 1) * 100f / 3f, LengthUnit.Percent);
                tileLayer.Add(tile);
                StartCoroutine(LoadTile(tile, centerX + column, centerY + row, version));
            }
        }

        private IEnumerator LoadTile(Image target, int tileX, int tileY, int version)
        {
            var tileCount = 1 << Zoom;
            tileX = ((tileX % tileCount) + tileCount) % tileCount;
            tileY = Mathf.Clamp(tileY, 0, tileCount - 1);
            var url = $"https://tile.openstreetmap.org/{Zoom}/{tileX}/{tileY}.png";
            if (CachedTiles.TryGetValue(url, out var cached))
            {
                if (version == renderVersion && target.panel != null) target.image = cached;
                yield break;
            }

            using var request = UnityWebRequestTexture.GetTexture(url, true);
            request.SetRequestHeader("User-Agent", UserAgent);
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success) yield break;

            var texture = DownloadHandlerTexture.GetContent(request);
            CachedTiles[url] = texture;
            if (version == renderVersion && target.panel != null) target.image = texture;
        }

        private static int LongitudeToTile(double longitude, int zoom) => (int)Math.Floor((longitude + 180d) / 360d * (1 << zoom));

        private static int LatitudeToTile(double latitude, int zoom)
        {
            var radians = latitude * Math.PI / 180d;
            var value = (1d - Math.Asinh(Math.Tan(radians)) / Math.PI) / 2d * (1 << zoom);
            return (int)Math.Floor(value);
        }
    }
}
