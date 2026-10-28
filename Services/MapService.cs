namespace Operation_Extraction.Services;

using Operation_Extraction.Models;

/// <summary>
/// Generates the tactical 10x10 extraction map and related points of interest.
/// </summary>
public class MapService : IMapService
{
    private readonly Random _random;

    public MapService()
    {
        _random = new Random();
    }

    public GameMap GenerateMap()
    {
        var map = new GameMap();

        for (var x = 0; x < GameMap.GridSize; x++)
        {
            for (var y = 0; y < GameMap.GridSize; y++)
            {
                var roll = _random.Next(1, 101);
                var tileType = roll switch
                {
                    <= 45 => TileType.Open,
                    <= 70 => TileType.Forest,
                    <= 85 => TileType.Ruins,
                    <= 96 => TileType.Street,
                    _ => TileType.Water
                };

                map.Tiles[x, y] = new MapTile
                {
                    X = x,
                    Y = y,
                    Type = tileType,
                    IsBlocked = tileType == TileType.Water
                };
            }
        }

        return map;
    }

    public List<InfilPoint> GenerateInfilPoints(GameMap map)
    {
        var points = new List<InfilPoint>();
        while (points.Count < 6)
        {
            var x = _random.Next(0, GameMap.GridSize);
            var y = _random.Next(0, GameMap.GridSize);

            if (map.Tiles[x, y].IsBlocked)
            {
                continue;
            }

            var point = new InfilPoint
            {
                Name = $"Infil {points.Count + 1}",
                Description = "Player entry point.",
                X = x,
                Y = y,
                IsDynamic = false
            };

            map.Tiles[x, y].POI = point;
            points.Add(point);
        }

        return points;
    }

    public List<ExfilPoint> GenerateExfilPoints(GameMap map)
    {
        var points = new List<ExfilPoint>();
        while (points.Count < 3)
        {
            var x = _random.Next(0, GameMap.GridSize);
            var y = _random.Next(0, GameMap.GridSize);

            if (map.Tiles[x, y].IsBlocked || map.Tiles[x, y].POI is not null)
            {
                continue;
            }

            var point = new ExfilPoint
            {
                Name = $"Exfil {points.Count + 1}",
                Description = "Primary extraction lane.",
                X = x,
                Y = y,
                IsDynamic = false
            };

            map.Tiles[x, y].POI = point;
            points.Add(point);
        }

        return points;
    }

    public List<POI> GenerateDynamicPOIs(GameMap map, List<InfilPoint>? infilPoints = null, List<ExfilPoint>? exfilPoints = null)
    {
        var points = new List<POI>();
        var candidateTiles = new List<MapTile>();

        for (var x = 0; x < GameMap.GridSize; x++)
        {
            for (var y = 0; y < GameMap.GridSize; y++)
            {
                if (map.Tiles[x, y].IsBlocked || map.Tiles[x, y].POI is not null)
                {
                    continue;
                }

                candidateTiles.Add(map.Tiles[x, y]);
            }
        }

        var shuffled = candidateTiles.OrderBy(_ => _random.Next()).ToList();
        for (var index = 0; index < 3 && index < shuffled.Count; index++)
        {
            var tile = shuffled[index];
            var poi = new POI
            {
                Name = $"Dynamic POI {index + 1}",
                Description = "Unclaimed loot cache or intel source.",
                X = tile.X,
                Y = tile.Y,
                IsDynamic = true
            };

            tile.POI = poi;
            points.Add(poi);
        }

        return points;
    }
}
