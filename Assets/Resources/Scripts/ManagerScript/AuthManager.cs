using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthManager : MonoBehaviour
{
    
    #region ╫л╠шео
    private static AuthManager instance;
    public static AuthManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<AuthManager>();
            return instance;
        }
    }
    #endregion
}
