using System.Threading.Tasks;
using UnityEngine;

public class Column : MonoBehaviour
{
    [SerializeField] private float _shiftDownTweenDuration;
    [SerializeField] private Transform _tileContainer;
    public Tile[] Tiles { get; private set; }

    public void InitTiles(Tile tileTemplate, int startX, int count, DataType[] dataTypes)
    {
        Tiles = new Tile[count];
        for (var y = 0; y < Tiles.Length; y++)
        {
            Tiles[y] = Instantiate(tileTemplate, _tileContainer);
            Tiles[y].Init(startX, y, dataTypes[Random.Range(0, dataTypes.Length)]);
        }
    }

    public async Task ShiftDown(DataType emptyTile)
    {
        var tileSwapper = new TileSwapper();
        for (var i = Tiles.Length - 1; i >= 0; i--)
        {
            for (int j = i - 1; j >= 0; j--)
            {
                if (Tiles[i].DataType.Id==emptyTile.Id)
                {
                    await tileSwapper.SwapAsync(Tiles[i], Tiles[j], transform,_shiftDownTweenDuration,
                        Tiles[i].DataType != Tiles[j].DataType);
                }
            }
        }
    }
}
