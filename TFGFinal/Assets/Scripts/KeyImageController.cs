using UnityEngine;
using UnityEngine.UI;

public class KeyImageController : MonoBehaviour
{
    [SerializeField] private Image letterT;
    [SerializeField] private Image letterTPressed;
    [SerializeField] private Image letterP;
    [SerializeField] private Image letterPPressed;
    [SerializeField] private Image space;
    [SerializeField] private Image spacePressed;
    [SerializeField] private Image arrowLeft;
    [SerializeField] private Image arrowLeftPressed;
    [SerializeField] private Image arrowRight;
    [SerializeField] private Image arrowRightPressed;

    private void Update()
    {

        // Detecta las teclas pulsadas y cambia las imágenes correspondientes
        if (Input.GetKeyDown(KeyCode.T))
        {
            letterT.gameObject.SetActive(false);
            letterTPressed.gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            letterP.gameObject.SetActive(false);
            letterPPressed.gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            space.gameObject.SetActive(false);
            spacePressed.gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            arrowLeft.gameObject.SetActive(false);
            arrowLeftPressed.gameObject.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            arrowRight.gameObject.SetActive(false);
            arrowRightPressed.gameObject.SetActive(true);
        }
        // Detecta las teclas liberadas y cambia las imágenes correspondientes
        if (Input.GetKeyUp(KeyCode.T))
        {
            letterT.gameObject.SetActive(true);
            letterTPressed.gameObject.SetActive(false);
        }

        if (Input.GetKeyUp(KeyCode.P))
        {
            letterP.gameObject.SetActive(true);
            letterPPressed.gameObject.SetActive(false);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            space.gameObject.SetActive(true);
            spacePressed.gameObject.SetActive(false);
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            arrowLeft.gameObject.SetActive(true);
            arrowLeftPressed.gameObject.SetActive(false);
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            arrowRight.gameObject.SetActive(true);
            arrowRightPressed.gameObject.SetActive(false);
        }
    }
}
