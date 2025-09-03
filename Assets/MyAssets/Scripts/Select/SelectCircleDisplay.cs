using System;
using UnityEngine.Rendering.Universal;

public class SelectCircleDisplay : IDisposable
{
    private DecalProjector _decalProjector;
    private ISelectable _selectable;

    public SelectCircleDisplay(DecalProjector decalProjector, ISelectable selectable)
    {
        _decalProjector = decalProjector;
        _selectable = selectable;

        _selectable.Selected += OnSelected;
        _selectable.Deselected += OnDeselected;
    }

    public void Dispose()
    {
        _selectable.Selected -= OnSelected;
        _selectable.Deselected -= OnDeselected;
    }

    private void OnDeselected(ISelectable selectable)
    {
        _decalProjector.gameObject.SetActive(false);
    }

    private void OnSelected(ISelectable selectable)
    {
        _decalProjector.gameObject.SetActive(true);
    }
}
