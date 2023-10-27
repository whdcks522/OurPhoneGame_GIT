using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SingleSelectManager : MonoBehaviour
{
    public void starFallSelected(int level)
    {
        switch (level) 
        {
            case 0:
                SceneManager.LoadScene("StarFall_0");
                break;
            case 1:
                SceneManager.LoadScene("StarFall_1");
                break;
            case 2:
                SceneManager.LoadScene("StarFall_0");
                break;
        }
    }
}
