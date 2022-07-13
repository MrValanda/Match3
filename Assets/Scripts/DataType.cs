using UnityEngine;

[CreateAssetMenu(menuName = "Match3/DataType")]
public class DataType : ScriptableObject
{
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public int Score { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
}