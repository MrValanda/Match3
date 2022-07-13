using System.Collections.Generic;

public class MatchFinder
{
    public Match FindBestMatch(Tile[,] board)
    {
        Match bestMatch = null;
        for (var x = 0; x < board.GetLength(0); x++)
        {
            for (int y = 0; y < board.GetLength(1); y++)
            {
                Tile origin = board[x, y];
                var verticalConnections = GetVerticalConnections(origin, board);
                var horizontalConnections = GetHorizontalConnections(origin, board);
                Match currentMatch = new Match(origin, verticalConnections, horizontalConnections);

                if (currentMatch.Tiles == null) continue;

                if (bestMatch == null)
                {
                    bestMatch = currentMatch;
                }
                else if (bestMatch.Score < currentMatch.Score)
                {
                    bestMatch = currentMatch;
                }
            }
        }

        return bestMatch;
    }

    private Tile[] GetVerticalConnections(Tile origin, Tile[,] board)
    {
        int height = board.GetLength(1);
        List<Tile> verticalTiles = CreateVerticalTiles(origin, board, height);

        return verticalTiles.ToArray();
    }

    private Tile[] GetHorizontalConnections(Tile origin, Tile[,] board)
    {
        int width = board.GetLength(0);
        List<Tile> horizontalTiles = CreateHorizontalTiles(origin, board, width);

        return horizontalTiles.ToArray();
    }

    private List<Tile> CreateHorizontalTiles(Tile origin, Tile[,] board, int width)
    {
        List<Tile> horizontalTiles = new List<Tile>();

        for (int x = origin.X + 1; x < width; x++)
        {
            var other = board[x, origin.Y];
            if (TryAddTile(horizontalTiles, origin, other) == false) break;
        }

        for (int x = origin.X - 1; x >= 0; x--)
        {
            var other = board[x, origin.Y];
            if (TryAddTile(horizontalTiles, origin, other) == false) break;
        }

        return horizontalTiles;
    }

    private List<Tile> CreateVerticalTiles(Tile origin, Tile[,] board, int height)
    {
        List<Tile> verticalTiles = new List<Tile>();

        for (int y = origin.Y + 1; y < height; y++)
        {
            var other = board[origin.X, y];
            if (TryAddTile(verticalTiles, origin, other) == false) break;
        }

        for (int y = origin.Y - 1; y >= 0; y--)
        {
            var other = board[origin.X, y];
            if (TryAddTile(verticalTiles, origin, other) == false) break;
        }

        return verticalTiles;
    }

    private bool TryAddTile(List<Tile> tiles, Tile origin, Tile other)
    {
        if (other.DataType.Id != origin.DataType.Id) return false;
        tiles.Add(other);
        return true;
    }
}