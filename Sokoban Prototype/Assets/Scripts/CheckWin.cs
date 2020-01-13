using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckWin : MonoBehaviour
{
    bool checking;
    List<GameObject> buttons = new List<GameObject>();
    public void BeginChecking()
    {
        if (!checking)
        {
            checking = true;
            buttons = GetComponent<PlaceGoals>().GetButtons();
            StartCoroutine(Check());
        }
    }
    IEnumerator Check()
    {
        bool won = false;
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].transform.parent.childCount == 2 && 
                buttons[i].transform.parent.GetChild(1).CompareTag("Box"))
            {
                won = true;
            }
            else
            {
                won = false;
                break;
            }
            yield return null;
        }

        if (won)
        {
            checking = false;
            Debug.Log("GAME WON");
            yield return new WaitForSeconds(0.5f);
            GetComponent<GenerateGrid>().Restart();
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(Check());
        }
    }
}
