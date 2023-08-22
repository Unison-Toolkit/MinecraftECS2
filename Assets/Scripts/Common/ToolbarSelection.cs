using UnityEngine;
using UnityEngine.UI;

public class ToolbarSelection : MonoBehaviour
{
    int blockNum = 0;
    //public static Transform[] BlockUI = new Transform[7];
    public static Transform[] BlockUI;
    public static Transform BlockUIChild;

    private void Start()
    {
        BlockUI = new Transform[10];
        for (int i = 0; i < 7; i++)
        {
            BlockUI[i] = this.transform.GetChild(i);
        }
    }

    private void Update()
    {
        blockNum = Unity.Physics.Extensions.PickaxeController.SelectedIndex - 1;
        //print("blockNum=" +blockNum);

        for (int i = 0; i < 7; i++)
        {
            BlockUIChild = BlockUI[i].transform.GetChild(0);

            if (i == blockNum)
            {

                BlockUIChild.GetComponent<RawImage>().enabled = true;
            }
            else
            {
                BlockUIChild.GetComponent<RawImage>().enabled = false;
            }
        }
    }
}