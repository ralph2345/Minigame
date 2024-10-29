using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PuzzleButton : MonoBehaviour
{
    [SerializeField]
    private Transform puzzleField;

    [SerializeField]
    private GameObject btns; // buttons on card

    void Awake() {

        // This is the number of card in matching game
        for(int i = 1; i <= 11; i++) {
            GameObject button = Instantiate(btns);
            button.name = "" + i.ToString();
            button.transform.SetParent(puzzleField, false);

            button.tag = "PuzzleButton";
            button.transform.localScale = Vector3.zero;

            button.transform.DOScale(Vector3.one, 0.5f).SetDelay(i * 0.1f);  

        }
    }
}