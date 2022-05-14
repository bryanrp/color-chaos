using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardPurpleBullet : Bullet
{
    private WizardPurple _wizardPurple;
    private int _type;

    private void Start()
    {
        if (_gameManager == null) _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public void SetStats(WizardPurple wizardPurple, int type)
    {
        _wizardPurple = wizardPurple;
        _type = type;
    }

    private new void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Dead();
            collision.GetComponent<Player>().ChangeHealth(-1);
        }
        else
        {
            Bullet otherBullet = collision.GetComponent<Bullet>();
            if (otherBullet != null && otherBullet.IsFromPlayer())
            {
                Dead();
                otherBullet.Dead();
            }
        }
    }

    private new void Dead()
    {
        base.Dead();

    }
}
