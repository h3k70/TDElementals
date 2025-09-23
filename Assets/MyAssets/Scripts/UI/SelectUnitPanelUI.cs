using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SelectUnitPanelUI : MonoBehaviour
{
    [SerializeField] private CharacterCardUI _characterCardUIPref;

    private List<CharacterCardUI> _characterCards = new();
    private CharacterCardUI _selectedCharacterCard;

    public event Action<CharacterCardUI> SelectedCharacterCard;

    public void Init(List<Character> characters)
    {
        foreach (Character character in characters)
        {
            var card = Instantiate(_characterCardUIPref, transform);
            card.Init(character);
            _characterCards.Add(card);

            card.CharacterCardSelected += OnCharacterCardSelected;
        }
    }

    public void OnDestroy()
    {
        foreach (CharacterCardUI card in _characterCards)
            card.CharacterCardSelected -= OnCharacterCardSelected;
    }

    public void ResetAll()
    {
        foreach (CharacterCardUI card in _characterCards)
        {
            card.CharacterCardSelected -= OnCharacterCardSelected;
            Destroy(card.gameObject);
        }
    }

    private void OnCharacterCardSelected(CharacterCardUI card)
    {
        if (_selectedCharacterCard != null)
            _selectedCharacterCard.SetSelect(false);

        card.SetSelect(true);
        _selectedCharacterCard = card;

        SelectedCharacterCard?.Invoke(_selectedCharacterCard);
    }
}
