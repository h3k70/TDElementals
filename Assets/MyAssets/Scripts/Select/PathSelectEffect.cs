using UnityEngine;

public class PathSelectEffect : MonoBehaviour
{
    [SerializeField] private Path _path;
    [SerializeField] private float _speed;
    [SerializeField] private ParticleSystem _particleSystem;

    private bool _isWork;
    private PathMover _mover;

    private void Start()
    {
        gameObject.SetActive(false);
        _mover = new(transform, _speed, _path);
        _path.Selected += OnSelected;
        _mover.ReachedEndPoint += OnReachedEndPoint;
    }

    private void Update()
    {
        if (_isWork)
            _mover.Update();
    }

    private void OnDestroy()
    {
        _path.Selected -= OnSelected;
        _mover.ReachedEndPoint -= OnReachedEndPoint;
    }

    private void OnReachedEndPoint()
    {
        _mover.Restart();
    }

    private void OnSelected(bool value)
    {
        if (value)
        {
            _mover.Restart();
            _isWork = true;
            _particleSystem.gameObject.SetActive(true);
        }
        else
        {
            _isWork = false;
            _particleSystem.gameObject.SetActive(false);
        }
    }
}
