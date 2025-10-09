using System.Collections;
using NUnit.Framework;
using UnityEngine;

public class Skelton : Monsters
{
    Monsters skelton = new();

    protected override void Start()
    {
        skelton.Name = "Skelton";
        skelton.EnemyHP = 10;
        skelton.DropCoin = 2;
        skelton.MyStartPos = this.transform.position;
        skelton.MyNowPos = this.GetComponent<Transform>().position;
        skelton.FindLength = 10.0f;
    }

    protected override void Update()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        string Tag = "";
        if (collision != null)
        {
            Tag = collision.gameObject.tag;
            if (Tag.Contains("Player"))
            {
                skelton.EnemyDamaged(collision.gameObject.layer);
            }
        }
    }
}
