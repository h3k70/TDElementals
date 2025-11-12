using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillIconUI : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Image _cooldown;

    public Skill Skill { get; private set; }

    public void Init(Skill skill)
    {
        Skill = skill;
        _image.sprite = Skill.Icon;
        Skill.CooldownStarted += OnCooldownStarted;
    }

    private void OnDestroy()
    {
        Skill.CooldownStarted -= OnCooldownStarted;
    }

    private void OnCooldownStarted(float time, Skill skill)
    {
        StartCoroutine(CooldownJob(time));
    }

    private IEnumerator CooldownJob(float time)
    {
        float maxTime = time;

        while (time > 0)
        {
            _cooldown.fillAmount = time / maxTime;
            time -= Time.deltaTime;
            yield return null;
        }
    }
}
