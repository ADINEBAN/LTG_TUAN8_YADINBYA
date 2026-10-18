using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Helper script attached in _Scene_1 to return back to _Scene_3_2 for continuous testing.
/// </summary>
public class SceneReturnHelper : MonoBehaviour
{
    public void ReturnToPractice()
    {
        SceneManager.LoadScene("_Scene_3_2");
    }
}
