using System.Collections.Generic;
using System.Linq;

public class Match
{
    public readonly List<Tile> Tiles;
    public readonly float Score;

    public Match(Tile origin,Tile[] verticalConnections,Tile[] horizontalConnections,int minMatchLength=2)
    {
        Tiles = new List<Tile>();
        bool need = false;
        Tiles.Add(origin);

        need |= TryAddRange(horizontalConnections, minMatchLength);
        need |= TryAddRange(verticalConnections, minMatchLength);
        if (need==false)
        {
            Tiles = null;
        }
        
        if (Tiles != null)
        {
            Score = Tiles.Sum(x => x.DataType.Score);
        }
    }

    private bool TryAddRange(Tile[] tile,int minMatchLength)
    {
        if (tile.Length >= minMatchLength)
        {
            Tiles?.AddRange(tile);
            return true;
        }

        return false;
    }
}
