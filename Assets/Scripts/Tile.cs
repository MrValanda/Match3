using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Tile : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Image _backGround;
    [SerializeField] private Color _selectColor;
    [SerializeField] private Color _normalColor;
    public Transform IconTransform => _icon.transform;

    private Button _button;
    public int X { get; private set; }
    public int Y { get; private set; }

    public DataType DataType { get; private set; }
    public event Action<Tile> TileClick;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnButtonClick);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnButtonClick);
    }

    public void Init(int x, int y, DataType dataType)
    {
        X = x;
        Y = y;
        ChangeDataType(dataType);
    }

    public void ChangeDataType(DataType dataType)
    {
        if (dataType == null) return;

        DataType = dataType;
        _icon.sprite = DataType.Icon;
    }

    public void SwapIcon(Tile tile)
    {
        (_icon, tile._icon) = (tile._icon, _icon);

        DataType tmp = DataType;

        ChangeDataType(tile.DataType);
        tile.ChangeDataType(tmp);
    }

    public void Select()
    {
        _backGround.color = _selectColor;
    }

    public void Deselect()
    {
        _backGround.color = _normalColor;
    }

    private void OnButtonClick()
    {
        TileClick?.Invoke(this);
    }
}