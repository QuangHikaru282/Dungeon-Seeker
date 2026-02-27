using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadBegin : MonoBehaviour
{
    public void Start()
    {
        SceneLoadData.nextSceneName = "Gameplay_lv1";
    }
}
