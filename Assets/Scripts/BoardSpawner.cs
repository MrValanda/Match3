using UnityEngine;

public class BoardSpawner : MonoBehaviour
{
    [field: SerializeField] public int Width { get; private set; }
    [field: SerializeField] public int Height { get; private set; }

    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Column _columnPrefab;
    [SerializeField] private DataType[] _dataTypes;

    public Column[] CreateBoard()
    {
        Column[] columns = new Column[Width];

        for (int x = 0; x < Width; x++)
        {
            var column = Instantiate(_columnPrefab, transform);
            column.InitTiles(_tilePrefab, x, Height, _dataTypes);
            columns[x] = column;
        }

        return columns;
    }

    public DataType GetRandomDataType()
    {
        return _dataTypes[Random.Range(0, _dataTypes.Length)];
    }
}