using System;
using Data;
using Enemy;
using Infrastructure.Factory;
using Infrastructure.Services;
using Infrastructure.Services.PersistentProgress;
using StaticData;
using UnityEngine;

namespace Logic
{
    public class EnemySpawner : MonoBehaviour, ISavedProgress
    {
        public MonsterTypeId MonsterTypeId;
        private string _id;
        public bool Slain => _slain;
        [SerializeField]private bool _slain;
        private IGameFactory _factory;
        private EnemyDeath _enemyDeath;

        private void Awake()
        {
            _id = GetComponent<UniqueId>().Id;
            _factory = AllServices.Containter.Single<IGameFactory>();
        }

        public void LoadProgress(PlayerProgress progress)
        {
            if (progress.KillData.ClearedSpawners.Contains(_id))
                _slain = true;
            else
            {
                Spawn();
            }
        }

        private void Spawn()
        {
            GameObject monster = _factory.CreateMonster(MonsterTypeId, transform);
            _enemyDeath = monster.GetComponent<EnemyDeath>();
            _enemyDeath.Happened += Slay;
        }

        private void Slay()
        {
            if(_enemyDeath != null)
                _enemyDeath.Happened -= Slay;
            _slain = true;
        }

        public void UpdateProgress(PlayerProgress progress)
        {
            if(_slain)
                progress.KillData.ClearedSpawners.Add(_id);
        }
    }
}