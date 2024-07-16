using UnityEngine;

public class ExitGame : MonoBehaviour
{
    // Método que se ejecutará cuando se presione el botón
    public void QuitGame()
    {
        // Salir del juego y terminar la ejecución
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}