using System.Collections;
using UnityEngine;
using GrazerCore.GameElements.EnemyGroup;
using GrazerCore.Tool;
using GrazerCore.Factories;
using GrazerCore.GameFlow.Events;

namespace SkateHero.GameFlow.Events
{
    [CreateAssetMenu(fileName = "GameEventSpawnEnemy", menuName = "SkateGuy/GameEvent/GameEventSpawnEnemy")]
    public class GameEventSpawnEnemy : GameEvent
    {
        [SerializeField]
        private EnemyTeamData[] m_EnemyTeamDatas = null;
        private int waveIndex = 0;
        private int enemyTeamCount = -1;
        private EnemyTeam currentEnemyTeam = null;
        private EnemyTeam lastEnemyTeam = null;
        private Coroutine waveCoroutine = null;

        public override void Launch()
        {
            waveIndex = 0;
            enemyTeamCount = m_EnemyTeamDatas.Length;
            CallWave();
        }

        public override void Close()
        {
            CloseCallWave();
            if (lastEnemyTeam != null)
            {
                lastEnemyTeam.Close();
            }
            if (currentEnemyTeam != null)
            {
                currentEnemyTeam.Close();
            }
        }

        private void CallWave()
        {
            var teamCount = m_EnemyTeamDatas.Length;
            if (waveIndex >= teamCount)
            {
                return;
            }
            var teamData = m_EnemyTeamDatas[waveIndex];
            waveCoroutine = CoroutineAgent.StartEntrustCoroutine(WaveCalling(teamData));
            waveIndex++;
        }

        private void CloseCallWave()
        {
            if (waveCoroutine != null)
            {
                CoroutineAgent.StopEntrustCoroutine(waveCoroutine);
            }
        }

        private void ClearCheck(EnemyTeam clearedTeam)
        {
            enemyTeamCount--;
            if (enemyTeamCount <= 0)
            {
                CloseCallWave();
                //  All clear
                GameEventDone();
            }
            else if (lastEnemyTeam == clearedTeam && !currentEnemyTeam.IsWorking)
            {
                CloseCallWave();
                currentEnemyTeam.SummonMember();
                CallWave();
            }
        }

        IEnumerator WaveCalling(EnemyTeamData teamData)
        {
            var team = EnemyTeamFactory.GetEnemyTeam(teamData.EnemyTeam);
            team.OnAllMemberGone.AddListener(ClearCheck);
            lastEnemyTeam = currentEnemyTeam;
            currentEnemyTeam = team;
            yield return new WaitForSeconds(teamData.WaveWaitTime);
            currentEnemyTeam.SummonMember();
            CallWave();
        }
    }
}
