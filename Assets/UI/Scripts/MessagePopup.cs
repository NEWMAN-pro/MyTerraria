using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessagePopup : MonoBehaviour
{
    public Text messageText;
    public float delay = 2f;

    private void OnEnable()
    {
        StartCoroutine(HideMessage());
    }

    public void ShowMessage(string message)
    {
        messageText.text = message;
        gameObject.SetActive(true);
    }

    public IEnumerator HideMessage()
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
