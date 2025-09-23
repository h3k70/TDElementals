using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PopText : MonoBehaviour
{
    [SerializeField] private float _showTime = 0.8f;
    [SerializeField] private TMP_Text _text;

    private Coroutine _showJob;

    public void Show(string text)
    {
        if (gameObject.activeInHierarchy == false)
            return;

        _text.text = text;

        if (_showJob != null)
            StopCoroutine(_showJob);

        _showJob = StartCoroutine(ShowJob());
    }

    private void Hide()
    {
        _text.text = "";
    }

    private IEnumerator ShowJob()
    {
        transform.DOShakePosition(_showTime, 0.5f);
        float time = Time.time + _showTime;

        while (true)
        {
            if (time < Time.time)
            {
                Hide();
            }
            yield return null;
        }
    }
}
