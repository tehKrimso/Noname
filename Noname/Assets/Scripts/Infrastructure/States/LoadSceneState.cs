﻿using System.Collections.Generic;
using CameraLogic;
using Data;
using Enemy;
using Hero;
using Infrastructure.Factory;
using Infrastructure.Services.PersistentProgress;
using Logic;
using UI;
using UnityEngine;

namespace Infrastructure.States
{
    public class LoadSceneState : IPayloadedState<string>
    {
        private const string InitialPointTag = "InitialPoint";
        private const string EnemySpawnerTag = "EnemySpawner";

        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly LoadingCurtain _curtain;
        private readonly IGameFactory _gameFactory;
        private readonly IPersistentProgressService _progressService;

        public LoadSceneState(GameStateMachine stateMachine, SceneLoader sceneLoader, LoadingCurtain curtain,
            IGameFactory gameFactory, IPersistentProgressService progressService)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _curtain = curtain;
            _gameFactory = gameFactory;
            _progressService = progressService;
        }

        public void Enter(string sceneName)
        {
            _curtain.Show();
            _gameFactory.Cleanup();
            _sceneLoader.Load(sceneName, OnLoaded);
        }

        public void Exit()
        {
            _curtain.Hide();
        }

        private void OnLoaded()
        {
            InitGameWorld();
            InitDropedLoot();
            InformProgressReaders();


            _stateMachine.Enter<GameLoopState>();
        }

        private void InformProgressReaders()
        {
            foreach (ISavedProgressReader progressReader in _gameFactory.ProgressReaders)
            {
                progressReader.LoadProgress(_progressService.Progress);
            }
        }

        private void InitGameWorld()
        {
            InitSpawners();

            GameObject hero = InitHero();

            //
            InitHud(hero);
            //


            CameraFollow(hero);
        }

        private void InitDropedLoot()
        {
            foreach (LootPieceData unpickedLoot in _progressService.Progress.WorldData.LootData
                         .UnpickedLoot)
            {
                var lootPiece = _gameFactory.CreateLoot();
                lootPiece.GetComponent<UniqueId>().Id = unpickedLoot.Id;
                lootPiece.transform.position = unpickedLoot.LootPosition.AsUnityVector();
                lootPiece.Initialize(unpickedLoot.Loot);
            }
        }

        private void InitSpawners()
        {
            foreach (GameObject spawnerObject in GameObject.FindGameObjectsWithTag(EnemySpawnerTag))
            {
                var spawner = spawnerObject.GetComponent<EnemySpawner>();
                _gameFactory.Register(spawner);
            }
        }

        private void InitHud(GameObject hero)
        {
            GameObject hud = _gameFactory.CreateHud();
            hud.GetComponentInChildren<ActorUI>().Construct(hero.GetComponent<HeroHealth>());
        }

        private GameObject InitHero()
        {
            return _gameFactory.CreateHero(GameObject.FindWithTag(InitialPointTag));
        }

        private void CameraFollow(GameObject hero)
        {
            Camera.main.GetComponent<CameraFollow>().Follow(hero);
        }
    }
}