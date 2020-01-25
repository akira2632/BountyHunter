using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterSystem_V4.Skill;
using CharacterSystem_V4;

public class SkillEffectTester : MonoBehaviour
{
    [Header("TestSetting")]
    [Tooltip("要測試的效果")]
    public SkillEffector effector;
    public GameObject HitFrom, HitAt;
    [Space(10)]
    [Min(0)]
    public int test_Damage;
    public bool test_ShowDamage;

    [ContextMenu("PlayEffect")]
    private void PlayEffect()
    {
        effector.PlayEffect(new DamageData()
        {
            Damage = test_Damage,
            HitAt = HitAt.transform.position,
            HitFrom = HitFrom.transform.position
        }, test_ShowDamage);
    }
}
