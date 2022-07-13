using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

public class TileSwapper
{
    public async Task SwapAsync(Tile first, Tile second, Transform swappingOverlay, float swapTweenDuration,
        bool anim = true)
    {
        var firstIconTransform = first.IconTransform;
        var secondIconTransform = second.IconTransform;

        firstIconTransform.SetParent(swappingOverlay);
        secondIconTransform.SetParent(swappingOverlay);
        if (anim)
        {
            var sequence = DOTween.Sequence();

            sequence.Join(firstIconTransform.DOMove(secondIconTransform.position, swapTweenDuration)
                    .SetEase(Ease.OutBack))
                .Join(secondIconTransform.DOMove(firstIconTransform.position, swapTweenDuration).SetEase(Ease.OutBack));

            await sequence.Play()
                .AsyncWaitForCompletion();
        }
        else
        {
            (firstIconTransform.position, secondIconTransform.position) =
                (secondIconTransform.position, firstIconTransform.position);
        }

        firstIconTransform.SetParent(second.transform);
        secondIconTransform.SetParent(first.transform);
        first.SwapIcon(second);
    }
}