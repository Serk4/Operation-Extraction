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
                    <= 45 => TerrainType.Open,
                    <= 70 => TerrainType.Forest,
                    <= 85 => TerrainType.Ruins,
                    <= 96 => TerrainType.Street,
                    _ => TerrainType.Water
                };

                map.Tiles[x, y] = new MapTile
                {
                    X = x,
                    Y = y,
                    Terrain = tileType,
                    TileType = TileType.Empty,
                    IsBlocked = tileType == TerrainType.Water
                };
            }
        }

        return map;
    }

    public List<POI> GenerateFixedPOIs(GameMap map)
    {
        var fixedPoiDefinitions = new[]
        {
            new { Name = "Police Station", Description = "Law enforcement outpost.", X = 2, Y = 2 },
            new { Name = "Gas Station", Description = "Fuel and roadside stop.", X = 7, Y = 1 },
            new { Name = "Hospital", Description = "Medical complex.", X = 1, Y = 7 },
            new { Name = "Shopping Mall", Description = "Large indoor plaza.", X = 5, Y = 6 },
            new { Name = "Radio Tower", Description = "High-ground communications node.", X = 8, Y = 5 }
        };

        var points = new List<POI>();
        foreach (var definition in fixedPoiDefinitions)
        {
            var tile = map.GetTile(definition.X, definition.Y);
            tile.IsBlocked = false;

            var poi = new POI
            {
                Name = definition.Name,
                Description = definition.Description,
                X = definition.X,
                Y = definition.Y,
                IsDynamic = false
            };

            tile.POI = poi;
            tile.TileType = TileType.POI;
            points.Add(poi);
        }

        return points;
    }

    public List<InfilPoint> GenerateInfilPoints(GameMap map)
    {
        var points = new List<InfilPoint>();
        while (points.Count < 6)
        {
            var x = _random.Next(0, GameMap.GridSize);
            var y = _random.Next(0, GameMap.GridSize);

            if (map.Tiles[x, y].IsBlocked || map.Tiles[x, y].POI is not null)
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
            map.Tiles[x, y].TileType = TileType.Infil;
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
            map.Tiles[x, y].TileType = TileType.Exfil;
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
            tile.TileType = TileType.DynamicPOI;
            points.Add(poi);
        }

        return points;
    }
}
