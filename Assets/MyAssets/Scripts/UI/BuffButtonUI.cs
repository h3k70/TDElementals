using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffButtonUI : MonoBehaviour
{
    [SerializeField] private Buffs _buffType;
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _textLvl;
    [SerializeField] private TMP_Text _textBuffValue;
    [SerializeField] private TMP_Text _textDescription;
    [SerializeField] private TMP_Text _textCost;
    [SerializeField] private Image _cooldownImage;

    private Base _base1;
    private int _lvl = 0;
    private int _clickCount = 0;
    private int _maxLvl = 20;
    private float _baseBuffValue = .05f;
    private float _buffValue = .05f;
    private float _baseDuration = 20f;
    private float _duration = 20f;
    private float _cd = 6f;
    private float _addCd = 1f;
    private float _addDuration = 2f;
    private float _cost = 1f;
    private float _addCost = 1f;
    private bool _isReady = true;

    public void Init(Base base1)
    {
        _base1 = base1;
        _button.onClick.AddListener(OnClick);
        TryLvlUp();
    }

    private void OnClick()
    {
        if (_isReady && _base1.SelectedCardUnit.TotalExp >= _cost)
        {
            _base1.SelectedCardUnit.CmdPayExpCost(_cost);
            _base1.CmdBuffUnitInRadius(_buffType, _buffValue, _duration);
            _isReady = false;
            StartCoroutine(CooldownJob());
            TryLvlUp();
            _cooldownImage.gameObject.SetActive(true);
        }
    }

    private void TryLvlUp()
    {
        if (_clickCount + 1 < _lvl)
        {
            _clickCount++;
            return;
        }
        _clickCount = 0;

        if (_lvl + 1 <= _maxLvl)
        {
            _lvl++;
            _textLvl.text = _lvl.ToString();

            _cost += _addCost;
            _textCost.text = _cost.ToString();
            _cd += _addCd;
        }

        _buffValue = 1 + _baseBuffValue * _lvl;
        _textBuffValue.text = _buffValue.ToString();

        _duration = _baseDuration + (_lvl - 1) * _addDuration;
    }

    private IEnumerator CooldownJob()
    {
        yield return new WaitForSeconds(_cd);
        _cooldownImage.gameObject.SetActive(false);
        _isReady = true;
    }
}
