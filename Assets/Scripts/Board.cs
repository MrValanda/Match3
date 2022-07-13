using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using System.Threading.Tasks;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    [SerializeField] private BoardSpawner _boardSpawner;
    [SerializeField] private float _swapTweenDuration;
    [SerializeField] private Transform _swappingOverlay;
    [SerializeField] private DataType _emptyTile;

    public UnityEvent Match;

    private Column[] _columns;
    private Tile[,] _board;

    private readonly List<Tile> _selectionTiles = new List<Tile>();

    private bool _isPaused;

    private async void Awake()
    {
        _columns = _boardSpawner.CreateBoard();
        _board = new Tile[_boardSpawner.Width, _boardSpawner.Height];
        for (var x = 0; x < _board.GetLength(0); x++)
        {
            for (int y = 0; y < _board.GetLength(1); y++)
            {
                _board[x, y] = GetTile(x, y);
            }
        }

        await TryMatchAsync();
    }

    private void OnEnable()
    {
        if (_columns == null) return;

        foreach (var column in _columns)
        {
            foreach (var columnTile in column.Tiles)
            {
                columnTile.TileClick += Select;
            }
        }
    }

    private async Task ShiftDown()
    {
        List<Task> tasks = new List<Task>();
        foreach (var column in _columns)
        {
            tasks.Add(column.ShiftDown(_emptyTile));
        }

        foreach (var task in tasks)
        {
            await task;
        }
    }

    private void OnDisable()
    {
        if (_columns == null) return;
        foreach (var column in _columns)
        {
            foreach (var columnTile in column.Tiles)
            {
                columnTile.TileClick -= Select;
            }
        }
    }

    private async void Select(Tile tile)
    {
        if (_isPaused) return;

        if (_selectionTiles.Contains(tile) == false)
        {
            if (_selectionTiles.Count == 1)
            {
                if (TheyNeighbors(tile, _selectionTiles[0]))
                    _selectionTiles.Add(tile);
            }
            else
            {
                tile.Select();
                _selectionTiles.Add(tile);
            }
        }
        else
        {
            tile.Deselect();
            _selectionTiles.Remove(tile);
        }

        if (_selectionTiles.Count < 2) return;

        await SwapSelectedTiles();

        _selectionTiles.Clear();
    }

    private async Task SwapSelectedTiles()
    {
        TileSwapper tileSwapper = new TileSwapper();
        _isPaused = true;

        _selectionTiles[0].Deselect();

        await tileSwapper.SwapAsync(_selectionTiles[0], _selectionTiles[1], _swappingOverlay, _swapTweenDuration);

        if (!await TryMatchAsync())
            await tileSwapper.SwapAsync(_selectionTiles[0], _selectionTiles[1], _swappingOverlay, _swapTweenDuration);

        _isPaused = false;
    }

    private bool TheyNeighbors(Tile first, Tile second)
    {
        return Mathf.Abs(first.X - second.X) == 0 && Mathf.Abs(first.Y - second.Y) == 1
               || Mathf.Abs(first.X - second.X) == 1 && Mathf.Abs(first.Y - second.Y) == 0;
    }

    private async Task<bool> TryMatchAsync()
    {
        var didMatch = false;
        MatchFinder matchFinder = new MatchFinder();

        var match = matchFinder.FindBestMatch(_board);
        while (match != null)
        {
            didMatch = true;

            await RemoveTiles(match.Tiles);

            match = matchFinder.FindBestMatch(_board);
        }

        return didMatch;
    }

    private async Task RemoveTiles(IEnumerable<Tile> tiles)
    {
        _isPaused = true;
        var deflateSequence = DOTween.Sequence();
        foreach (var matchTile in tiles)
        {
            deflateSequence.Join(matchTile.IconTransform.DOScale(Vector3.zero, _swapTweenDuration)
                .SetEase(Ease.InBack).OnComplete(() => matchTile.ChangeDataType(_emptyTile)));
        }

        await deflateSequence.Play().AsyncWaitForCompletion();
        Match?.Invoke();
        await ShiftDown();
        await RespawnTiles();
        _isPaused = false;
    }

    private async Task RespawnTiles()
    {
        var inflateSequence = DOTween.Sequence();
        for (var x = 0; x < _board.GetLength(0); x++)
        {
            for (int y = 0; y < _board.GetLength(1); y++)
            {
                if (_board[x, y].DataType.Id == _emptyTile.Id)
                {
                    _board[x, y].ChangeDataType(_boardSpawner.GetRandomDataType());
                    inflateSequence.Join(_board[x, y].IconTransform.DOScale(Vector3.one, _swapTweenDuration)
                        .SetEase(Ease.OutBack));
                }
            }
        }

        await inflateSequence.Play().AsyncWaitForCompletion();
    }

    private Tile GetTile(int x, int y)
    {
        x = Mathf.Clamp(x, 0, _boardSpawner.Width - 1);
        y = Mathf.Clamp(y, 0, _boardSpawner.Height - 1);

        return _columns[x].Tiles[y];
    }
}