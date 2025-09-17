using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Data;

public class ShowInstructions : UdonSharpBehaviour
{

    [SerializeField] private GameObject[] enableTargets;
    [SerializeField] private Scrollbar scrollbar;
    private DataList relation = new DataList()
        {
            0f,
            0.23f,
            0.57f,
            1f
        };
    private int Index;

    private void Start()
    {
        scrollbar.value = 0;
        Button_Show_Pressed_Off();
    }


    //Called in Event
    public void Button_Show_Pressed_On()
    {
        Debug.Log("llamado");
        for (int i = 0; i < enableTargets.Length; i++)
        {
            enableTargets[i].SetActive(true);
            Debug.Log("xd");
        }
    }

    //Called in Event
    public void Button_Show_Pressed_Off()
    {
        Debug.Log("desllamando");
        for (int i = 0; i < enableTargets.Length; i++)
        {
            enableTargets[i].SetActive(false);
        }
    }

    public void Button_Prev_Pressed()
    {
        Index = Mathf.Clamp(Index - 1, 0, scrollbar.numberOfSteps-1);
        ChangeSlider();
    }

    public void Button_Next_Pressed()
    {
        Index = Mathf.Clamp(Index + 1, 0, scrollbar.numberOfSteps - 1);
        ChangeSlider();
    }

    private void ChangeSlider()
    {
        scrollbar.value = relation[Index].Float;
    }

}
