using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Tile[] upNeighbours;
    public Tile[] rightNeighbours;
    public Tile[] downNeighbours;
    public Tile[] leftNeighbours;

    private void Awake()
    {
        transform.localScale = Vector3.zero;

        transform.DOScale(Vector3.one, 1f)
            .SetEase(Ease.OutElastic);
    }
}
