using System;
using System.Collections.Generic;
using Mirror;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SkillManager : NetworkBehaviour
{
    [SerializeField] private Character _character;
    [SerializeField] private List<Skill> _allSkills;
    [SerializeField] private List<Skill> _currentSkills;
    [SerializeField] private Skill _autoAtack;

    public List<Skill> AllSkills => _allSkills;
    public List<Skill> Skills => _currentSkills;
    public Skill AutoAtack { get => _autoAtack; }

    public event Action<Skill> SkillAdded;
    public event Action<Skill> SkillRemoved;

    public void Init()
    {
        _autoAtack.Init(_character);

        foreach (var skill in _allSkills)
        {
            skill.Init(_character);
        }
    }
}
