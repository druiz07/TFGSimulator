using UnityEngine;

public class ExitGame : MonoBehaviour
{
    // M�todo que se ejecutar� cuando se presione el bot�n
    public void QuitGame()
    {
        // Salir del juego y terminar la ejecuci�n
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}